using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JX3PZ.ViewModels;

namespace JX3PZ.Views
{
    public partial class EquipStoneSelect : UserControl
    {

        public EquipStoneSelectViewModel _VM { get; private set; }

        public static readonly DependencyProperty FilterNameProperty = DependencyProperty.Register(
            nameof(FilterName), typeof(string), typeof(EquipStoneSelect), new PropertyMetadata(default(string)));

        public string FilterName
        {
            get { return (string) GetValue(FilterNameProperty); }
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
    }
}
