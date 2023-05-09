namespace JX3CalculatorShared.Data
{
    public abstract class AbsAttrItem
    {
        public string FullID { get; set; }
        public string Type { get; set; }
        public string SID { get; set; } // SimpleID
        public string Comment { get; set; }
        public int Denominator { get; set; }

        public bool IsValue()
        {
            bool res = Type == "Value" || Type == "Empty";
            return res;
        }
    }

    public class SkillAttrItem : AbsAttrItem
    {
    }

    public class AttrItem : AbsAttrItem
    {
        public int Target { get; set; }
        public string LuaID { get; set; }
        public string ShortName { get; set; }
        public string GeneratedMagic { get; set; } = "";
        public string CategoryTitle { get; set; }
        public string Category { get; set; }
        public string SubType { get; set; }
        public string Description { get; set; }
        public string DescriptionName { get; set; }
        public string EquipTag { get; set; } = "";
        public int Strength { get; set; }
    }
}