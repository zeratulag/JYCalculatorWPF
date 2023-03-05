using JX3CalculatorShared.ViewModels;
using JYCalculator.Globals;

namespace JYCalculator.ViewModels
{
    public class AboutDialogViewModel : AboutDialogViewModelBase
    {


        public AboutDialogViewModel()
        {
            URLDict = XFAppStatic.URLDict;
            MainName = "惊羽计算器Pro v" + XFAppStatic.AppVersion.ToString() + " \"Nirvana\" Release";
            Description = "Desc";

            BuildDateTime = XFAppStatic.BuildDateTime.ToString("G");
            LastPatchTime = XFAppStatic.LastPatchTime.ToString("d");
            LastPatchURL = XFAppStatic.LastPatchURL;
            GameDLC = XFAppStatic.GameVersion;
            ThanksTo = "805，北林，青墨白宣，JX3BOX。";

            GitUrl = URLDict["Git"];
            JBUrl = URLDict["JB"];
            TMUrl = URLDict["TM"];

        }
    }
}