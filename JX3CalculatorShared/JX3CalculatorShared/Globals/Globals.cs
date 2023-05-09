using JX3CalculatorShared.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Immutable;
using System;

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
        public const string RESOURCE_DIAMOND_URI = URI_PREFIX + "Resource/Diamond/";
        public const string RESOURCE_IMAGE_URI = URI_PREFIX + "Resource/Images/";

        public const string SinaWBURL = @"https://weibo.com/u/1841842934"; // 个人主页

        public const string AT_PATH = DATA_FOLDER + "TM_Ats.xlsx";
        public const string BUILD_PATH = DATA_FOLDER + "BuildDate.txt";
        public const string EquipMap_Path = DATA_FOLDER + "EquipMap.xlsx";
        public const string Pz_Path = DATA_FOLDER + "pz.xlsx";
        public const string LevelData_Path = DATA_FOLDER + "levelData.json";

        public static string XinFaTag; // 需要手动设定
        public static string XinFa; // 需要手动设定


        public const string HasteModifyType = "atHasteBase";
    }

    public static class StaticConst
        // 存储一些游戏常量，此类的成员将会被using static 直接访问
    {
        public const int CurrentLevel = 120; // 当前人物等级
        public const double FPS_PER_SECOND = 16.0;
        public const double GCD = 1.5;
        public const int GCD_FPS = 24;
        public const int NumberOfQiXue = 12;
        public const double G_KILO = 1024.0; // 郭氏千
        public static readonly GlobalParams fGP;
        public const double CriticalDamageStart = 1.75; // 初始会效
        public const double CriticalDamageMax = 3.00; // 会效最大值
        public const double DefMax = 0.75; // 防御减伤最大值
        public const double HJMax = 0.8; // 化劲减伤最大值


        public static readonly string[] UsefulStoneAttrs =
        {
            // 通用有效五彩石属性
            "atSurplusValueBase", "atStrainBase"
        };

        static StaticConst()
        {
            fGP = new GlobalParams(CurrentLevel);
        }
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

        /// <summary>
        /// 真实四舍五入
        /// </summary>
        /// <param name="x">需要舍入的数</param>
        /// <param name="nDigits">保留位数，默认为0</param>
        /// <returns>结果</returns>
        public static double RealRound(this double x, int nDigits = 0)
        {
            double n = Math.Pow(10, nDigits);
            double res = (int) (x * n + 0.5) / n;
            return res;
        }

        public static int MathRound(this double x)
        {
            int res = (int) (x + 0.5);
            return res;
        }

        public static int MathRound(this decimal x)
        {
            int res = (int) (x + 0.5m);
            return res;
        }
    }

    public static class StringConsts
    {
        public const string TooltipDivider0 = "---";
        public const string TooltipDivider = "\n" + TooltipDivider0 + "\n";

        public static readonly ImmutableArray<char> ChinaNumber = "零一二三四五六七八九十".ToCharArray().ToImmutableArray();
        public static readonly ImmutableArray<char> ChinaBigNumber = "零壹贰叁肆伍陆柒捌玖拾".ToCharArray().ToImmutableArray();
    }

    public static class GlobalParamLUA
    {
        public const double fPlayerCriticalCof = 0.75; // 会效起点
        public const double fCriticalStrikeParam = 9.530; // 会心
        public const double fCriticalStrikePowerParam = 3.335; // 会效
        public const double fDefCriticalStrikeParam = 9.530; // 御劲
        public const double fDecriticalStrikePowerParam = 1.380; // 化劲
        public const double fHitValueParam = 6.931; // 命中
        public const double fDodgeParam = 3.703; // 闪躲
        public const double fParryParam = 4.345; // 招架
        public const double fInsightParam = 9.189; // 无双
        public const double fPhysicsShieldParam = 5.091; // 外防
        public const double fMagicShieldParam = 5.091; // 内防
        public const double fOvercomeParam = 9.530; // 破防
        public const double fHasteRate = 11.695; // 加速
        public const double fToughnessDecirDamageCof = 2.557; // 御劲减会效
        public const double fSurplusParam = 13.192; // 破招
        public const double fAssistedPowerCof = 9.53; // 侠客属性
    }


    public static class BaseGlobalParams
    {
        // 宇宙常数类
        public const double CT = GlobalParamLUA.fCriticalStrikeParam;
        public const double CF = GlobalParamLUA.fCriticalStrikePowerParam;
        public const double HT = GlobalParamLUA.fHitValueParam;
        public const double WS = GlobalParamLUA.fInsightParam;
        public const double Def = GlobalParamLUA.fPhysicsShieldParam;
        public const double OC = GlobalParamLUA.fOvercomeParam;
        public const double HS = GlobalParamLUA.fHasteRate;
        public const double PZ = GlobalParamLUA.fSurplusParam;
        public const double PDef = GlobalParamLUA.fPhysicsShieldParam; // 外防
        public const double MDef = GlobalParamLUA.fMagicShieldParam; // 内防
        public const double HJ = GlobalParamLUA.fDecriticalStrikePowerParam; // 化劲
        public const double YJ = GlobalParamLUA.fDefCriticalStrikeParam; // 御劲
        public const double YJCF = GlobalParamLUA.fToughnessDecirDamageCof; // 御劲减会效

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
                {nameof(PZ), PZ},
                {nameof(HJ), HJ},
                {nameof(PDef), PDef},
                {nameof(MDef), MDef},
                {nameof(YJ), YJ},
                {nameof(YJCF), YJCF},
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
        public readonly double HJ = 0; // 化劲
        public readonly double PDef = 0;
        public readonly double MDef = 0;
        public readonly double YJ = 0;
        public readonly double YJCF = 0; // 御劲减会效

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
            HJ = BaseGlobalParams.HJ * levelFactor;
            PDef = BaseGlobalParams.PDef * levelFactor;
            MDef = BaseGlobalParams.MDef * levelFactor;
            YJ = BaseGlobalParams.YJ * levelFactor;
            YJCF = BaseGlobalParams.YJCF * levelFactor;

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
                double value = (double) this.GetField(key);
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
                {nameof(PDef), PDef},
                {nameof(MDef), MDef},
                {nameof(HJ), HJ},
                {nameof(YJ), YJ},
                {nameof(YJCF), YJCF}
            };
            return res;
        }
    }

    public static class SystemPrimaryAttribute
    {
        // 描述系统自带主属性加成的常数
        public const int VitalityToMaxLifeBase = 10; // 体质 atMaxLifeBase
        public const double SpiritToMagicCriticalStrike = 655.36 / 1024.0; // 根骨加内功会心 atMagicCriticalStrike
        public const double StrengthToPhysicsOvercome = 307.2 / 1024.0; // 力道加外功破防 atPhysicsOvercomeBase
        public const double StrengthToPhysicsAttackPower = 153.6 / 1024.0; // 力道加外功基础攻击 atPhysicsAttackPowerBase
        public const double AgilityToPhysicsCriticalStrike = 655.36 / 1024.0; // 身法加外功会心 atPhysicsCriticalStrike
        public const double SpunkToMagicAttackPower = 184.32 / 1024.0; // 元气加内功基础攻击 atMagicAttackPowerBase
        public const double SpunkToMagiOvercome = 307.2 / 1024.0; // 元气加内功基础破防 atMagicOvercome
    }
}