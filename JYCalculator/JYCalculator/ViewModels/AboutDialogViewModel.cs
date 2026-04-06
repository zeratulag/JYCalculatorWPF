using JX3CalculatorShared.ViewModels;
using JYCalculator.Globals;
using System.Globalization;

namespace JYCalculator.ViewModels
{
    public class AboutDialogViewModel : AboutDialogViewModelBase
    {


        public AboutDialogViewModel()
        {
            URLDict = XFAppStatic.URLDict;
            QQGroupNumber = XFAppStatic.QQGroupNumber;
            MainName = "惊羽诀配装计算器Pro v" + XFAppStatic.AppVersion.ToString() + " \"Nirvana\" Release";
            Description = "Desc";

            BuildDateTime = XFAppStatic.BuildDateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            LastPatchTime = XFAppStatic.LastPatchTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            LastPatchURL = XFAppStatic.LastPatchURL;
            GameDLC = XFAppStatic.GameVersion;
            ThanksTo = "805，北林，青墨白宣，小介介，JX3BOX。";

            GitUrl = URLDict["Git"];
            JBUrl = URLDict["JB"];
            TMUrl = URLDict["TM"];

        }
    }
}