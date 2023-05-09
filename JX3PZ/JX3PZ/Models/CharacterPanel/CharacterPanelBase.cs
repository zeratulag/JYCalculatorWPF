using System.Collections.Generic;

namespace JX3PZ.Models
{
    public class CharacterPanelBase<TKey, TSlot> where TSlot: IPanelAttributeSlot
    {
        public readonly Dictionary<TKey, TSlot> Dict;

        public CharacterPanelBase()
        {
            Dict = new Dictionary<TKey, TSlot>(6);
        }

        public void UpdateFrom(IDictionary<string, int> valueDict)
        {
            foreach (var kvp in Dict)
            {
                kvp.Value.UpdateFrom(valueDict);
            }
        }

        public TSlot this[TKey key] => Dict[key];
    }
}