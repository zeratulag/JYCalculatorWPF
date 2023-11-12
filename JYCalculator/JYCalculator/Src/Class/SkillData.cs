using JYCalculator.Globals;
using System;
using System.Collections.Generic;

namespace JYCalculator.Class
{
    public partial class SkillData
    {
        #region 成员

        public double IgnoreB { get; set; }
        public double APCoef { get; set; }

        public double PZCoef { get; set; } = 0;

        #endregion

        #region 构造

        /// <summary>
        /// 复制构造
        /// </summary>
        /// <param name="old"></param>
        public SkillData(SkillData old) : base(old)
        {
            Info = old.Info;
            IgnoreB = old.IgnoreB;
            APCoef = old.APCoef;
            PZCoef = old.PZCoef;
        }


        /// <summary>
        /// 重置此技能信息，删除所有秘籍效果
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (Info != null)
            {
                APCoef = Info.AP_Coef;
                IgnoreB = Info.IgnoreB;
            }
        }

        /// <summary>
        /// 更新技能系数
        /// </summary>
        public override void UpdateCoef()
        {
            var ap_Coef = GetAPCoef();
            APCoef = ap_Coef;
            UpdatePZCoef();
        }

        /// <summary>
        /// 更新破招系数
        /// </summary>
        public void UpdatePZCoef()
        {
            if (Info.IsP)
            {
                if (Name == "PZ_BaiYu")
                {
                    PZCoef = XFStaticConst.fGP.XPZ_BY;
                }
                else
                {
                    PZCoef = XFStaticConst.fGP.XPZ;
                }
            }
        }


        public override bool ApplyValueEffect(string key, double value)
        {
            var handled = base.ApplyValueEffect(key, value);
            if (!handled)
            {
                handled = true;
                switch (key)
                {
                    case "IgnoreB_P":
                    case "IgnoreB":
                    {
                        IgnoreB += value;
                        break;
                    }

                    default:
                    {
                        handled = false;
                        throw new ArgumentException($"未识别的key！ {key}");
                    }
                }
            }

            return handled;
        }

        public override bool ApplyOtherEffect(string key, List<object> value)
        {
            var handled = base.ApplyOtherEffect(key, value);
            if (!handled)
            {
                handled = true;
                switch (key)
                {
                    case "ZMWS_Add_Dmg":
                    {
                        break;
                    }

                    default:
                    {
                        handled = false;
                        throw new ArgumentException($"未识别的key！ {key}");
                    }
                }
            }

            return handled;
        }

        #endregion
    }
}