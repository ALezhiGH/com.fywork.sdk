using Fyw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDK
{
    /// <summary>
    /// 联系控制器
    /// </summary>
    public class FywLinkHandler : LinkHandler
    {
        /// <summary>
        /// 联系弹窗
        /// </summary>
        protected GameObject link_form;

        /// <summary>
        /// 联系数据
        /// </summary>
        protected List<LinkData> link_datas;

        /// <summary>
        /// 唤醒处理
        /// </summary>
        private void Awake()
        {
            this.link_datas = new List<LinkData>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="args"></param>
        protected override void Init(Hashtable args)
        {
            this.link_datas.Clear();
            this.url = args["url"].ToString();
            this.StartCoroutine(this.Get(this.url + $"?game_id={this.chnl_ctrl.Game}&chnl_id={this.chnl_ctrl.Chnl}"));
        }

        /// <summary>
        /// 调用处理
        /// </summary>
        protected override void Call()
        {
            GameObject canvas = GameObject.Find("Canvas");
            if (null == canvas)
            {
                LogTool.Error("没有找到Canvas");
                this.Back();
                return;
            }
            GameObject obj = Resources.Load("LinkForm") as GameObject;
            this.link_form = GameObject.Instantiate(obj);
            this.link_form.transform.SetParent(canvas.transform);
            this.link_form.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            this.link_form.GetComponent<RectTransform>().sizeDelta = new Vector3(0, 0, 0);
            this.link_form.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            LinkForm form = this.link_form.GetComponent<LinkForm>();
            form.SetClose(this.CloseHandle);
            form.ShowData(this.link_datas);
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
                ArrayList list = table["data"] as ArrayList;
                if (null == list)
                {
                    return;
                }
                LinkData temp;
                foreach (var item in list)
                {
                    temp = LinkData.Parse(item as Hashtable);
                    if (null != temp)
                    {
                        this.link_datas.Add(temp);
                    }
                }
            }
            catch (System.Exception e)
            {
                LogTool.Error(e.StackTrace);
            }
        }

        /// <summary>
        /// 关闭处理
        /// </summary>
        private void CloseHandle()
        {
            if (null != this.link_form)
            {
                Object.Destroy(this.link_form);
            }
            this.Back();
        }
    }
}