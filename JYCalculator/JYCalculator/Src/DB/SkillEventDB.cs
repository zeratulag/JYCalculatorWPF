using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using System.Collections.Immutable;
using JYCalculator.Data;

namespace JYCalculator.DB
{
    public class SkillEventDB : IDB<string, SkillEventItem>
    {
        public readonly ImmutableDictionary<string, SkillEventItem> Data;
        public readonly ImmutableDictionary<int, SkillEventItem> DictByID;

        public SkillEventItem Get(string name)
        {
            return Data[name];
        }

        public SkillEventItem GetEventByID(int ID)
        {
            SkillEventItem res = null;
            DictByID.TryGetValue(ID, out res);
            return res;
        }

        public SkillEventDB(ImmutableDictionary<string, SkillEventItem> data)
        {
            Data = data;
            DictByID = Data.ToImmutableDictionary(e => e.Value.ID, e => e.Value);
        }

        public SkillEventDB(DataLoader dataLoader) : this(dataLoader.SkillEvent)
        {
        }

        public SkillEventDB() : this(StaticXFData.Data)
        {
        }
    }
}