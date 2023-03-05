using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Data;
using JYCalculator.DB;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;

namespace JYCalculator.ViewModels
{
    // 单一技能所有秘籍的VM
    public class SkillMiJiViewModel : CollectionViewModel<MiJiViewModel>
    {
        #region 成员

        public readonly string SkillKey;
        public readonly string SkillName;
        public readonly int SkillID;
        public int nChecked { get; set; }
        public readonly List<string> CheckedNames; // 已选中的秘籍名
        public readonly List<string> CheckedRecipeIDs; // 已选中的秘籍ID
        public readonly List<Recipe> CheckedRecipes; // 已选中的秘籍
        public readonly SkillRecipeGroup CheckedRecipeGroup; // 已选中的秘籍组

        public readonly ImmutableDictionary<int, string> RecipeID2Name;

        public readonly ImmutableDictionary<string, MiJiViewModel> RawID2VM;

        private readonly bool IsBaoYu = false; // 是否为暴雨秘籍

        [DoNotNotify]
        public bool HasQiPo => IsBaoYu && CheckedNames.Contains("BY_QiPo"); // 判断是否激活了气魄秘籍

        // 形如 暴雨[0/4]的指示
        public string HeaderName => $"{SkillName} [{nChecked:D1}/4]";
        public bool IsValid => nChecked <= 4;

        public bool IsSupport; // 是否是支持的秘籍

        public SolidColorBrush Color => GetColor();

        #endregion

        #region 构造

        public SkillMiJiViewModel(IEnumerable<MiJiViewModel> items) : base(items)
        {
            SkillKey = Data[0].Skill_key;
            SkillName = Data[0].SkillName;
            SkillID = Data[0].SkillID;
            foreach (var item in items)
            {
                if (item.Skill_key != SkillKey)
                {
                    throw new ArgumentException("必须为相同的SkillKey！");
                }
            }

            RecipeID2Name = items.ToImmutableDictionary(_ => _.RecipeID, _ => _.Name);
            RawID2VM = Data.ToImmutableDictionary(_ => _.RawID, _ => _);

            CheckedNames = new List<string>(10);
            CheckedRecipeIDs = new List<string>(10);
            CheckedRecipes = new List<Recipe>(10);
            CheckedRecipeGroup = new SkillRecipeGroup();


            IsBaoYu = SkillKey == "BY";

            PostConstructor();
        }

        public SkillMiJiViewModel(XFDataBase xfDb, string skillkey) : this(xfDb.Recipe.RecipeMiJiViewModels[skillkey])
        {
        }

        public SkillMiJiViewModel(string skillkey) : this(StaticXFData.DB, skillkey)
        {
        }

        /// <summary>
        /// 从数据库中批量生成
        /// </summary>
        public static IDictionary<string, SkillMiJiViewModel> MakeViewModels(XFDataBase xfDb)
        {
            var res = xfDb.Recipe.RecipeMiJiViewModels.Keys.ToDictionary(skillkey => skillkey,
                skillkey => new SkillMiJiViewModel(skillkey));
            return res.ToImmutableDictionary();
        }

        public static IDictionary<string, SkillMiJiViewModel> MakeViewModels()
        {
            return MakeViewModels(StaticXFData.DB);
        }

        #endregion


        #region 方法

        /// <summary>
        /// 查找哪些秘籍被激活了
        /// </summary>
        public void FindChecked()
        {
            int n = 0;
            CheckedNames.Clear();
            CheckedRecipeIDs.Clear();
            CheckedRecipes.Clear();
            foreach (var vm in Data)
            {
                if (vm.IsChecked)
                {
                    CheckedNames.Add(vm.Name);
                    CheckedRecipeIDs.Add(vm.RawID);
                    CheckedRecipes.Add(vm.Recipe);
                    n += 1;
                }
            }
            nChecked = n;

            CheckedRecipeGroup._Update(CheckedRecipes);
        }

        public SolidColorBrush GetColor()
        {
            var res = new SolidColorBrush(Colors.Black);
            if (!IsValid)
            {
                res = new SolidColorBrush(Colors.Red);
            }

            return res;
        }


        #endregion

        #region 事件处理

        protected override void _Update()
        {
            FindChecked();
            GetColor();
            SendMessage();
        }
        protected void SendMessage()
        {
            if (IsBaoYu) StaticMessager.Send(StaticMessager.BaoYuMiJiChangedMsg);
        }

        #endregion

        #region 导入

        /// <summary>
        /// 取消选中所有秘籍
        /// </summary>
        protected void _UnCheckAll()
        {
            foreach (var item in Data)
            {
                item.IsChecked = false;
            }
        }

        /// <summary>
        /// 只选中列表中的秘籍
        /// </summary>
        /// <param name="RawIDs"></param>
        protected void _SetChecked(IEnumerable<string> RawIDs)
        {
            _UnCheckAll();
            foreach (var rawID in RawIDs)
            {
                if (RawID2VM.ContainsKey(rawID))
                {
                    RawID2VM[rawID].IsChecked = true;
                }
            }
        }

        public void Load(IEnumerable<string> RawIDs)
        {
            ActionUpdateOnce(_SetChecked, RawIDs);
        }

        #endregion

        protected override void _DEBUG()
        {
            Trace.WriteLine($"{SkillName}秘籍：");
            CheckedNames.TraceCat();
        }
    }
}