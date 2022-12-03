using JX3CalculatorShared.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace JX3CalculatorShared.Globals
{
    // 存储一些和心法无关的常数和常量
    public static class AppStatic
    // 存储一些计算器设置常量（发布版本）
    {
        public const string URI_PREFIX = "pack://application:,,,/";
        public const string DATA_FOLDER = "Resource/Data/";
        public const string RESOURCE_ICON_FOLDER = "Resource/Icons/";
        public const string RESOURCE_ICON_URI = URI_PREFIX + RESOURCE_ICON_FOLDER;
        public const string SinaWBURL = @"https://weibo.com/zeratulag/home"; // 个人主页
    }

    public static class StaticData
        // 存储一些游戏常量，此类的成员将会被using static 直接访问
    {
        public const int CurrentLevel = 120; // 当前人物等级
        public const double FPS_PER_SECOND = 16.0;
        public const double GCD = 1.5;
        public const int GCD_FPS = 24;
        public const int NumberOfQiXue = 12;
        public const double G_KILO = 1024.0; // 郭氏千
    }

    public static class Funcs
    {
        public static string MergeIDLevel(int iD, int level)
        {
            var res = $"{iD}_{level}";
            return res;
        }

        public static (int ID, int Level) SplitIDLevel(string rawID)
        {
            var idseq = rawID.Split('_');
            int ID = int.Parse(idseq[0]);
            int Level = int.Parse(idseq[1]);
            return (ID, Level);
        }
    }

    public static class Strings
    {
        public const string TooltipDivider0 = "---";
        public const string TooltipDivider = "\n" + TooltipDivider0 + "\n";
    }

    public static class BaseGlobalParams
    {
        // 宇宙常数类
        public const double CT = 9.530;
        public const double CF = 3.335;
        public const double HT = 6.931;
        public const double WS = 9.189;
        public const double Def = 5.091;
        public const double OC = 9.530;
        public const double HS = 11.695;
        public const double PZ = 13.192;

        public static readonly ImmutableDictionary<string, double> Dict;

        public static Dictionary<string, double> GetDict()
        {
            var res = new Dictionary<string, double>()
            {
                {nameof(CT), CT},
                {nameof(CF), CF},
                {nameof(HT), HT},
                {nameof(WS), WS},
                {nameof(Def), Def},
                {nameof(OC), OC},
                {nameof(HS), HS},
                {nameof(PZ), PZ}
            };
            return res;
        }

        static BaseGlobalParams()
        {
            Dict = GetDict().ToImmutableDictionary();
        }
    }

    public static class GlobalData
    {
        public static readonly ImmutableDictionary<int, int> BossBaseDefs = new Dictionary<int, int>()
        {
            {121, 11073}, {122, 15528}, {123, 26317}, {124, 27550}
        }.ToImmutableDictionary();

        public static readonly ImmutableDictionary<int, string> MuZhuangDescNames = new Dictionary<int, string>()
        {
            {121, "初级试炼木桩"}, {122, "中级试炼木桩"}, {123, "高级试炼木桩"}, {124, "极境试炼木桩"}
        }.ToImmutableDictionary();
    }

    public class GlobalParams
    {
        public readonly double CT = 0;
        public readonly double CF = 0;
        public readonly double HT = 0;
        public readonly double WS = 0;
        public readonly double Def = 0;
        public readonly double OC = 0;
        public readonly double HS = 0;
        public readonly double PZ = 0;

        public readonly int _Level;

        [JsonIgnore] public readonly Dictionary<string, double> Dict;

        public GlobalParams(int level)
        {
            _Level = level;
            double levelFactor = LevelFactor(level);

            CT = BaseGlobalParams.CT * levelFactor;
            CF = BaseGlobalParams.CF * levelFactor;
            HT = BaseGlobalParams.HT * levelFactor;
            WS = BaseGlobalParams.WS * levelFactor;
            Def = BaseGlobalParams.Def * levelFactor;
            OC = BaseGlobalParams.OC * levelFactor;
            HS = BaseGlobalParams.HS * levelFactor;

            PZ = BaseGlobalParams.PZ;

            Dict = GetDict();
        }

        public static double LevelFactor(int level)
        {
            double res = 0;
            if (level <= 15)
            {
                res = 50;
            }
            else if (15 <= level && level < 90)
            {
                res = 4 * level - 10;
            }
            else if (90 <= level && level < 95)
            {
                res = 85 * (level - 90) + 350;
            }
            else if (95 <= level && level < 100)
            {
                res = 185 * (level - 95) + 775;
            }
            else if (100 <= level && level < 110)
            {
                res = 205 * (level - 100) + 1700;
            }
            else if (110 <= level && level < 130)
            {
                res = 450 * (level - 110) + 3750;
            }

            return res;
        }


        public List<string> GetCatStr()
        {
            List<string> res = new List<string>();
            res.Add($"等级：{_Level}");

            int nParams = BaseGlobalParams.Dict.Count;
            string[] paramsStrings = new string[nParams];

            int i = 0;
            foreach (string key in BaseGlobalParams.Dict.Keys)
            {
                double value = (double)this.GetField(key);
                paramsStrings[i] = $"{key}: {value}";
                i++;
            }

            var body = string.Join(", ", paramsStrings);
            res.Add(body);
            res.Add("");

            return res;
        }

        public void Cat()
        {
            var res = GetCatStr();
            res.Cat();
        }

        public Dictionary<string, double> GetDict()
        {
            var txt = JsonConvert.SerializeObject(this, Formatting.Indented);
            var res = new Dictionary<string, double>()
            {
                {nameof(CT), CT},
                {nameof(CF), CF},
                {nameof(HT), HT},
                {nameof(WS), WS},
                {nameof(Def), Def},
                {nameof(OC), OC},
                {nameof(HS), HS},
                {nameof(PZ), PZ},
            };
            return res;
        }
    }

}