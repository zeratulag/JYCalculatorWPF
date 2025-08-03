using CommunityToolkit.Mvvm.Input;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Utils;
using JX3CalculatorShared.ViewModels;
using JX3PZ.Class;
using JX3PZ.Data;
using JX3PZ.Globals;
using JX3PZ.Src;
using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace JX3PZ.ViewModels
{
    public class EquipEnhanceViewModel : AbsViewModel
    {
        public int Position;
        public int SubType;
        public EquipMapItem Map;

        public ImmutableArray<Enhance> EnhanceSource { get; } // 所有小附魔
        public readonly ImmutableArray<BigFM> AllBigFMSource; // 所有大附魔
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

        public Enhance SelectedEnhance { get; set; } // 当前选中小附魔
        public BigFM SelectedBigFM { get; set; } // 当前选中的大附魔

        public RelayCommand<string> DropEnhanceCmd { get; }
        public RelayCommand RemoveStrengthCmd { get; }
        public Equip SelectedEquip { get; set; }

        public readonly int HasteEnhanceIdx; // 加速附魔的位置，-1表示没有

        public EquipEnhanceViewModel(int position) : base(nameof(RealStrengthLevel), nameof(SelectedEnhance),
            nameof(SelectedBigFM))
        {
            Position = position;
            Map = EquipMapLib.GetEquipMapItem(position);
            SubType = Map.SubType;
            SlotMaxStrengthLevel = Map.MaxStrengthLevel;
            MaxStrengthLevel = SlotMaxStrengthLevel;
            RealMaxStrengthLevel = MaxStrengthLevel;
            EnhanceSource = StaticPzData.Enhance.UseFulEnhances[SubType];
            if (StaticPzData.Enhance.BigFM.ContainsKey(Map.SubTypeEnum))
            {
                AllBigFMSource = StaticPzData.Enhance.BigFM[Map.SubTypeEnum];
                HasBigFM = true;
                BigFMSource = CollectionViewSource.GetDefaultView(AllBigFMSource);
                BigFMSource.Filter = (_ => IsBigFMFit((BigFM)_));
            }
            else
            {
                AllBigFMSource = new ImmutableArray<BigFM>();
                HasBigFM = false;
                BigFMSource = null;
            }

            DropEnhanceCmd = new RelayCommand<string>(DropEnhance);
            RemoveStrengthCmd = new RelayCommand(RemoveStrength);

            HasteEnhanceIdx = GetHasteEnhance();
            SetOutName("Enhance");
        }

        public void DropEnhance(string param)
        {
            if (param == "0") // 取下小附魔
            {
                SelectedEnhanceIndex = -1;
            }
            else if (param == "1") // 取下大附魔
            {
                SelectedBigFMIndex = -1;
            }
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

        public void OnSelectedEquipChanged()
        {
            ApplyEquipFilter();
        }

        public void ApplyEquipFilter()
        {
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
            SelectedEnhanceIndex = EnhanceSource.IndexOf(_ => _.ID == enhance);
            SelectedBigFMIndex = 0;
            if (enchant > 0)
            {
                SelectedBigFMIndex = 0;
            }
            else
            {
                SelectedBigFMIndex = -1;
            }
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
        /// 基于计算器结果，计算收益最高的附魔
        /// </summary>
        /// <param name="result">计算器结果</param>
        /// <returns>最好的idx和最高收益值</returns>
        public BestEnhanceResult GetBestEnhanceByCalcResult(CalcResult result)
        {
            if (result == null) return new BestEnhanceResult(Position, 0, 0, EnhanceSource[0]);
            var drv = result.DamageDeriv;
            var profits = EnhanceSource.Select(_ => result.DamageDeriv.ProfitList.CalcProfit(_)).ToArray();
            var idx = profits.WhichMax();
            var p = profits[idx];
            return new BestEnhanceResult(Position, idx, p, EnhanceSource[idx]);
        }

        /// <summary>
        /// 找到加速附魔的位置
        /// </summary>
        /// <returns>加速附魔的位置</returns>
        public int GetHasteEnhance()
        {
            var i = EnhanceSource.IndexOf(_ => _.IsHaste && !_.Name.StartsWith("连雾·"));
            return i;
        }
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
            Slot = (EquipSlotEnum)position;
            Index = index;
            Value = value;
        }
    }
}