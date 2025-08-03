using JX3CalculatorShared.Globals;
using JYCalculator.Globals;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Serilog;
using static JYCalculator.Globals.XFStaticConst;


namespace JYCalculator.Class
{
    // 用于对FullCharacter进行属性计算

    public static class FullCharacterModifier
    {
        #region 属性计算部分

        public static void ProcessBaseWeaponDamage(this FullCharacter fChar, double value)
        {
            fChar.BaseWeaponDamage += value;
        }

        public static void ProcessPhysicsCriticalStrikeRate(this FullCharacter fChar, double value)
        {
            if (fChar.Has_Special_Buff)
            {
                throw new ArgumentException("Cannot add CT after has special_buff!");
            }
            else
            {
                fChar.PhysicsCriticalStrikeRate += value;
            }
        }

        public static void ProcessPhysicsCriticalStrike(this FullCharacter fChar, double value)
        {
            fChar.PhysicsCriticalStrike += value;
        }

        public static void ProcessPhysicsCriticalPowerRate(this FullCharacter fChar, double value)
        {
            fChar.PhysicsCriticalPowerValue += value;
        }

        public static void ProcessPhysicsCriticalPower(this FullCharacter fChar, double value)
        {
            ProcessPhysicsCriticalPowerRate(fChar, value / CurrentLevelParams.CriticalPower);
        }

        public static void ProcessStrainRate(this FullCharacter fChar, double value)
        {
            fChar.StrainRate += value;
        }

        public static void ProcessBaseStrain(this FullCharacter fChar, double value)
        {
            fChar.BaseStrain += value;
            ProcessFinalStrain(fChar, value * (1 + fChar.StrainPercent));
        }

        public static void ProcessFinalStrain(this FullCharacter fChar, double value)
        {
            fChar.FinalStrain += value;
        }

        public static void ProcessStrainPercent(this FullCharacter fChar, double value)
        {
            fChar.StrainPercent += value;
            ProcessFinalStrain(fChar, value * fChar.BaseStrain);
        }

        public static void ProcessHaste(this FullCharacter fChar, double value)
        {
            fChar.Haste += value; // 注意此处加速改变
        }

        public static void ProcessExtraHaste(this FullCharacter fChar, double value)
        {
            fChar.ExtraHaste += value; // 注意此处加速改变
        }

        public static void ProcessBaseSurplus(this FullCharacter fChar, double value)
        {
            fChar.BaseSurplus += value;
        }

        public static void ProcessPhysicsFinalAttackPower(this FullCharacter fChar, double value)
        {
            fChar.PhysicsFinalAttackPower += value;
        }

        public static void ProcessPhysicsBaseAttackPower(this FullCharacter fChar, double value)
        {
            fChar.PhysicsBaseAttackPower += value;
            ProcessPhysicsFinalAttackPower(fChar, value * (1 + fChar.PhysicsAttackPowerPercent));
        }

        public static void ProcessPhysicsAttackPowerPercent(this FullCharacter fChar, double value)
        {
            fChar.PhysicsAttackPowerPercent += value;
            ProcessPhysicsFinalAttackPower(fChar, value * fChar.PhysicsBaseAttackPower);
        }

        public static void ProcessPhysicsFinalOvercome(this FullCharacter fChar, double value)
        {
            fChar.PhysicsFinalOvercome += value;
        }

        public static void ProcessPhysicsBaseOvercome(this FullCharacter fChar, double value)
        {
            fChar.PhysicsBaseOvercome += value;
            ProcessPhysicsFinalOvercome(fChar, value * (1 + fChar.PhysicsOvercomePercent));
        }

        public static void ProcessPhysicsOvercomePercent(this FullCharacter fChar, double value)
        {
            fChar.PhysicsOvercomePercent += value;
            ProcessPhysicsFinalOvercome(fChar, value * fChar.PhysicsBaseOvercome);
        }

