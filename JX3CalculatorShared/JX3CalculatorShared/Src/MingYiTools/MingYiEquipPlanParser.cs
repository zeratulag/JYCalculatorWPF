using System;
using LuaUtils;

namespace MingYiTools
{
    public class MingYiEquipPlanParser
    {
        public string RawString { get; private set; }
        public string LuaString { get; private set; }
        public object LuaTable { get; private set; }

        public bool ParseSuccess { get; private set; } = false; // 是否解析成功;

        public MingYiItemData[] Items { get; set; }
        public MingYiPZData Data { get; set; }

        public MingYiEquipPlanParser(string rawString)
        {
            RawString = rawString;
        }

        public void Parse()
        {
            try
            {
                LuaString = LuaTools.DecompressLuaData(RawString);
                LuaTable = LuaTools.ParseLuaTableString(LuaString);
                Items = MingYiItemMapper.ParseLuaItemData(LuaTable);
                foreach (var e in Items)
                {
                    var x = e.GetDiamondLevelArray();
                }

                Data = new MingYiPZData(Items);
                ParseSuccess = true;
            }
            catch (Exception e)
            {
                ParseSuccess = false;
            }
        }
    }
}