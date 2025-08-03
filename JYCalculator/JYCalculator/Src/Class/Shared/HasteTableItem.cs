using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JYCalculator.Data;
using JYCalculator.Globals;

namespace JYCalculator.Class
{
    public class HasteTableItem : HasteTableItemBase
    {

        #region 构造函数

        public HasteTableItem(string name, string skillname, int rawFrame, int ncount, bool isMultiChannel = false) : base(name, skillname, rawFrame, ncount, isMultiChannel)
        {
        }

        // 基于动态数据构建
        public HasteTableItem(SkillData data)
        {
            // 通过加速计算的数据
            Name = data.Name;
            SkillName = data.SkillName;
            RawFrame = data.Frame;
            nCount = data.nCount;
            IsMultiChannel = data.Info.IsMultiChannel;
        }

        // 基于静态数据构建
        public HasteTableItem(SkillInfoItemBase info) : base(info)
        {
        }

        /// <summary>
        /// 复制构造
        /// </summary>
        /// <param name="old"></param>
        public HasteTableItem(HasteTableItem old) : base(old)
        {
        }

        #endregion


        #region 计算加速

        /// <summary>
        /// 根据加速值，计算加速
        /// </summary>
        /// <param name="HSP">加速等级</param>
        /// <param name="XWExtraHSP">心无额外加速</param>
        public void CalcHaste(int HSP, int XWExtraHSP = XFStaticConst.XinWuConsts.ExtraHaste)
        {
            Frame = XFStaticConst.CurrentHaste.CalcHasteFrame(RawFrame, nCount, HSP, 0, IsMultiChannel);
            XWFrame = XFStaticConst.CurrentHaste.CalcHasteFrame(RawFrame, nCount, HSP, XWExtraHSP, IsMultiChannel);
        }

        /// <summary>
        /// GCD对象
        /// </summary>
        /// <returns></returns>
        public new static HasteTableItem GetGCDItem()
        {
            return new HasteTableItem("GCD", "公共CD", StaticConst.GCD_FPS, 1);
        }

        #endregion
    }
}