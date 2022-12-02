using System;

namespace JX3CalculatorShared.Class
{
    public class AppMetaInfo
    {
        public Version Ver;
        public string XinFa;
        public string GameVersion;
        public int CurrentLevel;


        public AppMetaInfo(Version ver, string xinFa, string gameVersion, int currentLevel)
        {
            Ver = ver;
            XinFa = xinFa;
            GameVersion = gameVersion;
            CurrentLevel = currentLevel;
        }
        public AppMetaInfo()
        {
        }


    }
}