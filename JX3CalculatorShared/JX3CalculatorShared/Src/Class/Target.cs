using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

// ReSharper disable InconsistentNaming

namespace JX3CalculatorShared.Class
{
    public class Target : ObservableObject, ICatable
    {
        #region 成员

        public int Level { get; }
        public double DefCoef { get; } // 防御系数
        public string DescName { get; private set; }
        [ExcelIgnore] public string RealDescName => DescName;
        [ExcelIgnore] public string ToolTip { get; }
        public string ItemName { get; }

        public double PhysicsBaseShield { get; set; } // 基础外防
        public double PhysicsFinalShield { get; set; } // 最终外防
        public double MagicBaseShield { get; set; } // 基础内防
        public double MagicFinalShield { get; set; } // 最终内防
        public double PhysicsShieldPercent { get; set; } = 0.0; // +外防
        public double MagicShieldPercent { get; set; } = 0.0; // +内防
        public double PhysicsDamageCoefficient { get; set; } = 1.0; // 外功承伤
        public double MagicDamageCoefficient { get; set; } = 1.0; // 内功承伤

        #endregion

        public double PhysicsShieldReduceDamageValue => DamageTool.DefReduceDmg(PhysicsFinalShield, DefCoef); // 外防减伤值

        public double MagicShieldReduceDamageValue => DamageTool.DefReduceDmg(MagicFinalShield, DefCoef); // 内防减伤值值


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
            resList.Add(StringConsts.TooltipDivider0);
            resList.Add($"外功防御：{PhysicsShieldReduceDamageValue:P2} ({PhysicsFinalShield:F0})");
            resList.Add($"内功防御：{MagicShieldReduceDamageValue:P2} ({MagicFinalShield:F0})");
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
            DefCoef = BaseGlobalParams.Shield * GlobalParams.LevelFactor(level);

            bool hasKey = GlobalData.BossBaseDefs.ContainsKey(level);
            bool incompleteInput = descName == "" || baseDef == null;

            if (!hasKey && incompleteInput)
            {
                throw new ArgumentException("无效等级！");
            }

            DescName = descName != "" ? descName : GlobalData.MuZhuangDescNames[level];
            PhysicsBaseShield = baseDef ?? GlobalData.BossBaseDefs[level];

            PhysicsFinalShield = PhysicsBaseShield;
            MagicFinalShield = MagicBaseShield = PhysicsBaseShield;

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

            PhysicsBaseShield = old.PhysicsBaseShield;
            PhysicsFinalShield = old.PhysicsFinalShield;
            MagicBaseShield = old.MagicBaseShield;
            MagicFinalShield = old.MagicFinalShield;
            PhysicsShieldPercent = old.PhysicsShieldPercent;
            MagicShieldPercent = old.MagicShieldPercent;
            PhysicsDamageCoefficient = old.PhysicsDamageCoefficient;
            MagicDamageCoefficient = old.MagicDamageCoefficient;
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
            res.Add($"{PhysicsFinalShield:F0} 点外功防御，提供 {PhysicsShieldReduceDamageValue:P2} 外功减伤");
            res.Add($"{MagicFinalShield:F0} 点内功防御，提供 {MagicShieldReduceDamageValue:P2} 内功减伤");

            if (more)
            {
                res.Add(
                    $"{PhysicsBaseShield:F2} 点基础外功防御，{PhysicsFinalShield:F2} 点最终外功防御，{PhysicsShieldPercent:P2} 基础外防提升");
                res.Add($"{MagicBaseShield:F2} 点基础内功防御，{MagicFinalShield:F2} 点最终内功防御，{MagicShieldPercent:P2} 基础内防提升");
                res.Add($"{PhysicsDamageCoefficient:P2} 外功承伤，{MagicDamageCoefficient:P2} 内功承伤");
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
            res.Add(StringConsts.TooltipDivider0);
            res.Add($"外功防御：{PhysicsShieldReduceDamageValue:P2} ({PhysicsFinalShield:f0})");
            res.Add($"内功防御：{MagicShieldReduceDamageValue:P2} ({MagicFinalShield:f0})");
            res.Add("");
            if (cat)
            {
                res.Cat();
            }

            return res;
        }

        #endregion

        #region 属性计算

        public void AddSAtKVP(KeyValuePair<string, double> kvp)
        {
            //AddSAttr(kvp.Key, kvp.Value);
            this.ProcessZAttr(kvp.Key, kvp.Value);
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
            if (charAttrs != null && !charAttrs.IsEmptyOrNull && charAttrs.Simplified) // 仅仅允许添加已简化后的属性
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
    public class TargetViewModel
    {
        public int Level { get; }
        public string DescName { get; }
        public bool AllWaysFullHP { get; }
        public string RealDescName { get; }

        public TargetViewModel(Target CTarget, bool allWaysFullHP)
        {
            DescName = CTarget.DescName;
            Level = CTarget.Level;
            AllWaysFullHP = allWaysFullHP;
            RealDescName = GetFullDescName();
        }

        public string GetFullDescName()
        {
            if (AllWaysFullHP)
            {
                return $"{DescName}(满血)";
            }
            return DescName;
        }
    }
}