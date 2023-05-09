using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JX3CalculatorShared.Messages;

namespace JX3CalculatorShared.ViewModels
{
    public class ItemDTSlotViewModel : ComboBoxViewModel<ItemDT>
    {
        #region 成员

        public readonly ImmutableDictionary<int, int> Item2Index; // 通过ItemID导入与导出, ItemID到Index的映射
        public readonly ImmutableDictionary<string, int> RawID2Index; // 通过ItemID导入与导出, ItemID RawID到Index的映射
        public bool IsEnabled { get; set; } = true;

        public readonly ItemDTTypeEnum Type;
        public readonly string ItemDTType;
        public string ItemDTShowType { get; }
        public int SelectedUIID => SelectedItem.UIID;
        public string SelectedRawID => SelectedItem.RawID;

        #endregion

        public ItemDTSlotViewModel(IEnumerable<ItemDT> data) : base(data, _ => _.DLCLevel == StaticConst.CurrentLevel)
        {
            var dict = ImmutableDictionary.CreateBuilder<int, int>();
            var rdictb = ImmutableDictionary.CreateBuilder<string, int>();

            for (int i = 0; i < ItemsSource.Length; i++)
            {
                dict.Add(ItemsSource[i].UIID, i);
                rdictb.Add(ItemsSource[i].RawID, i);
            }

            Item2Index = dict.ToImmutable();
            RawID2Index = rdictb.ToImmutable();

            ItemDTType = ItemsSource[0].ItemDTType;
            Type = ItemsSource[0].Type;
            ItemDTShowType = GetItemDTShowType();
        }


        public string GetItemDTShowType()
        {
            var res = ItemDTType.Substring(0, 2) + "\n" + ItemDTType.Substring(2);
            return res;
        }

        public void Load(int itemID)
        {
            // 基于ItemID载入
            SelectedIndex = Item2Index.GetValueOrDefault(itemID, 0);
        }

        public void Load(string rawID)
        {
            if (rawID.Contains("#"))
            {
                SelectedIndex = RawID2Index.GetValueOrDefault(rawID, 0);
            }
            else
            {
                int itemid = Convert.ToInt32(rawID);
                Load(itemid);
            }
        }

        public void Reset()
        {
            SelectedIndex = 0;
        }

        public void CancelItemDT(CancelItemDTMessage message)
        {
            if (message.Type == Type && message.UIID == SelectedUIID)
            {
                SelectedIndex = 0;
            }
        }
    }

    public class ItemDTConfigSav
    {
        public bool IsChecked;
        public Dictionary<ItemDTTypeEnum, string> Data;
    }
}