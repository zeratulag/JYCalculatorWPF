using System.Collections.Generic;
using JX3CalculatorShared.Utils;
using JX3PZ.Data;
using System.Collections.Immutable;
using JX3CalculatorShared.Globals;
using System.Data;
using System.Linq;
using JX3PZ.Globals;
using JX3PZ.ViewModels;

namespace JX3PZ.Class
{
    public class EquipAttribute
    {
        public readonly EquipSubTypeEnum SubType;
        public readonly ImmutableArray<AttributeEntry> BaseEntry; // 基础白字属性（防具的防御，武器的伤害）
        public readonly ImmutableDictionary<string, int> BaseEntryDict; // 属性字典

        public readonly ImmutableArray<int> MagicEntryID;
        public readonly ImmutableArray<int> BasicMagicEntryID;
        public readonly ImmutableArray<int> ExtraMagicEntryID;
        public readonly ImmutableArray<int> DiamondID;

        public readonly ImmutableArray<AttributeTabItem> MagicEntry;
        public readonly ImmutableArray<AttributeTabItem> BasicMagicEntry;
        public readonly ImmutableArray<AttributeTabItem> ExtraMagicEntry;
        public readonly ImmutableArray<DiamondTabItem> Diamond;
        public readonly int NDiamond = 0; // 镶嵌孔个数

        public EquipAttribute(Equip e)
        {
            SubType = e.SubTypeEnum;
            BaseEntry = GetBaseEntries(e);
            BaseEntryDict = BaseEntry.ToImmutableDictionary(_ => _.ModifyType, _ => _.Value);

            MagicEntryID = ParseIntStr(e.MagicEntry_Str);
            BasicMagicEntryID = ParseIntStr(e.BasicMagicEntry_Str);
            ExtraMagicEntryID = ParseIntStr(e.ExtraMagicEntry_Str);
            DiamondID = ParseIntStr(e.DiamondID_Str);

            MagicEntry = AttributeTabLib.Gets(MagicEntryID);
            BasicMagicEntry = AttributeTabLib.Gets(BasicMagicEntryID);
            ExtraMagicEntry = AttributeTabLib.Gets(ExtraMagicEntryID);
            Diamond = DiamondTabLib.Gets(DiamondID);

            if (DiamondID != null)
            {
                NDiamond = DiamondID.Length;
            }
        }

        public ImmutableArray<AttributeEntry> GetBaseEntries(Equip e)
        {
            var res = ImmutableArray.CreateBuilder<AttributeEntry>();
            if (IsValidModifyType(e.Base1Type))
            {
                res.Add(new AttributeEntry(e.Base1Type, e.Base1Value, AttributeEntryTypeEnum.EquipBase));
            }

            if (IsValidModifyType(e.Base2Type))
            {
                res.Add(new AttributeEntry(e.Base2Type, e.Base2Value, AttributeEntryTypeEnum.EquipBase));
            }

            if (IsValidModifyType(e.Base3Type))
            {
                res.Add(new AttributeEntry(e.Base3Type, e.Base3Value, AttributeEntryTypeEnum.EquipBase));
            }

            return res.ToImmutableArray();
        }

        public bool IsValidModifyType(string modifyType)
        {
            // 是否为有效属性
            if (modifyType == null || modifyType.IsEmptyOrWhiteSpace()) return false;
            return modifyType != "atInvalid";
        }


        public static ImmutableArray<int> ParseIntStr(string x)
        {
            if (x.IsEmptyOrWhiteSpace() || x == "[]")
            {
                return new ImmutableArray<int>();
            }
            else
            {
                var res = StringTool.ParseIntList(x).ToImmutableArray();
                return res;
            }
        }

