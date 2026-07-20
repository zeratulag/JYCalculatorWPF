using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Controls;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Messages;
using JX3CalculatorShared.Utils;
using JYCalculator.Class;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

namespace JX3CalculatorShared.ViewModels
{
    public class ItemDTConfigViewModelBase : CollectionViewModel<ItemDTSlotViewModel>, IRecipient<CancelItemDTMessage>
    {
        public ImmutableDictionary<ItemDTTypeEnum, ItemDTSlotViewModel> ItemDTSlotDict;
        public Dictionary<string, double> SAts;
        public List<ItemDT> ValidItemDts; // 存储有效的单体列表
        public CharAttrCollection SCharAttr;  // 最终有效属性
        public Dictionary<ItemDTTypeEnum, int> Config; // 存储ItemID的字典
        public NamedAttrs AttrsDesc; // 调试用
        public ImmutableArray<ItemDTSlotViewModel> ItemsSource { get; set; }
        public bool IsChecked { get; set; } = true;
        public int ValidItemDTNum { get; set; } // 有效单体数
        public string Header { get; set; }
        public ImmutableArray<ItemDT> ValidItemDTItems { get; set; } = ImmutableArray<ItemDT>.Empty;
        public RelayCommand OneKeyBestItemDTCmd { get; }
        public RelayCommand CopyAllItemDTNamesCmd { get; }

        private CalcResult ProfitCalcResult;

        public ItemDTConfigViewModelBase()
        {
            OneKeyBestItemDTCmd = new RelayCommand(OneKeyBestItemDT, CanOneKeyBestItemDT);
            CopyAllItemDTNamesCmd = new RelayCommand(CopyAllItemDTNames, CanCopyAllItemDTNames);
            WeakReferenceMessenger.Default.Register(this);
        }

        public void UpdateProfitsFromCalcResult(CalcResult result)
        {
            ProfitCalcResult = result;
            foreach (var item in Data)
            {
                item.UpdateProfitsFromCalcResult(result);
            }

            OneKeyBestItemDTCmd.NotifyCanExecuteChanged();
        }

        private bool CanOneKeyBestItemDT()
        {
            return ProfitCalcResult != null && ProfitCalcResult.Success &&
                   ProfitCalcResult.DamageDeriv?.ProfitList != null;
        }

        private void OneKeyBestItemDT()
        {
            if (!CanOneKeyBestItemDT()) return;

            ActionUpdateOnce(LoadBestItemDT);
        }

        private void LoadBestItemDT()
        {
            IsChecked = true;
            var selectedIndexes = GetBestItemDTIndexes();
            for (int i = 0; i < selectedIndexes.Length; i++)
            {
                Data[i].SelectedIndex = selectedIndexes[i];
            }
        }

        private void CopyAllItemDTNames()
        {
            var text = string.Join("\r\n", ValidItemDTItems.Select(e => e.DescName));
            Clipboard.SetText(text);
        }

        private bool CanCopyAllItemDTNames()
        {
            return ValidItemDTItems.Any();
        }

        private int[] GetBestItemDTIndexes()
        {
            var selectedIndexes = Data.Select(_ => _.SelectedIndex).ToArray();
            var dynamicSlots = new List<ItemDTSlotCandidate[]>(Data.Length);

            for (int i = 0; i < Data.Length; i++)
            {
                var slotCandidates = GetCompressedCandidates(Data[i], i);
                if (slotCandidates.Length == 0)
                {
                    continue;
                }

                if (slotCandidates.Select(_ => _.HasteDelta).Distinct().Count() <= 1)
                {
                    selectedIndexes[i] = slotCandidates.OrderByDescending(_ => _.Profit).First().Index;
                }
                else
                {
                    dynamicSlots.Add(slotCandidates);
                }
            }

            if (dynamicSlots.Count == 0)
            {
                return selectedIndexes;
            }

            var state = SolveBestDynamicSlots(dynamicSlots);
            if (state != null)
            {
                foreach (var kvp in state.SelectedIndexes)
                {
                    selectedIndexes[kvp.Key] = kvp.Value;
                }
            }

            return selectedIndexes;
        }

        private ItemDTSlotCandidate[] GetCompressedCandidates(ItemDTSlotViewModel slot, int slotIndex)
        {
            var profitList = ProfitCalcResult.DamageDeriv.ProfitList;
            var currentHaste = GetHasteValue(slot.SelectedItemDT);
            var candidates = new List<ItemDTSlotCandidate>(slot.ItemsSource.Length);

            for (int i = 0; i < slot.ItemsSource.Length; i++)
            {
                var item = slot.ItemsSource[i].ItemDT;
                if (!item.IsValid && i != slot.SelectedIndex)
                {
                    continue;
                }

                candidates.Add(new ItemDTSlotCandidate
                {
                    SlotIndex = slotIndex,
                    Index = i,
                    HasteDelta = GetHasteValue(item) - currentHaste,
                    Profit = profitList.CalcProfit(item),
                });
            }

            return candidates
                .GroupBy(_ => _.HasteDelta)
                .Select(_ => _.OrderByDescending(candidate => candidate.Profit).First())
                .ToArray();
        }

