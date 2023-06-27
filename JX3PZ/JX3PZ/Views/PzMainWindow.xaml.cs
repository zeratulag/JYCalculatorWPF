using JX3CalculatorShared.Views;
using JX3PZ.Globals;
using JX3PZ.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using J3PZ.Views;

namespace JX3PZ.Views
{
    /// <summary>
    /// PzMainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PzMainWindow : Window
    {

        public TabItem[] TabItems;

        public readonly PzMainWindowViewModels _VM;

        public readonly PzOverviewWindow PzOverview;

        public PzMainWindow()
        {
            InitializeComponent();
            _VM = new PzMainWindowViewModels();
            DataContext = _VM;
            BindViewModels();
            PzGlobalContext.Views.PzMain = this;

            PzOverview = new PzOverviewWindow();
        }

        public void BindViewModels()
        {
            TabItems = TabControl_Pz.GeTabItems();
            for (int i = 0; i < TabItems.Length; i++)
            {
                TabItems[i].DataContext = _VM.Data[i];
                var tab = TabItems[i].Content as PzTabItem;
                tab?.equipEmbed.ConnectVM(_VM.Data[i].EquipEmbedVM);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;  // cancels the window close
            this.Hide();      // Programmatically hides the window
            PzOverview.Hide();
        }

        private void CopyFinalDPS(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(_VM.FinalDPStxtF);
        }

        public void ShowOverview()
        {
            PzOverview.Show();
            PzOverview.Activate();
        }

        private void PzMainWindow_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                ReadFile(files[0]);
                e.Handled = true;
            }
        }

        public void ReadFile(string filepath)
        {
            _VM.ReadFile(filepath);
        }
    }
}
