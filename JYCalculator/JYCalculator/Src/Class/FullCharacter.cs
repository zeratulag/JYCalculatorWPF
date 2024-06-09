using System.Collections.Generic;
using static JYCalculator.Globals.XFStaticConst;

namespace JYCalculator.Class
{
    public partial class FullCharacter
    {
        #region 成员

        // 基础力道，最终力道
        public double Base_L { get; set; }
        public double Final_L { get; set; }


        // 力道提升
        public double L_Percent { get; set; }


        // 伤害提高
        public double DmgAdd { get; set; } // 外功伤害提高

        #endregion

        #region 构造

        /// <summary>
        /// 表示人物完整属性的类
        /// </summary>
        /// <param name="base_l">基础元气</param>
        /// <param name="final_l">最终元气</param>
        /// <param name="base_ap">基础攻击</param>
        /// <param name="final_ap">最终攻击</param>
        /// <param name="base_oc">基础破防</param>
        /// <param name="final_oc">最终破防</param>
        /// <param name="wp">武器伤害</param>
        /// <param name="ct">会心</param>
        /// <param name="cf">会效</param>
        /// <param name="ws">无双</param>
        /// <param name="pz">破招</param>
        /// <param name="hs">加速</param>
        /// <param name="lPercent">元气提升</param>
        /// <param name="ap_percent">攻击提升</param>
        /// <param name="oc_percent">破防提升</param>
        /// <param name="ignorea">无视防御A</param>
        /// <param name="dmgadd">伤害提高</param>
        /// <param name="name">名称</param>
        public FullCharacter(double base_l, double final_l, double base_ap, double final_ap, double base_oc,
            double final_oc,
            double wp, double ct, double cf, double ws, double pz, double hs,
            double lPercent, double ap_percent, double oc_percent,
            double ignorea, double dmgadd,
            string name = "完整面板")
        {
            Base_L = base_l;
            Final_L = final_l;
            Base_AP = base_ap;
            Final_AP = final_ap;
            Base_OC = base_oc;
            Final_OC = final_oc;
            WP = wp;
            CT = ct;
            CF = cf;
            WS = ws;
            PZ = pz;
            HS = hs;
            L_Percent = lPercent;
            AP_Percent = ap_percent;
            OC_Percent = oc_percent;
            IgnoreA = ignorea;
            DmgAdd = dmgadd;
            Name = name;
            PostConstructor();
        }

        /// <summary>
        /// 从InitCharacter转换
        /// </summary>
        /// <param name="iChar">初始属性</param>
        public FullCharacter(InitCharacter iChar)
        {
            Base_L = iChar.Base_L;
            Final_L = iChar.Final_L;
            Base_AP = iChar.Base_AP;
            Final_AP = iChar.Final_AP;
            Base_OC = iChar.OC;
            Final_OC = iChar.OC;

            WP = iChar.WP;
            CT = iChar.CT;
            CF = iChar.CF;
            WS = iChar.WS;
            PZ = iChar.PZ;
            HS = iChar.HS;


            L_Percent = iChar.L_Percent;
            AP_Percent = 0.0;
            OC_Percent = 0.0;

            IgnoreA = 0.0;
            DmgAdd = 0.0;

            Name = iChar.Name;
            Had_BigFM_jacket = iChar.Had_BigFM_jacket;
            Had_BigFM_hat = iChar.Had_BigFM_hat;
            PostConstructor();
        }

        /// <summary>
        /// 复制构造函数
        /// </summary>
        /// <param name="old"></param>
        public FullCharacter(FullCharacter old)
        {
            Base_L = old.Base_L;
            Final_L = old.Final_L;
            Base_AP = old.Base_AP;
            Final_AP = old.Final_AP;
            Base_OC = old.Base_OC;
            Final_OC = old.Final_OC;

            WP = old.WP;
            CT = old.CT;
            CF = old.CF;
            WS = old.WS;
            PZ = old.PZ;
            HS = old.HS;


            L_Percent = old.L_Percent;
            AP_Percent = old.AP_Percent;
            OC_Percent = old.OC_Percent;

            IgnoreA = old.IgnoreA;
            DmgAdd = old.DmgAdd;

            Name = old.Name;
            ExtraSP = old.ExtraSP;
            Is_XW = old.Is_XW;
            Has_Special_Buff = old.Has_Special_Buff;

            Had_BigFM_jacket = old.Had_BigFM_jacket;
            Had_BigFM_hat = old.Had_BigFM_hat;

            PostConstructor();
        }

        // 复制构造

        // 复制构造，更改名称

        #endregion

        #region 显示

        public IList<string> GetCatStrList()
        {
            var hadDict = new Dictionary<bool, string>()
            {
                {true, "已"}, {false, "未"}
            };

            var res = new List<string>
            {
                $"{Final_L:F2}({Base_L:F2}) 力道，" +
                $"{Final_AP:F2}({Base_AP:F2}) 攻击，" +
                $"{Final_OC:F2}({Base_OC:F2}) 破防，" +
                $"{WP:F2} 武器伤害",

                $"{CT:P2} 会心，{CF:P2} 会效，{WS:P2} 无双，" +
                $"{HS}({HS / fGP.HS + ExtraSP / 1024:P2}) 加速，{PZ} 破招",
                $"{L_Percent:P2} 元气提升，{AP_Percent:P2} 攻击提升，{OC_Percent:P2} 破防提升，" +
                $"{IgnoreA:P2} 无视防御A，{DmgAdd:P2} 伤害提高，",

                $"{hadDict[Is_XW]}处于心无状态，{hadDict[Has_Special_Buff]}计算特殊增益",

                $"常规GCD: {Normal_GCD:F4} s, 大心无GCD: {BigXW_GCD:F4} s"
            };

            return res;
        }

        #endregion

    }
}