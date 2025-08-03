using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Utils;

namespace JYCalculator.Data
{
    public class SkillBuffGroup
    {
        public ImmutableArray<SkillBuff> SkillBuffs { get; }

        public string Key { get; }

        public ImmutableArray<Recipe> Recipes;
        public ImmutableArray<SkillModifier> SkillModifiers;

        public bool IsEmpty => SkillBuffs.Length == 0; // 是否为空

        public static SkillBuff[] UniqueSorted(IEnumerable<SkillBuff> buffs)
        {
            SkillBuff[] result = buffs
                .Distinct() // 去重，基于 Equals 和 GetHashCode
                .OrderBy(buff => buff) // 排序，基于 IComparable<SkillBuff>
                .ToArray(); // 转换为数组
            return result;
        }

        public SkillBuffGroup(ImmutableArray<SkillBuff> buffs)
        {
            SkillBuffs = buffs;
            Key = ToStr();
        }

        public void Parse()
        {
            Recipes = SkillBuffs.Select(buff => buff.BuffToRecipe.CRecipe).ToImmutableArray();
            SkillModifiers = SkillBuffs.Select(buff => buff.BuffToRecipe.CModifier).ToImmutableArray();
        }

        public SkillBuffGroup(IEnumerable<SkillBuff> buffs) : this(UniqueSorted(buffs).ToImmutableArray())
        {
        }

        public string ToStr()
        {
            var str_list = SkillBuffs.Select(_ => _.ToString());
            var res = str_list.StrJoin("|");
            return res;
        }

        // 当做为技能Key时的后缀
        public string GetSkillKeySuffix()
        {
            return $"[{ToStr()}]";
        }
    }
}