        public ImmutableArray<AttributeEntryViewModel> GetBasicMagicAttributeVMs(int strengthLevel = 0)
        {
            if (BasicMagicEntry == null)
            {
                return ImmutableArray<AttributeEntryViewModel>.Empty;
            }

            var basic = BasicMagicEntry.Select(_ =>
                _.GetAttributeEntry(AttributeEntryTypeEnum.EquipBasicMagic, strengthLevel));
            var res = basic.Select(_ => _.GetViewModel()).ToImmutableArray();
            return res;
        }

        public ImmutableArray<AttributeEntryViewModel> GetExtraMagicAttributeVMs(int strengthLevel = 0)
        {
            if (ExtraMagicEntry == null)
            {
                return ImmutableArray<AttributeEntryViewModel>.Empty;
            }

            var extra = ExtraMagicEntry.Select(_ =>
                _.GetAttributeEntry(AttributeEntryTypeEnum.EquipExtraMagic, strengthLevel));

            var l = new List<AttributeEntry>();
            foreach (var _ in ExtraMagicEntry)
            {
                var ae = _.GetAttributeEntry(AttributeEntryTypeEnum.EquipExtraMagic, 0);
                ae.GetViewModel();
                l.Add(ae);
            }

            var res = extra.Select(_ => _.GetViewModel()).ToImmutableArray();
            return res;
        }

        public ImmutableArray<AttributeStrengthEntryViewModel> GetExtraMagicStrengthAttributeVMs(int strengthLevel = 0)
        {
            var res = GetStrengthVMs(ExtraMagicEntry, strengthLevel, AttributeEntryTypeEnum.EquipExtraMagic);
            return res;
        }

        public ImmutableArray<AttributeStrengthEntryViewModel> GetBasicMagicStrengthAttributeVMs(int strengthLevel = 0)
        {
            var res = GetStrengthVMs(BasicMagicEntry, strengthLevel, AttributeEntryTypeEnum.EquipBasicMagic);
            return res;
        }

        /// <summary>
        /// 生成强化后的VM
        /// </summary>
        /// <param name="data">数据集合</param>
        /// <param name="strengthLevel">强化等级</param>
        /// <param name="typeEnum">类型</param>
        /// <returns></returns>
        public static ImmutableArray<AttributeStrengthEntryViewModel> GetStrengthVMs(
            ImmutableArray<AttributeTabItem> data,
            int strengthLevel = 0, AttributeEntryTypeEnum typeEnum = AttributeEntryTypeEnum.Default)
        {
            if (data == null)
            {
                return ImmutableArray<AttributeStrengthEntryViewModel>.Empty;
            }

            var l = new List<AttributeStrengthEntryViewModel>();
            foreach (var _ in data)
            {
                var ae = _.GetAttributeStrengthEntry(typeEnum, strengthLevel);
                l.Add(new AttributeStrengthEntryViewModel(ae));
            }

            var res = l.ToImmutableArray();
            return res;
        }

        /// <summary>
        /// 基于镶嵌的五行石等级，获得镶嵌的五行石属性
        /// </summary>
        /// <param name="levels"></param>
        /// <returns></returns>
        public DiamondLevelItem[] GetDiamondLevelItems(IEnumerable<int> levels)
        {
            var diamonds = new List<DiamondLevelItem>(Diamond.Length);
            int i = 0;
            foreach (var l in levels)
            {
                diamonds.Add(Diamond[i].LevelItems[l]);
                i++;
            }

            var res = diamonds.ToArray();
            return res;
        }

        public IEnumerable<string> GetDiamondDesc()
        {
            if (NDiamond > 0)
            {
                var res = Diamond.Select(_ => _.GetDesc(0));
                return res;
            }
            return Enumerable.Empty<string>();
        }

        public void AttachEquipOptionWaterDesc(string desc)
        {
            // 在合适的地方增加水特效信息
            foreach (var _ in ExtraMagicEntry)
            {
                if (_.SpecialDesc.Contains("可叠加十层"))
                {
                    _.SpecialDesc += desc;
                }
            }
        }
    }
}