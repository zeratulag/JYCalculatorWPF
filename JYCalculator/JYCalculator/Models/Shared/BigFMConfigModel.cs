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
                SkillEvents.Add(StaticXFData.DB.BaseSkillInfo.Events["BigFM_BELT"]);
            }

            if (Wrist != null)
            {
                SkillEvents.Add(Wrist.ExpansionPackLevel == 130
                    ? StaticXFData.DB.BaseSkillInfo.Events["BigFM_WRIST_130"]
                    : StaticXFData.DB.BaseSkillInfo.Events["BigFM_WRIST"]);
            }

            if (Shoes != null)
            {
                string key = $"BigFM_SHOES_{Shoes.ExpansionPackLevel}";
                SkillEvents.Add(StaticXFData.DB.BaseSkillInfo.Events[key]);
            }
        }
    }
}