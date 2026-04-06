using System.Collections.Generic;
using JX3CalculatorShared.Common;
using System.Collections.Immutable;

namespace JX3PZ.Data
{
    public class DiamondDatabase : IDB<long, DiamondItemEntry>
    {
        public ImmutableDictionary<long, DiamondItemEntry> Data;

        public ImmutableDictionary<int, DiamondItemEntry> EnchantIDToEntry;

        public ImmutableDictionary<int, int> EnchantIDToLevel;

        public DiamondDatabase(IEnumerable<DiamondItemEntry> items)
        {
            Data = items.ToImmutableDictionary(e => e.UiID, e => e);
            EnchantIDToEntry = items.ToImmutableDictionary(e => e.EnchantID, e => e);
            EnchantIDToLevel = items.ToImmutableDictionary(e => e.EnchantID, e => e.Level);
        }

        public DiamondItemEntry Get(long uiid)
        {
            return Data[uiid];
        }

        public DiamondItemEntry GetDiamondInfoFromEnchantID(int enchantID)
        {
            return EnchantIDToEntry[enchantID];
        }

        public int GetDiamondLevelFromEnchantID(int enchantID)
        {
            return EnchantIDToLevel.GetValueOrDefault(enchantID);
        }
    }
}