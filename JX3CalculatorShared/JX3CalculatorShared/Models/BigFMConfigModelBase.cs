using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.Src.Data;
using JX3CalculatorShared.Src.DB;
using JX3CalculatorShared.Utils;
using JX3CalculatorShared.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace JX3CalculatorShared.Models
{
    public class BigFMConfigModelBase: IModel
    {
        public BigFM Hat;
        public BigFM Jacket;
        public BigFM Belt;
        public BigFM Wrist;
        public BigFM Shoes;
        public BigFM Bottoms;
        public Dictionary<EquipSubTypeEnum, BigFM> Dict; // 已装备的大附魔效果（类型字典）
        public Dictionary<string, double> SAttrs; // 总属性
        public CharAttrCollection SCharAttr;
        public NamedAttrs AttrsDesc;
        public BigFMConfigArg Arg;
        public List<SkillEventItem> SkillEvents; // 事件

        public BigFMConfigModelBase()
        {
            Dict = new Dictionary<EquipSubTypeEnum, BigFM>();

            foreach (var subtype in BigFMDBBase.SubTypeOrder)
            {
                Dict.Add(subtype, null); // 全置为空
            }

            SAttrs = new Dictionary<string, double>(6);
            Arg = new BigFMConfigArg();
            SkillEvents = new List<SkillEventItem>(6);
        }

        public void Dispatch()
        {
            Hat = Dict[EquipSubTypeEnum.HAT];
            Jacket = Dict[EquipSubTypeEnum.JACKET];
            Belt = Dict[EquipSubTypeEnum.BELT];
            Wrist = Dict[EquipSubTypeEnum.WRIST];
            Shoes = Dict[EquipSubTypeEnum.SHOES];
            //Bottoms = Dict[EquipSubTypeEnum.bottoms];
        }

        public virtual void Calc()
        {
            CalcConfigArg();
        }

        /// <summary>
        /// 计算大附魔头/衣提供的总属性
        /// </summary>
        public void GetSAttrs()
        {
            var names = new List<string>();
            SAttrs.Clear();
            foreach (var KVP in Dict)
            {
                if (KVP.Value?.SAttrs != null && KVP.Value?.SAttrs.Count > 0)
                {
                    SAttrs.ValueDictAppend(KVP.Value.SAttrs);
                    names.Add(KVP.Value.DescName);
                }
            }

            SCharAttr = new CharAttrCollection(SAttrs, null, true);
            AttrsDesc = new NamedAttrs(names, SCharAttr);
            AttrsDesc.ParcelName("大附魔：");
        }

        /// <summary>
        /// 计算大附魔头/衣提供的总属性，考虑已经初始属性有的头/衣
        /// </summary>
        public NamedAttrs GetNamedSAttrs(bool hasHat, bool hasJacket)
        {
            var names = new List<string>();
            var sAttrs = new Dictionary<string, double>(6);
            var realDict = Dict.ToDictionary(_ => _.Key, _ => _.Value);

            if (hasHat)
            {
                realDict.Remove(EquipSubTypeEnum.HAT);
            }

            if (hasJacket)
            {
                realDict.Remove(EquipSubTypeEnum.JACKET);
            }

            foreach (var KVP in realDict)
            {
                if (KVP.Value?.SAttrs != null && KVP.Value?.SAttrs.Count > 0)
                {
                    sAttrs.ValueDictAppend(KVP.Value.SAttrs);
                    names.Add(KVP.Value.DescName);
                }
            }

            var sCharAttr = new CharAttrCollection(sAttrs, null, true);
            var attrsDesc = new NamedAttrs(names, sCharAttr);
            attrsDesc.ParcelName("大附魔：");
            return attrsDesc;
        }

        public void CalcConfigArg()
        {
            Arg.Belt = Belt;
            Arg.Shoes = Shoes;
            Arg.Wrist = Wrist;
        }
    }
}