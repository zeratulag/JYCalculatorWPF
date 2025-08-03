using JX3CalculatorShared.Globals;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace JX3CalculatorShared.Class
{
    public static class TargetModifier
    {
        #region 属性计算部分

        public static void ProcessPhysicsFinalShield(this Target target, double value)
        {
            target.PhysicsFinalShield += value;
        }

        public static void ProcessPhysicsBaseShield(this Target target, double value)
        {
            target.PhysicsBaseShield += value;
            target.PhysicsFinalShield += value * (1 + target.PhysicsShieldPercent);
        }

        public static void ProcessPhysicsShieldPercent(this Target target, double value)
        {
            target.PhysicsShieldPercent += value;
            target.PhysicsFinalShield += target.PhysicsBaseShield * value;
        }

        public static void ProcessMagicFinalShield(this Target target, double value)
        {
            target.MagicFinalShield += value;
        }

        public static void ProcessMagicBaseShield(this Target target, double value)
        {
            target.MagicBaseShield += value;
            target.MagicFinalShield += value * (1 + target.MagicShieldPercent);
        }

        public static void ProcessMagicShieldPercent(this Target target, double value)
        {
            target.MagicShieldPercent += value;
            target.MagicFinalShield += target.MagicBaseShield * value;
        }

        public static void ProcessPhysicsDamageCoefficient(this Target target, double value)
        {
            target.PhysicsDamageCoefficient += value;
        }

        public static void ProcessMagicDamageCoefficient(this Target target, double value)
        {
            target.MagicDamageCoefficient += value;
        }

        #endregion
    }

    public delegate void TargetModifierDelegate(Target target, double value);

    public static class TargetModifierManager
    {
        public static readonly ImmutableDictionary<ZAttributeType, TargetModifierDelegate> ModifierDict;

        static TargetModifierManager()
        {
            var dict = new Dictionary<ZAttributeType, TargetModifierDelegate>()
            {
                {ZAttributeType.PhysicsFinalShield, TargetModifier.ProcessPhysicsFinalShield},
                {ZAttributeType.PhysicsBaseShield, TargetModifier.ProcessPhysicsBaseShield},
                {ZAttributeType.PhysicsShieldPercent, TargetModifier.ProcessPhysicsShieldPercent},
                {ZAttributeType.MagicFinalShield, TargetModifier.ProcessMagicFinalShield},
                {ZAttributeType.MagicBaseShield, TargetModifier.ProcessMagicBaseShield},
                {ZAttributeType.MagicShieldPercent, TargetModifier.ProcessMagicShieldPercent},
                {ZAttributeType.PhysicsDamageCoefficient, TargetModifier.ProcessPhysicsDamageCoefficient},
                {ZAttributeType.MagicDamageCoefficient, TargetModifier.ProcessMagicDamageCoefficient}
            };
            ModifierDict = dict.ToImmutableDictionary();
        }

        public static void ProcessZAttr(this Target target, ZAttributeType key, double value)
        {
            bool success = ModifierDict.TryGetValue(key, out TargetModifierDelegate modifier);
            if (success)
            {
                modifier(target, value);
            }
            else
            {
                Log.Information($"无效的属性！ {key}:{value} ");
            }
        }

        public static void ProcessZAttr(this Target target, string key, double value)
        {
            ZAttributeType zkey = ZAttributeType.None;
            var success = Enum.TryParse(key, out zkey);
            if (success && zkey != ZAttributeType.None)
            {
                ProcessZAttr(target, zkey, value);
            }
            else
            {
                //Log.Information($"未知的属性！ {key}:{value} ");
            }
        }
    }
}