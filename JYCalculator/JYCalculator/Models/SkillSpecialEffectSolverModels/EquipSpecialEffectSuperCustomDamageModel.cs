using JX3CalculatorShared.Data;
using JYCalculator.Class;
using JYCalculator.Data;
using SqlSugar;
using Syncfusion.Data.Extensions;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JYCalculator.Models.SkillSpecialEffectSolverModels
{
    public class EquipSpecialEffectSuperCustomDamageModel
    {
        // 专用计算特效附加伤害的类
        public EquipSpecialEffectEntry[] SuperCustomDamage { get; }
        public EquipSpecialEffectEntry[] DOTSuperCustomDamage { get; }

        public SkillNumModel SkillNum { get; }

        public List<(SkillInfoItem Skill, double NormalInterval, double XWInterval)>
            Result { get; private set; } // 直接伤害结果

        public EquipSpecialEffectSuperCustomDamageModel(EquipSpecialEffectConfigArg equipSpecialEffectConfig,
            SkillNumModel skillNum)
        {
            SuperCustomDamage = equipSpecialEffectConfig.SuperCustomDamage;
            DOTSuperCustomDamage = equipSpecialEffectConfig.DOTSuperCustomDamage;
            SkillNum = skillNum;
            Result =
                new List<(SkillInfoItem Skill, double NormalInterval, double XWInterval)>(10);
        }

        public void CalcSuperCustomDamage()
        {
            //var exitsSkills = new List<SkillInfoItem>(4);
            foreach (var e in SuperCustomDamage)
            {
                var skill = e.GetSuperCustomDamageSkill();
                var normalInterval = SkillNum.Normal.BasicSkillFreq.CalcMeanTriggerInterval(e.SkillEvent);
                var XWInterval = SkillNum.XW.BasicSkillFreq.CalcMeanTriggerInterval(e.SkillEvent);
                //if (exitsSkills.Contains(skill)) continue;
                Result.Add((skill, normalInterval, XWInterval));
                //exitsSkills.Add(skill);
            }
        }

        // 特效不叠加，取最高品级对应的特效
        public EquipSpecialEffectEntry[] UniqueDOTSuperCustomDamage()
        {
            if (DOTSuperCustomDamage == null || DOTSuperCustomDamage.Length == 0)
            {
                return Array.Empty<EquipSpecialEffectEntry>();
            }

            var maxEntry = DOTSuperCustomDamage
                .OrderByDescending(e => e.EquipLevel)
                .First();

            return new[] { maxEntry };
        }


        // 精简护手腰带加速DOT
        public void CalcDOTSuperCustomDamage()
        {
            var exitSkills = new List<SkillInfoItem>();
            foreach (var e in UniqueDOTSuperCustomDamage())
            {
                var skill = e.GetSuperCustomDamageSkill();
                if (exitSkills.Contains(skill)) continue;
                var nCount = CalcDOTSuperCustomDamageCount(SkillNum.SkillHaste.HSP);
                var normalInterval = SkillNum.Normal.BasicSkillFreq.CalcMeanTriggerInterval(e.SkillEvent);
                var XWInterval = SkillNum.XW.BasicSkillFreq.CalcMeanTriggerInterval(e.SkillEvent);
                Result.Add((skill, normalInterval / nCount, XWInterval / nCount));
                exitSkills.Add(skill);
            }
        }

        public static int CalcDOTSuperCustomDamageCount(int nHasteBase)
        {
            int result = 12;
            int realNHasteBase = (int) Math.Floor((double) nHasteBase);

            if (realNHasteBase != 0 && realNHasteBase <= 9232)
            {
                result = (int) Math.Floor(12 + 3.249566724436742e-4 * realNHasteBase);
            }
            else if (realNHasteBase <= 19285)
            {
                result = (int) Math.Floor(15 + 1.9894558838157763e-4 * (realNHasteBase - 9232));
            }
            else if (realNHasteBase <= 30158)
            {
                result = (int) Math.Floor(17 + 2.759128115515497e-4 * (realNHasteBase - 19285));
            }
            else if (realNHasteBase <= 42057)
            {
                result = (int) Math.Floor(20 + 1.6808135137406504e-4 * (realNHasteBase - 30158));
            }
            else
            {
                result = 22;
            }

            return result;
        }


        public void Calc()
        {
            CalcSuperCustomDamage();
            CalcDOTSuperCustomDamage();
        }
    }
}