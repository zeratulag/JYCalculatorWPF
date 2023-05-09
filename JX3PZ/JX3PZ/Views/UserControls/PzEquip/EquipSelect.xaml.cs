using System.Windows;
using System.Windows.Controls;

namespace JX3PZ.Views
{
    /// <summary>
    /// PzEquipTabItem.xaml 的交互逻辑
    /// </summary>
    public partial class EquipSelect : UserControl
    {
        public static readonly DependencyProperty MinLevelProperty = DependencyProperty.Register(
            nameof(MinLevel), typeof(int), typeof(EquipSelect), new PropertyMetadata(default(int)));

        public int MinLevel
        {
            get { return (int) GetValue(MinLevelProperty); }
            set { SetValue(MinLevelProperty, value); }
        }

        public static readonly DependencyProperty MaxLevelProperty = DependencyProperty.Register(
            nameof(MaxLevel), typeof(int), typeof(EquipSelect), new PropertyMetadata(default(int)));

        public int MaxLevel
        {
            get { return (int) GetValue(MaxLevelProperty); }
            set { SetValue(MaxLevelProperty, value); }
        }

        public EquipSelect()
        {
            InitializeComponent();
        }
    }
}
