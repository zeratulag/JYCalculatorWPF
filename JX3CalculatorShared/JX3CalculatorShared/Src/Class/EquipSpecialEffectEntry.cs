using JX3CalculatorShared.Globals;
using JYCalculator.Class;
using Serilog.Debugging;
using System;

namespace JX3CalculatorShared.Data
{
    public partial class EquipSpecialEffectEntry
    {
        public void AttachSkillEvent(SkillEventItem item)
        {
            SkillEvent = item;
        }

        public void Parse()
        {
            if (SpecialEffectBaseType != EquipSpecialEffectBaseTypeEnum.AdaptiveBuff)
            {
                IsSnapAdaptiveBuff = false;
                return;
            }
            IsSnapAdaptiveBuff = EquipSpecialEffectEntryAdaptiveBuffSolverManager.IsSnapAdaptiveBuffEffect(this);
        }

        public void CheckSpecialEffectBaseType(EquipSpecialEffectBaseTypeEnum tarTypeEnum)
        {
            if (SpecialEffectBaseType != tarTypeEnum)
            {
                throw new ArgumentException($"非法的特效类型！{Name}，期望为{tarTypeEnum}");
            }
        }

    }
}