using JX3CalculatorShared.Class;
using JX3CalculatorShared.Utils;
using JX3PZ.Globals;
using System.Collections.Generic;
using System.Linq;

namespace JX3PZ.Models
{
    public class PanelAttributeSlot : IPanelAttributeSlot // 属性
    {
        public readonly string Name; // 名称，例如Vitality

        public readonly string BaseKey; // 属性名称，例如 atVitalityBase
        public string BasePercentAddKey { get; protected set; } // atVitalityBasePercentAdd

        public readonly HashSet<string> ExtraKey; // 额外的属性名称

        public readonly AttributeID BaseAttribute;
        public string DescName { get; protected set; } // 描述名

        public int Base { get; protected set; } = 0; // 基础属性
        public int BasePercentAdd { get; protected set; } = 0; // 基础增益，在基础属性上按照百分比提升
        public int Additional { get; protected set; } = 0; // 额外属性
        public int Final { get; protected set; } = 0; // 最终属性值

        public const decimal GKiloDenominator = PzConst.GKiloDenominator;

        public PanelAttributeSlot(string name, string keySuffix, params string[] extraKey)
        {
            Name = name;
            BaseKey = $"at{Name}{keySuffix}";
            BasePercentAddKey = $"{BaseKey}PercentAdd";
            ExtraKey = new HashSet<string>(6);
            ExtraKey.AddRange(extraKey);
            BaseAttribute = AttributeID.Get(BaseKey);
            DescName = BaseAttribute.SimpleDesc;
        }

        public static int GetFinalValue(int baseValue, int basePercentAdd)
        {
            decimal percentAdd = basePercentAdd / GKiloDenominator;
            decimal finalValue = baseValue * (1 + percentAdd);
            int res = (int)finalValue;
            return res;
        }

        public void Calc()
        {
            Final = GetFinalValue(Base, BasePercentAdd) + Additional;
        }

        public void AddAdditional(int value)
        {
            Additional += value;
            // 注意，需要手动调用Calc()更新计算
        }

        public void AddBase(int value)
        {
            Base += value;
            // 注意，需要手动调用Calc()更新计算
        }

        public void AddBasePercent(int value)
        {
            BasePercentAdd += value;
            // 注意，需要手动调用Calc()更新计算
        }

        /// <summary>
        /// 更新属性
        /// </summary>
        /// <param name="baseValue">基础属性</param>
        /// <param name="basePercentAddValue">百分比提升属性</param>
        public virtual void UpdateFrom(int baseValue = 0, int basePercentAddValue = 0)
        {
            AddBase(baseValue);
            AddBasePercent(basePercentAddValue);
            Calc();
        }

        public void UpdateFrom(IDictionary<string, int> valueDict, IEnumerable<string> extraKey)
        {
            int extraValue = 0;
            if (extraKey != null)
            {
                extraValue = extraKey.Sum(key => valueDict.GetValueOrUseDefault(key, 0));
            }

            valueDict.TryGetValue(BaseKey, out int baseValue);
            valueDict.TryGetValue(BasePercentAddKey, out int basePercentAddValue);
            UpdateFrom(baseValue + extraValue, basePercentAddValue);
        }

        public virtual void UpdateFrom(IDictionary<string, int> valueDict)
        {
            UpdateFrom(valueDict, ExtraKey);
        }


        #region 生成描述相关

        public string GetValueDesc()
        {
            var res = $"{GetDesc1()} {GetDesc2()}";
            return res;
        }

        public string GetDesc1()
        {
            return $"{Base}";
        }

        public string GetDesc2()
        {
            return $"({Final})";
        }

        public virtual List<string> GetDescTips()
        {
            var res = new List<string>
            {
                $"{PzConstString.Base}{DescName} {Base}",
                $"{PzConstString.Base}{DescName}{PzConstString.PercentAdd} {BasePercentAdd / GKiloDenominator:P2}",
                $"{PzConstString.Final}{DescName} {Final}"
            };

            return res;
        }

        #endregion
    }
}