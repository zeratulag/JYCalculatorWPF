using JX3PZ.Class;
using JX3PZ.Data;

namespace JX3PZ.Src
{
    public static class StaticPzData
    {
        public static readonly PzData Data;

        public static readonly pzEquipData Equip;

        public static readonly pzEnhanceData Enhance;

        public static readonly pzStoneData Stone;

        static StaticPzData()
        {
            Data = new PzData();
            Equip = new pzEquipData();
            Enhance = new pzEnhanceData();
            Stone = new pzStoneData();
        }

        public static void SetPath(string path)
        {
            Data.Path = path;
        }

        public static void Load()
        {
            Data.Load();
            Equip.Load(Data.Equip.Values);
            Enhance.LoadEnhance(Data.Enhance.Values);
            Stone.Load(Data.Stone.Values);
        }

        public static PzSet GetPzSet(int setID)
        {
            return Data.GetPzSet(setID);
        }
    }
}