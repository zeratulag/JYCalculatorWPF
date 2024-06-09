using JX3CalculatorShared.Globals;
using JX3PZ.Data;
using JX3PZ.Globals;
using System.Collections.Immutable;

namespace JX3PZ.Class
{
    public class AttributeStrengthEntry : AttributeEntry
    {
        // 表示可以精炼强化的属性词条
        public static readonly ImmutableArray<decimal> StrengthFactors; // 精炼系数%

        static AttributeStrengthEntry()
        {
            const int n = 11;
            var res = new decimal[n];
            for (int i = 0; i < n; i++)
            {
                res[i] = CalcStrengthFactor(i);
            }

            StrengthFactors = res.ToImmutableArray();
        }

        public int AddValue { get; private set; } = 0; // 精炼值
        public int StrengthLevel { get; private set; } // 精炼等级
        public int FinalValue => Value + AddValue;

        public AttributeStrengthEntry(string modifyType, int baseValue,
            AttributeEntryTypeEnum entryType = AttributeEntryTypeEnum.Default, int strengthLevel = 0) : base(modifyType,
            baseValue, entryType)
        {
            Value = baseValue;
            Strength(strengthLevel);
        }

        public AttributeStrengthEntry(AttributeTabItem item,
            AttributeEntryTypeEnum entryType = AttributeEntryTypeEnum.Default, int strengthLevel = 0) : base(item,
            entryType)
        {
            Strength(strengthLevel);
        }

        // 进行精炼
        public void Strength(int level)
        {
            StrengthLevel = level;
            Update();
        }

        public void Update()
        {
            if (Attribute.IsValue && Attribute.CanStrength)
            {
                AddValue = GetStrengthValue(Value, StrengthLevel);
            }
            // 更新数值
        }

        /// <summary>
        /// 精炼提升属性百分比
        /// </summary>
        /// <param name="k">强化等级</param>
        /// <returns>属性提升百分比</returns>
        private static decimal CalcStrengthFactor(int k)
        {
            decimal y = k * (k * 0.3m + 0.7m) / 2m;
            return y;
        }

        /// <summary>
        /// 获取精炼提升的属性
        /// </summary>
        /// <param name="baseValue">基础值</param>
        /// <param name="k">精炼等级</param>
        /// <returns>精炼提升值</returns>
        public static int GetStrengthValue(double baseValue, int k)
        {
            var v = (decimal)baseValue * StrengthFactors[k] / 100m;
            int res = v.MathRound();
            return res;
        }

        public string[] GetDescs()
        {
            string[] res;
            if (Attribute.IsValue)
            {
                if (Attribute.CanStrength && StrengthLevel > 0)
                {
                    res = Attribute.GetStrengthDescs(Value, AddValue, EntryType);
                }
                else
                {
                    res = new[] { Attribute.GetDesc(Value, EntryType) };
                }
            }
            else
            {
                res = new[] { Desc };
            }

            return res;
        }

        public override string GetDesc()
        {
            string res;
            if (Attribute.IsValue && Attribute.CanStrength)
            {
                if (StrengthLevel > 0)
                {
                    res = Attribute.GetStrengthDesc(Value, AddValue, EntryType);
                }
                else
                {
                    res = Attribute.GetDesc(Value, EntryType);
                }
            }
            else
            {
                res = Desc;
            }

            return res;
        }

        /// <summary>
        /// 转化为最终属性词条（以精炼后的值为准）
        /// </summary>
        /// <returns></returns>
        public AttributeEntry ToFinalAttributeEntry()
        {
            var res = new AttributeEntry(ModifyType, FinalValue, EntryType);
            return res;
        }
    }
}