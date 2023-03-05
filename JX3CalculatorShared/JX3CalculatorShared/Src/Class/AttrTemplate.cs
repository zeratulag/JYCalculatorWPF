using JX3CalculatorShared.Data;
using System;
using System.Text.RegularExpressions;

namespace JX3CalculatorShared.Class
{
    public class AbsAttrTemplate // 抽象模板，包括任务属性和技能属性
    {
        public readonly string FullID;
        public readonly string SID;
        public readonly double Denominator;
        public readonly string Type;
        public readonly bool IsValue;

        public AbsAttrTemplate(string fullid, string sid, int denominator, string type)
        {
            FullID = fullid;
            SID = sid;
            Denominator = denominator;
            Type = type;
            IsValue = (Type == "Value" || Type == "Empty");
        }

        /// <summary>
        /// 若SID存在则返回，否则返回FullID
        /// </summary>
        /// <returns></returns>
        public string GetSID()
        {
            if (string.IsNullOrEmpty(SID))
            {
                return FullID;
            }
            else
            {
                return SID;
            }
        }
    }

    public class SkillAttrTemplate : AbsAttrTemplate
    {
        public SkillAttrTemplate(SkillAttrItem item) : base(item.FullID, item.SID, item.Denominator, item.Type)
        {
        }
    }

    public class AttrTemplate : AbsAttrTemplate
    {
        #region 成员

        public readonly string ShortName;

        public readonly int Target;

        public readonly string FullBaseDesc;
        public readonly string FullBaseDescFmt;
        public readonly string FullBaseDescStrengthFmt;


        public static readonly Regex IntPattern = new Regex(@"\{\S*\}", RegexOptions.Compiled);
        public static readonly Regex PctPattern = new Regex(@"\{\S*\}%", RegexOptions.Compiled);

        #endregion

        #region 构造

        public AttrTemplate(AttrItem item) : base(item.FullID, item.SID, item.Denominator, item.Type)
        {
            ShortName = item.ShortName;
            Target = item.Target;

            var fullBaseDesc = item.GeneratedMagic ?? "";


            (FullBaseDesc, FullBaseDescFmt) = GetDescFmt(fullBaseDesc);
            FullBaseDescStrengthFmt = fullBaseDesc.Replace("{D0}", "{0:D}(+{1:D})");
        }

        public static AttrTemplate Get(string fullid)
        {
            return AtLoader.GetAtTemplate(fullid);
        }

        public (string Desc, string DescFmt) GetDescFmt(string fullBaseDesc)
        {
            string replacement;
            Regex pattern;
            if (fullBaseDesc.Contains("}%"))
            {
                replacement = "{0:P2}"; // 百分数
                pattern = PctPattern;
            }
            else
            {
                replacement = "{0:D}"; // 整数
                pattern = IntPattern;
            }

            string Desc = pattern.Replace(fullBaseDesc, "");
            string DescFmt = pattern.Replace(fullBaseDesc, replacement);
            return (Desc, DescFmt);
        }

        #endregion

        #region 判断相等

        public override int GetHashCode()
        {
            return FullID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as AttrTemplate;
            if (object.ReferenceEquals(null, other)) return false;
            return FullID == other.FullID;
        }

        public static bool operator ==(AttrTemplate at1, AttrTemplate at2)
        {
            if (object.ReferenceEquals(null, at1))
                return object.ReferenceEquals(null, at2);
            else if (object.ReferenceEquals(null, at2))
                return false;
            return at1.Equals(at2);
        }

        public static bool operator !=(AttrTemplate at1, AttrTemplate at2)
        {
            if (object.ReferenceEquals(null, at1))
                return !object.ReferenceEquals(null, at2);
            else if (object.ReferenceEquals(null, at2))
                return true;
            return !at1.Equals(at2);
        }

        #endregion

        #region 方法

        public bool IsValid()
        {
            return FullID != "atInvalid";
        }

        /// <summary>
        /// 获取属性完整描述
        /// </summary>
        /// <param name="nvalue">整数输入，FullID对应的值</param>
        /// <returns></returns>
        public string GetDesc(int nvalue)
        {
            string res = null;

            if (IsValid())
            {
                if (Denominator == 1)
                {
                    res = string.Format(FullBaseDescFmt, nvalue);
                }
                else
                {
                    double real_value = nvalue / Denominator;
                    res = string.Format(FullBaseDescFmt, real_value);
                }
            }

            return res;
        }


        /// <summary>
        /// 获取强化属性描述字符串
        /// </summary>
        /// <param name="value">原始值</param>
        /// <param name="addvalue">强化提高值</param>
        /// <returns>字符串</returns>
        public string GetStrengthDesc(int value, int addvalue)
        {
            string res = null;
            if (IsValid())
            {
                if (Denominator != 1)
                {
                    throw new ArgumentException("属性不可强化！");
                }

                res = string.Format(FullBaseDescStrengthFmt, value, addvalue);
            }

            return res;
        }


        /// <summary>
        /// 获取强化属性描述富文本字符串
        /// </summary>
        /// <param name="value">原始值</param>
        /// <param name="addvalue">强化提高值</param>
        /// <returns>字符串</returns>
        public string GetStrengthDescRichFmt(int value, int addvalue)
        {
            string res0 = null;

            if (IsValid())
            {
                res0 = string.Format(FullBaseDescStrengthFmt, value, addvalue);
            }

            int index = res0.IndexOf('(');
            var str1 = res0.Substring(0, index);
            var str2 = res0.Substring(index);

            string richfmtstr1 = "<Run>" + str1 + "</Run>";
            string richfmtstr2 = "<Run Foreground = \"#7ee3a3\">" + str2 + "</Run>";

            string richfmtres = richfmtstr1 + richfmtstr2;
            return richfmtres;
        }

        #endregion
    }
}