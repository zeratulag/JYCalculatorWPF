using System;
using System.Linq;
using JX3CalculatorShared.Common;
using MingYiTools;

namespace JX3CalculatorShared.Models
{
    public class MingYiServerModel : IModel
    {
        public MingYiOwnerInfoEntity[] MingYiOwnerInfoList { get; }
        public MingYiServerInfo[] MingYiServerInfoList { get; private set; } = null;

        public MingYiServerModel(MingYiOwnerInfoEntity[] items)
        {
            MingYiOwnerInfoList = items;
        }

        public void Calc()
        {
            GroupServers();
        }

        public void GroupServers()
        {
            var dict =
                MingYiOwnerInfoList.GroupBy(e => e.ServerName).ToDictionary(g => g.Key, g => g.ToArray());
            MingYiServerInfoList = dict.Select(e => new MingYiServerInfo(e.Key, e.Value)).OrderBy(s => s.ServerName)
                .ToArray();
        }
    }
}