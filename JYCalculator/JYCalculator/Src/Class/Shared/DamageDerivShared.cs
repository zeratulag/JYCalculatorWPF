using JX3CalculatorShared.Class;

namespace JYCalculator.Class
{
    public partial class DamageDeriv : DamageDerivBase
    {
        public new AttrWeight Weight;

        #region 构造

        public DamageDeriv(string name) : base(name)
        {
        }

        public new DamageDeriv Copy()
        {
            return new DamageDeriv(this);
        }

        public static DamageDeriv CalcSkillDamageDeriv(SkillDamage damage)
        {
            var res = damage.CalcDeriv();
            return res;
        }

        #endregion

        #region 方法

        public static DamageDeriv WeightedSum(DamageDeriv[] derivs, double[] weights)
        {
            // 多个收益加权求和
            var res = new DamageDeriv("Result");
            for (int i = 0; i < derivs.Length; i++)
            {
                res.WeightedAdd(derivs[i], weights[i]);
            }

            return res;
        }

        public DamageDeriv GetAttrWeightedDeriv(AttrWeight aw)
        {
            var res = Copy();
            res.ApplyAttrWeight(aw);
            return res;
        }

        #endregion
    }
}