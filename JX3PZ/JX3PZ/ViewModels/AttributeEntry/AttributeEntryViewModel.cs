using JX3CalculatorShared.Utils;
using System.Windows.Documents;

namespace JX3PZ.ViewModels
{
    public class AttributeEntryViewModel
    {
        // 词条属性的VM
        public string Desc { get; protected set; }
        public virtual string Color { get; protected set; } = "#000000";

        public AttributeEntryViewModel(string desc = "", string color = "#000000")
        {
            Desc = desc;
            Color = color;
        }

        #region 流文档元素

        // 生成Run对象
        public Run GetRun()
        {
            return FlowDocumentTool.GetRun(Desc, Color);
        }

        // 生成Span对象
        public virtual Span GetSpan(object tag = null)
        {
            Span span = new Span();
            if (tag != null)
            {
                span.Tag = tag;
            }
            var run = GetRun();
            span.Inlines.Add(run);
            return span;
        }

        #endregion
    }
}