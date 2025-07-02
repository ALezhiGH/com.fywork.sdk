using System.Collections;
using UnityEngine;

namespace Fyw
{
    /// <summary>
    /// 推荐数据
    /// </summary>
    public class RecData
    {
        /// <summary>
        /// 链接
        /// </summary>
        private string url;

        /// <summary>
        /// 名字
        /// </summary>
        private string name;

        /// <summary>
        /// 图标
        /// </summary>
        private string icon;

        /// <summary>
        /// 包名
        /// </summary>
        private string code;

        /// <summary>
        /// 描述1
        /// </summary>
        private string desc;

        /// <summary>
        /// 优先级
        /// </summary>
        private int prio;

        /// <summary>
        /// Icon图像
        /// </summary>
        private Sprite image;

        /// <summary>
        /// 链接
        /// </summary>
        public string Url { get => this.url; }

        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get => this.name; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get => this.icon; }

        /// <summary>
        /// 包名
        /// </summary>
        public string Code { get => this.code; }

        /// <summary>
        /// 描述1
        /// </summary>
        public string Desc { get => this.desc; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int Prio { get => this.prio; }

        /// <summary>
        /// Icon图像
        /// </summary>
        public Sprite Image { get => this.image; set => this.image = value; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="url"></param>
        /// <param name="name"></param>
        /// <param name="icon"></param>
        /// <param name="code"></param>
        /// <param name="desc"></param>
        /// <param name="prio"></param>
        public RecData(string url, string name, string icon, string code, string desc, int prio)
        {
            this.url = url;
            this.name = name;
            this.icon = icon;
            this.code = code;
            this.desc = desc;
            this.prio = prio;
            this.image = null;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static RecData Parse(Hashtable table)
        {
            try
            {
                if (null == table)
                {
                    return null;
                }
                if (!table.ContainsKey("url"))
                {
                    return null;
                }
                if (!table.ContainsKey("name"))
                {
                    return null;
                }
                if (!table.ContainsKey("icon"))
                {
                    return null;
                }
                if (!table.ContainsKey("code"))
                {
                    return null;
                }
                if (!table.ContainsKey("desc"))
                {
                    return null;
                }
                if (!table.ContainsKey("prio"))
                {
                    return null;
                }
                string url = table["url"].ToString();
                string name = table["name"].ToString();
                string icon = table["icon"].ToString();
                string code = table["code"].ToString();
                string desc = table["desc"].ToString();
                int prio = int.Parse(table["prio"].ToString());
                return new RecData(url, name, icon, code, desc, prio);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.StackTrace);
                return null;
            }
        }
    }
}