using Fyw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SDK
{
    /// <summary>
    /// 泛鱼推荐控制器
    /// </summary>
    public class FywRecHandler : RecHandler
    {
        /// <summary>
        /// 推荐弹窗
        /// </summary>
        protected GameObject rec_form;

        /// <summary>
        /// 推荐数据
        /// </summary>
        protected List<RecData> rec_datas;

        /// <summary>
        /// 唤醒处理
        /// </summary>
        private void Awake()
        {
            this.rec_datas = new List<RecData>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="args"></param>
        protected override void Init(Hashtable args)
        {
            this.rec_datas.Clear();
            this.url = args["url"].ToString();
            this.StartCoroutine(this.Get(this.url + $"?game_id={this.chnl_ctrl.Game}&chnl_id={this.chnl_ctrl.Chnl}"));
        }

        /// <summary>
        /// 调用处理
        /// </summary>
        protected override void Call()
        {
            GameObject obj = Resources.Load("RecForm") as GameObject;
            this.rec_form = GameObject.Instantiate(obj);
            this.rec_form.transform.SetParent(GameObject.Find("Canvas").transform);
            this.rec_form.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            this.rec_form.GetComponent<RectTransform>().sizeDelta = new Vector3(0, 0, 0);
            this.rec_form.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            RecForm form = this.rec_form.GetComponent<RecForm>();
            form.SetClose(this.CloseHandle);
            form.ShowData(this.rec_datas);
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
                    if (null == list)
                {
                    return;
                }
                RecData temp;
                foreach (var item in list)
                {
                    temp = RecData.Parse(item as Hashtable);
                    if (null != temp && PlatGeneric.CheckApp(temp.Code) <= 0)//只推荐没有的应用
                    {
                        this.rec_datas.Add(temp);
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
            if (null != this.rec_form)
            {
                Object.Destroy(this.rec_form);
            }
            this.Back();
        }
    }
}