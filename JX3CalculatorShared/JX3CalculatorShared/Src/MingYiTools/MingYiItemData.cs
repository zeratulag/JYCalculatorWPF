using JX3PZ.Src;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Newtonsoft.Json;

namespace MingYiTools
{
    public class MingYiItemData
    {
        // 基础属性 (使用 int? 允许为空，对应 Lua 中可能缺省的字段)
        public int? nPos { get; set; }
        public int? nStrengthLevel { get; set; } // 精炼等级
        public int? nStackNum { get; set; }
        public int? nDurability { get; set; }
        public int? nMaxDurability { get; set; }
        public int? nMaxStrengthLevel { get; set; } // 最大精炼等级
        public int? nQuality { get; set; } // 品质, 5橙

        // ID 类字段建议使用 long (防止 unsigned int 溢出)
        public int? dwTabIndex { get; set; } // 物品Tab ID
        public int? dwTabType { get; set; } // Tab类型，678
        public int? dwTabSubIndex { get; set; }
        public int? dwItemFEAEnchantID { get; set; } // 五彩石ID
        public int? dwTemporaryEnchantID { get; set; } // 大附魔ID
        public int? dwPermanentEnchantID { get; set; } // 小附魔ID

        // 列表类型 (例如五行石)
        public List<int> aDiamondEnchant { get; set; } = new List<int>(); // 五行石附魔ID列表

        public static readonly ImmutableArray<string> POS_TO_NAME_ARR = new[]
        {
            "PRIMARY_WEAPON", "UNKNOW_1", "SECONDARY_WEAPON", "JACKET", "HAT", // 0, 1, 2, 3, 4
            "NECKLACE", "RING_1", "RING_2", "BELT", "PENDANT", // 5, 6, 7, 8, 9
            "BOTTOMS", "SHOES", "WRIST", "BULLETS" // 10, 11, 12, 13
        }.ToImmutableArray();


        [JsonIgnore]
        public string PositionName
        {
            get
            {
                int idx = nPos ?? 1; // 保持原始默认行为：null -> 1
                if (idx < 0 || idx >= POS_TO_NAME_ARR.Length)
                {
                    return $"UNKNOW_{idx}";
                }
                return POS_TO_NAME_ARR[idx];
            }
        }

        public int[] GetDiamondLevelArray()
        {
            // 解析五行石等级
            return aDiamondEnchant.Select(StaticPzData.Data.DiamondDB.GetDiamondLevelFromEnchantID).ToArray();
        }
    }
}