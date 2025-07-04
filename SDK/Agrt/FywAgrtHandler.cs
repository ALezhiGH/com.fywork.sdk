using Fyw;
using System.Collections;
using UnityEngine;

namespace SDK
{
    /// <summary>
    /// 泛鱼协议控制器
    /// </summary>
    public class FywAgrtHandler : AgrtHandler
    {
        /// <summary>
        /// 保存key
        /// </summary>
        private string save_key = "";

        /// <summary>
        /// 协议弹窗
        /// </summary>
        private GameObject agrt_form;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="args"></param>
        protected override void Init(Hashtable args)
        {
            this.save_key = args["save"].ToString();
            this.PrivUrl = args["priv"].ToString();
            this.ServUrl = args["serv"].ToString();
            this.Back();
        }

        /// <summary>
        /// 调用处理
        /// </summary>
        protected override void Call()
        {
            if (PlayerPrefs.GetInt(this.save_key) != 1)//没有同意过协议就必须要取点击同意协议
            {
                GameObject canvas = GameObject.Find("Canvas");
                if (null == canvas)
                {
                    LogTool.Error("没有找到Canvas");
                    this.Back();
                    return;
                }
                GameObject agrt_obj = Resources.Load("AgrtForm") as GameObject;
                this.agrt_form = GameObject.Instantiate(agrt_obj);
                this.agrt_form.transform.SetParent(canvas.transform);
                this.agrt_form.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                this.agrt_form.GetComponent<RectTransform>().sizeDelta = new Vector3(0, 0, 0);
                this.agrt_form.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                AgrtForm form = this.agrt_form.GetComponent<AgrtForm>();
                form.AppendUrl("privacy", this.PrivUrl);
                form.AppendUrl("service", this.ServUrl);
                form.SetAgree(this.CallHandle);
            }
            else
            {
                this.Back();
            }
        }

        /// <summary>
        /// 同意处理
        /// </summary>
        private void CallHandle()
        {
            if (null != this.agrt_form)
            {
                agrt_form.SetActive(false);
                Object.Destroy(this.agrt_form);
            }
            PlayerPrefs.SetInt(this.save_key, 1);
            PlayerPrefs.Save();
            this.Back();
        }
    }
}