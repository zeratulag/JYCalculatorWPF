using System.ComponentModel;
using System.Windows;

namespace JYCalculator.Views.Dialogs
{
    /// <summary>
    /// AboutDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AboutDialog : Window
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;  // cancels the window close    
            this.Hide();      // Programmatically hides the window
        }
    }
}
