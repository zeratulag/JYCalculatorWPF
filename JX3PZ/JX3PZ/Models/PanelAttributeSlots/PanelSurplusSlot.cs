using JX3CalculatorShared.Globals;
using System.Collections.Generic;

namespace JX3PZ.Models
{
    public class PanelSurplusSlot : PanelAttributeSlot
    {
        public new const string Name = "SurplusValue";
        public const string Suffix = "Base";
        public double SurplusDamageCoef { get; protected set; }
        public double SurplusDamage { get; protected set; }

        public PanelSurplusSlot() :
            base(Name, Suffix)
        {
            BasePercentAddKey = $"at{Name}AddPercent";
        }

        public void CalcXFDamage(XinFaAttribute xf)
        {
            // 计算当前心法下的破招伤害
            SurplusDamageCoef = (StaticConst.CurrentLevelGlobalParams.Surplus * (1 + xf.PZCoef));
            SurplusDamage = Final * SurplusDamageCoef;
        }

        public override List<string> GetDescTips()
        {
            var res = base.GetDescTips();
            res.Add($"破招伤害 {SurplusDamage:F0}");
            return res;
        }
    }
}