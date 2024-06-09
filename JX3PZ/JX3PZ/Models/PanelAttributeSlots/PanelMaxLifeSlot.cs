using JX3PZ.Data;
using JX3PZ.Globals;
using System.Collections.Generic;

namespace JX3PZ.Models
{
    public class PanelMaxLifeSlot : IPanelAttributeSlot
    {
        public const decimal GKiloDenominator = PzConst.GKiloDenominator;

        public int MaxLifeBase { get; protected set; } // 基础气血上限 atMaxLifeBase
        public int MaxLifePercentAdd { get; protected set; } // 基础气血上限提高 atMaxLifePercentAdd
        public int MaxLifeAdditional { get; protected set; } // 额外气血上限 atMaxLifeAdditional
        public int FinalMaxLifeAddPercent { get; protected set; } // 最终气血上限提高 atFinalMaxLifeAddPercent
        public int FinalMaxLifeBase { get; protected set; } // 最终基础气血上限
        public int FinalMaxLifeFinal { get; protected set; } // 最终气血上限最终


        public const string MaxLifeBaseKey = "atMaxLifeBase";
        public const string MaxLifePercentAddKey = "atMaxLifePercentAdd";
        public const string MaxLifeAdditionalKey = "atMaxLifeAdditional";
        public const string FinalMaxLifeAddPercentKey = "atFinalMaxLifeAddPercent";

        public const string DescName = "最大气血值";

        public void Calc()
        {
            FinalMaxLifeBase = (int)(MaxLifeBase * (1 + MaxLifePercentAdd / GKiloDenominator)) + MaxLifeAdditional;
            FinalMaxLifeFinal = (int)(FinalMaxLifeBase * (1 + FinalMaxLifeAddPercent / GKiloDenominator));
        }

        public void UpdateFrom(int maxLifeBase = 0, int maxLifePercentAdd = 0, int maxLifeAdditional = 0,
            int finalMaxLifeAddPercent = 0)
        {
            MaxLifeBase += maxLifeBase;
            MaxLifePercentAdd += maxLifePercentAdd;
            MaxLifeAdditional += maxLifeAdditional;
            FinalMaxLifeAddPercent += finalMaxLifeAddPercent;
            Calc();
        }

        public void UpdateFrom(IDictionary<string, int> valueDict)
        {
            valueDict.TryGetValue(MaxLifeBaseKey, out int maxLifeBase);
            valueDict.TryGetValue(MaxLifePercentAddKey, out int maxLifePercentAdd);
            valueDict.TryGetValue(MaxLifeAdditionalKey, out int maxLifeAdditional);
            valueDict.TryGetValue(FinalMaxLifeAddPercentKey, out int finalMaxLifeAddPercent);
            UpdateFrom(maxLifeBase, maxLifePercentAdd, maxLifeAdditional, finalMaxLifeAddPercent);
        }


        public void ApplyLevelData(PlayerLevelData levelData)
        {
            // 应用玩家初始属性的初始气血部分
            MaxLifeBase += levelData.atMaxLifeBase;
            Calc();
        }


        public List<string> GetDescTips()
        {
            var MaxLifeBaseStr = $"{PzConstString.Base}{PzConstString.Base}{DescName} {MaxLifeBase}";
            var MaxLifePercentAddStr =
                $"{PzConstString.Base}{PzConstString.Base}{DescName}{PzConstString.PercentAdd} {MaxLifePercentAdd / GKiloDenominator:P2}";
            var FinalMaxLifeBaseStr = $"{PzConstString.Base}{PzConstString.Final}{DescName} {FinalMaxLifeBase}";
            var FinalMaxLifeFinalStr = $"{PzConstString.Final}{PzConstString.Final}{DescName} {FinalMaxLifeFinal}";
            var DescTips = new List<string>
                {MaxLifeBaseStr, MaxLifePercentAddStr, FinalMaxLifeBaseStr, FinalMaxLifeFinalStr};
            return DescTips;
        }

        public string GetDesc1()
        {
            return FinalMaxLifeFinal.ToString();
        }

        public string GetDesc2()
        {
            return $"({FinalMaxLifeBase})";
        }

        public string GetValueDesc()
        {
            return $"{GetDesc1()} {GetDesc2()}";
        }
    }
}