using JX3CalculatorShared.Class;
using JX3CalculatorShared.Utils;
using JX3PZ.Data;
using System.Windows.Documents;

namespace JX3PZ.ViewModels
{
    public class EnhanceAttributeEntryViewModel : IconAttributeEntryViewModel
    {
        public static readonly EnhanceAttributeEntryViewModel EmptyEnhance;

        static EnhanceAttributeEntryViewModel()
        {
            EmptyEnhance = new EnhanceAttributeEntryViewModel(e: null);
        }


        // 附魔在装备预览界面显示的VM
        public EnhanceAttributeEntryViewModel(string desc = "", string color = "#000000", string iconPath = null) : base(
            desc, color, iconPath)
        {
        }

        public EnhanceAttributeEntryViewModel(Enhance e)
        {
            UpdateFrom(e);
            // 从附魔构建
        }

        public void UpdateFrom(Enhance e)
        {
            // 从附魔构建
            if (e == null)
            {
                Desc = "未强化";
            }
            else
            {
                Desc = e.Desc;
            }

            (Color, IconPath) = Enhance.GetColorImage(e);
        }


        public EnhanceAttributeEntryViewModel(BigFM b)
        {
            // 从大附魔构建
            if (b != null)
            {
                Desc = b.EnhanceDesc;
                (Color, IconPath) = BigFM.GetColorImage(b);
            }
        }

        #region 流文档元素

        public override Span GetSpan(object tag = null)
        {
            var run = GetRun();
            var span = FlowDocumentTool.GetImageSpan(IconPath, run);
            if (tag != null)
            {
                span.Tag = tag;
            }
            return span;
        }

        #endregion

    }
}