using JX3CalculatorShared.Class;
using JYCalculator.ViewModels;
using Newtonsoft.Json;


namespace JYCalculator.Class
{
    public class JBBBPZPanel
    {
        #region 成员

        public JBBB BaseJBBB; // 基础数据
        public JBPZPlan Plan { get; set; } // 经过解析的

        [JsonIgnore] public InitInputSav InitInput; // 基于JB信息解析得到的结果

        #endregion

        public JBBBPZPanel(JBBB j)
        {
            BaseJBBB = j;
            Plan = new JBPZPlan(j);
        }

        #region 构造

        public void Parse()
        {
            Plan.Parse();
            GetInitInput();
        }

        /// <summary>
        /// 解析输入面板，包括属性和特效
        /// </summary>
        /// <returns></returns>
        public InitInputSav GetInitInput()
        {
            var initChar = new InitCharacter(this);
            InitInput = new InitInputSav(initChar, Plan.EquipOptionSav, Plan.BigFMConfig, Plan.EquipDict);
            return InitInput;
        }

        #endregion

    }
}