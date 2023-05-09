using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JX3CalculatorShared.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JX3CalculatorShared.DB
{
    public class BigFMDBBase : IDB<string, BigFM>
    {
        public ImmutableDictionary<string, BigFM> Data;

        public static readonly ImmutableArray<EquipSubTypeEnum> SubTypeOrder = new EquipSubTypeEnum[]
        {
            //用于分类排序存储各种大附魔的数组
            EquipSubTypeEnum.HAT,
            EquipSubTypeEnum.JACKET,
            EquipSubTypeEnum.BELT,
            EquipSubTypeEnum.WRIST,
            EquipSubTypeEnum.SHOES,
            //EquipSubTypeEnum.BOTTOMS
        }.ToImmutableArray();

        public ImmutableDictionary<int, int> ID2UIID; // 大附魔ID到物品ID的映射关系

        public BigFMDBBase(IEnumerable<Enchant> itemdata, IEnumerable<BottomsFMItem> bottomsdata)
        {
            var data1 = itemdata.ToDictionary(_ => _.Name,
                _ => new BigFM(_));
            var data2 = bottomsdata.ToDictionary(_ => _.Name,
                _ => new BigFM(_));

            foreach (var kvp in data2)
            {
                if (kvp.Value.Rank <= 0) continue;
                data1.Add(kvp.Key, kvp.Value);
            }

            Data = data1.ToImmutableDictionary();
            DispatchBigFM();

            var e2i = new Dictionary<int, int>(Data.Count);
            foreach (var _ in Data.Values)
            {
                if (e2i.ContainsKey(_.ID)) continue;
                e2i.Add(_.ID, _.UIID);
            }

            ID2UIID = e2i.ToImmutableDictionary();

        }

        public ImmutableArray<BigFM> Jacket { get; private set; }
        public ImmutableArray<BigFM> Hat { get; private set; }
        public ImmutableArray<BigFM> Belt { get; private set; }
        public ImmutableArray<BigFM> Shoes { get; private set; }
        public ImmutableArray<BigFM> Wrist { get; private set; }
        public ImmutableArray<BigFM> Bottoms { get; private set; }
        public ImmutableDictionary<EquipSubTypeEnum, ImmutableArray<BigFM>> TypeData { get; private set; } // 按照不同类型分别构建大附魔列表的字典
        public static IDictionary<EquipSubTypeEnum, BigFM[]> GroupBigFM(IEnumerable<BigFM> BigFMs)
        {
            var res = BigFMs.GroupBy(_ => _.SubType).
                ToDictionary(
                    g => g.Key,
                    g => g.
                        OrderBy(_ => _.DLCLevel).
                        ThenBy(_ => _.Rank).
                        ThenBy(_ => _.LevelMax).
                        ThenBy(_ => _.ID).
                        ToArray());
            return res;
        }

        public IDictionary<EquipSubTypeEnum, BigFM[]> GroupBigFM()
        {
            var dict = GroupBigFM(Data.Values);
            return dict;
        }

        /// <summary>
        /// 根据大附魔类型，将大附魔分别存储到数组中，用作下拉框用
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void DispatchBigFM()
        {
            var Dict = GroupBigFM();
            var bigFMDict = new Dictionary<EquipSubTypeEnum, ImmutableArray<BigFM>>();
            foreach (var KVP in Dict)
            {
                var valueArray = KVP.Value.ToImmutableArray();
                switch (KVP.Key)
                {
                    case EquipSubTypeEnum.JACKET:
                        { Jacket = valueArray; break; }

                    case EquipSubTypeEnum.HAT:
                        { Hat = valueArray; break; }

                    case EquipSubTypeEnum.BELT:
                        { Belt = valueArray; break; }

                    case EquipSubTypeEnum.SHOES:
                        { Shoes = valueArray; break; }

                    case EquipSubTypeEnum.WRIST:
                        { Wrist = valueArray; break; }
                }

                if (KVP.Key != EquipSubTypeEnum.BOTTOMS) bigFMDict.Add(KVP.Key, valueArray);
            }
            TypeData = bigFMDict.ToImmutableDictionary(data => data.Key, data => data.Value.ToImmutableArray());
        }

        public BigFM Get(string name)
        {
            var res = Data[name];
            return res;
        }

        public BigFM this[string name] => Get(name);

        /// <summary>
        /// 基于EnchantID确定ItemID
        /// </summary>
        /// <param name="enchantId">附魔ID</param>
        /// <returns></returns>
        public int GetItemID(int enchantId)
        {
            var res = ID2UIID.GetValueOrUseDefault(enchantId, -1);
            return res;
        }
    }
}