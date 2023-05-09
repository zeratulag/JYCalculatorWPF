using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JX3CalculatorShared.Class
{
    public class SkillDataBase : ObservableObject
    {
        public string Name { get; }
        public string SkillName => Info.Skill_Name;
        public int nCount { get; set; }
        public double CD { get; set; } // CD 事件
        public double CostEnergy { get; set; } // 神机消耗
        public double AddCT { get; set; } // 增加会心
        public double AddCF { get; set; } // 增加会效
        public double AddDmg { get; set; } // 增加伤害
        public double ChannelIntervalCoef { get; set; } = 1; // 系数修改倍率
        public int Frame { get; set; } // 帧数
        public double IntervalTime => Frame / StaticConst.FPS_PER_SECOND; // 初始时间间隔
        public double nChannelInterval => Info.nChannelInterval * ChannelIntervalCoef;
        public double WPCoef => Info.WP_Coef; // 武伤系数


        public SkillInfoItemBase Info;
        public HashSet<string> RecipeNames; // 已应用的秘籍名称
        public HashSet<string> SkillModifierNames; // 已应用的SkillModifier名称

        public SkillDataBase(SkillInfoItemBase item)
        {
            Info = item;
            Name = Info.Name;

            RecipeNames = new HashSet<string>(6);
            SkillModifierNames = new HashSet<string>();

            Reset();
        }

        public SkillDataBase(SkillDataBase old)
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

            ChannelIntervalCoef = old.ChannelIntervalCoef;

            RecipeNames = old.RecipeNames.ToHashSet();
            SkillModifierNames = old.SkillModifierNames.ToHashSet();
        }

        /// <summary>
        /// 重置此技能信息，删除所有秘籍效果
        /// </summary>
        public virtual void Reset()
        {
            CD = Info.CD;
            CostEnergy = Info.CostEnergy;
            nCount = Info.nCount;
            Frame = Info.Frame;

            AddCT = Info.Add_CT;
            AddCF = Info.Add_CF;
            AddDmg = Info.Add_Dmg;

            ChannelIntervalCoef = 1.0;

            RecipeNames.Clear();
            SkillModifierNames.Clear();
        }

        public double GetAPCoef()
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
                case SkillDataTypeEnum.NormalDOT:
                {
                    ap_Coef = 0;
                    break;
                }
            }

            return ap_Coef;
        }

        /// <summary>
        /// 计算技能系数
        /// </summary>
        /// <param name="finalG">最终郭氏系数（分子）</param>
        /// <returns></returns>
        public virtual double CalcAPCoef(double finalG)
        {
            throw new NotImplementedException();
        }

        public double CalcDOTAPCoef(double finalG, int count, double intervalframe)
        {
            var coef1 = CalcAPCoef(finalG);
            var coef2 = Math.Max(16, (int) (intervalframe * count / 12.0)) / StaticConst.FPS_PER_SECOND / count;
            return coef1 * coef2;
        }


        public virtual void Update()
        {
        }

        public virtual bool ApplyValueEffect(string key, double value)
        {
            bool handled = true; // 是否在基类已经解决完毕
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
                    Frame += (int) value;
                    break;
                }

                case "CD":
                {
                    CD += value;
                    break;
                }

                case "Add_nCount":
                {
                    nCount += (int) value;
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
                {
                    break;
                }
                default:
                {
                    handled = false;
                    break;
                }
            }

            return handled;
        }


        public virtual bool ApplyOtherEffect(string key, List<object> value)
        {
            bool handled = true; // 是否在基类已经解决完毕
            switch (key)
            {
                case "Coef":
                {
                    foreach (var v in value)
                    {
                        var realv = (double) v;
                        ChannelIntervalCoef *= realv;
                    }

                    break;
                }

                case "QiPo":
                {
                    break;
                }

                default:
                {
                    handled = false;
                    break;
                }
            }

            return handled;
        }

        public virtual void UpdateCoef()
        {
        }

        public void ApplyRecipe(Recipe recipe, bool update = false)
        {
            // 应用秘籍，默认不重新计算系数
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

        public void ApplyRecipes(IEnumerable<Recipe> recipes, bool update = false)
        {
            // 应用多个秘籍，默认不重新计算系数
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

        public void ApplySkillRecipeGroup(SkillRecipeGroup recipeGroup, bool update = false)
        {
            // 应用秘籍组，默认不重新计算系数
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
    }
}