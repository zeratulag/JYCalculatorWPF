using JX3CalculatorShared.Globals;
using JX3PZ.Globals;

namespace JX3PZ.Data
{
    public class EquipMapItem
    {
        #region 自动生成

        public string Label { get; set; }
        public string Remark { get; set; }
        public int Position { get; set; } = -1;
        public int SubType { get; set; } = -1;
        public int TabType { get; set; } = -1;
        public string ShortDesc { get; set; }
        public int DiamondSlotCount { get; set; } = 0; // 镶嵌孔计数

        #endregion

        // 以下为手动指定类型
        public EquipSlotEnum Name { get; set; }
        public decimal ScoreCoef { get; set; }
        public EquipTypeEnum Type { get; set; }
        public bool Enhance { get; set; }
        public bool Enchant { get; set; }

        public int IconID { get; private set; } // 装备栏的图标
        public int MaxStrengthLevel { get; private set; } // 装备栏最大精炼等级

        #region 解析

        public EquipSubTypeEnum SubTypeEnum;

        #endregion

        public void Parse()
        {
            SubTypeEnum = (EquipSubTypeEnum)SubType;
            IconID = -(100 + SubType);

            MaxStrengthLevel = 6;
            if (SubTypeEnum == EquipSubTypeEnum.PRIMARY_WEAPON || SubTypeEnum == EquipSubTypeEnum.RING)
            {
                MaxStrengthLevel = 8;
            }
        }

    }

    public class WeaponMapItem
    {
        public int DetailType { get; set; } = -1;
        public string Weapon { get; set; }
        public double Speed { get; set; }
    }

    public class PlayerLevelData
    {
        // 人物初始属性
        public int atStrengthBase { get; set; }
        public int atAgilityBase { get; set; }
        public int atVitalityBase { get; set; }
        public int atSpiritBase { get; set; }
        public int atSpunkBase { get; set; }
        public int atMaxLifeBase { get; set; }
    }

}