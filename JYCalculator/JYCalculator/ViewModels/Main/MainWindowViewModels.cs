using CommunityToolkit.Mvvm.Input;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.Utils;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Class;
using JYCalculator.Models;
using JYCalculator.Src;
using JYCalculator.Src.Class;
using JYCalculator.Src.Data;
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
using JYCalculator.Globals;
using JYCalculator.Views;

namespace JYCalculator.ViewModels
{
    /// <summary>
    /// MainWindow的ViewModel，同时也是存储各个子ViewModels的类
    /// </summary>
    public class MainWindowViewModels : AbsDependViewModel<AbsViewModel>
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
        public readonly CalculatorShell CalcShell;


        // 输出界面的VM
        public ProfitChartViewModel ProfitChartVM { get; set; }
        public OptimizationViewModel OptimizationVM { get; set; }


        protected bool HandlingInputChange = false;

        public const string NewFileName = "Untitled Project";
        public string CurrentFilePath { get; set; }
        public string CurrentFileName { get; set; } = NewFileName;

        public readonly string RawTitle = JYAppStatic.MainTitle;

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
        public RelayCommand ExportCalcSheetsCmd { get; }
        public RelayCommand OpenWorkDirCmd { get; }
        public RelayCommand ImportJBPanelCmd { get; }
        public RelayCommand SaveAsCmd { get; }
        public RelayCommand SaveCurrentCmd { get; }
        public RelayCommand OpenFileCmd { get; }

        public RelayCommand NewCmd { get; }
        public RelayCommand CloseCmd { get; }

        // 帮助命令
        public RelayCommand<string> OpenHelpCmd { get; }
        public RelayCommand ShowAboutCmd { get; }


        //DEBUG界面数据
        public NamedAttrs[] AllAttrs { get; set; }
        public FullCharacter[] AllFullCharacter { get; set; }
        public SkillData[] AllSkillTable { get; set; }
        public Target[] AllTargets { get; set; }
        public HasteTableItem[] HasteTable { get; set; }
        public BuffCoverItem[] CoverTable { get; set; }
        public SkillFreqCTs[] SkillFreqCTsTable { get; set; }
        public SkillDamage[] NormalSkillDamageTable { get; set; }
        public SkillDamage[] XWSkillDamageTable { get; set; }

        public SkillFreqCT[] NormalSkillFreqCTTable { get; set; }
        public SkillFreqCT[] XWSkillFreqCTTable { get; set; }

        public DPSTableItem[] DPSTable { get; set; }
        public JYCombatStatItem[] CombatStatTable { get; set; }
        public JYCombatStatItem[] SimpleCombatStatTable { get; set; }
        public DamageDeriv[] ProfitTable { get; set; }

        public double FinalDPS { get; set; }
        public string FinalDPStxt { get; set; }


        public string ProfitOrderDesc { get; set; }


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

            Model = new MainWindowModel(this);
            CalcShell = Model.CalcShell;

            // 初始化命令
            OpenDebugWindowCmd = new RelayCommand(OpenDebugWindow);
            ExportCalcSheetsCmd = new RelayCommand(ExportCalcSheets);
            OpenWorkDirCmd = new RelayCommand(OpenWorkDir);
            ImportJBPanelCmd = InitInputVM.ImportJBPanelCmd;
            NewCmd = new RelayCommand(New);
            SaveAsCmd = new RelayCommand(SaveAs);
            SaveCurrentCmd = new RelayCommand(SaveCurrent);
            OpenFileCmd = new RelayCommand(OpenFile);
            CloseCmd = new RelayCommand(Exit);

            OpenHelpCmd = new RelayCommand<string>(OpenHelp);
            ShowAboutCmd = new RelayCommand(ShowAbout);

            // 输出结果VM
            ProfitChartVM = new ProfitChartViewModel();
            OptimizationVM = new OptimizationViewModel();

