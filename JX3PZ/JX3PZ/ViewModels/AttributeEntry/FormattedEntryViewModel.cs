using JX3CalculatorShared.Utils;
using JX3PZ.Globals;
using JX3PZ.Models;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;

namespace JX3PZ.ViewModels
{
    public class FormattedEntryViewModel
    {
        // 用于模拟 精炼词条 类 同一行文字有多种颜色的VM

        public string Text1 { get; protected set; } = string.Empty;
        public string Text2 { get; protected set; } = string.Empty;
        public string Text3 { get; protected set; } = string.Empty;
        public string Color1 { get; protected set; } = ColorConst.White;
        public string Color2 { get; protected set; } = ColorConst.White;
        public string Color3 { get; protected set; } = ColorConst.White;

        public FormattedEntryViewModel()
        {
        }

        public FormattedEntryViewModel(string text1, string text2, string text3, string color1, string color2, string color3)
        {
            Text1 = text1;
            Text2 = text2;
            Text3 = text3;
            Color1 = color1;
            Color2 = color2;
            Color3 = color3;
        }

        public Span GetSpan(object tag = null)
        {
            var span = new Span();
            var runs = new List<Run>(3);
            runs.Add(FlowDocumentTool.GetRun(Text1, Color1));
            runs.Add(FlowDocumentTool.GetRun(Text2, Color2));
            runs.Add(FlowDocumentTool.GetRun(Text3, Color3));
            span.Inlines.AddRange(runs);
            if (tag != null)
            {
                span.Tag = tag;
            }
            return span;
        }
    }

    public class QualityEntryViewModel : FormattedEntryViewModel
    {
        // 表示 品质等级 8400 (+1042) 的类
        public QualityEntryViewModel(EquipLevelScore e)
        {
            Text1 = $"品质等级 {e.Level}";
            Color1 = ColorConst.Yellow;
            if (e.StrengthLevel > 0)
            {
                Text2 = $"(+{e.StrengthLevel})";
                Color2 = ColorConst.Strength;
            }
        }
    }

    public class ScoreEntryViewModel : FormattedEntryViewModel
    {
        // 表示 装备分数 25200 (+3125+1098) 的类
        public ScoreEntryViewModel(EquipLevelScore e)
        {
            Text1 = $"装备分数 {e.BaseScore}";
            Color1 = ColorConst.Orange;
            bool hasP2 = false;
            var t2b = new StringBuilder();
            if (e.StrengthScore > 0)
            {
                t2b.Append($"+{e.StrengthScore}");
                hasP2 = true;
            }

            if (e.EnhanceScore > 0)
            {
                t2b.Append($"+{e.EnhanceScore}");
                hasP2 = true;
            }

            if (hasP2)
            {
                Text2 = $"({t2b.ToString()})";
                Color2 = ColorConst.Strength;
            }
        }
    }
}