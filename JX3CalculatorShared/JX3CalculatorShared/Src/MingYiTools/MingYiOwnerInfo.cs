using System;
using System.Collections.Generic;
using System.Linq;
using LuaUtils;
using SqlSugar;

namespace MingYiTools
{
    [SugarTable("OwnerInfo")]
    public class MingYiOwnerInfo
    {
        [SugarColumn(ColumnName = "ownerkey", IsPrimaryKey = true)]
        public long OwnerKey { get; set; }

        [SugarColumn(ColumnName = "ownername")]
        public string OwnerName { get; set; } // 角色名称

        [SugarColumn(ColumnName = "servername")]
        public string ServerName { get; set; } // 服务器名称

        [SugarColumn(ColumnName = "ownerforce")]
        public int OwnerForce { get; set; } // 门派，7为唐门

        [SugarColumn(ColumnName = "ownerrole")]
        public int OwnerRole { get; set; }

        [SugarColumn(ColumnName = "ownerlevel")]
        public int OwnerLevel { get; set; } // 角色等级

        [SugarColumn(ColumnName = "ownerscore")]
        public string OwnerScore { get; set; }

        [SugarColumn(ColumnName = "ownersuitindex")]
        public int OwnerSuitIndex { get; set; }

        [SugarColumn(ColumnName = "time")] public long Time { get; set; }
        [SugarColumn(ColumnName = "extra")] public string Extra { get; set; }
    }


    public class MingYiOwnerInfoEntity : MingYiOwnerInfo
    {
        public DateTime LastTime { get; set; }
        public Dictionary<int, int> Scores { get; private set; } = null; // 原始装分列表
        public Dictionary<int, int> ValidScores { get; private set; } = null; // 有效的装分，包括1~4套装备

        // 构造函数：接收父类对象，手动复制所有属性
        public MingYiOwnerInfoEntity(MingYiOwnerInfo info)
        {
            if (info == null) return;

            // 1. 手动搬运数据 (虽然写起来累，但运行最快)
            OwnerKey = info.OwnerKey;
            OwnerName = info.OwnerName;
            ServerName = info.ServerName;
            OwnerForce = info.OwnerForce;
            OwnerRole = info.OwnerRole;
            OwnerLevel = info.OwnerLevel;
            OwnerScore = info.OwnerScore;
            OwnerSuitIndex = info.OwnerSuitIndex;
            Time = info.Time;
            Extra = info.Extra;

            // 2. 立即解析扩展字段
            // 这样外部拿到对象时，LastTime 和 Temp 已经有值了
            Parse();
        }

        public void Parse()
        {
            LastTime = DateTimeOffset.FromUnixTimeSeconds(Time).LocalDateTime;
            ParseScores();
        }

        public void ParseScores()
        {
            Scores = new Dictionary<int, int>(6);
            var scores = LuaTableParser.Parse(OwnerScore);
            scores = ((List<object>) scores)[0];
            if (scores is List<object> scoreList)
            {
                for (int i = 0; i < scoreList.Count; i++)
                {
                    long v = (long) scoreList[i];
                    Scores[i + 1] = (int) v;
                    // 安全转换并处理多种数值类型或字符串
                }
            }
            else if (scores is Dictionary<string, object> scoreDict)
            {
                Scores = scoreDict.ToDictionary(e => int.Parse(e.Key), e => (int) (long) e.Value);
            }

            ValidScores = new Dictionary<int, int>(6);
            for (int i = 0; i < 4; i++)
            {
                var v = Scores.TryGetValue(i + 1, out int value) ? value : 0;
                ValidScores.Add(i + 1, v);
            }
        }
    }
}