using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JX3CalculatorShared.Data
{
    /// <summary>
    /// 用于解析tab文件中形如(k, v1, v2)数据的工具
    /// buff: BeginAttrib1	BeginValue1A	BeginValue1B
    /// enchant: Attribute1ID	Attribute1Value1	Attribute1Value2
    /// attrib: ModifyType	Param1Min	Param1Max
    /// </summary>
    public class TabParser
    {
        public string Keyfmt;
        public string Valuefmt;
        public Func<string, bool> IsValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyfmt">tab中属性列名 "At_key{0:D}"</param>
        /// <param name="valuefmt">tab中值列名 "At_value{0:D}"</param>
        /// <param name="isValue">判断属性是否为可加数值类型的函数</param>
        public TabParser(string keyfmt, string valuefmt, Func<string, bool> isValue)
        {
            Keyfmt = keyfmt;
            Valuefmt = valuefmt;
            IsValue = isValue;
        }

        public string GetKey(int i)
        {
            return string.Format(Keyfmt, i);
        }

        public string GetValue(int i)
        {
            return string.Format(Valuefmt, i);
        }

        /// <summary>
        /// 解析属性，生成包含了数值属性和非数值属性的AttrCollection
        /// </summary>
        /// <param name="tab"></param>
        /// <returns></returns>
        public AttrCollection ParseItem(ILuaTable tab)
        {
            var valueAts = new Dictionary<string, double>();
            var otherAts = new Dictionary<string, List<object>>();
            int i = 1;
            while (tab.HasProperty(GetKey(i)))
            {
                var key_name = GetKey(i);
                var value_name = GetValue(i);
                var key = tab.GetPropertyValue<string>(key_name);
                if (string.IsNullOrEmpty(key)) break;
                var value_obj = tab.GetProperty(value_name);
                if (IsValue(key))
                {
                    var value = Convert.ToDouble(value_obj);
                    valueAts[key] = valueAts.GetValueOrUseDefault(key, 0) + value;
                }
                else
                {
                    if (!otherAts.ContainsKey(key))
                    {
                        otherAts.Add(key, new List<object>());
                    }

                    otherAts[key].Add(value_obj);
                }

                i += 1;
            }

            var res = new AttrCollection(valueAts, otherAts);
            return res;
        }

        public Dictionary<TKey, AttrCollection>
            ParseItemDictionary<TKey>(Dictionary<TKey, ILuaTable> data)
        {
            var res = data.ToDictionary(_ => _.Key, item => ParseItem(item.Value));
            return res;
        }

        /// <summary>
        /// 解析数值属性，Dict
        /// </summary>
        /// <param name="tab"></param>
        /// <returns></returns>
        public Dictionary<string, double> ParseValueDictItem(ILuaTable tab)
        {
            var valueAts = new Dictionary<string, double>();
            int i = 1;
            while (tab.HasProperty(GetKey(i)))
            {
                var key_name = GetKey(i);
                var value_name = GetValue(i);
                var key = tab.GetPropertyValue<string>(key_name);
                if (string.IsNullOrEmpty(key)) break;
                var value_obj = tab.GetProperty(value_name);
                var value = Convert.ToDouble(value_obj);
                valueAts[key] = valueAts.GetValueOrUseDefault(key, 0) + value;
                i += 1;
            }

            return valueAts;
        }
    }
}