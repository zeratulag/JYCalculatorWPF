using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using PropertyChanged;

namespace JX3CalculatorShared.Class
{
    public abstract class AbsDataFrame<TKey, TValue> : ObservableObject
    {
        public readonly Dictionary<TKey, TValue> Data;

        public Func<TValue, TKey> GetKeyFromValue = null;// 从Value中获取Key的方法

        public TValue[] Values { get; protected set; }

        protected AbsDataFrame(int n = 4)
        {
            // 事先指定大小
            Data = new Dictionary<TKey, TValue>(n);
        }

        protected AbsDataFrame(Func<TValue, TKey> getKeyFromValue)
        {
            // 指定取Key的函数
            GetKeyFromValue = getKeyFromValue;
        }

        protected AbsDataFrame(IEnumerable<TValue> values, Func<TValue, TKey> getKeyFromValue)
        {
            GetKeyFromValue = getKeyFromValue;
            Data = values.ToDictionary(_ => GetKeyFromValue(_), _ => _);
        }
        
        public void ApplyFunc(Action<TValue> act)
        {
            // 对每一个元素应用操作
            foreach (var _ in Data.Values)
            {
                act(_);
            }
        }

        public void GetValues()
        {
            Values = Data.Values.ToArray();
        }

        public TValue this[TKey index] => Data[index]; // 获取
    }
}