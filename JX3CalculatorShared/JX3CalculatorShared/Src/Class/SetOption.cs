using JX3CalculatorShared.Src.Data;
using JX3CalculatorShared.Utils;
using System.Collections.Immutable;

namespace JX3CalculatorShared.Src.Class
{
    public class SetOption
    {
        public readonly int SetID;
        public readonly string SetName;
        public readonly string Effect2;
        public readonly string Effect4;
        public readonly ImmutableArray<string> EquipIDs;

        public SetOption(SetOptionItem item)
        {
            SetID = item.SetID;
            SetName = item.SetName;
            Effect2 = item.Effect2;
            Effect4 = item.Effect4;
            EquipIDs = StringTool.ParseStringList(item.RawEquipIDs).ToImmutableArray();
        }

    }
}