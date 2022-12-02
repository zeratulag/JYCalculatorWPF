using System.Collections.Generic;
using JX3CalculatorShared.Src.Class;
using JYCalculator.Globals;
using MiniExcelLibs.Attributes;

namespace JYCalculator.Class
{
    public class DamageDeriv
    {
        public string Name { get; set; }
        public double Base_AP { get; set; } // 基础攻击
        public double Base_OC { get; set; } // 基础破防
        public double Base_L { get; set; } // 基础力道
        public double PZ { get; set; } // 破招
        public double WS { get; set; } // 无双
        public double CT { get; set; } // 会心
        public double CF { get; set; } // 会效
        public double WP { get; set; } // 武器伤害
        public double Final_AP { get; set; } // 最终攻击
        public double Final_OC { set; get; } // 最终破防
        public double Final_L { get; set; } // 最终力道

        [ExcelIgnore]
        public AttrProfitList ProfitList { get; protected set; }

        public string OrderDesc => ProfitList.OrderDesc;

        public AttrWeight Weight = JYConsts.PointWeight;

        public string ToolTip => Weight.ToolTip;

        #region 构造

        public DamageDeriv(string name)
        {
            Name = name;
        }

        // 复制构造
        public DamageDeriv(DamageDeriv old)
        {
            Name = old.Name;
            Final_AP = old.Final_AP;
            WP = old.WP;
            PZ = old.PZ;
            WS = old.WS;
            CT = old.CT;
            CF = old.CF;
            Final_OC = old.Final_OC;
            Base_AP = old.Base_AP;
            Base_OC = old.Base_OC;
            Final_L = old.Final_L;
            Base_L = old.Base_L;
        }

        public DamageDeriv Copy()
        {
            return new DamageDeriv(this);
        }

        // 计算最终力道收益
        public static double GetFinal_L(double base_ap, double final_ap, double ct, double base_oc)
        {
            double res = 0;
            res += base_ap * JYConsts.AP_PER_L;
            res += base_oc * JYConsts.OC_PER_L;
            res += final_ap * JYConsts.F_AP_PER_L;
            res += ct * JYConsts.CT_PER_L;
            return res;
        }

        public void GetFinal_L()
        {
            Final_L = GetFinal_L(Base_AP, Final_AP, CT, Base_OC);
        }

        public void GetBase_L(double y_Percent)
        {
            Base_L = Final_L * (1 + y_Percent);
        }

        // 从伤害数据中计算出导数
        public static DamageDeriv CalcSkillDamageDeriv(SkillDamage damage)
        {
            var res = damage.CalcDeriv();
            return res;
        }

        // 修复会心收益
        public void FixCT(double ct)
        {
            CT = ct;
            GetFinal_L();
        }

        public void FixCT(double ct, double y_Percent)
        {
            FixCT(ct);
            GetBase_L(y_Percent);
        }

        #endregion

        #region 方法

        /// <summary>
        /// 按照权重相加，用于合并求出最终收益
        /// </summary>
        /// <param name="other">另一个对象</param>
        /// <param name="w">权重系数（技能频率）</param>
        public void WeightedAdd(DamageDeriv other, double w = 1.0)
        {
            Final_AP += other.Final_AP * w;
            WP += other.WP * w;
            PZ += other.PZ * w;
            WS += other.WS * w;
            CT += other.CT * w;
            CF += other.CF * w;
            Final_OC += other.Final_OC * w;
            Base_AP += other.Base_AP * w;
            Base_OC += other.Base_OC * w;
            Final_L += other.Final_L * w;
            Base_L += other.Base_L * w;
        }

        // 多个收益加权求和
        public static DamageDeriv WeightedSum(DamageDeriv[] derivs, double[] weights)
        {
            var res = new DamageDeriv("Result");
            for (int i = 0; i < derivs.Length; i++)
            {
                res.WeightedAdd(derivs[i], weights[i]);
            }

            return res;
        }

        // 根据属性加权求收益
        public DamageDeriv GetAttrWeightedDeriv(AttrWeight aw)
        {
            var res = Copy();
            res.Weight = aw;
            res.Name = aw.Name;
            res.Base_AP *= aw.AP;
            res.Base_L *= aw.L;
            res.Base_OC *= aw.OC;
            res.CT *= aw.CT;
            res.CF *= aw.CF;
            res.WS *= aw.WS;
            res.PZ *= aw.PZ;
            res.WP *= aw.WP;

            res.Final_AP *= aw.Final_AP;
            res.Final_L *= aw.Final_L;
            res.Final_OC *= aw.Final_OC;

            return res;
        }

        // 单点收益
        public AttrProfitList GetPointAttrDerivList()
        {
            var l = new List<AttrProfitItem>
            {
                new AttrProfitItem(nameof(Final_AP), "最终攻击", Final_AP),
                new AttrProfitItem(nameof(Final_OC), "最终破防", Final_AP),
                new AttrProfitItem(nameof(Final_L), "最终力道", Final_L),
                new AttrProfitItem(nameof(Base_AP), "基础攻击", Base_AP),
                new AttrProfitItem(nameof(Base_OC), "基础破防", Base_OC),
                new AttrProfitItem(nameof(Base_L), "基础力道", Base_L),
                new AttrProfitItem(nameof(PZ), "破招", PZ),
                new AttrProfitItem(nameof(WS), "无双", WS),
                new AttrProfitItem(nameof(CT), "会心", CT),
                new AttrProfitItem(nameof(CF), "会效", CF),
                new AttrProfitItem(nameof(WP), "武伤", WP)
            };

            var res = new AttrProfitList(l);
            res.Proceed();
            ProfitList = res;
            return res;
        }

        // 单分收益
        public AttrProfitList GetScoreAttrDerivList()
        {
            var l = new List<AttrProfitItem>
            {
                new AttrProfitItem(nameof(Base_AP), "攻击", Base_AP),
                new AttrProfitItem(nameof(Base_OC), "破防", Base_OC),
                new AttrProfitItem(nameof(Base_L), "力道", Base_L),
                new AttrProfitItem(nameof(PZ), "破招", PZ),
                new AttrProfitItem(nameof(WS), "无双", WS),
                new AttrProfitItem(nameof(CT), "会心", CT),
                new AttrProfitItem(nameof(CF), "会效", CF),
                new AttrProfitItem(nameof(WP), "武伤", WP)
            };

            var res = new AttrProfitList(l);
            res.Proceed();
            ProfitList = res;
            return res;
        }

        public double[] GetValueArr()
        {
            var res = new[]
            {
                Base_AP, Base_OC, Base_L,
                PZ, WS,
                CT, CF,
                //WP
            };
            return res;
        }

        public static string[] GetDescArr()
        {
            var res = new[] {"攻击", "破防", "力道",
                "破招", "无双",
                "会心", "会效",
                //"武伤"
            };
            return res;
        }


        #endregion
    }


   
}