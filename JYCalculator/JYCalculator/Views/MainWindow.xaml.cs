using Minimod.PrettyPrint;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using JX3CalculatorShared.Views;
using JYCalculator.Models;
using JYCalculator.Src;
using JYCalculator.Views;
using JYCalculator.ViewModels;
using JYCalculator.Views.Dialogs;
using Newtonsoft.Json;
using System.ComponentModel;

namespace JYCalculator
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 成员

        MainWindowHelper Helper;
        private readonly MainWindowViewModels _VMs;

        private readonly DebugMainWindow _DebugMainWindow = null;

        private readonly AboutDialog _AboutDialog = null;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            PreProceed();

            Helper = new MainWindowHelper(this);
            _VMs = new MainWindowViewModels(this);

            _AboutDialog = new AboutDialog();

            BindViewModels();

            MakeCommands();
            LoadDefault();

#if DEBUG
            _DebugMainWindow = new DebugMainWindow(_VMs);
            Show();
            _DebugMainWindow.Owner = this;
            _DEBUG();
#endif

            _VMs.EnableAutoUpdate();

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
            JYMainWindow.DataContext = _VMs;
            Author_txb.DataContext = _VMs;
            BindComboBoxes();
            BindMiJi();
            BindDPS();
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
        }

        public void BindEquipOption()
        {
            Grid_EquipOption.DataContext = _VMs.EquipOptionVM;

            EquipOption_WP_cbb.DataContext = _VMs.EquipOptionVM.WPViewModel;

            EquipOption_YZ_cbb.DataContext = _VMs.EquipOptionVM.YZViewModel;
        }

        public void BindFightOption()
        {
            Grid_FightOption.DataContext = _VMs.FightOptionVM;

            FightOption_Target_cbb.DataContext = _VMs.FightOptionVM.TargetViewModel;
            FightOption_Ability_cbb.DataContext = _VMs.FightOptionVM.AbilityViewModel;
            FightOption_ZhenFa_cbb.DataContext = _VMs.FightOptionVM.ZhenFaViewModel;
        }


        /// <summary>
        /// 批量绑定奇穴下拉框
        /// </summary>
        public void BindQiXue()
        {
            var vm = _VMs.QiXueVM;
            Expander_QiXue.DataContext = vm;
        }

        public void BindItemDT()
        {
            GroupBox_ItemDT.DataContext = _VMs.ItemDTVM;
        }

        public void BindBigFM()
        {
            ItemsControl_BigFMConfig.DataContext = _VMs.BigFMVM;
        }

        public void BindMiJi()
        {
            foreach (var kvp in _VMs.SkillMiJiVM.SkillMiJi)
            {
                var skillKey = kvp.Key;
                var viewModel = kvp.Value;
                var (expanderName, listViewName) = ViewNameTool.GetMiJiElementsName(skillKey);

                Helper.MiJiExpanderDict[expanderName].DataContext = viewModel;
            }
        }

        public void BindBuff()
        {
            Expander_Buff_Self.DataContext = _VMs.BuffVM.Buff_Self;
            Expander_Buff_Banquet.DataContext = _VMs.BuffVM.Buff_Banquet;
            Expander_Buff_Normal.DataContext = _VMs.BuffVM.Buff_Normal;

            Expander_DeBuff_Normal.DataContext = _VMs.BuffVM.DeBuff_Normal;

            Expander_Buff_Extra.DataContext = _VMs.BuffVM.Buff_Extra;
            Expander_Buff_ExtraStack.DataContext = _VMs.BuffVM.Buff_ExtraStack;
        }


        public void BindInitChar()
        {
            TabItem_InitCharInput.DataContext = _VMs.InitInputVM;
            InitCharPanelInput.DataContext = _VMs.InitChar;
        }

        public void BindDPS()
        {
            ProfitChart.DataContext = _VMs.ProfitChartVM;
            ProfitWeight_cbb.DataContext = _VMs.ProfitChartVM;
        }


        #endregion

        #region 加载初始值

        // 加载默认奇穴
        public void LoadDefault()
        {
            _VMs.LoadDefault();
            _VMs.New();
        }

        #endregion

        private void BtnExportMiJi_Click(object sender, RoutedEventArgs e)
        {
            var res = _VMs.SkillMiJiVM.Config;
            Trace.WriteLine(res.PrettyPrint());
        }


        private void BtnOpenDebugWindow_Click(object sender, RoutedEventArgs e)
        {
            OpenDebugWindow();
        }

        public void OpenDebugWindow()
        {
            _DebugMainWindow.Show();
        }

        private void BtnExportAll_Click(object sender, RoutedEventArgs e)
        {
            ExportAll();
        }

        private MainWindowSav ExportAll()
        {
            var res = _VMs.Model.Export();
            var json = JsonConvert.SerializeObject(res, Formatting.Indented);
            Trace.WriteLine(json);
            return res;
        }


        private void NewCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _VMs.New();
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
            var confirmed = _VMs.CanExit();
            e.Cancel = !confirmed;  // cancels the window close
        }

    }
}