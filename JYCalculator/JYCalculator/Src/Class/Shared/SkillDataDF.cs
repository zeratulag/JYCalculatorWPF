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
                    SyncBaoYu();
                    SyncBaiYu();
                    break;
                }
                case "TL":
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 同步暴雨的秘籍给梨花暴雨
        /// </summary>
        public void SyncBaoYu()
        {
            var lhby = Data["LHBY"];
            var by = Data["BY"];
            lhby.IgnoreB = by.IgnoreB;
            lhby.AddDmg = by.AddDmg;
            lhby.AddCT = by.AddCT;
            lhby.AddCF = by.AddCF;
        }


        /// <summary>
        /// 白雨跳珠(非侠士) 吃追命的无视防御
        /// </summary>
        public void SyncBaiYu()
        {
            Data["BaiYuTiaoZhu"].IgnoreB = Data["ZM"].IgnoreB;
        }

    }
}