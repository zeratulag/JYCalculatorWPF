using JX3CalculatorShared.Utils;
using JX3CalculatorShared.Views;
using JX3CalculatorShared.Views.Dialogs;
using JX3PZ.Views;
using JYCalculator.Globals;
using JYCalculator.Models;
using JYCalculator.Src;
using JYCalculator.ViewModels;
using Minimod.PrettyPrint;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace JYCalculator.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 成员

        MainWindowHelper Helper;
        private readonly MainWindowViewModels _VM;
        private readonly DebugMainWindow _DebugMainWindow = null;
        private readonly AboutDialog _AboutDialog = null;
        private readonly AboutDialogViewModel _AboutVM = null;
        public readonly PzMainWindow _PzMainWindow = null;

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            PreProceed();

            Helper = new MainWindowHelper(this);
            _VM = new MainWindowViewModels(this);

            _AboutDialog = new AboutDialog();
            _AboutVM = new AboutDialogViewModel();

            BindViewModels();

            MakeCommands();
            LoadDefault();

            _DebugMainWindow = new DebugMainWindow(_VM.DebugVM);
            _PzMainWindow = new PzMainWindow();

            Show();
            _DebugMainWindow.Owner = this;

#if DEBUG
            _DEBUG();
#endif

            _VM.EnableAutoUpdate();
            GlobalContext.Views.Main = this;

            StartUp(); // 处理文件打开方式
            _VM.DoCalc();
        }

        // 当打开文件方式启动时，需要额外处理
        public void StartUp()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length <= 1)
            {
                return;
            }

            var file = new FileInfo(args[1]);
            if (file.Exists)
            {
                ReadFile(file.FullName);
            }
        }


        public void _DEBUG()
        {
            GroupBox_Debug.Visibility = Visibility.Visible;
            //_DebugMainWindow.Show();
        }

        public void MakeCommands()
        {
        }


        public void PreProceed()
        {
            ConsoleMainProgram.ConsoleMain();
            GroupBox_Debug.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 将View和ViewModel进行绑定
        /// </summary>
        public void BindViewModels()
        {
            JYMainWindow.DataContext = _VM;
            BindMiJi();
            _AboutDialog.DataContext = _AboutVM;
        }

        #region 为复选框填充数据源


        public void BindMiJi()
        {
            foreach (var kvp in _VM.SkillMiJiVM.SkillMiJi)
            {
                var skillKey = kvp.Key;
                var viewModel = kvp.Value;
                var (expanderName, listViewName) = ViewNameTool.GetMiJiElementsName(skillKey);
                Helper.MiJiExpanderDict[expanderName].DataContext = viewModel;
            }
        }
        #endregion

        #region 加载初始值

        // 加载默认奇穴
        public void LoadDefault()
        {
            _VM.LoadDefault();
            _VM.New();
        }

        #endregion

        public void ExportMiJi()
        {
            var res = _VM.SkillMiJiVM.Config;
            Trace.WriteLine(res.PrettyPrint());
        }

        public MainWindowSav ExportAll()
        {
            var res = _VM.Model.Export();
            var json = JsonConvert.SerializeObject(res, Formatting.Indented);
            Trace.WriteLine(json);
            return res;
        }

        public void OpenDebugWindow()
        {
            _DebugMainWindow.Show();
            _DebugMainWindow.Owner = this;
        }




        private void NewCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _VM.New();
        }

        private bool NewCmd_CanExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            return true;
        }

        public void ShowAbout()
        {
            _AboutDialog.Show();
        }


        // 退出时提醒保存
        protected override void OnClosing(CancelEventArgs e)
        {
            var confirmed = _VM.CanExit();
            e.Cancel = !confirmed; // cancels the window close
            if (confirmed)
            {
                Application.Current.Shutdown();
            }
        }

        private void CopyFinalDPS(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(_VM.CalcShell.FinalDPStxtF);
            CopyTextblock_pop.IsOpen = true;
        }

        // 复制TextBlock
        private void CopyTextBlock(object sender, RoutedEventArgs e)
        {
            CommandTool.CopyTextBlock(sender, e);
            CopyTextblock_pop.IsOpen = true;
        }

        private void GroupBox_Profit_MouseDown(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(_VM.ProfitOrderDesc);
            CopyTextblock_pop.IsOpen = true;
        }


        private void MainWindow_OnDrop(object sender, DragEventArgs e)
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

        #region 配装器相关

        public void OpenPzMainWindow()
        {
            _PzMainWindow.Show();
            //_PzMainWindow.Owner = this;
            _PzMainWindow.Activate();
        }

        #endregion

        private void MainWindow_OnDragOver(object sender, DragEventArgs e)
        {
            HandlerTool.CommonOnDragOver(sender, e);
        }
    }
}