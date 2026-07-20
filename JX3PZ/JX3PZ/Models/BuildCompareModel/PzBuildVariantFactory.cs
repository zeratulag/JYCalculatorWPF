using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3PZ.Class;
using JYCalculator.Models;
using System.Linq;
using JX3PZ.Globals;

namespace JX3PZ.Models
{
    public class PzBuildVariantFactory
    {
        public InitInputSav CreateEquipInitInput(PzPlanModel sourceModel, int position, Equip equip)
        {
            var collection = CreateEquipSnapshotCollection(sourceModel, position, equip);
            return CreateInitInput(collection, sourceModel.XFPanel?.Name);
        }

        public InitInputSav CreateEquipInitInput(JBPZEquipSnapshotCollection sourceCollection, int position,
            Equip equip, string title)
        {
            var collection = CreateEquipSnapshotCollection(sourceCollection, position, equip);
            return CreateInitInput(collection, title);
        }

        private JBPZEquipSnapshotCollection CreateEquipSnapshotCollection(PzPlanModel sourceModel, int position,
            Equip equip)
        {
            var source = sourceModel.ExportJBPZEquipSnapshotCollection();
            return CreateEquipSnapshotCollection(source, position, equip);
        }

        private JBPZEquipSnapshotCollection CreateEquipSnapshotCollection(JBPZEquipSnapshotCollection source,
            int position, Equip equip)
        {
            if (source.Dict == null)
            {
                source.GetDict();
            }

            var data = source.Dict.ToDictionary(kvp => kvp.Key, kvp => CopySnapshot(kvp.Value));
            var slot = ((EquipSlotEnum)position).ToString();
            var target = data.ContainsKey(slot) ? data[slot] : new JBPZEquipSnapshot();
            if (target == null)
            {
                target = new JBPZEquipSnapshot();
            }

            target.id = equip?.EID;
            data[slot] = target;

            return new JBPZEquipSnapshotCollection(data);
        }

        private InitInputSav CreateInitInput(JBPZEquipSnapshotCollection collection, string title)
        {
            var model = new PzBuildToInitInputModel(collection, title);
            return model.Calc();
        }

        private static JBPZEquipSnapshot CopySnapshot(JBPZEquipSnapshot source)
        {
            if (source == null)
            {
                return null;
            }

            return new JBPZEquipSnapshot
            {
                id = source.id,
                stone = source.stone,
                enchant = source.enchant,
                enhance = source.enhance,
                strength = source.strength,
                embedding = source.embedding?.ToArray()
            };
        }
    }
}
