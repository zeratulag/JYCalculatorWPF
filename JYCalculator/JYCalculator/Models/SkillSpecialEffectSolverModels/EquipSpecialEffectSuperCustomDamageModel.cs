using JX3CalculatorShared.Data;
using JYCalculator.Data;
using System.Collections.Generic;
using JYCalculator.Class;

namespace JYCalculator.Models.SkillSpecialEffectSolverModels
{
    public class EquipSpecialEffectSuperCustomDamageModel
    {
        // 专用计算特效附加伤害的类
        public EquipSpecialEffectEntry[] SuperCustomDamage { get; }
        public SkillNumModel SkillNum { get; }
        public List<(SkillInfoItem Skill, double NormalInterval, double XWInterval)> Result { get; private set; } // 直接伤害结果

        public EquipSpecialEffectSuperCustomDamageModel(EquipSpecialEffectConfigArg equipSpecialEffectConfig,
            SkillNumModel skillNum)
        {
            SuperCustomDamage = equipSpecialEffectConfig.SuperCustomDamage;
            SkillNum = skillNum;
        }

        public void CalcSuperCustomDamage()
        {
            Result =
                new List<(SkillInfoItem Skill, double NormalInterval, double XWInterval)>(SuperCustomDamage.Length);
            foreach (var e in SuperCustomDamage)
            {
                var skill = e.GetSuperCustomDamageSkill();
                var normalInterval = SkillNum.Normal.BasicSkillFreq.CalcMeanTriggerInterval(e.SkillEvent);
                var XWInterval = SkillNum.XW.BasicSkillFreq.CalcMeanTriggerInterval(e.SkillEvent);
                Result.Add((skill, normalInterval, XWInterval));
            }
        }

        public void Calc()
        {
            CalcSuperCustomDamage();
        }

    }
}