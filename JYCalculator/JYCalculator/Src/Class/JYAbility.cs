using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.Src.Data;
using JYCalculator.Src.Data;


namespace JYCalculator.Src.Class
{
    public class JYAbility: Ability
    {
        #region 成员

        public new readonly AbilitySkillNumItem Normal;
        public new readonly AbilitySkillNumItem BigXW;

        #endregion

        #region 构造

        public JYAbility(AbilityItem abilityitem, AbilitySkillNumItem normal, AbilitySkillNumItem bigXW):base(abilityitem, normal, bigXW) 
        {
            Normal = normal;
            BigXW = bigXW;
        }

        #endregion

        #region 方法

        

        #endregion


    }
}