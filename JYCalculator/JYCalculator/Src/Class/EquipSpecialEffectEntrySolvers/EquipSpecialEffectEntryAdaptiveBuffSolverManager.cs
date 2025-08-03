using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JYCalculator.Src.Class;

namespace JYCalculator.Class
{
    public static class EquipSpecialEffectEntryAdaptiveBuffSolverManager
    {
        public static readonly
            ImmutableDictionary<EquipSpecialEffectTypeEnum, Func<EquipSpecialEffectEntry, FullCharacter, BuffRecord>>
            FunDict; // 简单的直接BUFF处理
        public static readonly ImmutableHashSet<EquipSpecialEffectTypeEnum> SnapAdaptiveEffectSet; // 是否为快照型的适应之力

        static EquipSpecialEffectEntryAdaptiveBuffSolverManager()
        {
            
            FunDict =
                new Dictionary<EquipSpecialEffectTypeEnum, Func<EquipSpecialEffectEntry, FullCharacter, BuffRecord>>()
                {
                    {EquipSpecialEffectTypeEnum.BOTTOMS__AdaptivePower, BOTTOMS__AdaptivePower},
                    {EquipSpecialEffectTypeEnum.HAT__AdaptivePower, HAT__AdaptivePower},
                    {EquipSpecialEffectTypeEnum.NECKLACE__CriticalStrike_To_CriticalPower, NECKLACE__CriticalStrike_To_CriticalPower},
                    {EquipSpecialEffectTypeEnum.NECKLACE__Overcome_To_AttackPower, NECKLACE__Overcome_To_AttackPower},
                }.ToImmutableDictionary();

            SnapAdaptiveEffectSet = new HashSet<EquipSpecialEffectTypeEnum>()
            {
                EquipSpecialEffectTypeEnum.HAT__AdaptivePower,
                EquipSpecialEffectTypeEnum.NECKLACE__CriticalStrike_To_CriticalPower,
                EquipSpecialEffectTypeEnum.NECKLACE__Overcome_To_AttackPower,
            }.ToImmutableHashSet();
        }

        public static bool IsSnapAdaptiveBuffEffect(EquipSpecialEffectEntry entry)
        {
            // 判断一个适应之力是否为快照型的
            return SnapAdaptiveEffectSet.Contains(entry.SpecialEffectType);
        }

        public static EquipSpecialEffectEntry[] SortAdaptiveBuffEntriesByCalcOrder(
            this IEnumerable<EquipSpecialEffectEntry> items)
        {
            // 根据结算顺序对item进行排序
            var res = items
                .OrderByDescending(x => x.IsSnapAdaptiveBuff) // True排在前面
                .ThenBy(x => (int)x.SpecialEffectType) // 枚举值小的在前面
                .ToArray();
            return res;
        }

        public static BuffRecord GetAdaptiveBuffRecord(this EquipSpecialEffectEntry entry, FullCharacter fChar)
        {
            entry.CheckSpecialEffectBaseType(EquipSpecialEffectBaseTypeEnum.AdaptiveBuff);

            if (FunDict.TryGetValue(entry.SpecialEffectType, out var func))
            {
                return func(entry, fChar);
            }

            // 建议用异常提示未注册的类型
            throw new NotSupportedException($"未知的装备特效类型: {entry.SpecialEffectType}，请检查是否已注册对应处理函数。");
        }


        public static BuffRecord MakeBuffRecord(int buffID, int level)
        {
            return new BuffRecord(buffID, level, 1.0, 1.0);
        }

        public static BuffRecord BOTTOMS__AdaptivePower_Level1(FullCharacter fChar)
        {
            // 精简下装适应之力_高级版 40794_1
            const double nStrainRate_YZ = 0.9;
            var currentStrain = fChar.FinalStrainValue; // 最终无双值
            const int buffID = 30749;
            int buffLevel = 0;
            if (currentStrain < nStrainRate_YZ)
            {
                buffLevel = 1; // 无双低于阈值加无双
            }
            else
            {
                buffLevel = 2; // 无双高于阈值加攻击
            }

            var res = MakeBuffRecord(buffID, buffLevel);
            return res;
        }

