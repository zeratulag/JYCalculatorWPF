using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using PropertyChanged;

namespace JX3CalculatorShared.ViewModels
{
    public class MiJiViewModelBase : IconToolTipCheckBoxViewModel
    {
        public int RecipeID;
        public string RawID;
        public int SkillID;
        public int TypeID;
        public int DesignLevel;
        public string Skill_key;
        public string SAt_key1;
        public int SAt_value1;
        public string SkillName;
        public Recipe Recipe;

        public MiJiViewModelBase(RecipeItem item)
        {
            RecipeID = item.RecipeID;
            Name = item.Name;

            RawID = Funcs.MergeIDLevel(item.RecipeID, item.RecipeLevel);
            //DescName = item.DescName;
            DesignLevel = item.DesignLevel;

            SkillID = item.SkillID;
            Skill_key = item.Skill_key;

            TypeID = item.TypeID;
            Quality = item.Quality;
            ShortDesc = item.ShortDesc;

            IconID = item.IconID;
            IconPath = BindingTool.IconID2Path(IconID);

            ToolTip = item.ToolTip + item.GetToolTipTail();

            SAt_key1 = item.SAt_key1;
            SAt_value1 = item.SAt_value1;

            SkillName = GetSkillName(item.DescName);
        }

        [DoNotNotify] public string ShortDesc { get; }
        [DoNotNotify] public int Quality { get; }
        [DoNotNotify] public new string IconPath { get; }

        /// <summary>
        /// 从描述中提取技能名称
        /// </summary>
        /// <param name="descName">描述名，例如 "《乾坤一掷·暴雨梨花针》参悟残页"</param>
        /// <returns>技能名，例如 "暴雨梨花针"</returns>
        public string GetSkillName(string descName)
        {
            return descName.Split("》")[0].Split("·")[1];
        }
    }
}