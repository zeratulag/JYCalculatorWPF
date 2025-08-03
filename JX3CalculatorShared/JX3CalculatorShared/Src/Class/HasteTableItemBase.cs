using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JYCalculator.Data;

namespace JX3CalculatorShared.Class
{
    public class HasteTableItemBase
    {
        #region 成员
        public string Name { get; protected set; }
        public string SkillName { get; protected set; } // 技能名称
        public int nCount { get; protected set; } // 跳数
        public double RawIntervalTime => RawFrame / StaticConst.FRAMES_PER_SECOND; // 初始单跳时间
        public int RawFrame { get; protected set; } // 初始帧数
        public double RawTime => nCount * RawIntervalTime; // 初始总时间
        public double Frame { get; protected set; } // 加速后帧数
        public double IntervalTime => Frame / StaticConst.FRAMES_PER_SECOND; // 加速后单跳时间
        public double Time => nCount * IntervalTime; // 加速后总时间
        public double XWFrame { get; protected set; } // 心无期间加速后帧数
        public double XWIntervalTime => XWFrame / StaticConst.FRAMES_PER_SECOND; // 心无期间加速后单跳时间
        public double XWTime => nCount * XWIntervalTime; // 心无期间加速后总时间


        public bool IsMultiChannel;

        #endregion

        public HasteTableItemBase()
        {
        }

        public HasteTableItemBase(string name, string skillname, int rawFrame, int ncount, bool isMultiChannel = false)
        {
            // 输入数据
            Name = name;
            SkillName = skillname;
            RawFrame = rawFrame;
            nCount = ncount;
            IsMultiChannel = isMultiChannel;
        }


        public HasteTableItemBase(SkillInfoItemBase info)
        {
            Name = info.Name;
            SkillName = info.SkillName;
            RawFrame = info.Frame;
            IsMultiChannel = info.IsMultiChannel;
        }

        /// <summary>
        /// 复制构造
        /// </summary>
        /// <param name="old"></param>
        public HasteTableItemBase(HasteTableItemBase old)
        {
            Name = old.Name;
            SkillName = old.SkillName;
            RawFrame = old.RawFrame;
            nCount = old.nCount;
            IsMultiChannel = old.IsMultiChannel;
        }



        public HasteTableItemBase Copy()
        {
            return new HasteTableItemBase(this);
        }

        /// <summary>
        /// GCD对象
        /// </summary>
        /// <returns></returns>
        public static HasteTableItemBase GetGCDItem()
        {
            return new HasteTableItemBase("GCD", "公共CD", StaticConst.GCD_FPS, 1);
        }
    }
}