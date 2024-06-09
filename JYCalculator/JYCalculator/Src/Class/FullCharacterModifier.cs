using JX3CalculatorShared.Globals;
using JYCalculator.Globals;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using static JYCalculator.Globals.XFStaticConst;


namespace JYCalculator.Class
{
    // 用于对FullCharacter进行属性计算

    public static class FullCharacterModifier
    {
        #region 属性计算部分

        public static void ProcessWP(this FullCharacter fChar, double value)
        {
            fChar.WP += value;
        }

        public static void ProcessCT(this FullCharacter fChar, double value)
        {
            if (fChar.Has_Special_Buff)
            {
                throw new ArgumentException("Cannot add CT after has special_buff!");
            }
            else
            {
                fChar.CT += value;
            }
        }

        public static void ProcessCT_Point(this FullCharacter fChar, double value)
        {
            ProcessCT(fChar, value / fGP.CT);
        }

        public static void ProcessCF(this FullCharacter fChar, double value)
        {
            fChar.CF += value;
        }

        public static void ProcessCF_Point(this FullCharacter fChar, double value)
        {
            ProcessCF(fChar, value / fGP.CF);
        }

        public static void ProcessWS(this FullCharacter fChar, double value)
        {
            fChar.WS += value;
        }

        public static void ProcessWS_Point(this FullCharacter fChar, double value)
        {
            ProcessWS(fChar, value / fGP.WS);
        }

        public static void ProcessHSP(this FullCharacter fChar, double value)
        {
            fChar.HS += value; // 注意此处加速改变
        }

        public static void ProcessExtraSP(this FullCharacter fChar, double value)
        {
            fChar.ExtraSP += value; // 注意此处加速改变
        }

        public static void ProcessPZ(this FullCharacter fChar, double value)
        {
            fChar.PZ += value;
        }

        public static void ProcessFinal_AP(this FullCharacter fChar, double value)
        {
            fChar.Final_AP += value;
        }

        public static void ProcessBase_AP(this FullCharacter fChar, double value)
        {
            fChar.Base_AP += value;
            ProcessFinal_AP(fChar, value * (1 + fChar.AP_Percent));
        }

        public static void ProcessAP_Percent(this FullCharacter fChar, double value)
        {
            fChar.AP_Percent += value;
            ProcessFinal_AP(fChar, value * fChar.Base_AP);
        }

        public static void ProcessFinal_OC(this FullCharacter fChar, double value)
        {
            fChar.Final_OC += value;
        }

        public static void ProcessBase_OC(this FullCharacter fChar, double value)
        {
            fChar.Base_OC += value;
            ProcessFinal_OC(fChar, value * (1 + fChar.OC_Percent));
        }

        public static void ProcessOC_Percent(this FullCharacter fChar, double value)
        {
            fChar.OC_Percent += value;
            ProcessFinal_OC(fChar, value * fChar.Base_OC);
        }

        public static void ProcessS(this FullCharacter fChar, double value) //身法
        {
            ProcessCT_Point(fChar, value * XFConsts.CT_PER_S);
        }


        public static void ProcessIgnoreA(this FullCharacter fChar, double value)
        {
            fChar.IgnoreA += value;
        }

        public static void ProcessDmgAdd(this FullCharacter fChar, double value)
        {
            fChar.DmgAdd += value;
        }

        public static void ProcessP_DmgAdd(this FullCharacter fChar, double value)
        {
            ProcessDmgAdd(fChar, value);
        }

        public static void ProcessAll_DmgAdd(this FullCharacter fChar, double value)
        {
            ProcessP_DmgAdd(fChar, value);
        }


        public static void ProcessFinal_L(this FullCharacter fChar, double value) // 增加力道
        {
            fChar.Final_L += value;
            ProcessBase_AP(fChar, value * XFConsts.AP_PER_L);
            ProcessBase_OC(fChar, value * XFConsts.OC_PER_L);
            ProcessFinal_AP(fChar, value * XFConsts.F_AP_PER_L);
            ProcessCT_Point(fChar, value * XFConsts.CT_PER_L);
        }

        public static void ProcessBase_L(this FullCharacter fChar, double value)
        {
            fChar.Base_L += value;
            ProcessFinal_L(fChar, value * (1 + fChar.L_Percent));
        }

        public static void ProcessL_Percent(this FullCharacter fChar, double value)
        {
            fChar.L_Percent += value;
            ProcessFinal_L(fChar, value * fChar.Base_L);
        }


