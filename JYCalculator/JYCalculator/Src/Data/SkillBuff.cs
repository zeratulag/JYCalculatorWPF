using System;
using System.Collections.Generic;
using JX3CalculatorShared.Class;
using JYCalculator.Globals;
using JYCalculator.Src.Class;

namespace JYCalculator.Data
{

    public static class SkillBuffFactory
    {
        public static readonly Dictionary<string, SkillBuff> Dict = new Dictionary<string, SkillBuff>(12);

        public static SkillBuff Create(string skillBuffStr)
        {
            if (Dict.TryGetValue(skillBuffStr, out var skillBuff))
            {
                return skillBuff;
            }
            else
            {
                skillBuff = new SkillBuff(skillBuffStr);
                Dict.Add(skillBuffStr, skillBuff);
                return skillBuff;
            }
        }

        public static void AfterParse()
        {
            foreach (var skillBuff in Dict.Values)
            {
                skillBuff.AfterParse();
            }
        }
    }


    [Equals]
    public class SkillBuff : IComparable<SkillBuff>, IComparable
    {
        public int ID { get; private set; } // 技能的唯一标识符
        public int Level { get; private set; } // 技能的等级
        public int Stack { get; private set; } = 1; // 技能的叠加层数（默认为 1）

        // 构造函数：从传入的参数直接初始化 SkillBuff
        public SkillBuff(int id, int level, int stack = 1)
        {
            ID = id;
            Level = level;
            Stack = stack;
        }

        // 构造函数：从字符串格式解析 SkillBuff
        public SkillBuff(string skillBuffStr)
        {
            if (string.IsNullOrWhiteSpace(skillBuffStr))
            {
                throw new ArgumentException("输入字符串不能为空或仅包含空格。", nameof(skillBuffStr));
            }

            try
            {
                // 按 'x' 分隔字符串，分为两部分：ID_Level 和 Stack
                var parts = skillBuffStr.Split('x');
                if (parts.Length == 0 || parts.Length > 2)
                {
                    throw new FormatException("输入字符串格式无效，应为 {ID}_{Level}x{Stack}。");
                }

                // 提取 ID 和 Level
                var idAndLevel = parts[0].Split('_');
                if (idAndLevel.Length != 2)
                {
                    throw new FormatException("输入字符串格式无效，应为 {ID}_{Level}。");
                }

                ID = int.Parse(idAndLevel[0]); // 解析 ID
                Level = int.Parse(idAndLevel[1]); // 解析 Level

                // 如果存在 Stack 部分，解析它；如果不存在，默认为 1
                Stack = parts.Length == 2 ? int.Parse(parts[1]) : 1;
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentException ||
                                       ex is IndexOutOfRangeException)
            {
                throw new FormatException($"解析 SkillBuff 字符串失败：{skillBuffStr}。", ex);
            }
        }

        // 将对象转为字符串格式
        public new string ToString() => $"{ID}_{Level}x{Stack}";

        [IgnoreDuringEquals]
        public string BuffID => $"{ID}_{Level}";

        [IgnoreDuringEquals]
        public KBuff Buff { get; private set; }

        [IgnoreDuringEquals]
        public BuffToRecipeItem BuffToRecipe { get; private set; }

        public void FindBuff()
        {
            Buff = StaticXFData.DB.Buff.QueryBuffByID(BuffID);
        }

        public void AfterParse()
        {
            FindBuff();
            FindBuffToRecipe();
        }

        private void FindBuffToRecipe()
        {
            BuffToRecipe = StaticXFData.Data.BuffToRecipe[BuffID];
        }

        #region 自动生成的

        // 显式定义 == 运算符
        public static bool operator ==(SkillBuff left, SkillBuff right) => Operator.Weave(left, right);

        // 显式定义 != 运算符
        public static bool operator !=(SkillBuff left, SkillBuff right) => Operator.Weave(left, right);

        public int CompareTo(SkillBuff other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;
            var idComparison = ID.CompareTo(other.ID);
            if (idComparison != 0) return idComparison;
            var levelComparison = Level.CompareTo(other.Level);
            if (levelComparison != 0) return levelComparison;
            return Stack.CompareTo(other.Stack);
        }

        public int CompareTo(object obj)
        {
            if (obj is null) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            return obj is SkillBuff other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(SkillBuff)}");
        }

        public static bool operator <(SkillBuff left, SkillBuff right)
        {
            return Comparer<SkillBuff>.Default.Compare(left, right) < 0;
        }

        public static bool operator >(SkillBuff left, SkillBuff right)
        {
            return Comparer<SkillBuff>.Default.Compare(left, right) > 0;
        }

        public static bool operator <=(SkillBuff left, SkillBuff right)
        {
            return Comparer<SkillBuff>.Default.Compare(left, right) <= 0;
        }

        public static bool operator >=(SkillBuff left, SkillBuff right)
        {
            return Comparer<SkillBuff>.Default.Compare(left, right) >= 0;
        }

        #endregion
    }
}