        public static void ProcessBaseAgility(this FullCharacter fChar, double value) //身法
        {
            ProcessPhysicsCriticalStrike(fChar, value * XFConsts.AgilityToPhysicsCriticalStrike);
        }


        public static void ProcessAllShieldIgnorePercent(this FullCharacter fChar, double value)
        {
            fChar.AllShieldIgnore += value;
        }

        public static void ProcessPhysicsDamageAdd(this FullCharacter fChar, double value)
        {
            fChar.PhysicsDamageAdd += value;
        }

        public static void ProcessAllDamageAdd(this FullCharacter fChar, double value)
        {
            ProcessPhysicsDamageAdd(fChar, value);
        }


        public static void ProcessFinalStrength(this FullCharacter fChar, double value) // 增加力道
        {
            fChar.FinalStrength += value;
            ProcessPhysicsBaseAttackPower(fChar, value * XFConsts.FinalStrengthToPhysicsBaseAttackPower);
            ProcessPhysicsBaseOvercome(fChar, value * XFConsts.FinalStrengthToPhysicsBaseOvercome);
            ProcessPhysicsFinalAttackPower(fChar, value * XFConsts.JY_FinalStrengthToPhysicsFinalAttackPower);
            ProcessPhysicsCriticalStrike(fChar, value * XFConsts.JY_FinalStrengthToPhysicsCriticalStrike);
        }

        public static void ProcessBaseStrength(this FullCharacter fChar, double value)
        {
            fChar.BaseStrength += value;
            ProcessFinalStrength(fChar, value * (1 + fChar.StrengthPercent));
        }

        public static void ProcessBaseStrengthPercent(this FullCharacter fChar, double value)
        {
            fChar.StrengthPercent += value;
            ProcessFinalStrength(fChar, value * fChar.BaseStrength);
        }


        /// <summary>
        /// 增加全属性
        /// </summary>
        /// <param name="value"></param>
        public static void ProcessAllBasePotent(this FullCharacter fChar, double value)
        {
            ProcessBaseStrength(fChar, value);
            ProcessBaseAgility(fChar, value);
        }

        #endregion

        #region 属性转换部分

        /// <summary>
        /// 在会破属性之和保持不变的情况下，转移部分会心点数到破防
        /// </summary>
        /// <param name="value">点数</param>
        public static void TransCTToOC(this FullCharacter fChar, double value)
        {
            ProcessPhysicsCriticalStrike(fChar, -value);
            ProcessPhysicsBaseOvercome(fChar, value);
        }

        /// <summary>
        /// 在无招属性之和保持不变的情况下，转移部分无双点数到破招
        /// </summary>
        /// <param name="value">点数</param>
        public static void TransWSToPZ(this FullCharacter fChar, double value)
        {
            ProcessBaseStrain(fChar, -value);
            ProcessBaseSurplus(fChar, value);
        }

        /// <summary>
        /// 在会破属性之和保持不变的情况下，重新设置面板会心百分比
        /// </summary>
        /// <param name="ct">目标会心百分比</param>
        public static void Reset_CT(this FullCharacter fChar, double ct)
        {
            var delta = fChar.CT_Point - ct * XFStaticConst.CurrentLevelParams.CriticalStrike;
            TransCTToOC(fChar, delta);
        }

        /// <summary>
        /// 在无招属性之和保持不变的情况下，重新设置面板无双百分比
        /// </summary>
        /// <param name="ws">目标无双百分比</param>
        public static void Reset_WS(this FullCharacter fChar, double ws)
        {
            var delta = fChar.WS_Point - ws * XFStaticConst.CurrentLevelParams.Strain;
            TransWSToPZ(fChar, delta);
        }

        #endregion
    }

    public delegate void FullCharacterModifierDelegate(FullCharacter fChar, double value);

    public static class FullCharacterModifierManager
    {
        public static readonly ImmutableDictionary<ZAttributeType, FullCharacterModifierDelegate> ModifierDict;

