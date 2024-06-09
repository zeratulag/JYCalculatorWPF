using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Utils;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Globals;
using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;

namespace JYCalculator.Class
{
    [ToString]
    [JsonObject(MemberSerialization.OptOut)]
    public partial class InitCharacter : AbsViewModel, ICatsable
    {
        #region 成员

        // 基础攻击，最终攻击, 武伤,
        // 会心, 会效, 无双, 破招, 破防, 加速,
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
        [JsonIgnore] public double HSPct => Math.Min(HS / XFStaticConst.fGP.HS, HasteBase.MAX_HS); // 面板加速值
        [JsonIgnore] public double OCPct => OC / XFStaticConst.fGP.OC; // 面板破防值
        [JsonIgnore] public string Name { get; set; }

        #endregion

        #region 构造

        public InitCharacter(string name = "") : base(InputPropertyNameType.All)
        {
            // 除了HSPct和OCPct之外的所有变量都是输入变量
            ExcludePropertyNames = new HashSet<string>() { nameof(HSPct), nameof(OCPct) };
            Name = name;
        }

        public InitCharacter()
        {
        }

        public InitCharacter Copy()
        {
            return new InitCharacter(this);
        }

        public InitCharacter(JBBBPZPanel panel) : base()
        {
            _UpdateFromJBPanel(panel.BaseJBBB);
        }

        #endregion

        public void LoadFromJBPanel(JBBBPZPanel panel)
        {
            ActionUpdateOnce(_UpdateFromJBPanel, panel.BaseJBBB);
        }

        public void LoadFromIChar(InitCharacter ichar)
        {
            ActionUpdateOnce(_UpdateFromIChar, ichar);
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

        #region 属性计算

        /// <summary>
        /// 区分内功和外功属性
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddSAttr(string key, double value)
        {
            //string key1 = key.RemovePrefix(XFAppStatic.TypePrefix);
            _AddSAttr(key, value);
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

        protected override void _Update()
        {
        }


        protected override void _RefreshCommands()
        {
        }

        #region 进阶计算

        [DoNotNotify][JsonIgnore] public double CTOC_PointSum => CT * XFStaticConst.fGP.CT + OC; // 会破点数之和
        [DoNotNotify][JsonIgnore] public double WSPZ_PointSum => XFStaticConst.fGP.WS * WS + PZ; // 无双破招点数之和

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
            var delta = CT * XFStaticConst.fGP.CT - ct * XFStaticConst.fGP.CT;
            TransCTToOC(delta);
        }

        /// <summary>
        /// 在无招属性之和保持不变的情况下，重新设置面板无双百分比
        /// </summary>
        /// <param name="ws">目标无双百分比</param>
        public void Reset_WS(double ws)
        {
            var delta = WS * XFStaticConst.fGP.WS - ws * XFStaticConst.fGP.WS;
            TransWSToPZ(delta);
        }

        #endregion
    }
}