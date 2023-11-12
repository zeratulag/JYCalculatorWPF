using JX3PZ.Globals;
using System.Collections.Generic;
using JX3PZ.Class;

namespace JX3PZ.Models
{
    public class PanelWeaponSlot: IPanelAttributeSlot
    {
        public readonly WeaponAttributeTypeEnum WeaponType;
        public readonly string Name;

        public readonly string AttackSpeedKey;
        public readonly string DamageBaseKey;
        public readonly string DamageRandKey;

        public int AttackSpeed { get; protected set; }
        public int DamageBase { get; protected set; } // 固定武器伤害
        public int DamageRand { get; protected set; } // 随机武器伤害
        public int DamageMin { get; private set; } = 0; // 最小武器伤害
        public int DamageMax { get; private set; } = 0; // 最大武器伤害
        public double DamageAverage { get; private set; } = 0; // 平均武器伤害
        public double Speed { get; private set; } = 0; // 攻速
        public double DamagePerSecond { get; private set; } = 0; // DPS

        public PanelWeaponSlot(WeaponAttributeTypeEnum weaponType)
        {
            WeaponType = weaponType;
            Name = WeaponType.ToString();
            AttackSpeedKey = $"at{Name}WeaponAttackSpeedBase";
            DamageBaseKey = $"at{Name}WeaponDamageBase";
            DamageRandKey = $"at{Name}WeaponDamageRand";
        }

        public void UpdateFrom(IDictionary<string, int> valueDict)
        {
            var model = new WeaponBaseAttrs(WeaponType);
            model.UpdateFrom(valueDict);

            AttackSpeed = model.WeaponAttackSpeedBase;
            Speed = model.WeaponSpeed;
            DamageBase += model.WeaponDamageMin;
            DamageRand += model.WeaponDamageRand;
            DamageMin += model.WeaponDamageMin;
            DamageMax += model.WeaponDamageMax;
            DamageAverage += model.WeaponDamage;
            DamagePerSecond = model.WeaponDamagePerSecond;
        }

        public string GetValueDesc()
        {
            return $"{GetDesc1()} {GetDesc2()}";
        }

        public string GetDesc1()
        {
            return DamageAverage.ToString("F1");
        }

        public string GetDesc2()
        {
            return $"{DamageMin}~{DamageMax}";
        }

        public static string AcquireTypeDesc(WeaponAttributeTypeEnum weaponType)
        {
            var prefix = weaponType == WeaponAttributeTypeEnum.Melee ? "近身" : "远程";
            var res = prefix + "武器伤害";
            return res;
        }


        public List<string> GetDescTips()
        {
            var res = new List<string>()
            {
                $"{AcquireTypeDesc(WeaponType)}提高 {DamageMin} - {DamageMax}，平均伤害 {DamageAverage:F1}",
                $"武器速度 {Speed:F1}",
                $"每秒伤害 {DamagePerSecond:F1}"
            };
            return res;
        }
    }
}