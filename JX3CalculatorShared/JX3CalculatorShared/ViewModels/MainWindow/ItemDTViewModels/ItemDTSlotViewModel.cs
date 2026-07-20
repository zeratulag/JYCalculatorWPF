using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Messages;
using JYCalculator.Class;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JX3CalculatorShared.ViewModels
{
    public class ItemDTSlotViewModel : ComboBoxViewModel<ItemDTViewModel>
    {
        #region 成员

        public readonly ImmutableDictionary<int, int> Item2Index; // 通过ItemID导入与导出, ItemID到Index的映射
        public readonly ImmutableDictionary<string, int> RawID2Index; // 通过ItemID导入与导出, ItemID RawID到Index的映射
        public bool IsEnabled { get; set; } = true;

        public readonly ItemDTTypeEnum Type;
        public readonly string ItemDTType;
        public string ItemDTShowType { get; }
        public ItemDT SelectedItemDT => SelectedItem?.ItemDT;
        public int SelectedUIID => SelectedItemDT.UIID;
        public string SelectedRawID => SelectedItemDT.RawID;
        public double BaseDps { get; private set; }
        private CalcResult ProfitCalcResult;

        #endregion

        public ItemDTSlotViewModel(IEnumerable<ItemDT> data) : base(
            data.Where(item => item.ExpansionPackLevel >= StaticConst.CurrentLevel).Select(item => new ItemDTViewModel(item)))
        {
            var dict = ImmutableDictionary.CreateBuilder<int, int>();
            var rdictb = ImmutableDictionary.CreateBuilder<string, int>();

            for (int i = 0; i < ItemsSource.Length; i++)
            {
                dict.Add(ItemsSource[i].ItemDT.UIID, i);
                rdictb.Add(ItemsSource[i].ItemDT.RawID, i);
            }

            Item2Index = dict.ToImmutable();
            RawID2Index = rdictb.ToImmutable();

            ItemDTType = ItemsSource[0].ItemDT.ItemDTType;
            Type = ItemsSource[0].ItemDT.Type;
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

        public void UpdateProfitsFromCalcResult(CalcResult result)
        {
            ProfitCalcResult = result;
            if (result == null || !result.Success || result.DamageDeriv?.ProfitList == null)
            {
                ClearProfits();
                return;
            }

            BaseDps = result.FinalDPS;
            RefreshRelativeProfits();
        }

        private void RefreshRelativeProfits()
        {
            if (ProfitCalcResult == null || !ProfitCalcResult.Success ||
                ProfitCalcResult.DamageDeriv?.ProfitList == null)
            {
                ClearProfits();
                return;
            }

            var profitList = ProfitCalcResult.DamageDeriv.ProfitList;
            var selectedProfit = SelectedItemDT == null ? 0 : profitList.CalcProfit(SelectedItemDT);
            var selectedHasteValue = GetHasteValue(SelectedItemDT);
            foreach (var item in ItemsSource)
            {
                item.ItemDTProfit.IsCurrent = item == SelectedItem;
                var hasteDelta = GetHasteValue(item.ItemDT) - selectedHasteValue;
                item.ItemDTProfit.IsUnsupportedHasteChange = !ProfitCalcResult.IsHasteChangeSupported(hasteDelta);
                item.ItemDTProfit.Value = profitList.CalcProfit(item.ItemDT) - selectedProfit;
                item.ItemDTProfit.Percent = BaseDps > 0 ? item.ItemDTProfit.Value / BaseDps : 0;
            }
        }

        private void ClearProfits()
        {
            BaseDps = 0;
            foreach (var item in ItemsSource)
            {
                item.ItemDTProfit.Clear();
            }
        }

        private double GetHasteValue(ItemDT item)
        {
            if (item == null || item.Data == null)
            {
                return 0;
            }

            return item.Data.TryGetValue(AppStatic.HasteModifyType, out var value) ? value : 0;
        }

        protected override void _Update()
        {
            RefreshRelativeProfits();
        }

        protected override void _RefreshCommands()
        {
        }
    }

    public class ItemDTConfigSav
    {
        public bool IsChecked;
        public Dictionary<string, string> Data;
    }
}
