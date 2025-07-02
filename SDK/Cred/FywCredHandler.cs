using Fyw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDK
{
    /// <summary>
    /// 泛鱼资质控制器
    /// </summary>
    public class FywCredHandler : CredHandler
    {
        /// <summary>
        /// 资质弹窗
        /// </summary>
        private GameObject cred_form;

        /// <summary>
        /// 资质数据
        /// </summary>
        protected List<CredData> cred_datas;

        /// <summary>
        /// 唤醒处理
        /// </summary>
        private void Awake()
        {
            this.cred_datas = new List<CredData>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="args"></param>
        protected override void Init(Hashtable args)
        {
            this.cred_datas.Clear();
            this.url = args["url"].ToString();
            this.StartCoroutine(this.Get(this.url + $"?game_id={this.chnl_ctrl.Game}&cnty_id={this.chnl_ctrl.Cnty}"));
        }

        /// <summary>
        /// 调用处理
        /// </summary>
        protected override void Call()
        {
            GameObject cred_obj = Resources.Load("CredForm") as GameObject;
            this.cred_form = GameObject.Instantiate(cred_obj);
            this.cred_form.transform.SetParent(GameObject.Find("Canvas").transform);
            this.cred_form.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            this.cred_form.GetComponent<RectTransform>().sizeDelta = new Vector3(0, 0, 0);
            this.cred_form.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            CredForm form = this.cred_form.GetComponent<CredForm>();
            form.SetClose(this.CloseHandle);
            form.ShowData(this.cred_datas);
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="data"></param>
        protected override void Parse(string data)
        {
            try
            {
                ArrayList list = JsonTool.jsonDecode(data) as ArrayList;
                if (null == list)
                {
                    return;
                }
                CredData temp;
                foreach (var item in list)
                {
                    temp = CredData.Parse(item as Hashtable);
                    if (null != temp)
                    {
                        this.cred_datas.Add(temp);
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
            if (null != this.cred_form)
            {
                Object.Destroy(this.cred_form);
            }
            this.Back();
        }
    }
}