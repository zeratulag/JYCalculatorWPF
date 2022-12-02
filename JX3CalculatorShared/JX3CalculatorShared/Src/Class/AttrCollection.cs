using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.Src.Data;
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
                        var v = new List<object>() {itemKvp.Value};
                        otherAts.Add(itemKvp.Key, v);
                    }
                }
            }

            return (valueAts, otherAts);
        }

        public AttrCollection(IDictionary<string, double> data, Func<string, bool> isValue)
        {
            var (valueAts, otherAts) = ParseData(data, isValue);
            Values = valueAts.ToDict();
            Others = otherAts.ToDict();
            Simplified = false;
        }

        public AttrCollection Copy()
        {
            var res = new AttrCollection(Values, Others, Simplified);
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

                var newvalue = KVP.Value / (double) template.Denominator;
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
                    newvalue = (from _ in newvalue select (object) ((int) _ / template.Denominator)).ToList();
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

    public class NamedAttrs
    {
        /// <summary>
        /// 带命名的属性集合，用于在Debug界面上显示属性内容
        /// </summary>

        #region 成员

        public string Name { get; set; }

        public AttrCollection Attr { get; }
        public string Desc { get; }

        public static NamedAttrs Empty = new NamedAttrs("空");

        #endregion

        #region 构造

        /// <summary>
        /// 空的
        /// </summary>
        /// <param name="name"></param>
        public NamedAttrs(string name)
        {
            Attr = null;
            Name = name;
            Desc = "无属性";
        }

        public NamedAttrs(string name, AttrCollection attr)
        {
            Attr = attr;
            Name = name;
            if (Attr != null)
            {
                Desc = Attr.ToStr();
            }
            else
            {
                Desc = "";
            }
        }

        public NamedAttrs(IEnumerable<string> names, AttrCollection attr, string sep = ", ") :
            this(names.StrJoin(sep), attr)
        {
        }

        public NamedAttrs(BaseBuff buff) : this(buff.DescName, buff.SCharAttrs)
        {
        }

        public NamedAttrs(BaseBuffGroup buffGroup) : this(buffGroup.DescNames, buffGroup.SCharAttr)
        {
        }

        #endregion

        public void FormatName(string fmt = "{0}")
        {
            Name = String.Format(fmt, Name);
        }

        /// <summary>
        /// 使用 prefix:[Name] 修改名称
        /// </summary>
        /// <param name="prefix"></param>
        public void ParcelName(string prefix)
        {
            string res = prefix + "[{0}]";
            FormatName(res);
        }
    }

    /// </summary>
    public class CharAttrCollection : AttrCollection
    {
        public static readonly Func<string, AttrTemplate> GetTemplate = AtLoader.GetAtTemplate;

        public static CharAttrCollection Empty = new CharAttrCollection(false);
        public static CharAttrCollection SimplifiedEmpty = new CharAttrCollection(true);

        #region 构造

        public CharAttrCollection(bool simplified = false) : base(simplified: simplified)
        {
        }

        public CharAttrCollection(AttrCollection attr) : base(attr.Values, attr.Others)
        {
            Simplified = attr.Simplified;
        }

        public CharAttrCollection(IDictionary<string, double> data) : base(data, AtLoader.At_is_Value)
        {
        }

        public CharAttrCollection(Dictionary<string, double> value,
            Dictionary<string, List<object>> other,
            bool simplified = false) : base(value, other, simplified)
        {
        }

        #endregion

        #region 修改

        public new CharAttrCollection Multiply(double k)
        {
            var attr = base.Multiply(k);
            return new CharAttrCollection(attr);
        }

        #endregion


        public CharAttrCollection Simplify()
        {
            var res = base.Simplify(GetTemplate);
            return new CharAttrCollection(res);
        }

        public static CharAttrCollection Sum(IEnumerable<CharAttrCollection> ats)
        {
            var res = AttrCollection.Sum(ats);
            return new CharAttrCollection(res);
        }
    }

    /// <summary>
    /// 描述技能属性的类
    /// </summary>
    public class SkillAttrCollection : AttrCollection
    {
        public static readonly Func<string, SkillAttrTemplate> GetTemplate = AtLoader.GetSkillAttrTemplate;

        public static SkillAttrCollection Empty = new SkillAttrCollection();
        public static SkillAttrCollection SimplifiedEmpty = new SkillAttrCollection();

        #region 构造

        public SkillAttrCollection(bool simplified = false) : base(simplified)
        {
        }

        public SkillAttrCollection(AttrCollection attr) : base(attr.Values, attr.Others)
        {
            Simplified = attr.Simplified;
        }

        public SkillAttrCollection(IDictionary<string, double> data) : base(data, AtLoader.SkillAt_is_Value)
        {
        }

        #endregion

        public SkillAttrCollection Simplify()
        {
            var res = base.Simplify(GetTemplate);
            return new SkillAttrCollection(res);
        }

        public static SkillAttrCollection Sum(IEnumerable<SkillAttrCollection> ats)
        {
            var res = AttrCollection.Sum(ats);
            return new SkillAttrCollection(res);
        }
    }
}