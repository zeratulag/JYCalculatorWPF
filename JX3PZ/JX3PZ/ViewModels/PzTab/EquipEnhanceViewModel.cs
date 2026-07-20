using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Utils;
using JX3CalculatorShared.ViewModels;
using JX3PZ.Class;
using JX3PZ.Data;
using JX3PZ.Globals;
using JX3PZ.Src;
using JYCalculator.Class;
using PropertyChanged;
using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace JX3PZ.ViewModels
{
    public class EquipEnhanceViewModel : AbsViewModel
    {
        public int Position;
        public int SubType;
        public EquipMapItem Map;

        public ImmutableArray<Enhance> AllEnhanceSource { get; } // 所有小附魔
        public ObservableCollection<EnhanceItem> EnhanceItems { get; private set; }
        public readonly ImmutableArray<BigFM> AllBigFMSource; // 所有大附魔

        public ICollectionView EnhanceSource { get; private set; } // 当前大附魔
        public ICollectionView BigFMSource { get; private set; } // 当前大附魔
        //private int _strengthLevel;

        public int StrengthLevel
        {
            get => RealStrengthLevel;
            set => RealStrengthLevel = Math.Min(Math.Max(0, value), RealMaxStrengthLevel);
        } // 强化等级

        public int RealStrengthLevel { get; set; } = 0;

        public int MaxStrengthLevel { get; private set; } // 最大强化等级
        public int RealMaxStrengthLevel;
        private readonly int SlotMaxStrengthLevel; // 装备栏最大强化等级
        public bool HasBigFM { get; set; } // 是否有大附魔

        public int SelectedEnhanceIndex { get; set; }
        public int SelectedBigFMIndex { get; set; }

        [AlsoNotifyFor(nameof(HasSelectedEnhance), nameof(SelectedEnhanceItem))]
        public Enhance SelectedEnhance { get; set; } // 当前选中小附魔

        [AlsoNotifyFor(nameof(HasSelectedBigFM))]
        public BigFM SelectedBigFM { get; set; } // 当前选中的大附魔

        public bool HasSelectedEnhance => SelectedEnhance != null;
        public bool HasSelectedBigFM => SelectedBigFM != null;
        public double BaseDps { get; private set; }

        public EnhanceItem SelectedEnhanceItem
        {
            get => EnhanceItems?.FirstOrDefault(e => e.Enhance == SelectedEnhance);
            set
            {
                if (value == null)
                {
                    SelectedEnhanceIndex = -1;
                    SelectedEnhance = null;
                    RefreshRelativeProfits();
                    return;
                }

                SelectedEnhanceIndex = EnhanceSource?.Cast<EnhanceItem>().ToList().IndexOf(value) ?? -1;
                SelectedEnhance = value.Enhance;
                RefreshRelativeProfits();
            }
        }

        public RelayCommand<string> DropEnhanceCmd { get; }
        public RelayCommand<string> CopyEnhanceNameCmd { get; }
        public RelayCommand RemoveStrengthCmd { get; }
        public Equip SelectedEquip { get; set; }

        public readonly int HasteEnhanceIdx; // 加速附魔的位置，-1表示没有
        private CalcResult ProfitCalcResult;

        public EquipEnhanceViewModel(int position) : base(nameof(RealStrengthLevel), nameof(SelectedEnhance),
            nameof(SelectedBigFM))
        {
            Position = position;
            Map = EquipMapLib.GetEquipMapItem(position);
            SubType = Map.SubType;
            SlotMaxStrengthLevel = Map.MaxStrengthLevel;
            MaxStrengthLevel = SlotMaxStrengthLevel;
            RealMaxStrengthLevel = MaxStrengthLevel;
            AllEnhanceSource = StaticPzData.Enhance.UseFulEnhances[SubType];
            EnhanceItems = new ObservableCollection<EnhanceItem>(
                AllEnhanceSource.Select(e => new EnhanceItem {Enhance = e}));
            EnhanceSource = CollectionViewSource.GetDefaultView(EnhanceItems);
            EnhanceSource.Filter = (e => IsEnhanceFit(((EnhanceItem) e).Enhance));

            if (StaticPzData.Enhance.BigFM.TryGetValue(Map.SubTypeEnum, out var bigFMs))
            {
                AllBigFMSource = bigFMs;
                HasBigFM = true;
                BigFMSource = CollectionViewSource.GetDefaultView(AllBigFMSource);
                BigFMSource.Filter = (e => IsBigFMFit((BigFM) e));
            }
            else
            {
                AllBigFMSource = new ImmutableArray<BigFM>();
                HasBigFM = false;
                BigFMSource = null;
            }

            DropEnhanceCmd = new RelayCommand<string>(DropEnhance);
            CopyEnhanceNameCmd = new RelayCommand<string>(CopyEnhanceName, CanCopyEnhanceName);
            RemoveStrengthCmd = new RelayCommand(RemoveStrength);

            HasteEnhanceIdx = GetHasteEnhance();
            SetOutName("Enhance");
        }

        public void DropEnhance(string param)
        {
            if (param == "0") // 取下小附魔
            {
                SelectedEnhanceIndex = -1;
                SelectedEnhance = null;
                RefreshRelativeProfits();
            }
            else if (param == "1") // 取下大附魔
            {
                SelectedBigFMIndex = -1;
                SelectedBigFM = null;
            }
        }

        private void CopyEnhanceName(string param)
        {
            if (param == "0")
            {
                Clipboard.SetText(SelectedEnhance.ItemName);
            }
            else if (param == "1")
            {
                Clipboard.SetText(SelectedBigFM.ItemName);
            }
            Growl.Success("复制成功");
        }

        private bool CanCopyEnhanceName(string param)
        {
            if (param == "0")
            {
                return SelectedEnhance != null;
            }

            if (param == "1")
            {
                return SelectedBigFM != null;
            }

            return false;
        }

        public void RemoveStrength()
        {
            StrengthLevel = 0;
        }

        public bool IsBigFMFit(BigFM bigFM)
        {
            if (SelectedEquip == null)
            {
                return true;
            }
            else
            {
                return SelectedEquip.FitBigFM(bigFM);
            }
        }

        public bool IsEnhanceFit(Enhance item)
        {
            if (SelectedEquip == null)
            {
                return true;
            }
            else
            {
                return SelectedEquip.FitEnhance(item);
            }
        }

        public void OnSelectedEquipChanged()
        {
            ApplyEquipFilter();
        }

        public void ApplyEquipFilter()
        {
            EnhanceSource.Refresh();
            if (HasBigFM) BigFMSource.Refresh();
        }

        protected override void _Update()
        {
        }

        private void _ChangeEquip(Equip e)
        {
            SelectedEquip = e;
            RealMaxStrengthLevel = e == null ? SlotMaxStrengthLevel : e.MaxStrengthLevel;
        }

        public void ChangeEquip(Equip e)
        {
            ActionUpdateOnce(_ChangeEquip, e);
        }

        protected override void _RefreshCommands()
        {
            CopyEnhanceNameCmd.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// 载入
        /// </summary>
        /// <param name="strength">强化等级</param>
        /// <param name="enhance">小附魔ID</param>
        /// <param name="enchant">大附魔ID</param>
        private void _Load(int strength = 0, int enhance = -1, int enchant = -1)
        {
            StrengthLevel = strength;

            // 查找小附魔在 EnhanceSource 中的索引
            SelectedEnhanceIndex = -1;
            SelectedEnhance = null;
            if (enhance >= 0)
            {
                int idx = 0;
                foreach (EnhanceItem item in EnhanceSource)
                {
                    var e = item.Enhance;
                    if (e.ID == enhance)
                    {
                        SelectedEnhanceIndex = idx;
                        SelectedEnhance = e;
                        break;
                    }

                    idx++;
                }
            }

            // 查找大附魔在 BigFMSource 中的索引
            SelectedBigFMIndex = -1;
            SelectedBigFM = null;
            if (enchant > 0 && HasBigFM && BigFMSource != null)
            {
                int idx = 0;
                foreach (BigFM b in BigFMSource)
                {
                    if (b.ID == enchant)
                    {
                        SelectedBigFMIndex = idx;
                        SelectedBigFM = b;
                        break;
                    }

                    idx++;
                }
            }

            RefreshRelativeProfits();
        }

        private void _Load(JBPZEquipSnapshot s)
        {
            if (s == null)
            {
                _Load(0, -1, -1);
            }
            else
            {
                _Load(s.strength, s.enhance ?? -1, s.enchant ?? -1);
            }
        }

        public void Load(JBPZEquipSnapshot s) => ActionUpdateOnce(_Load, s);


        public void StrengthToMaxLevel()
        {
            // 一键强化满
            StrengthLevel = SlotMaxStrengthLevel;
        }

        public void OneKeyBigFM()
        {
            if (BigFMSource == null || BigFMSource.IsEmpty)
            {
                return;
            }

            SelectedBigFMIndex = BigFMSource.Cast<object>().Count() - 1;
        }

        /// <summary>
        /// 从计算器结果刷新小附魔收益
        /// </summary>
        /// <param name="result">计算器结果</param>
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
            var selectedProfit = SelectedEnhance == null ? 0 : profitList.CalcProfit(SelectedEnhance);
            var selectedHasteValue = GetHasteValue(SelectedEnhance);
            foreach (var item in EnhanceItems)
            {
                item.EnhanceProfit.IsCurrent = item.Enhance == SelectedEnhance;
                var hasteDelta = GetHasteValue(item.Enhance) - selectedHasteValue;
                item.EnhanceProfit.IsUnsupportedHasteChange = !ProfitCalcResult.IsHasteChangeSupported(hasteDelta);
                item.EnhanceProfit.Value = profitList.CalcProfit(item.Enhance) - selectedProfit;
                item.EnhanceProfit.Percent = BaseDps > 0 ? item.EnhanceProfit.Value / BaseDps : 0;
            }

            EnhanceSource?.Refresh();
        }

        private void ClearProfits()
        {
            BaseDps = 0;
            foreach (var item in EnhanceItems)
            {
                item.EnhanceProfit.Clear();
            }

            EnhanceSource?.Refresh();
        }

        private int GetHasteValue(Enhance enhance)
        {
            return enhance != null && enhance.IsHaste ? enhance.Attribute1Value : 0;
        }

        /// <summary>
        /// 基于计算器结果，计算收益最高的附魔
        /// </summary>
        /// <param name="result">计算器结果</param>
        /// <returns>最好的idx和最高收益值</returns>
        public BestEnhanceResult GetBestEnhanceByCalcResult(CalcResult result)
        {
            if (result == null || !result.Success || result.DamageDeriv?.ProfitList == null)
            {
                return new BestEnhanceResult(Position, 0, 0, AllEnhanceSource[0]);
            }

            var profits = AllEnhanceSource.Select(_ => result.DamageDeriv.ProfitList.CalcProfit(_)).ToArray();
            var idx = profits.WhichMax();
            var p = profits[idx];
            return new BestEnhanceResult(Position, idx, p, AllEnhanceSource[idx]);
        }

        /// <summary>
        /// 找到加速附魔的位置
        /// </summary>
        /// <returns>加速附魔的位置</returns>
        public int GetHasteEnhance()
        {
            var i = AllEnhanceSource.IndexOf(_ => _.IsHaste && !_.Name.StartsWith("连雾·"));
            return i;
        }

        /// <summary>
        /// 小附魔候选项，附带当前计算结果下的收益
        /// </summary>
        public class EnhanceItem
        {
            public Enhance Enhance { get; set; }
            public ProfitItemInfo EnhanceProfit { get; set; } = new ProfitItemInfo();
            public string ToolTip => Enhance.ToolTip;
        }

        public class BestEnhanceResult
        {
            // 描述一种带名称的值属性
            public EquipSlotEnum Slot;
            public readonly int Index;
            public readonly double Value;
            public readonly string Name;

            public BestEnhanceResult(EquipSlotEnum slot, int index, double value, string name)
            {
                Slot = slot;
                Index = index;
                Value = value;
                Name = name;
            }

            public BestEnhanceResult(int position, int index, double value, Enhance e)
            {
                Name = e.Name;
                Slot = (EquipSlotEnum) position;
                Index = index;
                Value = value;
            }
        }
    }
}
