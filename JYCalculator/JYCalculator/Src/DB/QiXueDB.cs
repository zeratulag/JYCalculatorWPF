using JX3CalculatorShared.Src.Data;
using JYCalculator.Src.Data;
using System.Collections.Generic;
using JX3CalculatorShared.Src.DB;

namespace JYCalculator.Src.DB
{
    public class QiXueDB : QiXueDBBase
    {
        #region 成员

        #endregion

        #region 构造

        public QiXueDB(IDictionary<string, QiXueItem> qixuedict) : base(qixuedict)
        {
        }

        public QiXueDB(JYDataLoader jyDataLoader) : this(jyDataLoader.QiXue)
        {

        }

        public QiXueDB() : this(StaticJYData.Data)
        {

        }

        #endregion

        #region 取出

        #endregion
    }
}