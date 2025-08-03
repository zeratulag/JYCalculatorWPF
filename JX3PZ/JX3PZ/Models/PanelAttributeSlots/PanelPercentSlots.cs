using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using System.Collections.Generic;

namespace JX3PZ.Models
{
    public class PanelHasteSlot : PanelPercentAttributeSlot
    {
        public new const string Name = "Haste";
        public const string Suffix = "Base";

        public static readonly double PointCoef = StaticConst.CurrentLevelGlobalParams.Haste;
        public PanelHasteSlot() : base(Name, Suffix, PointCoef, PointToPercentConvertTypeEnum.Linear)
        {
            PercentKey = $"{BasePointKey}PercentAdd";
            PercentAttribute = KAttributeID.Get(PercentKey);
        }
    }

    public class PanelStrainSlot : PanelPercentAttributeSlot
    {
        // 无双
        public new const string Name = "Strain";
        public const string Suffix = "Base";

        public static readonly double PointCoef = StaticConst.CurrentLevelGlobalParams.Strain;
        public PanelStrainSlot() : base(Name, Suffix, PointCoef, PointToPercentConvertTypeEnum.Linear)
        {
            BasePointPercentAddKey = $"at{Name}Percent";
            PercentKey = $"at{Name}Rate";
            PercentAttribute = KAttributeID.Get(PercentKey);
        }
    }

    public class PanelPhysicsShieldSlot : PanelPercentAttributeSlot
    {
        // 外防
        public new const string Name = "PhysicsShield";
        public const string Suffix = "Base";

        public static readonly double PointCoef = StaticConst.CurrentLevelGlobalParams.PhysicsShield;

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
        public const string Suffix = "";

        public static readonly double PointCoef = StaticConst.CurrentLevelGlobalParams.MagicShield;

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
        public const string Suffix = "Base";

        public static readonly double PointCoef = StaticConst.CurrentLevelGlobalParams.DefPlayerDamage;

        public PanelDecriticalDamageSlot() : base(Name, Suffix, PointCoef, PointToPercentConvertTypeEnum.NonLinear)
        {
            BasePointPercentAddKey = $"at{Name}Percent";
            PercentKey = $"{BasePointKey}KiloNumRate";
            PercentDenominator = 1024.0;
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
        public const string Suffix = "Base";

        public static readonly double PointCoef = StaticConst.CurrentLevelGlobalParams.DefCriticalStrike;
        public static readonly double CFPointCoef = StaticConst.CurrentLevelGlobalParams.DefCriticalPower;
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