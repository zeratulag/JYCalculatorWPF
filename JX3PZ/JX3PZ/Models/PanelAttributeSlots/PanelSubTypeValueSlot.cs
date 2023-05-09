using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JX3PZ.Globals;
using System.Collections.Generic;
using System.Linq;

namespace JX3PZ.Models
{
    public class PanelSubTypeValueSlot : PanelSubTypeSlotBase
    {
        // 用于描述攻击力破防这种既有点数又有百分比提升的量

        public readonly string BaseKey; // 属性名称，例如 atVitalityBase
        public string BasePercentAddKey { get; protected set; }// atVitalityBasePercentAdd
        public int Base { get; protected set; } = 0; // 基础属性
        public int BasePercentAdd { get; protected set; } = 0; // 基础增益
        public int Additional { get; protected set; } = 0; // 额外属性
        public int Final { get; protected set; } = 0; // 最终属性值

        public readonly string DescName;

        public PanelSubTypeValueSlot(DamageSubTypeEnum subType, string keySuffix = "", params string[] extraKey) :
            base(subType, extraKey)
        {
            BaseKey = $"at{Name}{keySuffix}";
            BasePercentAddKey = $"{BaseKey}PercentAdd";
            PointAttribute = AttributeID.Get(BaseKey);
            DescName = PointAttribute.SimpleDesc;
        }

        public static int GetFinalValue(int baseValue, int basePercentAdd)
        {
            decimal percentAdd = basePercentAdd / GKiloDenominator;
            decimal finalValue = baseValue * (1 + percentAdd);
            int res = (int) finalValue;
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
        public void UpdateFrom(int baseValue = 0, int basePercentAddValue = 0)
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

        public override void UpdateFrom(IDictionary<string, int> valueDict)
        {
            UpdateFrom(valueDict, ExtraKey);
        }

        #region 显示相关

        public string GetDesc1()
        {
            return Final.ToString();
        }

        public string GetDesc2()
        {
            return $"({Base.ToString()})";
        }

        public string GetValueDesc()
        {
            return GetDesc1();
        }

        public List<string> GetDescTips()
        {
            var res = new List<string>(6)
            {
                $"{PzConstString.Base}{DescName} {Base}",
                $"{PzConstString.Base}{DescName}{PzConstString.PercentAdd} {BasePercentAdd / GKiloDenominator:P2}",
            };

            if (Additional > 0)
            {
                res.Add($"{PzConstString.Additional}{DescName} {Additional}");
            }

            res.Add($"{PzConstString.Final}{DescName} {Final}");
            return res;
        }

        #endregion


    }
}