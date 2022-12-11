using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Force.DeepCloner;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JYCalculator.Class;
using JYCalculator.Globals;
using JYCalculator.Src.DB;
using MiniExcelLibs.Attributes;
using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.Utils;
using PropertyChanged;
using static JYCalculator.Globals.JYStaticData;

namespace JYCalculator.Src.Class
{
    public class FullCharacter : ObservableObject
    {
        #region 成员

        // 基础力道，最终力道，基础攻击，最终攻击，基础破防，最终破防，
        public double Base_L { get; private set; }
        public double Final_L { get; private set; }
        public double Base_AP { get; private set; }
        public double Final_AP { get; private set; }
        public double Base_OC { get; private set; }
        public double Final_OC { get; private set; }

        // 武器伤害，会心，会效，无双，破招，加速，
        public double WP { get; private set; }
        public double CT { get; private set; }
        public double CF { get; private set; }
        public double WS { get; private set; }
        public double PZ { get; private set; }
        public double HS { get; private set; }


        // 力道提升，攻击提升，破防提升，
        public double L_Percent { get; private set; }
        public double AP_Percent { get; private set; }
        public double OC_Percent { get; private set; }


        // 无视防御A，伤害提高
        public double IgnoreA { get; private set; }
        public double DmgAdd { get; private set; } // 外功伤害提高

        public double ExtraSP { get; private set; } = 0; // 额外加速率（仅在大心无期间有）
        public bool Is_XW { get; private set; } = false; // 是否处于心无状态，
        public bool Has_Special_Buff { get; set; } = false; // 是否已经计算了自身神力弩心催寒buff
        public bool Had_BigFM_hat { get; set; } = false; // 是否已经包括帽子大附魔
        public bool Had_BigFM_jacket { get; set; } = false; // 是否已经包括上衣大附魔

        public double Normal_GCD;
        public double BigXW_GCD;

        public double GCD => Is_XW ? BigXW_GCD : Normal_GCD;

        public double Base_OC_Pct => Base_OC / fGP.OC; // 面板基础破防
        public double Final_OC_Pct => Final_OC / fGP.OC; // 面板最终破防
        public double HS_Pct => Math.Min(HS / fGP.HS, HasteBase.MAX_HS); // 面板加速值

        public double NPC_Coef = JYConsts.NPC_Coef; // 非侠士增伤

        [ExcelColumn(Index = 0)] public string Name { get; set; } = "F面板"; // 命名

        #endregion

        #region 构造

        private void PostConstructor()
        {
            PropertyChanged += OnPropertyChangedHandler;
            UpdateGCD();
        }

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
        public FullCharacter Copy()
        {
            return new FullCharacter(this);
        }

        // 复制构造，更改名称
        public FullCharacter Copy(string name)
        {
            var res = new FullCharacter(this)
            {
                Name = name
            };
            return res;
        }

        #endregion

        /// <summary>
        /// 同步快照面板
        /// </summary>
        /// <param name="xwCharacter"></param>
        public void SnapShotFromBurst(FullCharacter xwCharacter)
        {
            if (!xwCharacter.Is_XW)
            {
                throw new ArgumentException("必须是爆发期面板！");
            }

            Base_AP = xwCharacter.Base_AP;
            AP_Percent = xwCharacter.AP_Percent;
            Final_AP = xwCharacter.Final_AP;

            CT = xwCharacter.CT;
            CF = xwCharacter.CF;
            WS = xwCharacter.WS;

            ExtraSP = xwCharacter.ExtraSP;
            Name += "心无快照";
        }

        public FullCharacter GetBurstSnapShot(FullCharacter xwCharacter)
        {
            var other = new FullCharacter(this);
            other.SnapShotFromBurst(xwCharacter);
            return other;
        }


        #region 显示

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
            Add_CT_Point(value * JYConsts.CT_PER_S);
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
            Add_Base_AP(value * JYConsts.AP_PER_L);
            Add_Base_OC(value * JYConsts.OC_PER_L);
            Add_Final_AP(value * JYConsts.F_AP_PER_L);
            Add_CT_Point(value * JYConsts.CT_PER_L);
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
                    if (!JYDataBase.UselessAttrs.Contains(key))
                    {
                        Trace.WriteLine($"未知的属性！\t{key}\t{value} ");
                    }

