using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JYCalculator.Src.Class;

namespace JX3CalculatorShared.DB
{
    public class BuffDBBase : IDB<string, KBuff>
    {
        public static readonly ImmutableArray<string> BuffTypes =
            EnumTool.GetStrValues<BuffTypeEnum>().ToImmutableArray(); // Buff分类

        public readonly ImmutableDictionary<string, KBuff> Buff_Self;
        public readonly ImmutableDictionary<string, KBuff> Buff_Normal;
        public readonly ImmutableDictionary<string, KBuff> DeBuff_Normal;
        public readonly ImmutableDictionary<string, KBuff> Buff_Banquet;
        public readonly ImmutableDictionary<string, KBuff> Buff_Extra;
        public readonly ImmutableDictionary<string, KBuff> Buff_ExtraStack;
        public readonly ImmutableDictionary<string, KBuff> Buff_ExtraTrigger;
        public readonly ImmutableDictionary<string, KBuff> Buff_Special;
        public readonly ImmutableDictionary<string, KBuff> Buff_Skill;
        public readonly ImmutableDictionary<string, KBuff> Buff_Equip;
        public readonly ImmutableDictionary<string, KBuff> Buff_EquipSpecialEffect;

        public readonly ImmutableDictionary<string, BuffTypeEnum> TypeMap; // 表明每种buff对应的Type类型的字典
        public readonly ImmutableDictionary<string, string> QiXueToBuff; // 表示奇穴与Buff名称的关联

        public readonly ImmutableDictionary<string, KBuff> AllBuffData; // 基于Key查询所有BUFF的字典
        public readonly ImmutableDictionary<BuffTypeEnum, ImmutableDictionary<string, KBuff>> GroupedBuffDict;
        public readonly ImmutableDictionary<(int ID, int Level), KBuff> AllBuffByIDLevel; // 基于ID和Level查询所有BUFF的字典

