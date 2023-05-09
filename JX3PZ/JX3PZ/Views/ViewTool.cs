using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;

namespace JX3PZ.Views
{
    public static class FlowDocumentPagePadding
    {
        public static Thickness GetPagePadding(DependencyObject obj)
        {
            return (Thickness) obj.GetValue(PagePaddingProperty);
        }

        public static void SetPagePadding(DependencyObject obj, Thickness value)
        {
            obj.SetValue(PagePaddingProperty, value);
        }

        public static readonly DependencyProperty PagePaddingProperty =
            DependencyProperty.RegisterAttached("PagePadding", typeof(Thickness), typeof(FlowDocumentPagePadding),
                new UIPropertyMetadata(new Thickness(double.NegativeInfinity), (o, args) =>
                {
                    var fd = o as FlowDocument;
                    if (fd == null) return;
                    var dpd = DependencyPropertyDescriptor.FromProperty(FlowDocument.PagePaddingProperty,
                        typeof(FlowDocument));
                    dpd.RemoveValueChanged(fd, PaddingChanged);
                    fd.PagePadding = (Thickness) args.NewValue;
                    dpd.AddValueChanged(fd, PaddingChanged);
                }));

        public static void PaddingChanged(object s, EventArgs e)
        {
            ((FlowDocument) s).PagePadding = GetPagePadding((DependencyObject) s);
        }
    }
}