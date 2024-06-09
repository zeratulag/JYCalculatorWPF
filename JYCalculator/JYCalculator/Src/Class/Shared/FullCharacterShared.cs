using CommunityToolkit.Mvvm.ComponentModel;
using Force.DeepCloner;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JYCalculator.Data;
using JYCalculator.Globals;
using MiniExcelLibs.Attributes;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace JYCalculator.Class
{
    public partial class FullCharacter : ObservableObject
    {
        #region 成员

        // 基础攻击，最终攻击，基础破防，最终破防，
        public double Base_AP { get; set; }
        public double Final_AP { get; set; }
        public double Base_OC { get; set; }
        public double Final_OC { get; set; }

        // 武器伤害，会心，会效，无双，破招，加速
        public double WP { get; set; }
        public double CT { get; set; }
        public double CF { get; set; }
        public double WS { get; set; }
        public double PZ { get; set; }
        public double HS { get; set; }

        // 攻击提升，破防提升
        public double AP_Percent { get; set; }
        public double OC_Percent { get; set; }

        // 无视防御A
        public double IgnoreA { get; set; }

        public double ExtraSP { get; set; } = 0; // 额外加速率（仅在大心无期间有）
        public bool Is_XW { get; private set; } = false; // 是否处于心无状态，
        public bool Has_Special_Buff { get; set; } = false; // 是否已经计算了自身神力弩心催寒buff
        public bool Had_BigFM_hat { get; set; } = false; // 是否已经包括帽子大附魔
        public bool Had_BigFM_jacket { get; set; } = false; // 是否已经包括上衣大附魔
        public double Normal_GCD;
        public double BigXW_GCD;
        public double GCD => Is_XW ? BigXW_GCD : Normal_GCD;
        public double Base_OC_Pct => Base_OC / XFStaticConst.fGP.OC; // 面板基础破防
        public double Final_OC_Pct => Final_OC / XFStaticConst.fGP.OC; // 面板最终破防
        public double HS_Pct => Math.Min(HS / XFStaticConst.fGP.HS, HasteBase.MAX_HS); // 面板加速值
        public double NPC_Coef = XFConsts.NPC_Coef; // 非侠士增伤
        [ExcelColumn(Index = 0)] public string Name { get; set; } = "F面板"; // 命名

        #endregion

        #region 构造

        private void PostConstructor()
        {
            PropertyChanged += OnPropertyChangedHandler;
            UpdateGCD();
        }

        public FullCharacter Copy()
        {
            return new FullCharacter(this);
        }

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

        #endregion

        #region 属性计算

        /// <summary>
        /// 区分内功和外功属性
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddSAttr(string key, double value)
        {
            //string key1 = key;
            //if (!key1.EndsWith("_DmgAdd"))
            //{
            //    key1 = key.RemovePrefix(XFAppStatic.TypePrefix);
            //}
            //_AddSAttr(key1, value);
            this.ProcessZAttr(key, value);
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

            var XWBuff = bigXW ? StaticXFData.DB.Buff.BigXW : StaticXFData.DB.Buff.XW;

            FullCharacter other = this.DeepClone();
            other.AddBaseBuff(XWBuff);
            //other.ProcessCT(XFStaticConst.XW.CT);
            //other.ProcessCF(XFStaticConst.XW.CF);
            //other.ExtraSP += XFStaticConst.XW.ExtraSP * bigXW.ToInt();
            other.AddSAttrDict(attrDict);

            if (AppStatic.XinFaTag == "TL")
            {
                other.ProcessAP_Percent(XFStaticConst.XW.AP_Percent);
            }

            other.Is_XW = true;
            other.Name = Name + "_爆发状态";
            return other;
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

        public void RemoveCharAttrCollection(AttrCollection charAttrs)
        {
            if (charAttrs == null) return;

            if (!charAttrs.IsEmptyOrNull && charAttrs.Simplified) // 仅仅允许添加已简化后的属性
            {
                RemoveSAttrDict(charAttrs.Values);
            }
        }

        public void RemoveNamedAttrs(NamedAttrs attrs)
        {
            // 移除属性
            RemoveCharAttrCollection(attrs.Attr);
        }

        public void AddBaseBuff(BaseBuff baseBuff)
        {
            // 增加基础BUFF
            AddCharAttrCollection(baseBuff.SCharAttrs);
        }

        public void AddZhenFa(ZhenFa zhen)
        {
            AddNamedAttrs(zhen.AttrsDesc);
        }

        public void RemoveZhenFa(ZhenFa zhen)
        {
            // 移除阵法属性（前提是必须开了这个阵）
            RemoveNamedAttrs(zhen.AttrsDesc);
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
            Normal_GCD = XFStaticConst.CurrentHaste.SKT(StaticConst.GCD, (int)HS, 0);
            BigXW_GCD = XFStaticConst.CurrentHaste.SKT(StaticConst.GCD, (int)HS, XFStaticConst.XW.ExtraSP);
        }

        #endregion

        #region 进阶计算

        [DoNotNotify] public double CT_Point => CT * XFStaticConst.fGP.CT; // 会心点数

        [DoNotNotify] public double WS_Point => WS * XFStaticConst.fGP.WS; // 无双点数

        [DoNotNotify] public double CTOC_Point => CT_Point + Base_OC; // 会破点数之和

        [DoNotNotify] public double WSPZ_Point => WS_Point + PZ; // 无双破招点数之和


        #endregion
    }
}