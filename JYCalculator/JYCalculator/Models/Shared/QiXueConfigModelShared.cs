using JX3CalculatorShared.Models;
using JX3CalculatorShared.Utils;
using JYCalculator.Data;
using JYCalculator.Globals;
using JYCalculator.ViewModels;
using System.Linq;
using JYCalculator.Utils;

namespace JYCalculator.Models
{
    public partial class QiXueConfigModel : QiXueConfigModelBase
    {
        #region 成员

        public bool 秋风散影 { get; private set; }
        public bool 聚精凝神 { get; private set; }

        #endregion

        #region 构建

        public void UpdateInput(QiXueConfigViewModel vm)
        {
            QiXueNames = vm.ItemNames;
            QiXueNamesSet = QiXueNames.ToHashSet();
            Calc();
        }

        #endregion

        /// <summary>
        /// 计算心无相关属性
        /// </summary>
        public void GetXW()
        {
            XWCD = 90;
            XWDuration = 10 + 5.0 * 聚精凝神.ToInt();
            XWExtraSP = XFStaticConst.XW.ExtraSP * 聚精凝神.ToInt();
            NormalDuration = XWCD - XWDuration;
        }

        public void GetRecipes()
        {
            var dict = StaticXFData.DB.Recipe.OtherAssociate;

            AssociateKeys.Clear();
            OtherRecipes.Clear();

            foreach (var name in QiXueNames)
            {
                var key = $"奇穴-{name}";
                if (dict.ContainsKey(key))
                {
                    AssociateKeys.Add(key);
                    OtherRecipes.AddRange(dict[key]);
                }
            }
        }

        public void GetSkillEffects()
        {
            SkillModifiers = StaticXFData.DB.SkillModifier.GetQiXueMods(QiXueNamesSet);
        }

        public void GetSkillEvents()
        {
            SkillEvents = StaticXFData.DB.SkillInfo.GetQiXueEvents(QiXueNamesSet);
        }

        /// <summary>
        /// 计算有哪些激活的个人Buff
        /// </summary>
        public void GetSelfBuffNames()
        {
            SelfBuffNames = StaticXFData.DB.Buff.GetQiXueBuffs(QiXueNamesSet);
        }

        public void GetIsSupport()
        {
            IsSupport = this.IsSupported(StaticXFData.Data.Setting);
        }

    }
}