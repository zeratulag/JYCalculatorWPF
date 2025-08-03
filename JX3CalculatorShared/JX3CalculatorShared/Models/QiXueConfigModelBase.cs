using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using System.Collections.Generic;
using System.Linq;


namespace JX3CalculatorShared.Models
{
    public class QiXueConfigModelBase : IModel
    {
        public static readonly int NSLOTS = StaticConst.NumberOfQiXue;
        public string[] QiXueNames;
        public HashSet<string> QiXueNamesSet; // 方便检索用
        public List<Recipe> OtherRecipes;
        public List<string> AssociateKeys;
        public List<SkillEventItem> SkillEvents; // 有效的触发
        public List<SkillModifier> SkillModifiers;
        public List<string> SelfBuffNames; // 有效的自身Buff名称
        public Dictionary<string, bool> Tags;
        public double XWCD { get; protected set; } // TOOD[JY];
        public double XWDuration { get; protected set; }
        public int XWExtraHaste { get; protected set; } // 心无额外加速，204
        public double NormalDuration { get; protected set; } // 常规时间
        public HashSet<SkillBuild> CompatibleSkillBuilds { get; protected set; } // 支持的技能构建

        public bool IsSupport; // 是否是支持的奇穴

        public QiXueConfigModelBase()
        {
            QiXueNames = new string[NSLOTS];
            OtherRecipes = new List<Recipe>(12);
            AssociateKeys = new List<string>(12);

            Tags = new Dictionary<string, bool>(10);
            SkillModifiers = new List<SkillModifier>();
            SkillEvents = new List<SkillEventItem>();
            SelfBuffNames = new List<string>();
            QiXueNamesSet = QiXueNames.ToHashSet();
        }

        public virtual void Calc()
        {
        }

        /// <summary>
        /// 是否激活了奇穴
        /// </summary>
        /// <param name="name">奇穴名，例如"秋风散影"</param>
        public bool Has(string name)
        {
            return QiXueNamesSet.Contains(name);
        }
    }
}