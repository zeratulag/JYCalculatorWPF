using JYCalculator.DB;
using JYCalculator.Globals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static JYCalculator.Globals.XFStaticConst;

namespace JYCalculator.Class
{
    public partial class FullCharacter
    {
        #region 成员

        // 基础力道，最终力道
        public double Base_L { get; private set; }
        public double Final_L { get; private set; }


        // 力道提升
        public double L_Percent { get; private set; }


        // 伤害提高
        public double DmgAdd { get; private set; } // 外功伤害提高

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
            Base_L = iChar.L;
            Final_L = iChar.L;
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


            L_Percent = 0.0;
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

        #region 属性计算_各种属性

        public void Add_WP(double value)
        {
            WP += value;
        }

        public void Add_CT(double value)
        {
            if (Has_Special_Buff)
            {
                throw new ArgumentException("Cannot add CT after has special_buff!");
            }
            else
            {
                CT += value;
            }
        }

        public void Add_CT_Point(double value)
        {
            Add_CT(value / fGP.CT);
        }

        public void Add_CF(double value)
        {
            CF += value;
        }

        public void Add_CF_Point(double value)
        {
            Add_CF(value / fGP.CF);
        }

        public void Add_WS(double value)
        {
            WS += value;
        }

        public void Add_WS_Point(double value)
        {
            Add_WS(value / fGP.WS);
        }

        public void Add_HSP(double value)
        {
            HS += value; // 注意此处加速改变
        }

        public void Add_PZ(double value)
        {
            PZ += value;
        }

        public void Add_Final_AP(double value)
        {
            Final_AP += value;
        }

        public void Add_Base_AP(double value)
        {
            Base_AP += value;
            Add_Final_AP(value * (1 + AP_Percent));
        }

        public void Add_AP_Percent(double value)
        {
            AP_Percent += value;
            Add_Final_AP(value * Base_AP);
        }

        public void Add_Final_OC(double value)
        {
            Final_OC += value;
        }

        public void Add_Base_OC(double value)
        {
            Base_OC += value;
            Add_Final_OC(value * (1 + OC_Percent));
        }

        public void Add_OC_Percent(double value)
        {
            OC_Percent += value;
            Add_Final_OC(value * Base_OC);
        }

        public void Add_S(double value) //身法
        {
            Add_CT_Point(value * XFConsts.CT_PER_S);
        }


        public void Add_IgnoreA(double value)
        {
            IgnoreA += value;
        }

        public void Add_DmgAdd(double value)
        {
            DmgAdd += value;
        }

        public void Add_P_DmgAdd(double value)
        {
            Add_DmgAdd(value);
        }

        public void Add_All_DmgAdd(double value)
        {
            Add_P_DmgAdd(value);
        }


        public void Add_Final_L(double value) // 增加力道
        {
            Final_L += value;
            Add_Base_AP(value * XFConsts.AP_PER_L);
            Add_Base_OC(value * XFConsts.OC_PER_L);
            Add_Final_AP(value * XFConsts.F_AP_PER_L);
            Add_CT_Point(value * XFConsts.CT_PER_L);
        }

        public void Add_Base_L(double value)
        {
            Base_L += value;
            Add_Final_L(value * (1 + L_Percent));
        }

        public void Add_L_Percent(double value)
        {
            L_Percent += value;
            Add_Final_L(value * Base_L);
        }


        /// <summary>
        /// 增加全属性
        /// </summary>
        /// <param name="value"></param>
        public void Add_All_BasePotent(double value)
        {
            Add_Base_L(value);
            Add_S(value);
        }


        /// <summary>
        /// 属性修改分派，注意这段代码是由Python程序SwitchTool.py生成的
        /// </summary>
        /// <param name="key">简化后的属性名称</param>
        /// <param name="value">属性值</param>
        protected void _AddSAttr(string key, double value)
        {
            switch (key)
            {
                case "WP":
                    {
                        Add_WP(value);
                        break;
                    }
                case "CT":
                    {
                        Add_CT(value);
                        break;
                    }
                case "CT_Point":
                    {
                        Add_CT_Point(value);
                        break;
                    }
                case "CF":
                    {
                        Add_CF(value);
                        break;
                    }
                case "CF_Point":
                    {
                        Add_CF_Point(value);
                        break;
                    }
                case "WS":
                    {
                        Add_WS(value);
                        break;
                    }
                case "WS_Point":
                    {
                        Add_WS_Point(value);
                        break;
                    }
                case "HSP":
                    {
                        Add_HSP(value);
                        break;
                    }
                case "PZ":
                    {
                        Add_PZ(value);
                        break;
                    }
                case "Final_AP":
                    {
                        Add_Final_AP(value);
                        break;
                    }
                case "Base_AP":
                    {
                        Add_Base_AP(value);
                        break;
                    }
                case "AP_Percent":
                    {
                        Add_AP_Percent(value);
                        break;
                    }
                case "Final_OC":
                    {
                        Add_Final_OC(value);
                        break;
                    }
                case "Base_OC":
                    {
                        Add_Base_OC(value);
                        break;
                    }
                case "OC_Percent":
                    {
                        Add_OC_Percent(value);
                        break;
                    }
                case "S":
                    {
                        Add_S(value);
                        break;
                    }

                case "IgnoreA":
                    {
                        Add_IgnoreA(value);
                        break;
                    }
                case "DmgAdd":
                    {
                        Add_DmgAdd(value);
                        break;
                    }
                case "Final_L":
                    {
                        Add_Final_L(value);
                        break;
                    }
                case "Base_L":
                    {
                        Add_Base_L(value);
                        break;
                    }
                case "L_Percent":
                    {
                        Add_L_Percent(value);
                        break;
                    }
                case "All_BasePotent":
                    {
                        Add_All_BasePotent(value);
                        break;
                    }

                case "P_DmgAdd":
                    {
                        Add_P_DmgAdd(value);
                        break;
                    }

                case "All_DmgAdd":
                    {
                        Add_All_DmgAdd(value);
                        break;
                    }

                default:
                    {
                        if (!XFDataBase.UselessAttrs.Contains(key))
                        {
                            Trace.WriteLine($"未知的F属性！\t{key}\t{value} ");
                        }

                        break;
                    }
            }
        }

        #endregion
    }
}