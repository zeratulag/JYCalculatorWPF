using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JX3PZ.Class;
using JX3PZ.Globals;
using JX3PZ.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


namespace JX3CalculatorShared.Class
{
    public class BigFM : ICatsable
    {
        #region 成员

        public static readonly ImmutableDictionary<EquipSubTypeEnum, string> AttrMap =
            new Dictionary<EquipSubTypeEnum, string>
            {
                {EquipSubTypeEnum.JACKET, "BaseAttackPower"}, {EquipSubTypeEnum.HAT, "BaseOvercome"}
            }.ToImmutableDictionary();

        public readonly string Name;
        public readonly string DescName;

        public int IconID { get; }
        public readonly int UIID;
        public readonly int ID;

        public readonly EquipSubTypeEnum SubType;

        public readonly int LevelMin;
        public readonly int LevelMax;
        public readonly int Rank;
        public readonly int ExpansionPackLevel;
        public string EnhanceDesc;
        public readonly int Score;
        public readonly int Magic;
        public readonly int Physics;

        public readonly BottomsFMTag Tag;
        public readonly ImmutableDictionary<string, double> SAttrs; // 大附魔包括的属性（简化后）
        public readonly ImmutableArray<AttributeEntry> AttributeEntries; // 以AttributeEntry表示的属性
        public int Quality { get; }
        public string ItemName => DescName;
        public string ToolTip { get; }
        public string Desc { get; }

        public readonly EnhanceAttributeEntryViewModel VM;

        public string SkillNameSuffix => $"{ExpansionPackLevel}#{Rank}"; // 大附魔对应技能名的后缀

        public bool IsSuperCustom => ExpansionPackLevel >= 130 || IsFixedDamaged(LevelMin); // 是否为固定伤害, 130 级开始全是固定伤害

        #endregion

        #region 构造

        /// <summary>
        /// 游戏内的大附魔（不包括下装）
        /// </summary>
        /// <param name="name">唯一名称</param>
        /// <param name="descName">描述名称</param>
        /// <param name="toolTip"></param>
        /// <param name="iconId">图标ID</param>
        /// <param name="uiid">物品ID</param>
        /// <param name="id">附魔ID</param>
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
            int iconId, int uiid, int id,
            int quality, EquipSubTypeEnum subType,
            int levelMin, int levelMax, int rank,
            int magic = -1, int physics = -1, BottomsFMTag tag = BottomsFMTag.NotBottoms)
        {
            Name = name;
            DescName = descName;

            IconID = iconId;

            UIID = uiid;
            ID = id;

            Quality = quality;
            SubType = subType;

            LevelMin = levelMin;
            LevelMax = levelMax;
            Rank = rank;

            Magic = magic;
            Physics = physics;
            Tag = tag;

            SAttrs = GetSAttrs().ToImmutableDictionary();
            var ae = GetAttributeEntries();
            AttributeEntries = ae?.ToImmutableArray() ?? ImmutableArray<AttributeEntry>.Empty;

            ToolTip = toolTip + GetToolTipTail();
            Desc = GetDesc();
        }

        public Dictionary<string, double> GetSAttrs()
        {
            var res = new Dictionary<string, double>(2);
            string key;
            if (AttrMap.TryGetValue(SubType, out var value))
            {
                key = value;
                res.Add($"Magic{key}", Magic);
                res.Add($"Physics{key}", Physics);
            }

            return res;
        }

        public AttributeEntry[] GetAttributeEntries()
        {
            AttributeEntry[] res = null;
            if (SubType == EquipSubTypeEnum.HAT)
            {
                // 帽子大附魔加破防
                res = new AttributeEntry[]
                {
                    new AttributeEntry("atPhysicsOvercomeBase", Physics, AttributeEntryTypeEnum.BigFM),
                    new AttributeEntry("atMagicOvercome", Magic, AttributeEntryTypeEnum.BigFM),
                };
            }
            else
            {
                if (SubType == EquipSubTypeEnum.JACKET)
                {
                    // 上衣大附魔加攻击减承疗
                    var ld = new Dictionary<string, int>()
                    {
                        {"BE_THERAPY_COEFFICIENT", -102},
                        {"PHYSICS_ATTACK_POWER_BASE", Physics},
                        {"MAGIC_ATTACK_POWER_BASE", Magic},
                    };
                    res = AttributeIDLoader.GetAttributeEntriesFromLuaDict(ld, AttributeEntryTypeEnum.BigFM);
                }
            }

            return res;
        }


        public string GetToolTipTail()
        {
            var res = $"\n\nUIID: {UIID}\t附魔ID: {ID}";
            return res;
        }

        public string GetDesc()
        {
            var res = $"{LevelMin}品 ~ {LevelMax}品";
            return res;
        }


        /// <summary>
        /// 从BigFMItem构造
        /// </summary>
        /// <param name="item">BigFMItem对象</param>
        public BigFM(Enchant item) : this(item.Name, item.ItemName, item.ToolTip,
            item.IconID, item.UIID, item.ID,
            item.Quality, item.SubType,
            item.Level_Min, item.Level_Max, item.Rank,
            magic: item.Magic, physics: item.Physics, tag: BottomsFMTag.NotBottoms)
        {
            ExpansionPackLevel = item.ExpansionPackLevel;
            EnhanceDesc = item.EnhanceDesc;
            Score = item.Score;
            VM = new EnhanceAttributeEntryViewModel(this);
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
            item.IconID, item.UIID, item.ID,
            item.Quality, EquipSubTypeEnum.BOTTOMS,
            0, item.Level_Max, item.Rank,
            magic: 0, physics: 0, tag: ParseBottomsFMTag(item.Name))
        {
        }

        #endregion

        public override int GetHashCode()
        {
            return UIID.GetHashCode();
        }


        public bool Equals(BigFM other)
        {
            return other != null && other.UIID == UIID;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BigFM);
        }

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

        public static (string Color, string Path) GetColorImage(BigFM b)
        {
            string color;
            string path;
            if (b == null)
            {
                color = ColorConst.Inactive;
                path = BindingTool.ImageName2Path("enchant-null");
            }
            else
            {
                color = ColorConst.Enhance;
                path = BindingTool.ImageName2Path("enchant");
            }

            return (color, path);
        }

        public static bool IsFixedDamaged(int levelMin)
        {
            return levelMin >= 14300;
        }

    }
}