using JX3PZ.Class;

namespace JX3CalculatorShared.Class
{
    public class SimpleAttributeEntry
    {
        public string SID;
        public double Value;
        public SimpleAttributeEntry(string sID, double value)
        {
            SID = sID;
            Value = value;
        }

        public SimpleAttributeEntry(AttributeEntry a)
        {
            if (a.Attribute.IsValue && a.Attribute.SID.IsNotEmptyOrWhiteSpace())
            {
                SID = a.Attribute.SID;
                Value = a.Value / (double) a.Attribute.Denominator;
            }
            else
            {
                SID = "Empty";
                Value = 0.0;
            }
        }
    }
}