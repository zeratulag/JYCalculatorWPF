using JX3CalculatorShared.Class;
using JX3CalculatorShared.Src;
using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.Src.Data;
using JX3CalculatorShared.Utils;
using JYCalculator.Class;
using JYCalculator.Models;
using JYCalculator.Src.Class;
using JYCalculator.Src.Data;
using JYCalculator.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using JX3CalculatorShared.ViewModels;

namespace JYCalculator.Src
{
    // 计算器的Shell
    public class CalculatorShell
    {
        /// <summary>
        /// 基于输入，计算技能信息，技能数和玩家面板
        /// </summary>

        #region 成员

        // 输入成员
        public InitInputViewModel InitInput;

        public EquipOptionConfigModel Equip => InitInput.EquipOption.Model;
        public BigFMConfigModel BigFM => InitInput.BigFM.Model;


        public QiXueConfigModel QiXue;

        public Target CTarget; // 当前目标

        public AbilityItem AbilityItem;

        public JYAbility JyAbility;
        public ZhenFa Zhen;

        public FightTimeSummary FightTime;
        public BuffShellInput Buffs; // Buff输入

        public SkillDataDF SkillDFMiJiQiXue; // 仅考虑秘籍和奇穴的技能数据
        public SkillDataDF SkillDF; // 在基础上，考虑装备特效的技能数据

        public SkillHasteTable SkillHaste; // 加速表

        public CalculatorShellArg Arg;

        // 中间变量
        public readonly Dictionary<string, NamedAttrs> AllAttrsDict;

        public readonly FullCharacterGroup FullCharGroup; // 收集计算过程中所有的面板

        public int HS => (int) FullCharGroup.ZhenBuffed.HS; // 加速
        public int XWExtraSP => QiXue.XWExtraSP; // 心无额外加速

        public SkillNumModel SkillNum;

        public DPSKernelShell KernelShell;
        public DPSKernel CDPSKernel => KernelShell.CurrentDPSKernel;

        public bool IsSupport => QiXue.IsSupport & Arg.AllSkillMiJiIsSupport;

        public string FinalDPStxt;

        public string FinalDPStxtF; // 更加精确的DPS值

        #endregion

        #region 构造

        public CalculatorShell()
        {
            AllAttrsDict = new Dictionary<string, NamedAttrs>(6);
            FullCharGroup = new FullCharacterGroup();
        }

        public CalculatorShell(InitInputViewModel init,
            QiXueConfigModel qixue,
            Target target, AbilityItem abilityitem, ZhenFa zhen,
            FightTimeSummary fightTime,
            BuffShellInput buffs,
            SkillDataDF skillDataDfmijiqixue,
            CalculatorShellArg arg) : this()
        {
            UpdateInput(init,
                qixue,
                target, abilityitem, zhen,
                fightTime,
                buffs, skillDataDfmijiqixue, arg);
        }

        public CalculatorShell(MainWindowModel model) : this()
        {
            UpdateInput(model);
        }

        public void UpdateInput(InitInputViewModel init,
            QiXueConfigModel qixue,
            Target target, AbilityItem abilityitem, ZhenFa zhen,
            FightTimeSummary fightTime,
            BuffShellInput buffs, SkillDataDF skillDataDfmijiqixue, CalculatorShellArg arg)
        {
            InitInput = init;
            QiXue = qixue;

            CTarget = target?.Copy(); // 复制一份，防止意外修改
            CTarget?.SetDescName($"(带BUFF){CTarget.DescName}");

            AbilityItem = abilityitem;
            Zhen = zhen;
            FightTime = fightTime;

            Buffs = buffs;
            SkillDFMiJiQiXue = skillDataDfmijiqixue;
            SkillDF = SkillDFMiJiQiXue.Copy();

            SkillHaste = new SkillHasteTable(SkillDF);
            Arg = arg;
        }

        #endregion

        public void UpdateInput(MainWindowModel model)
        {
            UpdateInput(model.InitInputVM,
                model.QiXueVM.Model,
                model.FightOptionVM.SelectedTarget, model.FightOptionVM.SelectedAbility,
                model.FightOptionVM.SelectedZhenFa,
                model.FightTimeSummaryVM.Model,
                model.Shellbuffs, model.SkillDataDFMiJiQiXueVM.Model, model.CalcShellArg);
        }

