using JYCalculator.Data;
using JYCalculator.Models;
using JYCalculator.Src;
using System.Collections.Generic;
using System.Linq;

namespace JYCalculator.Class
{
    public class DuoZhuiSkillCountItem : JYSkillCountItem
    {
        //public double ZM_NJ; // 读条蹑景追命数
        //public double[] NJZMs; // 蹑景追命分布
        //public double[] SFNJZMs; // 瞬发蹑景追命分布

        //private static readonly double[] Normal_NJ_Weight = {23.0, 22.0, 5.0}; // 常规权重
        //private static readonly double[] Normal_SFNJ_Weight = {5.0, 0, 5.0, 17.0}; // 常规权重

        //private static readonly double[] XW_NJ_Weight = {19, 15, 2.0};
        //private static readonly double[] XW_SFNJ_Weight = {4, 0, 0, 11.0};

        public bool IsBigXW;

        //public double[] NJ_Weight => IsBigXW ? XW_NJ_Weight : Normal_NJ_Weight;
        //public double[] SFNJ_Weight => IsBigXW ? XW_SFNJ_Weight : Normal_SFNJ_Weight;
        public Dictionary<string, double> ZMDict { get; private set; }

        public double _XinWuCast;

        public DuoZhuiSkillCountItem(AbilitySkillNumItem item, QiXueConfigModel qiXue) : base(item, qiXue)
        {
            _XinWuCast = (double) item.XW * qiXue.XWDuration / item.Time ;
        }

        public override void ResetTime(double newTime)
        {
            base.ResetTime(newTime);
            var k = newTime / _Time;
            _XinWuCast *= k;
        }

        public void DoPreWork()
        {
            //SplitZhuiMing();
            //GetNJZMNum();
            //MakeNJZMDict();
        }

        // 拆分读条追命和瞬发追命
        //public void SplitZhuiMing()
        //{
        //    double ShunFa;
        //    double NieJing;

        //    if (IsBigXW)
        //    {
        //        ShunFa = 15;
        //        NieJing = 36;
        //    }
        //    else
        //    {
        //        ShunFa = 27;
        //        NieJing = 50;
        //    }

        //    double denominator = ShunFa + NieJing;

        //    double total = ZM + ZM_SF;
        //    ZM_NJ = total * NieJing / denominator;
        //    ZM_SF = total * ShunFa / denominator;
        //}

        //public double[] GetZMNumByWeightArr(double num, double[] weightArr)
        //{
        //    double[] res = new double[weightArr.Length];
        //    double totalWeight = weightArr.Sum();
        //    for (int i = 0; i < weightArr.Length; i++)
        //    {
        //        res[i] = weightArr[i] * num / totalWeight;
        //    }

        //    return res;
        //}

        //public void GetNJZMNum()
        //{
        //    NJZMs = GetZMNumByWeightArr(ZM_NJ, NJ_Weight);
        //    SFNJZMs = GetZMNumByWeightArr(ZM_SF, SFNJ_Weight);
        //}

        //public Dictionary<string, double> MakeNJZMDict()
        //{
        //    var res = new Dictionary<string, double>(10);
        //    for (int i = 0; i < NJZMs.Length; i++)
        //    {
        //        var key = $"ZM_NJ{i + 1}";
        //        var value = NJZMs[i];
        //        res.Add(key, value);
        //    }
        //    for (int i = 0; i < SFNJZMs.Length; i++)
        //    {
        //        string key = i == 0 ? "ZM_SF" : $"ZM_NJ{i}_SF";
        //        var value = SFNJZMs[i];
        //        res.Add(key, value);
        //    }

        //    ZMDict = res;
        //    return res;
        //}


        public override Dictionary<string, double> ToDict()
        {
            var res = base.ToDict();
            res.Add(nameof(_XinWuCast), _XinWuCast);
            //var zmDict = MakeNJZMDict();
            //res["ZM"] = 0;
            //foreach (var kvp in zmDict)
            //{
            //    res[kvp.Key] = kvp.Value;
            //}
            return res;
        }
    }
}