using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


namespace JX3CalculatorShared.Class
{
    /// <summary>
    /// 用于描述buff/秘籍的类
    /// </summary>
    public abstract class AttrModifier : ICatsable
    {
        #region 成员

        public readonly string Name;
        public readonly string DescName;

        public readonly AttrCollection AttrCollect;
        protected bool Simplified = false; // 用于标记是否简化过

        #endregion

        #region 构造


        /// <param name="name">唯一名称（编码）</param>
        /// <param name="descName">描述名</param>
        /// <param name="data">属性字典</param>
        /// <param name="isValue">判断属性是否为数值的范数</param>
        public AttrModifier(string name, string descName,
            IDictionary<string, double> data,
            Func<string, bool> isValue)
        {
            Name = name;
            DescName = descName;
            AttrCollect = new AttrCollection(data, isValue);
        }

        public AttrModifier(string name, string descName,
            AttrCollection attrCollect)
        {
            Name = name;
            DescName = descName;
            AttrCollect = attrCollect.Copy();
        }


        /// <param name="name">唯一名称（编码）</param>
        /// <param name="descName">描述名</param>
        /// <param name="valueAts">数值类属性字典</param>
        /// <param name="otherAts">非数值类属性字典</param>
        public AttrModifier(string name, string descName,
            IDictionary<string, double> valueAts, IDictionary<string, List<object>> otherAts)
        {
            Name = name;
            DescName = descName;
            AttrCollect = new AttrCollection(valueAts, otherAts);
        }


        #endregion

        #region 显示

        public virtual IList<string> GetCatStrList()
        {
            var res = new List<string>()
            {
                $"Name: {Name}, DescName: {DescName}",
                $"- 数值属性: {AttrCollect.Values.ToStr()}\n- 其他属性: {AttrCollect.Others.ToStr()}"
            };
            return res;
        }

        public string ToStr()
        {
            var res = GetCatStrList();
            return res.StrJoin("\n");
        }

        public void Cat()
        {
            var res = GetCatStrList();
            res.Cat();
        }

        #endregion

        #region 简化

        public static Dictionary<string, double> ValueAts_to_S(IDictionary<string, double> valueAts,
            Func<string, AbsAttrItem> AtFunc)
        {
            var result = new Dictionary<string, double>();
            foreach (var itemKvp in valueAts)
            {
                var at_k = AtFunc(itemKvp.Key);
                var s_ID = at_k.SID;
                if (s_ID != null)
                {
                    result[s_ID] = result.GetValueOrUseDefault(s_ID, 0.0) + itemKvp.Value / at_k.Denominator;
                }
            }

            return result;
        }

        public static Dictionary<string, List<object>> OtherAts_to_S(IDictionary<string, List<object>> otherAts,
            Func<string, AbsAttrItem> AtFunc)
        {
            var result = new Dictionary<string, List<object>>();
            foreach (var itemKvp in otherAts)
            {
                var at_k = AtFunc(itemKvp.Key);
                var s_ID = at_k.SID;
                if (s_ID != null)
                {
                    var v2 = new List<object>();
                    int deno = at_k.Denominator;
                    if (deno == -1)
                    {
                        v2 = itemKvp.Value;
                    }
                    else
                    {
                        var v2t = from v2item in itemKvp.Value select Convert.ToDouble(v2item) / deno;
                        v2.AddRange(v2t.Cast<object>());
                    }

                    result[s_ID] = v2;
                }
            }

            return result;
        }


        #endregion

        #region 方法


        #endregion
    }

    public class AttrModifierGroup : ICatsable
    {
        #region 成员

        public List<AttrModifier> Items;
        public List<string> Names;

        public Dictionary<string, double> ValueAts = null;
        public Dictionary<string, List<object>> OtherAts = null;

        public ImmutableDictionary<string, double> S_ValueAts = null;
        public ImmutableDictionary<string, List<object>> S_OtherAts = null;

        protected bool HasSummary = false;
        protected bool Simplified = false;

