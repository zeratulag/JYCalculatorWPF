using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace JX3CalculatorShared.Utils
{
    public static class ValueDictTool
    {
        /// <summary>
        /// Dict的value乘以x，用于模拟buff叠层数和覆盖率
        /// </summary>
        /// <param name="dict">数值字典</param>
        /// <param name="x">乘数</param>
        /// <returns>相乘后的结果</returns>
        public static Dictionary<string, double> ValueDictMultiply(this IDictionary<string, double> dict, double x)
        {
            var result = dict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value * x);

            return result;
        }

        /// <summary>
        /// 数值类字典相加，就地加给第一个dict，并且返回
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dict">原始字典</param>
        /// <param name="dict2">需要相加的字典</param>
        /// <returns>相加后的结果</returns>
        public static IDictionary<TKey, double> ValueDictAppend<TKey>(this IDictionary<TKey, double> dict,
            IDictionary<TKey, double> dict2)
        {
            if (dict2 != null)
            {
                foreach (var kvp in dict2)
                {
                    dict[kvp.Key] = dict.GetValueOrUseDefault(kvp.Key, 0.0) + kvp.Value;
                }
            }
            return dict;
        }


        /// <summary>
        /// 数值类字典相加，就地加给第一个dict，并且返回
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dict">原始字典</param>
        /// <param name="dict2">需要相加的字典</param>
        /// <returns>相加后的结果</returns>
        public static IDictionary<TKey, int> ValueDictAppend<TKey>(this IDictionary<TKey, int> dict,
            IDictionary<TKey, int> dict2)
        {
            if (dict2 != null)
            {
                foreach (var kvp in dict2)
                {
                    dict[kvp.Key] = dict.GetValueOrUseDefault(kvp.Key, 0) + kvp.Value;
                }
            }
            return dict;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict">数值类字典</param>
        /// <param name="kvp">返回的单一KVP</param>
        /// <returns>如果只有一种属性，则返回true，否则false</returns>
        public static bool GetSingleKVP(this IDictionary<string, double> dict, out KeyValuePair<string, double> kvp)
        {
            var nullkvp = new KeyValuePair<string, double>();

            if (dict.Count == 1)
            {
                kvp = dict.First();
                return true;
            }
            else
            {
                var distinctValues = dict.Values.Distinct().ToArray();
                if (distinctValues.Length == 1)
                {
                    var prefixes = new string[2] { "P_", "M_" };
                    var keys = from k in dict.Keys select k.RemovePrefixes(prefixes);
                    var distinctKeys = keys.Distinct().ToArray();
                    if (distinctKeys.Length == 1)
                    {
                        kvp = new KeyValuePair<string, double>(distinctKeys.First(), distinctValues.First());
                        return true;
                    }
                }
            }

            kvp = nullkvp;
            return false;
        }

    }
}