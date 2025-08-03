using JX3CalculatorShared.Globals;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace JX3CalculatorShared.Class
{
    public class OptimizedICharResultBase
    {

        /// <summary>
        /// 用于存储优化初始属性结果
        /// </summary>
        public double CT { get; set; }
        public double OC { get; set; }
        [JsonIgnore] public double OCPct => OC / StaticConst.CurrentLevelGlobalParams.Overcome; // 面板破防值
        [JsonIgnore] public double CTPoint => CT * StaticConst.CurrentLevelGlobalParams.CriticalStrike; // 会心点数
        public double CTProportion { get; set; } // 会心占比
        public double WS { get; set; }
        public double PZ { get; set; }
        [JsonIgnore] public double WSPoint => WS * StaticConst.CurrentLevelGlobalParams.Strain; // 无双点数
        public double WSProportion { get; set; } // 无双占比
        public double FinalDPS { get; set; }
        public double DPSIncreasement { get; set; } // 提升量

        public string[] DescStrings; // 描述字符串
        public string Desc;

        public OptimizedICharResultBase()
        {
            DescStrings = null;
            Desc = null;
        }

        protected void GetDescStrings()
        {
            var res = new List<string>
            {
                $"{CT:P2} 会心，{OC:F0}({OCPct:P2}) 破防，会心占比 {CTProportion:P2}",
                $"{WS:P2} 无双，{PZ:F0} 破招，无双占比 {WSProportion:P2}",
                $"最优DPS：{FinalDPS:F0}，提升 {DPSIncreasement:P2}"
            };
            DescStrings = res.ToArray();
        }

        public void Proceed()
        {
            GetDescStrings();
            Desc = DescStrings.Join('\n');
        }
    }
}