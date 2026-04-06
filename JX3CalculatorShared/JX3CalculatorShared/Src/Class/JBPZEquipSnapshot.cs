using MingYiTools;

namespace JX3CalculatorShared.Class
{
    public class JBPZEquipSnapshot
    {
        public string id { get; set; } // 装备ID 6_31719
        public int? stone { get; set; } = -1; // 五彩石ID
        public int? enchant { get; set; } = -1; // 大附魔信息
        public int? enhance { get; set; } = -1; // 小附魔信息
        public int strength { get; set; } = 0; // 精炼等级
        public int[] embedding { get; set; } = null; // 装备镶嵌

        public JBPZEquipSnapshot()
        {
        }

        /// <summary>
        /// 基于茗伊数据构建
        /// </summary>
        /// <param name="item"></param>
        public JBPZEquipSnapshot(MingYiItemData item)
        {
            id = $"{item.dwTabType}_{item.dwTabIndex}";
            stone = item.dwItemFEAEnchantID;
            enchant = item.dwTemporaryEnchantID;
            enhance = item.dwPermanentEnchantID;
            strength = item.nStrengthLevel ?? 0;
            embedding = item.GetDiamondLevelArray();
        }

        /// <summary>
        /// 基于茗伊数据库构建
        /// </summary>
        /// <param name="item"></param>
        public JBPZEquipSnapshot(MingYiEquipItemEntity item)
        {
            id = $"{item.TabType}_{item.TabIndex}";
            stone = item.FeaEnchant;
            enchant = item.TemporaryEnchant;
            enhance = item.PermanentEnchant;
            strength = item.Strength;
            embedding = item.GetDiamondLevelArray();
        }

    }
}