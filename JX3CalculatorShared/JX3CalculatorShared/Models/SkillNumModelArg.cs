namespace JX3CalculatorShared.Models
{
    public readonly struct SkillNumModelArg
    {
        public readonly bool HasZhen; // 是否有阵法
        public readonly double PiaoHuangCover; // 飘黄BUFF覆盖率
        public readonly int HS; // 加速值
        public readonly double ShortTimeBonus;

        public SkillNumModelArg(bool hasZhen, double piaoHuangCover, int hs, double shortTimeBonus = 0.0)
        {
            HasZhen = hasZhen;
            PiaoHuangCover = piaoHuangCover;
            HS = hs;
            ShortTimeBonus = shortTimeBonus;
        }
    }
}