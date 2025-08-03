using JX3CalculatorShared.Utils;
using JX3PZ.Class;
using JX3PZ.Data;
using JX3PZ.Models;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace JX3PZ.ViewModels
{
    public class EquipShowMagicViewModel : IEquipShow
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
        public bool HasDiamond { get; set; } = true;

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
                HasDiamond = equip.HasDiamond;
            }
            else
            {
                RequireDesc = null;
                HasDurability = false;
                DurabilityDesc = null;
                HasDiamond = false;
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


        #region 构建FlowDocument元素

        public Section GetMagicSection()
        {
            // 创建一个新的Section
            Section section = FlowDocumentTool.NewSection(tag: "Magic");
            var p1 = GetMagicStrengthParagraph(BasicMagicStrength, nameof(BasicMagicStrength));
            section.AddParagraph(p1);
            var p2 = GetMagicStrengthParagraph(ExtraMagicStrength, nameof(ExtraMagicStrength));
            section.AddParagraph(p2);
            var p3 = GetDiamondParagraph();
            section.AddParagraph(p3);
            return section;
        }

        public static Paragraph GetMagicStrengthParagraph(IList<AttributeStrengthEntryViewModel> entries, string Tag)
        {
            if (entries == null || !entries.Any()) return null;
            var para = FlowDocumentTool.NewParagraph(Tag);
            var spans = entries.Select(_ => _.GetSpan());
            para.AddLines(spans);
            return para;
        }

        public Paragraph GetDiamondParagraph()
        {
            if (!HasDiamond) return null;
            var para = FlowDocumentTool.NewParagraph(nameof(Diamonds));
            para.Margin = new Thickness(1, 0, 0, 0);
            para.AddLines(Diamonds.Select(d => d.MakeSpan()));
            return para;
        }

        public Paragraph GetRequireParagraph()
        {
            var para = FlowDocumentTool.NewParagraph();
            para.Foreground = Brushes.White;
            para.AddLines(RequireDesc, DurabilityDesc);
            return para;
        }

        public Section GetRequireSection()
        {
            Section section = FlowDocumentTool.NewSection(tag: "Require");
            var para = GetRequireParagraph();
            section.AddParagraph(para);
            return section;
        }

        #endregion
    }
}