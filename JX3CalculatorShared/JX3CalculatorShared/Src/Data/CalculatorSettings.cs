using System.Collections.Immutable;
using JX3CalculatorShared.Utils;

namespace JX3CalculatorShared.Src.Data
{
    public class CalculatorSettings
    {
        #region 导入属性

        public string RawEssentialQiXues { get; set; }
        public string RawBannedQiXues { get; set; }

        #endregion

        #region 生成属性
        public ImmutableHashSet<string> EssentialQiXues { get; protected set; }
        public ImmutableHashSet<string> BannedQiXues { get; protected set; }

        #endregion

        public void Parse()
        {
            EssentialQiXues = StringTool.ParseStringList(RawEssentialQiXues).ToImmutableHashSet();
            BannedQiXues = StringTool.ParseStringList(RawBannedQiXues).ToImmutableHashSet();
        }

    }
}