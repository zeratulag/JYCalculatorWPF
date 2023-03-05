using System.Collections.Generic;
using System.Collections.Immutable;

namespace JYCalculator.DB
{
    public partial class AttrWeightDB
    {
        #region 成员

        public static readonly ImmutableDictionary<string, string> AttrNameDict = new Dictionary<string, string>()
        {
            {"AP", "基础攻击"}, {"OC", "基础破防"}, {"L", "基础力道"},
            {"CT", "会心"}, {"CF", "会效"},
            {"PZ", "破招"}, {"WS", "无双"},
            {"Final_AP", "最终攻击"}, {"Final_OC", "最终破防"}, {"Final_L", "最终力道"},
            {"Base_AP", "基础攻击"}, {"Base_OC", "基础破防"}, {"Base_L", "基础力道"}
        }.ToImmutableDictionary();

        #endregion
    }
}