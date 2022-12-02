using JX3CalculatorShared.Models;
using JYCalculator.Src.Data;
using JYCalculator.ViewModels;

namespace JYCalculator.Models
{
    public class BigFMConfigModel: BigFMConfigModelBase
    {
        #region 成员

        #endregion

        #region 构建

        public void UpdateInput(BigFMConfigViewModel vm)
        {
            Dict = vm.EnabledItems;
            Dispatch();
            Calc();
        }

        #endregion

        #region 计算

        public override void Calc()
        {
            base.Calc();
            GetSkillEvents();
        }

        #endregion

        public void GetSkillEvents()
        {
            SkillEvents.Clear();
            if (Belt != null)
            {
                SkillEvents.Add(StaticJYData.DB.SkillInfo.Events["BigFM_BELT"]);
            }

            if (Wrist != null)
            {
                SkillEvents.Add(StaticJYData.DB.SkillInfo.Events["BigFM_WRIST"]);
            }

            if (Shoes != null)
            {
                string key = $"BigFM_SHOES_{Shoes.DLCLevel}";
                SkillEvents.Add(StaticJYData.DB.SkillInfo.Events[key]);
            }

        }

    }
}