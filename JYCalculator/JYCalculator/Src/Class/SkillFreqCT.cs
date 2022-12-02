using System;
using JYCalculator.Src;
using JYCalculator.Src.Class;

namespace JYCalculator.Class
{
    public class SkillFreqCT
    {
        /// <summary>
        /// 描述技能频率和会心率的类，用于计算会心触发事件
        /// </summary>

        #region 成员

        public string Name { get; }

        public double AddCT { get; }

        public double CT { get; set; }

        public double Freq { get; set; }

        #endregion

        #region 构建

        public SkillFreqCT(SkillData skill, FullCharacter fchar, double freq)
        {
            Name = skill.Name;
            AddCT = skill.AddCT;
            Freq = freq;

            CT = Math.Min(fchar.CT + AddCT, 1);
            UpdateFChar(fchar);
        }


        // 保持频率和会心提高不变，仅仅更新面板
        public void UpdateFChar(FullCharacter fchar)
        {
            CT = Math.Min(fchar.CT + AddCT, 1.0);
        }
        #endregion


    }

}