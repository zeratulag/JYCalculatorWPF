using JX3CalculatorShared.Class;
using JYCalculator.ViewModels;
using Newtonsoft.Json;
using System.Diagnostics;


namespace JYCalculator.Class
{
    public class JBPZPanel : JBPZPanelBase
    {
        #region 成员

        public new JBPZPlan EquipList { get; set; }

        [JsonIgnore] public InitInputSav InitInput; // 基于JB信息解析得到的结果

        #endregion

        public new static (bool success, JBPZPanel panel) TryImportFromJSON(string jsontxt)
        {
            JBPZPanel res = null;
            bool success = false;

            try
            {
                res = JsonConvert.DeserializeObject<JBPZPanel>(jsontxt);
            }
            catch
            {
                Trace.Write("格式错误！");
            }

            if (res != null)
            {
                success = true;
            }

            return (success, res);
        }


        #region 构造

        public void Parse()
        {
            EquipList.Parse();
            GetInitInput();
        }

        /// <summary>
        /// 解析输入面板，包括属性和特效
        /// </summary>
        /// <returns></returns>
        public InitInputSav GetInitInput()
        {
            var initchar = new InitCharacter(this);
            InitInput = new InitInputSav(initchar, EquipList.EquipOptionSav, EquipList.BigFMConfig);
            return InitInput;
        }

        #endregion

    }
}