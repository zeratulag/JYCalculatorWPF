namespace JX3PZ.Globals
{
    public enum EquipTypeEnum
    {
        armor = 7,
        trinket = 8,
        weapon = 6,
    }

    public enum WeaponAttributeTypeEnum
    {
        Melee,
        Range,
    }


    public enum EquipSlotEnum
    {
        HAT = 0,
        JACKET = 1,
        BELT = 2,
        WRIST = 3,
        BOTTOMS = 4,
        SHOES = 5,
        NECKLACE = 6,
        PENDANT = 7,
        RING_1 = 8,
        RING_2 = 9,
        SECONDARY_WEAPON = 10,
        PRIMARY_WEAPON = 11,
        TERTIARY_WEAPON = 12
    }

    public enum AttributeEntryTypeEnum
    {
        Default, // 默认，无特定类型
        EquipBase, // 装备白字基础属性，不可精炼（防具的白字防御，武器的攻速伤害），eg：外功防御等级提高225
        EquipBasicMagic, // 装备基础魔法属性，可精炼，eg：体质+2773 (+208)
        EquipExtraMagic, // 装备绿字魔法属性，可精炼，eg：外功攻击提高872 (+65)

        Diamond, // 五行石属性，eg：镶嵌孔：外功会心等级提高188
        Enhance, // 小附魔，eg：无双等级提高217
        BigFM, // 大附魔：eg：释放招式命中目标有20%几率使自身短时间内伤害获得不稳定的增幅（最终伤害有30%几率提高1%，有70%几率提高5%），该效果每30秒最多触发一次，并持续8秒。不在名剑大会中生效。
        Special, // 特效属性，eg：装备：命中后获得水·斩流气劲，自身提升外功攻击，可叠加十层，持续6秒。不可与该类其他气劲并存。
        Stone, // 五彩石属性
        Strength, // 精炼属性
        Set, // 套装属性

        XinFa, // 心法属性
        Buff, // Buff
    }


}