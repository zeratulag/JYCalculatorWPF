using System;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Class;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Utils;
using JYCalculator.Globals;
using HandyControl.Expression.Shapes;


namespace JYCalculator.Data
{
    public class SkillInfoItemBase : AbsSkillDataItem
    {
        public string FightName { get; set; }
        public double Interval { get; set; }
        public int Frame { get; set; }
        public int nCount { get; set; } = 1;
        public double CostEnergy { get; set; } = 0;
        public double CD { get; set; } = 0;
        public double Fixed_Min { get; set; } = 0;
        public double Fixed_Max { get; set; } = 0;
        public double Fixed_Dmg { get; set; } = 0;
        public double SurplusFactor { get; set; } = Double.NaN;
        public double nChannelInterval { get; set; }
        public double WP_Coef { get; set; } = 0;
        public double Add_Dmg { get; set; } = 0;
        public double Add_NPCDmg { get; set; } = 0;
        public double Add_CT { get; set; } = 0;
        public double Add_CF { get; set; } = 0;
        public int IconID { get; set; }
        public int SkillID { get; set; }
        public int Level { get; set; }
        public int RecipeType { get; set; }
        public ulong SkillEventMask1 { get; set; }
        public ulong SkillEventMask2 { get; set; }
        public int Cast_SkillID { get; set; }
        public ulong Cast_SkillEventMask1 { get; set; }
        public ulong Cast_SkillEventMask2 { get; set; }
        public string BelongKungfu { get; set; }
        public SkillDataTypeEnum Type { get; set; }
        public double SurplusCoef { get; private set; } = 0;
        public int EnergyInjection { get; private set; } // 注能次数

        public string RawBasicSkillNames { get; set; } // 无须复制
        public string RawAppliedRecipes { get; set; } // 无须复制
        public List<string> BasicSkillNames { get; private set; }
        public List<string> AppliedRecipes { get; private set; } = new List<string>();
        public List<string> AppliedSkillModifiers { get; private set; } = new List<string>();
        public List<string> AppliedSkillBuffs { get; private set; } = new List<string>();
        public List<string> SkillNameTags { get; private set; } = new List<string>();
        public string BaseSkillName { get; protected set; } = null; // 基础技能名称

        public string AppliedRecipesStr => AppliedRecipes.StrJoin(", ");
        public string AppliedSkillModifiersStr => AppliedSkillModifiers.StrJoin(", ");
        public string AppliedSkillBuffsStr => AppliedSkillBuffs.StrJoin(" | ");

        public bool IsPZ => Type == SkillDataTypeEnum.PZ; // 是否为类破招技能
        public bool IsMultiChannel => Type == SkillDataTypeEnum.MultiChannel; // 是否为多段倒读条
        public bool IsDerived { get; set; } = false; // 是否为衍生技能

        public SkillInfoItemBase()
        {
        }

        #region 复制构造函数

        public SkillInfoItemBase(SkillInfoItemBase item)
        {
            Name = item.Name;
            SkillName = item.SkillName;
            FightName = item.FightName;
            Interval = item.Interval;
            Frame = item.Frame;
            nCount = item.nCount;
            CostEnergy = item.CostEnergy;
            CD = item.CD;
            Fixed_Min = item.Fixed_Min;
            Fixed_Max = item.Fixed_Max;
            Fixed_Dmg = item.Fixed_Dmg;
            SurplusFactor = item.SurplusFactor;
            nChannelInterval = item.nChannelInterval;
            WP_Coef = item.WP_Coef;
            Add_Dmg = item.Add_Dmg;
            Add_NPCDmg = item.Add_NPCDmg;
            Add_CT = item.Add_CT;
            Add_CF = item.Add_CF;
            IconID = item.IconID;
            SkillID = item.SkillID;
            Level = item.Level;
            RecipeType = item.RecipeType;
            SkillEventMask1 = item.SkillEventMask1;
            SkillEventMask2 = item.SkillEventMask2;
            Cast_SkillID = item.Cast_SkillID;
            Cast_SkillEventMask1 = item.Cast_SkillEventMask1;
            Cast_SkillEventMask2 = item.Cast_SkillEventMask2;
            BelongKungfu = item.BelongKungfu;
            Type = item.Type;
            SurplusCoef = item.SurplusCoef;
            EnergyInjection = item.EnergyInjection;

            BaseSkillName = item.BaseSkillName;
            BasicSkillNames = item.BasicSkillNames.Copy();
            AppliedRecipes = item.AppliedRecipes.Copy();
            AppliedSkillModifiers = item.AppliedSkillModifiers.Copy();
            AppliedSkillBuffs = item.AppliedSkillBuffs.Copy();
            SkillNameTags = item.SkillNameTags.Copy();
        }

        #endregion

        public void AddSkillNameTag(string tag)
        {
            if (tag != null)
            {
                SkillNameTags.Add(tag);
            }
        }

        /// <summary>
        /// 基于SkillNameTags更新修饰后的技能名称
        /// </summary>
        public void UpdateSkillName()
        {
            var skillNameSuffix = SkillNameTags.StrJoin(",");
            skillNameSuffix = $"[{skillNameSuffix}]";
            SkillName += skillNameSuffix;
        }


