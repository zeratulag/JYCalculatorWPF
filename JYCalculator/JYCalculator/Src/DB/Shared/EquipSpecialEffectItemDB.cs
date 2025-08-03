using System.Collections.Generic;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using System.Collections.Immutable;
using System.Data.Common;
using System.Linq;
using JX3CalculatorShared.Globals;
using JX3PZ.Data;
using JX3PZ.Src;
using JYCalculator.Data;

namespace JYCalculator.DB
{
    public class EquipSpecialEffectItemDB : IDB<string, EquipSpecialEffectItem>
    {
        public readonly ImmutableDictionary<string, EquipSpecialEffectItem> Data;

        public readonly ImmutableDictionary<EquipSubTypeEnum, ImmutableArray<EquipSpecialEffectItem>> EquipSubTypeDict;
        public ImmutableArray<int> ValidPositions { get; private set; } //记录了这些部位有特效
        public ImmutableArray<EquipSpecialEffectItem> Hat { get; private set; }
        public ImmutableArray<EquipSpecialEffectItem> Belt { get; private set; }
        public ImmutableArray<EquipSpecialEffectItem> Wrist { get; private set; }
        public ImmutableArray<EquipSpecialEffectItem> Bottoms { get; private set; }
        public ImmutableArray<EquipSpecialEffectItem> Shoes { get; private set; }
        public ImmutableArray<EquipSpecialEffectItem> Necklace { get; private set; }
        public ImmutableArray<EquipSpecialEffectItem> Pendant { get; private set; }
        public ImmutableArray<EquipSpecialEffectItem> Ring { get; private set; }
        public ImmutableArray<EquipSpecialEffectItem> Secondary_Weapon { get; private set; }

        public EquipSpecialEffectItem Get(string EID)
        {
            EquipSpecialEffectItem res = null;
            if (EID != null)
            {
                Data.TryGetValue(EID, out res);
            }
            return res;
        }

        public EquipSpecialEffectItemDB(DataLoader dataLoader) : this(dataLoader.EquipSpecialEffectItems)
        {
        }

        public EquipSpecialEffectItemDB() : this(StaticXFData.Data.EquipSpecialEffectItems)
        {
        }

        public EquipSpecialEffectItemDB(IEnumerable<EquipSpecialEffectItem> items)
        {
            Data = items.ToImmutableDictionary(_ => _.EID, _ => _);
            EquipSubTypeDict = Data.Values.GroupBy(e => e.SubType).ToImmutableDictionary(
                g => g.Key,
                g => g.OrderBy(e => e.Level).ThenBy(e => e.SpecialEffectType).ThenBy(e => e.EquipSpecialEffectEventID)
                    .ToImmutableArray());
            DispatchBySubType();
            FindValidPositions();
        }

        public void DispatchBySubType()
        {
            Hat = EquipSubTypeDict[EquipSubTypeEnum.HAT];
            Belt = EquipSubTypeDict[EquipSubTypeEnum.BELT];
            Wrist = EquipSubTypeDict[EquipSubTypeEnum.WRIST];
            Bottoms = EquipSubTypeDict[EquipSubTypeEnum.BOTTOMS];
            Shoes = EquipSubTypeDict[EquipSubTypeEnum.SHOES];
            Necklace = EquipSubTypeDict[EquipSubTypeEnum.NECKLACE];
            Pendant = EquipSubTypeDict[EquipSubTypeEnum.PENDANT];
            Ring = EquipSubTypeDict[EquipSubTypeEnum.RING];
            Secondary_Weapon = EquipSubTypeDict[EquipSubTypeEnum.SECONDARY_WEAPON];
        }

        public void FindValidPositions()
        {
            var validPositions = new List<int>(12);
            for (int i = 0; i < 12; i++)
            {
                var subType = EquipMapLib.PositionToSubType(i);
                var equipSubType = (EquipSubTypeEnum) subType;
                if (EquipSubTypeDict.ContainsKey(equipSubType))
                {
                    validPositions.Add(i);
                }
            }

            validPositions.Sort();
            ValidPositions = validPositions.ToImmutableArray();
        }

        public IEnumerable<EquipSpecialEffectItem> GetItemsByPosition(int position)
        {
            var subType = EquipMapLib.PositionToSubTypeEnum(position);
            return EquipSubTypeDict[subType];
        }

        public void AttachEquipSpecialEffectEntry(EquipSpecialEffectEntryDB db)
        {
            foreach (var v in Data.Values)
            {
                v.AttachEquipSpecialEffectEntry(db.Get(v.Name));
            }
        }
    }
}