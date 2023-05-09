using System.Windows;
using System.Windows.Controls;
using HandyControl.Data;

namespace JX3PZ.Views
{
    /// <summary>
    /// PzEquipTabItem.xaml 的交互逻辑
    /// </summary>
    public partial class EquipEnhance : UserControl
    {
        public static readonly DependencyProperty StrengthLevelProperty = DependencyProperty.Register(
            nameof(StrengthLevel), typeof(int), typeof(EquipEnhance), new PropertyMetadata(default(int)));

        public int StrengthLevel
        {
            get { return (int)GetValue(StrengthLevelProperty); }
            set { SetValue(StrengthLevelProperty, value); }
        }

        public static readonly DependencyProperty MaxStrengthLevelProperty = DependencyProperty.Register(
            nameof(MaxStrengthLevel), typeof(int), typeof(EquipEnhance), new PropertyMetadata(default(int)));

        public int MaxStrengthLevel
        {
            get { return (int)GetValue(MaxStrengthLevelProperty); }
            set { SetValue(MaxStrengthLevelProperty, value); }
        }

        public EquipEnhance()
        {
            InitializeComponent();
        }

    }
}
