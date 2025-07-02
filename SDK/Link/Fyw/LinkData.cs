using System.Collections;
using UnityEngine;

namespace Fyw
{
    /// <summary>
    /// 联系数据
    /// </summary>
    public class LinkData
    {
        /// <summary>
        /// 名字
        /// </summary>
        private string name;

        /// <summary>
        /// 图标
        /// </summary>
        private string icon;

        /// <summary>
        /// key
        /// </summary>
        private string key;

        /// <summary>
        /// 打开路径
        /// </summary>
        private string path;

        /// <summary>
        /// Icon图像
        /// </summary>
        private Sprite image;

        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get => this.name; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get => this.icon; }

        /// <summary>
        /// 信息
        /// </summary>
        public string Key { get => this.key; }

        /// <summary>
        /// 打开路径
        /// </summary>
        public string Path { get => this.path; }

        /// <summary>
        /// Icon图像
        /// </summary>
        public Sprite Image { get => this.image; set => this.image = value; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="name"></param>
        /// <param name="icon"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        public LinkData(string name, string icon, string key, string path)
        {
            this.name = name;
            this.icon = icon;
            this.key = key;
            this.path = path;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static LinkData Parse(Hashtable table)
        {
            try
            {
                if (null == table)
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
                if (!table.ContainsKey("key"))
                {
                    return null;
                }
                string path;
                string name = table["name"].ToString();
                string icon = table["icon"].ToString();
                string key = table["key"].ToString();
                if (!table.ContainsKey("path"))
                {
                    path = "";
                }
                else
                {
                    path = table["path"].ToString();
                }
                return new LinkData(name, icon, key, path);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.StackTrace);
                return null;
            }
        }
    }
}