using JX3CalculatorShared.Utils;
using System.Collections.Generic;

namespace JX3CalculatorShared.Class
{
    public class CoverDFBase
    {
        // 表示通用覆盖率统计

        public readonly Dictionary<string, CoverItem> Data;
        public CoverItem this[string index] => Data[index];

        public CoverDFBase()
        {
            Data = new Dictionary<string, CoverItem>(6);
        }


        /// <summary>
        /// 重置为0
        /// </summary>
        public void Reset()
        {
            foreach (var _ in Data.Values)
            {
                _.Reset();
            }
        }

        /// <summary>
        /// 设定特定对象的覆盖率
        /// </summary>
        /// <param name="key">名称</param>
        /// <param name="normal">常规覆盖率</param>
        /// <param name="xw">心无覆盖率</param>
        public void SetCover(string key, double normal, double xw)
        {
            this[key].Normal = normal;
            this[key].XW = xw;
        }

        /// <summary>
        /// 合并统计表
        /// </summary>
        /// <param name="other">另一个对象</param>
        public void Merge(CoverDFBase other)
        {
            Data.Merge(other.Data);
        }
    }
}