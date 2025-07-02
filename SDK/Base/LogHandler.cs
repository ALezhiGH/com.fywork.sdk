
namespace SDK
{
    /// <summary>
    /// 登录处理器
    /// </summary>
    public abstract class LogHandler : NetHandler
    {
        /// <summary>
        /// 登录数据
        /// </summary>
        public LogData LogData { get; protected set; }

        /// <summary>
        /// 唤醒处理
        /// </summary>
        private void Awake()
        {
            this.LogData = new LogData();
        }
    }

    /// <summary>
    /// 登录数据
    /// </summary>
    public class LogData
    {
        /// <summary>
        /// 名称
        /// </summary>
        private string name;

        /// <summary>
        /// 头像链接
        /// </summary>
        private string head;

        /// <summary>
        /// 访问token
        /// </summary>
        private string token;

        /// <summary>
        /// 用户ID
        /// </summary>
        private string user_id;

        /// <summary>
        /// 用户游戏ID
        /// </summary>
        private string open_id;

        /// <summary>
        /// 用户厂商ID
        /// </summary>
        private string union_id;

        #region 属性

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get => this.name; }

        /// <summary>
        /// 头像链接
        /// </summary>
        public string Head { get => this.head; }

        /// <summary>
        /// 访问token
        /// </summary>
        public string Token { get => this.token; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get => this.user_id; }

        /// <summary>
        /// 用户游戏ID
        /// </summary>
        public string OpenID { get => this.open_id; }

        /// <summary>
        /// 用户厂商ID
        /// </summary>
        public string UnionID { get => this.union_id; }

        #endregion

        /// <summary>
        /// 构造
        /// </summary>
        public LogData()
        {
            this.name = string.Empty;
            this.head = string.Empty;
            this.token = string.Empty;
            this.user_id = string.Empty;
            this.open_id = string.Empty;
            this.union_id = string.Empty;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="name"></param>
        /// <param name="head"></param>
        /// <param name="token"></param>
        /// <param name="user_id"></param>
        /// <param name="open_id"></param>
        /// <param name="union_id"></param>
        public void Set(string name, string head, string token, string user_id, string open_id, string union_id)
        {
            this.name = name;
            this.head = head;
            this.token = token;
            this.user_id = user_id;
            this.open_id = open_id;
            this.union_id = union_id;
        }
    }
}