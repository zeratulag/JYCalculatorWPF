using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Data;
using JX3CalculatorShared.Utils;
using JYCalculator.Globals;
using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

// ReSharper disable InconsistentNaming


namespace JYCalculator.Src.Class
{
    public class Target : ObservableObject, ICatable 
    {
        #region 成员

        public int Level { get; }
        public double DefCoef { get; } // 防御系数
        public string DescName { get; private set; }

        [ExcelIgnore]
        public string ToolTip { get; }
        public string ItemName { get; }

        public double Base_PDef { get; set; }  // 基础外防
        public double Final_PDef { get; set; } // 最终外防
        public double Base_MDef { get; set; } // 基础内防
        public double Final_MDef { get; set; } // 最终内防
        public double PDef_Percent { get; set; } // +外防
        public double MDef_Percent { get; set; } // +内防
        public double P_YS { get; set; } // 外功易伤
        public double M_YS { get; set; } // 内功易伤

        #endregion

        public double PDefReduceDmg => DamageTool.DefReduceDmg(Final_PDef, DefCoef); // 外防值

        public double MDefReduceDmg => DamageTool.DefReduceDmg(Final_MDef, DefCoef); // 内防值


        public string GetItemName()
        {
            return $"{Level:D} {DescName}";
        }

        public void SetDescName(string name)
        {
            DescName = name;
        }

        public string GetToolTip()
        {
            var resList = new List<string>();
            resList.Add($"{DescName} {Level}级");
            resList.Add(Strings.TooltipDivider0);
            resList.Add($"外功防御：{PDefReduceDmg:P2} ({Final_PDef:F0})");
            resList.Add($"内功防御：{MDefReduceDmg:P2} ({Final_MDef:F0})");
            var res = String.Join("\n", resList);
            return res;
        }

        public double GetBearDef(double closingDef)
        {
            return DamageTool.BearDef(closingDef, this.DefCoef);
        }

        #region 构造函数

        public Target(int level, string descName = "", int? baseDef = null, string toolTip = "")
        {
            Level = level;
            DefCoef = BaseGlobalParams.Def * JYGlobalParams.LevelFactor(level);

            bool hasKey = GlobalData.BossBaseDefs.ContainsKey(level);
            bool incompleteInput = descName == "" || baseDef == null;

            if (!hasKey && incompleteInput)
            {
                throw new ArgumentException("无效等级！");
            }

            DescName = descName != "" ? descName : GlobalData.MuZhuangDescNames[level];
            Base_PDef = baseDef ?? GlobalData.BossBaseDefs[level];

            Final_PDef = Base_PDef;
            Final_MDef = Base_MDef = Base_PDef;
            PDef_Percent = MDef_Percent = 0.0;
            P_YS = M_YS = 0.0;

            ItemName = GetItemName();
            ToolTip = toolTip;
        }

        public Target(TargetItem item) : this(item.Level, item.ItemName, item.Def, item.ToolTip)
        {
        }

        public Target(Target old)
        {
            Level = old.Level;
            DefCoef = old.DefCoef;
            DescName = old.DescName;
            ToolTip = old.ToolTip;
            ItemName = old.ItemName;

            Base_PDef = old.Base_PDef;
            Final_PDef = old.Final_PDef;
            Base_MDef = old.Base_MDef;
            Final_MDef = old.Final_MDef;
            PDef_Percent = old.PDef_Percent;
            MDef_Percent = old.MDef_Percent;
            P_YS = old.P_YS;
            M_YS = old.M_YS;
        }

        public Target Copy()
        {
            return new Target(this);
        }


        #endregion

        #region 字符串显示输出

        public List<string> GetCatStrList(bool more = false)
        {
            List<string> res = new List<string>();
            res.Add($"{DescName}，{Level} 级目标，防御系数：{DefCoef:F2}");
            res.Add($"{Final_PDef:F0} 点外功防御，提供 {PDefReduceDmg:P2} 外功减伤");
            res.Add($"{Final_MDef:F0} 点内功防御，提供 {MDefReduceDmg:P2} 内功减伤");

            if (more)
            {
                res.Add($"{Base_PDef:F2} 点基础外功防御，{Final_PDef:F2} 点最终外功防御，{PDef_Percent:P2} 基础外防提升");
                res.Add($"{Base_MDef:F2} 点基础内功防御，{Final_MDef:F2} 点最终内功防御，{MDef_Percent:P2} 基础内防提升");
                res.Add($"{P_YS:P2} 外功易伤，{M_YS:P2} 内功易伤");
            }

            res.Add("");
            return res;
        }

