
using System.Globalization;

namespace SDK
{
    /// <summary>
    /// 定位处理器
    /// </summary>
    public abstract class PosHandler : NetHandler
    {
        /// <summary>
        /// 定位数据
        /// </summary>
        public PosData PosData { get; protected set; }

        /// <summary>
        /// 唤醒处理
        /// </summary>
        private void Awake()
        {
            this.PosData = new PosData();
        }

        /// <summary>
        /// 调用处理
        /// </summary>
        protected override void Call()
        {
            this.Back();
        }
    }

    /// <summary>
    /// 定位数据
    /// </summary>
    public class PosData
    {
        /// <summary>
        /// IP
        /// </summary>
        private string ip;

        /// <summary>
        /// 城市
        /// </summary>
        private string city;

        /// <summary>
        /// 地区
        /// </summary>
        private string area;

        /// <summary>
        /// 国区
        /// </summary>
        private string cnty;

        /// <summary>
        /// IP
        /// </summary>
        public string IP { get => this.ip; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get => this.city; }

        /// <summary>
        /// 地区
        /// </summary>
        public string Area { get => this.area; }

        /// <summary>
        /// 国区
        /// </summary>
        public string Cnty { get => this.cnty; }

        /// <summary>
        /// 构造
        /// </summary>
        public PosData()
        {
            this.ip = string.Empty;
            this.city = string.Empty;
            this.area = string.Empty;
            this.cnty = RegionInfo.CurrentRegion.Name;
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="city"></param>
        /// <param name="area"></param>
        /// <param name="cnty"></param>
        public void Set(string ip, string city, string area, string cnty)
        {
            this.ip = ip;
            this.city = city;
            this.area = area;
            this.cnty = cnty;
        }
    }
}