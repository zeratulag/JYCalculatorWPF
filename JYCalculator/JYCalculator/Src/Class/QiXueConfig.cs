using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Data;
using JX3CalculatorShared.Utils;
using JYCalculator.Src.DB;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


namespace JYCalculator.Class
{

    public class QiXueConfig : ICatsable
    {
        /// <summary>
        /// 描述奇穴配置
        /// </summary>

        #region 成员

        public static readonly QiXueDB AllQiXue = new QiXueDB(); // 所有奇穴的库

        public const string ValidCode = "123456789"; // 有效的奇穴编码

        public readonly ImmutableArray<int> Code;
        public ImmutableArray<string> Names;
        public ImmutableArray<int> Icons;
        public ImmutableArray<QiXueSkill> QiXueItems;

        public const int NumberOfQiXue = StaticData.NumberOfQiXue;

        #endregion

        #region 构造

        public QiXueConfig(IEnumerable<int> code)
        {
            Code = code.ToImmutableArray();
            if (code != null)
            {
                Parse();
            }
        }

        public static ImmutableArray<int> ParseCode(string codeStr)
        {
            var code = new int[NumberOfQiXue];
            int i = 0;
            foreach (var codei in codeStr)
            {
                if (ValidCode.Contains(codei))
                {
                    code[i] = codei.ToInt(); // char转换为数字
                    i++;
                }
            }

            var res = code.ToImmutableArray();
            return res;
        }

        public static ImmutableArray<int> ParseJBCode(JBQiXueItem jbQixue)
        {
            return ParseCode(jbQixue.sq);
        }


        /// <summary>
        /// 解析以字符串形式传递进来的奇穴配置
        /// 支持：3,1,1,2,1,2,4,1,2,3,1,4，311212412314
        /// </summary>
        /// <param name="codeStr">字符串形式的奇穴</param>
        public QiXueConfig(string codeStr)
        {
            Code = ParseCode(codeStr);
            Parse();
        }

        /// <summary>
        /// 导入JB奇穴
        /// </summary>
        /// <param name="jbQixue"></param>
        public QiXueConfig(JBQiXueItem jbQixue) : this(code: ParseJBCode(jbQixue))
        {
        }

        public virtual void Parse(QiXueDB allQiXue)
        {
            var names = new string[NumberOfQiXue];
            var icons = new int[NumberOfQiXue];
            var qiXueItems = new QiXueSkill[NumberOfQiXue];
            for (int i = 0; i < NumberOfQiXue; i++)
            {
                var j = Code[i];
                var qixueitem = allQiXue[i + 1, j];
                qiXueItems[i] = qixueitem;
                names[i] = qixueitem.ItemName;
                icons[i] = qixueitem.IconID;
            }

            Names = names.ToImmutableArray();
            Icons = icons.ToImmutableArray();
            QiXueItems = qiXueItems.ToImmutableArray();
        }

        public virtual void Parse()
        {
            Parse(AllQiXue);
        }

        #endregion

        #region 显示

        public IList<string> GetCatStrList()
        {
            var res = new List<string>()
            {
                $"奇穴：{Names.ToStr<string>()}",
                $"Code: {Code.ToStr<int>()}"
            };
            return res;
        }

        public override string ToString()
        {
            return ToStr();
        }

        public string ToStr()
        {
            return String.Join("\n", GetCatStrList());
        }

        public void Cat()
        {
            var res = ToStr();
            res.Cat();
        }

        #endregion

        #region 方法

        /// <summary>
        /// 判断是否激活了某个奇穴
        /// </summary>
        /// <param name="qixue">奇穴名</param>
        /// <returns></returns>
        public bool Has(string qixue)
        {
            return Names.Contains(qixue);
        }

        #endregion
    }

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
            else if (Has("秋万劫"))
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