        public int Calc() // 全流程计算
        {
            if (!IsSupport)
            {
                SetUnSupport();
                return 1;
            }

            var watch = System.Diagnostics.Stopwatch.StartNew();

            GetAbilityGenre();
            GetSkillDataDF(); // 计算出考虑装备特效后的技能数据
            GetAllAttrsDict(); // 先列出出所有的增益
            CalcZhenBuffedFullChar(); // 计算出带了所有增益（除了自身BUFF）的面板
            CalcBuffedTarget(); // 计算出带Debuff的目标
            CalcHasteTable(); // 计算加速表
            CalcSkillNum(); // 计算出技能频率
            CalcFightTime(); // 逐星流更新技能CD
            GetDPSKernelShell();
            CalcDPSKernelShell();
            CollectFChars();

            GetFinalDPS();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
#if DEBUG
            Trace.WriteLine($"计算DPS耗时：{elapsedMs}ms");
#endif
            return 0;
        }

        public void GetSkillDataDF()
        {
            SkillDF.AddRecipesAndApply(Equip.OtherRecipes);
        }

        public void GetAllAttrsDict()
        {
            AllAttrsDict.Clear();
            AllAttrsDict.Add("ZhenFa", Zhen.AttrsDesc);
            AllAttrsDict.Add("WP", Equip.WP.AttrsDesc);
            AllAttrsDict.Add("BigFM", InitInput.BigFMAttrsDesc);
            AllAttrsDict.Merge(Buffs.AllAttrsDict);
        }

        public void CalcZhenBuffedFullChar()
        {
            var allBuffs = AttrCollection.Sum(Buffs.Buff.Attr, Buffs.ItemDT.Attr, Equip.WP.SCharAttr,
                InitInput.BigFMAttrsDesc.Attr);
            FullCharGroup.UpdateInput(InitInput.InitChar, allBuffs, Zhen.AttrsDesc.Attr);
            FullCharGroup.CalcZhenBuffed();
        }

        public void CalcBuffedTarget()
        {
            CTarget.AddNamedAttrs(Buffs.DeBuff);
        }

        public void CalcHasteTable()
        {
            SkillHaste.UpdateHSP(this);
        }

        public void CalcSkillNum()
        {
            SkillNum = new SkillNumModel(QiXue, SkillHaste, JyAbility, Equip, BigFM, Zhen.IsOwn, Arg);
            SkillNum.Calc();
        }


        public void GetDPSKernelShell()
        {
            var arg = new DPSCalcShellArg(Equip.SL, QiXue.聚精凝神, Equip.YZ, BigFM);
            KernelShell = new DPSKernelShell(FullCharGroup.ZhenBuffed, CTarget, SkillDF, SkillNum, FightTime, arg);
        }

        public void CalcDPSKernelShell()
        {
            KernelShell.CalcAll();
            KernelShell.CalcProfit();
        }

        // 收集展示各种面板
        public void CollectFChars()
        {
            FullCharGroup.NormalFinal = KernelShell.BuffedFChars.Normal;
            FullCharGroup.BigXWFinal = KernelShell.BuffedFChars.XW;
            FullCharGroup.LongXWFinal = KernelShell.LongFChars.XW;
            FullCharGroup.ShortXWFinal = KernelShell.ShortFChars.XW;
            FullCharGroup.MakeFinalDict();
        }

        // 获取流派
        public void GetAbilityGenre()
        {
            var genere = StaticJYData.DB.Ability.GenreData[QiXue.SkillBaseNumGenre];
            JyAbility = genere[AbilityItem.Rank];
        }

        public void GetFinalDPS()
        {
            if (QiXue.IsSupport)
            {
                FinalDPStxt = $"{CDPSKernel.FinalDPS:F0}";
                FinalDPStxtF = $"{CDPSKernel.FinalDPS:F2}";
            }
            else
            {
                SetUnSupport();
            }
        }

        /// <summary>
        /// 设置奇穴秘籍不支持，不再计算DPS
        /// </summary>
        public void SetUnSupport()
        {
            FinalDPStxt = "不支持的奇穴/秘籍！";
            FinalDPStxtF = FinalDPStxt;
        }

        public void CalcFightTime()
        {
            var ZXXWCD = QiXue.XWCD;
            FightTime.UpdateXWCD(ZXXWCD);
        }

    }


    /// <summary>
    /// 存储一些需要传入的参数
    /// </summary>
    public class CalculatorShellArg
    {
        public readonly BigFMConfigArg BigFM;
        public readonly bool AllSkillMiJiIsSupport; // 秘籍是否支持

        public CalculatorShellArg(BigFMConfigArg bigFM, bool allSkillMiJiIsSupport)
        {
            BigFM = bigFM;
            AllSkillMiJiIsSupport = allSkillMiJiIsSupport;
        }

        public CalculatorShellArg()
        {
        }
    }
}