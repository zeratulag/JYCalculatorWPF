using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Data;

namespace JX3CalculatorShared.Src.DB
{
    public class QiXueDBBase: IDB<string, QiXueSkill>
    {
        public ImmutableDictionary<string, QiXueSkill> Data; // 所有奇穴的库
        public ImmutableArray<ImmutableArray<QiXueSkill>> QiXue;

        public QiXueDBBase(IDictionary<string, QiXueItem> qixuedict)
        {
            Data = qixuedict.ToImmutableDictionary(_ => _.Key, _ => new QiXueSkill(_.Value));

            var qixue = new ImmutableArray<QiXueSkill>[StaticData.NumberOfQiXue];

            var qixuegroup = from qixueitem in qixuedict.Values
                group qixueitem by qixueitem.position;
            foreach (var g in qixuegroup)
            {
                qixue[g.Key - 1] = g.OrderBy(_ => _.order).Select(_ => new QiXueSkill(_)).ToImmutableArray();
            }

            QiXue = qixue.ToImmutableArray();

        }

        public QiXueSkill Get(string name)
        {
            return Data[name];
        }

        /// <summary>
        /// 按照
        /// 取出奇穴
        /// </summary>
        /// <param name="position">奇穴重数，1~12</param>
        /// <param name="order">奇穴次数，1~5</param>
        /// <returns>奇穴数据</returns>
        public QiXueSkill Get(int position, int order)
        {
            return QiXue[position - 1][order - 1];
        }

        public QiXueSkill this[string name] => Get(name);
        public QiXueSkill this[int position, int order] => Get(position, order);
    }
}