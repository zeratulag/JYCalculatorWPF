using JX3CalculatorShared.Common;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Data;
using JX3CalculatorShared.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


namespace JX3CalculatorShared.Src.Class
{
    public class BigFM: ICatsable
    {
        #region 成员

        public static readonly ImmutableDictionary<EquipSubTypeEnum, string> AttrMap = new Dictionary<EquipSubTypeEnum, string>
        {
            {EquipSubTypeEnum.JACKET, "Base_AP"}, {EquipSubTypeEnum.HAT, "Base_OC"}
        }.ToImmutableDictionary();

        public readonly string Name;
        public readonly string DescName;

        public readonly int IconID;
        public readonly int ItemID;
        public readonly int EnchantID;

        public readonly EquipSubTypeEnum SubType;

        public readonly int LevelMin;
        public readonly int LevelMax;
        public readonly int Rank;
        public readonly int DLCLevel;


        public readonly int Magic;
        public readonly int Physics;

        public readonly BottomsFMTag Tag;
        public readonly ImmutableDictionary<string, double> SAttrs; // 大附魔包括的属性（简化后）

        public int Quality { get; }
        public string ItemName => DescName;
        public string IconPath { get; }

        public string ToolTip { get; }

        #endregion

        #region 构造

        /// <summary>
        /// 游戏内的大附魔（不包括下装）
        /// </summary>
        /// <param name="name">唯一名称</param>
        /// <param name="descName">描述名称</param>
        /// <param name="toolTip"></param>
        /// <param name="iconId">图标ID</param>
        /// <param name="itemID">物品ID</param>
        /// <param name="enchant_Id">附魔ID</param>
        /// <param name="quality">物品等级</param>
        /// <param name="subType">附魔位置</param>
        /// <param name="levelMin">最小品级</param>
        /// <param name="levelMax">最大品级</param>
        /// <param name="rank">大附魔的阶</param>
        /// <param name="magic">内功属性</param>
        /// <param name="physics">外功属性</param>
        /// <param name="tag"></param>
        public BigFM(string name, string descName,
            string toolTip,
            int iconId, int itemID, int enchant_Id,
            int quality, EquipSubTypeEnum subType,
            int levelMin, int levelMax, int rank,
            int magic = -1, int physics = -1, BottomsFMTag tag = BottomsFMTag.NotBottoms)
        {
            Name = name;
            DescName = descName;

            IconID = iconId;
            IconPath = BindingTool.IconID2Path(IconID);

            ItemID = itemID;
            EnchantID = enchant_Id;

            Quality = quality;
            SubType = subType;

            LevelMin = levelMin;
            LevelMax = levelMax;
            Rank = rank;

            Magic = magic;
            Physics = physics;
            Tag = tag;


            SAttrs = GetSAttrs().ToImmutableDictionary();
            ToolTip = toolTip + GetToolTipTail();
        }

        public Dictionary<string, double> GetSAttrs()
        {
            var res = new Dictionary<string, double>(2);
            string key;
            if (AttrMap.ContainsKey(SubType))
            {
                key = AttrMap[SubType];
                res.Add($"M_{key}", Magic);
                res.Add($"P_{key}", Physics);
            }

            return res;
        }


        public string GetToolTipTail()
        {
            var res = $"\n\n物品ID: {ItemID}\t附魔ID: {EnchantID}";
            return res;
        }

        /// <summary>
        /// 从BigFMItem构造
        /// </summary>
        /// <param name="item">BigFMItem对象</param>
        public BigFM(BigFMItem item) : this(item.Name, item.ItemName, item.ToolTip,
            item.IconID, item.ItemID, item.Enchant_ID,
            item.Quality, item.SubType,
            item.Level_Min, item.Level_Max, item.Rank,
            magic: item.Magic, physics: item.Physics, tag: BottomsFMTag.NotBottoms)
        {
            DLCLevel = item.DLCLevel;
        }

        public static BottomsFMTag ParseBottomsFMTag(string name)
        {
            BottomsFMTag result;
            if (name == "None")
            {
                result = BottomsFMTag.None;
            }
            else
            {
               var realname = name.Split('_').Last();
               var success = Enum.TryParse(realname, out BottomsFMTag res);
               result = success ? res : BottomsFMTag.NotBottoms;
            }
            return result;
        }

        public BigFM(BottomsFMItem item) : this(item.Name, item.ItemName, item.ToolTip,
            item.IconID, item.ItemID, item.Enchant_ID,
            item.Quality, EquipSubTypeEnum.BOTTOMS,
            0, item.Level_Max, item.Rank,
            magic: 0, physics: 0, tag: ParseBottomsFMTag(item.Name))
        {
        }


        #endregion

        #region 显示

        public string ToStr()
        {
            var res = GetCatStrList();
            return res.Join(",");
        }

        public void Cat()
        {
            var res = ToStr();
            res.Cat();
        }

        public IList<string> GetCatStrList()
        {
            var res = new List<string>()
            {
                $"Name: {Name}, Desc: {DescName}, Quality: {Quality:D}, SubType: {SubType.ToString()}",
                $"T{Rank:D} ({LevelMin:D} - {LevelMax:D})"
            };

            if (Physics != -1)
            {
                res.Add($"- 外功：{Physics:D}, 内功：{Magic:D}");
            }
            return res;
        }

        #endregion

        #region 方法

        public Dictionary<string, int> GetAtDict()
        {
            string S_ID = AttrMap.GetValueOrDefault(SubType);
            var res = new Dictionary<string, int>();
            var damage_type_value = new Dictionary<string, int>()
            {
                {"P", Physics}, {"M", Magic}
            };

            if (S_ID != null)
            {
                foreach (var KVP in damage_type_value)
                {
                    string k2 = $"{KVP.Key}_{S_ID}";
                    if (KVP.Value == -1)
                    {
                        throw new ArgumentException("错误的数值！");
                    }

                    res.Add(k2, KVP.Value);

                }
            }
            return res;
        }

        #endregion

    }
}