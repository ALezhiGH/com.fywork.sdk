using System.Collections;
using UnityEngine;

namespace Fyw
{
    /// <summary>
    /// 资质数据
    /// </summary>
    public class CredData
    {
        /// <summary>
        /// ID
        /// </summary>
        private int id;

        /// <summary>
        /// 名称
        /// </summary>
        private string name;

        /// <summary>
        /// 值
        /// </summary>
        private string val;

        /// <summary>
        /// 查询链接
        /// </summary>
        private string url;

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get => this.id; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get => this.name; }

        /// <summary>
        /// 值
        /// </summary>
        public string Val { get => this.val; }

        /// <summary>
        /// 查询链接
        /// </summary>
        public string Url { get => this.url; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <param name="url"></param>
        public CredData(int id, string name, string val, string url)
        {
            this.id = id;
            this.name = name;
            this.val = val;
            this.url = url;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static CredData Parse(Hashtable table)
        {
            try
            {
                if (null == table)
                {
                    return null;
                }
                if (!table.ContainsKey("id"))
                {
                    return null;
                }
                if (!table.ContainsKey("name"))
                {
                    return null;
                }
                if (!table.ContainsKey("val"))
                {
                    return null;
                }
                if (!table.ContainsKey("url"))
                {
                    return null;
                }
                int id = int.Parse(table["name"].ToString());
                string name = table["name"].ToString();
                string val = table["val"].ToString();
                string url = table["url"].ToString();
                return new CredData(id, name, val, url);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.StackTrace);
                return null;
            }
        }
    }
}