using JX3CalculatorShared.Globals;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace JX3CalculatorShared.Class
{
    public static class TargetModifier
    {
        #region 属性计算部分

        public static void ProcessFinal_PDef(this Target target, double value)
        {
            target.Final_PDef += value;
        }

        public static void ProcessBase_PDef(this Target target, double value)
        {
            target.Base_PDef += value;
            target.Final_PDef += value * (1 + target.PDef_Percent);
        }

        public static void ProcessPDef_Percent(this Target target, double value)
        {
            target.PDef_Percent += value;
            target.Final_PDef += target.Base_PDef * value;
        }

        public static void ProcessFinal_MDef(this Target target, double value)
        {
            target.Final_MDef += value;
        }

        public static void ProcessBase_MDef(this Target target, double value)
        {
            target.Base_MDef += value;
            target.Final_MDef += value * (1 + target.MDef_Percent);
        }

        public static void ProcessMDef_Percent(this Target target, double value)
        {
            target.MDef_Percent += value;
            target.Final_MDef += target.Base_MDef * value;
        }

        public static void ProcessP_YS(this Target target, double value)
        {
            target.P_YS += value;
        }

        public static void ProcessM_YS(this Target target, double value)
        {
            target.M_YS += value;
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
                {ZAttributeType.Final_PDef, TargetModifier.ProcessFinal_PDef},
                {ZAttributeType.Base_PDef, TargetModifier.ProcessBase_PDef},
                {ZAttributeType.PDef_Percent, TargetModifier.ProcessPDef_Percent},
                {ZAttributeType.Final_MDef, TargetModifier.ProcessFinal_MDef},
                {ZAttributeType.Base_MDef, TargetModifier.ProcessBase_MDef},
                {ZAttributeType.MDef_Percent, TargetModifier.ProcessMDef_Percent},
                {ZAttributeType.P_YS, TargetModifier.ProcessP_YS},
                {ZAttributeType.M_YS, TargetModifier.ProcessM_YS}
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
                Trace.WriteLine($"无效的属性！ {key}:{value} ");
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
                Trace.WriteLine($"未知的属性！ {key}:{value} ");
            }
        }
    }
}