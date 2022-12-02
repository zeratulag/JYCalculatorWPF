using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.Src.Data;
using JX3CalculatorShared.Utils;
using JYCalculator.Globals;

namespace JYCalculator.Class
{
    public class SkillData : ObservableObject
    {
        #region 成员

        public readonly SkillInfoItem Info;
        public string Name { get; }
        public string SkillName => Info.Skill_Name;
        public int nCount { get; set; }
        public double CD { get; set; } // CD 事件
        public double CostEnergy { get; set; } // 神机消耗

        public double AddCT { get; set; } // 增加会心
        public double AddCF { get; set; } // 增加会效
        public double AddDmg { get; set; } // 增加伤害

        public double IgnoreB { get; set; }
        public double ChannelIntervalCoef { get; set; } = 1;// 系数修改倍率

        public int Frame { get; set; } // 帧数
        public double IntervalTime => Frame / StaticData.FPS_PER_SECOND; // 初始时间间隔
        public double nChannelInterval => Info.nChannelInterval * ChannelIntervalCoef;

        public double APCoef { get; set; } 
        public double WPCoef => Info.WP_Coef; // 武伤系数

        public readonly HashSet<string> RecipeNames; // 已应用的秘籍名称
        public readonly HashSet<string> SkillModifierNames; // 已应用的SkillModifier名称

        #endregion

        #region 构造

        public SkillData(SkillInfoItem item)
        {
            Info = item;
            Name = Info.Name;

            RecipeNames = new HashSet<string>(6);
            SkillModifierNames = new HashSet<string>();

            Reset();
        }

        /// <summary>
        /// 复制构造
        /// </summary>
        /// <param name="old"></param>
        public SkillData(SkillData old)
        {
            Info = old.Info;
            Name = old.Name;
            CD = old.CD;
            CostEnergy = old.CostEnergy;
            nCount = old.nCount;
            Frame = old.Frame;

            AddCT = old.AddCT;
            AddCF = old.AddCF;
            AddDmg = old.AddDmg;

            IgnoreB = old.IgnoreB;

            ChannelIntervalCoef = old.ChannelIntervalCoef;

            APCoef = old.APCoef;

            RecipeNames = old.RecipeNames.ToHashSet();
            SkillModifierNames = old.SkillModifierNames.ToHashSet();
        }


        /// <summary>
        /// 重置此技能信息，删除所有秘籍效果
        /// </summary>
        public void Reset()
        {

            CD = Info.CD;
            CostEnergy = Info.CostEnergy;
            nCount = Info.nCount;
            Frame = Info.Frame;

            AddCT = Info.Add_CT;
            AddCF = Info.Add_CF;
            AddDmg = Info.Add_Dmg;
            IgnoreB = Info.IgnoreB;

            ChannelIntervalCoef = 1.0;

            APCoef = Info.AP_Coef;

            RecipeNames.Clear();
            SkillModifierNames.Clear();
        }

        /// <summary>
        /// 重置此技能信息
        /// </summary>
        /// <returns></returns>
        public SkillData GetReset()
        {
            return new SkillData(Info);
        }

        public SkillData Copy()
        {
            return new SkillData(this);
        }

        public void Update()
        {
            UpdateCoef();
        }


        /// <summary>
        /// 更新技能系数
        /// </summary>
        public void UpdateCoef()
        {
            var finalG = nChannelInterval;
            double ap_Coef = 0;
            switch (Info.Type)
            {
                case SkillDataTypeEnum.Channel:
                    {
                        finalG += Frame;
                        ap_Coef = CalcAPCoef(finalG);
                        break;
                    }

                case SkillDataTypeEnum.Normal:
                case SkillDataTypeEnum.Exclude:
                    {
                        ap_Coef = CalcAPCoef(finalG);
                        break;
                    }

                case SkillDataTypeEnum.DOT:
                    {
                        ap_Coef = CalcDOTAPCoef(finalG, nCount, Frame);
                        break;
                    }

                case SkillDataTypeEnum.Physics:
                case SkillDataTypeEnum.NoneAP:
                case SkillDataTypeEnum.PZ:
                    {
                        break;
                    }
            }

            APCoef = ap_Coef;
        }


