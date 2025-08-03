using JX3CalculatorShared.Class;
using JYCalculator.Data;
using System.Collections.Generic;

namespace JYCalculator.Class
{
    public class AttrWeight : AttrWeightBase
    {
        public double BaseStrength { get; set; }
        public double FinalStrength { get; set; } = double.NaN;

        public AttrWeight(DiamondValueItem item) : base(item)
        {
            BaseStrength = item.BaseStrength;
        }

        public AttrWeight(string name, string toolTip = "") : base(name, toolTip)
        {
        }

        public override Dictionary<string, double> ToDict()
        {
            var res = base.ToDict();
            res.Add(nameof(BaseStrength), BaseStrength);
            res.Add(nameof(FinalStrength), FinalStrength);
            return res;
        }
    }
}