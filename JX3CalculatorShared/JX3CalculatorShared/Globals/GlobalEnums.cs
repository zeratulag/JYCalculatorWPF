namespace JX3CalculatorShared.Globals
{
    // 伤害类型
    public enum DamageTypeEnum
    {
        Physics,
        Magic
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
    }

    // 装备子类型
    public enum EquipSubTypeEnum
    {
        PRIMARY_WEAPON = 0,
        SECONDARY_WEAPON = 1,
        JACKET = 2,
        HAT = 3,
        NECKLACE = 4,
        RING = 5,
        BELT = 6,
        PENDANT = 7,
        BOTTOMS = 8,
        SHOES = 9,
        WRIST = 10,
    }

    public enum EquipSubTypeEnumDesc
    {
        近身武器 = 0,
        远程武器 = 1,
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
        Buff_Self,
        Buff_Normal,
        DeBuff_Normal,
        Buff_Banquet,
        Buff_Extra,
        Buff_ExtraStack,
        Buff_ExtraTrigger,
    }
    public enum SkillDataTypeEnum
    {
        Normal, // 常规
        Channel, // 逆读条
        DOT, // DOT
        NoneAP, // 不吃AP加成
        Physics, // 外功
        PZ, // 破招
        Exclude, // 排除 
    }

    public enum ItemDTTypeEnum
    {
        FoodSupport, // 食品辅助
        FoodEnhance, // 食品增强
        MedSupport, // 药品辅助
        MedEnhance, // 药品增强
        HomeWine, // 家园酿造
        HomeCook, // 家园烹饪
        WeaponWhetstone, // 武器磨石
    }

    public enum RecipeTypeEnum
    {
        秘籍,
        奇穴,
        武器,
        套装,
    }

    public enum BottomsFMTag
    {
        NotBottoms = -1, // 非下装部位
        None = 0, // 无下装大附魔
        CanJuan = 1, // 残卷
        YuJian = 2, // 玉简
    }

}