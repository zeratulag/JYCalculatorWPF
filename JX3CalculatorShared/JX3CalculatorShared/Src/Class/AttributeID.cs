using JX3CalculatorShared.Data;
using System;
using System.Text.RegularExpressions;
using JX3CalculatorShared.Utils;
using JX3PZ.Data;
using JX3PZ.Globals;
using System.Data;

namespace JX3CalculatorShared.Class
{
    public class AbsAttributeID // 抽象模板，包括任务属性和技能属性
    {
        public readonly string FullID;
        public readonly string SID;
        public readonly int Denominator;
        public readonly string Type;
        public readonly bool IsValue;

        public AbsAttributeID(string fullid, string sid, int denominator, string type)
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

        // 对于特殊属性，获得颜色
        public string GetSpecialColor()
        {
            if (!IsValue)
            {
                if (FullID == "atSetEquipmentRecipe" || FullID == "atSkillEventHandler")
                {
                    return "#ff9600";
                }
            }

            return "#000000";
        }


        #region 重写等于方法

        public override int GetHashCode()
        {
            return FullID.GetHashCode();
        }

        public bool Equals(AbsAttributeID other)
        {
            return other != null && other.FullID == this.FullID;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AbsAttributeID);
        }

        #endregion
    }

    public class SkillAttributeId : AbsAttributeID
    {
        public SkillAttributeId(SkillAttrItem item) : base(item.FullID, item.SID, item.Denominator, item.Type)
        {
        }
    }

    public class AttributeIDPattern
    {
        public static readonly Regex IntPattern = new Regex(@"\{\S*\}", RegexOptions.Compiled);
        public static readonly Regex PctPattern = new Regex(@"\{\S*\}%", RegexOptions.Compiled);
        public static readonly Regex ValuePattern = new Regex(@"(?<=\{)([0-9D/*-]+?)(?=\})");
        public static readonly DataTable dt = new DataTable(); // 用于计算表达式的工具表

        public static string GetValuePattern(string GeneratedMagic)
        {
            // 提取表达式格式
            var m = ValuePattern.Match(GeneratedMagic);
            if (m.Success)
            {
                return m.Value;
            }
            else
            {
                return null;
            }
        }

        public static double GetRealValue(int value, string realValueExpression)
        {
            // 基于表达式格式，计算真实的值
            var expression = realValueExpression.Replace("D0", value.ToString());
            var res = (double) dt.Compute(expression, "");
            return res;
        }
    }


    public class AttributeID : AbsAttributeID
    {
        #region 成员

        public readonly int Target;

        public readonly string CategoryTitle;
        public readonly string Category;
        public readonly string SubType;

        public readonly string DescriptionName;

        public readonly string FullBaseDesc; // 外功会心效果等级提高
        public readonly string FullBaseDescFmt; // "外功会心效果等级提高{0:D}"
        public readonly string FullBaseDescStrengthFmt;
        public readonly string FullDesc; // 外功会心效果等级
        public readonly string SimpleDesc; // 外功会心效果

        public readonly string EquipTag; // 会效
        public readonly bool CanStrength; // 是否可以被精炼

        public readonly bool IsPercent; // 是否为百分比
        public readonly string RealValueExpression; // 从GeneratedMagic中提取的数值模板

        #endregion

        #region 构造

        public AttributeID(AttrItem item) : base(item.FullID, item.SID, item.Denominator, item.Type)
        {
            Target = item.Target;
            CategoryTitle = item.CategoryTitle;
            Category = item.Category;
            SubType = item.SubType;
            EquipTag = item.EquipTag;
            DescriptionName = item.DescriptionName;
            CanStrength = item.Strength.ToBool();

            var fullBaseDesc = item.GeneratedMagic ?? "";

            (FullBaseDesc, FullBaseDescFmt) = GetDescFmt(fullBaseDesc);
            FullBaseDescStrengthFmt = fullBaseDesc.Replace("{D0}", "{0:D}(+{1:D})");
            FullDesc = FullBaseDesc.RemoveSuffix("提高");
            SimpleDesc = FullDesc.RemoveSuffix("等级");

            IsPercent = item.GeneratedMagic.Contains("%");

            RealValueExpression = AttributeIDPattern.GetValuePattern(item.GeneratedMagic);
        }

        public static AttributeID Get(string fullId)
        {
            return AttributeIDLoader.GetAttributeID(fullId);
        }

        public (string Desc, string DescFmt) GetDescFmt(string fullBaseDesc)
        {
            string replacement;
            Regex pattern;
            if (fullBaseDesc.Contains("}%"))
            {
                replacement = "{0:P2}"; // 百分数
                pattern = AttributeIDPattern.PctPattern;
            }
            else
            {
                replacement = "{0:D}"; // 整数
                pattern = AttributeIDPattern.IntPattern;
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
            var other = obj as AttributeID;
            if (object.ReferenceEquals(null, other)) return false;
            return FullID == other.FullID;
        }

        public static bool operator ==(AttributeID at1, AttributeID at2)
        {
            if (object.ReferenceEquals(null, at1))
                return object.ReferenceEquals(null, at2);
            else if (object.ReferenceEquals(null, at2))
                return false;
            return at1.Equals(at2);
        }

        public static bool operator !=(AttributeID at1, AttributeID at2)
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
        /// 获取属性完整描述，eg：外功攻击提高3618 (+137)
        /// </summary>
        /// <param name="value">整数输入，FullID对应的值</param>
        /// <returns></returns>
        public string GetFullDesc(int value)
        {
            string res = null;

            if (IsValid())
            {
                switch (Denominator)
                {
                    case 1:
                    {
                        res = string.Format(FullBaseDescFmt, value);
                        break;
                    }
                    case 0:
                    {
                        if (RealValueExpression != null)
                        {
                            double real_value = GetRealValueByFmt(value);
                            res = string.Format(FullBaseDescFmt, (int) real_value);
                        }
                        break;
                    }

                    default:
                    {
                        double real_value = value / (double) Denominator;

                        try
                        {
                            res = string.Format(FullBaseDescFmt, real_value);
                        }
                        catch
                        {
                            var vstr = GetValueStr(value);
                            var mid = value > 0 ? "提高" : "降低";
                            res = $"{FullDesc}{mid}{vstr}";
                        }

                        break;
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// 基于自身的属性与格式化字符串，获得真实值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public double GetRealValueByFmt(int value)
        {
            return AttributeIDPattern.GetRealValue(value, RealValueExpression);
        }


        public string GetValueStr(int value)
        {
            string res = null;
            if (IsValid())
            {
                if (Denominator == 1)
                {
                    res = string.Format("{0:D0}", value);
                }
                else
                {
                    double real_value = value / (double) Denominator;
                    string fmt;
                    if (IsPercent)
                    {
                        fmt = "{0:P0}";
                    }
                    else
                    {
                        fmt = "{0:F1}";
                    }

                    res = string.Format(fmt, real_value);
                }
            }

            return res;
        }

        /// <summary>
        /// 获取简化描述，eg：力道+934
        /// </summary>
        /// <param name="value">整数值</param>
        /// <returns></returns>
        public string GetSimpleDesc(int value)
        {
            string vstr = null;

            if (IsValid())
            {
                if (Denominator == 1 && value != 0)
                {
                    vstr = value.ToString("D");
                }
                else
                {
                    if (value != 0)
                    {
                        double real_value = Math.Abs(value) / (double) Denominator;
                        vstr = real_value.ToString("F1");
                    }
                }
            }

            string op = "+";
            if (value == 0)
            {
                op = null;
            }

            if (value < 0)
            {
                op = "-";
            }

            var res = $"{SimpleDesc}{op}{vstr}";
            return res;
        }


        public string GetDesc(int value, AttributeEntryTypeEnum attributeType)
        {
            string res;
            // 基于属性类型，获得不同的描述
            switch (attributeType)
            {
                case AttributeEntryTypeEnum.EquipBase:
                case AttributeEntryTypeEnum.EquipExtraMagic:
                case AttributeEntryTypeEnum.Diamond:
                case AttributeEntryTypeEnum.Enhance:
                case AttributeEntryTypeEnum.Stone:
                case AttributeEntryTypeEnum.Default:
                {
                    res = GetFullDesc(value);
                    break;
                }
                case AttributeEntryTypeEnum.EquipBasicMagic: // 体质+5581，力道+1082
                {
                    res = GetSimpleDesc(value);
                    break;
                }
                default:
                {
                    res = GetFullDesc(value);
                    break;
                }
            }

            return res;
        }


        /// <summary>
        /// 获取强化属性描述字符串列表
        /// </summary>
        /// <param name="value">原始值</param>
        /// <param name="addValue">强化提高值</param>
        /// <returns>字符串列表</returns>
        public string[] GetStrengthDescs(int value, int addValue, AttributeEntryTypeEnum attributeType)
        {
            string baseRes = GetDesc(value, attributeType);
            var res = new[] {baseRes, $"{addValue:D}"};
            return res;
        }

        /// <summary>
        /// 获取强化属性描述字符串
        /// </summary>
        /// <param name="value">原始值</param>
        /// <param name="addValue">强化提高值</param>
        /// <returns>字符串</returns>
        public string GetStrengthDesc(int value, int addValue, AttributeEntryTypeEnum attributeType)
        {
            var l = GetStrengthDescs(value, addValue, attributeType);
            var res = $"{l[0]}(+{l[1]})";
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