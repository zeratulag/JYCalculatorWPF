using JX3CalculatorShared.Data;
using JX3CalculatorShared.DB;
using JYCalculator.Data;
using System.Collections.Generic;

namespace JYCalculator.DB
{
    public class SetOptionDB : SetOptionDBBase
    {

        #region 构造
        public SetOptionDB(IEnumerable<SetOptionItem> itemData) : base(itemData)
        {
        }

        public SetOptionDB(XFDataLoader xfDataLoader) : this(xfDataLoader.SetOption.Values)
        {
        }

        public SetOptionDB() : this(StaticXFData.Data)
        {
        }

        #endregion

    }
}