        public static BuffRecord BOTTOMS__AdaptivePower_Level2(FullCharacter fChar)
        {
            // 精简下装适应之力会心_高级版 40794_2
            const double nStrainRate_YZ = 0.9;
            var currentStrain = fChar.FinalStrainValue; // 最终无双值
            const int buffID = 30770;
            int buffLevel = 0;
            if (currentStrain < nStrainRate_YZ)
            {
                buffLevel = 1; // 无双低于阈值加无双
            }
            else
            {
                buffLevel = 2; // 无双高于阈值加会心
            }

            var res = MakeBuffRecord(buffID, buffLevel);
            return res;
        }

        public static BuffRecord BOTTOMS__AdaptivePower(this EquipSpecialEffectEntry entry, FullCharacter fChar)
        {
            switch (entry.SkillLevel)
            {
                case 1:
                {
                    return BOTTOMS__AdaptivePower_Level1(fChar);
                    break;
                }
                case 2:
                {
                    return BOTTOMS__AdaptivePower_Level2(fChar);
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException("Invalid SkillLevel");
                }
            }
        }

        public static BuffRecord HAT__AdaptivePower(this EquipSpecialEffectEntry entry, FullCharacter SnapfChar)
        {
            //黄字帽子适应之力
            var pos = GetMaxAttributePosition(SnapfChar);
            const int buffID = 29519;
            var buffLevelArr = new int[] {3 * entry.SkillLevel - 2, 3 * entry.SkillLevel - 1, 3 * entry.SkillLevel};
            var buffLevel = buffLevelArr[pos];
            var res = MakeBuffRecord(buffID, buffLevel);
            return res;
        }

        /// <summary>
        /// 计算基础破防，会心等级，破招值，并且判断哪个最高
        /// </summary>
        /// <param name="fChar">输入的面板</param>
        /// <returns></returns>
        public static int GetMaxAttributePosition(FullCharacter fChar)
        {
            int pf = (int) fChar.PhysicsBaseOvercome; // 基础破防等级
            int hx = (int) fChar.PhysicsCriticalStrike; // 基础会心等级
            int pz = (int) fChar.BaseSurplus; // 基础破招值
            var attrArr = new int[] {pf, hx, pz};
            int maxValue = attrArr.Max();
            int pos = Array.IndexOf(attrArr, maxValue);
            return pos;
        }

        public static ImmutableArray<int> NECKLACE__CriticalStrike_To_CriticalPower_LevelToStackDenominator
            = ImmutableArray.Create(0, 4748, 4942, 5302, 5683);
        // 项链会心加会效计算BUFF层数专用

        public static BuffRecord NECKLACE__CriticalStrike_To_CriticalPower(this EquipSpecialEffectEntry entry, FullCharacter SnapfChar)
        {
            // 黄字项链会心加会效
            int hx = (int) SnapfChar.PhysicsCriticalStrike; // 基础会心等级
            int lv = entry.SkillLevel;
            int denominator = NECKLACE__CriticalStrike_To_CriticalPower_LevelToStackDenominator[entry.SkillLevel];
            int nStack = (int) hx / denominator;
            const int BuffID = 29528;
            var res = new BuffRecord(BuffID, lv, 1.0, nStack);
            return res;
        }

        public static ImmutableArray<int> NECKLACE__Overcome_To_AttackPower_LevelToStackDenominator =
            ImmutableArray.Create(0, 5427, 5648, 6060, 6496);
        // 项链破防加攻击计算BUFF层数专用
        public static BuffRecord NECKLACE__Overcome_To_AttackPower(this EquipSpecialEffectEntry entry, FullCharacter SnapfChar)
        {
            // 黄字项链破防加攻击
            int pf = (int)SnapfChar.PhysicsBaseOvercome; // 基础破防等级
            int lv = entry.SkillLevel;
            int denominator = NECKLACE__Overcome_To_AttackPower_LevelToStackDenominator[entry.SkillLevel];
            int nStack = (int) pf / denominator;
            const int BuffID = 29529;
            var res = new BuffRecord(BuffID, lv, 1.0, nStack);
            return res;
        }

    }
}