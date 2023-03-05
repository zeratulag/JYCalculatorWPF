using JX3CalculatorShared.Common;
using JYCalculator.Class;
using JYCalculator.Data;
using JYCalculator.Globals;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace JYCalculator.DB
{
    public partial class AttrWeightDB : IDB<string, AttrWeight>
    {
        #region 成员

        public readonly ImmutableDictionary<string, AttrWeight> Data;
        public readonly ImmutableArray<AttrWeight> Arr;

        #endregion

        #region 构造

        public AttrWeightDB(XFDataLoader dl)
        {
            var arr = new List<AttrWeight>(12);
            arr.Add(XFConsts.PointWeight);
            arr.Add(XFConsts.ScoreWeight);

            int i = 1;
            var DiamondDict = dl.DiamondValue;
            while (DiamondDict.ContainsKey(i))
            {
                var v = DiamondDict[i];
                arr.Add(new AttrWeight(v));
                i++;
            }

            Arr = arr.ToImmutableArray();
            Data = Arr.ToImmutableDictionary(_ => _.Name, _ => _);
        }

        public AttrWeightDB() : this(StaticXFData.Data)
        {
        }

        #endregion

        public AttrWeight Get(string name)
        {
            return Data[name];
        }
    }
}