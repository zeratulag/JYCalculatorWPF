namespace JX3CalculatorShared.Globals
{
    // 伤害类型
    public enum DamageTypeEnum
    {
        Physics, // 外功
        Magic // 内功
    }

    // 伤害类型
    public enum DamageSubTypeEnum
    {
        Physics, // 外功
        Magic, // 内功
        Lunar, // 阴性
        Solar, // 阳性
        Neutral, // 混元
        Poison, // 毒性
    }

    // 主属性
    public enum PrimaryTypeEnum
    {
        Vitality, // 体质
        Spirit, // 根骨
        Strength, // 力道
        Agility, // 身法
        Spunk, // 元气
    }

    // 物品品级
    public enum QualityEnum
    {
        None = -1,
        GREY = 0,
        WHITE = 1,
        GREEN = 2,
        BLUE = 3,
        PURPLE = 4,
        ORANGE = 5,
        INVALID = 6,
    }

    // 装备子类型
    public enum EquipSubTypeEnum
    {
        PRIMARY_WEAPON = 0, // 主武器
        SECONDARY_WEAPON = 1, // 副武器（暗器囊）
        JACKET = 2, // 上衣
        HAT = 3, // 帽子
        NECKLACE = 4,
        RING = 5, // 戒指
        BELT = 6, // 腰带
        PENDANT = 7, // 腰坠
        BOTTOMS = 8, // 下装
        SHOES = 9, // 鞋子
        WRIST = 10, // 护腕
    }

    public enum EquipSubTypeEnumDesc
    {
        武器 = 0,
        暗器 = 1,
        上衣 = 2,
        帽子 = 3,
        项链 = 4,
        戒指 = 5,
        腰带 = 6,
        腰坠 = 7,
        下装 = 8,
        鞋子 = 9,
        护腕 = 10,
    }

    public enum EquipSubTypeEnumShortDesc
    {
        弩 = 0,
        嚢 = 1,
        衣 = 2,
        帽 = 3,
        链 = 4,
        戒 = 5,
        腰 = 6,
        坠 = 7,
        裤 = 8,
        鞋 = 9,
        腕 = 10,
    }

    public enum EquipOptionType
    {
        Default = -1,
        WP,
        YZ
    }

    public static class EquipSubTypeTool
    {
        public static string ToDescString(this EquipSubTypeEnum typeEnum)
        {
            var res = (EquipSubTypeEnumDesc) typeEnum;
            return res.ToString();
        }

        public static string ToShortDescString(this EquipSubTypeEnum typeEnum)
        {
            var res = (EquipSubTypeEnumShortDesc) typeEnum;
            return res.ToString();
        }
    }

    public enum BuffTypeEnum
    {
        Default = -1,
        Buff_Self,
        Buff_Normal,
        DeBuff_Normal,
        Buff_Banquet,
        Buff_Extra,
        Buff_ExtraStack,
        Buff_ExtraTrigger,
        Buff_Special,
        Buff_Skill,
        Buff_Equip,
        Buff_EquipSpecialEffect,
    }

    public enum SkillDataTypeEnum
    {
        Default = -1,
        Normal, // 常规
        Channel, // 逆读条
        NormalChannel, // 表示计算技能攻击系数的时候需要加上考虑读条时间（夺魄）
        MultiChannel, // 多段逆读条（130修改了加速算法）
        DOT, // DOT
        NoneAP, // 不吃AP加成
        Physics, // 外功
        PZ, // 破招
        Exclude, // 排除 
        SuperCustom, // 固定伤害，只吃吃等级压制和易伤
        NormalDOT, // 穿心弩专用
    }

    public enum ItemDTTypeEnum
    {
        Default = -1,
        FoodSupport, // 食品辅助
        FoodEnhance, // 食品增强
        MedSupport, // 药品辅助
        MedEnhance, // 药品增强
        HomeWine, // 家园酿造
        HomeCook, // 家园烹饪
        HomeBalm, // 家园调香
        WeaponWhetstone, // 武器磨石
    }

    public enum RecipeTypeEnum
    {
        Default = -1,
        秘籍,
        奇穴,
        武器,
        套装,
        奇穴触发,
    }

    public enum BottomsFMTag
    {
        NotBottoms = -1, // 非下装部位
        None = 0, // 无下装大附魔
        CanJuan = 1, // 残卷
        YuJian = 2, // 玉简
    }

    public enum EquipSpecialEffectTypeEnum
    {
        None = 0,
        SECONDARY_WEAPON__SuperCustomDamage,
        WRIST__SuperCustomDamage,

        RING__Overcome,
        RING__CriticalStrike,
        RING__Surplus,
        BELT__Attribute,
        BELT__Random_Attribute,
        PENDANT__Overcome,
        PENDANT__CriticalPower,
        SHOES__CriticalStrike,
        SHOES__Overcome,
        BOTTOMS__Strain,

        HAT__AdaptivePower = 110,
        NECKLACE__CriticalStrike_To_CriticalPower = 120,
        NECKLACE__Overcome_To_AttackPower = 121,

        BOTTOMS__AdaptivePower = 200,

        // 计算顺序：
        // 1. 帽子：（快照）基于会心破防破招，修改会心破防破招
        // 2. 项链：（快照）基于会心破防修改会效攻击
        // 3. 下装：（实时）判断无双，修改会心破防
    }

    public enum EquipSpecialEffectBaseTypeEnum
    {
        None = 0,
        SuperCustomDamage,
        AdaptiveBuff,
        AddBuff,
        RandomBuff
    }

    // 事件触发类型
    public enum SkillEventTypeEnum
    {
        Default = 0,
        Cast,
        Hit,
        CriticalStrike
    }
}