        /// <summary>
        /// 判断技能能否触发事件
        /// </summary>
        /// <param name="skillInfoItem">技能</param>
        /// <param name="eventItem">技能事件</param>
        /// <returns>能否触发</returns>
        public static bool CanTrigger(SkillInfoItemBase skillInfoItem, SkillEventTypeMask eventItem)
        {
            if (skillInfoItem.Type == SkillDataTypeEnum.DOT || skillInfoItem.Type == SkillDataTypeEnum.PZ)
            {
                // DOT和破招无法触发任何事件
                return false;
            }

            bool skill1 = (skillInfoItem.SkillEventMask1 & eventItem.EventMask1) > 0;
            bool skill2 = (skillInfoItem.SkillEventMask2 & eventItem.EventMask2) > 0;
            bool cast1 = (skillInfoItem.Cast_SkillEventMask1 & eventItem.EventMask1) > 0;
            bool cast2 = (skillInfoItem.Cast_SkillEventMask2 & eventItem.EventMask2) > 0;
            bool res = skill1 || skill2 || cast1 || cast2;
            return res;
        }

        /// <summary>
        /// 判断技能能否触发事件
        /// </summary>
        /// <param name="skillInfoItem">技能</param>
        /// <param name="eventItem">技能事件</param>
        /// <returns>能否触发</returns>
        public static bool CanTrigger(SkillInfoItemBase skillInfoItem, SkillEventItem eventItem)
        {
            if (skillInfoItem.Type == SkillDataTypeEnum.DOT || skillInfoItem.Type == SkillDataTypeEnum.PZ)
            {
                // DOT和破招无法触发任何事件
                return false;
            }

            bool SkillEventTypeMaskCanTrigger = CanTrigger(skillInfoItem, eventItem.EventTypeMask);
            bool eventskill = (skillInfoItem.SkillID > 0) && (skillInfoItem.SkillID == eventItem.EventSkillID);
            bool res = SkillEventTypeMaskCanTrigger || eventskill;
            return res;
        }

        public bool CanTrigger(SkillEventTypeMask eventItem)
        {
            return CanTrigger(this, eventItem);
        }

        public bool CanTrigger(SkillEventItem eventItem)
        {
            return CanTrigger(this, eventItem);
        }

        public IEnumerable<string> TriggerEvent(IEnumerable<SkillEventItem> eventItems)
        {
            var res = from item in eventItems
                where CanTrigger(item)
                select item.Name;
            return res.ToImmutableArray();
        }

        /// <summary>
        /// 判断技能是否吃秘籍
        /// </summary>
        /// <param name="info">技能</param>
        /// <param name="recipe">秘籍</param>
        /// <returns></returns>
        public static bool CanBeEffectedByRecipe(SkillInfoItemBase info, Recipe recipe)
        {
            bool res = false;

            if (recipe.EffectSkillName.Count > 0)
            {
                res = recipe.EffectSkillName.Contains(info.Name);
                if (res)
                {
                    return true;
                }
            }

            bool skillRecipe = (info.SkillID > 0) && (info.SkillID == recipe.SkillID);
            bool skillRecipeType =
                (info.RecipeType > 0) && (info.RecipeType == recipe.SkillRecipeType);
            res = skillRecipe || skillRecipeType;
            if (res)
            {
                recipe.EffectSkillName.Add(info.Name);
            }

            return res;
        }

        public bool CanBeEffectedByRecipe(Recipe recipe)
        {
            return CanBeEffectedByRecipe(this, recipe);
        }

        public IEnumerable<string> EffectRecipe(IEnumerable<Recipe> recipes)
        {
            var res = from item in recipes
                where CanBeEffectedByRecipe(item)
                select item.Name;
            return res.ToImmutableArray();
        }


        // 获取充能次数
        public int GetEnergyInjectionNum()
        {
            int res = 0;

            if (Type == SkillDataTypeEnum.PZ || Type == SkillDataTypeEnum.DOT)
            {
                return 0;
            }

            if (AppStatic.XinFaTag == "JY")
            {
                if ((BelongKungfu == "百步穿杨" || BelongKungfu == "乾坤一掷") && Name != "BaiYuTiaoZhu" &&
                    Name != "KongQueLing")
                {
                    // 注意奇穴的白雨跳珠不能触发注能
                    res += 1;
                }

                if (Name == SkillKeyConst.穿心弩)
                {
                    res += 3;
                }
            }

            return res;
        }

        public void Parse()
        {
            EnergyInjection = GetEnergyInjectionNum();
            GetSurplusCoef();
            ParseBaseSkillNames();
            ParseAppliedRecipes();
        }

        public void ParseBaseSkillNames()
        {
            if (RawBasicSkillNames == null || RawBasicSkillNames.Trim() == String.Empty)
            {
                RawBasicSkillNames = Name;
            }

            BasicSkillNames = StringTool.ParseStringList(RawBasicSkillNames).ToList();
        }

        public void ParseAppliedRecipes()
        {
            AppliedRecipes = StringTool.ParseStringList(RawAppliedRecipes).ToList();
        }

        // 计算破招系数
        public void GetSurplusCoef()
        {
            if (IsPZ)
            {
                int nFactor = (int) SurplusFactor;
                var factor = (double) nFactor / StaticConst.G_KILO_SQUARE_NUM;
                SurplusCoef = XFStaticConst.CurrentLevelParams.Surplus * (1 + factor);
            }
        }

        // 判断能否被SkillModifer影响
        public bool CanBeEffectedBySkillModifer(SkillModifier modifier)
        {
            bool hasBasicIntersection = BasicSkillNames.Any(x => modifier.BasicSkillNames.Contains(x));
            if (hasBasicIntersection) return true;
            if (modifier.SkillNames != null)
            {
                bool hasNameIntersection = BasicSkillNames.Any(x => modifier.SkillNames.Contains(x));
                var res = hasBasicIntersection || hasNameIntersection;
                return res;
            }

            return false;
        }

        public bool HasBeenEffectedBySkillModifier(SkillModifier modifier)
        {
            return AppliedSkillModifiers.Contains(modifier.Name);
        }

        public bool HasBeenEffectedBySkillRecipe(Recipe recipe)
        {
            return AppliedRecipes.Contains(recipe.Name);
        }
    }
}