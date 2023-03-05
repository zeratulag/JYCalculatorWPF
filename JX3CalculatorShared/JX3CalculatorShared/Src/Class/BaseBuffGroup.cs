using JX3CalculatorShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JX3CalculatorShared.Class
{
    public class BaseBuffGroup
    {
        #region 成员

        public List<BaseBuff> Items;
        public List<string> Names;
        public List<int> IconIDs;
        public bool IsTarget;
        public IEnumerable<string> DescNames => from _ in Items select _.DescName; // 描述名称

        public CharAttrCollection CharAttr;
        public CharAttrCollection SCharAttr;

        public bool IsEmpty => Items.Count == 0;
        public bool IsEmptyOrNull => (this == null) || IsEmpty;

        public static BaseBuffGroup Empty = new BaseBuffGroup(false); // 表示一个空的Buff组
        public static BaseBuffGroup TargetEmpty = new BaseBuffGroup(true); // 表示一个空的对目标Buff组

        #endregion

        #region 构造

        /// <summary>
        /// 构造BaseBuff组
        /// </summary>
        /// <param name="items">BaseBuff对象</param>
        /// <param name="isTarget">是否作用于目标</param>
        /// <param name="filer">是否按照游戏内覆盖方式计算</param>
        /// <exception cref="ArgumentException"></exception>
        public BaseBuffGroup(IEnumerable<BaseBuff> items, bool isTarget = false, bool filer = false)
        {
            Items = filer ? FilterBaseBuffs(items) : items.ToList();
            var names = (from _ in Items select _.Name).ToList();
            Names = names;

            IconIDs = (from _ in Items select _.IconID).ToList<int>();

            if (!IsEmpty)
            {
                var isTargets = from item in Items select item.IsTarget;
                if (isTargets.GetUniqueNum() > 1)
                {
                    throw new ArgumentException("目标不一致！");
                }

                IsTarget = isTargets.First();

                if (IsTarget != isTarget)
                {
                    throw new ArgumentException("isTarget参数有误！");
                }
            }
            else
            {
                Items = new List<BaseBuff>();
                // 当为空时，需要手动指定是否为目标
                IsTarget = isTarget;
            }

            Calc();
        }


        // 表示空的
        public BaseBuffGroup(bool isTarget = false)
        {
            Items = new List<BaseBuff>();
        }

        #endregion

        #region 修改

        public void Add(BaseBuff item)
        {
            if (item.IsTarget != IsTarget)
            {
                throw new ArgumentException("目标不一致！");
            }

            Items.Add(item);
            Names.Add(item.Name);
            IconIDs.Add(item.IconID);
        }


        public static BaseBuffGroup Sum(IEnumerable<BaseBuffGroup> baseBuffGroups)
        {
            BaseBuffGroup res;
            var items = new List<BaseBuff>();
            foreach (var group in baseBuffGroups)
            {
                if (group != null) items.AddRange(group.Items);
            }

            if (items.Count > 0)
            {
                res = new BaseBuffGroup(items);
            }
            else
            {
                res = BaseBuffGroup.Empty;
            }

            return res;
        }

        public BaseBuff this[int i]
        {
            get => Items[i];
            set => Items[i] = value;
        }

        #endregion

        #region 简化

        /// <summary>
        /// 计算汇总，获得最终的属性求和
        /// </summary>
        public void Calc()
        {
            if (IsEmpty)
            {
                CharAttr = CharAttrCollection.Empty;
                SCharAttr = CharAttrCollection.SimplifiedEmpty;
            }
            else
            {
                var atts = from _ in Items select _.CharAttrs;
                CharAttr = atts.Sum();
                SCharAttr = CharAttr.Simplify();
            }
        }

        #endregion

        public NamedAttrs ToNamedAttrs()
        {
            NamedAttrs res;
            res = this != null ? new NamedAttrs(this) : NamedAttrs.Empty;
            return res;
        }


        /// <summary>
        /// 基于游戏内的Buff覆盖方式，获得有效的最终可叠加Buff列表
        /// </summary>
        /// <param name="items">原始Buff列表</param>
        /// <returns>可以叠加的Buff列表</returns>
        public static List<BaseBuff> FilterBaseBuffs(IEnumerable<BaseBuff> items)
        {
            var res = new List<BaseBuff>(10);

            var appendTypeIntensityDict = new Dictionary<int, List<BaseBuff>>(10);

            foreach (var item in items)
            {
                if (item.AppendType == -1)
                {
                    res.Add(item);
                }
                else
                {
                    appendTypeIntensityDict.AddIntoList(item.AppendType, item);
                }
            }

            BaseBuff maxBaseBuff;
            foreach (var KVP in appendTypeIntensityDict)
            {
                if (KVP.Value.Count > 1)
                {
                    KVP.Value.Sort((_1, _2) => _1.Intensity.CompareTo(_2.Intensity));
                }
                maxBaseBuff = KVP.Value.Last();
                res.Add(maxBaseBuff);
            }

            res.Sort((_1, _2) => _1.Order.CompareTo(_2.Order));

            return res;
        }
    }
}