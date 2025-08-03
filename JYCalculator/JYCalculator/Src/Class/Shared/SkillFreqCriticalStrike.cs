using System;

namespace JYCalculator.Class
{
    public class SkillFreqCriticalStrike
    {
        /// <summary>
        /// 描述技能频率和会心率的类，用于计算会心触发事件
        /// </summary>

        #region 成员

        public string Name { get; }

        public double AddCriticalStrikeRate { get; }

        public double CriticalStrikeValue { get; set; }

        public double Freq { get; set; }

        #endregion

        #region 构建

        public SkillFreqCriticalStrike(SkillFreqCriticalStrike old)
        {
            Name = old.Name;
            AddCriticalStrikeRate = old.AddCriticalStrikeRate;
            CriticalStrikeValue = old.CriticalStrikeValue;
            Freq = old.Freq;
        }


        public SkillFreqCriticalStrike(SkillData skill, FullCharacter fchar, double freq)
        {
            Name = skill.Name;
            AddCriticalStrikeRate = skill.AddCriticalStrikeRate;
            Freq = freq;

            CriticalStrikeValue = Math.Min(fchar.PhysicsCriticalStrikeValue + AddCriticalStrikeRate, 1);
            UpdateFChar(fchar);
        }

        public SkillFreqCriticalStrike Copy()
        {
            return new SkillFreqCriticalStrike(this);
        }


        // 保持频率和会心提高不变，仅仅更新面板
        public void UpdateFChar(FullCharacter fchar)
        {
            CriticalStrikeValue = Math.Min(fchar.PhysicsCriticalStrikeValue + AddCriticalStrikeRate, 1.0);
        }
        #endregion


    }

}