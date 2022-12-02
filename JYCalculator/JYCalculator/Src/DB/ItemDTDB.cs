using JX3CalculatorShared.Src.Data;
using JYCalculator.Src.Data;
using System.Collections.Generic;
using JX3CalculatorShared.Src.DB;

namespace JYCalculator.Src.DB
{
    public class ItemDTDB : ItemDTDBBase
    {
        #region 成员

        // 用于分类排序存储各种单体的数组

        // 用于汇总各个数组

        #endregion

        #region 构造

        public ItemDTDB(IEnumerable<ItemDTItem> itemdata) : base(itemdata)
        {
        }

        public ItemDTDB(JYDataLoader jyDataLoader) : this(jyDataLoader.ItemDT)
        {
        }

        public ItemDTDB() : this(StaticJYData.Data)
        {
        }
        #endregion

        #region 取出

        #endregion
    }
}