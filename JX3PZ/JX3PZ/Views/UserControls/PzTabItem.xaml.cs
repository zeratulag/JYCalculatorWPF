using System.Windows;
using System.Windows.Controls;

namespace JX3PZ.Views
{
    /// <summary>
    /// PzEquipTabItem.xaml 的交互逻辑
    /// </summary>
    public partial class PzTabItem : UserControl
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            nameof(Position), typeof(int), typeof(PzTabItem), new PropertyMetadata(default(int)));

        public int Position
        {
            get { return (int)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }


        public PzTabItem()
        {
            InitializeComponent();
        }

        //public EquipSelectViewModel EquipVM;

        //public void MakeVMs()
        //{
        //    EquipVM = new EquipSelectViewModel(Position);
        //}

    }
}
