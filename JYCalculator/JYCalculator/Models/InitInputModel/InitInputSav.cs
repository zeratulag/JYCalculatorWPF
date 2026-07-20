using JX3CalculatorShared.ViewModels;
using JYCalculator.Class;
using System.Collections.Generic;

namespace JYCalculator.Models
{
    // 序列化存储
    public class InitInputSav
    {
        public readonly InitCharacter InitChar;
        public readonly EquipOptionConfigSav EquipOption;
        public readonly BigFMSlotConfig[] BigFM;
        public readonly IDictionary<string, string> EquipSpecialEffect;

        public InitInputSav(InitCharacter initChar, EquipOptionConfigSav equipOption, BigFMSlotConfig[] bigFM,
            IDictionary<string, string> equipSpecialEffect = null)
        {
            InitChar = initChar;
            EquipOption = equipOption;
            BigFM = bigFM;
            EquipSpecialEffect = equipSpecialEffect;
        }
    }
}
