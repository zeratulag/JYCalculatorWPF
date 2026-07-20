using JX3PZ.ViewModels;
using System.Windows;

namespace JX3PZ.Views.BuildCompare
{
    public partial class PzBuildCompareExportProgressWindow : Window
    {
        public PzBuildCompareExportProgressWindow(PzBuildCompareExportProgressViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
