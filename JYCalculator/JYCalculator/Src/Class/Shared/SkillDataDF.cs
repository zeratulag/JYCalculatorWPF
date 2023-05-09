using JX3CalculatorShared.Globals;
using JYCalculator.Globals;

namespace JYCalculator.Class
{
    public partial class SkillDataDF
    {
        public override void PostProceed()
        {
            switch (AppStatic.XinFaTag)
            {
                case "JY":
                    {
                        Data["PZ_ZM"].IgnoreB = Data["ZM"].IgnoreB; // 破招也能吃到无视防御
                        break;
                    }
                case "TL":
                    {
                        break;
                    }
            }
        }
    }
}