        /// <summary>
        /// 增加全属性
        /// </summary>
        /// <param name="value"></param>
        public static void ProcessAll_BasePotent(this FullCharacter fChar, double value)
        {
            ProcessBase_L(fChar, value);
            ProcessS(fChar, value);
        }

        #endregion

        #region 属性转换部分

        /// <summary>
        /// 在会破属性之和保持不变的情况下，转移部分会心点数到破防
        /// </summary>
        /// <param name="value">点数</param>
        public static void TransCTToOC(this FullCharacter fChar, double value)
        {
            ProcessCT_Point(fChar, -value);
            ProcessBase_OC(fChar, value);
        }

        /// <summary>
        /// 在无招属性之和保持不变的情况下，转移部分无双点数到破招
        /// </summary>
        /// <param name="value">点数</param>
        public static void TransWSToPZ(this FullCharacter fChar, double value)
        {
            ProcessWS_Point(fChar, -value);
            ProcessPZ(fChar, value);
        }

        /// <summary>
        /// 在会破属性之和保持不变的情况下，重新设置面板会心百分比
        /// </summary>
        /// <param name="ct">目标会心百分比</param>
        public static void Reset_CT(this FullCharacter fChar, double ct)
        {
            var delta = fChar.CT_Point - ct * XFStaticConst.fGP.CT;
            TransCTToOC(fChar, delta);
        }

        /// <summary>
        /// 在无招属性之和保持不变的情况下，重新设置面板无双百分比
        /// </summary>
        /// <param name="ws">目标无双百分比</param>
        public static void Reset_WS(this FullCharacter fChar, double ws)
        {
            var delta = fChar.WS_Point - ws * XFStaticConst.fGP.WS;
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
                {ZAttributeType.WP, FullCharacterModifier.ProcessWP},
                {ZAttributeType.CT, FullCharacterModifier.ProcessCT},
                {ZAttributeType.CT_Point, FullCharacterModifier.ProcessCT_Point},
                {ZAttributeType.CF, FullCharacterModifier.ProcessCF},
                {ZAttributeType.CF_Point, FullCharacterModifier.ProcessCF_Point},
                {ZAttributeType.WS, FullCharacterModifier.ProcessWS},
                {ZAttributeType.WS_Point, FullCharacterModifier.ProcessWS_Point},
                {ZAttributeType.HSP, FullCharacterModifier.ProcessHSP},
                {ZAttributeType.ExtraSP, FullCharacterModifier.ProcessExtraSP},
                {ZAttributeType.PZ, FullCharacterModifier.ProcessPZ},
                {ZAttributeType.Final_AP, FullCharacterModifier.ProcessFinal_AP},
                {ZAttributeType.Base_AP, FullCharacterModifier.ProcessBase_AP},
                {ZAttributeType.AP_Percent, FullCharacterModifier.ProcessAP_Percent},
                {ZAttributeType.Final_OC, FullCharacterModifier.ProcessFinal_OC},
                {ZAttributeType.Base_OC, FullCharacterModifier.ProcessBase_OC},
                {ZAttributeType.OC_Percent, FullCharacterModifier.ProcessOC_Percent},
                {ZAttributeType.S, FullCharacterModifier.ProcessS},
                {ZAttributeType.IgnoreA, FullCharacterModifier.ProcessIgnoreA},
                {ZAttributeType.DmgAdd, FullCharacterModifier.ProcessDmgAdd},
                {ZAttributeType.Final_L, FullCharacterModifier.ProcessFinal_L},
                {ZAttributeType.Base_L, FullCharacterModifier.ProcessBase_L},
                {ZAttributeType.L_Percent, FullCharacterModifier.ProcessL_Percent},
                {ZAttributeType.All_BasePotent, FullCharacterModifier.ProcessAll_BasePotent},
                {ZAttributeType.P_DmgAdd, FullCharacterModifier.ProcessP_DmgAdd},
                {ZAttributeType.All_DmgAdd, FullCharacterModifier.ProcessAll_DmgAdd},

                {ZAttributeType.P_Base_AP, FullCharacterModifier.ProcessBase_AP},
                {ZAttributeType.P_AP_Percent, FullCharacterModifier.ProcessAP_Percent},
                {ZAttributeType.P_Base_OC, FullCharacterModifier.ProcessBase_OC},
                {ZAttributeType.P_OC_Percent, FullCharacterModifier.ProcessOC_Percent},
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
                Trace.WriteLine($"无效的属性！ {key}:{value} ");
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
                Trace.WriteLine($"未知的属性！ {key}:{value} ");
            }
        }
    }
}