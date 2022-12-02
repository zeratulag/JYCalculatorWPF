using JX3CalculatorShared.Class;
using JYCalculator.Src.Data;

namespace JYCalculator.Class
{
    public class JYCombatStatItem : CombatStatItem
    {
        #region 成员

        #endregion

        public JYCombatStatItem(string name, string skillName) : base(name, skillName)
        {
        }

        public JYCombatStatItem(SkillDamage data) : this(data.Name, data.SkillName)
        {
        }

        public JYCombatStatItem(SkillDamage data, double time) : this(data)
        {
            Num = data.Freq * time;

            CTNum = Num * data.CT;

            TotalDamage = data.FinalEDamage * Num;
            GetShowNum();
        }

        public JYCombatStatItem(JYCombatStatItem old) : this(old.Name, old.SkillName)
        {
            Num = old.Num;
            CTNum = old.CTNum;
            TotalDamage = old.TotalDamage;
            Proportion = old.Proportion;
            Rank = old.Rank;
            ShowNumMultiplier = old.ShowNumMultiplier;
            ShowNum = old.ShowNum;
        }

        public JYCombatStatItem Copy()
        {
            return new JYCombatStatItem(this);
        }


        // 合并同名


        public static JYCombatStatItem Sum(params JYCombatStatItem[] items)
        {
            var res = new JYCombatStatItem(items[0].Name, items[0].SkillName)
            {
                ShowNumMultiplier = items[0].ShowNumMultiplier
            };
            foreach (var _ in items)
            {
                res.Num += _.Num;
                res.CTNum += _.CTNum;
                res.TotalDamage += _.TotalDamage;
            }

            return res;
        }


        /// <summary>
        /// 获得化简形式的战斗统计（合并同名技能）
        /// </summary>
        /// <returns></returns>
        public JYCombatStatItem ToSimple()
        {
            var combatName = StaticJYData.DB.SkillInfo.Name2CombatName[Name];
            var res = this.Copy();
            res.Name = combatName;
            return res;
        }
    }
}