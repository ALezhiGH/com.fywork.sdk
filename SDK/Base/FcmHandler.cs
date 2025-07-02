
namespace SDK
{
    /// <summary>
    /// 防沉迷处理器
    /// </summary>
    public abstract class FcmHandler : ServHandler
    {
        protected bool _complianced;

        public string UserId { get; set; }

        public bool Complianced { get => _complianced; }
    }
}