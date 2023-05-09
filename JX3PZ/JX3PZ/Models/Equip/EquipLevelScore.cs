namespace JX3PZ.Models
{
    public class EquipLevelScore : EquipScore
    {
        // 用于描述传递装分和品级的类
        public readonly int Level; // 基础品级
        public readonly int StrengthLevel; // 精炼带来的品级提升

        public EquipLevelScore(int level = 0, int strengthLevel = 0,
            int baseScore = 0, int strengthScore = 0, int enhanceScore = 0) :
            base(baseScore, strengthScore, enhanceScore)
        {
            Level = level;
            StrengthLevel = strengthLevel;
        }
    }

    public class EquipScore
    {
        public readonly int BaseScore; // 基础装分
        public readonly int StrengthScore; // 精炼带来的装分提升
        public readonly int EnhanceScore; // 镶嵌+附魔装分
        public readonly int TotalScore; // 总装分

        public EquipScore(int baseScore = 0, int strengthScore = 0, int enhanceScore = 0)
        {
            BaseScore = baseScore;
            StrengthScore = strengthScore;
            EnhanceScore = enhanceScore;
            TotalScore = BaseScore + StrengthScore + EnhanceScore;
        }

        public string GetToolTip()
        {
            var res =
                $"装备分数 {TotalScore}\r\n\r\n装备分数可以通过提升装备品质、精炼装备栏、熔嵌五行石至装备栏上，\r\n以及熔嵌五彩石和使用附魔至装备上来提升。\r\n\r\n基础分数: {BaseScore}\r\n精炼分数: {StrengthScore}\r\n熔嵌和附魔分数: {EnhanceScore}";
            return res;
        }
    }
}