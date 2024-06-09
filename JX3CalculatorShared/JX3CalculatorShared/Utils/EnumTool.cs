using JX3CalculatorShared.Globals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JX3CalculatorShared.Utils
{
    public static class EnumTool
    {
        /// <summary>
        /// 获得一个枚举类中由所有元素字符串的数值
        /// </summary>
        /// <typeparam name="T">枚举类</typeparam>
        /// <returns>由所有元素字符串组成的数组</returns>
        public static string[] GetStrValues<T>()
        {
            var values = Enum.GetValues(typeof(T));
            var result = (from T x in values select x.ToString()).ToArray();
            return result;
        }

        /// <summary>
        /// 获得一个枚举类中由所有元素的值
        /// </summary>
        /// <typeparam name="T">枚举类</typeparam>
        /// <returns>由所有元素组成的IEnumerable</returns>
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static DamageTypeEnum GetDamageType(this DamageSubTypeEnum e)
        {
            if (e == DamageSubTypeEnum.Physics)
            {
                return DamageTypeEnum.Physics;
            }
            else
            {
                return DamageTypeEnum.Magic;
            }
        }

    }
}