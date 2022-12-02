using JX3CalculatorShared.Src.Data;
using JX3CalculatorShared.Src.DB;
using JYCalculator.Src.Data;
using System.Collections.Generic;

namespace JYCalculator.Src.DB
{
    public class BigFMDB : BigFMDBBase
    {
        #region 成员

        // 用于分类排序存储各种大附魔的数组

        #endregion

        #region 构造
        public BigFMDB(IEnumerable<BigFMItem> itemdata, IEnumerable<BottomsFMItem> bottomsdata) : base(itemdata, bottomsdata)
        {
        }

        public BigFMDB(JYDataLoader jyDataLoader) : this(jyDataLoader.BigFM, jyDataLoader.BottomsFM)
        {
        }

        public BigFMDB() : this(StaticJYData.Data)
        {
        }

        #endregion

        #region 取出

        #endregion
    }
}