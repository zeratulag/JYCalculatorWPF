using JX3CalculatorShared.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace JX3CalculatorShared.Views.UserControls
{
    public class RichTextBoxExt : RichTextBox
    {
        // 创建一个新的依赖属性，类型为FlowDocument
        public static readonly DependencyProperty DocumentSourceProperty =
            DependencyProperty.Register(
                nameof(DocumentSource),
                typeof(FlowDocument),
                typeof(RichTextBoxExt),
                new FrameworkPropertyMetadata(null, OnDocumentSourceChanged));

        // .NET属性包装器
        public FlowDocument DocumentSource
        {
            get { return (FlowDocument) GetValue(DocumentSourceProperty); }
            set { SetValue(DocumentSourceProperty, value); }
        }

        // 当DocumentSource改变时，直接更新RichTextBox的Document属性
        private static void OnDocumentSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichTextBoxExt richTextBox)
            {
                var newDoc = e.NewValue as FlowDocument;
                // 直接赋值，这是最高效的方式
                if (newDoc == null)
                {
                    richTextBox.Document = new FlowDocument();
                    return;
                }

                if (ReferenceEquals(richTextBox.Document, newDoc))
                {
                    return;
                }

                richTextBox.Document = newDoc.Parent == null ? newDoc : newDoc.Copy();
            }
        }
    }
}