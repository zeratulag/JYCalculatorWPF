using System.Collections.Generic;
using JX3CalculatorShared.Common;
using JYCalculator.Class;
using System.Collections.Immutable;
using System.Linq;

namespace JYCalculator.Data
{
    public class SkillRatioManager
    {
        public ImmutableArray<SkillRatioItem> SkillRatios { get; protected set; }
        public Period<BaseSkillRatioGroup> DefaultBaseSkillRatioGroups { get; private set; } // 默认紫武的
        public Period<BaseSkillRatioGroup> ChengWuBaseSkillRatioGroups { get; private set; } // 橙武的

        public ImmutableDictionary<(bool ChenWu, int Rank), Period<BaseSkillRatioGroup>> BaseSkillRatioGroups
        {
            get;
            private set;
        } // 所有的基础技能比例组，键为(是否为橙武, Rank)

        public SkillRatioManager(ImmutableArray<SkillRatioItem> skillRatios)
        {
            SkillRatios = skillRatios;
        }

        public void run()
        {
            GroupData();
        }

        public void GroupData()
        {
            BaseSkillRatioGroups = SkillRatios.GroupBy(e => (e.ChengWu, e.Rank))
                .ToImmutableDictionary(g => g.Key, BaseSkillRatioGroup.MakePeriodBaseSkillRatioGroup);
        }

        /// <summary>
        /// 基于橙武与手法获取技能组
        /// </summary>
        /// <param name="chenWu"></param>
        /// <param name="rank"></param>
        /// <returns></returns>
        public Period<BaseSkillRatioGroup> GetPeriodBaseSkillRatioGroup(bool chenWu, int rank)
        {
            var nearRank = rank;
            Period<BaseSkillRatioGroup> res = null;
            while (!BaseSkillRatioGroups.TryGetValue((chenWu, nearRank), out res))
            {
                nearRank += 1;
                if (nearRank > 10)
                {
                    break;
                }
            }

            if (res != null) return res;
            nearRank = rank;

            while (!BaseSkillRatioGroups.TryGetValue((chenWu, nearRank), out res))
            {
                nearRank -= 1;
                if (nearRank < 1)
                {
                    break;
                }
            }

            return res;
        }
    }
}