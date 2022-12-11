using System;
using System.Collections.Generic;
using System.Diagnostics;
using JX3CalculatorShared.Class;
using JYCalculator.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Utils;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Globals;
using Newtonsoft.Json;
using PropertyChanged;
using static JYCalculator.Globals.JYStaticData;

namespace JYCalculator.Src.Class
{
    [ToString]
    [JsonObject(MemberSerialization.OptOut)]
    public class InitCharacter : AbsViewModel, ICatsable
    {
        #region 成员

        // 力道, 基础攻击，最终攻击, 武伤,
        // 会心, 会效, 无双, 破招, 破防, 加速,
        public double L { get; set; }
        public double Base_AP { get; set; }
        public double Final_AP { get; set; }
        public double WP { get; set; }
        public double CT { get; set; }
        public double CF { get; set; }
        public double WS { get; set; }
        public double PZ { get; set; }
        public double OC { get; set; }
        public double HS { get; set; }

        public bool Had_BigFM_hat { get; set; } = false; // 是否已经包括帽子大附魔
        public bool Had_BigFM_jacket { get; set; } = false; // 是否已经包括上衣大附魔

        [JsonIgnore] public double HSPct => Math.Min(HS / fGP.HS, HasteBase.MAX_HS); // 面板加速值

        [JsonIgnore] public double OCPct => OC / fGP.OC; // 面板破防值

        [JsonIgnore] public string Name { get; set; }

        #endregion

        #region 构造

        // 除了HSPct和OCPct之外的所有变量都是输入变量
        public InitCharacter(string name = "") : base(InputPropertyNameType.All)
        {
            ExcludePropertyNames = new HashSet<string>() {nameof(HSPct), nameof(OCPct)};
            Name = name;
        }

        public InitCharacter()
        {
        }


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

        public InitCharacter Copy()
        {
            return new InitCharacter(this);
        }

        #endregion


        public InitCharacter(JBPZPanel panel) : base()
        {
            _UpdateFromJBPanel(panel);
        }

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

        public void LoadFromJBPanel(JBPZPanel panel)
        {
            ActionUpdateOnce(_UpdateFromJBPanel, panel);
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

        public void LoadFromIChar(InitCharacter ichar)
        {
            ActionUpdateOnce(_UpdateFromIChar, ichar);
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

        public string ToStr()
        {
            var strL = GetCatStrList();
            return strL.StrJoin("\n");
        }

        public void Cat()
        {
            var str = ToStr();
            str.Cat();
        }

        #endregion

        #region 方法

        public FullCharacter ToFullCharacter(string name = "")
        {
            var res = new FullCharacter(this);
            if (name != "")
            {
                res.Name = name;
            }

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
            Add_CT_Point(value * JYConsts.CT_PER_S);
        }

        public void Add_L(double value) // 最终力道
        {
            L += value;
            Add_Base_AP(value * JYConsts.AP_PER_L);
            Add_OC(value * JYConsts.OC_PER_L);

            Add_Final_AP(value * JYConsts.F_AP_PER_L);
            Add_CT_Point(value * JYConsts.CT_PER_L);
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
                    Trace.WriteLine($"未知的属性！ {key}:{value} ");
                    break;
                }
            }
        }

        /// <summary>
        /// 区分内功和外功属性
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddSAttr(string key, double value)
        {
            string key1 = key;
            if (key.StartsWith("P_") && !key.EndsWith("_DmgAdd")) // 攻击和破防统一去除前缀
            {
                key1 = key.Substring(2);
            }

            _AddSAttr(key1, value);
        }

        /// <summary>
        /// 减少属性
        /// </summary>
        /// <param name="kvp"></param>
        public void RemoveSAtKVP(KeyValuePair<string, double> kvp)
        {
            AddSAttr(kvp.Key, -kvp.Value);
        }

        public void RemoveSAttrDict(IDictionary<string, double> dict)
        {
            if (dict != null)
            {
                foreach (var kvp in dict)
                {
                    RemoveSAtKVP(kvp);
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


        protected override void _Update()
        {
        }

        protected override void _Load<TSave>(TSave sav)
        {
        }

        protected override void _RefreshCommands()
        {
        }


        #region 进阶计算

        [DoNotNotify] public double CTOC_PointSum => CT * fGP.CT + OC; // 会破点数之和

        [DoNotNotify] public double WSPZ_PointSum => fGP.WS * WS + PZ; // 无双破招点数之和


        /// <summary>
        /// 在会破属性之和保持不变的情况下，转移部分会心点数到破防
        /// </summary>
        /// <param name="value">点数</param>
        public void TransCTToOC(double value)
        {
            Add_CT_Point(-value);
            Add_OC(value);
        }

        /// <summary>
        /// 在无招属性之和保持不变的情况下，转移部分无双点数到破招
        /// </summary>
        /// <param name="value">点数</param>
        public void TransWSToPZ(double value)
        {
            Add_WS_Point(-value);
            Add_PZ(value);
        }

        /// <summary>
        /// 在会破属性之和保持不变的情况下，重新设置面板会心百分比
        /// </summary>
        /// <param name="ct">目标会心百分比</param>
        public void Reset_CT(double ct)
        {
            var delta = CT * fGP.CT - ct * fGP.CT;
            TransCTToOC(delta);
        }

        /// <summary>
        /// 在无招属性之和保持不变的情况下，重新设置面板无双百分比
        /// </summary>
        /// <param name="ws">目标无双百分比</param>
        public void Reset_WS(double ws)
        {
            var delta = WS * fGP.WS - ws * fGP.WS;
            TransWSToPZ(delta);
        }


        #endregion
    }
}