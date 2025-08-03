using JX3CalculatorShared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using JX3CalculatorShared.Class;

namespace JYCalculator.Data
{
    public class SkillInfoItem : SkillInfoItemBase
    {
        public double AP_Coef { get; set; } = 0;
        public double IgnoreB { get; set; } = 0;


        // 应用技能修饰
        public void ApplySkillModifier(SkillModifier modifier)
        {
            if (modifier == null) return;
            if (!CanBeEffectedBySkillModifer(modifier)) throw new ArgumentException($@"{Name} 无法被{modifier.Name} 修饰！");
            if (HasBeenEffectedBySkillModifier(modifier)) throw new ArgumentException($@"{Name} 已被{modifier.Name} 修饰！");

            this.ProcessSkillModifier(modifier);
            AppliedSkillModifiers.Add(modifier.Name);
            modifier.SkillNames.Add(Name);
            AddSkillNameTag(modifier.SkillNameTag);
        }

        // 应用技能秘籍
        public void ApplySkillRecipe(Recipe recipe)
        {
            if (recipe == null) return;
            if (!CanBeEffectedByRecipe(recipe)) throw new ArgumentException($@"{Name} 无法被 {recipe.Name} 修饰！");
            if (HasBeenEffectedBySkillRecipe(recipe)) throw new ArgumentException($@"{Name} 已被 {recipe.Name} 修饰！");
            this.ProcessRecipe(recipe);
            AppliedRecipes.Add(recipe.Name);
            recipe.EffectSkillName.Add(Name);
            AddSkillNameTag(recipe.SkillNameTag);
        }

        public SkillInfoItem() : base()
        {
        }

        public SkillInfoItem(SkillInfoItem item) : base(item)
        {
            AP_Coef = item.AP_Coef;
            IgnoreB = item.IgnoreB;
        }

        // 计算派生技能的Key
        public static string CalcDerivedSkillInfoName(string name, SkillBuffGroup group)
        {
            if (group == null || group.IsEmpty) return name;
            var keySuffix = group.GetSkillKeySuffix();
            var result = name + keySuffix;
            return result;
        }

        // 当本技能产生派生时，计算名称
        public string GetDerivedSkillInfoName(SkillBuffGroup group) => CalcDerivedSkillInfoName(Name, group);

        // 就地计算应用了SkillBuff后的技能效果
        private void ApplySkillBuffGroup(SkillBuffGroup group)
        {
            BaseSkillName = Name;
            if (group == null || group.IsEmpty) return;
            var keySuffix = group.GetSkillKeySuffix();
            Name = GetDerivedSkillInfoName(group);
            group.SkillModifiers.ForEach(ApplySkillModifier);
            group.Recipes.ForEach(ApplySkillRecipe);
            UpdateSkillName();
            IsDerived = true;
            AppliedSkillBuffs.AddRange(group.SkillBuffs.Select(sb => sb.ToString()));
        }

        // 创建派生技能

        public SkillInfoItem MakeDerivedSkillInfo(SkillBuffGroup group)
        {
            if (group == null || group.IsEmpty) return this;
            var res = new SkillInfoItem(this);
            res.ApplySkillBuffGroup(group);
            return res;
        }
    }
}