using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;

namespace JX3PZ.Models
{
    public class PanelSubTypePercentSlot : PanelSubTypeSlotBase
    {
        // 描述会心会效这种以百分比计数的量
        public string PointKey { get; protected set; } // 属性名称，例如 atPhysicsCriticalStrike
        public int Point { get; protected set; } = 0; // 点数属性
        public double PointPct { get; protected set; }
        public double RatePct { get; protected set; }

        public static readonly double PointCoef;
        public new static readonly string Suffix;
        public void AddPoint(int value)
        {
            Point += value;
            // 注意，需要手动调用Calc()更新计算
        }

        public PanelSubTypePercentSlot(DamageSubTypeEnum damageSubType, params string[] extraPointKey) : base(
            damageSubType, extraPointKey)
        {

        }

        public void GetAttributes()
        {
            PointAttribute = AttributeID.Get(PointKey);
        }

    }
}