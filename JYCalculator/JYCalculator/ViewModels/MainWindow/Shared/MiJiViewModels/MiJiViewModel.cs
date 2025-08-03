using JX3CalculatorShared.Data;
using JX3CalculatorShared.ViewModels;
using JYCalculator.DB;

namespace JYCalculator.ViewModels
{
    /// <summary>
    /// 单个秘籍VM
    /// </summary>
    public class MiJiViewModel : MiJiViewModelBase
    {

        #region 构造
        public MiJiViewModel(RecipeItem item) : base(item)
        {
        }

        public void AttachRecipe(RecipeDB db)
        {
            Recipe = db.Dict[RawID];
        }

        #endregion
    }
}