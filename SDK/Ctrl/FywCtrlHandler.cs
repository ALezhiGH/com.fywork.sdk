using System.Collections;

namespace SDK
{
    /// <summary>
    /// 泛鱼控制处理器
    /// </summary>
    public class FywCtrlHandler : CtrlHandler
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="args"></param>
        protected override void Init(Hashtable args)
        {
            this.url = args["url"].ToString();
            this.StartCoroutine(this.Get(this.url + $"?game_id={this.chnl_ctrl.Game}&chnl_id={this.chnl_ctrl.Chnl}"));
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
                table = table["data"] as Hashtable;
                if (null == table)
                {
                    return;
                }
                if (!table.ContainsKey("payflag"))
                {
                    return;
                }
                if (!table.ContainsKey("adsflag"))
                {
                    return;
                }
                if (!table.ContainsKey("shrflag"))
                {
                    return;
                }
                if (!table.ContainsKey("saveurl"))
                {
                    return;
                }
                if (!table.ContainsKey("rankurl"))
                {
                    return;
                }
                int pay_flag = int.Parse(table["pay_flag"].ToString());
                int ads_flag = int.Parse(table["ads_flag"].ToString());
                int shr_flag = int.Parse(table["shr_flag"].ToString());
                string save_url = table["save_url"].ToString();
                string rank_url = table["rank_url"].ToString();
                this.CtrlData.Set(pay_flag > 0, ads_flag > 0, shr_flag > 0, save_url, rank_url);
            }
            catch (System.Exception e)
            {
                LogTool.Error(e.StackTrace);
            }
        }
    }
}