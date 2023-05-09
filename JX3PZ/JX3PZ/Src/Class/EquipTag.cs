using System.Collections.Immutable;
using JX3CalculatorShared.Utils;

namespace JX3PZ.Class
{
    public class EquipTag
    {
        public readonly ImmutableArray<string> BasicTag;
        public readonly ImmutableArray<string> ExtraTag;
        public readonly ImmutableArray<string> DiamondTag;

        public EquipTag(Equip eq)
        {
            BasicTag = ParseStrList(eq.BasicTag_Str);
            ExtraTag = ParseStrList(eq.ExtraTag_Str);
            DiamondTag = ParseStrList(eq.DiamondTag_Str);
        }

        public static ImmutableArray<string> ParseStrList(string x)
        {
            if (x.IsEmptyOrWhiteSpace() || x == "[]")
            {
                return new ImmutableArray<string>();
            }
            else
            {
                var res = StringTool.ParseStringList(x, " ").ToImmutableArray();
                return res;
            }
        }

        public bool HasHaste => ExtraTag.Contains("加速"); // 是否有加速
    }
}