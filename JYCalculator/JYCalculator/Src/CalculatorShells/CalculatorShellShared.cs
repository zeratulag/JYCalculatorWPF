using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src;
using JX3CalculatorShared.Utils;
using JYCalculator.Class;
using JYCalculator.Data;
using JYCalculator.Models;
using JYCalculator.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MethodTimer;
using JYCalculator.Globals;

namespace JYCalculator.Src
{
    // 计算器的Shell
    public partial class CalculatorShell
    {
        /// <summary>
        /// 基于输入，计算技能信息，技能数和玩家面板
        /// </summary>

        #region 成员

        // 输入成员
        public InitInputViewModel InitInput;
        public EquipOptionConfigModel Equip => InitInput.EquipOptionVM.Model;
        public BigFMConfigModel BigFM => InitInput.BigFMVM.Model;

        public QiXueConfigModel QiXue;

        public Target CTarget; // 当前目标

        public AbilityItem AbilityItem;

        public XFAbility XfAbility;
        public ZhenFa Zhen;

        public FightTimeSummary FightTime;
        public BuffShellInput Buffs; // Buff输入

        public SkillDataDF SkillDFMiJiQiXue; // 仅考虑秘籍和奇穴的技能数据
        public SkillDataDF SkillDF; // 在基础上，考虑装备特效的技能数据
        public Period<SkillDataDF> SkillDFs; // 分为不同阶段的技能数据

        public SkillHasteTable SkillHaste; // 加速表

        public CalculatorShellArg Arg;

        // 中间变量
        public readonly Dictionary<string, NamedAttrs> AllAttrsDict;

        public readonly FullCharacterGroup FullCharGroup; // 收集计算过程中所有的面板

        public int Haste => (int)FullCharGroup.ZhenBuffed.Haste; // 加速
        public int XWExtraHaste => QiXue.XWExtraHaste; // 心无额外加速

        public SkillNumModel SkillNum;

        public DPSKernelShell KernelShell;
        public DPSKernel CDPSKernel => KernelShell.CurrentDPSKernel;


        public string FinalDPStxt;

        public string FinalDPStxtF; // 更加精确的DPS值

        public int CalcStatus { get; private set; } = -1; // 表示计算结果， 0表示计算成功
        public SkillBuild CSkillBuild { get; protected set; } = null;// 流派
        public bool IsSupport { get; protected set; } = false; // 是否支持

        // 优化
        public DPSKernelOptimizer DpsKernelOp; // 
        public MultiZhenFaOptimizer MultiZhenDPSOp; // 多重阵法优化器

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

        [Time]
        public CalculatorShell Calc() // 全流程计算
        {

            GetSkillBuild();
            if (AppStatic.XinFaTag == "TL")
            {
                CalcBoLiangFenXingArg();
            }

            GetSkillDataDF(); // 计算出考虑装备特效后的技能数据
            GetAllAttrsDict(); // 先列出出所有的增益
            CalcZhenBuffedFullChar(); // 计算出带了所有增益（除了自身BUFF）的面板
            CalcBuffedTarget(); // 计算出带Debuff的目标
            CalcHasteTable(); // 计算加速表
            CalcSkillNum(); // 计算出技能频率
            CalcTLSkillDataDFModel(); // 天罗更新特殊奇穴
            CalcFightTime(); // 逐星流更新技能CD
            GetDPSKernelShell();
            CalcDPSKernelShell();
            CollectFChars();

            GetFinalDPS();

            if (Arg.EnableOptimization)
            {
                GetOptimizer();
            }

            if (!IsSupport)
            {
                SetUnSupport();
                CalcStatus = 1;
            }
            else
            {
                CalcStatus = 0;
            }

            return this;
        }

