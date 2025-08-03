using System.Collections.Generic;
using System.Linq;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Class;

namespace JX3CalculatorShared.Src
{
    /// <summary>
    /// 存储一些需要传入的参数
    /// </summary>
    public class CalculatorShellArg
    {
        public BigFMConfigArg BigFM { get; }
        public int HS { get; } // 当前加速值
        public BuffSpecialArg BuffSpecial { get; }
        public bool AllSkillMiJiIsSupport => RecipeCompatibleSkillBuilds.Length > 0; // 秘籍是否支持
        public SkillBuild[] RecipeCompatibleSkillBuilds { get; }
        public bool EnableOptimization { get; } // 是否启用优化建议
        public double ShortTimeBonus { get; } // 短时间战斗红利系数
        public bool TargetAllWaysFullHP { get; } // 目标永远满血
        public EquipSpecialEffectConfigArg EquipSpecialEffectConfig { get; }

        public CalculatorShellArg(IEnumerable<SkillBuild> recipeCompatibleSkillBuilds, int hs,
            bool opt, BigFMConfigArg bigFM,
            BuffSpecialArg buffSpecial, EquipSpecialEffectConfigArg equipSpecialEffectConfig,
            bool targetAllWaysFullHP = false, double shortTimeBonus = 0.0)
        {
            RecipeCompatibleSkillBuilds = recipeCompatibleSkillBuilds.ToArray();
            BigFM = bigFM;
            HS = hs;
            EnableOptimization = opt;
            BuffSpecial = buffSpecial;
            ShortTimeBonus = shortTimeBonus;
            EquipSpecialEffectConfig = equipSpecialEffectConfig;
            TargetAllWaysFullHP = targetAllWaysFullHP;
        }

        public CalculatorShellArg()
        {
        }

        public CalculatorShellArg(CalculatorShellArg old)
        {
            RecipeCompatibleSkillBuilds = old.RecipeCompatibleSkillBuilds.ToArray();
            BigFM = old.BigFM;
            HS = old.HS;
            EnableOptimization = old.EnableOptimization;
            BuffSpecial = old.BuffSpecial;
            ShortTimeBonus = old.ShortTimeBonus;
            TargetAllWaysFullHP = old.TargetAllWaysFullHP;
            EquipSpecialEffectConfig = old.EquipSpecialEffectConfig;
        }

        public CalculatorShellArg Copy()
        {
            return new CalculatorShellArg(this);
        }
    }
}