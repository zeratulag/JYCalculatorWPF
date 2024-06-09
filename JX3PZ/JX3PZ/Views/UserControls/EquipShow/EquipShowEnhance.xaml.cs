using System.Windows;
using System.Windows.Controls;

namespace JX3PZ.Views
{
    public partial class EquipShowEnhance : UserControl
    {

        public static readonly DependencyProperty HasBigFMSlotProperty = DependencyProperty.Register(
            nameof(HasBigFMSlot), typeof(bool), typeof(EquipShowEnhance), new PropertyMetadata(default(bool)));

        public bool HasBigFMSlot
        {
            get { return (bool)GetValue(HasBigFMSlotProperty); }
            set { SetValue(HasBigFMSlotProperty, value); }
        }

        public static readonly DependencyProperty HasBigFMProperty = DependencyProperty.Register(
            nameof(HasBigFM), typeof(bool), typeof(EquipShowEnhance), new PropertyMetadata(default(bool)));

        public bool HasBigFM
        {
            get { return (bool)GetValue(HasBigFMProperty); }
            set { SetValue(HasBigFMProperty, value); }
        }


        public EquipShowEnhance()
        {
            InitializeComponent();
        }
    }
}
