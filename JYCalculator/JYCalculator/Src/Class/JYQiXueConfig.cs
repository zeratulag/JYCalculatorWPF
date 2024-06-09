using JX3CalculatorShared.Data;
using JX3CalculatorShared.Utils;
using JYCalculator.DB;
using System.Collections.Generic;

namespace JYCalculator.Class
{
    public class JYQiXueConfig : QiXueConfig
    {
        public new static readonly QiXueDB AllQiXue = new QiXueDB();

        #region 构造

        public JYQiXueConfig(IEnumerable<int> code) : base(code: code)
        {
        }

        public JYQiXueConfig(string codeStr) : base(codeStr: codeStr)
        {
        }


        public JYQiXueConfig(JBQiXueItem jbQixue) : base(jbQixue: jbQixue)
        {
        }

        #endregion

        #region 方法

        /// <summary>
        /// 是否有大心无
        /// </summary>
        /// <returns></returns>
        public bool HasBigXW()
        {
            return Has("聚精凝神");
        }

        public (double duration, double cd) GetXWTime()
        {
            double duration = 10.0 + 5 * HasBigXW().ToInt();
            double cd = 100.0;
            if (Has("雷甲三铉"))
            {
                cd = 90;
            }
            else if (Has("千秋万劫"))
            {
                cd = 105.5;
            }

            return (duration, cd);
        }

        #endregion
    }

}