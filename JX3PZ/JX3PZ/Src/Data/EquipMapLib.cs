using JX3CalculatorShared.Globals;
using JX3PZ.Globals;
using System;
using System.Collections.Immutable;
using static JX3CalculatorShared.Utils.ImportTool;

namespace JX3PZ.Data
{
    public static class EquipMapLib
    {
        public static string Path;

        public static ImmutableDictionary<EquipSlotEnum, EquipMapItem> Equip { get; private set; }
        public static ImmutableDictionary<int, EquipMapItem> EquipSubType { get; private set; } // SubType到MapItem的字典
        public static ImmutableDictionary<EquipSubTypeEnum, EquipTypeEnum> SubTypeToEquipType { get; private set; }

        public static ImmutableDictionary<EquipSubTypeEnum, bool> HasBigFMSlot { get; private set; } // 每个部位是否有大附魔
        public static ImmutableDictionary<int, WeaponMapItem> Weapon { get; private set; }

        public static int MAX_SUB_TYPE { get; private set; } = 0; // SubType最大值，注意从0开始，所以总数需要+1
        public static int MAX_POSITION { get; private set; } = 0; // Position的最大值

        public static void Load(string path)
        {
            Path = path;
            LoadEquipMap();
            LoadWeaponMap();
        }

        public static void LoadEquipMap()
        {
            Equip = ReadSheetAsDict<EquipSlotEnum, EquipMapItem>(Path, "Equip", _ => _.Name);
            var subtype = ImmutableDictionary.CreateBuilder<int, EquipMapItem>();
            var subtypeTotype = ImmutableDictionary.CreateBuilder<EquipSubTypeEnum, EquipTypeEnum>();
            var hasbigFM = ImmutableDictionary.CreateBuilder<EquipSubTypeEnum, bool>();
            foreach (var _ in Equip.Values)
            {
                _.Parse();
                if (!subtype.ContainsKey(_.SubType))
                {
                    subtype.Add(_.SubType, _);
                }

                if (!subtypeTotype.ContainsKey(_.SubTypeEnum))
                {
                    subtypeTotype.Add(_.SubTypeEnum, _.Type);
                    hasbigFM.Add(_.SubTypeEnum, _.Enchant);
                }

                MAX_SUB_TYPE = Math.Max(MAX_SUB_TYPE, _.SubType);
                MAX_POSITION = Math.Max(MAX_POSITION, _.Position);
            }

            EquipSubType = subtype.ToImmutable();
            SubTypeToEquipType = subtypeTotype.ToImmutable();
            HasBigFMSlot = hasbigFM.ToImmutable();
        }

        public static void LoadWeaponMap()
        {
            Weapon = ReadSheetAsDict<int, WeaponMapItem>(Path, "Weapon", _ => _.DetailType);
        }

        /// <summary>
        /// 基于Position获取对应的SubType
        /// </summary>
        /// <param name="pos">位置</param>
        /// <returns></returns>
        public static int PositionToSubType(int pos)
        {
            var _ = GetEquipMapItem(pos);
            return _.SubType;
        }

        public static EquipSubTypeEnum PositionToSubTypeEnum(int pos)
        {
            var _ = GetEquipMapItem(pos);
            return _.SubTypeEnum;
        }

        /// <summary>
        /// SubType到EquipType转换
        /// </summary>
        /// <param name="subType"></param>
        /// <returns></returns>
        public static EquipTypeEnum GetEquipTypeBySubType(EquipSubTypeEnum subType)
        {
            return SubTypeToEquipType[subType];
        }

        public static EquipMapItem GetEquipMapItem(int pos)
        {
            return Equip[(EquipSlotEnum)pos];
        }

        public static bool EquipSubTypeHasBigFMSlot(this EquipSubTypeEnum subType)
        {
            return HasBigFMSlot[subType];
        }

        public static EquipMapItem GetEquipMapItemBySubType(EquipSubTypeEnum subType)
        {
            return EquipSubType[(int)subType];
        }

        public static string GetShortLabel(this EquipMapItem map)
        {
            var res = map.Label;
            if (map.SubTypeEnum == EquipSubTypeEnum.PRIMARY_WEAPON)
            {
                res = "武器";
            }
            else
            {
                if (map.SubTypeEnum == EquipSubTypeEnum.SECONDARY_WEAPON)
                {
                    res = "暗器";
                }
            }

            return res;
        }
    }
}