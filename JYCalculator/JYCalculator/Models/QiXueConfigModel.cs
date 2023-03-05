using JYCalculator.Globals;

namespace JYCalculator.Models
{
    public partial class QiXueConfigModel
    {
        #region 成员

        // Tags

        // 是否拥有如下奇穴
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


        public void GetNums()
        {
            BYPerCast = 梨花带雨 ? 7 : 5;
            CX_DOT_Stack = 摧心 ? 3 : 2;
        }

        /// <summary>
        /// 获取当前流派
        /// </summary>
        protected void GetGenre()
        {
            string res = XFStaticConst.Genre.逐星百里_回肠;

            if (百里追魂)
            {
                if (回肠荡气)
                {
                    res = XFStaticConst.Genre.逐星百里_回肠;
                }
                else if (白雨跳珠)
                {
                    res = XFStaticConst.Genre.逐星百里_白雨;
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