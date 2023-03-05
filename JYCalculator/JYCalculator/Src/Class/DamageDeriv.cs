using JX3CalculatorShared.Class;
using JYCalculator.Globals;
using System.Collections.Generic;

namespace JYCalculator.Class
{
    public partial class DamageDeriv
    {
        public double Base_L { get; set; } // 基础力道
        public double Final_L { get; set; } // 最终力道

        #region 构造

        // 复制构造
        public DamageDeriv(DamageDeriv old) : base(old)
        {
            Final_L = old.Final_L;
            Base_L = old.Base_L;
        }

        // 计算最终力道收益
        public static double GetFinal_L(double base_ap, double final_ap, double ct, double base_oc)
        {
            double res = 0;
            res += base_ap * XFConsts.AP_PER_L;
            res += base_oc * XFConsts.OC_PER_L;
            res += final_ap * XFConsts.F_AP_PER_L;
            res += ct * XFConsts.CT_PER_L;
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

        // 修复会心收益
        public void FixCT(double ct)
        {
            CT = ct;
            GetFinal_L();
        }

        public void FixCT(double ct, double l_Percent)
        {
            FixCT(ct);
            GetBase_L(l_Percent);
        }

        #endregion

        #region 方法

        /// <summary>
        /// 按照权重相加，用于合并求出最终收益
        /// </summary>
        /// <param name="other">另一个对象</param>
        /// <param name="w">权重系数（技能频率）</param>
        public new void WeightedAdd(DamageDeriv other, double w = 1.0)
        {
            base.WeightedAdd(other, w);
            Final_L += other.Final_L * w;
            Base_L += other.Base_L * w;
        }

        public new void ApplyAttrWeight(AttrWeight aw)
        {
            // 根据属性加权求收益
            base.ApplyAttrWeight(aw);
            Weight = aw;
            Base_L *= aw.L;
            Final_L *= aw.Final_L;
        }

        public AttrProfitList GetPointAttrDerivList()
        {
            // 单点收益
            var l = GetPointAttrDerivListBase();
            l.Add(new AttrProfitItem(nameof(Base_L), "基础力道", Base_L));
            l.Add(new AttrProfitItem(nameof(Final_L), "最终力道", Final_L));

            var res = new AttrProfitList(l);
            res.Proceed();
            ProfitList = res;
            return res;
        }


        // 单分收益
        public AttrProfitList GetScoreAttrDerivList()
        {
            var l = GetScoreAttrDerivListBase();
            l.Add(new AttrProfitItem(nameof(Base_L), "力道", Base_L));

            var res = new AttrProfitList(l);
            res.Proceed();
            ProfitList = res;
            return res;
        }

        public override List<double> GetValueArr()
        {
            var res = base.GetValueArr();
            res.Insert(2, Base_L);
            return res;
        }

        public override List<string> GetDescArr()
        {
            var res = base.GetDescArr();
            res.Insert(2, "力道");
            return res;
        }

        #endregion
    }
}