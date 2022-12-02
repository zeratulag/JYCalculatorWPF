using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.Src.Data;
using JX3CalculatorShared.Src.DB;
using JYCalculator.Src.Data;
using System.Collections.Generic;

namespace JYCalculator.Src.DB
{
    public class ZhenFaDB : ZhenFaDBBase
    {
        #region 成员

        #endregion

        #region 构造

        public ZhenFaDB(IDictionary<string, ZhenFa> zhenFaDict, IEnumerable<ZhenFa_dfItem> zhenFaDf) : base(zhenFaDict, zhenFaDf)
        {
        }

        public ZhenFaDB(JYDataLoader jyDataLoader) : this(jyDataLoader.ZhenFa_dict, jyDataLoader.ZhenFa_df)
        {
        }

        public ZhenFaDB() : this(StaticJYData.Data)
        {
        }

        #endregion
    }
}