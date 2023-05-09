using System;
using CommunityToolkit.Mvvm.Input;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Utils;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Class;
using JYCalculator.Data;
using JYCalculator.Globals;
using JYCalculator.Models;
using JYCalculator.Src;
using JYCalculator.Views;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using CommunityToolkit.Mvvm.Messaging;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Messages;
using JX3PZ.Models;
using JX3PZ.ViewModels;
using JYCalculator.Messages;
using System.Numerics;


namespace JYCalculator.ViewModels
{
    /// <summary>
    /// MainWindow的ViewModel，同时也是存储各个子ViewModels的类
    /// </summary>
    public partial class MainWindowViewModels : AbsDependViewModel<AbsViewModel>, IRecipient<StringMessage>,
        IRecipient<PzPlanMessage>
    {
        #region 成员

        private readonly MainWindow _MW;

        public readonly AllSkillMiJiConfigViewModel SkillMiJiVM;
        public readonly QiXueConfigViewModel QiXueVM;
        public readonly FightOptionConfigViewModel FightOptionVM;
        public readonly ItemDTConfigViewModel ItemDTVM;
        public readonly AllBuffConfigViewModel BuffVM;
        public readonly BigFMConfigViewModel BigFMVM;

        public readonly InitInputViewModel
            InitInputVM; // f(InitCharacter, BigFMConfigViewModel, EquipOptionConfigViewModel)

        public readonly InitCharacter InitChar;
        public readonly EquipOptionConfigViewModel EquipOptionVM;

        public readonly FightTimeSummaryViewModel FightTimeSummaryVM;
        public readonly SkillDataDFViewModel SkillDFMiJiQiXueVM; // 基于秘籍+奇穴的技能数据获取，未考虑装备带的技能修饰

        public readonly MainWindowModel Model;
        public CalculatorShell CalcShell => Model.CalcShell;

        private PzMainWindowViewModels _PZMW => _MW._PzMainWindow._VM;

        // 输出界面的VM
        public ProfitChartViewModel ProfitChartVM { get; set; }
        public OptimizationViewModel OptimizationVM { get; set; }

        // DEBUG界面VM
        public readonly DebugWindowViewModel DebugVM;

        protected bool HandlingInputChange = false;

        public const string NewFileName = "Untitled Project";
        public string CurrentFilePath { get; set; }
        public string CurrentFileName { get; set; } = NewFileName;

        public readonly string RawTitle = XFAppStatic.MainTitle;

        public bool IsPZSyncWithCalc { get; set; } // 是否同步

        public string InitInputModeDesc => IsPZSyncWithCalc ? "配装器模式" : "手动输入模式";

        public string Title
        {
            get
            {
                var res = $"{RawTitle} - {CurrentFileName}";

                if (IsDirty)
                {
                    res += " *";
                }

                return res;
            }
        }

        public bool IsDirty { get; set; } // 是否未保存

        public bool HadSaveAs = false; // 是否已经进行过一次另存为操作;

        public bool IsNew = true; // 是否新建

        // 命令
        public RelayCommand OpenDebugWindowCmd { get; }
        public RelayCommand OpenImportJBBBDialogCmd { get; }
        public RelayCommand SaveAsCmd { get; }
        public RelayCommand SaveCurrentCmd { get; }
        public RelayCommand OpenFileCmd { get; }

        public RelayCommand NewCmd { get; }
        public RelayCommand CloseCmd { get; }

        public RelayCommand OpenPZMWCmd { get; }

        // 帮助命令
        public RelayCommand<string> OpenHelpCmd { get; }
        public RelayCommand ShowAboutCmd { get; }

        // 输出部分
        public DPSTableItem[] DPSTable { get; set; }
        public CombatStatItem[] SimpleCombatStatTable { get; set; }

        public double FinalDPS { get; private set; }
        public string FinalDPStxt { get; private set; }

        public string ProfitOrderDesc { get; private set; }

        public MainWindowViewModels(MainWindow mw) : base(InputPropertyNameType.None)
        {
            _MW = mw;

            SkillMiJiVM = new AllSkillMiJiConfigViewModel();
            QiXueVM = new QiXueConfigViewModel();
            FightOptionVM = new FightOptionConfigViewModel();
            ItemDTVM = new ItemDTConfigViewModel();
            BuffVM = new AllBuffConfigViewModel();
            BigFMVM = new BigFMConfigViewModel();

            InitChar = new InitCharacter("");
            EquipOptionVM = new EquipOptionConfigViewModel();

            InitInputVM = new InitInputViewModel(InitChar, EquipOptionVM, BigFMVM);

            SkillDFMiJiQiXueVM = new SkillDataDFViewModel(SkillMiJiVM, QiXueVM);

            FightTimeSummaryVM = new FightTimeSummaryViewModel(FightOptionVM, QiXueVM);

            var depends = new AbsViewModel[]
            {
                InitInputVM,
                SkillMiJiVM, QiXueVM,
                FightOptionVM,
                ItemDTVM, BuffVM
            };
            SetDependVMs(depends);

            InputChanged -= UpdateAndRefresh;
            InputChanged += InputChangedEventHandler;

            // 注册信息接收处理
            WeakReferenceMessenger.Default.Register<StringMessage>(this);
            WeakReferenceMessenger.Default.Register<PzPlanMessage>(this);

            Model = new MainWindowModel(this);

            // 初始化命令
            OpenDebugWindowCmd = new RelayCommand(OpenDebugWindow);
            OpenImportJBBBDialogCmd = new RelayCommand(OpenImportJBPanelDialog);
            NewCmd = new RelayCommand(New);
            SaveAsCmd = new RelayCommand(SaveAs);
            SaveCurrentCmd = new RelayCommand(SaveCurrent);
            OpenFileCmd = new RelayCommand(OpenFile);
            CloseCmd = new RelayCommand(Exit);

            OpenPZMWCmd = new RelayCommand(OpenPZMW);

            OpenHelpCmd = new RelayCommand<string>(OpenHelp);
            ShowAboutCmd = new RelayCommand(ShowAbout);

            // 输出结果VM
            ProfitChartVM = new ProfitChartViewModel();
            OptimizationVM = new OptimizationViewModel();


            // Debug界面VM
            DebugVM = new DebugWindowViewModel(this);

            GlobalContext.ViewModels.Main = this;

            Proceed();
            _SendMessage = true;
        }

        #endregion

        #region 构造

        /// <summary>
        /// 部分数据需要绑定后才能获取，需要在VM绑定到View之后才能运行
        /// </summary>
        public void PostConstuctor()
        {
        }

        #endregion

        public void Proceed()
        {
            //ProtectQiXueBuff();
        }


        #region 方法

        public ImmutableDictionary<string, SkillMiJiViewModel> MakeSkillMiJiViewModels()
        {
            var res = SkillMiJiViewModel.MakeViewModels();
            return (ImmutableDictionary<string, SkillMiJiViewModel>) res;
        }

        protected void _Load(CalcData data)
        {
            QiXueVM.Load(data.QiXueConfig);
            SkillMiJiVM.Load(data.SkillMiJiConfig);
            InitInputVM.ImportJBBB(data.JBPz);
        }

        public void Load(CalcData data)
        {
            ActionUpdateOnce(_Load, data);
        }

        public void LoadDefault()
        {
            var sample = CalcData.GetSample();
            Load(sample);
        }

        #endregion

        protected override void _Update()
        {
            _RealUpdate();
        }

        // 计算更新逻辑，由于耗时可能较长，放在新的线程内。
        protected async void _RealUpdate()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            await Task.Run(Model.Calc);
            UpdateResults();

            if (_SendMessage)
            {
                SendResultMessage();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
#if DEBUG
            Trace.WriteLine($"计算总耗时：{elapsedMs}ms");
#endif
        }

        protected void UpdateResults()
        {
            FinalDPStxt = CalcShell.FinalDPStxt;
            if (Model.CalcShellStatus == 0)
            {
                UpdateResultTables();
                UpdateOptimization();
            }

            DebugVM.Update(); // 同步到Debug界面
        }

        protected void SendResultMessage()
        {
            if (GlobalContext.IsPZSyncWithCalc)
            {
                var msg = GetCalcResultMessage();
                WeakReferenceMessenger.Default.Send(msg);
            }
        }

        private CalcResultMessage GetCalcResultMessage()
        {
            var calcResult = Model.CalcShell.GetCalcResult();
            var calcVM = new CalcInputViewModel(QiXueVM.SelectedQiXueSkills,
                ItemDTVM.ValidItemDts.ToArray(), FightOptionVM.Summary, BuffVM.ValidBuffViewModels);
            var res = new CalcResultMessage(calcResult, calcVM);
            return res;
        }


        protected void UpdateResultTables()
        {
            FinalDPStxt = CalcShell.FinalDPStxt;
            var kernel = CalcShell.CDPSKernel;

            SimpleCombatStatTable = kernel.SimpleFinalCombatStat.Items;
            FinalDPS = kernel.FinalDPS;
            ProfitOrderDesc = kernel.FinalScoreProfit.OrderDesc;
            DPSTable = kernel.FinalDPSTable.Items;

            ProfitChartVM.UpdateSourceDF(kernel.FinalProfitDF);
        }


        protected void UpdateOptimization()
        {
            OptimizationVM.UpdateSource(CalcShell);
        }


        /// <summary>
        /// 计算出一共有哪些秘籍
        /// </summary>
        public void CalcRecipes()
        {
            var allrecipes = new List<Recipe>();
            allrecipes.AddRange(EquipOptionVM.EquipRecipes);
            allrecipes.AddRange(QiXueVM.Model.OtherRecipes);
        }


        public override void DisableAutoUpdate()
        {
            _AutoUpdate = false;
        }

        public override void EnableAutoUpdate()
        {
            _AutoUpdate = true;
        }

        protected void _Load(MainWindowSav sav)
        {
            Model._Load(sav);
        }

        public void Load(MainWindowSav sav)
        {
            ActionUpdateOnce(_Load, sav);
        }


        protected override void _RefreshCommands()
        {
        }

        /// <summary>
        /// 用于处理输出改变时导致的按需改变设置，接管常规的更新操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InputChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            if (HandlingInputChange) return; // 防止多次触发事件
            HandlingInputChange = true;
            var autoUpdate = _AutoUpdate; // 如果处于自动更新状态则关闭
            DisableAutoUpdate();
            //var name = e.PropertyName.RemovePrefix(InternalConstCache.OUTPUT); // 发送者的真实名称

            HandlingInputChange = false;

            if (IsNew)
            {
                if (InitChar.Name == null || InitChar.Name.IsEmptyOrWhiteSpace())
                {
                }
                else
                {
                    CurrentFileName = StringTool.GetSafeFilename(InitChar.Name); // 若当前为新建文件，并且尚未指定文件名，则同步JB的配装方案名
                }
            }

            IsDirty = true;
            if (autoUpdate)
            {
                EnableAutoUpdate();
                UpdateAndRefresh();
            }
        }


