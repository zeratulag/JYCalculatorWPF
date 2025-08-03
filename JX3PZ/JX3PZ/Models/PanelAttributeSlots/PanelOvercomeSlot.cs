﻿using JX3CalculatorShared.Globals;

namespace JX3PZ.Models
{
    public class PanelOvercomeSlot : PanelSubTypeValueSlot
    {
        public static readonly double PointCoef = StaticConst.CurrentLevelGlobalParams.Overcome;
        public double FinalPct => Final / PointCoef;
        public new static readonly string Suffix = "OvercomeBase";

        public PanelOvercomeSlot(DamageSubTypeEnum subType, params string[] extraKey) :
            base(subType, keySuffix: Suffix, extraKey)
        {
            BasePercentAddKey = $"at{Name}OvercomePercent";
            if (DamageType == DamageTypeEnum.Magic)
            {
                ExtraKey.Add($"atMagicOvercome");
            }
            if (SubType == DamageSubTypeEnum.Solar || SubType == DamageSubTypeEnum.Lunar)
            {
                ExtraKey.Add($"atSolarAndLunar{Suffix}");
            }
        }

        public new string GetDesc1()
        {
            return $"{FinalPct:P2}";
        }

        public new string GetDesc2()
        {
            return $"({Final})";
        }

        public new string GetValueDesc()
        {
            var res = $"{GetDesc1()} {GetDesc2()}";
            return res;
        }
    }
}