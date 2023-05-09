using System.Collections.Generic;
using System.Linq;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JX3PZ.Globals;

namespace JX3PZ.Models
{
    public class PanelCriticalDamageSlot : PanelSubTypePercentSlot // 属性
    {
        public readonly string KiloNumRateKey; // atPhysicsCriticalDamagePowerBaseKiloNumRate

        public new static readonly double PointCoef = StaticConst.fGP.CF;
        public const double KiloNumRateCoef = 1024.0;

        public int KiloNumRate { get; protected set; } = 0; // 百分比属性
        public double Final { get; protected set; } = 0; // 最终属性值
        public double PanelFinal => Final + StaticConst.CriticalDamageStart;

        public new static readonly string Suffix = "CriticalDamagePowerBase";

        public PanelCriticalDamageSlot(DamageSubTypeEnum damageSubType, params string[] extraPointKey) :
            base(damageSubType, extraPointKey)
        {
            KiloNumRateKey = $"at{Name}{Suffix}KiloNumRate";

            PointKey = $"at{Name}{Suffix}";
            ExtraKey.Add($"atAllType{Suffix}");

            GetAttributes();
            RateAttribute = AttributeID.Get(KiloNumRateKey);
            if (DamageType == DamageTypeEnum.Magic) ExtraKey.Add($"atMagic{Suffix}");

            if (SubType == DamageSubTypeEnum.Solar || SubType == DamageSubTypeEnum.Lunar)
            {
                ExtraKey.Add($"atSolarAndLunar{Suffix}");
            }
        }

        public static (double PointPct, double RatePct) GetFinalValue(int point, int kiloNumRate)
        {
            double pointPct = point / PointCoef;
            double ratePct = kiloNumRate / KiloNumRateCoef;
            return (pointPct, ratePct);
        }

        public void Calc()
        {
            (PointPct, RatePct) = GetFinalValue(Point, KiloNumRate);
            Final = PointPct + RatePct;
        }

        public void AddKiloNumRate(int value)
        {
            KiloNumRate += value;
            // 注意，需要手动调用Calc()更新计算
        }

        /// <summary>
        /// 更新属性
        /// </summary>
        /// <param name="point">点数属性</param>
        /// <param name="kiloNumRate">百分比属性</param>
        public void UpdateFrom(int point = 0, int kiloNumRate = 0)
        {
            AddPoint(point);
            AddKiloNumRate(kiloNumRate);
            Calc();
        }

        public void UpdateFrom(IDictionary<string, int> valueDict, IEnumerable<string> extraKey)
        {
            int extraValue = 0;
            if (extraKey != null)
            {
                extraValue = extraKey.Sum(key => valueDict.GetValueOrUseDefault(key, 0));
            }

            valueDict.TryGetValue(PointKey, out int point);
            valueDict.TryGetValue(KiloNumRateKey, out int kiloNumRate);
            UpdateFrom(point + extraValue, kiloNumRate);
        }

        public override void UpdateFrom(IDictionary<string, int> valueDict)
        {
            UpdateFrom(valueDict, ExtraKey);
        }


        #region 显示相关

        public string GetDesc1()
        {
            return GetDesc1(PanelFinal);
        }

        public string GetDesc2()
        {
            return GetDesc2(Point);
        }

        public string GetValueDesc()
        {
            var res = $"{GetDesc1()} {GetDesc2()}";
            return res;
        }

        public List<string> GetDescTips()
        {
            var res = new List<string>
            {
                $"{PointDescName} {Point}（{PzConstString.RateAdd}{RateDescName} {PointPct:P2}）",
                $"{RateDescName}{PzConstString.RateAdd} {RatePct:P2}",
                $"{PzConstString.Final}{RateDescName} {PanelFinal:P2}",
                $"会心效果的最大值为{StaticConst.CriticalDamageMax:P2}"
            };
            return res;
        }

        #endregion
    }
}