            Proceed();
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
            //InitChar.LoadFromIChar(data.InitChar);
            InitInputVM.ImportJBPanel(data.JBText);
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
            var t = Task.Run(_RealUpdate);
        }

        // 计算更新逻辑，由于耗时可能较长，放在新的线程内。
        protected void _RealUpdate()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Model.Calc();

            FinalDPStxt = CalcShell.FinalDPStxt;
            if (Model.CalcShellStatus == 0)
            {
                UpdateItemsSources();
                UpdateCalcTables();
                UpdateOptimization();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
#if DEBUG
            Trace.WriteLine($"计算总耗时：{elapsedMs}ms");
#endif
        }


        /// <summary>
        /// 强制刷新一些集合属性
        /// </summary>
        protected void UpdateItemsSources()
        {
            AllAttrs = Model.AllAttrs.ToArray();
            AllFullCharacter = CalcShell.FullCharGroup.Dict.Values.ToArray();
            AllSkillTable = CalcShell.SkillDF.Data.Values.ToArray();
            AllTargets = new[] {FightOptionVM.SelectedTarget, CalcShell.CTarget};
            HasteTable = CalcShell.SkillHaste.Dict.Values.ToArray();
            CoverTable = CalcShell.KernelShell.TriggerModel.BuffCover.Data.Values.ToArray();
            SkillFreqCTsTable = CalcShell.KernelShell.SkillFreqCTsArr;
        }

        protected void UpdateCalcTables()
        {
            FinalDPStxt = CalcShell.FinalDPStxt;
            var kernel = CalcShell.CDPSKernel;

            NormalSkillDamageTable = kernel.DamageDFs.Normal.Data.Values.ToArray();
            XWSkillDamageTable = kernel.DamageDFs.XW.Data.Values.ToArray();

            NormalSkillFreqCTTable = kernel.FreqCTDFs.Normal.Data.Values.ToArray();
            XWSkillFreqCTTable = kernel.FreqCTDFs.XW.Data.Values.ToArray();

            DPSTable = kernel.FinalDPSTable.Items;
            CombatStatTable = kernel.FinalCombatStat.Items;
            SimpleCombatStatTable = kernel.SimpleFinalCombatStat.Items;
            FinalDPS = kernel.FinalDPS;

            ProfitTable = kernel.FinalProfitDF.Items;
            ProfitOrderDesc = kernel.FinalScoreProfit.OrderDesc;


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
            //InputChanged -= InputChangedEventHandler;
            _AutoUpdate = false;
        }

        public override void EnableAutoUpdate()
        {
            //InputChanged += InputChangedEventHandler;
            _AutoUpdate = true;
        }

        protected override void _Load<TSave>(TSave sav)
        {
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
            var name = e.PropertyName.RemovePrefix(InternalConstCache.OUTPUT); // 发送者的真实名称
            switch (name)
            {
                case nameof(QiXueConfigViewModel): // 当奇穴修改时，更新对应的自身buff
                {
                    ConnectQiXueBuff();
                    break;
                }

                case nameof(AllSkillMiJiConfigViewModel): // 秘籍修改时更新
                {
                    ConnectSkillMiJiBuff();
                    break;
                }

                case nameof(FightOptionConfigViewModel):
                {
                    ConnectFightTimeBuffCover();
                    break;
                }
            }

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

            ;
        }

        protected void ConnetAllUIs()
        {
            ConnectQiXueBuff();
            ConnectSkillMiJiBuff();
            ConnectFightTimeBuffCover();
        }


        #region 更新方法

        // 设置特殊关联，例如奇穴关联buff

        /// <summary>
        /// 对那些和奇穴关联的BUFF，设置为不可选中，防止玩家误操作
        /// </summary>
        public void ProtectQiXueBuff()
        {
            var allNames = StaticJYData.DB.Buff.QiXueToBuff.Values;
            foreach (var buffName in allNames)
            {
                var vM = BuffVM.Buff_Self.BuffVMDict[buffName];
                vM.IsEnabled = false;
            }
        }

        // 设置奇穴关联buff是否选中
        public void ConnectQiXueBuff()
        {
            var allNames = StaticJYData.DB.Buff.QiXueToBuff.Values;
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

        // 导出过程计算表
        public void ExportCalcSheets()
        {
            Model.ExportCalcSheets();
        }

        // 打开工作目录
        public void OpenWorkDir()
        {
            if (!Directory.Exists(Model.WorkDir))
            {
                Directory.CreateDirectory(Model.WorkDir);
            }

            Process.Start(Model.WorkDir);
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
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "计算方案 (*.jyd)|*.jyd",
                FileName = CurrentFileName,
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

        // 打开

        public void OpenFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "计算方案 (*.jyd)|*.jyd",
                FileName = CurrentFileName,
                AddExtension = true,
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var jsontxt = File.ReadAllText(openFileDialog.FileName);
                (bool success, MainWindowSav sav) = ImportTool.TryDeJSON<MainWindowSav>(jsontxt);
                if (success)
                {
                    CurrentFilePath = openFileDialog.FileName;
                    CurrentFileName = Path.GetFileName(CurrentFilePath);
                    Load(sav);
                    HadSaveAs = true;
                    IsDirty = false;
                    IsNew = false;
                }
                else
                {
                    MessageBox.Show("错误的文件，请重新选择！", "Save error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
            CommandTool.OpenDictUrl(JYAppStatic.URLDict, para);
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
    }
}