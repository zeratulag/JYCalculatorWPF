using JX3CalculatorShared.Utils;
using System;
using System.Collections.Generic;

namespace JX3CalculatorShared.Class
{
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
}