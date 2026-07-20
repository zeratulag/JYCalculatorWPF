using JX3PZ.ViewModels;
using System.Windows;

namespace JX3PZ.Views.BuildCompare
{
    public partial class PzBuildCompareWindow : Window
    {
        public PzBuildCompareWindow(PzMainWindowViewModels pzMainVM, PzTabItemViewModel slotVM)
        {
            InitializeComponent();
            var vm = new PzBuildCompareWindowViewModel(pzMainVM, slotVM);
            vm.RequestClose += Close;
            DataContext = vm;
        }
    }
}
