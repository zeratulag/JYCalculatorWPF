using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace JX3CalculatorShared.Class
{
    public class JBPZEquipSnapshotCollection
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

        [JsonIgnore] public bool Had_BigFM_hat => HAT.enchant > 0; // 是否有帽子大附魔
        [JsonIgnore] public bool Had_BigFM_jacket => JACKET.enchant > 0; // 是否有上衣大附魔

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

        public JBPZEquipSnapshotCollection()
        {
        }


        public JBPZEquipSnapshotCollection(IDictionary<string, JBPZEquipSnapshot> data)
        {
            data.TryGetValue(nameof(HAT), out HAT);
            data.TryGetValue(nameof(BELT), out BELT);
            data.TryGetValue(nameof(SHOES), out SHOES);
            data.TryGetValue(nameof(WRIST), out WRIST);
            data.TryGetValue(nameof(JACKET), out JACKET);
            data.TryGetValue(nameof(RING_1), out RING_1);
            data.TryGetValue(nameof(RING_2), out RING_2);
            data.TryGetValue(nameof(BOTTOMS), out BOTTOMS);
            data.TryGetValue(nameof(PENDANT), out PENDANT);
            data.TryGetValue(nameof(NECKLACE), out NECKLACE);
            data.TryGetValue(nameof(PRIMARY_WEAPON), out PRIMARY_WEAPON);
            data.TryGetValue(nameof(SECONDARY_WEAPON), out SECONDARY_WEAPON);
            data.TryGetValue(nameof(TERTIARY_WEAPON), out TERTIARY_WEAPON);

            Dict = data.ToImmutableDictionary();
        }

    }
}