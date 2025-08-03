using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JX3PZ.Data
{
    public static class DiamondTabLib
    {
        public static readonly Dictionary<int, DiamondTabItem> Data;

        static DiamondTabLib()
        {
            Data = new Dictionary<int, DiamondTabItem>(10);
        }

        public static DiamondTabItem Get(int id)
        {
            DiamondTabItem res;
            if (Data.TryGetValue(id, out var value))
            {
                res = value;
            }
            else
            {
                res = new DiamondTabItem(id);
                Data.Add(id, res);
            }
            return res;
        }

        public static ImmutableArray<DiamondTabItem> Gets(ImmutableArray<int> ids)
        {
            if (ids != null)
            {
                var res = from _ in ids select Get(_);
                return res.ToImmutableArray();
            }
            else
            {
                return new ImmutableArray<DiamondTabItem>();
            }
        }
    }
}