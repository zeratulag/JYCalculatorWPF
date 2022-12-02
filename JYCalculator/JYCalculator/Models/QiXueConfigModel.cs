using JX3CalculatorShared.Utils;
using JYCalculator.Globals;
using JYCalculator.Src.Data;
using JYCalculator.ViewModels;
using System.Linq;
using JX3CalculatorShared.Models;

namespace JYCalculator.Models
{
    public class QiXueConfigModel : QiXueConfigModelBase
    {
        #region 成员

        // Tags

        // 是否拥有如下奇穴
        public bool 秋风散影 { get; private set; }
        public bool 聚精凝神 { get; private set; }
        public bool 梨花带雨 { get; private set; }
        public bool 百里追魂 { get; private set; }
        public bool 秋声泠羽 { get; private set; }
        public bool 白雨跳珠 { get; private set; }
        public bool 回肠荡气 { get; private set; }
        public bool 摧心 { get; private set; }
        public bool 鹰扬虎视 { get; private set; }
        public bool 星落如雨 { get; private set; }
        public bool 寒江夜雨 { get; private set; }


        public int BYPerCast; // 暴雨一次释放的跳数
        public int CX_DOT_Stack; // 穿心dot叠的最大层数;

        // 流派


        public string Genre; // 实际流派
        public string SkillBaseNumGenre; // 基础技能数流派

        #endregion

        #region 构建

        public void UpdateInput(QiXueConfigViewModel vm)
        {
            QiXueNames = vm.ItemNames;
            QiXueNamesSet = QiXueNames.ToHashSet();
            Calc();
        }

        #endregion

        public override void Calc()
        {
            GetRecipes();
            GetTags();
            GetXW();
            GetNums();
            GetSkillEffects();
            GetSkillEvents();
            GetSelfBuffNames();
            GetIsSupport();
            GetGenre();
        }

        public void GetRecipes()
        {
            var dict = StaticJYData.DB.Recipe.OtherAssociate;

            AssociateKeys.Clear();
            OtherRecipes.Clear();

            foreach (var name in QiXueNames)
            {
                var key = $"奇穴-{name}";
                if (dict.ContainsKey(key))
                {
                    AssociateKeys.Add(key);
                    OtherRecipes.AddRange(dict[key]);
                }
            }
        }

        public void GetTags()
        {
            秋风散影 = Has(nameof(秋风散影));
            聚精凝神 = Has(nameof(聚精凝神));
            梨花带雨 = Has(nameof(梨花带雨));
            百里追魂 = Has(nameof(百里追魂));
            秋声泠羽 = Has(nameof(秋声泠羽));
            白雨跳珠 = Has(nameof(白雨跳珠));
            回肠荡气 = Has(nameof(回肠荡气));
            鹰扬虎视 = Has(nameof(鹰扬虎视));
            摧心 = Has(nameof(摧心));
            星落如雨 = Has(nameof(星落如雨));
            寒江夜雨 = Has(nameof(寒江夜雨));
        }

        /// <summary>
        /// 是否激活了奇穴
        /// </summary>
        /// <param name="name">奇穴名，例如"秋风散影"</param>
        public bool Has(string name)
        {
            return QiXueNamesSet.Contains(name);
        }


        /// <summary>
        /// 计算心无相关属性
        /// </summary>
        public void GetXW()
        {
            XWCD = 90;
            XWDuration = 10 + 5.0 * 聚精凝神.ToInt();
            XWExtraSP = JYStaticData.XWConsts.ExtraSP * 聚精凝神.ToInt();
            NormalDuration = XWCD - XWDuration;
        }

        public void GetNums()
        {
            BYPerCast = 梨花带雨 ? 7 : 5;
            CX_DOT_Stack = 摧心 ? 3 : 2;
        }

        public void GetSkillEffects()
        {
            SkillModifiers = StaticJYData.DB.SkillModifier.GetQiXueMods(QiXueNamesSet);
        }

        public void GetSkillEvents()
        {
            SkillEvents = StaticJYData.DB.SkillInfo.GetQiXueEvents(QiXueNamesSet);
        }

        /// <summary>
        /// 计算有哪些激活的个人Buff
        /// </summary>
        public void GetSelfBuffNames()
        {
            SelfBuffNames = StaticJYData.DB.Buff.GetQiXueBuffs(QiXueNamesSet);
        }

        public void GetIsSupport()
        {
            IsSupport = IsSupported(StaticJYData.Data.Setting);
        }

        /// <summary>
        /// 获取当前流派
        /// </summary>
        protected void GetGenre()
        {
            string res = "ZhuXingBaiLi_HuiChang";

            if (百里追魂)
            {
                if (回肠荡气)
                {
                    res = "ZhuXingBaiLi_HuiChang";
                }
                else if (白雨跳珠)
                {
                    res = "ZhuXingBaiLi_BaiYu";
                }
            }

            SkillBaseNumGenre = res;
            Genre = res;
        }


        /// <summary>
        /// 逐星流下，计算心无CD
        /// </summary>
        /// <param name="normalZXFreq">常规逐星频率</param>
        /// <param name="XWZXFreq">心无逐星频率</param>
        /// <returns></returns>
        public double GetZXXWCD(double normalZXFreq, double XWZXFreq)
        {
            double ZX_Reduce_CD = 3; // 逐星降低心无CD
            double XW_LAG = 2.5; // 心无延迟释放
            if (!寒江夜雨)
            {
                return 90.0;
            }

            var res = 15 * (6 + normalZXFreq * ZX_Reduce_CD - XWZXFreq * ZX_Reduce_CD) /
                (1 + normalZXFreq * ZX_Reduce_CD) + XW_LAG;
            return res;
        }

        /// <summary>
        /// 设置逐星流心无CD;
        /// </summary>
        /// <param name="normalZXFreq"></param>
        /// <param name="XWZXFreq"></param>
        /// <returns></returns>
        public double SetZXXWCD(double normalZXFreq, double XWZXFreq)
        {
            var cd = GetZXXWCD(normalZXFreq, XWZXFreq);
            XWCD = cd;
            return cd;
        }

    }
}