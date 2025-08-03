using System;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using JYCalculator.Class;
using JYCalculator.Src.Class;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using JX3CalculatorShared.Class;
using JYCalculator.Data;

namespace JYCalculator.Models.SkillSpecialEffectSolverModels
{
    public class EquipSpecialEffectAdaptiveBuffModel
    {
        public EquipSpecialEffectEntry[] AdaptiveBuff { get; }
        public Period<FullCharacter> RealTimeCharacter { get; }
        public FullCharacter SnapCharacter { get; } // 进入战斗时的快照属性


        public EquipSpecialEffectEntry[] SnapAdaptiveBuff { get; } // 快照类效果
        public EquipSpecialEffectEntry[] RealTimeAdaptiveBuff { get; } // 实时类效果

        public BuffRecord[] SnapBuffRecords { get; private set; }

        public BaseBuff[] SnapBuffResults { get; private set; }
        public Period<List<BuffRecord>> RealTimeBuffRecords { get; private set; }

        public EquipSpecialEffectAdaptiveBuffModel(EquipSpecialEffectConfigArg equipSpecialEffectConfig, Period<FullCharacter> realTimeCharacter, FullCharacter snapCharacter)
        {
            AdaptiveBuff = equipSpecialEffectConfig.AdaptiveBuff.SortAdaptiveBuffEntriesByCalcOrder();
            RealTimeCharacter = realTimeCharacter;
            SnapCharacter = snapCharacter;

            SnapAdaptiveBuff = AdaptiveBuff.Where(e => e.IsSnapAdaptiveBuff).ToArray();
            RealTimeAdaptiveBuff = AdaptiveBuff.Where(e => !e.IsSnapAdaptiveBuff).ToArray();
        }

        public void Calc()
        {
            CalcSnapBuffRecords();
        }

        public void CalcSnapBuffRecords()
        {
            SnapBuffRecords = SnapAdaptiveBuff.Select(e => e.GetAdaptiveBuffRecord(SnapCharacter)).ToArray();
            SnapBuffResults = StaticXFData.DB.Buff.GetBaseBuffByRecords(SnapBuffRecords);
        }

        public void CalcSnap()
        {
            CalcSnapBuffRecords();
        }

        public void ApplySnapBuffResults()
        {
            RealTimeCharacter.Normal.AddBaseBuffs(SnapBuffResults);
            RealTimeCharacter.XinWu.AddBaseBuffs(SnapBuffResults);
        }

        public void CalcAndApplyRealTimeBuffs()
        {
            int n = RealTimeAdaptiveBuff.Length;
            RealTimeBuffRecords = new Period<List<BuffRecord>>(new List<BuffRecord>(n), new List<BuffRecord>(n));
            foreach (var e in RealTimeAdaptiveBuff)
            {
                var normalBuff = e.GetAdaptiveBuffRecord(RealTimeCharacter.Normal);
                RealTimeCharacter.Normal.AddBuffRecord(normalBuff);
                RealTimeBuffRecords.Normal.Add(normalBuff);

                var xinWuBuff = e.GetAdaptiveBuffRecord(RealTimeCharacter.XinWu);
                RealTimeCharacter.XinWu.AddBuffRecord(xinWuBuff);
                RealTimeBuffRecords.XinWu.Add(xinWuBuff);
            }
        }
    }
}