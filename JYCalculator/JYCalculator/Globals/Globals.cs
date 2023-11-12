using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JYCalculator.Class;
using JYCalculator.Src;
using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using System.Windows.Input;


namespace JYCalculator.Globals
{
    public static class XFAppStatic
    {
        public const string DATA_FOLDER = AppStatic.DATA_FOLDER;
        public const string DATA_PATH = DATA_FOLDER + "JY_Data.xlsx";
        public const string OUTPUT_PATH = DATA_FOLDER + "JY_Output.xlsx";
        public const string ZHENFA_PATH = DATA_FOLDER + "JY_Zhenfa.json";
        public const string SETTING_PATH = DATA_FOLDER + "JY_Setting.json";

        public const string DB_PATH = DATA_FOLDER + "JY_Output.db";

        public const string XinFa = "惊羽诀";
        public const string XinFaTag = "JY";

        public static readonly string TypePrefix = "";

        public const string FileExtension = "jyd"; // 扩展名
        public static readonly string FileFilter; // 打开方式的Filter;

        public const string GameVersion = "万灵当歌";

        public const string JX3BOXURL = @"https://www.jx3box.com/bps/49353"; // JX3BOX主页
        public const string GitHubURL = @"https://github.com/zeratulag/JYCalculatorWPF"; // GitHub主页
        public const string XFTutorialURL = @"https://www.jx3box.com/bps/68616"; // 当前版本惊羽攻略
        public const string TMTutorialURL = @"https://www.jx3box.com/bps/21041"; // 个人作品合集

        public static readonly ImmutableDictionary<string, string> URLDict; // URL 字典，方便Cmd调用

        public static readonly AppMetaInfo CurrentAppMeta;
        public static readonly Version AppVersion;
        public static readonly string MainTitle;
        public static readonly DateTime BuildDateTime; // 构建时间
        public static readonly DateTime LastPatchTime; // 最新技改时间
        public const string LastPatchURL = @"https://jx3.xoyo.com/show-2466-5961-1.html"; // 最新技改内容


        static XFAppStatic()
        {
            LastPatchTime = new DateTime(2023, 10, 26); // 最新技改时间

            AppVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var version = AppVersion.ToString();
            var dateTimestr = ImportTool.ReadAllTextFromResource(AppStatic.BUILD_PATH, Encoding.Default);
            BuildDateTime = DateTime.Parse(dateTimestr);

            CurrentAppMeta =
                new AppMetaInfo(AppVersion, XinFa, GameVersion, StaticConst.CurrentLevel, LastPatchTime);

            MainTitle = $"惊羽诀配装计算器 Pro（v{version} 公测版）";
            FileFilter = $"计算方案 (*.{FileExtension})|*.{FileExtension}"; // 打开方式的Filter

            var urlB = ImmutableDictionary.CreateBuilder<string, string>();
            urlB.Add("JB", JX3BOXURL);
            urlB.Add("Git", GitHubURL);
            urlB.Add("WB", AppStatic.SinaWBURL);
            urlB.Add("XF", XFTutorialURL);
            urlB.Add("TM", TMTutorialURL);
            URLDict = urlB.ToImmutable();

            switch (XinFaTag)
            {
                // 攻击和破防统一去除前缀
                case "JY":
                {
                    TypePrefix = "P_";
                    break;
                }

                case "TL":
                {
                    TypePrefix = "M_";
                    break;
                }
            }
        }

        public static void SyncToAppStatic()
        {
            AppStatic.XinFaTag = XinFaTag;
            AppStatic.XinFa = XinFa;
        }

        public static string GetXFSID(string key)
        {
            string key1 = key;
            if (!key1.EndsWith("_DmgAdd"))
            {
                key1 = key.RemovePrefix(TypePrefix);
            }

            return key1;
        }
    }


    public static class XFConsts
    {
        // 和心法有关的常数

        public const double ChannelIntervalToAPFactor = 10.0; // 内功为12，外功为10
        public const DamageTypeEnum CurrentDamnagType = DamageTypeEnum.Physics; // 当前心法伤害类型

