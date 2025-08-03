using JX3CalculatorShared.Utils;
using JX3PZ.ViewModels;
using System.Collections.Immutable;
using System;

namespace JX3PZ.Class
{
    public class PzSubSet
    {
        public string SubSetID { get; set; }
        public int SetID { get; set; }
        public int SubIndex { get; set; }
        public string SubSetName { get; set; }
        public int Num { get; set; }
        public string EID_Str { get; set; }
        public string EquipName_Str { get; set; }

        public ImmutableArray<string> EIDs { get; private set; }
        public ImmutableArray<string> EquipNames { get; private set; }

        public void Parse()
        {
            if (EID_Str == null)
            {
                EIDs = ImmutableArray<string>.Empty;
            }
            else
            {
                EIDs = StringTool.ParseStringList(EID_Str).ToImmutableArray();
                EquipNames = StringTool.ParseStringList(EquipName_Str).ToImmutableArray();
            }
        }
    }
}