        public void GetSkillBuild()
        {
            // 获取流派

            var validSkillBuild = QiXue.CompatibleSkillBuilds.Intersect(Arg.RecipeCompatibleSkillBuilds).OrderByDescending(_ => _.Priority).ToArray();

            if (validSkillBuild.Length > 0)
            {
                CSkillBuild = validSkillBuild.First();
                IsSupport = true;
            }
            else
            {
                IsSupport = false;
                CSkillBuild = StaticXFData.DB.SkillBuild.Default;
            }

            var skillNumGenre = CSkillBuild.Name;
            if (CSkillBuild.Name == GenreTypeEnum.百步凝形_丝路风语.ToString())
            {
                if (Equip.WP.IsBigCW)
                {
                    skillNumGenre = GenreTypeEnum.百步凝形_丝路风语_橙武.ToString();
                }
            }

            var abilityGenre = StaticXFData.DB.Ability.GenreData[skillNumGenre];
            XfAbility = abilityGenre[AbilityItem.Rank];
        }

        public void GetSkillDataDF()
        {
            SkillDF.AddRecipesAndApply(Equip.OtherRecipes);
            SkillDFs = new Period<SkillDataDF>(SkillDF, SkillDF.Copy());
            if (AppStatic.XinFaTag == "TL")
            {
                CalcTLSkillDataDFModel();
            }
        }

        public void GetAllAttrsDict()
        {
            AllAttrsDict.Clear();
            AllAttrsDict.Add("ZhenFa", Zhen.AttrsDesc);
            AllAttrsDict.Add("Weapon", Equip.WP.AttrsDesc);
            AllAttrsDict.Add("BigFM", InitInput.BigFMAttrsDesc);
            AllAttrsDict.Merge(Buffs.AllAttrsDict);
        }

        public void CalcZhenBuffedFullChar()
        {
            var allBuffs = AttrCollection.Sum(Buffs.Buff.Attr, Buffs.ItemDT.Attr, Equip.WP.SCharAttr,
                InitInput.BigFMAttrsDesc.Attr);
            FullCharGroup.UpdateInput(InitInput.InitCharVM, allBuffs, Zhen.AttrsDesc.Attr);
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
            SkillNum = new SkillNumModel(QiXue, SkillHaste, XfAbility, Equip, BigFM, Zhen.IsOwn, Arg, CSkillBuild);
            SkillNum.Calc();
        }


        public void CalcDPSKernelShell()
        {
            KernelShell.CalcAll();
            KernelShell.CalcProfit();
        }

        public void CollectFChars()
        {
            // 收集展示各种面板
            FullCharGroup.NormalFinal = KernelShell.BuffedFChars.Normal;
            FullCharGroup.BigXWFinal = KernelShell.BuffedFChars.XinWu;
            FullCharGroup.LongXWFinal = KernelShell.LongFChars.XinWu;
            FullCharGroup.ShortXWFinal = KernelShell.ShortFChars.XinWu;
            FullCharGroup.MakeFinalDict();
        }


        public void GetFinalDPS()
        {
            if (CSkillBuild != null)
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
            if (AppStatic.XinFaTag == "JY")
            {
                var ZXXWCD = QiXue.XWCD;
                FightTime.UpdateXWCD(ZXXWCD);
            }
        }

        public void GetOptimizer()
        {
            if (false) // 暂时不需要计算最优属性分配
            {
                DpsKernelOp = new DPSKernelOptimizer(KernelShell);
                DpsKernelOp.Calc();
            }
            MultiZhenDPSOp = new MultiZhenFaOptimizer(KernelShell, Zhen);
            MultiZhenDPSOp.Calc();
        }

        public CalcResult GetCalcResult()
        {
            var res = new CalcResult()
            {
                Success = (CalcStatus == 0),
                FinalDPS = CDPSKernel.FinalDPS,
                ProfitOrderDesc = CDPSKernel.FinalScoreProfit.OrderDesc,
                DamageDeriv = CDPSKernel.FinalProfitDF.PointDeriv
            };
            return res;
        }
    }
}