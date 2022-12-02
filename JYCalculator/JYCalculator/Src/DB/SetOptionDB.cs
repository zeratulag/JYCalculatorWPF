using JX3CalculatorShared.Src.Data;
using JX3CalculatorShared.Src.DB;
using JYCalculator.Src.Data;
using System.Collections.Generic;

namespace JYCalculator.Src.DB
{
    public class SetOptionDB: SetOptionDBBase
    {

        #region 成员

        #endregion

        #region 构造
        public SetOptionDB(IEnumerable<SetOptionItem> itemdata) : base(itemdata)
        {
        }

        public SetOptionDB(JYDataLoader jyDataLoader) : this(jyDataLoader.SetOption.Values)
        {
        }

        public SetOptionDB() : this(StaticJYData.Data)
        {
        }


        #endregion

        #region 取出

        #endregion


        #region 方法

        #endregion

    }
}