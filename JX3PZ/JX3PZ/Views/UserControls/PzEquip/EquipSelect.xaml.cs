using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            get { return (int)GetValue(MinLevelProperty); }
            set { SetValue(MinLevelProperty, value); }
        }

        public static readonly DependencyProperty MaxLevelProperty = DependencyProperty.Register(
            nameof(MaxLevel), typeof(int), typeof(EquipSelect), new PropertyMetadata(default(int)));

        public int MaxLevel
        {
            get { return (int)GetValue(MaxLevelProperty); }
            set { SetValue(MaxLevelProperty, value); }
        }

        public EquipSelect()
        {
            InitializeComponent();
        }

        private void LevelRangeSlider_LostMouseCapture(object sender, MouseEventArgs e)
        {
            var rangeSlider = sender as HandyControl.Controls.RangeSlider;

            // 获取绑定表达式
            var bindingExpressionStart = rangeSlider.GetBindingExpression(HandyControl.Controls.RangeSlider.ValueStartProperty);
            var bindingExpressionEnd = rangeSlider.GetBindingExpression(HandyControl.Controls.RangeSlider.ValueEndProperty);

            // 手动更新绑定源
            bindingExpressionStart?.UpdateSource();
            bindingExpressionEnd?.UpdateSource();
        }
    }
}
