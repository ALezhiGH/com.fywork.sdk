
namespace SDK
{
    /// <summary>
    /// 控制处理器
    /// </summary>
    public abstract class CtrlHandler : NetHandler
    {
        /// <summary>
        /// 控制数据
        /// </summary>
        public CtrlData CtrlData { get; protected set; }

        /// <summary>
        /// 唤醒处理
        /// </summary>
        private void Awake()
        {
            this.CtrlData = new CtrlData();
        }

        /// <summary>
        /// 调用处理
        /// </summary>
        /// <param name="call"></param>
        protected override void Call()
        {
            this.Back();
        }
    }

    /// <summary>
    /// 控制数据
    /// </summary>
    public class CtrlData
    {
        /// <summary>
        /// 付费标识
        /// </summary>
        private bool can_pay;

        /// <summary>
        /// 广告标识
        /// </summary>
        private bool can_ads;

        /// <summary>
        /// 分享标识
        /// </summary>
        private bool can_shr;

        /// <summary>
        /// 保存链接
        /// </summary>
        private string save_url;

        /// <summary>
        /// 排行链接
        /// </summary>
        private string rank_url;

        #region 属性

        /// <summary>
        /// 能否进行付费
        /// </summary>
        public bool CanPay { get => this.can_pay; }

        /// <summary>
        /// 能否显示广告
        /// </summary>
        public bool CanAds { get => this.can_ads; }

        /// <summary>
        /// 能否分享
        /// </summary>
        public bool CanShr { get => this.can_shr; }

        /// <summary>
        /// 能否进行保存
        /// </summary>
        public bool CanSave { get => this.save_url != string.Empty; }

        /// <summary>
        /// 能否显示排行
        /// </summary>
        public bool CanRank { get => this.rank_url != string.Empty; }

        /// <summary>
        /// 保存链接
        /// </summary>
        public string SaveUrl { get => this.save_url; }

        /// <summary>
        /// 排行链接
        /// </summary>
        public string RankUrl { get => this.rank_url; }

        #endregion

        /// <summary>
        /// 构造
        /// </summary>
        public CtrlData()
        {
            this.can_pay = false;
            this.can_ads = false;
            this.can_shr = false;
            this.save_url = string.Empty;
            this.rank_url = string.Empty;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="can_pay"></param>
        /// <param name="can_ads"></param>
        /// <param name="can_shr"></param>
        /// <param name="save_url"></param>
        /// <param name="rank_url"></param>
        public void Set(bool can_pay, bool can_ads, bool can_shr, string save_url, string rank_url)
        {
            this.can_pay = can_pay;
            this.can_ads = can_ads;
            this.can_shr = can_shr;
            this.save_url = save_url;
            this.rank_url = rank_url;
        }
    }
}