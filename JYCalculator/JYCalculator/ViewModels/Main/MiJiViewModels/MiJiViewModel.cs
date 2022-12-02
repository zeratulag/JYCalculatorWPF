using JX3CalculatorShared.Src.Data;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Src.DB;

namespace JYCalculator.ViewModels
{
    /// <summary>
    /// 单个秘籍VM
    /// </summary>
    public class MiJiViewModel : MiJiViewModelBase
    {
        #region 成员

        //public readonly string DescName;

        #endregion

        #region 构造

        public MiJiViewModel(RecipeItem item) : base(item)
        {
        }

        public void AttachRecipe(RecipeDB db)
        {
            Recipe = db.Dict[RawID];
        }

        #endregion

        #region 方法

        #endregion
    }
}