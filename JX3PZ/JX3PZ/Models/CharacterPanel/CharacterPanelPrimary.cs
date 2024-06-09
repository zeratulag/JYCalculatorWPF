using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JX3PZ.Data;
using System.Collections.Generic;

namespace JX3PZ.Models
{
    public class CharacterPanelPrimary : CharacterPanelBase<PrimaryTypeEnum, PanelPrimarySlot>
    {
        public PanelPrimarySlot Vitality { get; } // 体质
        public PanelPrimarySlot Spirit { get; } // 根骨
        public PanelPrimarySlot Strength { get; } // 力道
        public PanelPrimarySlot Agility { get; } // 身法
        public PanelPrimarySlot Spunk { get; } // 元气

        public CharacterPanelPrimary()
        {
            Vitality = new PanelPrimarySlot(PrimaryTypeEnum.Vitality);
            Spirit = new PanelPrimarySlot(PrimaryTypeEnum.Spirit);
            Strength = new PanelPrimarySlot(PrimaryTypeEnum.Strength);
            Agility = new PanelPrimarySlot(PrimaryTypeEnum.Agility);
            Spunk = new PanelPrimarySlot(PrimaryTypeEnum.Spunk);

            var dict = new Dictionary<PrimaryTypeEnum, PanelPrimarySlot>()
            {
                {PrimaryTypeEnum.Vitality, Vitality},
                {PrimaryTypeEnum.Spirit, Spirit},
                {PrimaryTypeEnum.Strength, Strength},
                {PrimaryTypeEnum.Agility, Agility},
                {PrimaryTypeEnum.Spunk, Spunk},
            };
            Dict.Merge(dict);
        }


        public void ApplyLevelData(PlayerLevelData levelData)
        {
            // 应用玩家初始主属性
            Vitality.AddBase(levelData.atVitalityBase);
            Spirit.AddBase(levelData.atSpiritBase);
            Strength.AddBase(levelData.atStrengthBase);
            Agility.AddBase(levelData.atAgilityBase);
            Spunk.AddBase(levelData.atSpunkBase);
        }

        public Dictionary<string, int> GetSystemPrimaryAttributeValues()
        {
            var res = new Dictionary<string, int>(10);
            foreach (var kvp in Dict)
            {
                var p = kvp.Value.GetSystemPrimaryAttributeValues();
                res.Merge(p);
            }
            return res;
        }

    }
}