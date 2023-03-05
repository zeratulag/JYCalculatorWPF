namespace JX3CalculatorShared.Class
{
    public class JBPZEquipSnapshot
    {
        public string id { get; set; } // 装备ID 6_31719
        public int? stone { get; set; } = -1; // 五彩石ID
        public int? enchant { get; set; } = -1; // 大附魔信息
        public int? enhance { get; set; } = 1; // 小附魔信息
        public int strength { get; set; } = 0; // 精炼等级
        public int[] embedding { get; set; } // 装备镶嵌
    }
}