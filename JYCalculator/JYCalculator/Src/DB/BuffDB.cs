using JX3CalculatorShared.Src.Data;
using JX3CalculatorShared.Src.DB;
using JYCalculator.Src.Data;
using System.Collections.Generic;

namespace JYCalculator.Src.DB
{
    public class BuffDB : BuffDBBase
    {
        #region 成员

        #endregion

        #region 构造

        public BuffDB(IEnumerable<Buff_dfItem> Buff_df) : base(Buff_df)
        {
        }

        public BuffDB(JYDataLoader jyDataLoader) : this(jyDataLoader.Buff_df)
        {
        }

        public BuffDB() : this(StaticJYData.Data.Buff_df)
        {
        }

        #endregion

        #region 取出

        #endregion
    }
}