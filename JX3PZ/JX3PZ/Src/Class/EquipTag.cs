using JX3CalculatorShared.Utils;
using System.Collections.Immutable;

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

            HasHaste = ExtraTag.Contains("加速"); // 是否有加速
            HasSpecial = ExtraTag.Contains("特效"); // 是否有特效
        }

        public static ImmutableArray<string> ParseStrList(string x)
        {
            if (x.IsEmptyOrWhiteSpace() || x == "[]")
            {
                return new ImmutableArray<string>();
            }
            else
            {
                var res = StringTool.ParseStringList(x, ' ').ToImmutableArray();
                return res;
            }
        }

        public bool HasHaste { get; } // 是否有加速
        public bool HasSpecial { get; } // 是否有特效
    }
}