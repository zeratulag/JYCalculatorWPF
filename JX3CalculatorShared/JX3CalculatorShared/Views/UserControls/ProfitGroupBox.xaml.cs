using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace JX3CalculatorShared.Views.UserControls
{
    /// <summary>
    /// ProfitGroupBox.xaml 的交互逻辑
    /// </summary>
    public partial class ProfitGroupBox : UserControl
    {
        public ProfitGroupBox()
        {
            InitializeComponent();
        }

        private void GroupBox_Profit_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(ProfitOrderDesc_txb.Text);
        }
    }
}
