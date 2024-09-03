using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JYCalculator.Class;
using JYCalculator.Src;
using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Text;


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
        public static readonly string SkillBuildFileFilter; // 武学方案Filter

        public const string GameVersion = "雾海寻龙";



        public const string JX3BOXURL = @"https://www.jx3box.com/bps/49353"; // JX3BOX主页
        public const string GitHubURL = @"https://github.com/zeratulag/JYCalculatorWPF"; // GitHub主页
        public const string TMTutorialURL = @"https://www.jx3box.com/bps/21041"; // 个人作品合集

        public static readonly ImmutableDictionary<string, string> URLDict; // URL 字典，方便Cmd调用

        public static readonly AppMetaInfo CurrentAppMeta;
        public static readonly Version AppVersion;
        public static readonly string MainTitle;
        public static readonly DateTime BuildDateTime; // 构建时间
        public static readonly DateTime LastPatchTime; // 最新技改时间

        #region 技改后需要修改
        public const string XFTutorialURL = @"https://www.jx3box.com/bps/78665"; // 当前版本惊羽攻略
        public const string LastPatchURL = @"https://jx3.xoyo.com/index/#/article-details?kid=1333872"; // 最新技改内容
        #endregion

        static XFAppStatic()
        {
            LastPatchTime = new DateTime(2024, 07, 08); // 最新技改时间

            AppVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var version = AppVersion.ToString();
            var dateTimeStr = ImportTool.ReadAllTextFromResource(AppStatic.BUILD_PATH, Encoding.Default).Trim();
            bool parseSuccess = DateTime.TryParseExact(dateTimeStr, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out BuildDateTime);
            if (!parseSuccess)
            {
                BuildDateTime = LastPatchTime; // 或者其他默认值
            }

            CurrentAppMeta =
                new AppMetaInfo(AppVersion, XinFa, GameVersion, StaticConst.CurrentLevel, LastPatchTime);

            MainTitle = $"惊羽诀配装计算器 Pro（v{version} 公测版）";
            FileFilter = $"计算方案 (*.{FileExtension})|*.{FileExtension}"; // 打开方式的Filter
            SkillBuildFileFilter = $"武学方案 (*.jyb)|*.jyb";

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

        public const DamageTypeEnum CurrentDamnagType = DamageTypeEnum.Physics; // 当前心法伤害类型

        public const double
            ChannelIntervalToAPFactor = CurrentDamnagType == DamageTypeEnum.Physics ? 10.0 : 12.0; // 内功为12，外功为10

        public const double AP_PER_L = 0.15; // 力道加基础攻击
        public const double OC_PER_L = 0.3; // 力道加基础破防
        public const double CT_PER_S = 0.64; // 身法加会心

        // 唐门_内功_箭弩

        public const double
            F_AP_PER_Y = 1792.0 / StaticConst.G_KILO_NUM; // 力道提高1.45外功AP，STRENGTH_TO_PHYSICS_ATTACK_POWER_COF 1485

        public const double
            CT_PER_Y = 584.0 / StaticConst.G_KILO_NUM; // 力道提高0.59会心，STRENGTH_TO_PHYSICS_CRITICAL_STRIKE_COF 604

        public const double
            F_AP_PER_L =
                (double) JYXinFa.STRENGTH_TO_PHYSICS_ATTACK_POWER_COF /
                StaticConst.G_KILO_NUM; // 力道提高1.45外功AP，STRENGTH_TO_PHYSICS_ATTACK_POWER_COF 1485

        public const double
            CT_PER_L = (double) JYXinFa.STRENGTH_TO_PHYSICS_CRITICAL_STRIKE_COF /
                       StaticConst.G_KILO_NUM; // 力道提高0.59会心，STRENGTH_TO_PHYSICS_CRITICAL_STRIKE_COF 604

        public const double
            NPC_Coef = (double) JYXinFa.DST_NPC_DAMAGE_COEFFICIENT /
                       StaticConst.G_KILO_NUM; // DST_NPC_DAMAGE_COEFFICIENT

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
        public const double BaiYuPZCoef = (0.5328765 - 1); // 白雨破招系数， dwSkillLevel == 2
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

            public const string XWBuffName = "XinWuPangWu";
            public const string BigXWBuffName = "XinWuPangWu(JuJingNingShen)";
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

    public static class SkillKeyConst
    {
        public const string 掠影穹苍 = "LveYingQiongCang";
        public const string 夺魄箭 = "DP";
        public const string 夺魄箭_牢甲利兵 = "DP_LaoJia";
        public const string 穿心弩 = "CXL";
        public const string 百里追魂 = "BL";
        public const string 逐星箭 = "ZX";
        public const string 追命箭 = "ZM";
        public const string 追命箭_瞬发 = "ZM_SF";
        public const string 破 = "PZ";
        public const string 破_白雨跳珠 = "PZ_BaiYu";
        public const string 罡风镖法 = "GF";
        public const string 孔雀翎 = "KongQueLing";
        public const string 穿林打叶 = "ChuanLinDaYe";

        public const string 星斗阑干 = "CW_DOT";

        public const string 追命箭_蹑景3_瞬发 = "ZM_NJ3_SF";

        public const string _暴雨梨花针_释放 = "_BYCast";
        public const string _夺魄箭_释放 = "_DPCast";
        public const string _追命箭_释放 = "_ZMCast";
        public const string _追命箭_瞬发_释放 = "_ZM_SFCast";

        public const string 暴雨梨花针 = "BY";
        public const string 暴雨梨花针2 = "LH2";
        public const string 暴雨梨花针3 = "LH3";
        public const string 暴雨梨花针4 = "LH4";
        public const string 暴雨梨花针5 = "LH5";
        public const string 暴雨梨花针6 = "LH6";
        public const string 暴雨梨花针7 = "LH7";

        public const string 白雨跳珠 = "BaiYuTiaoZhu";
    }
}