        #region 更新方法

        /// <summary>
        /// 处理信息接收事件
        /// </summary>
        /// <param name="message"></param>
        protected void _Receive(StringMessage message)
        {
            switch (message.Value)
            {
                case StaticMessager.Senders.QiXue:
                {
                    ConnectQiXueBuff();
                    break;
                }

                case StaticMessager.Senders.MiJi:
                case StaticMessager.Senders.BaoYuMiJi:
                {
                    ConnectSkillMiJiBuff();
                    break;
                }

                case StaticMessager.Senders.FightTime:
                {
                    ConnectFightTimeBuffCover();
                    break;
                }
            }
        }

        public void Receive(StringMessage message)
        {
            ActionWithOutUpdate(_Receive, message);
        }

        /// <summary>
        /// 对那些和奇穴关联的BUFF，设置为不可选中，防止玩家误操作
        /// </summary>
        public void ProtectQiXueBuff()
        {
            var allNames = StaticXFData.DB.Buff.QiXueToBuff.Values;
            foreach (var buffName in allNames)
            {
                var vM = BuffVM.Buff_Self.BuffVMDict[buffName];
                vM.IsEnabled = false;
            }
        }

        // 设置奇穴关联buff是否选中
        public void ConnectQiXueBuff()
        {
            var allNames = StaticXFData.DB.Buff.QiXueToBuff.Values;
            foreach (var buffName in allNames)
            {
                var vM = BuffVM.Buff_Self.BuffVMDict[buffName];
                var has = QiXueVM.Model.SelfBuffNames.Contains(buffName);
                vM.IsEnabled = has;
                vM.IsChecked = has;
            }
        }