        /// <summary>
        /// 计算技能系数
        /// </summary>
        /// <param name="finalG">最终郭氏系数（分子）</param>
        /// <returns></returns>
        public static double CalcAPCoef(double finalG)
        {
            var res = Math.Max(16, (int)finalG) / (16.0 * JYConsts.ChannelIntervalToAPFactor);
            return res;
        }

        public static double CalcDOTAPCoef(double finalG, int count, double intervalframe)
        {
            var coef1 = CalcAPCoef(finalG);
            var coef2 = Math.Max(16, (int)(intervalframe * count / 12.0)) / StaticData.FPS_PER_SECOND / count;
            return coef1 * coef2;
        }



        // 应用秘籍，默认不重新计算系数
        public void ApplyRecipe(Recipe recipe, bool update = false)
        {
            var name = recipe.Name;
            if (RecipeNames.Contains(name))
            {
                throw new ArgumentException($"已存在同名秘籍！{name}");
            }
            RecipeNames.Add(name);
            ApplyEffects(recipe.SSkillAttrs);
            if (update)
            {
                UpdateCoef();
            }
        }

        // 应用多个秘籍，默认不重新计算系数
        public void ApplyRecipes(IEnumerable<Recipe> recipes, bool update = false)
        {
            if (recipes == null) return;
            foreach (var _ in recipes)
            {
                ApplyRecipe(_, false);
            }

            if (update)
            {
                UpdateCoef();
            }
        }


        // 应用秘籍组，默认不重新计算系数
        public void ApplySkillRecipeGroup(SkillRecipeGroup recipeGroup, bool update = false)
        {
            if (recipeGroup == null) return;
            RecipeNames.AddRange(recipeGroup.Names);
            ApplyEffects(recipeGroup.SSkillAttrs);
            if (update)
            {
                UpdateCoef();
            }
        }


        /// <summary>
        /// 应用技能Mod
        /// </summary>
        /// <param name="modifier">技能Mod</param>
        /// <exception cref="ArgumentException"></exception>
        public void ApplySkillModifier(SkillModifier modifier)
        {
            var name = modifier.Name;
            if (SkillModifierNames.Contains(name))
            {
                throw new ArgumentException($"已存在同名MOD！{name}");
            }

            SkillModifierNames.Add(modifier.Name);
            ApplyEffects(modifier.SAttrs);
        }


        public void ApplyEffects(SkillAttrCollection skillAttr)
        {
            if (skillAttr == null || skillAttr.IsEmptyOrNull) return;
            ApplyEffects(skillAttr.Values);

            foreach (var KVP in skillAttr.Others)
            {
                ApplyOtherEffect(KVP.Key, KVP.Value);
            }
        }

        public void ApplyEffects(IDictionary<string, double> dict)
        {
            foreach (var KVP in dict)
            {
                ApplyValueEffect(KVP.Key, KVP.Value);
            }
        }


        public void ApplyValueEffect(string key, double value)
        {
            switch (key)
            {
                case "Add_CT":
                    {
                        AddCT += value;
                        break;
                    }

                case "Add_Dmg":
                    {
                        AddDmg += value;
                        break;
                    }

                case "Add_CF":
                    {
                        AddCF += value;
                        break;
                    }

                case "Frame":
                    {
                        Frame += (int)value;
                        break;
                    }

                case "IgnoreB_P":
                case "IgnoreB":
                    {
                        IgnoreB += value;
                        break;
                    }

                case "CD":
                    {
                        CD += value;
                        break;
                    }

                case "Add_nCount":
                    {
                        nCount += (int)value;
                        break;
                    }

                case "Add_CostEnergy":
                    {
                        CostEnergy += value;
                        break;
                    }

                case "Range":
                case "Target":
                case "Add_Energy":
                case "IgnoreB_M":
                    {
                        break;
                    }

                default:
                    {
                        throw new ArgumentException($"未识别的key！ {key}");
                    }

            }
        }

        public void ApplyOtherEffect(string key, List<object> value)
        {
            switch (key)
            {
                case "Coef":
                    {
                        foreach (var v in value)
                        {
                            var realv = (double)v;
                            ChannelIntervalCoef *= realv;
                        }
                        break;
                    }

                case "QiPo":
                case "ZMWS_Add_Dmg":
                    {
                        break;
                    }

                default:
                    {
                        throw new ArgumentException($"未识别的key！ {key}");
                    }
            }
        }
        #endregion
    }

}