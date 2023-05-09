using JX3CalculatorShared.Globals;

namespace JX3PZ.Models
{
    public class PanelOvercomeSlot : PanelSubTypeValueSlot
    {
        public new static readonly double PointCoef = StaticConst.fGP.OC;
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

        public string GetDesc1()
        {
            return $"{FinalPct:P2}";
        }

        public string GetDesc2()
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