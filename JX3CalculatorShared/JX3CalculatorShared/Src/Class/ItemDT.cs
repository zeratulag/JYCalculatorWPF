using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using System.Collections.Generic;
using System.Linq;

namespace JX3CalculatorShared.Class
{

    public class ItemDT : ICatsable
    {
        #region 成员

        public readonly string Name;
        public readonly string DescName;
        public readonly int IconID = -1;
        public readonly int ItemID = -1;
        public readonly ItemDTTypeEnum Type;
        public readonly string ItemName;
        public readonly Dictionary<string, double> Data;
        public readonly BaseBuff Buff;
        public readonly KeyValuePair<string, double> SimpleAttr;
        public readonly bool IsValid; // 是否为有效单体
        public readonly int DLCLevel; // 出现等级
        public readonly int BuffEnchantID; // Buff/EnchantID（因为武器磨石不是BUFF）
        public readonly int Level;

        public readonly string RawID; // 用于存储的编码

        public int Quality { get; } = -1;
        public string IconPath { get; }
        public string ItemNamePart1 { get; }
        public string ItemNamePart2 { get; }
        public string ToolTip { get; }
        public string ItemDTType { get; } // 中文类型（食品增强，食品辅助……）


        #endregion

        #region 构造

        /// <summary>
        /// 从ItemNameM中拆分两行，分别作为单体名称和单体属性描述
        /// </summary>
        /// <param name="itemNameM">输入的描述字符串</param>
        /// <returns></returns>
        public static (string part1, string part2) GetItemNames(string itemNameM)
        {
            var parts = itemNameM.Split('\n');
            var part1 = parts[0];
            var part2 = parts[1];
            return (part1, part2);
        }

        public static BaseBuff GenerateBaseBuff(string name, string descName, int iconID, Dictionary<string, double> data)
        {
            var res = new BaseBuff(name: name, descName: descName, iconID: iconID, isTarget: false, data: data);
            return res;
        }

        public BaseBuff GenerateBaseBuff()
        {
            var res = GenerateBaseBuff(Name, DescName, IconID, Data);
            return res;
        }

        public string GetToolTipTail()
        {
            string res = "";
            if (ItemID > 0)
            {

            }
            res = $"\n\nID: {ItemID}";
            return res;
        }


        public ItemDT(ItemDTItem item)
        {
            var parsedAts = item.ParseItem();
            Name = item.Name;
            DescName = item.DescName;
            IconID = item.IconID;
            ItemID = item.ItemID;
            Quality = item.Quality;
            Type = item.Type;
            ItemName = item.ItemName;
            DLCLevel = item.DLCLevel;
            BuffEnchantID = item.BuffEnchantID;
            Level = item.Level;

            RawID = $"{ItemID}#{Level}";

            ItemDTType = item.类型;

            ToolTip = item.ToolTip + GetToolTipTail();
            Data = parsedAts.Values;
            Buff = GenerateBaseBuff();
            SimpleAttr = Buff.SCharAttrs.Values.First();

            (ItemNamePart1, ItemNamePart2) = GetItemNames(item.ItemNameM);

            IconPath = BindingTool.IconID2Path(IconID);
            IsValid = (Quality > 0);
        }


        #endregion


        public string ToStr()
        {
            throw new System.NotImplementedException();
        }

        public void Cat()
        {
            throw new System.NotImplementedException();
        }

        public IList<string> GetCatStrList()
        {
            throw new System.NotImplementedException();
        }
    }
}