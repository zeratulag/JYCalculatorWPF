using JX3CalculatorShared.Class;
using JX3PZ.Data;
using JX3PZ.Globals;
using JX3PZ.Src;
using JX3PZ.Views;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using static MingYiTools.MingYiEquipItemEntity;

namespace MingYiTools
{
    /// <summary>
    /// 装备/物品表
    /// </summary>
    [SugarTable("EquipItems")]
    public class MingYiEquipItem
    {
        [SugarColumn(ColumnName = "ownerkey", IsPrimaryKey = true)]
        public long OwnerKey { get; set; }

        [SugarColumn(ColumnName = "suitindex")]
        public int SuitIndex { get; set; }

        [SugarColumn(ColumnName = "boxtype", IsPrimaryKey = true)]
        public int BoxType { get; set; }

        [SugarColumn(ColumnName = "boxindex", IsPrimaryKey = true)]
        public int BoxIndex { get; set; }

        [SugarColumn(ColumnName = "itemid")]
        public int ItemId { get; set; }

        [SugarColumn(ColumnName = "tabtype")]
        public int TabType { get; set; }

        [SugarColumn(ColumnName = "tabindex")]
        public int TabIndex { get; set; }

        [SugarColumn(ColumnName = "tabsubindex")]
        public int TabSubIndex { get; set; }

        [SugarColumn(ColumnName = "stacknum")]
        public int StackNum { get; set; }

        [SugarColumn(ColumnName = "uiid")]
        public int UiId { get; set; }

        [SugarColumn(ColumnName = "strength")]
        public int Strength { get; set; }

        [SugarColumn(ColumnName = "durability")]
        public int Durability { get; set; }

        [SugarColumn(ColumnName = "diamond_enchant")]
        public string DiamondEnchant { get; set; }

        [SugarColumn(ColumnName = "fea_enchant")]
        public int FeaEnchant { get; set; }

        [SugarColumn(ColumnName = "permanent_enchant")]
        public int PermanentEnchant { get; set; }

        [SugarColumn(ColumnName = "temporary_enchant")]
        public int TemporaryEnchant { get; set; }

        [SugarColumn(ColumnName = "desc")]
        public string Desc { get; set; }

        [SugarColumn(ColumnName = "time")]
        public int Time { get; set; }

        [SugarColumn(ColumnName = "extra")]
        public string Extra { get; set; }
    }

    public class MingYiEquipItemEntity: MingYiEquipItem
    {
        public int[] DiamondEnchantArr { get; private set; }
        public EquipSlotEnum EquipSlot { get; private set; }
        public JBPZEquipSnapshot JBEquipSnapshot { get; private set; }
        public bool IsValidSlot { get; private set; }
        // 这里可以添加一些额外的属性或方法，用于处理EquipItem的数据
        public bool IsValidEquip { get; private set; }

        public MingYiEquipItemEntityViewModel ViewModel { get; }


        public MingYiEquipItemEntity(MingYiEquipItem item)
        {
            if (item == null) return;

            // 主键信息
            OwnerKey = item.OwnerKey;
            BoxType = item.BoxType;
            BoxIndex = item.BoxIndex;

            // 基础信息
            SuitIndex = item.SuitIndex;
            ItemId = item.ItemId;
            UiId = item.UiId;
            Time = item.Time;
            Desc = item.Desc;

            // 背包/位置信息
            TabType = item.TabType;
            TabIndex = item.TabIndex;
            TabSubIndex = item.TabSubIndex;
            StackNum = item.StackNum;

            // 属性/耐久
            Strength = item.Strength;
            Durability = item.Durability;

            // 附魔信息
            DiamondEnchant = item.DiamondEnchant;
            FeaEnchant = item.FeaEnchant;
            PermanentEnchant = item.PermanentEnchant;
            TemporaryEnchant = item.TemporaryEnchant;

            // 扩展数据
            Extra = item.Extra;
            ViewModel = new MingYiEquipItemEntityViewModel
            {
                Strength = Strength
            };
        }

        public void ParseDiamondEnchantArr()
        {
            DiamondEnchantArr = ParseIntArray(DiamondEnchant);
        }

        public static int[] ParseIntArray(string input)
        {
            // 1. 边界防御：处理 null、空字符串 或 特殊标记 "0.0"
            if (string.IsNullOrEmpty(input) || input == "0.0")
            {
                return Array.Empty<int>();
            }
            // 2. 标准 JSON 解析
            // 情况 [] -> 返回 int[0]
            // 情况 [0,0] -> 返回 int[] { 0, 0 }
            // 情况 [6225,6218] -> 返回 int[] { 6225, 6218 }
            try
            {
                return JsonConvert.DeserializeObject<int[]>(input) ?? Array.Empty<int>();
            }
            catch
            {
                // 如果数据库可能出现脏数据（如非法的 "0.0.0" 或 "error"），这里返回空数组保底
                return Array.Empty<int>();
            }
        }


        public void Parse()
        {
            ParseDiamondEnchantArr();
            ParseEquipSlot();
            MakeJBEquipSnapshot();
            ParseViewModel();
        }

        private void MakeJBEquipSnapshot()
        {
            JBEquipSnapshot = IsValidSlot ? new JBPZEquipSnapshot(this) : null;
        }

        private void ParseEquipSlot()
        {
            EquipSlot = EquipMapLib.GetEquipSlotEnumFromMingYiEquip(BoxIndex);
            IsValidSlot = EquipSlot != EquipSlotEnum.INVALID;
            IsValidEquip = IsValidSlot && ItemId > 0;
            ViewModel.EquipSlot = EquipSlot;
            ViewModel.IsValidEquip = IsValidEquip;
        }

        public int[] GetDiamondLevelArray()
        {
            // 解析五行石等级
            return DiamondEnchantArr.Select(StaticPzData.Data.DiamondDB.GetDiamondLevelFromEnchantID).ToArray();
        }

        public void ParseViewModel()
        {
            if (!IsValidSlot) return;
            ViewModel.ParseDesc(Desc);
            ViewModel.Parse();
        }
    }

    public class RichTextSegment
    {
        public string Text { get; set; }
        public int Font { get; }
        // 存储十六进制颜色，例如 "#FF0000"
        // 如果原数据未指定颜色，则为 null（由 UI 决定默认颜色，通常是白色或黑色）
        public string Color { get;}

        public static Dictionary<int, string> Font2Color = new Dictionary<int, string>
        {
            { 27, ColorConst.Yellow},
            { 100, ColorConst.Yellow},
            { 101, ColorConst.Orange },
            { 105, ColorConst.Green },
            { 106, ColorConst.White},
            { 107, ColorConst.Gray},
            { 108, ColorConst.Gray},
            { 113, ColorConst.Blue },
            { 161, ColorConst.Gray },
            { 163, ColorConst.Yellow },
            { 165, ColorConst.Green },
            { 192, ColorConst.GreenStrength },
            // 根据需要添加更多映射
        };


        public RichTextSegment(string text, int font, string color = null)
        {
            Text = text;
            Font = font;
            Color = null;
            if (color != null)
            {
                Color = color;
            }
            else if (Font2Color.TryGetValue(font, out var value))
            {
                Color = value;
            }
        }
    }
}