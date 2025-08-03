using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JX3PZ.Globals;
using System.Collections.Generic;

namespace JX3PZ.Models
{
    public class PanelSubTypeSlotBase : IPanelAttributeSlot
    {
        // 用于描述会心，会效这种既有点数又有百分比的量

        public readonly DamageSubTypeEnum SubType;
        public readonly DamageTypeEnum DamageType;
        public readonly string Name;
        public readonly HashSet<string> ExtraKey; // 额外的属性名称
        public KAttributeID PointAttribute { get; protected set; }
        public KAttributeID RateAttribute { get; protected set; }

        public static readonly string Suffix = "";

        public const decimal GKiloDenominator = PzConst.GKiloDenominator;
        public string PointDescName => PointAttribute.FullDesc; // 点数描述名
        public string RateDescName => RateAttribute.FullDesc; // 几率描述名

        public PanelSubTypeSlotBase(DamageSubTypeEnum subType, params string[] extraKey)
        {
            SubType = subType;
            DamageType = SubType.GetDamageType();
            Name = SubType.ToString();
            ExtraKey = new HashSet<string>(6);
            ExtraKey.AddRange(extraKey);
        }

        public virtual void UpdateFrom(IDictionary<string, int> valueDict)
        {
            throw new System.NotImplementedException();
        }

        #region 生成描述相关

        public static string GetDesc1(double final)
        {
            var res = $"{final:P2}";
            return res;
        }

        public static string GetDesc2(int point)
        {
            var res = $"({point})";
            return res;
        }

        public static string GetValueDesc(double final, int point)
        {
            var d1 = GetDesc1(final);
            var d2 = GetDesc2(point);
            var res = $"{d1} {d2}";
            return res;
        }

        #endregion

    }
}