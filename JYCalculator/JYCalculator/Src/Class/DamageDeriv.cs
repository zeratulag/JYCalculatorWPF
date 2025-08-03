using JX3CalculatorShared.Class;
using JYCalculator.Globals;
using System.Collections.Generic;

namespace JYCalculator.Class
{
    public partial class DamageDeriv
    {
        public double BaseStrength { get; set; } // 基础力道
        public double FinalStrength { get; set; } // 最终力道

        #region 构造

        // 复制构造
        public DamageDeriv(DamageDeriv old) : base(old)
        {
            FinalStrength = old.FinalStrength;
            BaseStrength = old.BaseStrength;
        }

        // 计算最终力道收益
        public static double GetFinalStrength(double base_ap, double final_ap, double ct, double base_oc)
        {
            double res = 0;
            res += base_ap * XFConsts.FinalStrengthToPhysicsBaseAttackPower;
            res += base_oc * XFConsts.FinalStrengthToPhysicsBaseOvercome;
            res += final_ap * XFConsts.JY_FinalStrengthToPhysicsFinalAttackPower;
            res += ct * XFConsts.JY_FinalStrengthToPhysicsCriticalStrike;
            return res;
        }

        public void GetFinalStrength()
        {
            FinalStrength = GetFinalStrength(PhysicsBaseAttackPower, PhysicsFinalAttackPower, PhysicsCriticalStrike, PhysicsBaseOvercome);
        }

        public void GetBaseStrength(double y_Percent)
        {
            BaseStrength = FinalStrength * (1 + y_Percent);
        }

        // 从伤害数据中计算出导数

        // 修复会心收益
        public void FixCriticalStrike(double ct)
        {
            PhysicsCriticalStrike = ct;
            GetFinalStrength();
        }

        public void FixCriticalStrike(double ct, double l_Percent)
        {
            FixCriticalStrike(ct);
            GetBaseStrength(l_Percent);
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
            base.WeightedAdd(other, w);
            FinalStrength += other.FinalStrength * w;
            BaseStrength += other.BaseStrength * w;
        }

        public void ApplyAttrWeight(AttrWeight aw)
        {
            // 根据属性加权求收益
            base.ApplyAttrWeight(aw);
            Weight = aw;
            BaseStrength *= aw.BaseStrength;
            FinalStrength *= aw.FinalStrength;
        }

        public AttrProfitList GetPointAttrDerivList()
        {
            // 单点收益
            var l = GetPointAttrDerivListBase();
            l.Add(new AttrProfitItem(nameof(BaseStrength), "基础力道", BaseStrength));
            l.Add(new AttrProfitItem(nameof(FinalStrength), "最终力道", FinalStrength));

            var res = new AttrProfitList(l);
            res.Proceed();
            ProfitList = res;
            return res;
        }


        // 单分收益
        public AttrProfitList GetScoreAttrDerivList()
        {
            var l = GetScoreAttrDerivListBase();
            l.Add(new AttrProfitItem(nameof(BaseStrength), "力道", BaseStrength));

            var res = new AttrProfitList(l);
            res.Proceed();
            ProfitList = res;
            return res;
        }

        public override List<double> GetValueArr()
        {
            var res = base.GetValueArr();
            res.Insert(2, BaseStrength);
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