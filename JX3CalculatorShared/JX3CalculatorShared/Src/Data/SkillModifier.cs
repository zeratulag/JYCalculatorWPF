using JX3CalculatorShared.Src.Data;
using JX3CalculatorShared.Utils;
using System.Collections.Immutable;
using JX3CalculatorShared.Common;

namespace JX3CalculatorShared.Class
{
    public class SkillModifier : AbsGeneralItem, ILuaTable
    {
        public static TabParser Parser = new TabParser("SAt_key{0:D}", "SAt_value{0:D}", AtLoader.SkillAt_is_Value);

        public string RawSkillNames { get; set; }
        public string Type { get; set; }
        public string DescName { get; set; }
        public string Associate { get; set; }
        public string SAt_key1 { get; set; }
        public double SAt_value1 { get; set; }
        public string SAt_key2 { get; set; }
        public double SAt_value2 { get; set; }
        public string SAt_key3 { get; set; }
        public double SAt_value3 { get; set; }

        public ImmutableHashSet<string> SkillNames { get; private set; } // 可以被此Mod修饰的技能Name
        public ImmutableDictionary<string, double> SAttrs { get; private set; } // 数值属性


        public AttrCollection ParseItem()
        {
            return Parser.ParseItem(this);
        }

        public void ParseNames()
        {
            var res = StringTool.ParseStringList(RawSkillNames);
            SkillNames = res.ToImmutableHashSet();
        }

        public void PasteValueDictionary()
        {
            SAttrs = Parser.ParseValueDictItem(this).ToImmutableDictionary();
        }

        public void Parse()
        {
            ParseNames();
            PasteValueDictionary();
        }
    }
}