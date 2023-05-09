using System.Collections.Immutable;
using System.Linq;
using static JX3CalculatorShared.Utils.ImportTool;

namespace JX3PZ.Data
{
    public static class AttributeTabLib
    {
        public static ImmutableDictionary<int, AttributeTabItem> Data;

        public static string Path;

        public static void Load(string path)
        {
            Path = path;
            LoadAttributeEntry();
        }

        public static void LoadAttributeEntry()
        {
            Data = ReadSheetAsDict<int, AttributeTabItem>(Path, "attrib", _ => _.ID);
        }

        public static void Parse()
        {
            foreach (var _ in Data.Values)
            {
                _.Parse();
            }
        }

        public static AttributeTabItem Get(int id)
        {
            return Data[id];
        }

        public static ImmutableArray<AttributeTabItem> Gets(ImmutableArray<int> ids)
        {
            if (ids != null)
            {
                var res = from _ in ids select Get(_);
                return res.ToImmutableArray();
            }
            else
            {
                return new ImmutableArray<AttributeTabItem>();
            }
        }
    }
}