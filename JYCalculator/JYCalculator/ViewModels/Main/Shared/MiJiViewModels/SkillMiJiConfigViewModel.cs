using JX3CalculatorShared.Class;
using JX3CalculatorShared.Utils;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Data;
using JYCalculator.DB;
using JYCalculator.Globals;
using PropertyChanged;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using JX3CalculatorShared.Globals;

namespace JYCalculator.ViewModels
{
    /// <summary>
    /// 所有技能的所有秘籍VM
    /// </summary>
    public class AllSkillMiJiConfigViewModel : CollectionViewModel<SkillMiJiViewModel>
    {
        #region 成员

        public readonly ImmutableDictionary<string, SkillMiJiViewModel> SkillMiJi;

        public readonly ImmutableDictionary<int, SkillMiJiViewModel> SkillID2VM; // SkillID到VM的字典

        public readonly Dictionary<int, string[]> Config; // 存储了当前秘籍选项

        public readonly HashSet<string> ActivedRecipeIDs; // 存储了当前激活的秘籍ID

        public readonly Dictionary<string, SkillRecipeGroup> SkillRecipeGroups; // 各个技能的秘籍汇总

        [DoNotNotify] public bool HasQiPo => SkillMiJi["BY"].HasQiPo;

        public readonly List<Recipe> OtherRecipes; // 衍生的秘籍

        public bool IsSupport; // 是否是支持的秘籍

        #endregion

        #region 构造

        public AllSkillMiJiConfigViewModel(RecipeDB db)
        {
            var res = db.RecipeMiJiViewModels.Keys.ToDictionary(
                skillkey => skillkey,
                skillkey => new SkillMiJiViewModel(skillkey));
            SkillMiJi = res.ToImmutableDictionary();

            SkillID2VM = SkillMiJi.ToImmutableDictionary(_ => _.Value.SkillID, _ => _.Value);

            Data = SkillMiJi.Values.ToImmutableArray();
            Config = new Dictionary<int, string[]>(6);

            SkillRecipeGroups = new Dictionary<string, SkillRecipeGroup>(6);

            OtherRecipes = new List<Recipe>();
            ActivedRecipeIDs = new HashSet<string>(Config.Count * 5);

            AttachDependVMsOutputChanged();
            PostConstructor();
            SetOutName();
            _Update();
        }

        public AllSkillMiJiConfigViewModel() : this(StaticXFData.DB.Recipe)
        {
        }

        #endregion

        #region 方法

        protected void GetConfig()
        {
            ActivedRecipeIDs.Clear();
            foreach (var KVP in SkillMiJi)
            {
                var RecipeIDs = KVP.Value.CheckedRecipeIDs.ToArray();
                Config[KVP.Value.SkillID] = RecipeIDs;
                ActivedRecipeIDs.AddRange(RecipeIDs);
            }
        }

        public void GetIsSupport()
        {
            IsSupport = ActivedRecipeIDs.IsSupersetOf(StaticXFData.Data.Setting.EssentialMiJis);
        }

        protected void GetRecipeGroups()
        {
            SkillRecipeGroups.Clear();
            foreach (var KVP in SkillMiJi)
            {
                SkillRecipeGroups.Add(KVP.Key, KVP.Value.CheckedRecipeGroup);
            }
        }

        /// <summary>
        /// 获取关联的其他秘籍
        /// </summary>
        protected void GetOtherRecipes()
        {
            OtherRecipes.Clear();

            if (XFAppStatic.XinFaTag == "JY") // 惊羽专属，追命无声20%增伤秘籍
            {
                if (Config.ContainsKey(3096))
                {
                    if (Config[3096].Contains("5144_1"))
                    {
                        OtherRecipes.Add(StaticXFData.GetRecipe("MJ_ShunFaZhuiMingJianZengShang"));
                    }
                }
            }
        }


        protected override void _Update()
        {
            GetConfig();
            GetIsSupport();
            GetRecipeGroups();
            GetOtherRecipes();
            //SendMessage();
        }
        protected void SendMessage()
        {
            StaticMessager.Send(StaticMessager.MiJiChangedMsg);
        }

        protected void _Load(IDictionary<int, string[]> sav)
        {
            foreach (var KVP in sav)
            {
                if (SkillID2VM.ContainsKey(KVP.Key))
                {
                    SkillID2VM[KVP.Key].Load(KVP.Value);
                }
            }
        }

        public void Load(IDictionary<int, string[]> sav)
        {
            ActionUpdateOnce(_Load, sav);
        }

        protected override void _DEBUG()
        {
            Config.TraceCat();
        }

        public Dictionary<int, string[]> Export()
        {
            return Config;
        }

        #endregion
    }
}