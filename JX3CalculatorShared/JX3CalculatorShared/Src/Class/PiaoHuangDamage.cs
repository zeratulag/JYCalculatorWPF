using System;
using System.Diagnostics;
using Serilog;

namespace JYCalculator.Src.Class
{
    public static class PiaoHuangDamage
    {
        /// <summary>
        /// 计算基础伤害，基于 北天药宗\套路及子技能\北天药宗_飘黄附带额外伤害.lua
        /// </summary>
        /// <param name="nEquipScore">装备分数</param>
        /// <returns>基础伤害值</returns>
        public static double CalcBaseDamage(int nEquipScore)
        {
            // 确保 nEquipScore 不低于 0
            nEquipScore = Math.Max(nEquipScore, 0);
            double nBaseDamage = 0;

            // 计算基础伤害
            //nBaseDamage = 4.5332139409685934 * Math.Pow(nEquipScore, 2) / Math.Pow(10, 9)
            //              + 0.012353329087952482 * nEquipScore
            //              - 1707.7143246673932;

            // 计算基础伤害
            nBaseDamage = 0.0080170644 * nEquipScore - 1388.323959; // 0721技改后修改公式，详见：https://jx3.xoyo.com/index/index.html#/article-details?kid=1334759:~:text=16.%E5%8C%97%E5%A4%A9,%E5%8F%98%E5%8C%96%E6%9B%B4%E5%B9%B3%E6%BB%91%E3%80%82

            // 确保基础伤害不低于 1300
            double res = Math.Max(nBaseDamage, 1300);
            return res;
        }

        /// <summary>
        /// 计算总伤害
        /// </summary>
        /// <param name="nEquipScore">装备分数</param>
        /// <param name="nStackNum">层数</param>
        /// <returns>总伤害值</returns>
        public static double CalcDamage(int nEquipScore, int nStackNum)
        {
            // 获取基础伤害
            double baseDamage = CalcBaseDamage(nEquipScore);
            // 计算总伤害
            double res = Math.Max(nStackNum, 1) * baseDamage * 1.25 * 1.3 * 1.15 * 0.5;
            return res;
        }

    }
}