using System.Linq;
using static JX3CalculatorShared.Globals.AppStatic;

namespace JX3CalculatorShared.Utils

{
    public static class BindingTool
    {
        public static string IconID2Path(int iconID)
        {
            string result = $"{RESOURCE_ICON_URI}{iconID}.png";
            return result;
        }

        public static int IconPath2ID(string iconPath)
        {
            string s = iconPath.RemoveSuffix(".png");
            string id = s.Split("/").Last();
            int i = int.Parse(id);
            return i;
        }
    }
}