using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using PropertyChanged;

namespace JX3CalculatorShared.ViewModels
{
    public class MiJiViewModelBase : IconToolTipCheckBoxViewModel
    {
        public int RecipeID { get; }
        public string RawID { get; }
        public int SkillID { get; }
        public int TypeID{ get; }
        public int DesignLevel { get; }
        public string Skill_key { get; }
        public string SAt_key1 { get; }
        public int SAt_value1 { get; }
        public string SkillName { get; }
        public Recipe Recipe { get; protected set; }
        public int SkillIconID { get; }

        public string SkillToolTip { get; }

        public MiJiViewModelBase(RecipeItem item)
        {
            RecipeID = item.RecipeID;
            Name = item.Name;

            RawID = GlobalFunctions.MergeIDLevel(item.RecipeID, item.RecipeLevel);
            //DescName = item.DescName;
            DesignLevel = item.DesignLevel;
            SkillIconID = item.SkillIconID;
            SkillID = item.SkillID;
            Skill_key = item.Skill_key;

            TypeID = item.TypeID;
            Quality = item.Quality;
            ShortDesc = item.ShortDesc;
            SkillToolTip = item.SkillToolTip;

            IconID = item.IconID;

            ToolTip = item.ToolTip + item.GetToolTipTail();

            SAt_key1 = item.SAt_key1;
            SAt_value1 = item.SAt_value1;

            SkillName = GetSkillName(item.DescName);
        }

        [DoNotNotify] public string ShortDesc { get; }
        [DoNotNotify] public int Quality { get; }

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