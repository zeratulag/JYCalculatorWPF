using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3PZ.Class;
using JX3PZ.Data;
using JX3PZ.Globals;
using JX3PZ.Src;
using JYCalculator.Data;
using JYCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JX3PZ.Models
{
    public class PzBuildToInitInputModel
    {
        public JBPZEquipSnapshotCollection Collection { get; }
        public string Title { get; }
        public PzPlanModel Plan { get; private set; }
        public InitInputSav Result { get; private set; }

        public PzBuildToInitInputModel(JBPZEquipSnapshotCollection collection, string title = null)
        {
            Collection = collection;
            Title = title;
        }

        public InitInputSav Calc()
        {
            var snapshots = CreateEquipSnapShotModels();
            Plan = new PzPlanModel(snapshots);
            Plan.Calc();
            Plan.CalcSet();
            Plan.GetTotalEntryCollection();
            Plan.GetCharacterPanel();
            Plan.XFPanel.Name = Title;

            var adapter = new PzToXinFaInputAdapter(Plan);
            adapter.Calc();
            Result = adapter.InputSav;
            return Result;
        }

        private EquipSnapShotModel[] CreateEquipSnapShotModels()
        {
            var dict = GetDict();
            var result = new EquipSnapShotModel[PzConst.POSITIONS];

            for (int position = 0; position < PzConst.POSITIONS; position++)
            {
                var slot = ((EquipSlotEnum)position).ToString();
                var snapshot = dict.ContainsKey(slot) ? dict[slot] : null;
                result[position] = CreateEquipSnapShotModel(position, snapshot);
            }

            return result;
        }

        private IDictionary<string, JBPZEquipSnapshot> GetDict()
        {
            if (Collection.Dict == null)
            {
                Collection.GetDict();
            }

            return Collection.Dict;
        }

        private static EquipSnapShotModel CreateEquipSnapShotModel(int position, JBPZEquipSnapshot snapshot)
        {
            var equip = GetEquip(snapshot?.id);
            if (equip == null)
            {
                return new EquipSnapShotModel(position);
            }

            var enhance = GetEnhance(equip, snapshot?.enhance);
            var bigFm = GetBigFM(equip, snapshot?.enchant);
            var diamonds = GetDiamonds(equip, snapshot?.embedding);
            var stone = GetStone(snapshot?.stone);
            var strength = snapshot?.strength ?? 0;

            var model = new EquipSnapShotModel(position);
            model.Update(equip, strength, diamonds, stone, enhance, bigFm);
            return model;
        }

        private static Equip GetEquip(string eid)
        {
            if (string.IsNullOrWhiteSpace(eid))
            {
                return null;
            }

            StaticPzData.Data.Equip.TryGetValue(eid, out var equip);
            return equip;
        }

        private static Enhance GetEnhance(Equip equip, int? enhanceId)
        {
            if (equip == null || enhanceId == null || enhanceId <= 0)
            {
                return null;
            }

            if (!StaticPzData.Data.Enhance.TryGetValue(enhanceId.Value, out var enhance))
            {
                return null;
            }

            return equip.FitEnhance(enhance) ? enhance : null;
        }

        private static BigFM GetBigFM(Equip equip, int? enchantId)
        {
            if (equip == null || enchantId == null || enchantId <= 0)
            {
                return null;
            }

            var bigFm = StaticXFData.DB.BigFM.TypeData
                .Values
                .SelectMany(items => items)
                .FirstOrDefault(item => item.ID == enchantId.Value);
            if (bigFm == null)
            {
                return null;
            }

            return equip.FitBigFM(bigFm) ? bigFm : null;
        }

        private static DiamondLevelItem[] GetDiamonds(Equip equip, IEnumerable<int> levels)
        {
            if (equip == null || !equip.HasDiamond || levels == null)
            {
                return null;
            }

            var diamondLevels = levels.ToArray();
            if (diamondLevels.Length == 0)
            {
                return null;
            }

            var count = Math.Min(equip.Attributes.NDiamond, diamondLevels.Length);
            if (count <= 0)
            {
                return null;
            }

            var normalizedLevels = diamondLevels
                .Take(count)
                .Select(level => Math.Min(Math.Max(0, level), PzConst.MAX_DIAMOND_LEVEL))
                .ToArray();
            return equip.Attributes.GetDiamondLevelItems(normalizedLevels);
        }

        private static Stone GetStone(int? stoneId)
        {
            if (stoneId == null || stoneId <= 0)
            {
                return null;
            }

            StaticPzData.Data.Stone.TryGetValue(stoneId.Value, out var stone);
            return stone;
        }
    }
}
