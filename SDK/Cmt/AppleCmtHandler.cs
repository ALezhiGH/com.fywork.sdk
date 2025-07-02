using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SDK
{
    /// <summary>
    /// 苹果评论处理器
    /// </summary>
    public class AppleCmtHandler : CmtHandler
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="args"></param>
        protected override void Init(Hashtable args)
        {
            this.key = (string)args["key"];
            this.Back();
        }

        /// <summary>
        /// 唤起
        /// </summary>
        protected override void Call()
        {
#if UNITY_IOS
            DialogController.ShowPingLun(this.key, null);
#endif
            this.Back();
        }
    }
}