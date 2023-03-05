using JX3CalculatorShared.Class;
using JYCalculator.Data;
using System.Collections.Generic;

namespace JYCalculator.Class
{
    public class AttrWeight : AttrWeightBase
    {
        public double L { get; set; }
        public double Final_L { get; set; } = double.NaN;

        public AttrWeight(DiamondValueItem item) : base(item)
        {
            L = item.L;
        }

        public AttrWeight(string name, string toolTip = "") : base(name, toolTip)
        {
        }

        public override Dictionary<string, double> ToDict()
        {
            var res = base.ToDict();
            res.Add(nameof(L), L);
            res.Add(nameof(Final_L), Final_L);
            return res;
        }
    }
}