        public const double AP_PER_L = 0.15; // 力道加基础攻击
        public const double OC_PER_L = 0.3; // 力道加基础破防
        public const double CT_PER_S = 0.64; // 身法加会心

        // 唐门_内功_箭弩

        public const double
            F_AP_PER_Y = 1792.0 / StaticConst.G_KILO; // 力道提高1.45外功AP，STRENGTH_TO_PHYSICS_ATTACK_POWER_COF 1485

        public const double
            CT_PER_Y = 584.0 / StaticConst.G_KILO; // 力道提高0.59会心，STRENGTH_TO_PHYSICS_CRITICAL_STRIKE_COF 604

        public const double
            F_AP_PER_L = (double) JYXinFa.STRENGTH_TO_PHYSICS_ATTACK_POWER_COF / StaticConst.G_KILO; // 力道提高1.45外功AP，STRENGTH_TO_PHYSICS_ATTACK_POWER_COF 1485

        public const double
            CT_PER_L = (double) JYXinFa.STRENGTH_TO_PHYSICS_CRITICAL_STRIKE_COF / StaticConst.G_KILO; // 力道提高0.59会心，STRENGTH_TO_PHYSICS_CRITICAL_STRIKE_COF 604

        public const double NPC_Coef = (double) JYXinFa.DST_NPC_DAMAGE_COEFFICIENT / StaticConst.G_KILO; // DST_NPC_DAMAGE_COEFFICIENT

        public static AttrWeight PointWeight = new AttrWeight("单点属性", "提高一点属性增加的DPS")
            {AP = 1, L = 1, WP = 1, Final_AP = 1, Final_OC = 1, Final_L = 1};

        public static AttrWeight ScoreWeight = new AttrWeight("同分属性", "提高等分属性增加的DPS")
            {AP = 1048.0 / 2340.0, L = 524.0 / 2340.0, WP = 1571.0 / 2340.0};
    }


    public class XFGlobalParams : GlobalParams
    {
        public readonly double XPZ = 0; // 心法破招系数
        public readonly double XPZ_BY = 0; // 白雨破招系数

        public XFGlobalParams(int level) : base(level)
        {
            XPZ = PZ * (1 + XFStaticConst.XinFaPZCoef);
            XPZ_BY = PZ * (1 + XFStaticConst.BaiYuPZCoef);
            Dict.Add(nameof(XPZ), XPZ);
            Dict.Add(nameof(XPZ_BY), XPZ_BY);
        }
    }

    public static class XFStaticConst
    {
        public static readonly XFGlobalParams fGP;
        public const double XinFaPZCoef = (0.78 - 1); // 心法破招系数，唐门惊羽破招子技能.lua  dwSkillLevel == 1
        public const double BaiYuPZCoef = (0.4263012 - 1); // 白雨破招系数， dwSkillLevel == 2
        public const double PZ_BaiYuPer_Normal_ZX = 1.486; // 非心无期间，平均每个逐星附带的白雨破招数量

        public static readonly Haste CurrentHaste;

        public static readonly string[] UsefulStoneAttrs =
        {
            "atStrengthBase", "atPhysicsAttackPowerBase",
            "atPhysicsCriticalStrike", "atPhysicsOvercomeBase", "atPhysicsCriticalDamagePowerBase",
        };

        public static class XW
        {
            public const double CT = 1500.0 / 10000.0;
            public const double CF = 300.0 / 1024.0;
            public const int ExtraSP = 204;
            public const double AP_Percent = 512.0 / 1024.0; // 天罗专用
            public const double CD = 90.0; // CD时间
            public const double Time = 15.0; // 持续时间
        }

        public static class Genre
        {
            // 流派命名
            public const string 逐星百里_回肠 = "逐星百里_回肠";
            public const string 逐星百里_白雨 = "逐星百里_白雨";
            public const string 逐一白雨 = "逐一白雨";
            public const string 白雨流_万灵当歌 = "白雨流_万灵当歌";
        }

        static XFStaticConst()
        {
            fGP = new XFGlobalParams(StaticConst.CurrentLevel);
            CurrentHaste = new Haste(StaticConst.CurrentLevel);
        }
    }
}