using System.Windows;
using System.Windows.Controls;
using JX3PZ.ViewModels;

namespace JX3PZ.Views
{
    /// <summary>
    /// PzEquipTabItem.xaml 的交互逻辑
    /// </summary>
    public partial class EquipEmbed : UserControl
    {

        public static readonly DependencyProperty HasStoneProperty = DependencyProperty.Register(
            nameof(HasStone), typeof(bool), typeof(EquipEmbed), new PropertyMetadata(default(bool)));

        // 当前装备是否有五彩石槽
        public bool HasStone
        {
            get { return (bool) GetValue(HasStoneProperty); }
            set { SetValue(HasStoneProperty, value); }
        }

        public static readonly DependencyProperty ShowStoneProperty = DependencyProperty.Register(
            nameof(ShowStone), typeof(bool), typeof(EquipEmbed), new PropertyMetadata(default(bool)));

        // 是否显示五彩石
        public bool ShowStone
        {
            get { return (bool) GetValue(ShowStoneProperty); }
            set { SetValue(ShowStoneProperty, value); }
        }

        public EquipEmbedViewModel _VM { get; private set; }

        public EquipEmbed()
        {
            InitializeComponent();
        }

        public void ConnectVM(EquipEmbedViewModel vm)
        {
            _VM = vm;
            vm._View = this;
            equipStoneSelect.ConnectVM(vm.StoneSelectVM);
        }

    }
}
