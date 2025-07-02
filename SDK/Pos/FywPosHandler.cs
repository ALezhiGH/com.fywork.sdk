using System.Collections;

namespace SDK
{
    /// <summary>
    /// 泛鱼定位处理器
    /// </summary>
    public class FywPosHandler : PosHandler
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="args"></param>
        protected override void Init(Hashtable args)
        {
            this.url = args["url"].ToString();
            this.StartCoroutine(this.Get(this.url));
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="data"></param>
        protected override void Parse(string data)
        {
            try
            {
                Hashtable table = JsonTool.jsonDecode(data) as Hashtable;
                if (null == table)
                {
                    return;
                }
                if (!table.ContainsKey("ip"))
                {
                    return;
                }
                if (!table.ContainsKey("city"))
                {
                    return;
                }
                if (!table.ContainsKey("region"))
                {
                    return;
                }
                if (!table.ContainsKey("country"))
                {
                    return;
                }
                string ip = table["ip"].ToString();
                string city = table["city"].ToString();
                string area = table["region"].ToString();
                string cnty = table["country"].ToString();
                this.PosData.Set(ip, city, area, cnty);
            }
            catch (System.Exception e)
            {
                LogTool.Error(e.StackTrace);
            }
        }
    }
}