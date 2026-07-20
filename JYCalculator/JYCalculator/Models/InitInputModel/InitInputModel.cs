using JYCalculator.Class;

namespace JYCalculator.Models
{
    public class InitInputModel
    {
        public JBBBPZPanel Panel { get; }
        public InitInputSav Result { get; private set; }
        public InitInputSav InputSav { get; private set; }

        public InitInputModel(JBBBPZPanel panel)
        {
            Panel = panel;
        }

        public InitInputSav Calc()
        {
            Panel.Plan.Parse();
            var initChar = new InitCharacter(Panel);
            Result = new InitInputSav(initChar, Panel.Plan.EquipOptionSav, Panel.Plan.BigFMConfig,
                Panel.Plan.EquipDict);
            InputSav = Result;
            Panel.InitInput = Result;
            return Result;
        }
    }
}
