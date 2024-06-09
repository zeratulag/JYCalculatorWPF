using JX3CalculatorShared.Class;
using JX3PZ.Globals;
using JX3PZ.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JX3PZ.Models
{
    public class PzPlanModel
    {
        // 配装方案模型

        public EquipSnapShotModel[] SnapShots;

        public int DiamondCount;
        public int DiamondIntensity;

        public PzSetModel SetModel;

        public Dictionary<int, EquipShowSetViewModel> SetViewModels;
        public EquipScore CEquipScore;

        public AttributeEntryCollection EntryCollection;

        public CharacterPanel Panel; // 全面板属性
        public XinFaCharacterPanel XFPanel; // 当前心法面板属性
        public EquipStoneModel EquipStone;
        public EquipSnapShotModel PrimaryWeaponModel => SnapShots[(int)EquipSlotEnum.PRIMARY_WEAPON]; // 主武器的Model
        public EquipSnapShotModel PendantModel => SnapShots[(int)EquipSlotEnum.PENDANT]; // 腰坠的Model

        public PzPlanModel()
        {
            EquipStone = new EquipStoneModel();
            XFPanel = new XinFaCharacterPanel();
        }

        public PzPlanModel(IEnumerable<EquipSnapShotModel> snapShots) : this()
        {
            SnapShots = snapShots.ToArray();
        }

        public PzPlanModel(IEnumerable<PzTabItemViewModel> snapShots) : this()
        {
            UpdateFrom(snapShots);
        }

        public void UpdateFrom(IEnumerable<PzTabItemViewModel> snapShots)
        {
            SnapShots = snapShots.Select(_ => _.EquipSnapShot).ToArray();
        }

        public void Calc()
        {
            GetDiamondCount();
            UpdateStoneModel();
            GetEquipScore();
            GetEntries();
            GetXinFaEntries();
            GetStoneEntries();
#if DEBUG
            Trace.WriteLine("已完成配装计算！");
#endif
        }

        public void GetXinFaEntries()
        {
            // 计算心法加成
            EntryCollection.Append(XinFaAttribute.CurrentXinFa.AttributeEntries);
        }

        public void GetEntries()
        {
            var attributeEntryCollections = SnapShots.Select(_ => _.AttributeEntry?.EntryCollection).ToArray();
            EntryCollection = AttributeEntryCollection.Sum(attributeEntryCollections);
        }

        public void GetStoneEntries()
        {
            EntryCollection.Append(EquipStone.ActiveAttributeEntries);
        }


        private void GetEquipScore()
        {
            // 计算总装分
            var baseScore = 0;
            var strengthScore = 0;
            var enhanceScore = 0;

            foreach (var _ in SnapShots)
            {
                if (_.CEquip != null)
                {
                    baseScore += _.Score.BaseScore;
                    strengthScore += _.Score.StrengthScore;
                    enhanceScore += _.Score.EnhanceScore;
                }
            }

            CEquipScore = new EquipScore(baseScore, strengthScore, enhanceScore);
        }


        // 计算五行石个数
        public void GetDiamondCount()
        {
            var count = 0;
            var intensity = 0;
            foreach (var _ in SnapShots)
            {
                count += _.DiamondCount;
                intensity += _.DiamondIntensity;
            }

            DiamondCount = count;
            DiamondIntensity = intensity;
        }

        public void UpdateStoneModel()
        {
            EquipStone.UpdateFrom(this);
            EquipStone.Calc();
        }

        // 套装效果统计
        public void GetSetCount()
        {
            var items = new List<SetCountItem>(10);
            foreach (var _ in SnapShots)
            {
                if (_.CEquip != null)
                {
                    if (_.CEquip.SetID > 0)
                    {
                        var item = new SetCountItem(_.Position, _.CEquip);
                        items.Add(item);
                    }
                }
            }

            SetModel = new PzSetModel(items);
            SetModel.Calc();
        }

        public void CalcSet()
        {
            GetSetCount();
            GetSetViewModels();

#if DEBUG
            Trace.WriteLine("已完成套装计算！");
#endif
        }

        public void GetSetViewModels()
        {
            var vms = SetModel.GetUpdateViewModels();
            SetViewModels = vms;
        }

        public void GetTotalEntryCollection()
        {
            EntryCollection.Append(SetModel?.SetEntries);
        }

        public void GetCharacterPanel()
        {
            Panel = new CharacterPanel();
            Panel.Calc(EntryCollection.ValueDict);
            XFPanel = new XinFaCharacterPanel(Panel);
        }


        public JBPZEquipSnapshotCollection ExportJBPZEquipSnapshotCollection()
        {
            var data = SnapShots.ToDictionary(_ => _.EquipSlot.ToString(), _ => _.ExportJBPZEquipSnapshot());
            var res = new JBPZEquipSnapshotCollection(data);
            return res;
        }

        public PzMainSav Export()
        {
            var equipList = ExportJBPZEquipSnapshotCollection();
            var title = XFPanel.Name;
            var res = new PzMainSav() { EquipList = equipList, Title = title };
            return res;
        }

    }
}