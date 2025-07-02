using System.Collections;
using UnityEngine;

namespace SDK
{
    /// <summary>
    /// 泛鱼论坛处理器
    /// </summary>
    public class FywFrmHandler : FrmHandler
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="args"></param>
        protected override void Init(Hashtable args)
        {
            this.url = args["url"].ToString();
            this.Back();
        }

        /// <summary>
        /// 调用处理
        /// </summary>
        protected override void Call()
        {
            Application.OpenURL(this.url);
            this.Back();
        }
    }
}