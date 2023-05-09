using System.Collections.Generic;

namespace JX3PZ.Models
{
    public interface IPanelAttributeSlot
    {
        void UpdateFrom(IDictionary<string, int> valueDict);
    }
}