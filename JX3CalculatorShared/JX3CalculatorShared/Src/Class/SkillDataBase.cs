using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using JYCalculator.Data;

namespace JX3CalculatorShared.Class
{
    public class SkillDataBase : ObservableObject
    {
        public string Name { get; }
        public string SkillName => Info.SkillName;
        public int nCount { get; set; }
        public double CD { get; set; } // CD 事件
        public double CostEnergy { get; set; } // 神机消耗
        public double AddCriticalStrikeRate { get; set; } // 增加会心
        public double AddCriticalPowerRate { get; set; } // 增加会效
        public double AddDamage { get; set; } // 增加伤害
        public double AddNPC_Coef { get; set; } // 增加非侠士
        public double ChannelIntervalCoef { get; set; } = 1; // 系数修改倍率
        public int Frame { get; set; } // 帧数
        public double IntervalTime => Frame / StaticConst.FRAMES_PER_SECOND; // 初始时间间隔
        public double nChannelInterval => Info.nChannelInterval * ChannelIntervalCoef;
        public double WeaponDamageCoef => Info.WP_Coef; // 武伤系数

        public List<string> SkillNameTags { get; private set; } = new List<string>();

        public SkillInfoItemBase Info;
        public HashSet<string> AppliedRecipes; // 已应用的秘籍名称
        public HashSet<string> AppliedSkillModifiers; // 已应用的SkillModifier名称

        public SkillDataBase(SkillInfoItemBase item)
        {
            Info = item;
            Name = Info.Name;

            AppliedRecipes = new HashSet<string>(6);
            AppliedSkillModifiers = new HashSet<string>();

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

            AddCriticalStrikeRate = old.AddCriticalStrikeRate;
            AddCriticalPowerRate = old.AddCriticalPowerRate;
            AddDamage = old.AddDamage;
            AddNPC_Coef = old.AddNPC_Coef;

            ChannelIntervalCoef = old.ChannelIntervalCoef;

            AppliedRecipes = old.AppliedRecipes.ToHashSet();
            AppliedSkillModifiers = old.AppliedSkillModifiers.ToHashSet();

            SkillNameTags = old.SkillNameTags.Copy();
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

            AddCriticalStrikeRate = Info.Add_CT;
            AddCriticalPowerRate = Info.Add_CF;
            AddDamage = Info.Add_Dmg;
            AddNPC_Coef = Info.Add_NPCDmg;

            ChannelIntervalCoef = 1.0;

            AppliedRecipes.Clear();
            AppliedRecipes.AddRange(Info.AppliedRecipes);
            AppliedSkillModifiers.Clear();
            AppliedSkillModifiers.AddRange(Info.AppliedRecipes);
            SkillNameTags.Clear();
            SkillNameTags.AddRange(Info.SkillNameTags);
        }

        public double GetAPCoef()
        {
            var finalG = nChannelInterval;
            double ap_Coef = 0;
            switch (Info.Type)
            {
                case SkillDataTypeEnum.NormalChannel:
                {
                    finalG += Frame;
                    ap_Coef = CalcAPCoef(finalG);
                    break;
                }

                case SkillDataTypeEnum.Normal:
                case SkillDataTypeEnum.Exclude:
                case SkillDataTypeEnum.MultiChannel:
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
        /// <param name="finalG">最终郭氏系数（nChannelInterval）</param>
        /// <returns></returns>
        public virtual double CalcAPCoef(double finalG)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 计算DOT系数
        /// </summary>
        /// <param name="finalG">最终郭氏系数（nChannelInterval）</param>
        /// <param name="count">跳数</param>
        /// <param name="intervalframe">dot生效间隔（帧）</param>
        /// <returns></returns>
        public double CalcDOTAPCoef(double finalG, int count, double intervalframe)
        {
            var coef1 = CalcAPCoef(finalG);
            var coef2 = Math.Max(16, (int) (intervalframe * count / 12.0)) / StaticConst.FRAMES_PER_SECOND / count;
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
                    AddCriticalStrikeRate += value;
                    break;
                }

                case "Add_Dmg":
                {
                    AddDamage += value;
                    break;
                }

                case "Add_CF":
                {
                    AddCriticalPowerRate += value;
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
                    if (Info.FightName != "穿心(DOT)" || !SkillName.Contains("穿林"))
                    {
                        nCount += (int) value; // 穿林穿心DOT不吃跳数加成
                    }

                    break;
                }

                case "Add_CostEnergy":
                {
                    CostEnergy += value;
                    break;
                }

                case "Add_NPC_Dmg":
                {
                    AddNPC_Coef += value;
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

            if (Info.AppliedRecipes.Contains(name)) // 如果Info已经有了这个秘籍就跳过
            {
                return;
            }

            if (recipe.RecipeID == 5145)
            {
                // 瞬发追命特殊处理
                if (Info.IsDerived)
                {
                    return; // 对于派生技能不应该再次修饰
                }
                if (Frame > 0)
                {
                    return; // 非瞬发的追命不应该吃到此效果
                }
            }

            if (AppliedRecipes.Contains(name))
            {
                throw new ArgumentException($"已存在同名秘籍！{name}");
            }

            AppliedRecipes.Add(name);
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
            AppliedRecipes.AddRange(recipeGroup.Names);
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
            if (Info.AppliedSkillModifiers.Contains(name))
            {
                throw new ArgumentException($"已存在同名MOD！{name}");
            }

            if (AppliedSkillModifiers.Contains(name))
            {
                throw new ArgumentException($"已存在同名MOD！{name}");
            }

            AppliedSkillModifiers.Add(modifier.Name);
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