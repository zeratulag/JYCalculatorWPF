using System.ComponentModel;
using System.Windows;
using JX3PZ.Globals;
using JX3PZ.ViewModels.PzOverview;

namespace J3PZ.Views
{
    /// <summary>
    /// PzOverviewWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PzOverviewWindow : Window
    {
        public readonly PzOverviewWindowViewModel _VM;
        public PzOverviewWindow()
        {
            InitializeComponent();
            _VM = new PzOverviewWindowViewModel();
            this.DataContext = _VM;
            //HorizontalOverview.DataContext = _VM.OverviewVM;
            PzGlobalContext.Views.PzOverview = this;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;  // cancels the window close    
            this.Hide();      // Programmatically hides the window
        }

        private void CaptureOverviewScreenShot_Btn_OnClick(object sender, RoutedEventArgs e)
        {
            CaptureOverviewImg();
        }

        public void CaptureOverviewImg()
        {
            HorizontalOverview.CaptureRenderImg();
        }

        private void CloseWindow_Btn_OnClick(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void CopyOverviewScreenShot_Btn_OnClick(object sender, RoutedEventArgs e)
        {
            HorizontalOverview.CopyRenderImg();
        }
    }
}
