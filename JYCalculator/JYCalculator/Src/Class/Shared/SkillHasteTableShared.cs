using CommunityToolkit.Mvvm.ComponentModel;
using JYCalculator.Src;
using System.Collections.Immutable;

namespace JYCalculator.Class
{
    public partial class SkillHasteTable : ObservableObject
    {
        #region 共享成员

        /// <summary>
        /// 一些已经固定的Item，不需要重复计算
        /// </summary>
        public readonly HasteTableItem GCD;

        public readonly SkillDataDF SkillDF;
        public readonly ImmutableDictionary<string, HasteTableItem> Dict;
        public int HSP { get; set; } // 加速
        public int XWExtraHSP { get; set; } // 心无额外加速

        #endregion

        /// <summary>
        /// 更新加速输入
        /// </summary>
        /// <param name="hs"></param>
        /// <param name="xwextrahsp"></param>
        public void UpdateHSP(int hs, int xwextrahsp)
        {
            HSP = hs;
            XWExtraHSP = xwextrahsp;
            Calc();
        }

        public void UpdateHSP(CalculatorShell shell)
        {
            UpdateHSP(shell.Haste, shell.XWExtraHaste);
        }

        public void Calc()
        {
            foreach (var KVP in Dict)
            {
                KVP.Value.CalcHaste(HSP, XWExtraHSP);
            }
        }
    }
}