using System;
using System.Collections.Immutable;
using System.IO;
using JYCalculator.Class;
using System.Reflection;
using System.Text;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using Syncfusion.Windows.Shared;


namespace JYCalculator.Globals
{
    public static class JYAppStatic
    {
        public const string DATA_FOLDER = AppStatic.DATA_FOLDER;
        public const string DATA_PATH = DATA_FOLDER + "JY_Data.xlsx";
        public const string OUTPUT_PATH = DATA_FOLDER + "JY_Output.xlsx";
        public const string ZHENFA_PATH = DATA_FOLDER + "JY_Zhenfa.json";
        public const string SETTING_PATH = DATA_FOLDER + "JY_Setting.json";
        public const string AT_PATH = DATA_FOLDER + "TM_Ats.xlsx";

        public const string BUILD_PATH = DATA_FOLDER + "BuildDate.txt";

        public const string XinFa = "惊羽诀";
        public const string GameVersion = "横刀断浪";

        public const string JX3BOXURL = @"https://www.jx3box.com/tool/49353"; // JX3BOX主页

        public const string GitHubURL = @"https://github.com/zeratulag/JYCalculatorWPF"; // GitHub主页

        public const string JYTutorialURL = @"https://www.jx3box.com/bps/46381"; // 当前版本惊羽攻略
        public const string TMTutorialURL = @"https://www.jx3box.com/bps/21041"; // 个人作品合集

        public static readonly ImmutableDictionary<string, string> URLDict; // URL 字典，方便Cmd调用

        public static readonly AppMetaInfo CurrentAppMeta;
        public static readonly Version AppVersion;
        public static readonly string MainTitle;
        public static readonly DateTime BuildDateTime; // 构建时间
        

        static JYAppStatic()
        {
            AppVersion = Assembly.GetExecutingAssembly().GetName().Version;
            CurrentAppMeta =
                new AppMetaInfo(AppVersion, XinFa, GameVersion, StaticData.CurrentLevel);
            var version = AppVersion.ToString();

            var dateTimestr = ImportTool.ReadAllTextFromResource(BUILD_PATH, Encoding.Default);
            BuildDateTime = DateTime.Parse(dateTimestr);

            MainTitle = $"惊羽诀计算器 Pro（v{version} 公测版）";

            var urlB = ImmutableDictionary.CreateBuilder<string, string>();
            urlB.Add("JB", JX3BOXURL);
            urlB.Add("Git", GitHubURL);
            urlB.Add("WB", AppStatic.SinaWBURL);
            urlB.Add("JY", JYTutorialURL);
            urlB.Add("TM", TMTutorialURL);
            URLDict = urlB.ToImmutable();
        }
    }


    public static class JYConsts
    {
        // 和心法有关的常数
        // 唐门_内功_箭弩

        public const double ChannelIntervalToAPFactor = 10.0; // 内功为12，外功为10
        public const DamageTypeEnum CurrentDamnagType = DamageTypeEnum.Physics; // 当前心法伤害类型

        public const double AP_PER_L = 0.15; // 力道加基础攻击
        public const double OC_PER_L = 0.3; // 力道加基础破防
        public const double CT_PER_S = 0.64; // 身法加会心

        public const double
            F_AP_PER_Y = 1792.0 / StaticData.G_KILO; // 力道提高1.45外功AP，STRENGTH_TO_PHYSICS_ATTACK_POWER_COF 1485

        public const double
            CT_PER_Y = 584.0 / StaticData.G_KILO; // 力道提高0.59会心，STRENGTH_TO_PHYSICS_CRITICAL_STRIKE_COF 604

        public const double
            F_AP_PER_L = 1485.0 / StaticData.G_KILO; // 力道提高1.45外功AP，STRENGTH_TO_PHYSICS_ATTACK_POWER_COF 1485

        public const double
            CT_PER_L = 604.0 / StaticData.G_KILO; // 力道提高0.59会心，STRENGTH_TO_PHYSICS_CRITICAL_STRIKE_COF 604

        public const double NX_OC_Percent = 154.0 / StaticData.G_KILO;
        public const double CH_IgnoreA = 410.0 / StaticData.G_KILO;
        public const double NPC_Coef = 123.0 / StaticData.G_KILO; // DST_NPC_DAMAGE_COEFFICIENT


        public static AttrWeight PointWeight = new AttrWeight("单点属性", "提高一点属性增加的DPS")
            {AP = 1, L = 1, WP = 1, Final_AP = 1, Final_OC = 1, Final_L = 1};

        public static AttrWeight ScoreWeight = new AttrWeight("同分属性", "提高等分属性增加的DPS")
            {AP = 1048.0 / 2340.0, L = 524.0 / 2340.0, WP = 1571.0 / 2340.0};
    }


    public class JYGlobalParams : GlobalParams
    {
        public readonly double XPZ = 0; // 心法破招系数

        public JYGlobalParams(int level) : base(level)
        {
            XPZ = PZ * (1 + JYStaticData.XinFaPZCoef);
            Dict.Add(nameof(XPZ), XPZ);
        }
    }

    public static class JYStaticData
    {
        public static readonly JYGlobalParams fGP;
        public const double XinFaPZCoef = (0.78 - 1); // 心法破招系数

        public static class XWConsts
        {
            public const double CT = 1500.0 / 10000.0;
            public const double CF = 300.0 / 1024.0;
            public const int ExtraSP = 204;
        }

        public static readonly Haste CurrentHaste;

        static JYStaticData()
        {
            fGP = new JYGlobalParams(StaticData.CurrentLevel);
            CurrentHaste = new Haste(StaticData.CurrentLevel);
        }
    }
}