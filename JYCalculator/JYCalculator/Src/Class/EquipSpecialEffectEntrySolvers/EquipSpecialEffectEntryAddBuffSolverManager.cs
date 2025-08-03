using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JYCalculator.Src.Class;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace JYCalculator.Class
{
    public static class EquipSpecialEffectEntryAddBuffSolverManager
    {
        public static readonly
            ImmutableDictionary<EquipSpecialEffectTypeEnum, Func<EquipSpecialEffectEntry, BuffRecord>>
            SimpleFunDict; // 简单的直接BUFF处理

        public static readonly
            ImmutableDictionary<EquipSpecialEffectTypeEnum, Func<EquipSpecialEffectEntry, double, BuffRecord>>
            CoverFunDict; // 复杂一点，带覆盖率的BUFF处理

        static EquipSpecialEffectEntryAddBuffSolverManager()
        {
            SimpleFunDict = new Dictionary<EquipSpecialEffectTypeEnum, Func<EquipSpecialEffectEntry, BuffRecord>>()
            {
                {EquipSpecialEffectTypeEnum.BELT__Attribute, BELT__Attribute},
                {EquipSpecialEffectTypeEnum.BOTTOMS__Strain, BOTTOMS__Strain},
                {EquipSpecialEffectTypeEnum.PENDANT__CriticalPower, PENDANT__CriticalPower},
                {EquipSpecialEffectTypeEnum.PENDANT__Overcome, PENDANT__Overcome},
                {EquipSpecialEffectTypeEnum.RING__Overcome, RING__Overcome},
                {EquipSpecialEffectTypeEnum.RING__CriticalStrike, RING__CriticalStrike},
                {EquipSpecialEffectTypeEnum.RING__Surplus, RING__Surplus}
            }.ToImmutableDictionary();
            ;

            CoverFunDict =
                new Dictionary<EquipSpecialEffectTypeEnum, Func<EquipSpecialEffectEntry, double, BuffRecord>>()
                {
                    {EquipSpecialEffectTypeEnum.SHOES__CriticalStrike, SHOES__CriticalStrike},
                    {EquipSpecialEffectTypeEnum.SHOES__Overcome, SHOES__Overcome},
                }.ToImmutableDictionary();
        }

        // 计算那些直接叠加的BUFF，无须覆盖率
        public static BuffRecord GetSimpleAddBuffRecord(this EquipSpecialEffectEntry entry)
        {
            CheckSpecialEffectBaseType(entry);
            if (entry.NeedCalcEventFreq)
            {
                // 不需要计算触发，此时覆盖率为1
                throw new ArgumentException($"该特效类型需要计算频率才能决定！{entry.Name}");
            }

            if (SimpleFunDict.TryGetValue(entry.SpecialEffectType, out var func))
            {
                return func(entry);
            }

            // 建议用异常提示未注册的类型
            throw new NotSupportedException($"未知的装备特效类型: {entry.SpecialEffectType}，请检查是否已注册对应处理函数。");
        }

        public static void CheckSpecialEffectBaseType(EquipSpecialEffectEntry entry)
        {
            entry.CheckSpecialEffectBaseType(EquipSpecialEffectBaseTypeEnum.AddBuff);
        }

        // 计算那些直接叠加的BUFF，带覆盖率
        public static BuffRecord GetCoverAddBuffRecord(this EquipSpecialEffectEntry entry, double cover)
        {
            CheckSpecialEffectBaseType(entry);
            if (!entry.NeedCalcEventFreq)
            {
                // 不需要计算触发，此时覆盖率为1
                throw new ArgumentException($"该特效类型不需要计算频率才能决定！{entry.Name}");
            }

            if (CoverFunDict.TryGetValue(entry.SpecialEffectType, out var func))
            {
                return func(entry, cover);
            }

            // 建议用异常提示未注册的类型
            throw new NotSupportedException($"未知的装备特效类型: {entry.SpecialEffectType}，请检查是否已注册对应处理函数。");
        }

        public static BuffRecord MakeDefaultBuffRecord(EquipSpecialEffectEntry entry, int level, double cover = 1.0)
        {
            var res = new BuffRecord(entry.BuffID1, level, cover, entry.BuffStack1);
            return res;
        }

        public static BuffRecord MakeDefaultBuffRecord(EquipSpecialEffectEntry entry, double cover = 1.0)
        {
            var res = new BuffRecord(entry.BuffID1, entry.SkillLevel, cover, entry.BuffStack1);
            return res;
        }

        #region 细分处理

        public static BuffRecord BELT__Attribute(EquipSpecialEffectEntry entry)
        {
            // 精简腰带提高属性_正常版
            var res = MakeDefaultBuffRecord(entry);
            return res;
        }

        public static BuffRecord BOTTOMS__Strain(EquipSpecialEffectEntry entry)
        {
            // 精简下装提高无双_正常版
            var res = MakeDefaultBuffRecord(entry);
            return res;
        }

        public static BuffRecord PENDANT__CriticalPower(EquipSpecialEffectEntry entry)
        {
            // 黄字腰坠加会效
            var res = MakeDefaultBuffRecord(entry);
            return res;
        }

        public static BuffRecord PENDANT__Overcome(EquipSpecialEffectEntry entry)
        {
            // 黄字腰坠加破防
            var res = MakeDefaultBuffRecord(entry);
            return res;
        }

        public static BuffRecord RING__Overcome(EquipSpecialEffectEntry entry)
        {
            // 精简戒指1加破防
            var res = MakeDefaultBuffRecord(entry, level: entry.SkillLevel * 2 - 1);
            return res;
        }

        public static BuffRecord RING__CriticalStrike(EquipSpecialEffectEntry entry)
        {
            // 精简戒指2加会心
            var res = MakeDefaultBuffRecord(entry, level: entry.SkillLevel * 2 - 1);
            return res;
        }

        public static BuffRecord RING__Surplus(EquipSpecialEffectEntry entry)
        {
            // 精简戒指3加破招
            var res = MakeDefaultBuffRecord(entry, level: entry.SkillLevel * 2 - 1);
            return res;
        }

        // 以下是需要处理覆盖率的特效类型

        public static BuffRecord SHOES__CriticalStrike(EquipSpecialEffectEntry entry, double cover)
        {
            // 黄字鞋子加会心
            var res = MakeDefaultBuffRecord(entry, cover: cover);
            return res;
        }

        public static BuffRecord SHOES__Overcome(EquipSpecialEffectEntry entry, double cover)
        {
            // 黄字鞋子加破防
            var res = MakeDefaultBuffRecord(entry, cover: cover);
            return res;
        }

        #endregion
    }
}