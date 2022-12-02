using JYCalculator.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace JYCalculator.Views
{
    /// <summary>
    /// DebugWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DebugMainWindow : Window
    {
        private MainWindowViewModels _VMs;

        public DebugMainWindow(MainWindowViewModels vms)
        {
            _VMs = vms;
            InitializeComponent();

            BindViewModels();

        }

        public void BindViewModels()
        {
            JYDebugWindow.DataContext = _VMs;

            Expander_FightTimeSummary.DataContext = _VMs.FightTimeSummaryVM;

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;  // cancels the window close    
            this.Hide();      // Programmatically hides the window
        }

    }
}
