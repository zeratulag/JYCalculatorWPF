using JX3CalculatorShared.Data;
using JX3CalculatorShared.DB;
using JYCalculator.Data;
using System.Collections.Generic;

namespace JYCalculator.DB
{
    public class BigFMDB : BigFMDBBase
    {

        #region 构造
        public BigFMDB(IEnumerable<Enchant> itemdata, IEnumerable<BottomsFMItem> bottomsdata) : base(itemdata, bottomsdata)
        {
        }

        public BigFMDB(XFDataLoader xfDataLoader) : this(xfDataLoader.BigFM, xfDataLoader.BottomsFM)
        {
        }

        public BigFMDB() : this(StaticXFData.Data)
        {
        }

        #endregion

    }
}