using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JYCalculator.Data;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JYCalculator.DB
{
    public class SkillInfoDB : IDB<string, SkillInfoItem>
    {
        #region 成员

        public readonly ImmutableDictionary<string, SkillInfoItem> Skills;
        public readonly ImmutableDictionary<string, SkillEventItem> Events;

        public RecipeDB RecipeDb;

        public readonly ImmutableDictionary<string, string> Name2CombatName; // 技能名到战斗统计名称的字典
        public ImmutableDictionary<string, ImmutableArray<string>> Skill2Event { get; private set; } // 每个技能能触发哪些事件
        public ImmutableDictionary<string, ImmutableArray<string>> Event2Skill { get; private set; } // 每个事件可以由哪些技能触发

        public ImmutableDictionary<string, ImmutableArray<string>> Skill2Recipe { get; private set; } // 每个技能能吃哪些秘籍
        public ImmutableDictionary<string, ImmutableArray<string>> Recipe2Skill { get; private set; } // 每个秘籍可以由哪些技能生效


        public readonly ImmutableDictionary<string, ImmutableArray<string>> QiXueToEvents; // 奇穴对应的事件（一对多）

        #endregion

        #region 构造

        public SkillInfoDB(IDictionary<string, SkillInfoItem> skillItems,
            IDictionary<string, SkillEventItem> eventItems)
        {

            var data = from kvp in skillItems
                       where kvp.Value.Type != SkillDataTypeEnum.Exclude
                       select kvp;

            Skills = data.ToImmutableDictionary();
            Events = eventItems.ToImmutableDictionary();

            Name2CombatName = Skills.ToImmutableDictionary(_ => _.Key, _ => _.Value.Fight_Name);

            var events = eventItems.ToDict();
            var PZ = SkillEventItem.GetPZEventItemFromSL(events["SL"]);
            events.Add(nameof(PZ), PZ);
            Events = events.ToImmutableDictionary();

            var qx2E = new Dictionary<string, List<string>>();
            foreach (var KVP in Events)
            {
                if (KVP.Value.Type == "奇穴")
                {
                    var QXName = KVP.Value.Associate;
                    qx2E.AddIntoList(QXName, KVP.Key);
                }
            }
            QiXueToEvents = qx2E.ToImmutableDictionary(_ => _.Key, _ => _.Value.ToImmutableArray());
        }

        public SkillInfoDB(XFDataLoader xfDataLoader) : this(xfDataLoader.SkillData, xfDataLoader.SkillEvent)
        {
        }

        public SkillInfoDB() : this(StaticXFData.Data)
        {
        }



        public void AttachRecipeDB(RecipeDB recipeDb)
        {
            RecipeDb = recipeDb;
        }

        public void Process()
        {
            SummarySkillTriggerEvent();
            SummarySkillEffectRecipe();
            AttachEffectSkillsToRecipeDB();
        }

        #endregion

        #region 取出

        public SkillInfoItem Get(string name)
        {
            return Skills[name];
        }
        public SkillInfoItem this[string name] => Get(name);

        public SkillEventItem GetEvent(string name)
        {
            return Events[name];
        }

        #endregion

        #region 方法

        public (IDictionary<string, List<string>> skill2item, IDictionary<string, List<string>> item2skill)
            SummarySkillItem<T>(IDictionary<string, T> dict, Func<SkillInfoItem, T, bool> func)
        {
            var skill2item = new Dictionary<string, List<string>>();
            var item2skill = new Dictionary<string, List<string>>();

            foreach (var skillKVP in Skills)
            {
                var skillName = skillKVP.Key;
                //var skillName = skillKVP.Value.Skill_Name;
                var skillItem = skillKVP.Value;

                foreach (var itemKVP in dict)
                {
                    var itemName = itemKVP.Key;
                    //var itemName = itemKVP.Value.EventName;
                    var Item = itemKVP.Value;

                    if (func(skillItem, Item))
                    {
                        skill2item.AddIntoList(skillName, itemName);
                        item2skill.AddIntoList(itemName, skillName);
                    }

                }
            }

            skill2item.SortValueList();
            item2skill.SortValueList();

            return (skill2item, item2skill);
        }


        /// <summary>
        /// 获取哪些事件可以由哪些技能触发的表
        /// </summary>
        public void SummarySkillTriggerEvent()
        {

            var (skill2event, event2skill) = SummarySkillItem(Events, SkillInfoItemBase.CanTrigger);

            Skill2Event = skill2event.ToImmutableDictionary(_ => _.Key,
                _ => _.Value.ToImmutableArray());

            Event2Skill = event2skill.ToImmutableDictionary(_ => _.Key,
                _ => _.Value.ToImmutableArray());

            foreach (var KVP in Event2Skill)
            {
                Events[KVP.Key].SetTriggerSkillNames(KVP.Value);
            }
        }

        /// <summary>
        /// 获取哪些秘籍可以由哪些技能生效的表
        /// </summary>
        public void SummarySkillEffectRecipe(IDictionary<string, Recipe> recipes)
        {

            var (skill2recipe, recipe2skill) = SummarySkillItem(recipes, SkillInfoItemBase.CanEffectRecipe);

            Skill2Recipe = skill2recipe.ToImmutableDictionary(_ => _.Key,
                _ => _.Value.ToImmutableArray());

            Recipe2Skill = recipe2skill.ToImmutableDictionary(_ => _.Key,
                _ => _.Value.ToImmutableArray());
        }

        public void SummarySkillEffectRecipe()
        {
            SummarySkillEffectRecipe(RecipeDb.Data);
        }

        #endregion
        /// <summary>
        /// 为秘籍DB添加生效技能组字段
        /// </summary>
        public void AttachEffectSkillsToRecipeDB()
        {
            RecipeDb.AttachEffectSkills(Recipe2Skill);
        }


        /// <summary>
        /// 获取奇穴中含有哪些触发事件名称
        /// </summary>
        /// <param name="qiXueNames">奇穴名</param>
        /// <returns>有效触发事件</returns>
        public List<SkillEventItem> GetQiXueEvents(ISet<string> qiXueNames)
        {
            var res = new List<SkillEventItem>();
            foreach (var qxName in qiXueNames)
            {
                if (QiXueToEvents.ContainsKey(qxName))
                {
                    var eventNames = QiXueToEvents[qxName];
                    var events = from _ in eventNames select Events[_];
                    res.AddRange(events);
                }
            }
            return res;
        }

    }
}