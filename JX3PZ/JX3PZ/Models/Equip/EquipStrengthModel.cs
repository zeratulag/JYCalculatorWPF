using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Windows.Documents;
using JX3CalculatorShared.Common;
using JX3PZ.Class;
using JX3PZ.Globals;
using JX3PZ.ViewModels;

namespace JX3PZ.Models
{
    public class EquipStrengthModel : IModel
    {
        // 装备精炼模型
        public readonly Equip CEquip;
        public readonly int Strength = 0; // 精炼等级
        public readonly int RealStrength = 0; // 有效强化等级
        public readonly bool HasEquip = false;

        public ImmutableArray<AttributeStrengthEntry> BasicMagicStrengthEntry { get; private set; } =
            ImmutableArray<AttributeStrengthEntry>.Empty; // 强化后的基础魔法属性（白字）

        public ImmutableArray<AttributeStrengthEntry> ExtraMagicStrengthEntry { get; private set; } =
            ImmutableArray<AttributeStrengthEntry>.Empty; // 强化后的额外魔法属性（绿字）

        public int StrengthLevel { get; private set; } = 0; // 精炼带来的品级提升
        public int StrengthScore { get; private set; } = 0; // 精炼带来的装分提升
        public bool ModelHasCalculated { get; private set; } = false; // 是否已经完成属性计算
        public List<AttributeEntry> AllAttributeEntry { get; private set; } // 所有属性

        public EquipStrengthModel(Equip cEquip, int strength = 0)
        {
            CEquip = cEquip;
            Strength = strength;
            HasEquip = CEquip != null;
            if (HasEquip)
            {
                RealStrength = CEquip.CalcRealStrength(strength);
            }
            else
            {
                RealStrength = 0;
            }
        }

        public void Calc()
        {
            GetStrengthEntry();
            GetStrengthLevelScore();
            GetAllAttributeEntries();
            ModelHasCalculated = true;
        }

        public void GetStrengthEntry()
        {
            // 计算精炼后的属性

            if (CEquip?.Attributes.BasicMagicEntry != null)
            {
                BasicMagicStrengthEntry = CEquip.Attributes.BasicMagicEntry.Select(_ =>
                        _.GetAttributeStrengthEntry(AttributeEntryTypeEnum.EquipBasicMagic, RealStrength))
                    .ToImmutableArray();
            }

            if (CEquip?.Attributes.ExtraMagicEntry != null)
            {
                ExtraMagicStrengthEntry = CEquip.Attributes.ExtraMagicEntry.Select(_ =>
                        _.GetAttributeStrengthEntry(AttributeEntryTypeEnum.EquipExtraMagic, RealStrength))
                    .ToImmutableArray();
            }
        }

        public void GetStrengthLevelScore()
        {
            // 计算精炼后的品级和装分提升
            if (HasEquip)
            {
                StrengthLevel = CEquip.CalcStrengthLevel(RealStrength);
                StrengthScore = CEquip.CalcStrengthScore(RealStrength);
            }
        }

        public (AttributeStrengthEntryViewModel[] Basic, AttributeStrengthEntryViewModel[] Extra) GetViewModels()
        {
            // 生成用于装备显示的ViewModels

            if (!ModelHasCalculated)
            {
                Calc();
            }

            AttributeStrengthEntryViewModel[] basic = null;
            AttributeStrengthEntryViewModel[] extra = null;

            if (BasicMagicStrengthEntry != null && BasicMagicStrengthEntry.Any())
            {
                basic = BasicMagicStrengthEntry.Select(_ => new AttributeStrengthEntryViewModel(_)).ToArray();
            }

            if (ExtraMagicStrengthEntry != null && ExtraMagicStrengthEntry.Any())
            {
                extra = ExtraMagicStrengthEntry.Select(_ => new AttributeStrengthEntryViewModel(_)).ToArray();
            }

            return (basic, extra);
        }

        public List<AttributeEntry> GetAllAttributeEntries()
        {
            // 生成所有属性
            var res = new List<AttributeEntry>(BasicMagicStrengthEntry.Length + ExtraMagicStrengthEntry.Length + 5);
            var basic = BasicMagicStrengthEntry.Select(_ => _.ToFinalAttributeEntry());
            var extra = ExtraMagicStrengthEntry.Select(_ => _.ToFinalAttributeEntry());
            res.AddRange(basic);
            res.AddRange(extra);
            AllAttributeEntry = res;
            return res;
        }
    }
}