        #endregion

        public void ConnectSkillMiJiBuff()
        {
            var QiPo = BuffVM.Buff_Self.BuffVMDict["QiPo"];
            var has = SkillMiJiVM.HasQiPo;
            QiPo.IsEnabled = has;
            QiPo.IsChecked = has;
        }

        // 战斗时间与覆盖率的关联
        public void ConnectFightTimeBuffCover()
        {
            var HaoLing = BuffVM.Buff_ExtraStack.BuffVMDict["HaoLingSanJun"];
            HaoLing.Cover = 60.0 / FightOptionVM.FightTime; // 号令三军覆盖率与战斗时间挂钩
        }

        #region 命令

        // 打开Debug窗口
        public void OpenDebugWindow()
        {
            _MW.OpenDebugWindow();
        }


        public void OpenImportJBPanelDialog()
        {
            InitInputVM.OpenImportJBBBDialog();
        }

        #endregion

        #region 导入导出文件操作

        // 导出全部配置

        public void New()
        {
            IsNew = true;
            IsDirty = false;
            HadSaveAs = false;
            CurrentFileName = NewFileName;
            CurrentFilePath = null;
        }

        public void SaveToFile(string filename)
        {
            var sav = Model.Export();
            var json = JsonConvert.SerializeObject(sav, Formatting.Indented);
            File.WriteAllText(filename, json, Encoding.UTF8);
            IsDirty = false;
            IsNew = false;
            InitInputVM.ClearJBTitle();
        }

