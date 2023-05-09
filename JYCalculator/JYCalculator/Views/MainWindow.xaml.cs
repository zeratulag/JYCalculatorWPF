using JX3CalculatorShared.Utils;
using JX3CalculatorShared.Views;
using JX3CalculatorShared.Views.Dialogs;
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
using System.Windows.Controls;
using System.Windows.Input;
using JX3PZ.Views;
using JYCalculator.Globals;

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
            Author_txb.DataContext = _VM;
            BindComboBoxes();
            BindMiJi();
            BindDPS();
            _AboutDialog.DataContext = _AboutVM;
        }


        #region 为复选框填充数据源

        public void BindComboBoxes()
        {
            BindEquipOption();
            BindFightOption();
            BindQiXue();

            BindItemDT();
            BindBigFM();
            BindBuff();

            BindInitChar();
            BindDPS();
            BindOptimization();
        }

        public void BindEquipOption()
        {
            Grid_EquipOption.DataContext = _VM.EquipOptionVM;

            EquipOption_WP_cbb.DataContext = _VM.EquipOptionVM.WPViewModel;

            EquipOption_YZ_cbb.DataContext = _VM.EquipOptionVM.YZViewModel;
        }

        public void BindFightOption()
        {
            Grid_FightOption.DataContext = _VM.FightOptionVM;

            FightOption_Target_cbb.DataContext = _VM.FightOptionVM.TargetViewModel;
            FightOption_Ability_cbb.DataContext = _VM.FightOptionVM.AbilityViewModel;
            FightOption_ZhenFa_cbb.DataContext = _VM.FightOptionVM.ZhenFaViewModel;
        }


        /// <summary>
        /// 批量绑定奇穴下拉框
        /// </summary>
        public void BindQiXue()
        {
            var vm = _VM.QiXueVM;
            Expander_QiXue.DataContext = vm;
        }

        public void BindItemDT()
        {
            GroupBox_ItemDT.DataContext = _VM.ItemDTVM;
        }

        public void BindBigFM()
        {
            ItemsControl_BigFMConfig.DataContext = _VM.BigFMVM;
        }

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

        public void BindBuff()
        {
            Expander_Buff_Self.DataContext = _VM.BuffVM.Buff_Self;
            Expander_Buff_Banquet.DataContext = _VM.BuffVM.Buff_Banquet;
            Expander_Buff_Normal.DataContext = _VM.BuffVM.Buff_Normal;

            Expander_DeBuff_Normal.DataContext = _VM.BuffVM.DeBuff_Normal;

            Expander_Buff_Extra.DataContext = _VM.BuffVM.Buff_Extra;
            Expander_Buff_ExtraStack.DataContext = _VM.BuffVM.Buff_ExtraStack;
        }


        public void BindInitChar()
        {
            Grid_InitChar.DataContext = _VM.InitInputVM;
            InitCharPanelInput.DataContext = _VM.InitChar;
        }

        public void BindDPS()
        {
            ProfitChart.DataContext = _VM.ProfitChartVM;
            ProfitWeight_cbb.DataContext = _VM.ProfitChartVM;
        }

        public void BindOptimization()
        {
            GroupBox_Optimization.DataContext = _VM.OptimizationVM;
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

        private void BtnExportMiJi_Click(object sender, RoutedEventArgs e)
        {
            var res = _VM.SkillMiJiVM.Config;
            Trace.WriteLine(res.PrettyPrint());
        }


        private void BtnOpenDebugWindow_Click(object sender, RoutedEventArgs e)
        {
            OpenDebugWindow();
        }

        public void OpenDebugWindow()
        {
            _DebugMainWindow.Show();
            _DebugMainWindow.Owner = this;
        }

        private void BtnExportAll_Click(object sender, RoutedEventArgs e)
        {
            ExportAll();
        }

        private MainWindowSav ExportAll()
        {
            var res = _VM.Model.Export();
            var json = JsonConvert.SerializeObject(res, Formatting.Indented);
            Trace.WriteLine(json);
            return res;
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

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabItem_MultiZhen.IsSelected)
            {
                Expander_CombatStat.IsExpanded = false;
            }
        }

        private void JYMainWindow_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);
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

        private void OpenPZ_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenPzMainWindow();
        }
    }
}