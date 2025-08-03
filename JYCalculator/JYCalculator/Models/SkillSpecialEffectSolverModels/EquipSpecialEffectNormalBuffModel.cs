using System;
using System.Collections.Generic;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JYCalculator.Class;
using JYCalculator.Data;
using JYCalculator.Src.Class;

namespace JYCalculator.Models.SkillSpecialEffectSolverModels
{
    
    public class EquipSpecialEffectNormalBuffModel
    {
        // 解决常规BUFF类装备特效（包括固定BUFF和随机BUFF，不包括自适应BUFF）
        public EquipSpecialEffectConfigArg EquipSpecialEffectConfig { get; }
        public SkillNumModel SkillNum { get; }

        public List<BuffRecord> AddBuffResults; // 直接加的Buff结果
        public Period<List<BuffRecord>> PeriodAddBuffResults; // 区分常规和心无状态下的直接加Buff结果（覆盖率不同）
        public Period<List<BuffRecord>> PeriodRandomBuffResults; // 区分常规和心无状态下的直接加Buff结果（覆盖率不同）

        public Period<List<BuffRecord>> FinalBuffRecordResults;

        public Period<BaseBuff[]> FinalBuffs; // 最终的Buff结果，包含了所有的Buff（包括覆盖率和非覆盖率的）

        public EquipSpecialEffectNormalBuffModel(EquipSpecialEffectConfigArg equipSpecialEffectConfig,
            SkillNumModel skillNum)
        {
            EquipSpecialEffectConfig = equipSpecialEffectConfig;
            SkillNum = skillNum;
        }

        // 仅计算，不修改属性和技能数
        public void Calc()
        {
            CalcAddBuff();
            CalcRandomBuff();
            SummaryBuffRecords();
            MakeFinalBuffs();
        }


        private void MakeFinalBuffs()
        {
            var normalBuff = StaticXFData.DB.Buff.GetBaseBuffByRecords(FinalBuffRecordResults.Normal);
            var xwBuff = StaticXFData.DB.Buff.GetBaseBuffByRecords(FinalBuffRecordResults.XinWu);
            FinalBuffs = new Period<BaseBuff[]>(normalBuff, xwBuff);
        }


        public void CalcAddBuff()
        {
            int n = EquipSpecialEffectConfig.AddBuff.Length;
            AddBuffResults = new List<BuffRecord>(n);
            PeriodAddBuffResults = new Period<List<BuffRecord>>(new List<BuffRecord>(n), new List<BuffRecord>(n));

            foreach (var e in EquipSpecialEffectConfig.AddBuff)
            {
                if (e.NeedCalcEventFreq)
                {
                    var cover = SkillNum.CalcCDBuffCoverRate(e.SkillEvent);
                    PeriodAddBuffResults.Normal.Add(e.GetCoverAddBuffRecord(cover.normalCover));
                    PeriodAddBuffResults.XinWu.Add(e.GetCoverAddBuffRecord(cover.xwCover));
                }
                else
                {
                    AddBuffResults.Add(e.GetSimpleAddBuffRecord());
                }
            }
        }

        private void CalcRandomBuff()
        {
            PeriodRandomBuffResults = new Period<List<BuffRecord>>(new List<BuffRecord>(), new List<BuffRecord>());
            foreach (var e in EquipSpecialEffectConfig.RandomBuff)
            {
                var cover = SkillNum.CalcCDBuffCoverRate(e.SkillEvent);
                PeriodRandomBuffResults.Normal.AddRange(e.GetRandomBuffRecord(cover.normalCover));
                PeriodRandomBuffResults.XinWu.AddRange(e.GetRandomBuffRecord(cover.xwCover));
            }
        }

        public void SummaryBuffRecords()
        {
            // 总结最终加了哪些BUFF
            FinalBuffRecordResults = new Period<List<BuffRecord>>(new List<BuffRecord>(20), new List<BuffRecord>(20));
            FinalBuffRecordResults.Normal.AddRange(AddBuffResults);
            FinalBuffRecordResults.XinWu.AddRange(AddBuffResults);

            FinalBuffRecordResults.Normal.AddRange(PeriodAddBuffResults.Normal);
            FinalBuffRecordResults.XinWu.AddRange(PeriodAddBuffResults.XinWu);

            FinalBuffRecordResults.Normal.AddRange(PeriodRandomBuffResults.Normal);
            FinalBuffRecordResults.XinWu.AddRange(PeriodRandomBuffResults.XinWu);
        }
    }
}