        static FullCharacterModifierManager()
        {
            var dict = new Dictionary<ZAttributeType, FullCharacterModifierDelegate>()
            {
                {ZAttributeType.BaseWeaponDamage, FullCharacterModifier.ProcessBaseWeaponDamage},
                {ZAttributeType.PhysicsCriticalStrike, FullCharacterModifier.ProcessPhysicsCriticalStrike},
                {ZAttributeType.PhysicsCriticalStrikeRate, FullCharacterModifier.ProcessPhysicsCriticalStrikeRate},

                {ZAttributeType.PhysicsCriticalPowerRate, FullCharacterModifier.ProcessPhysicsCriticalPowerRate},
                {ZAttributeType.PhysicsCriticalPower, FullCharacterModifier.ProcessPhysicsCriticalPower},

                {ZAttributeType.Haste, FullCharacterModifier.ProcessHaste},
                {ZAttributeType.ExtraHaste, FullCharacterModifier.ProcessExtraHaste},
                {ZAttributeType.BaseSurplus, FullCharacterModifier.ProcessBaseSurplus},

                {ZAttributeType.AllShieldIgnore, FullCharacterModifier.ProcessAllShieldIgnorePercent},

                {ZAttributeType.FinalStrength, FullCharacterModifier.ProcessFinalStrength},
                {ZAttributeType.BaseStrength, FullCharacterModifier.ProcessBaseStrength},
                {ZAttributeType.StrengthPercent, FullCharacterModifier.ProcessBaseStrengthPercent},

                {ZAttributeType.BaseAgility, FullCharacterModifier.ProcessBaseAgility},

                {ZAttributeType.AllBasePotent, FullCharacterModifier.ProcessAllBasePotent},

                {ZAttributeType.PhysicsDamageAdd, FullCharacterModifier.ProcessPhysicsDamageAdd},
                {ZAttributeType.AllDamageAdd, FullCharacterModifier.ProcessAllDamageAdd},

                {ZAttributeType.PhysicsBaseAttackPower, FullCharacterModifier.ProcessPhysicsBaseAttackPower},
                {ZAttributeType.PhysicsAttackPowerPercent, FullCharacterModifier.ProcessPhysicsAttackPowerPercent},
                {ZAttributeType.PhysicsFinalAttackPower, FullCharacterModifier.ProcessPhysicsFinalAttackPower},

                {ZAttributeType.PhysicsBaseOvercome, FullCharacterModifier.ProcessPhysicsBaseOvercome},
                {ZAttributeType.PhysicsOvercomePercent, FullCharacterModifier.ProcessPhysicsOvercomePercent},

                {ZAttributeType.StrainRate, FullCharacterModifier.ProcessStrainRate},
                {ZAttributeType.FinalStrain, FullCharacterModifier.ProcessFinalStrain},
                {ZAttributeType.BaseStrain, FullCharacterModifier.ProcessBaseStrain},
                {ZAttributeType.StrainPercent, FullCharacterModifier.ProcessStrainPercent},
            };
            ModifierDict = dict.ToImmutableDictionary();
        }

        public static void ProcessZAttr(this FullCharacter fChar, ZAttributeType key, double value)
        {
            bool success = ModifierDict.TryGetValue(key, out FullCharacterModifierDelegate modifier);
            if (success)
            {
                modifier(fChar, value);
            }
            else
            {
                //Log.Information($"无效的属性！ {key}:{value} ");
            }
        }

        public static void ProcessZAttr(this FullCharacter fChar, string key, double value)
        {
            ZAttributeType zkey = ZAttributeType.None;
            var success = Enum.TryParse(key, out zkey);
            if (success && zkey != ZAttributeType.None)
            {
                ProcessZAttr(fChar, zkey, value);
            }
            else
            {
                //Log.Information($"未知的属性！ {key}:{value}");
            }
        }
    }
}