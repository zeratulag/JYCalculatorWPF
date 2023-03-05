using JX3CalculatorShared.Data;
using JX3CalculatorShared.DB;
using JYCalculator.Data;
using System.Collections.Generic;

namespace JYCalculator.DB
{
    public class QiXueDB : QiXueDBBase
    {

        #region 构造
        public QiXueDB(IDictionary<string, QiXueItem> qixuedict) : base(qixuedict)
        {
        }

        public QiXueDB(XFDataLoader xfDataLoader) : this(xfDataLoader.QiXue)
        {

        }

        public QiXueDB() : this(StaticXFData.Data)
        {

        }

        #endregion

    }
}