using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JYCalculator.Data;
using JYCalculator.ViewModels;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JYCalculator.DB
{
    public class RecipeDB : IDB<string, Recipe>
    {
        #region 成员

        public readonly ImmutableDictionary<string, Recipe> Data; // 基于Name构建的字典

        public readonly ImmutableDictionary<string, Recipe> Dict; // 按照(RecipeID, RecipeLevel)构建的字典

        public readonly ImmutableDictionary<string, Recipe> MiJiData; // 仅仅包括秘籍

        public readonly ImmutableDictionary<string, ImmutableArray<MiJiViewModel>> RecipeMiJiViewModels;

        public readonly ImmutableDictionary<string, ImmutableArray<string>> OtherAssociateName; // 其他Recipe到名称的的关联表
        public readonly ImmutableDictionary<string, ImmutableArray<Recipe>> OtherAssociate; // 其他Recipe的关联表


        #endregion

        #region 构造

        public IDictionary<string, MiJiViewModel[]> DispatchRecipeMiJi(IEnumerable<RecipeItem> recipeMJ)
        {
            var itemgroup = from _ in recipeMJ where !_.IsExclude group _ by _.Skill_key into g select g;

            var itemdict = itemgroup.ToDictionary(
                g => g.Key,
                g => g.Select(i => new MiJiViewModel(i)).
                    OrderBy(i => i.TypeID).
                    ThenBy(i => i.DesignLevel).
                    ThenBy(i => i.SAt_key1).ThenBy(i => i.SAt_value1).
                    ToArray());

            return itemdict;
        }

        public RecipeDB(IEnumerable<RecipeItem> recipeMJ, IEnumerable<RecipeItem> recipeOther)
        {
            var data = recipeMJ.ToDictionary(_ => _.Name,
                _ => new Recipe(_));

            MiJiData = data.ToImmutableDictionary();

            foreach (var recipeOtherItem in recipeOther)
            {
                data.Add(recipeOtherItem.Name, new Recipe(recipeOtherItem));
            }

            Data = data.ToImmutableDictionary();
            Dict = data.ToImmutableDictionary(_ => GlobalFunctions.MergeIDLevel(_.Value.RecipeID, _.Value.RecipeLevel),
                _ => _.Value);

            RecipeMiJiViewModels = DispatchRecipeMiJi(recipeMJ)
                .ToImmutableDictionary(kvp => kvp.Key,
                    kvp => kvp.Value.ToImmutableArray());
            AttachRecipeToVMs();

            OtherAssociateName = FindOtherAssociateName(recipeOther).ToImmutableDictionary(_ => _.Key, _ => _.Value.ToImmutableArray());
            OtherAssociate = FindOtherAssociate().ToImmutableDictionary();

        }

        public RecipeDB(XFDataLoader xfDataLoader) : this(xfDataLoader.RecipeMJ, xfDataLoader.RecipeOther)
        {
        }

        public RecipeDB() : this(StaticXFData.Data)
        {
        }

        public static Dictionary<string, List<string>> FindOtherAssociateName(IEnumerable<RecipeItem> recipeOther)
        {
            var res = new Dictionary<string, List<string>>();
            foreach (var item in recipeOther)
            {
                var key = $"{item.Type}-{item.Associate}";
                if (!res.ContainsKey(key))
                {
                    res.Add(key, new List<string>());
                }

                res[key].Add(item.Name);
            }

            return res;
        }

        public Dictionary<string, ImmutableArray<Recipe>> FindOtherAssociate()
        {
            var res = new Dictionary<string, ImmutableArray<Recipe>>();
            foreach (var KVP in OtherAssociateName)
            {
                var value = KVP.Value.Select(_ => this[_]).ToImmutableArray();
                res.Add(KVP.Key, value);
            }

            return res;
        }

        /// <summary>
        /// 设定各个秘籍生效的技能
        /// </summary>
        /// <param name="recipe2Skill">描述每个秘籍可以对哪些技能生效的字典</param>
        public void AttachEffectSkills(IDictionary<string, ImmutableArray<string>> recipe2Skill)
        {
            foreach (var kvp in recipe2Skill)
            {
                var recipe = Get(kvp.Key);
                recipe.SetEffectSkills(kvp.Value);
            }
        }

        public void AttachRecipeToVMs()
        {
            foreach (var KVP in RecipeMiJiViewModels)
            {
                foreach (var vm in KVP.Value)
                {
                    vm.AttachRecipe(this);
                }
            }
        }

        #endregion

        #region 取出

        public Recipe Get(string name)
        {
            return Data[name];
        }

        public Recipe this[string name] => Get(name);

        public Recipe GetRecipeByIdLevel(int recipeID, int recipeLevel)
        {
            var rawID = $@"{recipeID}_{recipeLevel}";
            return Dict[rawID];
        }

        #endregion
    }
}