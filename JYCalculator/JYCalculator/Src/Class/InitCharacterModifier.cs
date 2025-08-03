using JX3CalculatorShared.Globals;
using JYCalculator.Globals;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System;
using Serilog;
using static JYCalculator.Globals.XFStaticConst;


namespace JYCalculator.Class
{
    public static class InitCharacterModifier
    {
        public static void ProcessBaseWeaponDamage(this InitCharacter iChar, double value)
        {
            iChar.BaseWeaponDamage += value;
        }

        public static void ProcessPhysicsCriticalStrikeRate(this InitCharacter iChar, double value)
        {
            iChar.PhysicsCriticalStrikeRate += value;
        }

        public static void ProcessPhysicsCriticalStrike(this InitCharacter iChar, double value)
        {
            iChar.PhysicsCriticalStrike += value;
        }

        public static void ProcessPhysicsCriticalPowerRate(this InitCharacter iChar, double value)
        {
            iChar.PhysicsCriticalPowerValue += value;
        }

        public static void ProcessPhysicsCriticalPower(this InitCharacter iChar, double value)
        {
            ProcessPhysicsCriticalPowerRate(iChar, value / CurrentLevelParams.CriticalPower);
        }

        public static void ProcessStrainRate(this InitCharacter iChar, double value)
        {
            iChar.StrainRate += value;
        }

        public static void ProcessBaseStrain(this InitCharacter iChar, double value)
        {
            iChar.BaseStrain += value;
        }

        public static void ProcessHaste(this InitCharacter iChar, double value)
        {
            iChar.Haste += value;
        }

        public static void ProcessBaseSurplus(this InitCharacter iChar, double value)
        {
            iChar.BaseSurplus += value;
        }

        public static void ProcessPhysicsFinalAttackPower(this InitCharacter iChar, double value)
        {
            iChar.PhysicsFinalAttackPower += value;
        }

        public static void ProcessPhysicsBaseAttackPower(this InitCharacter iChar, double value)
        {
            iChar.PhysicsBaseAttackPower += value;
            ProcessPhysicsFinalAttackPower(iChar, value);
        }

        public static void ProcessPhysicsBaseOvercome(this InitCharacter iChar, double value)
        {
            iChar.PhysicsBaseOvercome += value;
        }

        public static void ProcessAgility(this InitCharacter iChar, double value) //身法
        {
            ProcessPhysicsCriticalStrike(iChar, value * XFConsts.AgilityToPhysicsCriticalStrike);
        }

        public static void ProcessFinalStrength(this InitCharacter iChar, double value) // 最终力道
        {
            ProcessPhysicsBaseAttackPower(iChar, value * XFConsts.FinalStrengthToPhysicsBaseAttackPower);
            ProcessPhysicsBaseOvercome(iChar, value * XFConsts.FinalStrengthToPhysicsBaseOvercome);

            ProcessPhysicsFinalAttackPower(iChar, value * XFConsts.JY_FinalStrengthToPhysicsFinalAttackPower);
            ProcessPhysicsCriticalStrike(iChar, value * XFConsts.JY_FinalStrengthToPhysicsCriticalStrike);
        }

        public static void ProcessBaseStrength(this InitCharacter iChar, double value)
        {
            iChar.BaseStrength += value;
            ProcessFinalStrength(iChar, value * (1 + iChar.StrengthPercent));
        }

        public static void ProcessBaseStrengthPercent(this InitCharacter iChar, double value)
        {
            iChar.StrengthPercent += value;
            ProcessFinalStrength(iChar, value * iChar.BaseStrength);
        }

        /// <summary>
        /// 增加全属性
        /// </summary>
        /// <param name="value"></param>
        public static void ProcessAllBasePotent(this InitCharacter iChar, double value)
        {
            ProcessBaseStrength(iChar, value);
            ProcessAgility(iChar, value);
            // TODO: 配装器里还需要增加体质
        }
    }

    public delegate void InitCharacterModifierDelegate(InitCharacter iChar, double value);

    public static class InitCharacterModifierManager
    {
        public static readonly ImmutableDictionary<ZAttributeType, InitCharacterModifierDelegate> ModifierDict;

        static InitCharacterModifierManager()
        {
            var dict = new Dictionary<ZAttributeType, InitCharacterModifierDelegate>()
            {
                {ZAttributeType.BaseWeaponDamage, InitCharacterModifier.ProcessBaseWeaponDamage},

                {ZAttributeType.PhysicsCriticalStrike, InitCharacterModifier.ProcessPhysicsCriticalStrike},
                {ZAttributeType.PhysicsCriticalStrikeRate, InitCharacterModifier.ProcessPhysicsCriticalStrikeRate},

                {ZAttributeType.PhysicsCriticalPowerRate, InitCharacterModifier.ProcessPhysicsCriticalPowerRate},
                {ZAttributeType.PhysicsCriticalPower, InitCharacterModifier.ProcessPhysicsCriticalPower},

                {ZAttributeType.Haste, InitCharacterModifier.ProcessHaste},
                {ZAttributeType.BaseSurplus, InitCharacterModifier.ProcessBaseSurplus},

                {ZAttributeType.FinalStrength, InitCharacterModifier.ProcessFinalStrength},
                {ZAttributeType.BaseStrength, InitCharacterModifier.ProcessBaseStrength},
                {ZAttributeType.StrengthPercent, InitCharacterModifier.ProcessBaseStrengthPercent},

                {ZAttributeType.BaseAgility, InitCharacterModifier.ProcessAgility},

                {ZAttributeType.AllBasePotent, InitCharacterModifier.ProcessAllBasePotent},

                {ZAttributeType.PhysicsBaseAttackPower, InitCharacterModifier.ProcessPhysicsBaseAttackPower},
                {ZAttributeType.PhysicsFinalAttackPower, InitCharacterModifier.ProcessPhysicsFinalAttackPower},

                {ZAttributeType.PhysicsBaseOvercome, InitCharacterModifier.ProcessPhysicsBaseOvercome},

                {ZAttributeType.StrainRate, InitCharacterModifier.ProcessStrainRate},
                {ZAttributeType.BaseStrain, InitCharacterModifier.ProcessBaseStrain},
            };
            ModifierDict = dict.ToImmutableDictionary();
        }

        public static void ProcessZAttr(this InitCharacter iChar, ZAttributeType key, double value)
        {
            bool success = ModifierDict.TryGetValue(key, out InitCharacterModifierDelegate modifier);
            if (success)
            {
                modifier(iChar, value);
            }
            else
            {
                //Log.Information($"无效的属性！ {key}:{value} ");
            }
        }

        public static void ProcessZAttr(this InitCharacter iChar, string key, double value)
        {
            ZAttributeType zkey = ZAttributeType.None;
            var success = Enum.TryParse(key, out zkey);
            if (success && zkey != ZAttributeType.None)
            {
                ProcessZAttr(iChar, zkey, value);
            }
            else
            {
                //Log.Information($"未知的属性！ {key}:{value}");
            }
        }
    }
}