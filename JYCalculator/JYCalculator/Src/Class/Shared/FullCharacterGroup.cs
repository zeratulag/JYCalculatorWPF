using JX3CalculatorShared.Class;
using JX3CalculatorShared.Utils;
using System.Collections.Generic;

namespace JYCalculator.Class
{
    /// <summary>
    /// 存储一系列面板的类
    /// </summary>
    public class FullCharacterGroup
    {
        #region 成员

        public InitCharacter Init; // 初始面板
        public FullCharacter InitFull; // 从初始面板直接转换得到的
        public FullCharacter Buffed; // 增加了属性增益（Buff，单体，大附魔，水特效等），但是没考虑阵法，也没有考虑自身buff的
        public FullCharacter ZhenBuffed; // 考虑了Buff+阵法，但是没有考虑自身buff（神力弩心催寒）

        public FullCharacter NormalFinal; // +考虑特殊BUFF（神力弩心催寒）的常规阶段面板
        public FullCharacter BigXWFinal; // +考虑特殊BUFF（神力弩心催寒）的心无阶段面板

        public FullCharacter LongXWFinal; // 长时间心无最终
        public FullCharacter ShortXWFinal; // 短时间心无最终


        public AttrCollection BuffAttrs;
        public AttrCollection ZhenFaAttrs;

        public readonly Dictionary<string, FullCharacter> Dict;

        #endregion

        public FullCharacterGroup()
        {
            Dict = new Dictionary<string, FullCharacter>(8);
        }

        public FullCharacterGroup(InitCharacter init) : this()
        {
            Init = init;
        }

        /// <summary>
        /// 保持Buff和阵法不变，仅修改初始面板
        /// </summary>
        public void UpdateInput(InitCharacter init)
        {
            Init = init;
            InitFull = init.ToFullCharacter("初始面板");
            Buffed = null;
            ZhenBuffed = null;
            MakeDict();
        }


        public void UpdateInput(InitCharacter init, AttrCollection buff, AttrCollection zhenfa)
        {
            UpdateInput(init);

            BuffAttrs = buff;
            ZhenFaAttrs = zhenfa;
        }

        public void CalcZhenBuffed()
        {
            GetBuffed();
            GetZhenBuffed();
        }

        public void GetBuffed()
        {
            Buffed = InitFull.Copy();
            Buffed.Name = "+增益";
            Buffed.AddCharAttrCollection(BuffAttrs);
            Dict.SetKeyValue(nameof(Buffed), Buffed);
        }

        public void GetZhenBuffed()
        {
            ZhenBuffed = Buffed.Copy("+增益+阵法");
            ZhenBuffed.AddCharAttrCollection(ZhenFaAttrs);
            Dict.SetKeyValue(nameof(ZhenBuffed), ZhenBuffed);
        }

        public void MakeDict()
        {
            Dict.Clear();
            Dict.SetKeyValue(nameof(InitFull), InitFull);
            Dict.SetKeyValue(nameof(Buffed), Buffed);
            Dict.SetKeyValue(nameof(ZhenBuffed), ZhenBuffed);
        }

        public void MakeFinalDict()
        {
            Dict.SetKeyValue(nameof(NormalFinal), NormalFinal);
            Dict.SetKeyValue(nameof(BigXWFinal), BigXWFinal);
            Dict.SetKeyValue(nameof(LongXWFinal), LongXWFinal);
            Dict.SetKeyValue(nameof(ShortXWFinal), ShortXWFinal);
        }

    }
}