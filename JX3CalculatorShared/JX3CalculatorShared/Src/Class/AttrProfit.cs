using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JX3CalculatorShared.Class
{
    /// <summary>
    /// 存储属性收益值的结构体
    /// </summary>
    public struct AttrProfitItem
    {
        public string Name { get; }
        public string DescName { get; }
        public double Value { get; }

        public AttrProfitItem(string name, string descName, double value)
        {
            Name = name;
            DescName = descName;
            Value = value;
        }
    }

    public class AttrProfitList
    {
        public const double Threshold1 = 0.1;
        public const double ThresHold2 = 0.03;
        public const double ThresHold3 = 0.01;

        public List<AttrProfitItem> Data;
        public string OrderDesc;

        public AttrProfitList(List<AttrProfitItem> data)
        {
            Data = data;
        }

        public void Proceed()
        {
            Data = Data.OrderByDescending(_ => _.Value).ToList();
            OrderDesc = GetOrderDesc();
        }

        public string GetOrderDesc()
        {
            var sb = new StringBuilder();
            sb.Append(Data[0].DescName);
            for (int i = 1; i < Data.Count; i++)
            {
                var advantage = Data[i - 1].Value / Data[i].Value - 1;
                var sep = GetSep(advantage);
                sb.Append($" {sep} ");
                sb.Append(Data[i].DescName);
            }

            var res = sb.ToString();
            return res;
        }

        public static string GetSep(double adv)
        {
            string sep;
            if (adv > Threshold1)
            {
                sep = "≫";
            }
            else
            {
                if (adv > ThresHold2)
                {
                    sep = ">";
                }
                else
                {
                    if (adv > ThresHold3)
                    {
                        sep = "≥";
                    }
                    else
                    {
                        sep = "≈";
                    }
                }
            }

            return sep;
        }


        // 五行石镶嵌孔没有武伤
        public void AsDiamondAttrProfitList()
        {
            for (int i = 0; i < Data.Count; i++)
            {
                if (Data[i].Name == "WP")
                {
                    Data.RemoveAt(i);
                    break;
                }
            }

            Proceed();

        }
    }
}