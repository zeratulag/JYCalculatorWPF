using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace JYCalculator.Class.SkillCount
{
    public class NieJingZhuiMing
    {
        public double[] DuTiaoNieJing; // 读条蹑景计数
        public double[] ShunFaNieJing; // 瞬发蹑景计数, 注意这里把0蹑景瞬发加到蹑景3上

        public readonly double WuSheng; // 无声次数
        public readonly double ZhuiMing; // 追命次数

        public readonly double[] X;

        public static double[] NeiJing1_DuTiao_Coef = new double[2] { 0.118172, 0.231506 };
        public static double[] NeiJing0_3_ShunFa_Coef = new double[2] { -0.361307, 0.681770 };
        public static double ShunFa_Coef = 0.316247;

        public readonly bool IsFreq; // 是否为频率计算
        public Dictionary<string, double> Result; // 输出结果

        /// <summary>
        /// 计算蹑景追命的分布
        /// </summary>
        /// <param name="wuSheng">无声BUFF次数</param>
        /// <param name="zhuiMing">追命总数</param>
        /// <param name="isFreq">输入的是否为频率</param>
        public NieJingZhuiMing(double wuSheng, double zhuiMing, bool isFreq = false)
        {
            WuSheng = wuSheng;
            ZhuiMing = zhuiMing;
            X = new double[2] {WuSheng, ZhuiMing };
            IsFreq = isFreq;
            Result = new Dictionary<string, double>(8);
        }

        /// <summary>
        /// 向量内积
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double VectorProduct(double[] a, double[] b)

        {
            var n = a.Length;
            var res = 0.0;
            for (int i = 0; i < n; i++)
            {
                res += a[i] * b[i];
            }

            return res;
        }


        public void DoWork()
        {
            CalcCount();
            ToDict();
        }

        public void CalcCount()
        {
            var NeiJing1_DuTiao = VectorProduct(X, NeiJing1_DuTiao_Coef);
            var NeiJing0_3_ShunFa = VectorProduct(X, NeiJing0_3_ShunFa_Coef);
            
            var ShunFaTotal = ShunFa_Coef * WuSheng;

            var a = NeiJing1_DuTiao;
            var b = NeiJing0_3_ShunFa;
            var x = ZhuiMing;
            var y = ShunFaTotal;
            var DuTiaoTotal = x - y;

            DuTiaoNieJing = new double[4] {0, a, x - y - a, 0};
            ShunFaNieJing = new double[4] {0, y - a, a - b, b};
        }

        public void ToDict()
        {
            for (int i = 1; i < DuTiaoNieJing.Length; i++)
            {
                var key = $"ZM_NJ{i}";
                var value = DuTiaoNieJing[i];
                Result.Add(key, value);
            }

            for (int i = 1; i < ShunFaNieJing.Length; i++)
            {
                var key = $"ZM_NJ{i}_SF";
                var value = ShunFaNieJing[i];
                Result.Add(key, value);
            }

        }
    }
}