using JX3CalculatorShared.Common;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JX3PZ.Globals;
using JX3PZ.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace JX3PZ.Class
{
    public class WeaponBaseAttrs : IModel
    {
        // 武器基础属性
        public readonly WeaponAttributeTypeEnum WeaponAttributeType;
        public EquipSubTypeEnum WeaponSubType { get; }
        public int WeaponDamageMin { get; private set; } // 最小武器伤害
        public int WeaponDamageMax { get; private set; } // 最大武器伤害
        public int WeaponDamageRand { get; private set; } // 随机武器伤害
        public double WeaponDamage { get; private set; } // 平均武器伤害
        public double WeaponSpeed { get; private set; } // 攻速
        public int WeaponAttackSpeedBase { get; private set; } // 攻速（整数）
        public double WeaponDamagePerSecond { get; private set; } // 武器每秒伤害

        public WeaponBaseAttrs(WeaponAttributeTypeEnum weaponTypeEnum)
        {
            WeaponAttributeType = weaponTypeEnum;
        }

        public WeaponBaseAttrs(Weapon weapon)
        {
            var dict = weapon.Attributes.BaseEntryDict;
            WeaponSubType = weapon.SubTypeEnum;

            WeaponAttributeType = WeaponAttributeTypeEnum.Melee;
            if (WeaponSubType == EquipSubTypeEnum.SECONDARY_WEAPON)
            {
                WeaponAttributeType = WeaponAttributeTypeEnum.Range;
            }

            UpdateFrom(dict);
        }


        public void UpdateFrom(int damageBase = 0, int damageRand = 0, int speed = 0)
        {
            WeaponDamageMin = damageBase;
            WeaponDamageRand = damageRand;
            WeaponAttackSpeedBase = speed;
            WeaponSpeed = speed / 16.0;
            Calc();
        }

        public void UpdateFrom(IDictionary<string, int> dict)
        {
            string weapon = WeaponAttributeType.ToString();

            string DamageBaseKey = $"at{weapon}WeaponDamageBase";
            string DamageRandomKey = $"at{weapon}WeaponDamageRand";
            string SpeedKey = $"at{weapon}WeaponAttackSpeedBase";

            int damageBase = dict.GetValueOrUseDefault(DamageBaseKey);
            int damageRand = dict.GetValueOrUseDefault(DamageRandomKey);
            int speed = dict.GetValueOrUseDefault(SpeedKey);

            UpdateFrom(damageBase, damageRand, speed);
        }


        public void Calc()
        {
            WeaponDamageMax = WeaponDamageMin + WeaponDamageRand;
            WeaponDamage = WeaponDamageMin + WeaponDamageRand / 2.0;
            WeaponDamagePerSecond = WeaponDamage / WeaponSpeed;
        }

        public string[] GetDesc()
        {
            string w = "近身";
            if (WeaponSubType == EquipSubTypeEnum.SECONDARY_WEAPON)
            {
                w = "远程";
            }

            var str1 = $"{w}伤害提高 {WeaponDamageMin:F1} - {WeaponDamageMax:F1}";
            var str2 = $"每秒伤害 {WeaponDamagePerSecond:F1}";
            var res = new string[] { str1, str2 };
            return res;
        }


        public AttributeEntryViewModel[] GetViewModel()
        {
            var strs = GetDesc();
            var res = strs.Select(_ => new AttributeEntryViewModel(_, AttributeEntryTypeEnum.EquipBase.GetColor()))
                .ToArray();
            return res;
        }
    }
}