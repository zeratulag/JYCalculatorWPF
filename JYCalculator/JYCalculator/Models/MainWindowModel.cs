using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Src;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Src;
using JYCalculator.Src.Class;
using JYCalculator.ViewModels;
using MiniExcelLibs;
using MiniExcelLibs.OpenXml;

namespace JYCalculator.Models
{
    public class MainWindowModel : IModel
    {
        #region 成员

        // 输入成员
        public readonly MainWindowViewModels _VMs;

        public readonly AllSkillMiJiConfigViewModel SkillMiJiVM;
        public readonly QiXueConfigViewModel QiXueVM;
        public readonly FightOptionConfigViewModel FightOptionVM;
        public readonly ItemDTConfigViewModel ItemDTVM;
        public readonly AllBuffConfigViewModel BuffVM;

        public readonly InitInputViewModel InitInputVM;
        public readonly InitCharacter InitChar;
        public readonly EquipOptionConfigViewModel EquipOptionVM;

        public readonly FightTimeSummaryViewModel FightTimeSummaryVM;
        public readonly SkillDataDFViewModel SkillDataDFMiJiQiXueVM;

        // 输出成员
        public readonly string WorkDir; // 工作目录

        public readonly Dictionary<string, NamedAttrs> AllAttrsDict;

        public readonly CalculatorShell CalcShell;
        public CalculatorShellArg CalcShellArg;

        public BuffShellInput Shellbuffs;

        public FullCharacterGroup FullCharGroup;

        public readonly List<NamedAttrs> AllAttrs;

        public int CalcShellStatus = 0; // 表示计算是否成功


        #endregion 构建

        public MainWindowModel(MainWindowViewModels vm)
        {
            _VMs = vm;
            SkillMiJiVM = vm.SkillMiJiVM;
            QiXueVM = vm.QiXueVM;
            FightOptionVM = vm.FightOptionVM;
            ItemDTVM = vm.ItemDTVM;
            BuffVM = vm.BuffVM;
            InitInputVM = vm.InitInputVM;
            InitChar = vm.InitChar;
            EquipOptionVM = vm.EquipOptionVM;
            FightTimeSummaryVM = vm.FightTimeSummaryVM;
            SkillDataDFMiJiQiXueVM = vm.SkillDFMiJiQiXueVM;

            CalcShell = new CalculatorShell(this);
            CalcShellArg = new CalculatorShellArg();

            AllAttrsDict = CalcShell.AllAttrsDict;

            AllAttrs = new List<NamedAttrs>(8);

            WorkDir = GetWorkDir();
        }


        public void Calc()
        {
            var res = UpdateCalcShell();
            if (res == 0)
            {
                CalcAllAttrs();
                UpdateResult();
            }
            CalcShellStatus = res;
        }

        protected int UpdateCalcShell()
        {
            Shellbuffs = new BuffShellInput(BuffVM.BuffAttrsDesc, BuffVM.DebuffAttrsDesc, ItemDTVM.AttrsDesc);
            CalcShellArg = new CalculatorShellArg(InitInputVM.BigFM.Arg, SkillMiJiVM.IsSupport);
            CalcShell.UpdateInput(this);
            return CalcShell.Calc();
        }

        protected void CalcAllAttrs()
        {
            AllAttrs.Clear();
            foreach (var KVP in AllAttrsDict)
            {
                if (KVP.Value != null && KVP.Value.Attr != null)
                {
                    AllAttrs.Add(KVP.Value);
                }
            }
        }

        public void UpdateResult()
        {
        }

        #region 方法

        public Dictionary<string, object> GetCalcSheets()
        {
            var res = new Dictionary<string, object>()
            {
                ["人物属性"] = _VMs.AllFullCharacter,
                ["技能属性"] = _VMs.AllSkillTable,
                ["目标属性"] = _VMs.AllTargets,
                ["加速表"] = _VMs.HasteTable,
                ["覆盖率"] = _VMs.CoverTable,
                ["SkillFreqCTsTable"] = _VMs.SkillFreqCTsTable,
                ["常规技能伤害"] = _VMs.NormalSkillDamageTable,
                ["心无技能伤害"] = _VMs.XWSkillDamageTable,
                ["常规技能频率"] = _VMs.NormalSkillFreqCTTable,
                ["心无技能频率"] = _VMs.XWSkillFreqCTTable,
                ["DPSTable"] = _VMs.DPSTable,
                ["战斗统计"] = _VMs.CombatStatTable,
                ["属性收益"] = _VMs.ProfitTable,
            };
            return res;
        }

        public static string GetWorkDir()
        {
            var dir = Directory.GetCurrentDirectory();
            var res = Path.Combine(dir, "Data/");
            return res;
        }

        public string GetCalcSheetsPath()
        {
            var ctime = System.DateTime.Now;
            var timestr = ctime.ToString("yyyy-MM-dd HH-mm-ss");
            var filename = $"JYDEBUG_{timestr}.xlsx";
            var res = Path.Combine(WorkDir, filename);
            return res;
        }

        public void ExportCalcSheets()
        {
            var data = GetCalcSheets();
            var path = GetCalcSheetsPath();

            if (!Directory.Exists(WorkDir))
            {
                Directory.CreateDirectory(WorkDir);
            }

            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch
                {
                    // ignored
                }
            }

            var config = new OpenXmlConfiguration()
            {
                AutoFilter = false
            };

            MiniExcel.SaveAs(path, data, configuration: config);

            Trace.WriteLine($"已导出到{path}！");
        }

        public MainWindowSav Export()
        {
            var res = new MainWindowSav() {
                InitInput = InitInputVM.Export(),
                SkillMiJi = SkillMiJiVM.Export(),
                QiXue = QiXueVM.Export(),
                Buff = BuffVM.Export(),
                ItemDT = ItemDTVM.Export(),
                FightOption = FightOptionVM.Export(),
                AppMeta = Globals.JYAppStatic.CurrentAppMeta,
                SnapFinalDPS = CalcShell.CDPSKernel.FinalDPS,
            };

            return res;
        }

        public void _Load(MainWindowSav sav)
        {
            InitInputVM.Load(sav.InitInput);
            SkillMiJiVM.Load(sav.SkillMiJi);
            QiXueVM.Load(sav.QiXue);
            BuffVM.Load(sav.Buff);
            ItemDTVM.Load(sav.ItemDT);
            FightOptionVM.Load(sav.FightOption);
        }


        #endregion
    }

    // 用于导出的数据格式
    public class MainWindowSav
    {
        public AppMetaInfo AppMeta;

        public double SnapFinalDPS;

        public InitInputSav InitInput;
        public Dictionary<int, string[]> SkillMiJi;
        public int[] QiXue;
        public AllBuffConfigSav Buff;
        public ItemDTConfigSav ItemDT;
        public FightOptionSav FightOption;

    }


}