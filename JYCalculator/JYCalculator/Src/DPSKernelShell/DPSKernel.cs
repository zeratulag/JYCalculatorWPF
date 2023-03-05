using JYCalculator.Class;
using JYCalculator.Globals;

namespace JYCalculator.Src
{
    public partial class DPSKernel
    {
        #region 方法

        public void FixCTProfit(double ct)
        {
            // 修复会心收益
            FinalProfit = RawDeriv.Copy();
            FinalProfit.FixCT(ct, FinalFChars.Normal.L_Percent);
            FinalProfit.Name = "单点收益";
            FinalProfitDF = new ProfitDF(FinalProfit);
            FinalScoreProfit = FinalProfitDF.ScoreDeriv;
            FinalScoreProfit.GetScoreAttrDerivList();
        }

        #endregion

        #region 方法

        public void CalcCombatStat()
        {
            // 生成战斗统计
            var normal = new CombatStat(DamageFreqDFs.Normal);
            var xw = new CombatStat(DamageFreqDFs.XW);

            if (XFAppStatic.XinFaTag == "JY")
            {
                string CX_DOT_Name = "CX_DOT";
                if (Arg.SkillNum.QiXue.鹰扬虎视)
                {
                    CX_DOT_Name = "CXY_DOT";
                }

                string CX_DOT_Hit_Name = "_CX_DOT_Hit";

                Fix_CombatStat_DOT_Hit(normal, xw, CX_DOT_Name, CX_DOT_Hit_Name);
                Fix_CombatStat_DOT_Hit(normal, xw, "CW_DOT", "_CW_DOT_Hit");
            }

            var res = CombatStat.Merge(normal, xw);
            res.Proceed();

            FinalCombatStat = res;
            SimpleFinalCombatStat = FinalCombatStat.ToSimple();
        }

        #endregion
    }
}