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
            get { return (FlowDocument)GetValue(DocumentSourceProperty); }
            set { SetValue(DocumentSourceProperty, value); }
        }

        // 当DocumentSource改变时，直接更新RichTextBox的Document属性
        private static void OnDocumentSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichTextBoxExt richTextBox)
            {
                var newDocument = e.NewValue as FlowDocument;
                // 直接赋值，这是最高效的方式
                richTextBox.Document = newDocument ?? new FlowDocument();
            }
        }


        // Register the XamlText dependency property
        public static readonly DependencyProperty XamlTextProperty =
            DependencyProperty.Register(
                nameof(XamlText),
                typeof(string),
                typeof(RichTextBoxExt),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnXamlTextChanged)
            );

        // .NET property wrapper
        public string XamlText
        {
            get { return (string)GetValue(XamlTextProperty); }
            set { SetValue(XamlTextProperty, value); }
        }

        // Change callback for when XamlText is updated
        private static void OnXamlTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichTextBoxExt richTextBoxExt)
            {
                richTextBoxExt.UpdateXaml((string)e.NewValue);
            }
        }

        // UpdateXaml method to update the document content
        public void UpdateXaml(string xaml)
        {
            // If the XAML is empty, clear the RichTextBox.
            if (string.IsNullOrEmpty(xaml))
            {
                Document.Clear();
                return;
            }

            // Try to parse the XAML and apply it to the RichTextBox.
            try
            {
                // Load the XAML into a temporary FlowDocument.
                FlowDocument doc = XamlReader.Parse(xaml) as FlowDocument;
                if (doc != null)
                {
                    Document = doc;
                }
            }
            catch (XamlParseException)
            {
                // Handle exceptions if the XAML is invalid.
                // For production code, you might want to do something more robust.
                Document.Clear();
            }
        }

        public void UpdateXaml()
        {
            UpdateXaml(XamlText);
        }
    }
}