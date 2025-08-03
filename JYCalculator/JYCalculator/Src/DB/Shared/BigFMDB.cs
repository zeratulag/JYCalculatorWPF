using JX3CalculatorShared.Data;
using JX3CalculatorShared.DB;
using JYCalculator.Data;
using System.Collections.Generic;

namespace JYCalculator.DB
{
    public class BigFMDB : BigFMDBBase
    {

        #region 构造
        public BigFMDB(IEnumerable<Enchant> itemdata) : base(itemdata)
        {
        }

        public BigFMDB(XFDataLoader xfDataLoader) : this(xfDataLoader.BigFM)
        {
        }

        public BigFMDB() : this(StaticXFData.Data)
        {
        }

        #endregion

    }
}