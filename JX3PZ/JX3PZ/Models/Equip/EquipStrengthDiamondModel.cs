using JX3PZ.Class;
using JX3PZ.Data;
using System.Collections.Generic;
using System.Linq;
using JX3CalculatorShared.Common;
using JX3PZ.Globals;

namespace JX3PZ.Models
{
    public class EquipStrengthDiamondModel : IModel
    {
        // 装备精炼和强化模型
        public readonly Equip CEquip;
        public readonly bool HasEquip = false;
        public readonly int[] DiamondLevels = null;
        public readonly DiamondLevelItem[] Diamonds = null;

        public readonly EquipStrengthModel StrengthModel = null;

        public readonly bool HasDiamond = false; // 是否已经镶嵌了五行石（五行石等级>0）
        public int DiamondCount { get; private set; } = 0; // 五行石总数
        public int DiamondIntensity { get; private set; } = 0; // 五行石等级和
        public decimal DiamondScore { get; private set; } = 0m; // 五行石分数

        public List<AttributeEntry> EquipAttributeEntry { get; private set; } // 装备的所有属性
        public AttributeEntry[] DiamondAttributeEntry { get; private set; } = null;// 镶嵌的所有属性

        public EquipStrengthDiamondModel(Equip cEquip, int strength = 0)
        {
            CEquip = cEquip;
            HasEquip = CEquip != null;
            StrengthModel = new EquipStrengthModel(cEquip, strength);
        }

        public EquipStrengthDiamondModel(Equip cEquip, int strength = 0, int[] diamondLevels = null) : this(cEquip,
            strength)
        {
            DiamondLevels = diamondLevels;

            if (HasEquip)
            {
                Diamonds = cEquip.Attributes.GetDiamondLevelItems(diamondLevels);
            }
        }

        public EquipStrengthDiamondModel(Equip cEquip, int strength = 0, DiamondLevelItem[] diamonds = null) : this(
            cEquip,
            strength)
        {
            Diamonds = diamonds;
            DiamondLevels = diamonds?.Select(_ => _.Level).ToArray();
            HasDiamond = DiamondLevels != null && DiamondLevels.Select(_ => _ > 0).Any();
        }

        public void Calc()
        {
            StrengthModel.Calc();
            GetDiamondScore();
            GetAllAttributeEntries();
        }

        public void GetDiamondScore()
        {
            if (HasDiamond)
            {
                DiamondCount = DiamondLevels.Select(_ => _ > 0).Count();
                DiamondIntensity = DiamondLevels.Sum();
                DiamondScore = Diamonds.Sum(_ => _.GetScore());
            }
        }

        public void GetAllAttributeEntries()
        {
            EquipAttributeEntry = StrengthModel.AllAttributeEntry;
            if (HasDiamond)
            {
                DiamondAttributeEntry = Diamonds.Select(_ => _.Entry).ToArray();
            }
        }
    }
}