using JX3CalculatorShared.Class;

namespace JX3CalculatorShared.Models
{
    public readonly struct SkillNumModelArg
    {
        public readonly bool HasZhen; // 是否有阵法
        public readonly double PiaoHuangCover; // 飘黄BUFF覆盖率
        public readonly int PiaoHuangStack; // 飘黄BUFF层数
        public readonly int HS; // 加速值
        public readonly double ShortTimeBonus;
        public readonly SkillBuild CSkillBuild; // 技能流派
        public readonly bool TargetAllWaysFullHP; // 目标永远满血

        public SkillNumModelArg(bool hasZhen, double piaoHuangCover, int piaoHuangStack, int hs,
            SkillBuild cSkillBuild,
            bool targetAllWaysFullHp = false,
            double shortTimeBonus = 0.0)
        {
            HasZhen = hasZhen;
            PiaoHuangCover = piaoHuangCover;
            PiaoHuangStack = piaoHuangStack;
            HS = hs;
            CSkillBuild = cSkillBuild;
            ShortTimeBonus = shortTimeBonus;
            TargetAllWaysFullHP = targetAllWaysFullHp;
        }
    }
}