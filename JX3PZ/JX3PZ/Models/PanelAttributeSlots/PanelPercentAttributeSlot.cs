using JX3CalculatorShared.Class;
using JX3CalculatorShared.Utils;
using JX3PZ.Globals;
using System.Collections.Generic;
using System.Linq;

namespace JX3PZ.Models
{
    public enum PointToPercentConvertTypeEnum
    {
        // 点数属性转换的百分比的方式，防御类属性为非线性转换
        Linear, // 线性
        NonLinear, // 非线性转化
    }


    public class PanelPercentAttributeSlot : IPanelAttributeSlot // 属性
    {
        // 用于描述加速这种既有点数，又有百分比直加的无伤害类型的类
        public readonly PointToPercentConvertTypeEnum ConvertType;

        public readonly string Name; // 名称，例如Haste

        public readonly string BasePointKey = null; // 属性名称，例如 atHasteBase
        public string BasePointPercentAddKey { get; protected set; } = null; // atHasteBasePercentAdd
        public string AdditionalPointKey { get; protected set; } = null; // atHasteBasePercentAdd
        public string PercentKey { get; protected set; } = null; // 百分比直接加的key

        public readonly HashSet<string> ExtraKey; // 额外的属性名称

        public int BasePoint { get; protected set; } = 0; // 基础点数属性（加速等级）
        public int BasePointPercentAdd { get; protected set; } = 0; // 百分比提升
        public double PercentAddDenominator { get; protected set; } = 1024.0; // 百分比提升属性的分母

        public int AdditionalPoint { get; protected set; } = 0; //  额外点数属性
        public int Percent { get; protected set; } = 0; // 百分比直加属性（郭氏加速值）

        public double PercentDenominator { get; protected set; } = 1024.0; // 直加属性的分母

        // 计算获得
        public int FinalPoint { get; protected set; } = 0; // 最终点数
        public int GFinal { get; protected set; } = 0; // 最终属性值（最终郭氏加速值）
        public double Final { get; protected set; } = 0.0; // 最终属性值（浮点数）

        public virtual double Coef { get; protected set; } = 0; // 换算系数

        public readonly KAttributeID PointAttribute;
        public KAttributeID PercentAttribute { get; protected set; }
        public string PointDescName => PointAttribute.FullDesc; // 点数描述名
        public string PercentDescName => PercentAttribute.FullDesc; // 几率描述名

        public double FinalPointDenominator { get; protected set; }

        public double FinalPointPct { get; protected set; }
        public double PercentPct { get; protected set; }

        public PanelPercentAttributeSlot(string name, string keySuffix, double coef,
            PointToPercentConvertTypeEnum convertType = PointToPercentConvertTypeEnum.Linear,
            params string[] extraKey)
        {
            Name = name;
            BasePointKey = $"at{Name}{keySuffix}";
            Coef = coef;
            ConvertType = convertType;
            ExtraKey = new HashSet<string>(6);
            ExtraKey.AddRange(extraKey);

            PointAttribute = KAttributeID.Get(BasePointKey);
        }

        public double GetFinalPoint()
        {
            // 计算最终点数
            var fPoint = BasePoint * (1 + (double)BasePointPercentAdd / PercentAddDenominator) + AdditionalPoint;
            FinalPoint = (int)fPoint;
            return fPoint;
        }

        public void GetFinalValue()
        {
            double denominator = Coef;
            if (ConvertType == PointToPercentConvertTypeEnum.NonLinear)
            {
                denominator += FinalPoint;
            }

            FinalPointDenominator = denominator;

            FinalPointPct = FinalPoint / FinalPointDenominator;
            PercentPct = Percent / PercentDenominator;

            Final = FinalPointPct + PercentPct;
            GFinal = (int)(FinalPoint * PercentAddDenominator / FinalPointDenominator) + Percent;
        }

        public void Calc()
        {
            GetFinalPoint();
            GetFinalValue();
        }

        public void UpdateFrom(int basePoint = 0, int basePointPercentAdd = 0, int additionalPoint = 0, int percent = 0)
        {
            BasePoint += basePoint;
            BasePointPercentAdd += basePointPercentAdd;
            AdditionalPoint += additionalPoint;
            Percent += percent;
            Calc();
        }

        public void UpdateFrom(IDictionary<string, int> valueDict, IEnumerable<string> extraKey)
        {
            int extraValue = 0;
            if (extraKey != null)
            {
                extraValue = extraKey.Sum(key => valueDict.GetValueOrUseDefault(key, 0));
            }

            valueDict.TryGetValue(BasePointKey, out int point);
            var basePoint = point + extraValue;

            int basePointPercentAdd = 0;
            int additionalPoint = 0;
            int percent = 0;

            if (BasePointPercentAddKey != null)
            {
                valueDict.TryGetValue(BasePointPercentAddKey, out basePointPercentAdd);
            }

            if (AdditionalPointKey != null)
            {
                valueDict.TryGetValue(AdditionalPointKey, out additionalPoint);
            }

            if (PercentKey != null)
            {
                valueDict.TryGetValue(PercentKey, out percent);
            }

            UpdateFrom(basePoint, basePointPercentAdd, additionalPoint, percent);
        }

        public virtual void UpdateFrom(IDictionary<string, int> valueDict)
        {
            UpdateFrom(valueDict, ExtraKey);
        }


        #region 生成描述相关

        public string GetDesc1()
        {
            return $"{Final:P2}";
        }

        public string GetDesc2()
        {
            return $"({FinalPoint})";
        }

        public string GetValueDesc()
        {
            var res = $"{GetDesc1()} {GetDesc2()}";
            return res;
        }

        public virtual List<string> GetDescTips()
        {
            if (BasePointPercentAdd > 0 || Name.EndsWith("Shield"))
            {
                return GetDescTipsBaseAndFinal();
            }
            else
            {
                return GetDescTipsBase();
            }
        }

        public string GetPctDescName()
        {
            string pctDescName = PointAttribute.SimpleDesc;
            if (PercentAttribute != null)
            {
                pctDescName = PercentDescName;
            }

            return pctDescName;
        }

        private List<string> GetDescTipsBase()
        {
            // 仅有基础值，没有额外和最终值
            string pctDescName = GetPctDescName();

            var res = new List<string>(6)
            {
                $"{PointDescName} {FinalPoint}（{PzConstString.RateAdd}{pctDescName} {FinalPointPct:P2}）",
            };
            if (Percent > 0)
            {
                res.Add($"{pctDescName}{PzConstString.RateAdd} {PercentPct:P2}");
            }

            res.Add($"{PzConstString.Final}{pctDescName} {Final:P2}");
            return res;
        }


        private List<string> GetDescTipsBaseAndFinal()
        {
            string pctDescName = GetPctDescName();

            var res = new List<string>(8)
            {
                $"{PzConstString.Base}{PointDescName} {BasePoint}",
                $"{PointDescName}{PzConstString.PercentAdd} {BasePointPercentAdd / PercentAddDenominator:P2}",
            };

            if (AdditionalPoint > 0)
            {
                res.Add($"{PzConstString.Additional}{PointDescName} {AdditionalPoint}");
            }

            res.Add($"{PzConstString.Final}{PointDescName} {FinalPoint}");
            res.Add($"{PzConstString.Final}{pctDescName} {Final:P2}");
            return res;
        }

        #endregion
    }
}