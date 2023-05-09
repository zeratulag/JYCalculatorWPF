using JX3CalculatorShared.Globals;
using JX3PZ.Models;


public class PanelAttackSlot : PanelSubTypeValueSlot
{
    public new static readonly string Suffix = "AttackPowerBase";

    public PanelAttackSlot(DamageSubTypeEnum subType, params string[] extraKey) :
        base(subType, keySuffix: Suffix, extraKey)
    {
        BasePercentAddKey = $"at{Name}AttackPowerPercent";
        if (DamageType == DamageTypeEnum.Magic) ExtraKey.Add($"atMagic{Suffix}");
        if (SubType == DamageSubTypeEnum.Solar || SubType == DamageSubTypeEnum.Lunar)
        {
            ExtraKey.Add($"atSolarAndLunar{Suffix}");
        }
    }
}