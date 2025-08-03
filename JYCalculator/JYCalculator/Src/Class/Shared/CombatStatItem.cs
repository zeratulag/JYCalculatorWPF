using JX3CalculatorShared.Class;
using JYCalculator.Data;
using System.Collections.Generic;
using System.Linq;

namespace JYCalculator.Class
{
    public class CombatStatItem : CombatStatItemBase
    {

        public string FightName { get; protected set; }

        public CombatStatItem(string name, string skillName) : base(name, skillName)
        {
        }

        public CombatStatItem(SkillDamage data) : this(data.Name, data.SkillName)
        {
            QueryFightName();
        }

        public CombatStatItem(SkillDamage data, double time) : this(data)
        {
            Num = data.Freq * time;

            CTNum = Num * data.CriticalStrikeValue;

            TotalDamage = data.FinalExpectDamage * Num;
            GetShowNum();
        }

        public CombatStatItem(CombatStatItem old) : this(old.Name, old.SkillName)
        {
            Num = old.Num;
            CTNum = old.CTNum;
            TotalDamage = old.TotalDamage;
            Proportion = old.Proportion;
            Rank = old.Rank;
            ShowNumMultiplier = old.ShowNumMultiplier;
            ShowNum = old.ShowNum;
            FightName = old.FightName;
        }

        public CombatStatItem Copy()
        {
            return new CombatStatItem(this);
        }

        public void QueryFightName()
        {
            FightName = StaticXFData.DB.AllSkillInfo.Name2CombatName[Name];
        }


        // 合并同名


        public static CombatStatItem Sum(params CombatStatItem[] items)
        {
            var res = new CombatStatItem(items[0].Name, items[0].SkillName)
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
        //public CombatStatItem ToSimple()
        //{
        //    var res = this.Copy();
        //    res.DisplayName = res.FightName;
        //    return res;
        //}

        // 对于一组数据，基于总伤害降序排序
    }
}