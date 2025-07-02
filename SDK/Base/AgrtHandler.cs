
namespace SDK
{
    /// <summary>
    /// 协议处理器
    /// </summary>
    public abstract class AgrtHandler : ServHandler
    {
        /// <summary>
        /// 隐私链接
        /// </summary>
        public string PrivUrl { get; protected set; }

        /// <summary>
        /// 服务链接
        /// </summary>
        public string ServUrl { get; protected set; }
    }
}