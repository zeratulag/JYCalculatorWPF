using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JX3PZ.Data;
using JX3PZ.Globals;
using JX3PZ.Models;
using JX3PZ.Src;
using JX3PZ.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Windows.Documents;
using static System.Net.Mime.MediaTypeNames;

namespace JX3PZ.Class
{
    public class Equip
    {
        #region 自动生成

        public string Base1Type { get; set; }
        public int Base1Value { get; set; } = -1;
        public string Base2Type { get; set; }
        public int Base2Value { get; set; } = -1;
        public int Base3Max { get; set; } = -1;
        public string Base3Type { get; set; }
        public int Base3Value { get; set; } = -1;
        public string BasicMagicEntry_Str { get; set; }
        public string BasicTag_Str { get; set; } = "";
        public string BelongMap { get; set; }
        public string BelongSchool { get; set; }
        public int DetailType { get; set; } = -1;
        public int DiamondAttributeID1 { get; set; } = -1;
        public int DiamondAttributeID2 { get; set; } = -1;
        public int DiamondAttributeID3 { get; set; } = -1;
        public string DiamondID_Str { get; set; }
        public string DiamondTag_Str { get; set; }
        public string EID { get; set; }
        public string ExtraMagicEntry_Str { get; set; }
        public string ExtraTag_Str { get; set; } = "";
        public string GetType { get; set; }
        public int ID { get; set; } = -1;
        public int IconID { get; set; } = -1;
        public int Level { get; set; } = -1;
        public string MagicEntry_Str { get; set; } = "";
        public string MagicKind { get; set; }
        public string MagicType { get; set; }
        public int MaxStrengthLevel { get; set; } = -1;
        public string Name { get; set; }
        public string OtherTag { get; set; }
        public int Quality { get; set; } = -1;
        public int RecommendID { get; set; } = -1;
        public int RequireCamp { get; set; } = -1;
        public int SetID { get; set; } = -1;
        public string SkillDesc { get; set; } = "";
        public int SkillID { get; set; } = -1;
        public int SkillLevel { get; set; } = -1;
        public int SubType { get; set; } = -1;
        public int UIID { get; set; } = -1;
        public int MaxDurability { get; set; } = -1;
        public int Require1Type { get; set; } = -1;
        public int Require1Value { get; set; } = -1;
        public string GetInfo_JSON { get; set; } = "";
        public string ItemDesc { get; set; } = "";

        #endregion

        public bool HasBasicTag { get; private set; }
        public bool HasDiamond { get; private set; } // 是否有镶嵌孔
        public bool IsValid { get; private set; } // 是否不为空
        public bool IsWeapon { get; private set; } // 是否为武器
        public EquipSubTypeEnum SubTypeEnum { get; private set; } // 子分类
        public EquipTypeEnum EquipType { get; private set; }

        public EquipTag AttrTag { get; private set; } // 属性标签

        public EquipAttribute Attributes { get; private set; } // 所有属性

        public int Score { get; protected set; } // 装分
        public int RequirePlayerLevel { get; protected set; } // 需要等级

        public EquipOption CEquipOption { get; private set; } // 装备选项
        public string EquipOptionName => CEquipOption?.Name; // 装备选项名
        public EquipShowViewModel EquipShowVM
        {
            get
            {
                if (_EquipShowVM == null)
                {
                    ParseToolTip();
                }
                return _EquipShowVM;
            }
        }

        public bool IsGetInfoParsed { get; private set; }  = false;
        public EquipGetInfo GetInfo { get; protected set; }

        private EquipShowViewModel _EquipShowVM = null;

        public bool IsHaste => AttrTag.HasHaste; // 是否为加速装备

        public string ToolTip
        {
            get
            {
                if (_ToolTip == null)
                {
                    ParseToolTip();
                }
                return _ToolTip;
            }
        }

        private string _ToolTip = null;

        public void ParseAttrs()
        {
            Attributes = new EquipAttribute(this);
            AttrTag = new EquipTag(this);
            HasBasicTag = !BasicTag_Str.IsEmptyOrWhiteSpace();
            HasDiamond = !DiamondTag_Str.IsEmptyOrWhiteSpace();
            IsValid = Quality > 0;
            SubTypeEnum = (EquipSubTypeEnum) SubType;
            EquipType = EquipMapLib.GetEquipTypeBySubType(SubTypeEnum);
            IsWeapon = EquipType == EquipTypeEnum.weapon;
            GetScore();
            GetRequirePlayerLevel();
        }

        public void ParseGetInfo()
        {
            var infos = JsonConvert.DeserializeObject<EquipGetInfoItem[]>(GetInfo_JSON);
            GetInfo = new EquipGetInfo(infos);
            GetInfo.Parse();
            IsGetInfoParsed = true;
        }

        public EquipShowViewModel GetDefaultShow()
        {
            var model = new EquipSnapShotModel(this);
            _EquipShowVM = model.GetShow();
            if (SetID > 0)
            {
                var cset = GetPzSet();
                var vm = new EquipShowSetViewModel(cset);
                var res = new PzSetResult(this);
                vm.UpdateFrom(res);
                _EquipShowVM.Set = vm;
            }

            return _EquipShowVM;
        }

