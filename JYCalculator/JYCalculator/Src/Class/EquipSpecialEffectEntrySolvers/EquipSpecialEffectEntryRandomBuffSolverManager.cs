using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JYCalculator.Src.Class;
using Syncfusion.Linq;

namespace JYCalculator.Class
{
    public static class EquipSpecialEffectEntryRandomBuffSolverManager
    {
        public static readonly
            ImmutableDictionary<EquipSpecialEffectTypeEnum, Func<EquipSpecialEffectEntry, double, BuffRecord[]>>
            CoverFunDict; // 复杂一点，带覆盖率的BUFF处理


        static EquipSpecialEffectEntryRandomBuffSolverManager()
        {
            CoverFunDict =
                new Dictionary<EquipSpecialEffectTypeEnum, Func<EquipSpecialEffectEntry, double, BuffRecord[]>>()
                {
                    {EquipSpecialEffectTypeEnum.BELT__Random_Attribute, BELT__Random_Attribute},
                }.ToImmutableDictionary();
        }


        public static BuffRecord[] GetRandomBuffRecord(this EquipSpecialEffectEntry entry, double totalCover)
        {
            // 基于总覆盖率（所有随机BUFF的覆盖率之和）计算最终Buff组合
            if (entry.SpecialEffectBaseType != EquipSpecialEffectBaseTypeEnum.RandomBuff)
            {
                throw new ArgumentException($"非法的特效类型！{entry.Name}");
            }

            if (CoverFunDict.TryGetValue(entry.SpecialEffectType, out var func))
            {
                return func(entry, totalCover);
            }

            // 建议用异常提示未注册的类型
            throw new NotSupportedException($"未知的装备特效类型: {entry.SpecialEffectType}，请检查是否已注册对应处理函数。");
        }

        public static BuffRecord[] BELT__Random_Attribute(this EquipSpecialEffectEntry entry, double totalCover)
        {
            int ID = entry.BuffID1;
            int stack = entry.BuffStack1;
            int level1 = entry.SkillLevel * 3 - 2; // 加会心
            int level2 = entry.SkillLevel * 3 - 1; // 加破防
            int level3 = entry.SkillLevel * 3; // 加破招
            int[] levels = new[] {level1, level2, level3};
            int[] weights = new[] {33, 66 - 33, 100 - 66};
            int weightSum = System.Linq.Enumerable.Sum(weights);

            BuffRecord[] res = new BuffRecord[3];
            for (int i = 0; i < levels.Length; ++i)
            {
                double cover = totalCover * weights[i] / (double) weightSum;
                res[i] = new BuffRecord(ID, levels[i], cover, stack);
            }

            return res;
        }
    }
}