using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JX3CalculatorShared.Class;
using JYCalculator.Class;
using JYCalculator.Globals;
using JYCalculator.Models;
using MiniExcelLibs;
using MiniExcelLibs.OpenXml;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JYCalculator.Data;
using Serilog;

namespace JYCalculator.ViewModels
{
    public partial class DebugWindowViewModel : ObservableObject
    {
        #region 成员

        public readonly MainWindowViewModels _MVMs;
        public readonly MainWindowModel Model;

        // DEBUG界面数据
        public NamedAttrs[] AllAttrs { get; set; }
        public FullCharacter[] AllFullCharacter { get; set; }
        public IEnumerable<SkillInfoItem> SkillInfoTable { get; set; }
        public SkillData[] NormalSkillTable { get; set; }
        public SkillData[] XWSkillTable { get; set; }
        public Target[] AllTargets { get; set; }
        public HasteTableItem[] HasteTable { get; set; }
        public CoverItem[] CoverTable { get; set; }
        public SkillFreqCTs[] SkillFreqCTsTable { get; set; }
        public SkillDamage[] NormalSkillDamageTable { get; set; }
        public SkillDamage[] XWSkillDamageTable { get; set; }

        public SkillFreqCriticalStrike[] NormalSkillFreqCTTable { get; set; }
        public SkillFreqCriticalStrike[] XWSkillFreqCTTable { get; set; }

        public DPSTableItem[] DPSTable { get; set; }
        public CombatStatItem[] CombatStatTable { get; set; }
        public CombatStatItem[] CombatStatGroupTable { get; set; }
        public DamageDeriv[] ProfitTable { get; set; }

        // 命令
        public RelayCommand ExportCalcSheetsCmd { get; }
        public RelayCommand OpenWorkDirCmd { get; }

        public readonly string WorkDir; // 工作目录

        #endregion

        #region 构造

        public DebugWindowViewModel(MainWindowViewModels mVMs)
        {
            _MVMs = mVMs;
            Model = _MVMs.Model;

            // 命令
            ExportCalcSheetsCmd = new RelayCommand(ExportCalcSheets);
            OpenWorkDirCmd = new RelayCommand(OpenWorkDir);

            WorkDir = GetWorkDir();
            GlobalContext.ViewModels.Debug = this;
        }

        #endregion


        public void Update()
        {
            UpdateItemsSources();
            UpdateCalcTables();
        }

        protected void UpdateItemsSources()
        {
            AllAttrs = _MVMs.Model.AllAttrs.ToArray();
            AllFullCharacter = _MVMs.CalcShell.FullCharGroup.Dict.Values.ToArray();
            SkillInfoTable = StaticXFData.Data.AllSkillInfoItem;
            NormalSkillTable = _MVMs.CalcShell.SkillDFs.Normal.Data.Values.ToArray();
            XWSkillTable = _MVMs.CalcShell.SkillDFs.XinWu.Data.Values.ToArray();
            AllTargets = new[] { _MVMs.FightOptionVM.SelectedTarget, _MVMs.CalcShell.CTarget };
            HasteTable = _MVMs.CalcShell.SkillHaste.Dict.Values.ToArray();
            SkillFreqCTsTable = _MVMs.CalcShell.KernelShell.SkillFreqCTsArr;
            GetCoverTable();
        }

        protected void UpdateCalcTables()
        {
            var kernel = _MVMs.CalcShell.CDPSKernel;

            NormalSkillDamageTable = kernel.DamageDFs.Normal.Data.Values.ToArray();
            XWSkillDamageTable = kernel.DamageDFs.XinWu.Data.Values.ToArray();

            NormalSkillFreqCTTable = kernel.FreqCTDFs.Normal.Data.Values.ToArray();
            XWSkillFreqCTTable = kernel.FreqCTDFs.XinWu.Data.Values.ToArray();

            DPSTable = _MVMs.DPSTable;
            CombatStatTable = kernel.FinalCombatStat.Items;
            CombatStatGroupTable = kernel.FinalCombatStat.Groups;
            ProfitTable = kernel.FinalProfitDF.Items;
        }


        public static string GetWorkDir()
        {
            var dir = Directory.GetCurrentDirectory();
            var res = Path.Combine(dir, "Data/");
            return res;
        }

        public void OpenWorkDir()
        {
            // 打开工作目录
            if (!Directory.Exists(WorkDir))
            {
                Directory.CreateDirectory(WorkDir);
            }
            Process.Start(WorkDir);
        }


        public Dictionary<string, object> GetCalcSheets()
        {
            var res = new Dictionary<string, object>()
            {
                ["人物属性"] = AllFullCharacter,
                ["技能信息"] = SkillInfoTable,
                ["技能属性"] = NormalSkillTable,
                ["目标属性"] = AllTargets,
                ["加速表"] = HasteTable,
                ["覆盖率"] = CoverTable,
                ["SkillFreqCTsTable"] = SkillFreqCTsTable,
                ["常规技能伤害"] = NormalSkillDamageTable,
                ["心无技能伤害"] = XWSkillDamageTable,
                ["常规技能频率"] = NormalSkillFreqCTTable,
                ["心无技能频率"] = XWSkillFreqCTTable,
                ["DPSTable"] = DPSTable,
                ["战斗统计"] = CombatStatTable,
                ["属性收益"] = ProfitTable,
            };
            return res;
        }

        public string GetCalcSheetsPath()
        {
            var ctime = System.DateTime.Now;
            var timestr = ctime.ToString("yyyy-MM-dd HH-mm-ss");
            var filename = $"{XFAppStatic.XinFaTag}DEBUG_{timestr}.xlsx";
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

            Log.Information($"已导出到{path}！");
        }
    }
}