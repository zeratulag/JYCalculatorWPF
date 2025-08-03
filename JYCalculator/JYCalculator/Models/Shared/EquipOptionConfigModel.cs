using JX3CalculatorShared.Class;
using JX3CalculatorShared.Models;
using JYCalculator.Data;
using JYCalculator.ViewModels;

namespace JYCalculator.Models
{
    public class EquipOptionConfigModel : EquipOptionConfigModelBase
    {

        #region 构造

        public EquipOptionConfigModel() : base()
        {
        }

        public EquipOptionConfigModel(WPOption wp, YZOption yz, bool jn, bool sl) : base(wp, yz, jn, sl)
        {
        }

        public void UpdateInput(EquipOptionConfigViewModel vm)
        {
            UpdateInput(vm.SelectedWP, vm.SelectedYZ, vm.JN, vm.SL);
        }

        #endregion

        public override void Calc()
        {
            GetRecipes();
            GetSkillEvents();
        }

        public void GetRecipes()
        {
            var dict = StaticXFData.DB.Recipe.OtherAssociate;

            AssociateKeys.Clear();
            OtherRecipes.Clear();

            var wpkey = $"武器-{WP.Tag}";
            if (dict.ContainsKey(wpkey))
            {
                AssociateKeys.Add(wpkey);
            }

            if (JN)
            {
                AssociateKeys.Add("套装-JN");
            }


            foreach (var k in AssociateKeys)
            {
                OtherRecipes.AddRange(dict[k]);
            }
        }

        public void GetSkillEvents()
        {
            SkillEvents.Clear();
            if (SL)
            {
                SkillEvents.Add(StaticXFData.DB.BaseSkillInfo.Events["SL"]);
            }
            SkillEvents.AddRange(WP.SkillEvents);
        }

    }
}