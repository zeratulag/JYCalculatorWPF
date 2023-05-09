using System.Linq;

namespace JX3PZ.Class
{
    public class EquipGetInfoItem
    {
        public string GetType { get; set; }
        public string[] DescList { get; set; } = null;
        public RaidInfoItem[] RaidInfo { get; set; } = null;

        public string[] ToolTipList { get; private set; } = null;

        public int nRaid { get; private set; } = -1;

        public string Desc { get; private set; } = "";
        public string ToolTip { get; private set; } = "";

        public void Parse()
        {
            if (DescList == null)
            {
                ParseRaid();
            }
            GetDesc();
        }

        public void ParseRaid()
        {
            nRaid = RaidInfo.Length; // 在多少个副本里出现
            DescList = new string[nRaid];
            ToolTipList = new string[nRaid];

            for (int i = 0; i < nRaid; i++)
            {
                var _ = RaidInfo[i];
                _.Parse();
                DescList[i] = _.Map;
                ToolTipList[i] = _.Desc;
            }
        }

        public void GetDesc()
        {
            Desc = string.Join(@" / ", DescList);
            if (ToolTipList == null)
            {
                ToolTipList = DescList;
            }
            ToolTip = string.Join("\n", ToolTipList);
        }

    }


    public class RaidInfoItem
    {
        public string Map { get; set; } // 25人英雄西津渡
        public string[] Boss { get; set; } // 

        public string Desc { get; private set; } // 25人英雄西津渡：苏凤楼，李重茂

        public string GetDesc()
        {
            var boss = string.Join("，", Boss);
            var res = $"{Map}：{boss}";
            Desc = res;
            return res;
        }

        public void Parse()
        {
            GetDesc();
        }
    }

    public class EquipGetInfo
    {
        public EquipGetInfoItem[] EquipInfo { get; set; }


        public string[] DescList { get; private set; }
        public string[] ToolTipList { get; private set; }

        public string Desc { get; private set; }
        public string ToolTip { get; private set; }


        public EquipGetInfo(EquipGetInfoItem[] info)
        {
            EquipInfo = info;
        }


        public void Parse()
        {
            EquipInfo.ForEach(_ => _.Parse());
            GetDesc();
        }

        public void GetDesc()
        {
            int n = EquipInfo.Length;
            DescList = new string[n];
            ToolTipList = new string[n + 1];
            ToolTipList[0] = "装备来源";
            for (int i = 0; i < n; i++)
            {
                DescList[i] = $"[{EquipInfo[i].GetType}] {EquipInfo[i].Desc}";
                ToolTipList[i+1] = $"\r\n[{EquipInfo[i].GetType}]\r\n{EquipInfo[i].ToolTip}";
            }

            Desc = string.Join("\n", DescList);
            ToolTip = string.Join("\n", ToolTipList);
        }

    }
}