using System;

namespace JX3CalculatorShared.Class
{
    public class AppMetaInfo
    {
        public Version Ver;
        public string XinFa;
        public string GameVersion;
        public int CurrentLevel;
        public DateTime LastPatchTime;

        public AppMetaInfo(Version ver, string xinFa, string gameVersion, int currentLevel, DateTime lastPatchTime)
        {
            Ver = ver;
            XinFa = xinFa;
            GameVersion = gameVersion;
            CurrentLevel = currentLevel;
            LastPatchTime = lastPatchTime;
        }

        public AppMetaInfo()
        {
        }


    }
}