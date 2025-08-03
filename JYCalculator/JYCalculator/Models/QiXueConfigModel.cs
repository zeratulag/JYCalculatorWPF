using JYCalculator.Data;
using JYCalculator.Globals;
using System;

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
        public bool 空山独立 { get; private set; }

        // 雾海寻龙
        public bool 穿林打叶 { get; private set; }
        public bool 牢甲利兵 { get; private set; }
        public bool 蹑景追风 { get; private set; }
        public bool 掠影穹苍 { get; private set; }
        
        // 丝路风语
        public bool 凝形逐踪 { get; private set; }
        public bool 百步穿杨 { get; private set; }

        public int BYPerCast { get; private set; } // 暴雨一次释放的跳数
        public int CX_DOT_Stack { get; private set; } // 穿心dot叠的最大层数;
        public string CX_DOT_Key { get; private set; } // 穿心DOT的技能Key
        public string DP_Key { get; private set; } // 夺魄技能的Key

        public const double HJYY_BBCD = 1.0; // 寒江降低百步穿杨CD时间;
        // 流派


        //public string Genre; // 实际流派
        //public string SkillBaseNumGenre; // 基础技能数流派

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
            //GetIsSupport();
            GetCompatibleSkillBuild();
            GetGenre();
            GetCX_DOT_Key();
            GetDP_Key();
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
            空山独立 = Has(nameof(空山独立));
            穿林打叶 = Has(nameof(穿林打叶));
            牢甲利兵 = Has(nameof(牢甲利兵));
            蹑景追风 = Has(nameof(蹑景追风));
            掠影穹苍 = Has(nameof(掠影穹苍));
            凝形逐踪 = Has(nameof(凝形逐踪));
            百步穿杨 = Has(nameof(百步穿杨));
        }

        public void GetCX_DOT_Key()
        {
            string prefix;
            prefix = 鹰扬虎视 ? "CXY" : "CX";
            string part1 = $"{prefix}{CX_DOT_Stack}";
            string part2 = 穿林打叶 ? "_CL_DOT" : "_DOT";
            var res = part1 + part2;
            CX_DOT_Key = res;
        }

        public void GetDP_Key()
        {
            DP_Key = 牢甲利兵 ? SkillKeyConst.夺魄箭_牢甲利兵 : SkillKeyConst.夺魄箭;
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
                    if (鹰扬虎视)
                    {
                        res = XFStaticConst.Genre.逐星百里_白雨;
                    }
                    else
                    {
                        res = XFStaticConst.Genre.白雨流_万灵当歌;
                    }
                }
            }

            //SkillBaseNumGenre = res;
            //Genre = res;
        }


        /*/// <summary>
        /// 逐星流下，计算心无CD
        /// </summary>
        /// <param name="normalZXFreq">常规逐星频率</param>
        /// <param name="XWZXFreq">心无逐星频率</param>
        /// <returns></returns>
        public double GetZXXWCD(double normalZXFreq, double XWZXFreq)
        {
            double xWCD = XFStaticConst.XW.CD;

            if (!寒江夜雨)
            {
                return xWCD;
            }

            double ZX_Reduce_CD = 3; // 逐星降低心无CD
            double XW_LAG = 2.5; // 心无延迟释放

            var res = 15 * (6 + normalZXFreq * ZX_Reduce_CD - XWZXFreq * ZX_Reduce_CD) /
                (1 + normalZXFreq * ZX_Reduce_CD) + XW_LAG;
            return res;
        }*/

        /// <summary>
        /// 计算寒江心无实际CD
        /// </summary>
        /// <param name="normalFreq">常规寒江触发频率</param>
        /// <param name="xwFreq">常规寒江触发频率</param>
        /// <returns></returns>
        public double GetHJXWCD(double normalFreq, double xwFreq)
        {
            double xWCD = XFStaticConst.XinWuConsts.CD;

            if (!寒江夜雨)
            {
                return xWCD;
            }

            double HJReduceCD = 2.0; // 寒江减少CD时间
            double XW_LAG = 4.5; // 心无延迟释放
            var rawCD = (15 * HJReduceCD * normalFreq + xWCD - 15 * HJReduceCD * xwFreq) /
                        (1 + HJReduceCD * normalFreq);
            var res = Math.Max(rawCD + XW_LAG, 15.0); // 最低60秒
            return res;
        }

        /// <summary>
        /// 设置寒江流心无CD;
        /// </summary>
        /// <param name="normalFreq">常规寒江频率</param>
        /// <param name="xwFreq">心无寒江频率</param>
        /// <returns></returns>
        public double SetHJXWCD(double normalFreq, double xwFreq)
        {
            var cd = GetHJXWCD(normalFreq, xwFreq);
            XWCD = cd;
            return cd;
        }


        /// <summary>
        /// 基于寒江频率计算百里频率
        /// </summary>
        /// <param name="freq">寒江频率</param>
        /// <returns></returns>
        public double GetBLCDByHanJiangFreq(double freq)
        {
            double raw_blcd = StaticXFData.DB.BaseSkillInfo.Skills[SkillKeyConst.百里追魂].CD; // 原始CD
            double real_cd = raw_blcd;

            if (寒江夜雨)
            {
                real_cd = raw_blcd / (1 + freq * HJYY_BBCD) + 0.5; // 初始注能频率要比考虑了百里的频率高，故乘以惩罚系数0.986
            }

            return real_cd;
        }


        /*/// <summary>
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
        }*/
    }
}