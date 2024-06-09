using JX3PZ.Class;
using JX3PZ.Globals;

namespace JX3PZ.ViewModels
{
    public class AttributeStrengthEntryViewModel : FormattedEntryViewModel
    {
        // 精炼词条属性的VM

        public AttributeStrengthEntryViewModel(string desc1 = "", string desc2 = "", string color = ColorConst.Default)
        {
            Text1 = desc1;
            Text2 = desc2;
            Color1 = color;
            Color3 = color;
        }

        public AttributeStrengthEntryViewModel(AttributeStrengthEntry a)
        {
            var d = a.GetDescs();
            if (d.Length == 1)
            {
                Text1 = d[0];
                Text2 = "";
                Text3 = "";
            }

            if (d.Length == 2)
            {
                Text1 = $"{d[0]}";
                Text2 = $"(+{d[1]})";
                Text3 = "";
            }

            string col;

            if (!a.Attribute.IsValue)
            {
                col = a.Attribute.GetSpecialColor();
            }
            else
            {
                col = a.GetColor();
            }

            Color1 = col;
            Color2 = ColorConst.Strength;
            Color3 = col;
        }
    }
}