using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JYCalculator.Data;


namespace JYCalculator.Class
{
    public class XFAbility : Ability
    {
        #region 成员

        public new readonly AbilitySkillNumItem Normal;
        public new readonly AbilitySkillNumItem BigXW;

        #endregion

        #region 构造

        public XFAbility(AbilityItem abilityitem, AbilitySkillNumItem normal, AbilitySkillNumItem bigXW) :
            base(abilityitem, normal, bigXW)
        {
            Normal = normal;
            BigXW = bigXW;
        }

        #endregion

        #region 方法

        #endregion
    }
}