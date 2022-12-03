using JYCalculator.Globals;

namespace JYCalculator.ViewModels
{
    public class AboutDialogViewModel: AboutDialogViewModelBase
    {


        public AboutDialogViewModel()
        {
            URLDict = JYAppStatic.URLDict;
            MainName = "惊羽计算器Pro v" + JYAppStatic.AppVersion.ToString() + " \"Nirvana\" Release";
            Description = "Desc";

            BuildDateTime = JYAppStatic.BuildDateTime.ToString("G");
            GameDLC = JYAppStatic.GameVersion;
            ThanksTo = "805，北林，青墨白宣，JX3BOX。";

            GitUrl = URLDict["Git"];
            JBUrl = URLDict["JB"];
            TMUrl = URLDict["TM"];

        }
    }
}