        public BuffDBBase(IEnumerable<Buff_dfItem> Buff_df)
        {
            var typeMapb = ImmutableDictionary.CreateBuilder<string, BuffTypeEnum>();
            var qixuetoBuffb = ImmutableDictionary.CreateBuilder<string, string>();

            var buff_self = new Dictionary<string, KBuff>();
            var buff_normal = new Dictionary<string, KBuff>();
            var debuff_normal = new Dictionary<string, KBuff>();
            var buff_banquet = new Dictionary<string, KBuff>();
            var buff_extra = new Dictionary<string, KBuff>();
            var buff_extrastack = new Dictionary<string, KBuff>();
            var buff_extratrigger = new Dictionary<string, KBuff>();
            var buff_special = new Dictionary<string, KBuff>();
            var buff_skill = new Dictionary<string, KBuff>();
            var buff_equip = new Dictionary<string, KBuff>();
            var buff_equipSpecialEffect = new Dictionary<string, KBuff>();

            var all_buff = new Dictionary<string, KBuff>();

            foreach (var buff_dfitem in Buff_df)
            {
                BuffTypeEnum bufftype = buff_dfitem.Type;

                var key = buff_dfitem.Name;
                typeMapb.Add(key, bufftype);

                var value = new KBuff(buff_dfitem);
                var assocaite = buff_dfitem.Associate;

                all_buff.Add(buff_dfitem.BuffID, value);

                if (assocaite.IsNullOrWhiteSpace() || assocaite.IsEmptyOrWhiteSpace())
                {
                }
                else
                {
                    if (assocaite.StartsWith("QX#"))
                    {
                        var qxname = assocaite.Substring(3);
                        qixuetoBuffb.Add(qxname, key);
                    }
                }

                switch (bufftype)
                {
                    case BuffTypeEnum.Buff_Self:
                    {
                        if (value.DescName != "弩箭机关") buff_self.Add(key, value);
                        break;
                    }

                    case BuffTypeEnum.Buff_Normal:
                    {
                        buff_normal.Add(key, value);
                        break;
                    }

                    case BuffTypeEnum.Buff_Banquet:
                    {
                        buff_banquet.Add(key, value);
                        break;
                    }

                    case BuffTypeEnum.DeBuff_Normal:
                    {
                        debuff_normal.Add(key, value);
                        break;
                    }

                    case BuffTypeEnum.Buff_Extra:
                    {
                        buff_extra.Add(key, value);
                        break;
                    }

                    case BuffTypeEnum.Buff_ExtraStack:
                    {
                        buff_extrastack.Add(key, value);
                        break;
                    }

                    case BuffTypeEnum.Buff_ExtraTrigger:
                    {
                        buff_extratrigger.Add(key, value);
                        break;
                    }

                    case BuffTypeEnum.Buff_Special:
                    {
                        buff_special.Add(key, value);
                        break;
                    }

                    case BuffTypeEnum.Buff_Skill:
                    {
                        buff_skill.Add(key, value);
                        break;
                    }

                    case BuffTypeEnum.Buff_Equip:
                    {
                        buff_equip.Add(key, value);
                        break;
                    }

                    case BuffTypeEnum.Buff_EquipSpecialEffect:
                    {
                        buff_equipSpecialEffect.Add(key, value);
                        break;
                    }
                }
            }

            TypeMap = typeMapb.ToImmutable();
            QiXueToBuff = qixuetoBuffb.ToImmutable();

            Buff_Self = buff_self.ToImmutableDictionary();
            Buff_Normal = buff_normal.ToImmutableDictionary();
            DeBuff_Normal = debuff_normal.ToImmutableDictionary();
            Buff_Banquet = buff_banquet.ToImmutableDictionary();
            Buff_Extra = buff_extra.ToImmutableDictionary();
            Buff_ExtraStack = buff_extrastack.ToImmutableDictionary();
            Buff_ExtraTrigger = buff_extratrigger.ToImmutableDictionary();
            Buff_Special = buff_special.ToImmutableDictionary();
            Buff_Skill = buff_skill.ToImmutableDictionary();
            Buff_Equip = buff_equip.ToImmutableDictionary();
            Buff_EquipSpecialEffect = buff_equipSpecialEffect.ToImmutableDictionary();

            AllBuffData = all_buff.ToImmutableDictionary();
            AllBuffByIDLevel = AllBuffData.Values.ToImmutableDictionary(e => (e.ID, e.Level), e => e);

            GroupedBuffDict = new Dictionary<BuffTypeEnum, ImmutableDictionary<string, KBuff>>()
            {
                {BuffTypeEnum.Buff_Self, Buff_Self},
                {BuffTypeEnum.Buff_Normal, Buff_Normal},
                {BuffTypeEnum.DeBuff_Normal, DeBuff_Normal},
                {BuffTypeEnum.Buff_Banquet, Buff_Banquet},
                {BuffTypeEnum.Buff_Extra, Buff_Extra},
                {BuffTypeEnum.Buff_ExtraStack, Buff_ExtraStack},
                {BuffTypeEnum.Buff_ExtraTrigger, Buff_ExtraTrigger},
                {BuffTypeEnum.Buff_Special, Buff_Special},
                {BuffTypeEnum.Buff_Skill, Buff_Skill},
                {BuffTypeEnum.Buff_Equip, Buff_Equip},
                {BuffTypeEnum.Buff_EquipSpecialEffect, Buff_EquipSpecialEffect}
            }.ToImmutableDictionary();
        }

        private IDictionary<string, KBuff> GetDB(BuffTypeEnum bufftype)
        {
            return GroupedBuffDict[bufftype];
        }

        public KBuff Get(string name, BuffTypeEnum bufftype)
        {
            var db = GetDB(bufftype);
            return db[name];
        }

        public KBuff Get(string name)
        {
            var bufftype = GetBuffType(name);
            return Get(name, bufftype);
        }

        public KBuff GetBuffByID(string id) => AllBuffData[id];

        public BaseBuff this[string name] => Get(name);
        public BaseBuff this[string name, BuffTypeEnum bufftype] => Get(name, bufftype);

        public KBuff GetExtra(string name)
        {
            return Buff_Extra[name];
        }


