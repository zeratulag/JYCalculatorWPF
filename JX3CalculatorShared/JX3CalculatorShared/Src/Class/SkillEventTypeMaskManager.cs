using System.Collections.Generic;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;

namespace JX3CalculatorShared.Src.Class
{
    public static class SkillEventTypeMaskManager
    {
        public static readonly Dictionary<(SkillEventTypeEnum, ulong, ulong), SkillEventTypeMask> Data;

        static SkillEventTypeMaskManager()
        {
            Data = new Dictionary<(SkillEventTypeEnum, ulong, ulong), SkillEventTypeMask>(20);
        }

        private static SkillEventTypeMask MakeEntry(SkillEventTypeEnum eventType, ulong eventMask1, ulong eventMask2)
        {
            var res = new SkillEventTypeMask(eventType, eventMask1, eventMask2);
            Data.Add((eventType, eventMask1, eventMask2), res);
            return res;
        }

        public static SkillEventTypeMask Create(SkillEventTypeEnum eventType, ulong eventMask1, ulong eventMask2)
        {
            if (Data.TryGetValue((eventType, eventMask1, eventMask2), out var res))
            {
                return res;
            }

            var newRes = MakeEntry(eventType, eventMask1, eventMask2);
            return newRes;
        }
    }
}