                    break;
                }
            }
        }

        #endregion

        #region 属性计算

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

        public void AddSAtKVP(KeyValuePair<string, double> kvp)
        {
            AddSAttr(kvp.Key, kvp.Value);
        }

        public void AddSAttrDict(IDictionary<string, double> dict)
        {
            if (dict != null)
            {
                foreach (var kvp in dict)
                {
                    AddSAtKVP(kvp);
                }
            }
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


        public void AddCharAttrCollection(AttrCollection charAttrs)
        {
            if (charAttrs == null) return;

            if (!charAttrs.IsEmptyOrNull && charAttrs.Simplified) // 仅仅允许添加已简化后的属性
            {
                AddSAttrDict(charAttrs.Values);
            }
        }

        public void AddNamedAttrs(NamedAttrs attrs)
        {
            AddCharAttrCollection(attrs.Attr);
        }

        // 增加基础BUFF
        public void AddBaseBuff(BaseBuff baseBuff)
        {
            AddCharAttrCollection(baseBuff.SCharAttrs);
        }


        ///// <summary>
        ///// 计算加上增益（神力，弩心，催寒）之后的面板
        ///// </summary>
        ///// <param name="spcover">描述神力SL，弩心NX，催寒CH覆盖率的类</param>
        ///// <exception cref="ArgumentException">已添加过特殊增益!</exception>
        //public void AddSpecialBuff(SpecialBuffCover spcover)
        //{
        //    if (Has_Special_Buff)
        //        throw new ArgumentException("已添加过特殊增益，无法重复添加！");

        //    Add_CT(GlobalConsts.SL_CT * spcover.SL);
        //    Add_CF(GlobalConsts.SL_CF * spcover.SL);

        //    Add_OC_Percent(TLConsts.NX_OC_Percent * spcover.NX);
        //    Add_IgnoreA(TLConsts.CH_IgnoreA * spcover.CH);

        //    Has_Special_Buff = true;
        //}

        /// <summary>
        /// 进入心无杨威爆发状态。注意：仅当没有结算神力弩心催寒时才允许进入心无态
        /// </summary>
        /// <param name="bigXW">是否为大心无[聚精凝神]</param>
        /// <param name="attrDict">为心无状态添加的额外属性，通常用来表示腰坠破防</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public FullCharacter ToBurst(bool bigXW = true, Dictionary<string, double> attrDict = null)
        {
            if (Is_XW)
            {
                throw new ArgumentException("已处于心无状态！");
            }

            if (Has_Special_Buff)
            {
                throw new ArgumentException("Cannot add CT！");
            }

            FullCharacter other = this.DeepClone();

            other.Add_CT(XWConsts.CT);
            other.Add_CF(XWConsts.CF);

            other.ExtraSP += XWConsts.ExtraSP * bigXW.ToInt();
            other.AddSAttrDict(attrDict);

            other.Is_XW = true;
            other.Name = Name + "_爆发状态";
            return other;
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 当属性发生改变时执行的函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(HS):
                {
                    UpdateGCD();
                    break;
                }
            }
        }


        /// <summary>
        /// 计算GCD
        /// </summary>
        protected void UpdateGCD()
        {
            Normal_GCD = CurrentHaste.SKT(StaticData.GCD, (int) HS, 0);
            BigXW_GCD = CurrentHaste.SKT(StaticData.GCD, (int) HS, XWConsts.ExtraSP);
        }

        #endregion

        #region 进阶计算

        [DoNotNotify]
        public double CT_Point => CT * fGP.CT; // 会心点数

        [DoNotNotify]
        public double WS_Point => WS * fGP.WS; // 无双点数

        [DoNotNotify]
        public double CTOC_Point => CT_Point + Base_OC; // 会破点数之和

        [DoNotNotify]
        public double WSPZ_Point => WS_Point + PZ; // 无双破招点数之和


        /// <summary>
        /// 在会破属性之和保持不变的情况下，转移部分会心点数到破防
        /// </summary>
        /// <param name="value">点数</param>
        public void TransCTToOC(double value)
        {
            Add_CT_Point(-value);
            Add_Base_OC(value);
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
            var delta = CT_Point - ct * fGP.CT;
            TransCTToOC(delta);
        }

        /// <summary>
        /// 在无招属性之和保持不变的情况下，重新设置面板无双百分比
        /// </summary>
        /// <param name="ws">目标无双百分比</param>
        public void Reset_WS(double ws)
        {
            var delta = WS_Point - ws * fGP.WS;
            TransWSToPZ(delta);
        }

        #endregion

    }
}

namespace JYCalculator.Src
{
    public struct SpecialBuffCover
    {
        public readonly double SL, NX, CH;

        /// <summary>
        /// 描述自身特殊buff覆盖率的类
        /// </summary>
        /// <param name="sl">神力覆盖率</param>
        /// <param name="nx">弩心覆盖率</param>
        /// <param name="ch">催寒覆盖率</param>
        public SpecialBuffCover(double sl, double nx, double ch)
        {
            SL = sl;
            NX = nx;
            CH = ch;
        }
    }
}