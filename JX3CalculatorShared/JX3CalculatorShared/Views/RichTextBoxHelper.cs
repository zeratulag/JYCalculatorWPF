using JX3CalculatorShared.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace JX3CalculatorShared.Views
{
    public class RichTextBoxHelper : DependencyObject
    {
        public static string GetDocumentXaml(DependencyObject obj)
        {
            return (string)obj.GetValue(DocumentXamlProperty);
        }

        public static void SetDocumentXaml(DependencyObject obj, string value)
        {
            obj.SetValue(DocumentXamlProperty, value);
        }

        public static readonly DependencyProperty DocumentXamlProperty =
            DependencyProperty.RegisterAttached(
                "DocumentXaml",
                typeof(string),
                typeof(RichTextBoxHelper),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    PropertyChangedCallback = (obj, e) =>
                    {
                        var richTextBox = (RichTextBox)obj;

                        // Parse the XAML to a document (or use XamlReader.Parse())
                        var xaml = GetDocumentXaml(richTextBox);
                        var doc = (FlowDocument)FlowDocumentTool.ConvertXamlText(xaml);
                        // Set the document
                        richTextBox.Document = doc;
                    }
                });
    }
}