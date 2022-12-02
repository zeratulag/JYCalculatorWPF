using JX3CalculatorShared.Class;
using System.Collections.Generic;

namespace JX3CalculatorShared.Src
{
    public class BuffShellInput
    {

        #region 成员

        public readonly NamedAttrs Buff; // 表示自身BUFF+常用BUFF+额外BUFF之和
        public readonly NamedAttrs DeBuff; // 表示目标DEBUFF之和
        public readonly NamedAttrs ItemDT; // 表示单体之和

        public readonly Dictionary<string, NamedAttrs> AllAttrsDict;
        

        #endregion

        public BuffShellInput(NamedAttrs buff, NamedAttrs debuff, NamedAttrs itemDT)
        {
            Buff = buff;
            DeBuff = debuff;
            ItemDT = itemDT;
            AllAttrsDict = new Dictionary<string, NamedAttrs>()
            {
                {"Buff", Buff},
                {"DeBuff", DeBuff},
                {"ItemDT", ItemDT}
            };

        }

    }

}