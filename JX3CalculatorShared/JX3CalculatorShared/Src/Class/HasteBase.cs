using JX3CalculatorShared.Globals;
using System;
using System.Collections.Generic;

namespace JX3CalculatorShared.Class
{
    public struct ChannelSkillHastResult
    {
        public int TotalFrame; // 总帧数
        public int nCount;
        public double Frame => (double) TotalFrame / nCount; // 平均每一跳的间隔帧数

        public ChannelSkillHastResult(int totalFrame, int ncount)
        {
            TotalFrame = totalFrame;
            nCount = ncount;
        }

        public double Time => Frame / StaticConst.FRAMES_PER_SECOND; // 平均每一跳的间隔时间
        public double TotalTime => (double) TotalFrame / StaticConst.FRAMES_PER_SECOND;
    }

    public class HasteBase
    {
        public const int MAX_G_HSP = 256;
        public const double MAX_HS = MAX_G_HSP / 1024.0;
        public double fHS;

        public HasteBase(double fhs)
        {
            fHS = fhs;
        }

        public HasteBase(int level)
        {
            var baseHS = BaseGlobalParams.Haste;
            var levelf = GlobalParams.LevelFactor(level);
            fHS = baseHS * levelf;
        }

        public int gHSP(int HSP, int extraHSP)
        {
            int gHsp = (int) (HSP * 1024 / fHS);
            int res = gHsp + extraHSP;
            return res;
        }

        public int Final_gHSP(int HSP, int extraHSP)
        {
            int gHsp = gHSP(HSP, extraHSP);
            int res = Math.Min(MAX_G_HSP, gHsp);
            return res;
        }

        /// <summary>
        /// 计算非导引导技能加速后的读条帧数
        /// </summary>
        /// <param name="frames">初始读条时间（帧）</param>
        /// <param name="HSP">加速等级</param>
        /// <param name="extraHSP">额外加速率（常数）</param>
        /// <returns>加速后的读条时间（帧）</returns>
        public int CalcNormalHasteFrame(int frames, int HSP, int extraHSP = 0)
        {
            int gHsp = Final_gHSP(HSP, extraHSP);
            int finalFrames = (int) (frames * 1024.0 / (1024 + gHsp));
            return finalFrames;
        }


        public double CalcChannelHasteFrame(int frames, int count, int HSP, int extraHSP = 0)
        {
            var res = CalcChannelSkillHasteResult(frames, count, HSP, extraHSP);
            return res.Frame;
        }

        public double CalcHasteFrame(int frames, int count, int HSP, int extraHSP = 0, bool isChannel = false)
        {
            double res = 0;
            if (isChannel)
            {
                res = CalcChannelHasteFrame(frames, count, HSP, extraHSP);
            }
            else
            {
                res = (double) CalcNormalHasteFrame(frames, HSP, extraHSP);
            }
            return res;
        }

        /// <summary>
        /// 计算引导类技能的加速时间
        /// </summary>
        /// <param name="frames"></param>
        /// <param name="count"></param>
        /// <param name="HSP"></param>
        /// <param name="extraHSP"></param>
        /// <returns></returns>
        public ChannelSkillHastResult CalcChannelSkillHasteResult(int frames, int count, int HSP, int extraHSP = 0)
        {
            int rawTotalFrames = frames * count;
            int finalTotalFrames = CalcNormalHasteFrame(rawTotalFrames, HSP, extraHSP);
            var res = new ChannelSkillHastResult(finalTotalFrames, count);
            return res;
        }


        /// <summary>
        /// 计算加速后的读条时间
        /// </summary>
        /// <param name="time">初始读条时间（秒）</param>
        /// <param name="HSP">加速等级</param>
        /// <param name="extraHSP">额外加速率（常数）</param>
        /// <returns>加速后的读条时间（秒）</returns>
        public double CalcHasteTime(double time, int HSP, int extraHSP = 0)
        {
            int fps = (int) Math.Round(time * StaticConst.FRAMES_PER_SECOND);
            int finalFps = CalcNormalHasteFrame(frames: fps, HSP: HSP, extraHSP: extraHSP);
            double res = finalFps / StaticConst.FRAMES_PER_SECOND;
            return res;
        }


        public (Dictionary<int, int> FPS2HSP, Dictionary<int, double> HSP2Time) Threshold_fps(
            int fps = StaticConst.GCD_FPS,
            int extra_HSP = 0)
        {
            var fps2hsp = new Dictionary<int, int>(); // 最终帧数: 加速等级
            var hsp2time = new Dictionary<int, double>(); // 加速等级：最终读条时间
            int hsp = 0;
            int final_fps;
            while (true)
            {
                final_fps = CalcNormalHasteFrame(frames: fps, HSP: hsp, extraHSP: 0);
                if (!fps2hsp.ContainsKey(final_fps))
                {
                    fps2hsp.Add(final_fps, hsp);
                    hsp2time.Add(hsp, final_fps / StaticConst.FRAMES_PER_SECOND);
                }

                if (gHSP(HSP: hsp, extraHSP: 0) > 256)
                    break;
                ++hsp;
            }

            return (fps2hsp, hsp2time);
        }

        public double Calc_GCD_time(int HSP, int extra_HSP = 0)
        {
            var res = CalcHasteTime(StaticConst.GCD, HSP, extra_HSP);
            return res;
        }

        /// <summary>
        /// 计算技能的加速时间表
        /// </summary>
        /// <param name="interval">初始时间，单位秒</param>
        /// <param name="count">技能段数</param>
        /// <param name="HSP">加速等级</param>
        /// <param name="extra_HSP">额外加速率</param>
        /// <returns></returns>
        public SkillHasteTime Calc_SkillTime(double interval, int count, int HSP, int extra_HSP = 0)
        {
            double finalinterval = CalcHasteTime(interval, HSP, extra_HSP);
            double finaltime = count * finalinterval;
            double rawtime = count * interval;
            var res = new SkillHasteTime()
            {
                Count = count,
                RawInterval = interval,
                RawTime = rawtime,
                FinalInterval = finalinterval,
                FinalTime = finaltime
            };
            return res;
        }

        /// <summary>
        /// 计算一段GCD的加速阈值
        /// </summary>
        public int GetT1GCD_HSP()
        {
            var (fps2Hsp, hsp2Time) = Threshold_fps(StaticConst.GCD_FPS, 0);
            return fps2Hsp[StaticConst.GCD_FPS - 1];
        }
    }

    public class SkillHasteTime
    {
        /// <summary>
        /// 用于描述单个技能在加速作用后的时间的类
        /// </summary>
        public int Count;

        public double RawInterval, RawTime;
        public double FinalInterval, FinalTime;
    }
}