using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using HandyControl.Tools.Extension;
using JX3PZ.Class;
using JX3PZ.Data;
using JX3PZ.Globals;

namespace JX3PZ.ViewModels
{
    public class EquipShowHeadViewModel : ObservableObject, IEquipShow
    {
        // 装备头
        public bool HasEquip { get; private set; } = false;

        public string Name { get; private set; } // 装备名称
        public string StrengthStar1 { get; private set; } // 强化星星，★
        public string StrengthStar2 { get; private set; } // 未强化的星星，☆
        public int StrengthLevel { get; private set; } = 0; // 强化等级
        public int MaxStrengthLevel { get; private set; } = 6; // 最大强化等级
        public string SubTypeName { get; private set; } // 类型名称

        public int Quality { get; private set; }
        public string WeaponDetailTypeName { get; set; } // 武器详细类型名称
        public string WeaponSpeed { get; set; } // 速度

        public string StrengthStr => $"精炼等级: {StrengthLevel} / {MaxStrengthLevel}";
        public AttributeEntryViewModel[] BaseAttributeVMs { get; protected set; } // 基础属性词条

        public bool HasBaseAttributes { get; private set; } // 是否有基础属性
        public bool IsWeapon { get; private set; } // 是否为武器

        public EquipShowHeadViewModel()
        {
        }

        public EquipShowHeadViewModel(Equip equip, int strengthLevel = 0)
        {
            UpdateFrom(equip, strengthLevel);
        }


        public  void UpdateFrom(Equip equip, int strengthLevel = 0)
        {
            HasEquip = equip != null;
            if (!HasEquip)
            {
                return;
            }
            Name = equip.Name;
            Quality = equip.Quality;
            MaxStrengthLevel = equip.MaxStrengthLevel;
            StrengthLevel = Math.Min(strengthLevel, MaxStrengthLevel);
            StrengthStar1 = new string('★', StrengthLevel);
            StrengthStar2 = new string('☆', MaxStrengthLevel - StrengthLevel);
            var map = EquipMapLib.EquipSubType[equip.SubType];
            SubTypeName = map.Label;

            HasBaseAttributes = equip.Attributes.BaseEntry.Any();

            IsWeapon = equip.IsWeapon;

            Weapon wp = null;
            if (IsWeapon)
            {
                wp = equip as Weapon;
                WeaponDetailTypeName = EquipMapLib.Weapon[wp.DetailType].Weapon;
            }

            if (HasBaseAttributes)
            {
                if (IsWeapon)
                {
                    WeaponSpeed = $"速度 {wp?.WeaponBase.WeaponSpeed:F1}";
                    BaseAttributeVMs = wp?.WeaponBase.GetViewModel();
                }
                else
                {
                    BaseAttributeVMs = equip.Attributes.BaseEntry.Select(_ => _.GetViewModel()).ToArray();
                }
            }
            else
            {
                BaseAttributeVMs = null;
            }
        }

        public List<string> GetDescList()
        {
            var res = new List<string>(10)
            {
                $"{Name}\t\t0/{MaxStrengthLevel}",
            };

            if (IsWeapon)
            {
                res.Add($"{SubTypeName}\t\t{WeaponDetailTypeName}");
            }
            else
            {
                res.Add($"{SubTypeName}");
            }

            if (HasBaseAttributes)
            {
                res.AddRange(BaseAttributeVMs.Select(_ => _.Desc));
            }

            return res;
        }
    }
}