        public void Cat(bool more = false)
        {
            var resL = GetCatStrList(more);
            var res = resL.StrJoin("\n");
            res.Cat();
        }

        public List<string> GetToolTip(bool cat = false)
        {
            List<string> res = new List<string>();
            res.Add($"{DescName} {Level}级");
            res.Add(Strings.TooltipDivider0);
            res.Add($"外功防御：{PDefReduceDmg:P2} ({Final_PDef:f0})");
            res.Add($"内功防御：{MDefReduceDmg:P2} ({Final_MDef:f0})");
            res.Add("");
            if (cat)
            {
                res.Cat();
            }

            return res;
        }

        #endregion

        #region 计算属性变化_各种属性

        public void Add_Final_PDef(double value)
        {
            Final_PDef += value;
        }

        public void Add_Base_PDef(double value)
        {
            Base_PDef += value;
            Final_PDef += value * (1 + PDef_Percent);
        }

        public void Add_PDef_Percent(double value)
        {
            PDef_Percent += value;
            Final_PDef += Base_PDef * value;
        }

        public void Add_Final_MDef(double value)
        {
            Final_MDef += value;
        }

        public void Add_Base_MDef(double value)
        {
            Base_MDef += value;
            Final_MDef += value * (1 + MDef_Percent);
        }

        public void Add_MDef_Percent(double value)
        {
            MDef_Percent += value;
            Final_MDef += Base_MDef * value;
        }

        public void Add_P_YS(double value)
        {
            P_YS += value;
        }

        public void Add_M_YS(double value)
        {
            M_YS += value;
        }

        /// <summary>
        /// 属性修改分派，注意这段代码是由Python程序SwitchTool.py生成的
        /// </summary>
        /// <param name="key">简化后的属性名称</param>
        /// <param name="value">属性值</param>
        public void AddSAttr(string key, double value)
        {
            switch (key)
            {
                case "Final_PDef":
                {
                    Add_Final_PDef(value);
                    break;
                }
                case "Base_PDef":
                {
                    Add_Base_PDef(value);
                    break;
                }
                case "PDef_Percent":
                {
                    Add_PDef_Percent(value);
                    break;
                }
                case "Final_MDef":
                {
                    Add_Final_MDef(value);
                    break;
                }
                case "Base_MDef":
                {
                    Add_Base_MDef(value);
                    break;
                }
                case "MDef_Percent":
                {
                    Add_MDef_Percent(value);
                    break;
                }
                case "P_YS":
                {
                    Add_P_YS(value);
                    break;
                }
                case "M_YS":
                {
                    Add_M_YS(value);
                    break;
                }
                default:
                {
                    Trace.WriteLine($"未知的属性！ {key}:{value} ");
                    break;
                }
            }
        }

        #endregion

        #region 属性计算


        public void AddSAtKVP(KeyValuePair<string, double> kvp)
        {
            AddSAttr(kvp.Key, kvp.Value);
        }

        public void AddSAttrDict(IDictionary<string, double> dict)
        {
            if (dict != null)
            {
                foreach (var kvp in dict)
                {
                    AddSAtKVP(kvp);
                }
            }
        }

        public void AddCharAttrCollection(AttrCollection charAttrs)
        {
            if (charAttrs!=null && !charAttrs.IsEmptyOrNull && charAttrs.Simplified) // 仅仅允许添加已简化后的属性
            {
                AddSAttrDict(charAttrs.Values);
            }
        }

        public void AddNamedAttrs(NamedAttrs attrs)
        {
            AddCharAttrCollection(attrs.Attr);
        }

        #endregion

        #region 产生示例样例

        public static Target GetSample(bool cat = false)
        {
            Target target = new Target(113, "");

            if (cat)
            {
                target.Cat(true);
            }

            return target;
        }

        #endregion

        #region 制作木桩数据库

        public static ImmutableDictionary<int, Target> GeneratorMuZhuang()
        {
            var MZ_Dict = new Dictionary<int, Target>();
            foreach (int level in GlobalData.BossBaseDefs.Keys)
            {
                MZ_Dict[level] = new Target(level, "");
            }

            return MZ_Dict.ToImmutableDictionary();
        }

        #endregion

        public IList<string> GetCatStrList()
        {
            return GetCatStrList(false);
        }

        public string ToStr()
        {
            var res = GetCatStrList();
            return string.Join("\n", res);
        }

        public void Cat()
        {
            Cat(false);
        }
    }

    public static partial class DataBase
    {
        public static readonly ImmutableDictionary<int, Target> MuZhuangs = Target.GeneratorMuZhuang();
    }
}