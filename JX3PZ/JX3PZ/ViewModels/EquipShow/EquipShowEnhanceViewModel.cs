using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Utils;
using JX3PZ.Data;
using System.Windows;
using System.Windows.Documents;

namespace JX3PZ.ViewModels
{
    public class EquipShowEnhanceViewModel : ObservableObject
    {
        public EnhanceAttributeEntryViewModel Enhance { get; set; }
        public EnhanceAttributeEntryViewModel BigFM { get; set; }

        public bool HasBigFMSlot { get; set; } = false;
        public bool HasBigFM { get; set; } = false;

        public EquipShowEnhanceViewModel(bool hasBigFMSlot)
        {
            HasBigFMSlot = hasBigFMSlot;
            Enhance = null;
            BigFM = null;
        }

        public void UpdateFrom(Enhance e, BigFM b)
        {
            if (e == null)
            {
                Enhance = EnhanceAttributeEntryViewModel.EmptyEnhance;
            }
            else
            {
                Enhance = e.VM;
            }

            if (b == null)
            {
                BigFM = null;
                HasBigFM = false;
            }
            else
            {
                BigFM = b.VM;
                HasBigFM = true;
            }
        }

        #region 流文档元素

        public Section GetSection()
        {
            var sec = FlowDocumentTool.NewSection(nameof(Enhance));
            var para = FlowDocumentTool.NewParagraph();
            para.Margin = new Thickness(1, 0, 0, 0);
            var spanEnhance = Enhance?.GetSpan(nameof(Enhance) + "Span");
            var spanBigFM = BigFM?.GetSpan(nameof(BigFM) + "Span");
            var spans = new Span[2] { spanEnhance, spanBigFM };
            para.AddLines(spans);
            sec.AddParagraph(para);
            return sec;
        }

        #endregion
    }
}