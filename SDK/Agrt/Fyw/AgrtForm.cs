using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Fyw
{
    /// <summary>
    /// 协议弹窗
    /// </summary>
    public class AgrtForm : MonoBehaviour
    {
        /// <summary>
        /// 同意按钮
        /// </summary>
        public Button AgreeBtn;

        /// <summary>
        /// 超链接文本
        /// </summary>
        public HyperLink HyperLink;

        /// <summary>
        /// 网址信息
        /// </summary>
        private Dictionary<string, string> urls;

        /// <summary>
        /// 唤醒处理
        /// </summary>
        private void Awake()
        {
            this.urls = new Dictionary<string, string>();
            this.HyperLink.OnLinkClick.AddListener(this.OnLinkClick);
        }

        /// <summary>
        /// 追加地址
        /// </summary>
        /// <param name="name"></param>
        /// <param name="url"></param>
        public void AppendUrl(string name, string url)
        {
            this.urls[name] = url;
        }

        /// <summary>
        /// 设置同意回调
        /// </summary>
        /// <param name="call"></param>
        public void SetAgree(UnityAction call)
        {
            this.AgreeBtn.onClick.RemoveAllListeners();
            this.AgreeBtn.onClick.AddListener(call);
        }

        /// <summary>
        /// 点击拒绝
        /// </summary>
        public void RefuseClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// 链接点击处理
        /// </summary>
        /// <param name="name"></param>
        private void OnLinkClick(string name)
        {
            if (this.urls.ContainsKey(name))
            {
                Application.OpenURL(this.urls[name]);
            }
        }
    }
}