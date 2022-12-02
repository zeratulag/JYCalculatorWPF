namespace JX3CalculatorShared.Src.Data
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
        public string ShortName { get; set; }
        public string GeneratedMagic { get; set; }
    }
}