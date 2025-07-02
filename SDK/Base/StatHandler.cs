
namespace SDK
{
    /// <summary>
    /// 防沉迷处理器
    /// </summary>
    public abstract class StatHandler : ServHandler
    {
        /// <summary>
        /// 调用处理
        /// </summary>
        protected override void Call()
        {
            this.Back();
        }

        /// <summary>
        /// 增加统计
        /// </summary>
        /// <param name="key"></param>
        public abstract void AppendStat(string key);

        /// <summary>
        /// 增加统计
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public abstract void AppendStat(string key, int value);

        /// <summary>
        /// 增加统计
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public abstract void AppendStat(string key, string value);

        /// <summary>
        /// 打开面板
        /// </summary>
        /// <param name="panel_name"></param>
        public abstract void OpenPanel(string panel_name);

        /// <summary>
        /// 关闭面板
        /// </summary>
        /// <param name="panel_name"></param>
        public abstract void ClosePanel(string panel_name);
    }
}