        #endregion

        #region 构造

        /// <summary>
        /// 用于描述一组基础buff的组合
        /// </summary>
        /// <param name="items">List of Items，要求Name不重复</param>
        /// <exception cref="ArgumentException">当Name重复时抛出异常</exception>
        public AttrModifierGroup(IEnumerable<AttrModifier> items)
        {
            var names = (from item in items select item.Name).ToList();
            int n = names.Count();
            var n_distinct = names.Distinct().Count();
            if (n_distinct < n)
            {
                throw new ArgumentException("存在相同Name！");
            }

            Items = items.ToList();
            Names = names;
        }

        /// <summary>
        /// 当构成的元素发生变化时，需要删除缓存
        /// </summary>
        public void ClearCache()
        {
            ValueAts = null;
            OtherAts = null;
            S_ValueAts = null;
            S_OtherAts = null;
            HasSummary = false;
            Simplified = false;
        }

        #endregion

        #region 显示

        public string ToStr()
        {
            var res = GetCatStrList();
            return res.StrJoin("\n");
        }

        public void Cat()
        {
            var res = GetCatStrList();
            res.Cat();
        }

        public IList<string> GetCatStrList()
        {
            var outStrs = new List<string>()
            {
                $"包含成员：{Names.ToStr()}",
            };
            if (HasSummary)
            {
                outStrs.Add($"- 数值属性:{ValueAts.ToStr()}");
                outStrs.Add($"- 数值属性:{OtherAts.ToStr()}");
            }

            if (Simplified)
            {
                outStrs.Add($"- 简化后的数值属性: {S_ValueAts.ToStr()}\n- 其他属性: {S_OtherAts.ToStr()}");
            }
            return outStrs;
        }

        #endregion

        #region 修改

        /// <summary>
        /// 增加新元素
        /// </summary>
        /// <param name="item">新元素</param>
        /// <exception cref="ArgumentException"></exception>
        public void Append(AttrModifier item)
        {
            var newName = item.Name;
            if (Names.Contains(newName))
            {
                throw new ArgumentException($"存在相同Name！{newName}");
            }

            Items.Add(item);
            Names.Add(newName);
            ClearCache();
        }

        public void Extend(AttrModifierGroup other)
        {
            var newNames = other.Names;
            var shared_name = Names.Intersect(newNames);
            if (shared_name.Any())
            {
                throw new ArgumentException($"存在相同Name！{shared_name.ToStr()}");
            }

            Names.AddRange(newNames);
            Items.AddRange(other.Items);
            ClearCache();
        }

        public static AttrModifierGroup Sum(IEnumerable<AttrModifierGroup> attrModifierGroups)
        {
            var items = new List<AttrModifier>();
            foreach (var group in attrModifierGroups)
            {
                items.AddRange(group.Items);
            }
            var res = new AttrModifierGroup(items);
            return res;
        }

        public virtual AttrModifier this[int i]
        {
            get => Items[i];
            set => Items[i] = value;
        }

        #endregion

        #region 汇总


        #endregion

        #region 简化

        public static Dictionary<string, double> ValueAts_to_S(IDictionary<string, double> valueAts,
            Func<string, AbsAttrItem> AtFunc)
        {
            return AttrModifier.ValueAts_to_S(valueAts, AtFunc);
        }

        public static Dictionary<string, List<object>> OtherAts_to_S(IDictionary<string, List<object>> otherAts,
            Func<string, AbsAttrItem> AtFunc)
        {
            return AttrModifier.OtherAts_to_S(otherAts, AtFunc);
        }

        public virtual void At_to_S(Func<string, AbsAttrItem> AtFunc)
        {
            var S_Value = ValueAts_to_S(ValueAts, AtFunc);
            var S_Other = OtherAts_to_S(OtherAts, AtFunc);
            S_ValueAts = S_Value.ToImmutableDictionary();
            S_OtherAts = S_Other.ToImmutableDictionary();
            Simplified = true;
        }
        #endregion
    }
}