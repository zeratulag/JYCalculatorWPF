using System;
using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Class;
using JX3PZ.Class;
using JX3PZ.Data;
using JX3PZ.Models;
using System.Linq;

namespace JX3PZ.ViewModels
{
    public class PzEquipSummaryViewModel : ObservableObject
    {
        public readonly int Position; // 槽位编码

        public string SlotName { get; } // 部位名称，eg: 帽子
        public readonly int MaxSlotStrengthLevel;
        public readonly EquipMapItem Map;
        public Equip CEquip { get; private set; }
        public bool HasEquip { get; private set; } = false;
        public int Quality { get; private set; } = 0;
        public string EquipName { get; private set; } // 装备名称，eg：揽江·寒锁冠
        public int StrengthLevel { get; private set; } = 0; // 精炼等级
        public int MaxStrengthLevel { get; private set; } = 0; // 最大精炼
        public string StrengthStr => $"{StrengthLevel}/{MaxStrengthLevel}";
        public int Level { get; private set; } // 品质等级
        public string EquipTag { get; private set; } // 装备属性标签
        public int IconID { get; private set; } // 图标
        public string BorderImage { get; private set; } = "-1"; // 边框图标

        public int[] DiamondLevels; // 五行石等级
        public DiamondLevelItem[] Diamonds { get; private set; } // 五行石属性描述

        public Enhance CEnhance { get; private set; } // 附魔
        public BigFM CBigFM { get; private set; } // 大附魔
        public Stone CStone { get; private set; } // 五彩石
        public bool HasStone { get; private set; }

        public bool HasEnhance { get; private set; }
        public string EnhanceIcon { get; private set; } = "null";
        public bool HasBigFM { get; private set; }
        public string BigFMIcon { get; private set; } = "null";
        public EquipShowViewModel EquipShowVM { get; set; }

        public PzEquipSummaryViewModel(int position)
        {
            Position = position;
            Map = EquipMapLib.GetEquipMapItem(position);
            SlotName = Map.GetShortLabel();
            MaxSlotStrengthLevel = Map.MaxStrengthLevel;
            RefreshEquipProps();
            RefreshEnhanceProps();
        }

        public void UpdateFrom(EquipSnapShotModel vm)
        {
            CEquip = vm.CEquip;
            StrengthLevel = vm.Strength;
            Diamonds = vm.Diamonds.ToArray();
            DiamondLevels = vm.DiamondLevels;
            CEnhance = vm.CEnhance;
            CBigFM = vm.CBigFM;
            CStone = vm.CStone;

            RefreshEquipProps();
            RefreshEnhanceProps();
        }

        public void RefreshEquipProps()
        {
            // 更新与装备有关的属性
            HasEquip = CEquip != null;

            if (HasEquip)
            {
                IconID = CEquip.IconID;
                Quality = CEquip.Quality;
                Level = CEquip.Level;
                EquipName = CEquip.Name;
                MaxStrengthLevel = CEquip.MaxStrengthLevel;
                EquipTag = CEquip.MagicEntry_Str;
                StrengthLevel = Math.Min(StrengthLevel, MaxStrengthLevel);
                GetBorderImage();
            }
            else
            {
                IconID = Map.IconID;
                Quality = 0;
                Level = 0;
                EquipName = "";
                MaxStrengthLevel = MaxSlotStrengthLevel;
                EquipTag = "";
                BorderImage = "-1";
            }
        }

        public void RefreshEnhanceProps()
        {
            // 更新与附魔有关的属性

            if (HasEquip)
            {
                HasEnhance = CEnhance != null;
                HasBigFM = CBigFM != null;
                HasStone = CStone != null;
            }
            else
            {
                HasEnhance = false;
                HasBigFM = false;
                HasStone = false;
            }

            EnhanceIcon = HasEnhance ? "enhance" : "enhance-null";
            BigFMIcon = HasBigFM ? "enchant" : "enchant-null";
        }

        public void GetBorderImage()
        {
            if (HasEquip)
            {
                if (CEquip.Quality != 4)
                {
                    BorderImage = CEquip.Quality.ToString();
                }
                else
                {
                    if (StrengthLevel == MaxStrengthLevel)
                    {
                        BorderImage = "4-max";
                    }
                    else
                    {
                        BorderImage = "4";
                    }
                }
            }
            else
            {
                BorderImage = "-1";
            }
        }
    }
}