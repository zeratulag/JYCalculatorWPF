using JX3CalculatorShared.Globals;
using System;
using static System.Math;

namespace JX3CalculatorShared.Common
{
    public static class DamageTool
    {
        // 防御减伤计算
        public static double DefReduceDmg(double finalDef, double defCoef)
        {
            double res = finalDef / (defCoef + finalDef);
            res = Min(0.75, Max(res, -0.75));
            return res;
        }

        // 承伤率计算
        public static double BearDef(double closingDef, double defCoef)
        {
            return 1 - DefReduceDmg(closingDef, defCoef);
        }

        public static double ETable(double CT, double CF)
        {
            double ct = Min(1.0, Max(0.0, CT));
            double cf = Min(3.0, Max(1.75, CF));
            double res = ct * (cf - 1) + 1;
            return res;
        }

        /// <summary>
        /// 等级压制系数，现在侠士对低等级目标造成的伤害将提高，具体数值为：15%*等级差，最大提高150%；对高等级目标造成的伤害将降低，具体数值为：5%*等级差，最大降低50%。
        /// </summary>
        /// <param name="selfLevel">玩家等级</param>
        /// <param name="targetLevel">目标等级</param>
        /// <returns>等级压制伤害系数</returns>
        public static double LevelCrushCoef(int targetLevel, int selfLevel = StaticConst.CurrentLevel)
        {
            int levelDiff = Math.Min(Math.Abs(selfLevel - targetLevel), 10); // 绝对值等级差
            double res = 1;
            if (selfLevel >= targetLevel)
            {
                res = 1 + 0.15 * levelDiff;
            }
            else
            {
                res = 1 - 0.05 * levelDiff;
            }

            return res;
        }


    }

    public class Period<T> where T : class
    {
        public T Normal;
        public T XW;
        /// <summary>
        /// 用于描述在平时和心无不同状态的类集合
        /// </summary>
        /// <param name="normal">平时状态</param>
        /// <param name="xw">心无</param>
        public Period(T normal, T xw)
        {
            Normal = normal;
            XW = xw;
        }

        public Period()
        {

        }

    }
}