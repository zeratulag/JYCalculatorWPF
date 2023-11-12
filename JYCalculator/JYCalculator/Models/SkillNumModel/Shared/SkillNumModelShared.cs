using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Models;
using JX3CalculatorShared.Src;
using JYCalculator.Class;
using JYCalculator.Data;
using System.Collections.Generic;

namespace JYCalculator.Models
{
    public partial class SkillNumModel
    {
        #region 成员

        public readonly QiXueConfigModel QiXue;
        public readonly SkillHasteTable SkillHaste;
        public readonly XFAbility XfAbility;
        public readonly bool HasZhen;
        public readonly EquipOptionConfigModel Equip;
        public readonly BigFMConfigModel BigFM;
        public readonly CalculatorShellArg CalcShellArg; // 从CalculatorShell传进来的额外参数
        public double NormalTime;
        public double XWTime;
        public readonly NormalSkillNumModel Normal; // 常规技能数模型
        public readonly BigXWSkillNumModel XW; // 心无技能数
        public double XWCD; // 逐星流下心无的CD时间；

        public SkillNumModel(QiXueConfigModel qixue, SkillHasteTable skillhaste, XFAbility xfAbility,
            EquipOptionConfigModel equip,
            BigFMConfigModel bigfm,
            bool hasZhen, CalculatorShellArg calcShellArg)
        {
            QiXue = qixue;
            SkillHaste = skillhaste;
            XfAbility = xfAbility;

            Equip = equip;
            BigFM = bigfm;
            HasZhen = hasZhen;
            CalcShellArg = calcShellArg;


            HitSkillEvents = new Dictionary<string, SkillEventItem>(10);
            CTSkillEvents = new Dictionary<string, SkillEventItem>(10);
            const string piaohuang = "PiaoHuang"; // 飘黄事件
            HitSkillEvents.Add(piaohuang, StaticXFData.DB.SkillInfo.Events[piaohuang]);

            DispatchSkillEvents(Equip.SkillEvents);
            DispatchSkillEvents(BigFM.SkillEvents);
            DispatchSkillEvents(QiXue.SkillEvents);

            var skillNumModelArg =
                new SkillNumModelArg(HasZhen, CalcShellArg.BuffSpecial.PiaoHuangCover, CalcShellArg.HS);

            XW = new BigXWSkillNumModel(qixue, skillhaste, xfAbility.BigXW, Equip, BigFM, skillNumModelArg)
            { SkillEvents = HitSkillEvents };
            Normal = new NormalSkillNumModel(qixue, skillhaste, xfAbility.Normal, Equip, BigFM, skillNumModelArg)
            { SkillEvents = HitSkillEvents };

            NormalTime = QiXue.NormalDuration;
            XWTime = QiXue.XWDuration;
        }

        public readonly Dictionary<string, SkillEventItem> HitSkillEvents; // 技能触发事件（和会心无关的装备触发事件）
        public readonly Dictionary<string, SkillEventItem> CTSkillEvents; // 技能触发事件（和会心相关的事件，目前由奇穴触发，在本模型中只存储不运算）

        #endregion

        #region 构造

        /// <summary>
        /// 根据触发方式（会心触发还是命中触发）分别放入字典中
        /// </summary>
        /// <param name="items"></param>
        public void DispatchSkillEvents(IEnumerable<SkillEventItem> items)
        {
            foreach (var _ in items)
            {
                if (_.EventType == "CriticalStrike")
                {
                    CTSkillEvents.Add(_.Name, _);
                }
                else
                {
                    HitSkillEvents.Add(_.Name, _);
                }
            }
        }

        #endregion

        public void Calc()
        {
            // 计算基础频率
            XW.Calc();
            Normal.Rest = XW.Rest;
            Normal.Calc();

            // 计算技能相关触发（和会心无关的），注意这部分触发可能会改变技能数
            XW.CalcEvents();
            Normal.CalcEvents();

            Normal.CommonCalcAfter();
            XW.CommonCalcAfter();

            if (AppStatic.XinFaTag == "JY")
            {
                CalcJYEnergyInjection();
                if (QiXue.白雨跳珠)
                {
                    CalcBaiYu();
                }
            }
        }
    }
}