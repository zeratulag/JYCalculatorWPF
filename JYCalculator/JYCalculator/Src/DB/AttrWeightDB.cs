using System.Collections.Generic;
using System.Collections.Immutable;
using JYCalculator.Class;
using JX3CalculatorShared.Common;
using JYCalculator.Globals;
using JYCalculator.Src.Data;

namespace JYCalculator.Src.DB
{
    public class AttrWeightDB : IDB<string, AttrWeight>
    {
        #region 成员

        public readonly ImmutableDictionary<string, AttrWeight> Data;
        public readonly ImmutableArray<AttrWeight> Arr;

        public static readonly ImmutableDictionary<string, string> AttrNameDict = new Dictionary<string, string>()
        {
            {"AP", "基础攻击"}, {"OC", "基础破防"}, {"L", "基础力道"},
            {"CT", "会心"}, {"CF", "会效"},
            {"PZ", "破招"}, {"WS", "无双"},
            {"Final_AP", "最终攻击"}, {"Final_OC", "最终破防"}, {"Final_L", "最终力道"},
            {"Base_AP", "基础攻击"}, {"Base_OC", "基础破防"}, {"Base_L", "基础力道"}
        }.ToImmutableDictionary();

        #endregion

        #region 构造

        public AttrWeightDB(JYDataLoader dl)
        {
            var arr = new List<AttrWeight>(12);
            arr.Add(JYConsts.PointWeight);
            arr.Add(JYConsts.ScoreWeight);

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

        public AttrWeightDB(): this(StaticJYData.Data)
        {
        }


        #endregion

        public AttrWeight Get(string name)
        {
            return Data[name];
        }
    }
}