namespace JX3PZ.Data
{
    public class StoneCondition
    {
        // 描述五彩石属性激活条件
        public readonly int DiamondType;
        public readonly int Compare;
        public readonly int DiamondCount;  // 全身(五行石)大于等于...颗
        public readonly int DiamondIntensity;  // (五行石)等级和大于等于...颗

        public StoneCondition(int diamondType, int compare, int diamondCount, int diamondIntensity)
        {
            DiamondType = diamondType;
            Compare = compare;
            DiamondCount = diamondCount;
            DiamondIntensity = diamondIntensity;
        }


        /// <summary>
        /// 判断是否满足激活条件
        /// </summary>
        /// <param name="diamondCount">全身五行石个数</param>
        /// <param name="diamondIntensity">全身五行石等级和</param>
        /// <returns></returns>
        public bool IsActive(int diamondCount, int diamondIntensity)
        {
            return diamondCount >= DiamondCount && diamondIntensity >= DiamondIntensity;
        }

    }
}