        // 保存当前
        public void SaveCurrent()
        {
            if (HadSaveAs)
            {
                SaveToFile(CurrentFilePath);
            }
            else
            {
                SaveAs();
            }
        }

        // 另存为
        public void SaveAs()
        {
            var filename = InitInputVM.JBPanelTitle ?? CurrentFileName; // 如果从JB导入了不同的文件名

            var saveFileDialog = new SaveFileDialog
            {
                Filter = XFAppStatic.FileFilter,
                FileName = filename,
                AddExtension = true,
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                CurrentFilePath = saveFileDialog.FileName;
                CurrentFileName = Path.GetFileName(CurrentFilePath);
                SaveToFile(saveFileDialog.FileName);
                HadSaveAs = true;
            }
        }

        // 读取文件
        public void ReadFile(string filepath)
        {
            var sav = ReadFileAsSav(filepath);
            if (sav != null)
            {
                CurrentFilePath = filepath;
                CurrentFileName = Path.GetFileName(CurrentFilePath);
                Load(sav);
                HadSaveAs = true;
                IsDirty = false;
                IsNew = false;
                InitInputVM.ClearJBTitle();
            }
            else
            {
                MessageBox.Show("错误的文件，请重新选择！", "Save error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 读取文件
        public MainWindowSav ReadFileAsSav(string filepath)
        {
            var jsontxt = File.ReadAllText(filepath);
            (bool success, MainWindowSav sav) = ImportTool.TryDeJSON<MainWindowSav>(jsontxt);
            if (success)
            {
                return sav;
            }
            else
            {
                return null;
            }
        }


        // 打开文件（对话框）
        public MainWindowSav OpenFileAsSav(bool syncCurrentFilePath = false)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = XFAppStatic.FileFilter,
                FileName = CurrentFileName,
                AddExtension = true,
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var res = ReadFileAsSav(openFileDialog.FileName);
                if (syncCurrentFilePath) CurrentFilePath = openFileDialog.FileName;
                return res;
            }

            return null;
        }

        // 打开文件（对话框）
        public void OpenFile()
        {
            var sav = OpenFileAsSav(true);
            if (sav != null)
            {
                Load(sav);
                HadSaveAs = true;
                IsDirty = false;
                IsNew = false;
                InitInputVM.ClearJBTitle();
            }
        }


        // 退出

        [RelayCommand]
        public void Exit()
        {
            App.Current.MainWindow.Close();
        }

        #endregion

        #region 帮助菜单栏

        public void OpenHelp(string para)
        {
            CommandTool.OpenDictUrl(XFAppStatic.URLDict, para);
        }


        public void ShowAbout()
        {
            _MW.ShowAbout();
        }

        #endregion

        /// <summary>
        /// 当未保存时退出时，提醒保存
        /// </summary>
        /// <returns>是否确认退出</returns>
        protected bool _DirtyExit()
        {
            bool canExit = true;
            var msgResult = MessageBox.Show("当前计算方案尚未保存，是否立刻保存？", "Calculator", MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            switch (msgResult)
            {
                case MessageBoxResult.Yes:
                {
                    SaveCurrent();
                    canExit = true; // 完成保存，正常退出
                    break;
                }
                case MessageBoxResult.No:
                {
                    canExit = true; // 不保存直接退出
                    break;
                }
                case MessageBoxResult.Cancel:
                {
                    canExit = false; // 不再退出
                    break;
                }
            }

            return canExit;
        }

        // 是否可以退出
        public bool CanExit()
        {
            var res = true;
            if (IsDirty)
            {
                res = _DirtyExit();
            }

            return res;
        }


        #region 配装器页面

        public void OpenPZMW()
        {
            _MW.OpenPzMainWindow();
        }

        #endregion

        public void Receive(PzPlanMessage message)
        {
            ImportPzPlan(message.Plan);
            CurrentFileName = message.Title;
        }

        public void ImportPzPlan(PzPlanModel plan)
        {
            var adapter = new PzToXinFaInputAdapter(plan);
            adapter.Calc();
            InitInputVM.Load(adapter.InputSav);
            GlobalContext.IsPZSyncWithCalc = true;
        }
    }
}