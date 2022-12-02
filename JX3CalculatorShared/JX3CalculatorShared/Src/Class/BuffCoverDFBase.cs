using System.Collections.Generic;
using JX3CalculatorShared.Src.Class;

namespace JX3CalculatorShared.Class
{
    public class BuffCoverDFBase
    {
        public Dictionary<string, BuffCoverItem> Data;

        public BuffCoverDFBase()
        {
            var SL = new BuffCoverItem("SL", "神力");
            var BigFM_BELT = new BuffCoverItem("BigFM_BELT", "伤·腰");

            var db = new Dictionary<string, BuffCoverItem>();
            db.Add(nameof(SL), SL);
            db.Add(nameof(BigFM_BELT), BigFM_BELT);

            Data = db;
        }

        public BuffCoverItem this[string index] => Data[index];

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

    }
}