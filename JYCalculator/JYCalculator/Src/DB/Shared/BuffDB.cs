using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.DB;
using JYCalculator.Data;
using JYCalculator.Globals;
using System.Collections.Generic;

namespace JYCalculator.DB
{
    public class BuffDB : BuffDBBase
    {
        public readonly Buff XW;
        public readonly Buff BigXW;

        #region 构造

        public BuffDB(IEnumerable<Buff_dfItem> Buff_df) : base(Buff_df)
        {
        }

        public BuffDB(XFDataLoader xfDataLoader) : this(xfDataLoader.Buff_df)
        {
        }

        public BuffDB() : this(StaticXFData.Data.Buff_df)
        {
            XW = Buff_Special[XFStaticConst.XW.XWBuffName];
            BigXW = Buff_Special[XFStaticConst.XW.BigXWBuffName];
        }

        #endregion

    }
}