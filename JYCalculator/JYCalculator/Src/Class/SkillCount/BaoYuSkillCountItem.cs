using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JYCalculator.Data;
using JYCalculator.Globals;
using JYCalculator.Src;
using System;
using System.Collections.Generic;
using JYCalculator.Models;

namespace JYCalculator.Class
{
    /// <summary>
    /// 描述惊羽暴雨技能数/频率模型
    /// </summary>
    public class BaoYuSkillCountItem : JYSkillCountItem
    {
        #region 成员

        // Ability 对象中的标准释放时间

        // 以下属性需要结合SkillNumModel计算得到

        #endregion

        #region 构造

        // 基于输入的技能数据导入
        public BaoYuSkillCountItem(AbilitySkillNumItem item, QiXueConfigModel qiXue) : base(item, qiXue)
        {
            BLEffectNames = new[]
                {nameof(DP), nameof(ZM), nameof(ZM_SF), nameof(ZX), nameof(ZX_DOT), nameof(CXL), nameof(_BYCast), nameof(_BYTotalHitNum)};
        }
        // [TODO] 进一步优化
        #endregion


        // 计算罡风数

        /// <summary>
        /// 循环内加入百里，计算其损失的暴雨逐星
        /// </summary>
        /// <param name="BLFreq">百里目标频率</param>
        /// <param name="BLTime">百里释放时间</param>
        public void ApplyBLFreq(double BLFreq, double BLTime)
        {
            BL = BLFreq * _Time;
            var k = 1 - BLFreq * BLTime;

            DP *= k;
            ZM *= k;
            ZM_SF *= k;

            ZX *= k;
            ZX_DOT *= k;
            CXL *= k;

            _BYCast *= k;
            SetLHBYNums(_BYCast);
        }
    }
}