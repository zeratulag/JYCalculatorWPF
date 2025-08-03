using JX3CalculatorShared.Utils;
using Newtonsoft.Json;
using System;
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
        public const string RESOURCE_DIAMOND_URI = URI_PREFIX + "Resource/Diamond/";
        public const string RESOURCE_IMAGE_URI = URI_PREFIX + "Resource/Images/";

        public const string SinaWBURL = @"https://weibo.com/u/1841842934"; // 个人主页

        public const string AT_PATH = DATA_FOLDER + "TM_Ats.xlsx";
        public const string BUILD_PATH = DATA_FOLDER + "BuildDateTime.txt";
        public const string EquipMap_Path = DATA_FOLDER + "EquipMap.xlsx";
        public const string Pz_Path = DATA_FOLDER + "pz.xlsx";
        public const string LevelData_Path = DATA_FOLDER + "LevelData.json";

        public static string XinFaTag; // 需要手动设定
        public static string XinFa; // 需要手动设定

        public const string HasteModifyType = "atHasteBase";
    }

    public static class StaticConst
        // 存储一些游戏常量，此类的成员将会被using static 直接访问
    {
        public const int CurrentLevel = 130; // 当前人物等级
        public const int PreviousLevel = 120; // 上个人物等级
        public const double FRAMES_PER_SECOND = 16.0; // 每秒16逻辑帧
        public const double GCD = 1.5;
        public const int GCD_FPS = 24;
        public const int NumberOfQiXue = 12;
        public const double G_KILO_NUM = 1024.0; // 郭氏千
        public const double G_KILO_SQUARE_NUM = 1048576.0; // 郭氏千平方
        public static readonly GlobalParams CurrentLevelGlobalParams;
        public const double CriticalDamageStart = 1.75; // 初始会效
        public const double CriticalDamageMax = 3.00; // 会效最大值
        public const double DefMax = 0.75; // 防御减伤最大值
        public const double HJMax = 0.85; // 化劲减伤最大值


        public static readonly string[] UsefulStoneAttrs =
        {
            // 通用有效五彩石属性
            "atSurplusValueBase", "atStrainBase"
        };

        static StaticConst()
        {
            CurrentLevelGlobalParams = new GlobalParams(CurrentLevel);
        }
    }

    public static class GlobalFunctions
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

        /// <summary>
        /// 基于属性率，属性点数和转换系数计算最终属性值
        /// </summary>
        /// <param name="rate">属性率，eg 0.01 会心</param>
        /// <param name="point">属性等级，eg 58607 会心等级</param>
        /// <param name="coef">属性等级转换系数，eg 130级 197703.00</param>
        /// <returns>最终属性值，eg 0.306439</returns>
        public static double CalcFinalValueByRateAndPoint(double rate, double point, double coef)
        {
            double res = rate + point / coef;
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
        public const double fPlayerCriticalCof = 0.75; //裸体会效
        public const double fCriticalStrikeParam = 9.985; //会心
        public const double fCriticalStrikePowerParam = 3.679; //会效
        public const double fDefCriticalStrikeParam = 9.985; //御劲
        public const double fDecriticalStrikePowerParam = 1.669; //化劲
        public const double fHitValueParam = 7.644; //命中
        public const double fDodgeParam = 4.628; //闪避
        public const double fParryParam = 5.432; //招架
        public const double fInsightParam = 6.734; //无双（识破）
        public const double fPhysicsShieldParam = 6.364; //外防
        public const double fMagicShieldParam = 6.364; //内防
        public const double fOvercomeParam = 11.412; //破防
        public const double fHasteRate = 10.610; //急速
        public const double fToughnessDecirDamageCof = 2.784; //御劲效果
        public const double fSurplusParam = 7.421; //破招伤害
        public const double fAssistedPowerCof = 9.985; //凝神
    }


    public static class BaseGlobalParams
    {
        // 宇宙常数类
        public const double CriticalStrike = GlobalParamLUA.fCriticalStrikeParam;
        public const double CriticalPower = GlobalParamLUA.fCriticalStrikePowerParam;
        public const double Hit = GlobalParamLUA.fHitValueParam;
        public const double Strain = GlobalParamLUA.fInsightParam;
        public const double Shield = GlobalParamLUA.fPhysicsShieldParam;
        public const double Overcome = GlobalParamLUA.fOvercomeParam;
        public const double Haste = GlobalParamLUA.fHasteRate;
        public const double Surplus = GlobalParamLUA.fSurplusParam;
        public const double PhysicsShield = GlobalParamLUA.fPhysicsShieldParam; // 外防
        public const double MagicShield = GlobalParamLUA.fMagicShieldParam; // 内防
        public const double DefPlayerDamage = GlobalParamLUA.fDecriticalStrikePowerParam; // 化劲
        public const double DefCriticalStrike = GlobalParamLUA.fDefCriticalStrikeParam; // 御劲
        public const double DefCriticalPower = GlobalParamLUA.fToughnessDecirDamageCof; // 御劲减会效

        public static readonly ImmutableDictionary<string, double> Dict;

        public static Dictionary<string, double> GetDict()
        {
            var res = new Dictionary<string, double>()
            {
                {nameof(CriticalStrike), CriticalStrike},
                {nameof(CriticalPower), CriticalPower},
                {nameof(Hit), Hit},
                {nameof(Strain), Strain},
                {nameof(Shield), Shield},
                {nameof(Overcome), Overcome},
                {nameof(Haste), Haste},
                {nameof(Surplus), Surplus},
                {nameof(DefPlayerDamage), DefPlayerDamage},
                {nameof(PhysicsShield), PhysicsShield},
                {nameof(MagicShield), MagicShield},
                {nameof(DefCriticalStrike), DefCriticalStrike},
                {nameof(DefCriticalPower), DefCriticalPower},
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
        public readonly double CriticalStrike = 0; // 会心
        public readonly double CriticalPower = 0; // 会效
        public readonly double Hit = 0; // 命中
        public readonly double Strain = 0; // 无双
        public readonly double Shield = 0; // 防御
        public readonly double Overcome = 0; // 破防
        public readonly double Haste = 0; // 加速
        public readonly double Surplus = 0; // 破招
        public readonly double DefPlayerDamage = 0; // 化劲
        public readonly double PhysicsShield = 0; // 内攻防御
        public readonly double MagicShield = 0; // 外攻防御
        public readonly double DefCriticalStrike = 0;
        public readonly double DefCriticalPower = 0; // 御劲减会效

        public readonly int _Level;

        [JsonIgnore] public readonly Dictionary<string, double> Dict;

        public GlobalParams(int level)
        {
            _Level = level;
            double levelFactor = LevelFactor(level);

            CriticalStrike = BaseGlobalParams.CriticalStrike * levelFactor;
            CriticalPower = BaseGlobalParams.CriticalPower * levelFactor;
            Hit = BaseGlobalParams.Hit * levelFactor;
            Strain = BaseGlobalParams.Strain * levelFactor;
            Shield = BaseGlobalParams.Shield * levelFactor;
            Overcome = BaseGlobalParams.Overcome * levelFactor;
            Haste = BaseGlobalParams.Haste * levelFactor;
            DefPlayerDamage = BaseGlobalParams.DefPlayerDamage * levelFactor;
            PhysicsShield = BaseGlobalParams.PhysicsShield * levelFactor;
            MagicShield = BaseGlobalParams.MagicShield * levelFactor;
            DefCriticalStrike = BaseGlobalParams.DefCriticalStrike * levelFactor;
            DefCriticalPower = BaseGlobalParams.DefCriticalPower * levelFactor;

            Surplus = BaseGlobalParams.Surplus;

            Dict = GetDict();
        }

        public static double LevelFactor(int level)
        {
            double res = 0;

            if (level <= 15)
            {
                res = 50;
            }
            else if (level <= 90)
            {
                res = 4 * level - 10;
            }
            else if (level <= 95)
            {
                res = 85 * level - 7300;
            }
            else if (level <= 100)
            {
                res = 185 * level - 16800;
            }
            else if (level <= 110)
            {
                res = 205 * level - 18800;
            }
            else if (level <= 120)
            {
                res = 450 * level - 45750;
            }
            else
            {
                res = 1155 * level - 130350;
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
                {nameof(CriticalStrike), CriticalStrike},
                {nameof(CriticalPower), CriticalPower},
                {nameof(Hit), Hit},
                {nameof(Strain), Strain},
                {nameof(Shield), Shield},
                {nameof(Overcome), Overcome},
                {nameof(Haste), Haste},
                {nameof(Surplus), Surplus},
                {nameof(PhysicsShield), PhysicsShield},
                {nameof(MagicShield), MagicShield},
                {nameof(DefPlayerDamage), DefPlayerDamage},
                {nameof(DefCriticalStrike), DefCriticalStrike},
                {nameof(DefCriticalPower), DefCriticalPower}
            };
            return res;
        }
    }

    public static class SystemPrimaryAttribute
    {
        // 描述系统自带主属性加成的常数
        public const int VitalityToMaxLifeBase = 10; // 体质 atMaxLifeBase

        public const double StrengthToPhysicsBaseOvercome = 0.3; // 力道加外功基础破防 atPhysicsOvercomeBase
        public const double StrengthToPhysicsBaseAttackPower = 0.163; // 力道加外功基础攻击 atPhysicsAttackPowerBase
        public const double AgilityToPhysicsCriticalStrike = 0.9; // 身法加外功会心 atPhysicsCriticalStrike

        public const double SpiritToMagicCriticalStrike = 0.9; // 根骨加内功会心 atMagicCriticalStrike

        public const double SpunkToMagicBaseAttackPower = 0.181; // 元气加内功基础攻击 atMagicAttackPowerBase
        public const double SpunkToMagicBaseOvercome = 0.3; // 元气加内功基础破防 atMagicOvercome
    }
}