using System.Collections.Generic;
using JYCalculator.Globals;
using Newtonsoft.Json;

namespace JYCalculator.Src
{
    public class OptimizedICharResult
    {
        /// <summary>
        /// 用于存储优化初始属性结果
        /// </summary>

        public double CT { get; set; } 
        public double OC { get; set; }

        [JsonIgnore] public double OCPct => OC / JYStaticData.fGP.OC; // 面板破防值
        [JsonIgnore] public double CTPoint => CT * JYStaticData.fGP.CT; // 会心点数

        public double CTProportion { get; set; } // 会心占比


        public double WS { get; set; }
        public double PZ { get; set; }

        [JsonIgnore] public double WSPoint => WS * JYStaticData.fGP.WS; // 无双点数

        public double WSProportion { get; set; } // 无双占比


        public double FinalDPS { get; set; }

        public double DPSIncreasement { get; set; } // 提升量

        public string[] DescStrings; // 描述字符串
        public string Desc;

        public OptimizedICharResult()
        {
            DescStrings = null;
            Desc = null;
        }

        public OptimizedICharResult(FullCharInfo fInfo): base()
        {
            CT = fInfo.Info.ICT;
            OC = fInfo.Info.IOC;
            WS = fInfo.Info.IWS;
            PZ = fInfo.Info.IPZ;

            CTProportion = fInfo.CTProportion;
            WSProportion = fInfo.WSProportion;
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