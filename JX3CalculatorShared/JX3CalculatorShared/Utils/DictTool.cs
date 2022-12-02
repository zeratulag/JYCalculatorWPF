using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JX3CalculatorShared.Utils
{
    public static class DictTool
    {
        /// <summary>
        /// 将字典转换为可读的字符串形式
        /// </summary>
        /// <typeparam name="TKey">TKey</typeparam>
        /// <typeparam name="TValue">TValue</typeparam>
        /// <param name="dict">输入字典</param>
        /// <returns></returns>
        public static string ToStr<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp)
        {
            TValue value = kvp.Value;
            string res = $"{kvp.Key.ToString()}: {value.ToString()}";
            return res;
        }

        public static string ToStr(this KeyValuePair<string, double> kvp)
        {
            var value = kvp.Value;
            string res = $"{kvp.Key}: {StringTool.ToStr(value)}";
            return res;
        }


        public static string ToStr<TValue>(this IDictionary<object, TValue> dict, string sep = ", ")
        {
            return ToStr<object, TValue>(dict, sep);
        }

        public static string ToStr<TKey, TValue>(this IDictionary<TKey, TValue> dict, string sep = ", ")
        {
            string left = "{";
            string right = "}";

            var middleL = from kvp in dict select kvp.ToStr();
            var middle = String.Join(sep, middleL);
            var res = $"{left}{middle}{right}";
            return res;
        }


        public static string ToStr(this IDictionary<string, double> dict, string sep = ", ")
        {
            string left = "{";
            string right = "}";

            var middleL = from kvp in dict select kvp.ToStr();
            var middle = String.Join(sep, middleL);
            var res = $"{left}{middle}{right}";
            return res;
        }

        public static void Cat<TKey, TValue>(this IDictionary<TKey, TValue> dict, string sep = "")
        {
            var catstr = dict.ToStr(sep);
            Trace.WriteLine(catstr);
        }

        /// <summary>
        /// 取出键对应的值，若键不存在则使用默认值
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="dict">字典</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static TValue GetValueOrUseDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict,
            TKey key,
            TValue defaultValue)
        {
            if (key == null) return defaultValue;
            return dict.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public static TValue GetValueOrUseDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict,
            TKey key,
            Func<TValue> defaultValueProvider)
        {
            if (key == null) return defaultValueProvider();
            return dict.TryGetValue(key, out var value) ? value : defaultValueProvider();
        }

        /// <summary>
        /// 如果key存在，则Dict[key] += value，否则添加key-value
        /// </summary>
        /// <typeparam name="TKey">key类型</typeparam>
        /// <param name="dict">目标字典</param>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public static void AppendKeyValue<TKey>(this IDictionary<TKey, int> dict, TKey key, int value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] += value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        public static void AppendKeyValue<TKey>(this IDictionary<TKey, double> dict, TKey key, double value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] += value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        /// <summary>
        /// 非数值类的字典追加键值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AppendObjectKeyValue<TKey>(this IDictionary<TKey, List<object>> dict, TKey key,
            IEnumerable<object> value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, new List<object>());
            }

            dict[key].AddRange(value);
        }


        /// <summary>
        /// 合并字典（就地合并到第一个）
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="otherdict"></param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> dict,
            IDictionary<TKey, TValue> otherdict)
        {
            foreach (var kvp in otherdict)
            {
                dict.Add(kvp.Key, kvp.Value);
            }

            return dict;
        }


        /// <summary>
        /// 其他类字典相加，就地加给第一个dict，并且返回
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dict"></param>
        /// <param name="otherdict"></param>
        /// <returns>相加后的结果</returns>
        public static IDictionary<TKey, List<object>> ObjectDictAppend<TKey>(this IDictionary<TKey, List<object>> dict,
            IDictionary<TKey, List<object>> otherdict)
        {
            if (otherdict != null)
            {
                foreach (var kvp in otherdict)
                {
                    if (!dict.ContainsKey(kvp.Key))
                    {
                        dict.Add(kvp.Key, new List<object>());
                    }

                    dict[kvp.Key].AddRange(kvp.Value);
                }
            }

            return dict;
        }

        /// <summary>
        /// 对于[TKey, IList[TValue]]类字典，对于一个新的KVP，如果已经存在列表，则直接追加，否则新建列表。
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="dict">原字典</param>
        /// <param name="key">待插入的键</param>
        /// <param name="value">待插入值列表的新值</param>
        public static void AddIntoList<TKey, TValue>(this IDictionary<TKey, List<TValue>> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key].Add(value);
            }
            else
            {
                dict.Add(key, new List<TValue>() {value});
            }
        }

        /// <summary>
        /// 对[TKey, IList[TValue]]类字典，其值列表进行排序
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="dict"></param>
        public static void SortValueList<TKey, TValue>(this IDictionary<TKey, List<TValue>> dict)
        {
            foreach (var key in dict.Keys.ToArray())
            {
                var value = dict[key].ToList();
                value.Sort();
                dict[key] = value;
            }
        }

        /// <summary>
        /// 对形如(key, value)的值Tuple进行求和汇总，形成字典
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="tuples"></param>
        /// <returns></returns>
        public static Dictionary<TKey, int> SumToDict<TKey>(this IEnumerable<ValueTuple<TKey, int>> tuples)
        {
            var res = new Dictionary<TKey, int>();
            foreach (var tup in tuples)
            {
                var key = tup.Item1;
                var value = tup.Item2;
                if (res.ContainsKey(key))
                {
                    res[key] += value;
                }
                else
                {
                    res.Add(key, value);
                }
            }

            return res;
        }

        public static Dictionary<TKey, int> SumToDict<TKey>(this IEnumerable<ValueTuple<TKey, int>?> tuples)
        {
            var tupleValues = from tup in tuples where tup.HasValue select tup.Value;
            return tupleValues.SumToDict();
        }

        public static Dictionary<TKey, double> SumToDict<TKey>(this IEnumerable<ValueTuple<TKey, double>> tuples)
        {
            var res = new Dictionary<TKey, double>();
            foreach (var tup in tuples)
            {
                var key = tup.Item1;
                var value = tup.Item2;
                if (res.ContainsKey(key))
                {
                    res[key] += value;
                }
                else
                {
                    res.Add(key, value);
                }
            }

            return res;
        }

        public static Dictionary<TKey, TVal> ToDict<TKey, TVal>(this IDictionary<TKey, TVal> d)
        {
            var res = d?.ToDictionary(
                kvp => kvp.Key, kvp => kvp.Value);
            return res;
        }

        /// <summary>
        /// 向字典中添加KVPair
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="kvp"></param>
        public static void Add<TKey, TValue>(this Dictionary<TKey, TValue> dict,
            KeyValuePair<TKey, TValue> kvp)
        {
            dict.Add(kvp.Key, kvp.Value);
        }


        /// <summary>
        /// 设定key和value，如果key存在则修改value，否则加入kv pair
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key">需要设置的key</param>
        /// <param name="value">需要设置的value</param>
        public static void SetKeyValue<TKey, TValue>(this Dictionary<TKey, TValue> dict,
            TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        /// <summary>
        /// 字典是否为空
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static bool IsEmptyOrNull<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            var res = dict == null || dict.Count == 0;
            return res;
        }
    }
}