        public BaseBuffGroup GetSingleTypeBaseBuffGroup(IEnumerable<string> names, BuffTypeEnum bufftype)
        {
            var db = GetDB(bufftype);
            var items = from name in names select db[name];
            var res = new BaseBuffGroup(items);
            return res;
        }

        /// <summary>
        /// 判断Buff名称所属类别
        /// </summary>
        /// <param name="name">Buff名称</param>
        /// <returns>Buff所属类别</returns>
        public BuffTypeEnum GetBuffType(string name)
        {
            var res = TypeMap[name];
            return res;
        }

        /// <summary>
        /// 对一组BaseBuff名称，自动分析他们所属的类别，并且返回字典
        /// </summary>
        /// <param name="names">Buff名称列表</param>
        /// <returns></returns>
        public Dictionary<BuffTypeEnum, string[]> DisPatchBaseBuffNames(IEnumerable<string> names)
        {
            var res = new Dictionary<BuffTypeEnum, string[]>();
            var groupedNames = names.GroupBy(GetBuffType);
            res = groupedNames.ToDictionary(group => group.Key, group => group.ToArray());
            return res;
        }

        /// <summary>
        /// 根据Buff名称列表（可以不同类），返回分类的BaseBuffGroup字典
        /// </summary>
        /// <param name="names">Buff名称列表</param>
        /// <returns></returns>
        public Dictionary<BuffTypeEnum, BaseBuffGroup> GetBaseBuffGroupDict(IEnumerable<string> names)
        {
            var res = new Dictionary<BuffTypeEnum, BaseBuffGroup>();
            var dispatchedNames = DisPatchBaseBuffNames(names);
            foreach (var KVP in dispatchedNames)
            {
                var gtype = KVP.Key;
                var gnames = KVP.Value;
                res.Add(gtype, GetSingleTypeBaseBuffGroup(gnames, gtype));
            }

            return res;
        }

        /// <summary>
        /// 根据若干buff名称（可以不同类），返回最终求和的BaseBuffGroup
        /// </summary>
        /// <param name="names">Buff名称列表</param>
        /// <returns></returns>
        public BaseBuffGroup GetSumBaseBuffGroup(IEnumerable<string> names)
        {
            var disPatchedNames = DisPatchBaseBuffNames(names);
            if (disPatchedNames.ContainsKey(BuffTypeEnum.Buff_Extra) ||
                disPatchedNames.ContainsKey(BuffTypeEnum.DeBuff_Normal))
            {
                throw new ArgumentException("不可转换 DeBuff_Normal 或 Buff_Extra！");
            }

            var items = new List<BaseBuff>();
            foreach (var KVP in disPatchedNames)
            {
                var items_i = KVP.Value.Select(x => Get(x, KVP.Key));
                items.AddRange(items_i);
            }

            var res = new BaseBuffGroup(items);
            return res;
        }

        /// <summary>
        /// 获取奇穴中含有哪些自身Buff名称
        /// </summary>
        /// <param name="qiXueNames">奇穴名</param>
        /// <returns>有效自身Buff名称</returns>
        public List<string> GetQiXueBuffs(ISet<string> qiXueNames)
        {
            var res = new List<string>(QiXueToBuff.Count);
            foreach (var KVP in QiXueToBuff)
            {
                if (qiXueNames.Contains(KVP.Key))
                {
                    res.Add(KVP.Value);
                }
            }

            return res;
        }

        public KBuff QueryBuffByID(string buffID)
        {
            return AllBuffData[buffID];
        }

        public KBuff GetBuffByIDLeven(int ID, int Level)
        {
            return AllBuffByIDLevel[(ID, Level)];
        }

        /// <summary>
        /// 通过Record构建Buff
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public BaseBuff GetBaseBuffByRecord(BuffRecord r)
        {
            var buff = GetBuffByIDLeven(r.ID, r.Level);
            var res = buff.Emit(r);
            return res;
        }

        public BaseBuff[] GetBaseBuffByRecords(IEnumerable<BuffRecord> rs)
        {
            var res = rs.Select(GetBaseBuffByRecord).ToArray();
            return res;
        }
    }
}