        public virtual void Parse()
        {
            ParseAttrs();
            //ParseGetInfo();
        }

        public void ParseToolTip()
        {
            var vm = GetDefaultShow();
            var res = vm.GetDescList();
            _ToolTip = string.Join("\n", res);
        }

        public override int GetHashCode()
        {
            return EID.GetHashCode();
        }

        public bool Equals(Equip other)
        {
            return other != null && EID == other.EID;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Equip);
        }


        /// <summary>
        /// 该装备是否满足筛选器要求
        /// </summary>
        /// <param name="arg">筛选器</param>
        /// <returns></returns>
        public bool CanFilter(EquipFilterArg arg)
        {
            if (arg == null)
            {
                return true;
            }

            if (!IsValid) // 空装备
            {
                return true;
            }

            var minLevel = arg.MinLevel;
            if (Quality > 4)
            {
                // 橙武放宽品级要求
                minLevel = 5000;
                if (Name.StartsWith("煞·")) // 奇遇橙武和普通橙武完全一样
                {
                    return false;
                }
            }

            if (Level >= minLevel && Level <= arg.MaxLevel)
            {
                return arg.OtherFiler.Contains(OtherTag) && arg.AttrFilter.IsSubsetOf(AttrTag.ExtraTag);
            }

            return false;
        }

        /// <summary>
        /// 是否适用于大附魔
        /// </summary>
        /// <returns></returns>
        public bool FitEnchant(Enchant enchant)
        {
            var res = enchant.Level_Max >= Level && enchant.Level_Min <= Level && enchant.DestItemSubType == SubType;
            return res;
        }

        public bool FitBigFM(BigFM bigFM)
        {
            var res = bigFM.LevelMax >= Level && bigFM.LevelMin <= Level && bigFM.SubType == SubTypeEnum;
            return res;
        }

        public static int CalcEquipScore(int level, int quality, EquipSubTypeEnum subType)
        {
            decimal qualityCoef = 1.8m + 0.7m * (quality - 4);
            var typeCoef = EquipMapLib.GetEquipMapItemBySubType(subType).ScoreCoef;
            var res0 = level * qualityCoef * typeCoef;
            var res = res0.MathRound();
            return res;
        }

        public int GetScore()
        {
            Score = CalcEquipScore(Level, Quality, SubTypeEnum);
            return Score;
        }

        public int GetRequirePlayerLevel()
        {
            if (Require1Type == 5)
            {
                RequirePlayerLevel = Require1Value;
            }

            return RequirePlayerLevel;
        }

        public string GetRequireDesc()
        {
            string res = "";
            if (Require1Type == 5)
            {
                res = $"需要等级{RequirePlayerLevel}";
            }

            return res;
        }

        public string GetDurabilityDesc()
        {
            string res = "";
            if (MaxDurability > 0)
            {
                res = $"耐久度：{MaxDurability}/{MaxDurability}";
            }

            return res;
        }

        /// <summary>
        /// 计算真实生效的精炼品级（不能超过最大精炼等级）
        /// </summary>
        /// <param name="strength">精炼等级</param>
        /// <returns></returns>
        public int CalcRealStrength(int strength)
        {
            return Math.Min(strength, MaxStrengthLevel);
        }

        /// <summary>
        /// 精炼品级提升
        /// </summary>
        /// <param name="strength">精炼等级</param>
        /// <returns>精炼提升的品级</returns>
        public int CalcStrengthLevel(int strength)
        {
            var realStrength = CalcRealStrength(strength);
            var r = (Level * realStrength * (0.003m * realStrength + 0.007m)) / 2.0m;
            return r.MathRound();
        }

        /// <summary>
        /// 精炼装分提升
        /// </summary>
        /// <param name="strength">精炼等级</param>
        /// <returns>精炼提升的装分</returns>
        public int CalcStrengthScore(int strength)
        {
            var realStrength = CalcRealStrength(strength);
            var r = (Score * realStrength * (0.003m * realStrength + 0.007m)) / 2.0m;
            return r.MathRound();
        }

        public void AttachEquipOption(EquipOption o)
        {
            CEquipOption = o;
            if (CEquipOption != null)
            {
                var optionDesc = "\n" + CEquipOption.GetDesc();
                if (optionDesc != null)
                {
                    if (SubTypeEnum == EquipSubTypeEnum.PENDANT)
                    {
                        SkillDesc += optionDesc;
                    }

                    if (SubTypeEnum == EquipSubTypeEnum.PRIMARY_WEAPON)
                    {
                        Attributes.AttachEquipOptionWaterDesc(optionDesc);
                    }
                }
            }
        }

        public PzSet GetPzSet()
        {// 获取此装备关联的套装信息
            PzSet res = null;
            if (SetID > 0)
            {
                res = StaticPzData.GetPzSet(SetID);
            }
            return res;
        }

    }
}