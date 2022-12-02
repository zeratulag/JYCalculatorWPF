using JX3CalculatorShared.Common;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.Src.Data;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JX3CalculatorShared.Src.DB
{
    public class ItemDTDBBase: IDB<string, ItemDT>
    {
        public ImmutableDictionary<string, ItemDT> Data;

        public ItemDTDBBase(IEnumerable<ItemDTItem> itemdata)
        {
            Data = itemdata.ToImmutableDictionary(
                _ => _.Name,
                _ => new ItemDT(_));
            DispatchItemDT();
        }

        public ImmutableArray<ItemDT> FoodSupport { get; private set; }
        public ImmutableArray<ItemDT> FoodEnhance { get; private set; }
        public ImmutableArray<ItemDT> MedSupport { get; private set; }
        public ImmutableArray<ItemDT> MedEnhance { get; private set; }
        public ImmutableArray<ItemDT> HomeWine { get; private set; }
        public ImmutableArray<ItemDT> HomeCook { get; private set; }
        public ImmutableArray<ItemDT> WeaponWhetstone { get; private set; }
        public ImmutableDictionary<ItemDTTypeEnum, ImmutableArray<ItemDT>> TypeData { get; private set; } // 按照不同类型分别构建单体列表的字典

        public static IDictionary<ItemDTTypeEnum, ItemDT[]> GroupItemDT(IEnumerable<ItemDT> itemDTs)
        {
            var res = itemDTs.GroupBy(_ => _.Type).
                ToDictionary(
                    g => g.Key,
                    g => g.
                        OrderBy(
                            _ => _.Quality).
                        ThenBy(_ => _.SimpleAttr.Key).
                        ThenBy(_ => _.SimpleAttr.Value).
                        ToArray());
            return res;
        }

        public IDictionary<ItemDTTypeEnum, ItemDT[]> GroupItemDT()
        {
            return GroupItemDT(Data.Values);
        }

        /// <summary>
        /// 根据单体类型，将单体分别存储到数组中，用作下拉框用
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void DispatchItemDT()
        {
            var Dict = GroupItemDT();
            foreach (var KVP in Dict)
            {
                var valueArray = KVP.Value.ToImmutableArray();
                switch (KVP.Key)
                {
                    case ItemDTTypeEnum.FoodSupport: 
                    { FoodSupport = valueArray; break; }

                    case ItemDTTypeEnum.FoodEnhance:
                    { FoodEnhance = valueArray; break; }

                    case ItemDTTypeEnum.MedSupport:
                    { MedSupport = valueArray; break; }

                    case ItemDTTypeEnum.MedEnhance:
                    { MedEnhance = valueArray; break; }

                    case ItemDTTypeEnum.HomeWine:
                    { HomeWine = valueArray; break; }

                    case ItemDTTypeEnum.HomeCook:
                    { HomeCook = valueArray; break; }

                    case ItemDTTypeEnum.WeaponWhetstone:
                    { WeaponWhetstone = valueArray; break; }

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            TypeData = Dict.ToImmutableDictionary(data => data.Key, data => data.Value.ToImmutableArray());
        }

        public ItemDT Get(string name)
        {
            var res = Data[name];
            return res;
        }

        public ItemDT this[string name] => Get(name);
    }
}