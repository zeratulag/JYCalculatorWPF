using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JX3PZ.Globals;

namespace JX3PZ.Models
{
    public class PanelCriticalStrikeSlot : PanelSubTypePercentSlot
    {
        public readonly string RateKey; // atPhysicsCriticalStrikeBaseRate

        public new static readonly double PointCoef = StaticConst.fGP.CT;
        public const double RateCoef = 10000.0;

        public new static readonly string Suffix = "CriticalStrike";

        public int BaseRate { get; protected set; } = 0; // 百分比属性
        public double Final { get; protected set; } = 0; // 最终属性值



        public PanelCriticalStrikeSlot(DamageSubTypeEnum damageSubType, params string[] extraPointKey) :
            base(damageSubType, extraPointKey)
        {
            PointKey = $"at{Name}{Suffix}";
            RateKey = $"{PointKey}BaseRate";
            GetAttributes();
            RateAttribute = AttributeID.Get(RateKey);
            ExtraKey.Add($"atAllType{Suffix}");
            if (DamageType == DamageTypeEnum.Magic) ExtraKey.Add($"atMagic{Suffix}");

            if (SubType == DamageSubTypeEnum.Solar || SubType == DamageSubTypeEnum.Lunar)
            {
                ExtraKey.Add($"atSolarAndLunar{Suffix}");
            }
        }

        public static (double PointPct, double RatePct) GetFinalValue(int point, int baseRate)
        {
            double pointPct = point / PointCoef;
            double ratePct = baseRate / RateCoef;
            return (pointPct, ratePct);
        }

        public void Calc()
        {
            (PointPct, RatePct) = GetFinalValue(Point, BaseRate);
            Final = PointPct + RatePct;
        }

        public void AddBaseRate(int value)
        {
            BaseRate += value;
            // 注意，需要手动调用Calc()更新计算
        }

        /// <summary>
        /// 更新属性
        /// </summary>
        /// <param name="point">点数属性</param>
        /// <param name="baseRate">百分比属性</param>
        public void UpdateFrom(int point = 0, int baseRate = 0)
        {
            AddPoint(point);
            AddBaseRate(baseRate);
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
            valueDict.TryGetValue(RateKey, out int baseRate);
            UpdateFrom(point + extraValue, baseRate);
        }

        public override void UpdateFrom(IDictionary<string, int> valueDict)
        {
            UpdateFrom(valueDict, ExtraKey);
        }


        #region 显示相关

        public string GetDesc1()
        {
            return GetDesc1(Final);
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
                $"{PzConstString.Final}{RateDescName} {Final:P2}"
            };
            return res;
        }

        #endregion
    }
}