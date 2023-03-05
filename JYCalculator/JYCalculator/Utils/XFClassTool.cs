using System.Linq;
using JX3CalculatorShared.Models;
using JYCalculator.Data;

namespace JYCalculator.Utils
{
    public static class XFClassTool
    {

        #region QiXueConfigModel

        /// <summary>
        /// 检测当前奇穴方案是否支持计算
        /// </summary>
        /// <returns></returns>
        public static bool IsSupported(this QiXueConfigModelBase qixue, CalcSetting setting)
        {
            bool res = false;
            bool hasEssential = qixue.QiXueNamesSet.IsSupersetOf(setting.EssentialQiXues);
            if (hasEssential)
            {
                bool hasBanned = qixue.QiXueNamesSet.Intersect(setting.BannedQiXues).Any();
                res = hasEssential & !hasBanned;
            }
            return res;
        }

        #endregion
    }
}