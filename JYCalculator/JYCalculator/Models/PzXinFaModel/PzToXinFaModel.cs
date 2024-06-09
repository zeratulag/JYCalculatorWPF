using JX3CalculatorShared.Common;
using JX3CalculatorShared.ViewModels;
using JX3PZ.Data;
using JX3PZ.Globals;
using JX3PZ.Models;
using JYCalculator.Class;
using JYCalculator.Globals;
using JYCalculator.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace JYCalculator.Models
{
    public class PzToXinFaInputAdapter : IModel
    {
        // 将通用的不带心法的配装方案转化为心法相关的输入
        public readonly PzPlanModel PlanModel;

        public InitCharacter InitChar;
        public EquipOptionConfigSav EquipOption;
        public BigFMSlotConfig[] BigFM;

        public InitInputSav InputSav;

        public PzToXinFaInputAdapter(PzPlanModel planModel)
        {
            PlanModel = planModel;
        }

        public void Calc()
        {
            GetInputSave();
        }

        public void GetInputSave()
        {
            GetInitCharacter();
            GetEquipOption();
            GetBigFMConfig();
            InputSav = new InitInputSav(InitChar, EquipOption, BigFM);
        }

        private void GetInitCharacter()
        {
            InitChar = new InitCharacter(PlanModel.XFPanel);
        }

        private void GetEquipOption()
        {
            var keys = PlanModel.SetModel.SetKeys;
            bool jn = keys.Contains("JN");
            bool sl = keys.Contains("SL");
            string wpName = PlanModel.PrimaryWeaponModel.GetEquipOptionName();
            string yzName = PlanModel.PendantModel.GetEquipOptionName();
            EquipOption = new EquipOptionConfigSav(jn, sl, wpName, yzName);
        }

        private void GetBigFMConfig()
        {
            var full = PlanModel.SnapShots.Select(_ => _.GetBigFMSlotConfig()).ToArray();
            var order = GlobalContext.BigFMSlots;
            var res = new List<BigFMSlotConfig>(order.Length);
            foreach (var _ in order)
            {
                int pos = EquipMapLib.EquipSubType[(int)_].Position;
                res.Add(full[pos]);
            }

            BigFM = res.ToArray();
            InitChar.Had_BigFM_jacket = PlanModel.SnapShots[(int)EquipSlotEnum.JACKET].CBigFM != null;
            InitChar.Had_BigFM_hat = PlanModel.SnapShots[(int)EquipSlotEnum.HAT].CBigFM != null;
        }
    }
}