using JX3CalculatorShared.ViewModels;
using System.Windows.Controls;

namespace JX3CalculatorShared.Views.UserControls
{
    /// <summary>
    /// CalcResultView.xaml 的交互逻辑
    /// </summary>
    public partial class CalcResultView : UserControl
    {
        public CalcResultViewModel ViewModel => this.DataContext as CalcResultViewModel;

        public CalcResultView()
        {
            InitializeComponent();
        }

    }
}
