using System.Collections;
using UnityEngine;

namespace Fyw
{
    /// <summary>
    /// 更新数据
    /// </summary>
    public class UpdData
    {
        /// <summary>
        /// 最低版本号
        /// </summary>
        private string ver_min;

        /// <summary>
        /// 最新版本号
        /// </summary>
        private string ver_max;

        /// <summary>
        /// 当前版本号
        /// </summary>
        private string ver_now;

        /// <summary>
        /// 更新链接
        /// </summary>
        private string upd_url;

        /// <summary>
        /// 是否可以更新
        /// </summary>
        public bool CanUpd { get => this.upd_url != string.Empty; }

        /// <summary>
        /// 最新版本号
        /// </summary>
        public string VerNew { get => this.ver_max; }

        /// <summary>
        /// 当前版本号
        /// </summary>
        public string VerNow { get => this.ver_now; }

        /// <summary>
        /// 更新链接
        /// </summary>
        public string UpdUrl { get => this.upd_url; }

        /// <summary>
        /// 是否需要更新
        /// </summary>
        public bool NeedUpd
        {
            get
            {
                if (!this.CanUpd)
                {
                    return false;
                }
                string[] ver_max_nums = this.ver_max.Split('.');
                string[] ver_now_nums = this.ver_now.Split('.');
                if (ver_max_nums.Length < 3 || ver_now_nums.Length < 3)
                {
                    return false;
                }
                if (int.Parse(ver_max_nums[0]) > int.Parse(ver_now_nums[0]))
                {
                    return true;
                }
                else if (int.Parse(ver_max_nums[0]) == int.Parse(ver_now_nums[0]) &&
                    int.Parse(ver_max_nums[1]) > int.Parse(ver_now_nums[1]))
                {
                    return true;
                }
                else if (int.Parse(ver_max_nums[0]) == int.Parse(ver_now_nums[0]) &&
                    int.Parse(ver_max_nums[1]) == int.Parse(ver_now_nums[1]) &&
                    int.Parse(ver_max_nums[2]) > int.Parse(ver_now_nums[2]))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 是否强制更新
        /// </summary>
        public bool ForceUpd
        {
            get
            {
                if (!this.CanUpd)
                {
                    return false;
                }
                string[] ver_min_nums = this.ver_min.Split('.');
                string[] ver_now_nums = this.ver_now.Split('.');
                if (ver_min_nums.Length < 3 || ver_now_nums.Length < 3)
                {
                    return false;
                }
                if (int.Parse(ver_min_nums[0]) > int.Parse(ver_now_nums[0]))
                {
                    return true;
                }
                else if (int.Parse(ver_min_nums[0]) == int.Parse(ver_now_nums[0]) &&
                    int.Parse(ver_min_nums[1]) > int.Parse(ver_now_nums[1]))
                {
                    return true;
                }
                else if (int.Parse(ver_min_nums[0]) == int.Parse(ver_now_nums[0]) &&
                    int.Parse(ver_min_nums[1]) == int.Parse(ver_now_nums[1]) &&
                    int.Parse(ver_min_nums[2]) > int.Parse(ver_now_nums[2]))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="ver_min"></param>
        /// <param name="ver_max"></param>
        /// <param name="upd_url"></param>
        public UpdData(string ver_min, string ver_max, string upd_url)
        {
            this.ver_min = ver_min;
            this.ver_max = ver_max;
            this.upd_url = upd_url;
            this.ver_now = Application.version;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static UpdData Parse(Hashtable table)
        {
            try
            {
                if (null == table)
                {
                    return null;
                }
                if (!table.ContainsKey("min_ver"))
                {
                    return null;
                }
                if (!table.ContainsKey("max_ver"))
                {
                    return null;
                }
                if (!table.ContainsKey("upd_url"))
                {
                    return null;
                }
                string ver_min = table["min_ver"].ToString();
                string ver_max = table["max_ver"].ToString();
                string upd_url = table["upd_url"].ToString();
                return new UpdData(ver_min, ver_max, upd_url);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.StackTrace);
                return null;
            }
        }
    }
}