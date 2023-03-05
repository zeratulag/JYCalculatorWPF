using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;

namespace JX3CalculatorShared.Views.Dialogs
{
    /// <summary>
    /// AboutDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AboutDialog : Window
    {
        public string MainName { get; set; }

        public AboutDialog()
        {
            InitializeComponent();
        }

        private void HandleLinkClick(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = (Hyperlink)sender;
            string navigateUri = hl.NavigateUri.ToString();
            CommandTool.OpenUrl(navigateUri);
            e.Handled = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;  // cancels the window close    
            this.Hide();      // Programmatically hides the window
        }

        private void Head_Img_MouseDown(object sender, RoutedEventArgs e)
        {
            CommandTool.OpenUrl(AppStatic.SinaWBURL);
        }
    }
}
