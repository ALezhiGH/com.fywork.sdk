
namespace SDK
{
    /// <summary>
    /// 屏蔽字处理器
    /// </summary>
    public abstract class PbzHandler : NetHandler
    {
        /// <summary>
        /// 调用处理
        /// </summary>
        /// <param name="call"></param>
        protected override void Call()
        {
            this.Back();
        }

        /// <summary>
        /// 检测是否包含敏感词
        /// </summary>
        /// <param name="info"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public abstract bool CheckWord(string info, out string word);
    }
}