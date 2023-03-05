﻿using JYCalculator.Globals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static JYCalculator.Globals.XFStaticConst;

namespace JYCalculator.Class
{

    public partial class InitCharacter
    {
        #region 成员

        // 力道
        public double L { get; set; }

        #endregion

        #region 构造

        /// <summary>
        /// 表示人物初始属性（没有任何增益）的类
        /// </summary>
        /// <param name="l">力道</param>
        /// <param name="base_ap">基础攻击</param>
        /// <param name="final_ap">最终攻击</param>
        /// <param name="wp">武伤</param>
        /// <param name="ct">会心</param>
        /// <param name="cf">会效</param>
        /// <param name="ws">无双</param>
        /// <param name="pz">破招</param>
        /// <param name="oc">破防</param>
        /// <param name="hs">加速</param>
        /// <param name="had_BigFM_hat">是否带了伤帽</param>
        /// <param name="had_BigFM_jacket">是否带了伤衣</param>
        /// <param name="name">名称</param>
        public InitCharacter(double l, double base_ap, double final_ap, double wp,
            double ct, double cf, double ws,
            double pz, double oc, double hs,
            bool had_BigFM_hat = false, bool had_BigFM_jacket = false,
            string name = "") : this()
        {
            L = l;
            Base_AP = base_ap;
            Final_AP = final_ap;
            WP = wp;

            CT = ct;
            CF = cf;
            WS = ws;

            PZ = pz;
            OC = oc;
            HS = hs;

            Had_BigFM_hat = had_BigFM_hat;
            Had_BigFM_jacket = had_BigFM_jacket;
            Name = name;

            PostConstructor();
        }

        /// <summary>
        /// 复制构造
        /// </summary>
        /// <param name="old">旧的对象</param>
        public InitCharacter(InitCharacter old) : base()
        {
            L = old.L;
            Base_AP = old.Base_AP;
            Final_AP = old.Final_AP;
            WP = old.WP;

            CT = old.CT;
            CF = old.CF;
            WS = old.WS;

            PZ = old.PZ;
            OC = old.OC;
            HS = old.HS;

            Had_BigFM_jacket = old.Had_BigFM_jacket;
            Had_BigFM_hat = old.Had_BigFM_hat;
            Name = old.Name;
            PostConstructor();
        }

        #endregion

        /// <summary>
        /// 从导入的JB面板更新
        /// </summary>
        /// <param name="panel"></param>
        protected void _UpdateFromJBPanel(JBPZPanel panel)
        {
            L = panel.Strength;
            Base_AP = panel.PhysicsAttackPowerBase;
            Final_AP = panel.PhysicsAttackPower;
            WP = panel.MeleeWeaponDamage + panel.MeleeWeaponDamageRand / 2;

            CT = panel.PhysicsCriticalStrikeRate;
            CF = panel.PhysicsCriticalDamagePowerPercent;
            WS = panel.StrainPercent;

            PZ = panel.SurplusValue;
            OC = Math.Round(panel.PhysicsOvercomePercent * fGP.OC);
            HS = Math.Round(panel.HastePercent * fGP.HS);

            Had_BigFM_jacket = panel.EquipList.Had_BigFM_jacket;
            Had_BigFM_hat = panel.EquipList.Had_BigFM_hat;
            Name = panel.Title;
        }

        /// <summary>
        /// 从另外一个面板更新
        /// </summary>
        /// <param name="ichar"></param>
        protected void _UpdateFromIChar(InitCharacter ichar)
        {
            L = ichar.L;
            Base_AP = ichar.Base_AP;
            Final_AP = ichar.Final_AP;
            WP = ichar.WP;

            CT = ichar.CT;
            CF = ichar.CF;
            WS = ichar.WS;

            PZ = ichar.PZ;
            OC = ichar.OC;
            HS = ichar.HS;

            Had_BigFM_jacket = ichar.Had_BigFM_jacket;
            Had_BigFM_hat = ichar.Had_BigFM_hat;
            Name = ichar.Name;
        }

        #region 显示

        public IList<string> GetCatStrList()
        {
            var res = new List<string>
            {
                "人物初始属性：",
                $"{L:F0} 力道，{Base_AP:F0} 基础攻击，{Final_AP:F0} 最终攻击，{WP:F1} 武器伤害，{PZ:F0} 破招",

                $"{CT:P2} 会心，{CF:P2} 会效，{WS:P2} 无双，" +
                $"{OC:F0}({OC / fGP.OC:P2}) 破防，{HS:F0}({HS / fGP.HS:P2}) 加速",
            };

            res.Add("");
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
            CT += value;
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
            HS += value;
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
            Add_Final_AP(value);
        }

        public void Add_OC(double value)
        {
            OC += value;
        }

        public void Add_S(double value) //身法
        {
            Add_CT_Point(value * XFConsts.CT_PER_S);
        }

        public void Add_L(double value) // 最终力道
        {
            L += value;
            Add_Base_AP(value * XFConsts.AP_PER_L);
            Add_OC(value * XFConsts.OC_PER_L);

            Add_Final_AP(value * XFConsts.F_AP_PER_L);
            Add_CT_Point(value * XFConsts.CT_PER_L);
        }


        /// <summary>
        /// 增加全属性
        /// </summary>
        /// <param name="value"></param>
        public void Add_All_BasePotent(double value)
        {
            Add_L(value);
            Add_S(value);
            // TODO: 配装器里还需要增加体质
        }

        #endregion

        #region 属性计算

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

                case "OC":
                case "Base_OC":
                    {
                        Add_OC(value);
                        break;
                    }
                case "S":
                    {
                        Add_S(value);
                        break;
                    }
                case "L":
                case "Base_L":
                    {
                        Add_L(value);
                        break;
                    }
                case "All_BasePotent":
                    {
                        Add_All_BasePotent(value);
                        break;
                    }
                default:
                    {
                        Trace.WriteLine($"未知的I属性！ {key}:{value} ");
                        break;
                    }
            }
        }

        #endregion


        #region 示例样例

        public static InitCharacter GetSample(bool cat = false)
        {
            var I_C = new InitCharacter(4271, 13882, 21356, 1310.5, 0.5213, 1.8659, 0.4706, 3530, 9532, 441,
                had_BigFM_hat: false, had_BigFM_jacket: false, name: "初始实例面板");
            if (cat)
            {
                I_C.Cat();
            }

            return I_C;
        }

        #endregion
    }
}