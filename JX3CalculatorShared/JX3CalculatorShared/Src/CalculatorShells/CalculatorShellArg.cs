using JX3CalculatorShared.ViewModels;

namespace JX3CalculatorShared.Src
{
    /// <summary>
    /// 存储一些需要传入的参数
    /// </summary>
    public class CalculatorShellArg
    {
        public BigFMConfigArg BigFM;
        public int HS; // 当前加速值
        public BuffSpecialArg BuffSpecial;
        public bool AllSkillMiJiIsSupport; // 秘籍是否支持

        public bool EnableOptimization; // 是否启用优化建议
        public double ShortTimeBonus; // 短时间战斗红利系数

        public CalculatorShellArg(bool allSkillMiJiIsSupport, int hs, bool opt, BigFMConfigArg bigFM,
            BuffSpecialArg buffSpecial, double shortTimeBonus = 0.0)
        {
            BigFM = bigFM;
            HS = hs;
            EnableOptimization = opt;
            BuffSpecial = buffSpecial;
            AllSkillMiJiIsSupport = allSkillMiJiIsSupport;
            ShortTimeBonus = shortTimeBonus;
        }

        public CalculatorShellArg()
        {
        }

        public CalculatorShellArg(CalculatorShellArg old)
        {
            BigFM = old.BigFM;
            HS = old.HS;
            EnableOptimization = old.EnableOptimization;
            BuffSpecial = old.BuffSpecial;
            AllSkillMiJiIsSupport = old.AllSkillMiJiIsSupport;
            ShortTimeBonus = old.ShortTimeBonus;
        }

        public CalculatorShellArg Copy()
        {
            return new CalculatorShellArg(this);
        }
    }
}