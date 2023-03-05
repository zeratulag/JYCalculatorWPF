using JX3CalculatorShared.Models;
using JYCalculator.Data;
using JYCalculator.ViewModels;

namespace JYCalculator.Models
{
    public class BigFMConfigModel : BigFMConfigModelBase
    {

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
                SkillEvents.Add(StaticXFData.DB.SkillInfo.Events["BigFM_BELT"]);
            }

            if (Wrist != null)
            {
                SkillEvents.Add(StaticXFData.DB.SkillInfo.Events["BigFM_WRIST"]);
            }

            if (Shoes != null)
            {
                string key = $"BigFM_SHOES_{Shoes.DLCLevel}";
                SkillEvents.Add(StaticXFData.DB.SkillInfo.Events[key]);
            }

        }

    }
}