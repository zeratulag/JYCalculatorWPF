using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Utils;
using JX3PZ.Globals;
using JX3PZ.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace JX3PZ.ViewModels
{
    public class EquipShowTailViewModel : ObservableObject, IEquipShow
    {
        public string SkillDesc { get; private set; } = "";// 主动使用技能的描述
        public bool HasSkillDesc => SkillDesc.IsNotEmptyOrWhiteSpace(); // 是否有
        public string ItemDesc { get; private set; } = "";// 物品描述
        public bool HasItemDesc => ItemDesc.IsNotEmptyOrWhiteSpace(); // 是否有
        public string RequireDesc { get; private set; } // 需要等级120
        public string DurabilityDesc { get; private set; } // 耐久度：5485/5485
        public bool HasDurability { get; private set; } // 是否有耐久度 
        public FormattedEntryViewModel Quality { get; private set; } // 品级
        public FormattedEntryViewModel Score { get; private set; } // 装分

        public EquipShowTailViewModel()
        {
        }

        public EquipShowTailViewModel(EquipSnapShotModel model)
        {
            UpdateFrom(model);
        }

        public void UpdateFrom(EquipSnapShotModel model)
        {
            if (model.CEquip != null)
            {
                SkillDesc = model.CEquip.SkillDesc;
                RequireDesc = model.CEquip.GetRequireDesc();
                HasDurability = model.CEquip.MaxDurability > 0;
                DurabilityDesc = model.CEquip.GetDurabilityDesc();
                Quality = new QualityEntryViewModel(model.Score);
                Score = new ScoreEntryViewModel(model.Score);
                ItemDesc = model.CEquip.ItemDesc;
            }
            else
            {
                SkillDesc = "";
                RequireDesc = "";
                HasDurability = false;
                DurabilityDesc = "";
                Quality = null;
                Score = null;
                ItemDesc = "";
            }
        }

        public List<string> GetDescList()
        {
            var res = new List<string>(10)
            {
                RequireDesc,
            };

            if (HasDurability)
            {
                res.Add(DurabilityDesc);
            }

            if (HasSkillDesc)
            {
                res.Add("");
                res.Add(SkillDesc);
            }
            res.Add("");
            res.Add(Quality.Text1);
            res.Add(Score.Text1);
            return res;
        }

        #region 流文档元素

        public Section GetSection()
        {
            var section = new Section
            {
                Margin = new Thickness(0, 10, 0, 0),
                Tag = "Tail"
            };
            var p1 = GetSkillDescParagraph();
            section.AddParagraph(p1);
            var p2 = GetDescParagraph();
            section.AddParagraph(p2);
            var p3 = GetQualityScoreParagraph();
            section.AddParagraph(p3);
            return section;
        }

        public Paragraph GetSkillDescParagraph()
        {
            if (!HasSkillDesc)
            {
                return null;
            }

            var para = FlowDocumentTool.NewParagraph(nameof(SkillDesc));
            var span1 = FlowDocumentTool.GetSpan(SkillDesc, ColorConst.Green, nameof(SkillDesc));
            para.Inlines.Add(span1);
            return para;
        }

        public Paragraph GetDescParagraph()
        {
            if (!HasItemDesc)
            {
                return null;
            }

            var para = FlowDocumentTool.NewParagraph(nameof(ItemDesc));
            para.Margin = new Thickness(0, 5, 0, 5);

            var span2 = FlowDocumentTool.GetSpan(ItemDesc, ColorConst.Yellow, nameof(ItemDesc));
            span2.FontSize = 12;
            para.Inlines.Add(span2);
            return para;
        }

        public Paragraph GetQualityScoreParagraph()
        {
            var para = FlowDocumentTool.NewParagraph(nameof(Quality) + nameof(Score));
            var span3 = Quality?.GetSpan(nameof(Quality));
            var span4 = Score?.GetSpan(nameof(Score));
            var spans = new List<Span>(5);
            spans.Add(span3);
            spans.Add(span4);
            para.AddLines(spans);
            return para;
        }


        #endregion
    }
}