        private ItemDTSelectionState SolveBestDynamicSlots(List<ItemDTSlotCandidate[]> dynamicSlots)
        {
            var states = new Dictionary<int, ItemDTSelectionState>
            {
                [0] = new ItemDTSelectionState()
            };

            foreach (var slotCandidates in dynamicSlots)
            {
                var nextStates = new Dictionary<int, ItemDTSelectionState>();
                foreach (var stateKvp in states)
                {
                    foreach (var candidate in slotCandidates)
                    {
                        var hasteDelta = stateKvp.Key + candidate.HasteDelta;
                        var profit = stateKvp.Value.Profit + candidate.Profit;
                        if (!nextStates.TryGetValue(hasteDelta, out var oldState) || profit > oldState.Profit)
                        {
                            nextStates[hasteDelta] = stateKvp.Value.Add(candidate);
                        }
                    }
                }

                states = nextStates;
            }

            return states
                .Where(_ => ProfitCalcResult.IsHasteChangeSupported(_.Key))
                .Select(_ => _.Value)
                .OrderByDescending(_ => _.Profit)
                .FirstOrDefault();
        }

        private static int GetHasteValue(ItemDT item)
        {
            if (item == null || item.Data == null)
            {
                return 0;
            }

            return item.Data.TryGetValue(AppStatic.HasteModifyType, out var value)
                ? (int)Math.Round(value)
                : 0;
        }


        /// 整体是否启用
        public void UpdateCheckedChanged(object sender, PropertyChangedEventArgs e)
        {
            foreach (var _ in ItemsSource)
            {
                _.IsEnabled = IsChecked;
            }
        }

        protected override void _Update()
        {
            FindValid();
            GetNamedAttrs();
            GetHead();
        }

        protected void FindValid()
        {
            ValidItemDts.Clear();
            var Buffs = new List<BaseBuff>(8);
            ValidItemDTNum = 0;
            ValidItemDTItems = ImmutableArray<ItemDT>.Empty;

            SCharAttr = null;

            Config = ItemDTSlotDict.ToDictionary(_ => _.Key, _ => _.Value.SelectedUIID);

            if (!IsChecked) return;
            foreach (var VM in Data)
            {
                var dt = VM.SelectedItemDT;
                if (dt.IsValid)
                {
                    ValidItemDts.Add(dt);
                    Buffs.Add(dt.Buff);
                }
            }

            ValidItemDTNum = ValidItemDts.Count;
            ValidItemDTItems = ValidItemDts.ToImmutableArray();
            if (ValidItemDTNum > 0)
            {
                var buffGroup = new BaseBuffGroup(Buffs);
                buffGroup.Calc();
                SCharAttr = buffGroup.SCharAttr;
            }
        }

        protected override void _DEBUG()
        {
            Config.TraceCat();

            if (SCharAttr == null)
            {
                Log.Information("未使用单体！");
            }
            else
            {
                SCharAttr.Values.TraceCat();
            }

        }

        protected void GetNamedAttrs()
        {
            var names = from _ in ValidItemDts select _.DescName;
            AttrsDesc = new NamedAttrs(names, SCharAttr);
            AttrsDesc.ParcelName("单体：");
        }

        protected void GetHead()
        {
            var names = from _ in ValidItemDts select _.ItemName;
            var res = new StringBuilder();
            int i = 0;
            foreach (var name in names)
            {

                if (i > 0)
                {
                    res.Append(", ");
                    if (i % 2 == 0)
                    {
                        res.Append('\n');
                    }
                }

                res.Append(name);
                i++;
            }

            Header = res.ToString();
        }

        public ItemDTConfigSav Export()
        {
            var res = new ItemDTConfigSav()
            {
                IsChecked = IsChecked,
                Data = ItemDTSlotDict.ToDictionary(_ => _.Key.ToString(), _ => _.Value.SelectedRawID),
            };
            return res;
        }

        protected void _Load(ItemDTConfigSav sav)
        {
            IsChecked = sav.IsChecked;
            foreach (var KVP in ItemDTSlotDict)
            {
                string key = KVP.Key.ToString();
                if (sav.Data.TryGetValue(key, out var value))
                {
                    KVP.Value.Load(value);
                }
                else
                {
                    KVP.Value.Reset();
                }
            }
        }

        public void Load(ItemDTConfigSav sav)
        {
            ActionUpdateOnce(_Load, sav);
        }

        protected override void _RefreshCommands()
        {
            OneKeyBestItemDTCmd.NotifyCanExecuteChanged();
            CopyAllItemDTNamesCmd.NotifyCanExecuteChanged();
        }

        public void Receive(CancelItemDTMessage message)
        {
            ItemDTSlotDict[message.Type].CancelItemDT(message);
        }

        private class ItemDTSlotCandidate
        {
            public int SlotIndex;
            public int Index;
            public int HasteDelta;
            public double Profit;
        }

        private class ItemDTSelectionState
        {
            public readonly Dictionary<int, int> SelectedIndexes = new Dictionary<int, int>();
            public double Profit;

            public ItemDTSelectionState Add(ItemDTSlotCandidate candidate)
            {
                var res = new ItemDTSelectionState
                {
                    Profit = Profit + candidate.Profit
                };
                foreach (var kvp in SelectedIndexes)
                {
                    res.SelectedIndexes[kvp.Key] = kvp.Value;
                }

                res.SelectedIndexes[candidate.SlotIndex] = candidate.Index;
                return res;
            }
        }
    }
}
