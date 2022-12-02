﻿using System.Collections.Generic;
using System.Linq;

namespace JYCalculator.Class
{
    public class CombatStat
    {
        #region 成员
        
        public Dictionary<string, JYCombatStatItem> Data;
        public JYCombatStatItem[] Items;
        public double SumTotalDamage;
        #endregion

        #region 构造

        public CombatStat(SkillDamageFreqDF Df)
        {
            Data = new Dictionary<string, JYCombatStatItem>(Df.ValidSkillNames.Length);
            foreach (var name in Df.ValidSkillNames)
            {
                Data.Add(name, new JYCombatStatItem(Df.DamageDF.Data[name], Df.Time));
            }
        }

        public CombatStat(Dictionary<string, JYCombatStatItem> data)
        {
            Data = data;
        }

        public CombatStat(CombatStat old)
        {
            Data = old.Data.ToDictionary(_ => _.Key, _ => _.Value.Copy());
        }
        public CombatStat Copy()
        {
            return new CombatStat(this);
        }

        // 根据比重排序
        public void Proceed()
        {
            SortItems();
            SetRanksAndProportion();
        }

        // 按照比重排序
        public void SortItems()
        {
            var res = from _ in Data.Values orderby _.TotalDamage descending select _;
            Items = res.ToArray();
        }
        
        // 赋以Rank
        public void SetRanksAndProportion()
        {
            SumTotalDamage = 0;
            int i = 1;
            foreach (var _ in Items)
            {
                _.Rank = i;
                SumTotalDamage += _.TotalDamage;
                i ++ ;
            }

            foreach (var _ in Items)
            {
                _.Proportion = _.TotalDamage / SumTotalDamage;
            }
        }

        // 就地合并
        public void Merge(CombatStat other)
        {
            foreach (var _ in other.Data)
            {
                if (Data.ContainsKey(_.Key))
                {
                    Data[_.Key].Merge(other.Data[_.Key]);
                }
                else
                {
                    Data.Add(_.Key, _.Value);
                }
            }
        }

        // 合并多个时段的统计
        public static CombatStat Merge(params CombatStat[] stats)
        {
            var res = stats[0].Copy();
            for (int i = 1; i < stats.Length; i++)
            {
                res.Merge(stats[i]);
            }
            return res;
        }


        public CombatStat ToSimple()
        {
            var data = new Dictionary<string, JYCombatStatItem>(Data.Count);
            foreach (var _ in Data.Values)
            {
                var newItem = _.ToSimple();
                var newKey = newItem.Name;
                if (!data.ContainsKey(newKey))
                {
                    data.Add(newKey, newItem);
                }
                else
                {
                    data[newKey].Merge(newItem);
                }
            }

            var res = new CombatStat(data);
            res.Proceed();
            return res;
        }


        #endregion

        /// <summary>
        /// 设定特定技能的显示技能数倍率
        /// </summary>
        /// <param name="skillName">技能Name</param>
        /// <param name="value">值</param>
        protected void SetSkillShowNumMultiplier(string skillName, double value)
        {
            if (Data.ContainsKey(skillName))
            {
                Data[skillName].SetShowNumMultiplier(value);
            }
        }


        /// <summary>
        /// 修复DOT因为叠层导致的跳数统计错误
        /// </summary>
        /// <param name="dotName">DOT名称</param>
        /// <param name="hitNum">战斗统计次数</param>
        /// <param name="dotNum">实际伤害次数</param>
        public void FixDOTShowNum(string dotName, double hitNum, double dotNum)
        {
            if (dotNum > 0)
            {
                SetSkillShowNumMultiplier(dotName, hitNum / dotNum);
            }
            
        }

    }
}