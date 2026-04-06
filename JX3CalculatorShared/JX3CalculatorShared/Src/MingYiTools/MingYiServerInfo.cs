using System.Linq;

namespace MingYiTools
{
    public class MingYiServerInfo
    {
        public string ServerName { get; set; } // 区服名称
        public MingYiOwnerInfoEntity[] Items { get; set; }
        public int OwnerCount { get; set; } // 拥有角色数量

        public MingYiServerInfo(string serverName, MingYiOwnerInfoEntity[] items)
        {
            ServerName = serverName;
            Items = items.OrderBy(e => e.OwnerName).ToArray();
            OwnerCount = Items.Length;
        }

    }
}