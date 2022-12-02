using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Src.Class;
using JYCalculator.Class;
using JYCalculator.Models;
using JYCalculator.Src.Class;
using Microsoft.SqlServer.Server;

namespace JYCalculator.Src
{
    public class DPSKernel
    {
        #region 成员

        public readonly Period<FullCharacter> FinalFChars; // 最终输入的面板，计算了所有BUFF
        public readonly Target CTarget;

        public readonly Period<SkillDataDF> DataDfs;

        public readonly Period<SkillFreqCTDF> FreqCTDFs;

        public readonly FightTimeSummaryItem FightTime;

        public readonly DPSKernelArg Arg;

        // 以下属性由计算得到

        public FullCharacter SnapFChar;

        public Period<SkillDamageDF> DamageDFs;

        public Period<SkillDamageFreqDF> DamageFreqDFs;

        public Period<CombatStat> CombatStats;

        public CombatStat FinalCombatStat;
        public CombatStat SimpleFinalCombatStat; // 简化的，模拟游戏中的同名技能合并


        public bool HasCalculated = false; // 是否已经计算过

        public DPSTable FinalDPSTable;

        public double FinalDPS;

        public Period<DamageDeriv> DamageDerivs; // 记录不同时期的属性收益导数
        public DamageDeriv RawDeriv; // 最终导数收益（未修复会心额外收益）
        public DamageDeriv FinalProfit; // 最终导数收益
        public DamageDeriv FinalScoreProfit; // 最终单分收益
        public ProfitDF FinalProfitDF; // 最终收益总结
        #endregion

        #region 构造

        public DPSKernel(Period<FullCharacter> finalFChars, Target target, Period<SkillDataDF> datas,
            Period<SkillFreqCTDF> freqs, FightTimeSummaryItem fightTime, DPSKernelArg arg)
        {
            FinalFChars = finalFChars;
            CTarget = target;
            DataDfs = datas;
            FreqCTDFs = freqs;
            FightTime = fightTime;

            Arg = arg;
            SnapFChar = FinalFChars.Normal.GetBurstSnapShot(FinalFChars.XW);
        }

        #endregion

        #region 方法

        public void Calc()
        {
            CalcDamage();
            CalcDamageFreq();
            CalcDPS();
            CalcCombatStat();
            HasCalculated = true;
        }

        public void CalcDamage()
        {
            var normal = new SkillDamageDF(DataDfs.Normal, FinalFChars.Normal, SnapFChar, CTarget);
            normal.GetDamage();
            var xw = new SkillDamageDF(DataDfs.XW, FinalFChars.XW, FinalFChars.XW, CTarget);
            xw.GetDamage();
            DamageDFs = new Period<SkillDamageDF>(normal, xw);
        }

        public void CalcDamageFreq()
        {
            var normal = new SkillDamageFreqDF(DamageDFs.Normal, FreqCTDFs.Normal, FightTime.NormalTime);
            normal.Calc();
            var xw = new SkillDamageFreqDF(DamageDFs.XW, FreqCTDFs.XW, FightTime.XWTime);
            xw.Calc();
            DamageFreqDFs = new Period<SkillDamageFreqDF>(normal, xw);
        }

        public void CalcDPS()
        {
            var normal = DamageFreqDFs.Normal.GetDPSTableItem("Normal", "常规状态", FightTime.NormalCover);
            var xw = DamageFreqDFs.XW.GetDPSTableItem("XW", "爆发状态", FightTime.XWCover);
            FinalDPSTable = new DPSTable(normal, xw);
            FinalDPSTable.Proceed();
            FinalDPS = FinalDPSTable.DPS;
        }


        // 生成战斗统计
        public void CalcCombatStat()
        {
            var normal = new CombatStat(DamageFreqDFs.Normal);
            var xw = new CombatStat(DamageFreqDFs.XW);

            string CX_DOT_Name = "CX_DOT";
            if (Arg.SkillNum.QiXue.鹰扬虎视)
            {
                CX_DOT_Name = "CXY_DOT";
            }

            string CX_DOT_Hit_Name = "_CX_DOT_Hit";

            Fix_CombatStat_DOT_Hit(normal, xw, CX_DOT_Name, CX_DOT_Hit_Name);
            Fix_CombatStat_DOT_Hit(normal, xw, "CW_DOT", "_CW_DOT_Hit");

            var res = CombatStat.Merge(normal, xw);
            res.Proceed();

            FinalCombatStat = res;
            SimpleFinalCombatStat = FinalCombatStat.ToSimple();

        }

        /// <summary>
        /// 修复DOT跳数统计
        /// </summary>
        /// <param name="normalStat">常规战斗统计</param>
        /// <param name="xwStat">心无战斗统计</param>
        /// <param name="dotName">DOT名称</param>
        /// <param name="dotHitName">DOTHit名称</param>
        protected void Fix_CombatStat_DOT_Hit(CombatStat normalStat, CombatStat xwStat, string dotName, string dotHitName)
        {
            var normalHit = Arg.SkillNum.Normal.FinalSkillFreq[dotHitName];
            var normalDot = Arg.SkillNum.Normal.FinalSkillFreq[dotName];

            normalStat.FixDOTShowNum(dotName, normalHit, normalDot);

            var xwHit = Arg.SkillNum.XW.FinalSkillFreq[dotHitName];
            var xwDot = Arg.SkillNum.XW.FinalSkillFreq[dotName];

            xwStat.FixDOTShowNum(dotName, xwHit, xwDot);
        }



        
        // 计算收益导数
        public void CalcDerivs()
        {
            var normal = DamageFreqDFs.Normal.CalcDeriv();
            var xw = DamageFreqDFs.XW.CalcDeriv();
            DamageDerivs = new Period<DamageDeriv>(normal, xw);

            var derivs = new []{normal, xw};
            var weights = new[] {FightTime.NormalCover, FightTime.XWCover};
            var res = DamageDeriv.WeightedSum(derivs, weights);
            RawDeriv = res;
        }

        // 修复会心收益
        public void FixCTProfit(double ct)
        {
            FinalProfit = RawDeriv.Copy();
            FinalProfit.FixCT(ct, FinalFChars.Normal.L_Percent);
            FinalProfit.Name = "单点收益";
            FinalProfitDF = new ProfitDF(FinalProfit);
            FinalScoreProfit = FinalProfitDF.ScoreDeriv;
            FinalScoreProfit.GetScoreAttrDerivList();
        }

        #endregion

    }

    public readonly struct DPSKernelArg
    {
        public readonly SkillNumModel SkillNum;

        public DPSKernelArg(SkillNumModel skillNum)
        {
            SkillNum = skillNum;
        }
    }

}