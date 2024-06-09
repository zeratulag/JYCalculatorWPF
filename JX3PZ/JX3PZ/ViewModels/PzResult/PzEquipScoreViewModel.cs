using JX3PZ.Models;

namespace JX3PZ.ViewModels
{
    public class PzEquipScoreViewModel : PzAttributeSlotsViewModelBase
    {
        public PzEquipScoreViewModel() : base()
        {
            Title = "装备分数";
        }

        public void UpdateFrom(EquipScore equipScore)
        {
            ValueDesc = equipScore.TotalScore.ToString();
            Desc1 = ValueDesc;
            ToolTip = equipScore.GetToolTip();
        }

        public override void UpdateFrom(PzPlanModel model)
        {
            UpdateFrom(model.CEquipScore);
        }
    }
}