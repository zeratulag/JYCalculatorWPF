using JX3CalculatorShared.Common;
using JX3PZ.Class;
using JX3PZ.Data;
using System.Collections.Generic;

namespace JX3PZ.Models
{
    public class EquipStoneModel: IModel
    {
        public Stone CStone;
        public bool HasStone;
        public int DiamondCount;
        public int DiamondIntensity;
        public bool[] IsActive; // 词条激活状态
        public List<AttributeEntry> ActiveAttributeEntries; // 有效词条

        public EquipStoneModel(Stone cStone = null, int diamondCount = 0, int diamondIntensity = 0)
        {
            UpdateFrom(cStone, diamondCount, diamondIntensity);
        }

        public void UpdateFrom(Stone cStone, int diamondCount, int diamondIntensity)
        {
            CStone = cStone;
            HasStone = CStone != null;

            DiamondCount = diamondCount;
            DiamondIntensity = diamondIntensity;
        }

        public void UpdateFrom(PzPlanModel model)
        {
            UpdateFrom(model.PrimaryWeaponModel.CStone, model.DiamondCount, model.DiamondIntensity);
        }

        public void Calc()
        {
            CheckActivate();
            GetActiveEntries();
        }

        public void CheckActivate()
        {
            // 获取激活状态

            if (HasStone)
            {
                IsActive = CStone.Attributes.IsActive(DiamondCount, DiamondIntensity);
            }
            else
            {
                IsActive = null;
            }
        }

        public void GetActiveEntries()
        {
            // 获取激活词条
            ActiveAttributeEntries = new List<AttributeEntry>(5);
            if (HasStone)
            {
                
                for (int i = 0; i < IsActive.Length; i++)
                {
                    if (IsActive[i])
                    {
                        ActiveAttributeEntries.Add(CStone.Attributes.AttributeEntries[i]);
                    }
                }
            }
        }
    }
}