using JX3CalculatorShared.Class;
using System.Collections.Generic;


namespace JX3CalculatorShared.Common
{
    public interface ICatsable : ICatable
    {
        /// <summary>
        /// 表示可以显示，并且可以调用GetCatStrList获得字符串列表的类
        /// </summary>
        IList<string> GetCatStrList();
    }


    public interface ICatable
    {
        /// <summary>
        /// 表示可以显示，并且可以用ToStr()获得显示字符串的类
        /// </summary>
        string ToStr();

        void Cat();
    }


    public interface IDB<TKey, TValue>
    {
        /// <summary>
        /// 表示通用数据库类
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        TValue Get(TKey name);
    }


    public interface IIndex
    {
        void SetIndex(int index);
    }

    public interface IModel
    {
        void Calc();
    }
    public interface ILuaTable
    {
        AttrCollection ParseItem();
    }
}