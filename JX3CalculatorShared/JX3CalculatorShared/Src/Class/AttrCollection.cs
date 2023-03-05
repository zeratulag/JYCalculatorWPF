using JX3CalculatorShared.Utils;
using Minimod.PrettyPrint;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JX3CalculatorShared.Class
{
    /// <summary>
    /// 用于同时存储数值属性和非数值属性的不可变类
    /// </summary>
    public class AttrCollection
    {
        #region 成员

        public readonly Dictionary<string, double> Values; // 表示可加数值类属性
        public readonly Dictionary<string, List<object>> Others; // 表示不可加类属性（可以是数值，可以非数值）

        public bool Simplified = false; // 表示是否已经简化过（已简化的不能再次化简）

        // 是否为空
        public bool IsEmptyOrNull => (this == null) ||
                                     (Values == null && Others == null) ||
                                     (Values.IsEmptyOrNull() && Others.IsEmptyOrNull());

        #endregion

        #region 构造

        public AttrCollection(bool simplified = false)
        {
            Values = new Dictionary<string, double>();
            Others = new Dictionary<string, List<object>>();
            Simplified = simplified;
        }

        public AttrCollection(Dictionary<string, double> value, Dictionary<string, List<object>> other,
            bool simplified = false)
        {
            Values = value;
            Others = other;
            Simplified = simplified;
        }

        public AttrCollection(IDictionary<string, double> value, IDictionary<string, List<object>> other,
            bool simplified = false)
        {
            Values = value.ToDict();
            Others = other.ToDict();
            Simplified = simplified;
        }

        public static (Dictionary<string, double> values, Dictionary<string, List<object>> others) ParseData(
            IDictionary<string, double> data, Func<string, bool> isValue)
        {
            var valueAts = new Dictionary<string, double>();
            var otherAts = new Dictionary<string, List<object>>();
            if (data != null)
            {
                foreach (var itemKvp in data)
                {
                    if (isValue(itemKvp.Key))
                    {
                        valueAts.Add(itemKvp.Key, itemKvp.Value);
                    }
                    else
                    {
                        var v = new List<object>() { itemKvp.Value };
                        otherAts.Add(itemKvp.Key, v);
                    }
                }
            }

            return (valueAts, otherAts);
        }

        public AttrCollection(IDictionary<string, double> data, Func<string, bool> isValue)
        {
            // 复制构造
            var (valueAts, otherAts) = ParseData(data, isValue);
            Values = valueAts.ToDict();
            Others = otherAts.ToDict();
            Simplified = false;
        }

        public AttrCollection(AttrCollection old)
        {
            // 复制构造
            Values = old.Values.Copy();
            Others = old.Others.Copy();
            Simplified = old.Simplified;
        }

        public AttrCollection Copy()
        {
            var res = new AttrCollection(this);
            return res;
        }

        #endregion

        #region 方法

        protected void AppendValues(IDictionary<string, double> newValues)
        {
            foreach (var kvp in newValues)
            {
                Values.AppendKeyValue(kvp.Key, kvp.Value);
            }
        }

        protected void AppendOthers(IDictionary<string, List<object>> newOthers)
        {
            Others.ObjectDictAppend(newOthers);
        }


        /// <summary>
        /// 就地追加属性，将另一个属性集合就地加到本身
        /// </summary>
        /// <param name="other">另一个属性集合</param>
        public void Append(AttrCollection other)
        {
            if (other == null || other.IsEmptyOrNull) return;
            if (Simplified == other.Simplified)
            {
                AppendValues(other.Values);
                AppendOthers(other.Others);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// 就地按照倍数修改强度，用于模拟多层效果叠加，包括数值类属性和部分非数值类属性（考虑到技能秘籍的系数修饰属于非数值类属性）
        /// </summary>
        /// <param name="k">效果倍数</param>
        public void MultiplyEffect(double k)
        {
            Values.MultiplyEffect(k);
            Others.MultiplyEffect(k);
        } 

        #endregion

        #region 组合变换

        public static AttrCollection Add(AttrCollection at1, AttrCollection at2)
        {
            return Sum(at1, at2);
        }

        /// <summary>
        /// 将数值类属性乘以k，获得新的对象，非数值类属性不变
        /// </summary>
        /// <param name="k">系数</param>
        /// <returns></returns>
        public AttrCollection Multiply(double k)
        {
            var newValues = Values.ValueDictMultiply(k);
            var res = new AttrCollection(newValues, Others, Simplified);
            return res;
        }


        public static AttrCollection operator +(AttrCollection at1, AttrCollection at2)
        {
            return Add(at1, at2);
        }

        public static AttrCollection Sum(IEnumerable<AttrCollection> ats)
        {
            if (ats == null || !ats.Any()) return null;

            var res = new AttrCollection(ats.First().Simplified);
            foreach (var _ in ats)
            {
                res.Append(_);
            }

            return res;
        }

        public static AttrCollection Sum(params AttrCollection[] pats)
        {
            return Sum(ats: pats);
        }

        #endregion

        #region 简化

        public static Dictionary<string, double> SimplifyValues(IDictionary<string, double> values,
            Func<string, AbsAttrTemplate> func)
        {
            var res = new Dictionary<string, double>();
            foreach (var KVP in values)
            {
                var template = func(KVP.Key);
                var newkey = template.GetSID();
                if (template.Denominator == 0)
                {
                    throw new ArgumentException("未知的分母！");
                }

                var newvalue = KVP.Value / template.Denominator;
                res.AppendKeyValue(newkey, newvalue);
            }

            return res;
        }

        public static Dictionary<string, List<object>> SimplifyOthers(IDictionary<string, List<object>> others,
            Func<string, AbsAttrTemplate> func)
        {
            var res = new Dictionary<string, List<object>>();
            foreach (var KVP in others)
            {
                var template = func(KVP.Key);
                var newkey = template.GetSID();
                var newvalue = KVP.Value;

                if (template.Denominator > 1)
                {
                    newvalue = (from _ in newvalue select (object)((int)_ / template.Denominator)).ToList();
                }

                res.AppendObjectKeyValue(newkey, newvalue);
            }

            return res;
        }


        /// <summary>
        /// 简化为容易读写的形式，需要以一个模板函数作为输入
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public AttrCollection Simplify(Func<string, AbsAttrTemplate> func)
        {
            if (Simplified)
            {
                return this;
            }
            else
            {
                var values = SimplifyValues(Values, func);
                var others = SimplifyOthers(Others, func);
                var res = new AttrCollection(values, others)
                {
                    Simplified = true
                };
                return res;
            }
        }

        #endregion

        #region 显示

        public string ValueToStr()
        {
            return Values.ToStr();
        }

        public string OtherToStr()
        {
            return Others.PrettyPrint();
        }

        public string ToStr()
        {
            var part1 = ValueToStr();
            string res;
            if (Others != null && Others.Count > 0)
            {
                var part2 = OtherToStr();
                res = $"数值属性:\n{part1}\n其他属性:\n{part2}";
            }
            else
            {
                res = part1;
            }

            return res;
        }

        public virtual NamedAttrs ToNamed(string name = "")
        {
            return new NamedAttrs(name, this);
        }

        #endregion
    }
}