using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using System.Collections.Immutable;
using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.Globals;
using System.Collections.Generic;

namespace JYCalculator.DB
{
    public class SkillEventTypeMaskDB : IDB<string, SkillEventTypeMask>
    {
        public readonly ImmutableDictionary<string, SkillEventTypeMask> Data;
        public Dictionary<(SkillEventTypeEnum, ulong, ulong), SkillEventTypeMask> DataByKey;

        public SkillEventTypeMask Get(string name)
        {
            return Data[name];
        }

        public SkillEventTypeMaskDB()
        {
            Data = SkillEventTypeMaskManager.Data.ToImmutableDictionary(e => e.Value.Name,
                e => e.Value);
            DataByKey = SkillEventTypeMaskManager.Data;
        }

        public SkillEventTypeMask GetByKey(SkillEventTypeEnum eventType, ulong eventMask1, ulong eventMask2)
        {
            return DataByKey[(eventType, eventMask1, eventMask2)];
        }
    }
}