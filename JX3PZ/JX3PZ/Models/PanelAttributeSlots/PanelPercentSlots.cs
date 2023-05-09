using System.Collections.Generic;
using System.Windows.Documents;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;

namespace JX3PZ.Models
{
    public class PanelHasteSlot : PanelPercentAttributeSlot
    {
        public new const string Name = "Haste";
        public new const string Suffix = "Base";

        public new static readonly double PointCoef = StaticConst.fGP.HS;
        public PanelHasteSlot() : base(Name, Suffix, PointCoef, PointToPercentConvertTypeEnum.Linear)
        {
            PercentKey = $"{BasePointKey}PercentAdd";
            PercentAttribute = AttributeID.Get(PercentKey);
        }
    }

    public class PanelStrainSlot : PanelPercentAttributeSlot
    {
        // 无双
        public new const string Name = "Strain";
        public new const string Suffix = "Base";

        public new static readonly double PointCoef = StaticConst.fGP.WS;
        public PanelStrainSlot(): base(Name, Suffix, PointCoef, PointToPercentConvertTypeEnum.Linear)
        {
            BasePointPercentAddKey = $"at{Name}Percent";
            PercentKey = $"at{Name}Rate";
            PercentAttribute = AttributeID.Get(PercentKey);
        }
    }

    public class PanelPhysicsShieldSlot : PanelPercentAttributeSlot
    {
        // 外防
        public new const string Name = "PhysicsShield";
        public new const string Suffix = "Base";

        public new static readonly double PointCoef = StaticConst.fGP.PDef;

        public PanelPhysicsShieldSlot() : base(Name, Suffix, PointCoef, PointToPercentConvertTypeEnum.NonLinear)
        {
            BasePointPercentAddKey = $"at{Name}Percent";
            AdditionalPointKey = $"at{Name}Additional";
        }

        public override List<string> GetDescTips()
        {
            var res = base.GetDescTips();
            res.Add($"受到的外功伤害降低 {Final:P2}，最多可降低 {StaticConst.DefMax:P2}");
            return res;
        }
    }

    public class PanelMagicShieldSlot : PanelPercentAttributeSlot
    {
        // 内防
        public new const string Name = "MagicShield";
        public new const string Suffix = "";

        public new static readonly double PointCoef = StaticConst.fGP.MDef;

        public PanelMagicShieldSlot() : base(Name, Suffix, PointCoef, PointToPercentConvertTypeEnum.NonLinear)
        {
            BasePointPercentAddKey = $"at{Name}Percent";
            AdditionalPointKey = $"at{Name}Additional";
        }

        public override List<string> GetDescTips()
        {
            var res = base.GetDescTips();
            res.Add($"受到的内功伤害降低 {Final:P2}，最多可降低 {StaticConst.DefMax:P2}");
            return res;
        }
    }

    public class PanelDecriticalDamageSlot : PanelPercentAttributeSlot
    {
        // 化劲
        public new const string Name = "DecriticalDamagePower";
        public new const string Suffix = "Base";

        public new static readonly double PointCoef = StaticConst.fGP.HJ;

        public PanelDecriticalDamageSlot() : base(Name, Suffix, PointCoef, PointToPercentConvertTypeEnum.NonLinear)
        {
            BasePointPercentAddKey = $"at{Name}Percent";
            PercentKey = $"{BasePointKey}KiloNumRate";
            PercentDenominator = 1000.0;
        }

        public override List<string> GetDescTips()
        {
            var res = base.GetDescTips();
            res.Add($"玩家对你的所有伤害降低 {Final:P2}，最多可降低 {StaticConst.HJMax:P2}");
            return res;
        }
    }

    public class PanelToughnessSlot : PanelPercentAttributeSlot
    {
        // 御劲
        public new const string Name = "Toughness";
        public new const string Suffix = "Base";

        public new static readonly double PointCoef = StaticConst.fGP.YJ;
        public static readonly double CFPointCoef = StaticConst.fGP.YJCF;
        public double FinalCF => Final * PointCoef / CFPointCoef;

        public PanelToughnessSlot() : base(Name, Suffix, PointCoef, PointToPercentConvertTypeEnum.Linear)
        {
            PercentKey = $"{BasePointKey}Rate";
            BasePointPercentAddKey = $"at{Name}Percent";
            PercentDenominator = 10000.0;
        }

        public override List<string> GetDescTips()
        {
            var res = base.GetDescTips();
            res.Add($"被目标会心攻击的几率降低 {Final:P2}，被目标会心攻击时受到的额外伤害降低 {FinalCF:P2}");
            return res;
        }

    }
}