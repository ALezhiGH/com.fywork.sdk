using Fyw;
using System.Collections;
using UnityEngine;

namespace SDK
{
    /// <summary>
    /// 泛鱼更新处理器
    /// </summary>
    public class FywUpdHandler : UpdHandler
    {
        /// <summary>
        /// 保存key
        /// </summary>
        protected string save;

        /// <summary>
        /// 更新数据
        /// </summary>
        protected UpdData upd_data;

        /// <summary>
        /// 使用弹窗
        /// </summary>
        private GameObject use_form;

        /// <summary>
        /// 唤醒处理
        /// </summary>
        private void Awake()
        {
            this.save = string.Empty;
            this.upd_data = null;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="args"></param>
        protected override void Init(Hashtable args)
        {
            this.url = args["url"].ToString();
            this.save = args["save"].ToString();
            this.StartCoroutine(this.Get(this.url + $"?game_id={this.chnl_ctrl.Game}&chnl_id={this.chnl_ctrl.Chnl}"));
        }

        /// <summary>
        /// 调用处理
        /// </summary>
        protected override void Call()
        {
            if (null == this.upd_data)
            {
                this.Back();
                return;
            }
            string ver_old = PlayerPrefs.GetString(this.save, "");
            if (ver_old != this.upd_data.VerNew)
            {
                PlayerPrefs.SetString(this.save, this.upd_data.VerNew);
                PlayerPrefs.Save();
            }
            if (this.upd_data.ForceUpd)//强制更新次次提示
            {
                GameObject obj = Resources.Load("FupdForm") as GameObject;
                this.use_form = GameObject.Instantiate(obj);
                this.use_form.transform.SetParent(GameObject.Find("Canvas").transform);
                this.use_form.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                this.use_form.GetComponent<RectTransform>().sizeDelta = new Vector3(0, 0, 0);
                this.use_form.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                FupdForm form = this.use_form.GetComponent<FupdForm>();
                form.ShowData(this.upd_data);
                return;
            }
            if (ver_old != this.upd_data.VerNew && this.upd_data.NeedUpd)//普通更新只提示一次
            {
                GameObject obj = Resources.Load("NupdForm") as GameObject;
                this.use_form = GameObject.Instantiate(obj);
                this.use_form.transform.SetParent(GameObject.Find("Canvas").transform);
                this.use_form.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                this.use_form.GetComponent<RectTransform>().sizeDelta = new Vector3(0, 0, 0);
                this.use_form.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                NupdForm form = this.use_form.GetComponent<NupdForm>();
                form.SetClose(this.CloseHandle);
                form.ShowData(this.upd_data);
                return;
            }
            this.Back();
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
                this.upd_data = UpdData.Parse(table);
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
            if (null != this.use_form)
            {
                Object.Destroy(this.use_form);
            }
            this.Back();
        }
    }
}