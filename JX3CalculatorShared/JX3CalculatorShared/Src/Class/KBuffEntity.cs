using JX3CalculatorShared.Src;

namespace JX3CalculatorShared.Class
{
    public class KBuffEntity
    {
        // 表示游戏内Buff实际数值效果的类
        public string Name;
        public string BuffID;
        public double BuffCover;
        public double Stack;
        public KAttributeEntry[] KAttributeEntries;
    }
}