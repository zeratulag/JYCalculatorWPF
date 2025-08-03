namespace JX3CalculatorShared.Globals
{
    public enum ZAttributeType
    {
        None = 0, // 默认

        AllDamageAdd,
        PhysicsDamageAdd,

        BaseStrength, // 基础属性
        FinalStrength,
        StrengthPercent,

        BaseAgility,
        AllBasePotent,

        PhysicsCriticalPowerRate, // 会效值
        PhysicsCriticalPower, // 会效等级

        PhysicsCriticalStrikeValue, // 会心值
        PhysicsCriticalStrike, // 会心等级
        PhysicsCriticalStrikeRate, // 会心率

        Haste,
        ExtraHaste,

        PhysicsFinalAttackPower,
        PhysicsBaseAttackPower,
        PhysicsAttackPowerPercent,

        PhysicsBaseOvercome,
        PhysicsOvercomePercent,

        BaseSurplus,
        BaseWeaponDamage,

        StrainRate,
        BaseStrain,
        FinalStrain,
        StrainPercent,


        MagicBaseAttackPower,

        AllShieldIgnore,

        MagicBaseShield, // 目标防御属性
        PhysicsBaseShield,
        MagicDamageCoefficient,
        MagicShieldPercent,
        PhysicsFinalShield,
        MagicFinalShield,
        PhysicsDamageCoefficient,
        PhysicsShieldPercent,
    }
}