using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JYCalculator.Data;
using Syncfusion.Data.Extensions;

namespace JYCalculator.DB
{
    public class EquipSpecialEffectEntryDB : IDB<string, EquipSpecialEffectEntry>
    {
        public readonly ImmutableDictionary<string, EquipSpecialEffectEntry> Data;

        public EquipSpecialEffectEntry Get(string name)
        {
            EquipSpecialEffectEntry res = null;
            if (name != null)
            {
                Data.TryGetValue(name, out res);
            }
            return res;
        }

        public EquipSpecialEffectEntryDB(IEnumerable<EquipSpecialEffectEntry> items)
        {
            Data = items.ToImmutableDictionary(e => e.Name, e => e);
        }

        public EquipSpecialEffectEntryDB(DataLoader dataLoader) : this(dataLoader.EquipSpecialEffectEntries)
        {
        }

        public EquipSpecialEffectEntryDB() : this(StaticXFData.Data.EquipSpecialEffectEntries)
        {
        }

        public void AttachSkillEvent(SkillEventDB db)
        {
            foreach (var e in Data.Values)
            {
                var skillEventItem = db.GetEventByID(e.SkillEventID);
                e.AttachSkillEvent(skillEventItem);
            }
        }
    }
}