using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace JX3CalculatorShared.Src.Class
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

    public class JBPZPlanBase
    {
        public JBPZEquipSnapshot HAT; // 帽子
        public JBPZEquipSnapshot BELT; // 腰带
        public JBPZEquipSnapshot SHOES; // 鞋子
        public JBPZEquipSnapshot WRIST; // 护腕
        public JBPZEquipSnapshot JACKET; // 上衣
        public JBPZEquipSnapshot RING_1; // 戒指1
        public JBPZEquipSnapshot RING_2; // 戒指2
        public JBPZEquipSnapshot BOTTOMS; // 下装
        public JBPZEquipSnapshot PENDANT; // 腰坠
        public JBPZEquipSnapshot NECKLACE; // 项链
        public JBPZEquipSnapshot PRIMARY_WEAPON; // 近身武器
        public JBPZEquipSnapshot SECONDARY_WEAPON; // 远程武器
        public JBPZEquipSnapshot TERTIARY_WEAPON; // 近身武器（藏剑重兵）
        [JsonIgnore] public ImmutableDictionary<string, JBPZEquipSnapshot> Dict; // 以Dict形式存储的

        [JsonIgnore] public bool Had_BigFM_hat; // 是否有帽子大附魔

        [JsonIgnore] public bool Had_BigFM_jacket; // 是否有上衣大附魔

        public void GetDict()
        {
            var data = new Dictionary<string, JBPZEquipSnapshot>()
            {
                {nameof(HAT), HAT},
                {nameof(BELT), BELT},
                {nameof(SHOES), SHOES},
                {nameof(WRIST), WRIST},
                {nameof(JACKET), JACKET},
                {nameof(RING_1), RING_1},
                {nameof(RING_2), RING_2},
                {nameof(BOTTOMS), BOTTOMS},
                {nameof(PENDANT), PENDANT},
                {nameof(NECKLACE), NECKLACE},
                {nameof(PRIMARY_WEAPON), PRIMARY_WEAPON},
                {nameof(SECONDARY_WEAPON), SECONDARY_WEAPON},
                {nameof(TERTIARY_WEAPON), TERTIARY_WEAPON},
            };
            Dict = data.ToImmutableDictionary();
        }
    }
}