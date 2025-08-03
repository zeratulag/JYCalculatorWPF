using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JYCalculator.Data;
using Serilog;
using System;

namespace JYCalculator.Class
{
    public static class EquipSpecialEffectEntryDamageSolverManager
    {
        // 获取SuperCustomDamageEntry对应的伤害技能
        // 实现逻辑与 装备/黄字暗器dot.lua 同步
        public static SkillInfoItem GetSuperCustomDamageSkill(this EquipSpecialEffectEntry entry)
        {
            entry.CheckSpecialEffectBaseType(EquipSpecialEffectBaseTypeEnum.SuperCustomDamage);
            var skillID = entry.DamageSkillID;
            var level = entry.SkillLevel;
            var resList = StaticXFData.DB.AllSkillInfo.GetSkillByIDLevel(skillID, level);
            if (resList.Length != 1)
            {
                Log.Error("{EquipSpecialEffectEntry}不能确定唯一技能！", entry);
            }
            return resList[0];
        }
    }
}