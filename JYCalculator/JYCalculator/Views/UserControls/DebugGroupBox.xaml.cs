using JYCalculator.Globals;
using System.Windows;
using System.Windows.Controls;

namespace JYCalculator.Views.UserControls
{
    /// <summary>
    /// GroupBoxDebug.xaml 的交互逻辑
    /// </summary>
    public partial class DebugGroupBox : UserControl
    {
        public MainWindow _MV => GlobalContext.Views.Main;

        public DebugGroupBox()
        {
            InitializeComponent();
        }

        private void BtnExportMiJi_OnClick(object sender, RoutedEventArgs e)
        {
            _MV.ExportMiJi();
        }

        private void BtnExportAll_OnClick(object sender, RoutedEventArgs e)
        {
            _MV.ExportAll();
        }
    }
}
