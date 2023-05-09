using System.Collections.Generic;
using System.Collections.Immutable;
using JX3PZ.Class;
using JX3PZ.Data;
using JX3PZ.Globals;
using System.Linq;
using System.Xml.Linq;
using JX3PZ.Models;
using HandyControl.Tools.Extension;

namespace JX3PZ.ViewModels
{
    public class EquipShowMagicViewModel: IEquipShow
    {
        public ImmutableArray<AttributeEntryViewModel> BasicMagic { get; set; }
        public ImmutableArray<AttributeEntryViewModel> ExtraMagic { get; set; }

        public ImmutableArray<AttributeStrengthEntryViewModel> BasicMagicStrength { get; set; } // 强化后的
        public ImmutableArray<AttributeStrengthEntryViewModel> ExtraMagicStrength { get; set; } // 强化后的魔法属性
        public string SkillDesc { get; private set; } // 主动使用技能的描述
        public string RequireDesc { get; private set; } // 需要等级120
        public string DurabilityDesc { get; private set; } // 耐久度：5485/5485
        public bool HasDurability { get; private set; } // 是否有耐久度 

        public DiamondLevelItem[] Diamonds { get; set; }

        public EquipShowMagicViewModel()
        {
        }

        public EquipShowMagicViewModel(Equip equip, int strengthLevel = 0, DiamondLevelItem[] diamond = null)
        {
            UpdateFrom(equip, strengthLevel, diamond);
        }

        public EquipShowMagicViewModel(EquipStrengthDiamondModel model)
        {
            UpdateFrom(model);
        }

        public void UpdateFrom(Equip equip, int strengthLevel = 0, DiamondLevelItem[] diamond = null)
        {
            //BasicMagic = equip.Attributes.GetBasicMagicAttributeVMs();
            //ExtraMagic = equip.Attributes.GetExtraMagicAttributeVMs(strengthLevel);
            BasicMagicStrength = equip.Attributes.GetBasicMagicStrengthAttributeVMs(strengthLevel);
            ExtraMagicStrength = equip.Attributes.GetExtraMagicStrengthAttributeVMs(strengthLevel);
            Diamonds = diamond?.ToArray();
            UpdateEquipDesc(equip);
        }

        public void UpdateEquipDesc(Equip equip)
        {
            if (equip != null)
            {
                RequireDesc = equip.GetRequireDesc();
                HasDurability = equip.MaxDurability > 0;
                DurabilityDesc = equip.GetDurabilityDesc();
            }
            else
            {
                RequireDesc = null;
                HasDurability = false;
                DurabilityDesc = null;
            }
        }

        public void UpdateFrom(EquipStrengthModel model)
        {
            var vms = model.GetViewModels();
            BasicMagicStrength = vms.Basic?.ToImmutableArray() ?? ImmutableArray<AttributeStrengthEntryViewModel>.Empty;
            ExtraMagicStrength = vms.Extra?.ToImmutableArray() ?? ImmutableArray<AttributeStrengthEntryViewModel>.Empty;
        }

        public void UpdateFrom(EquipStrengthDiamondModel model)
        {
            UpdateFrom(model.StrengthModel);
            Diamonds = model.Diamonds?.ToArray();
            UpdateEquipDesc(model.CEquip);
        }

        public List<string> GetDescList()
        {
            var res = new List<string>(BasicMagicStrength.Length + ExtraMagicStrength.Length + 3);
            res.AddRange(BasicMagicStrength.Select(_ => _.Text1));
            res.AddRange(ExtraMagicStrength.Select(_ => _.Text1));
            return res;
        }
    }
}