using JX3CalculatorShared.Data;
using JX3CalculatorShared.DB;
using JYCalculator.Data;
using System.Collections.Generic;

namespace JYCalculator.DB
{
    public class ItemDTDB : ItemDTDBBase
    {

        #region 构造
        public ItemDTDB(IEnumerable<ItemDTItem> itemdata) : base(itemdata)
        {
        }

        public ItemDTDB(XFDataLoader xfDataLoader) : this(xfDataLoader.ItemDT)
        {
        }

        public ItemDTDB() : this(StaticXFData.Data)
        {
        }
        #endregion

    }
}