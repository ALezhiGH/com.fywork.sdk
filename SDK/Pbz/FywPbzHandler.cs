using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SDK
{
    /// <summary>
    /// 泛鱼字库处理器
    /// </summary>
    public class FywPbzHandler : PbzHandler
    {
        /// <summary>
        /// 现有敏感词集合
        /// </summary>
        protected HashSet<string> exist_datas;

        /// <summary>
        /// 额外敏感词集合
        /// </summary>
        protected HashSet<string> extra_datas;

        /// <summary>
        /// 唤醒处理
        /// </summary>
        private void Awake()
        {
            this.exist_datas = new HashSet<string>();
            this.extra_datas = new HashSet<string>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="args"></param>
        protected override void Init(Hashtable args)
        {
            this.extra_datas.Clear();
            this.url = args["url"].ToString();
            TextAsset asset = Resources.Load<TextAsset>("words");
            if (null != asset)
            {
                this.Parse(asset.text);
            }
            asset = Resources.Load<TextAsset>("extra");
            if (null != asset)
            {
                string[] datas = asset.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var data in datas)
                {
                    this.extra_datas.Add(data);
                }
            }
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
                this.exist_datas.Clear();
                int start = data.IndexOf("data\":\"");
                int end = data.IndexOf("\"}");
                if (start < 0 || end < 0 || start >= end)
                {
                    return;
                }
                start += 7;//移动到数据位置
                string info = data.Substring(start, end - start);
                info = info.Replace("\\/", "/");
                byte[] bytes = Convert.FromBase64String(info);
                string msg = Encoding.UTF8.GetString(bytes);
                string[] words = msg.Split('|');
                foreach (var word in words)
                {
                    if (word != string.Empty)
                    {
                        this.exist_datas.Add(word);
                    }
                }
            }
            catch (System.Exception e)
            {
                LogTool.Error(e.StackTrace);
            }
        }

        /// <summary>
        /// 检测是否包含敏感词
        /// </summary>
        /// <param name="info"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public override bool CheckWord(string info, out string word)
        {
            word = string.Empty;
            foreach (var item in this.exist_datas)
            {
                if (info.Contains(item))
                {
                    word = item;
                    return true;
                }
            }
            foreach (var item in this.extra_datas)
            {
                if (info.Contains(item))
                {
                    word = item;
                    return true;
                }
            }
            return false;
        }
    }
}