using JX3CalculatorShared.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace JX3CalculatorShared.Views.UserControls
{
    public class RichTextBoxExt : RichTextBox
    {
        // Register the XamlText dependency property
        public static readonly DependencyProperty XamlTextProperty =
            DependencyProperty.Register(
                "XamlText",
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