using JX3CalculatorShared.Data;
using JX3CalculatorShared.Utils;
using JYCalculator.DB;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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

    /// <summary>
    /// 奇穴库
    /// </summary>
    public static class JYQiXueLib
    {
        public static ImmutableDictionary<string, string> JBQiXueCodes = new Dictionary<string, string>()
        {
            {nameof(ZhuXingBaiLi_HuiChang), "2,2,3,2,1,2,4,2,2,1,1,4"},
            {nameof(ZhuXingBaiLi_BaiYu), "2,2,3,2,1,2,4,2,2,3,1,4"},
        }.ToImmutableDictionary();

        public static ImmutableDictionary<string, JYQiXueConfig> QiXueDict;
        public static readonly JYQiXueConfig ZhuXingBaiLi_HuiChang;
        public static readonly JYQiXueConfig ZhuXingBaiLi_BaiYu;

        static JYQiXueLib()
        {
            var qixuedict =
                JBQiXueCodes.ToDictionary(jbQixue => jbQixue.Key, jbQixue => new JYQiXueConfig(jbQixue.Value));
            QiXueDict = qixuedict.ToImmutableDictionary();

            ZhuXingBaiLi_HuiChang = QiXueDict[nameof(ZhuXingBaiLi_HuiChang)];
            ZhuXingBaiLi_BaiYu = QiXueDict[nameof(ZhuXingBaiLi_BaiYu)];
        }
    }
}