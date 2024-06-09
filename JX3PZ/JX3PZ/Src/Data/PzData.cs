using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.DB;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JX3PZ.Class;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using static JX3CalculatorShared.Utils.ImportTool;

namespace JX3PZ.Data
{
    public class PzData
    {
        public string Path;

        public ImmutableDictionary<int, Stone> Stone;
        public ImmutableDictionary<int, Enhance> Enhance;
        public ImmutableDictionary<int, PzSet> Set;
        public ImmutableDictionary<int, Enchant> Enchant;

        public ImmutableDictionary<int, Armor> Armor;
        public ImmutableDictionary<int, Weapon> Weapon;
        public ImmutableDictionary<int, Trinket> Trinket;

        public ImmutableDictionary<string, Equip> Equip;
        public ImmutableDictionary<string, PlayerLevelData> LevelData;

        public static int GetEquipID(Equip e) => e.ID;

        public void LoadTime(string path)
        {
            FuncTool.RunTime(Load);
        }

        public PzData(string path)
        {
            Path = path;
        }
        public PzData() { }

        public void LoadBefore()
        {
            LoadEnchant(); // 轻量级加载，共用大附魔
        }

        public void Load()
        {
            LoadStone();
            LoadSet();
            LoadEnhance();
            LoadEquip();
            LoadLevelData();
        }

        public void LoadLevelData()
        {
            LevelData = ReadJSON<Dictionary<string, PlayerLevelData>>(AppStatic.LevelData_Path).ToImmutableDictionary();
        }

        public void LoadStone()
        {
            Stone = ReadSheetAsDict<int, Stone>(Path, "stone", _ => _.ID);
            foreach (var _ in Stone.Values)
            {
                _.Parse();
            }
        }

        public void LoadEnhance()
        {
            Enhance = ReadSheetAsDict<int, Enhance>(Path, "enhance", _ => _.ID);
            foreach (var _ in Enhance.Values)
            {
                _.Parse();
            }
        }

        public void LoadEnchant()
        {
            Enchant = ReadSheetAsDict<int, Enchant>(Path, "enchant", _ => _.ID);  // 大附魔
            //foreach (var _ in Enchant.Values)
            //{
            //    _.Parse();
            //}
        }

        public void LoadEquip()
        {
            Armor = ReadSheetAsDict<int, Armor>(Path, "armor", GetEquipID);
            Weapon = ReadSheetAsDict<int, Weapon>(Path, "weapon", GetEquipID);
            Trinket = ReadSheetAsDict<int, Trinket>(Path, "trinket", GetEquipID);

            var eq = ImmutableDictionary.CreateBuilder<string, Equip>();

            foreach (var _ in Armor.Values)
            {
                _.Parse();
                eq.Add(_.EID, _);
            }

            foreach (var _ in Weapon.Values)
            {
                _.Parse();
                eq.Add(_.EID, _);
            }

            foreach (var _ in Trinket.Values)
            {
                _.Parse();
                eq.Add(_.EID, _);
            }

            Equip = eq.ToImmutable();

        }

        // 生成装备默认的界面，注意完整解析会比较慢，需要放在Task里操作
        public void GetEquipDefaultShow()
        {
            foreach (var e in Equip.Values)
            {
                e.GetDefaultShow();
            }
        }

        public void LoadSet()
        {
            Set = ReadSheetAsDict<int, PzSet>(Path, "set", _ => _.ID);
            foreach (var _ in Set.Values)
            {
                _.Parse();
            }
        }

        public PzSet GetPzSet(int setID)
        {
            return Set[setID];
        }


        // 为装备表添加EquipOption对象
        public void AttachEquipOption(EquipOption option)
        {
            if (option.EquipIDs == null) return;

            foreach (var _ in option.EquipIDs)
            {
                if (Equip.TryGetValue(_, out var e))
                {
                    e.AttachEquipOption(option);
                }
            }
        }

        public void AttachEquipOptions(EquipOptionDBBase db)
        {
            db.WPData.Values.ForEach(AttachEquipOption);
            db.YZData.Values.ForEach(AttachEquipOption);
        }
    }
}