﻿using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using JYCalculator.Class;

namespace JYCalculator.Utils
{
    public static class PeriodClassTool
    {
        public static SkillFreqCTs[] GetSkillFreqCTTable(this Period<SkillFreqCTDF> SkillFreqCTDf)
        {
            return SkillFreqCTs.GetSkillFreqCTTable(SkillFreqCTDf);
        }

        public static void ApplyRecipe(this Period<SkillDataDF> skillDfs, Recipe normal, Recipe xw)
        {
            // 添加并应用秘籍
            skillDfs.Normal.AddRecipeAndApply(normal);
            skillDfs.XinWu.AddRecipeAndApply(xw);
        }

        public static void ApplySkillModifier(this Period<SkillDataDF> skillDfs, SkillModifier normal, SkillModifier xw)
        {
            // 添加应用SkillModifier
            skillDfs.Normal.ApplySkillModifier(normal);
            skillDfs.XinWu.ApplySkillModifier(xw);
        }

        public static Period<SkillFreqCTDF> Copy(this Period<SkillFreqCTDF> old)
        {
            return new Period<SkillFreqCTDF>(old.Normal?.Copy(), old.XinWu?.Copy());
        }
    }
}