using System.Collections.Generic;
using System.Collections.Immutable;
using HandyControl.Tools.Extension;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;

namespace JX3PZ.Models
{
    public class CharacterPanelCriticalStrike: CharacterPanelBase<DamageSubTypeEnum, PanelCriticalStrikeSlot>
    {
        public PanelCriticalStrikeSlot Physics { get; } // 外功
        public PanelCriticalStrikeSlot Lunar { get; } // 阴性
        public PanelCriticalStrikeSlot Solar { get; } // 阳性
        public PanelCriticalStrikeSlot Neutral { get; } // 混元
        public PanelCriticalStrikeSlot Poison { get; } // 毒性

        public CharacterPanelCriticalStrike()
        {
            Physics = new PanelCriticalStrikeSlot(DamageSubTypeEnum.Physics);
            Lunar = new PanelCriticalStrikeSlot(DamageSubTypeEnum.Lunar);
            Solar = new PanelCriticalStrikeSlot(DamageSubTypeEnum.Solar);
            Neutral = new PanelCriticalStrikeSlot(DamageSubTypeEnum.Neutral);
            Poison = new PanelCriticalStrikeSlot(DamageSubTypeEnum.Poison);

            var dict = new Dictionary<DamageSubTypeEnum, PanelCriticalStrikeSlot>()
            {
                {DamageSubTypeEnum.Physics, Physics},
                {DamageSubTypeEnum.Lunar, Lunar},
                {DamageSubTypeEnum.Solar, Solar},
                {DamageSubTypeEnum.Neutral, Neutral},
                {DamageSubTypeEnum.Poison, Poison},
            };
            Dict.Merge(dict);
        }

    }
}