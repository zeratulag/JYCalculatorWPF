using JYCalculator.Data;
using JYCalculator.Models;
using JYCalculator.Src;
using System.Collections.Generic;

namespace JYCalculator.Class.SkillCount
{
    public class BaiBuNingXingSkillCountItem : JYSkillCountItem
    {
        public BaiBuNingXingSkillCountItem(AbilitySkillNumItem item, QiXueConfigModel qiXue) : base(item, qiXue)
        {
            百步凝形 = true;
            SetNormalZhuiMing();
        }

        public override Dictionary<string, double> ToDict()
        {
            var res = base.ToDict();
            //var zmDict = MakeNJZMDict();
            res[nameof(DP_LaoJia)] = DP_LaoJia;
            res[nameof(DP)] = DP;
            return res;
        }

        public void SetNormalZhuiMing()
        {
            ZM = ZM_SF; // 扣掉起手的追命
            ZM_SF = 0;
            if (!_XW)
            {
                ZM -= 1; // 扣掉起手的那个长读条追命，后续在 DPSKernelShell.FixNingXingZhuiMing 补回来
            }
        }

        public void AttachBaiBuChuanYang(bool targetAllWaysFullHp)
        {
            double coef = targetAllWaysFullHp ? 1 : 0.3;
            ZM_BBCY = ZM * coef;
        }

        public void DoPreWork()
        {
        }
    }
}