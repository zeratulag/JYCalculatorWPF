namespace JX3CalculatorShared.ViewModels
{
    public class EquipOptionConfigSav
    {
        public bool JN { get; set; }
        public bool SL { get; set; }
        public string WPName { get; set; } = "";
        public string YZName { get; set; } = "";

        public EquipOptionConfigSav()
        {
        }

        public EquipOptionConfigSav(bool jn, bool sl, string wpName, string yzName)
        {
            JN = jn;
            SL = sl;
            WPName = wpName;
            YZName = yzName;
        }
    }
}