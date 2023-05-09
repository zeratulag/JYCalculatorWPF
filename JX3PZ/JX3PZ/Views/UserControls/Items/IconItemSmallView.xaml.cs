using System.Windows;
using System.Windows.Controls;

namespace JX3CalculatorShared.Views.UserControls
{
    /// <summary>
    /// ItemDTItem.xaml 的交互逻辑
    /// </summary>
    public partial class IconItemSmallView : UserControl
    {

        public static readonly DependencyProperty IconHeightProperty = DependencyProperty.Register(
            nameof(IconHeight), typeof(int), typeof(IconItemSmallView), new PropertyMetadata(default(int)));

        public int IconHeight
        {
            get { return (int) GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }

        public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register(
            nameof(IconWidth), typeof(int), typeof(IconItemSmallView), new PropertyMetadata(default(int)));

        public int IconWidth
        {
            get { return (int) GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }

        public IconItemSmallView()
        {
            InitializeComponent();
        }
    }
}
