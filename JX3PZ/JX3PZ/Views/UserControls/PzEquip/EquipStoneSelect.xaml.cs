using JX3PZ.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace JX3PZ.Views
{
    public partial class EquipStoneSelect : UserControl
    {

        public EquipStoneSelectViewModel _VM { get; private set; }

        public static readonly DependencyProperty FilterNameProperty = DependencyProperty.Register(
            nameof(FilterName), typeof(string), typeof(EquipStoneSelect), new PropertyMetadata(default(string)));

        public string FilterName
        {
            get { return (string)GetValue(FilterNameProperty); }
            set { SetValue(FilterNameProperty, value); }
        }


        public EquipStoneSelect()
        {
            InitializeComponent();
        }

        public void ConnectVM(EquipStoneSelectViewModel vm)
        {
            _VM = vm;
            _VM._View = this;
        }

        private void StoneSelect_ListBox_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _VM.ConfirmSelection();
        }

        private void LevelSlider_LostMouseCapture(object sender, MouseEventArgs e)
        {
            // 获取 Slider
            var slider = sender as Slider;

            // 获取绑定表达式并手动更新绑定源
            var bindingExpression = slider.GetBindingExpression(Slider.ValueProperty);
            bindingExpression?.UpdateSource();
        }
    }
}
