using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace SDK
{
    /// <summary>
    /// 服务处理器
    /// </summary>
    public abstract class ServHandler : MonoBehaviour
    {
        /// <summary>
        /// 渠道控制器
        /// </summary>
        protected ChnlControl chnl_ctrl;

        /// <summary>
        /// 应用key
        /// </summary>
        protected string key;

        /// <summary>
        /// 回调行为
        /// </summary>
        protected UnityAction back;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="chnl_ctrl"></param>
        /// <param name="args"></param>
        /// <param name="back"></param>
        public void Init(ChnlControl chnl_ctrl, Hashtable args, UnityAction back)
        {
            this.chnl_ctrl = chnl_ctrl;
            this.back = back;
            this.Init(args);
        }

        /// <summary>
        /// 唤起
        /// </summary>
        /// <param name="back"></param>
        public void Call(UnityAction back)
        {
            this.back = back;
            this.Call();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="args"></param>
        protected abstract void Init(Hashtable args);

        /// <summary>
        /// 唤起
        /// </summary>
        /// <param name="call"></param>
        protected abstract void Call();

        /// <summary>
        /// 执行回调
        /// </summary>
        protected virtual void Back()
        {
            UnityAction back = this.back;
            this.back = null;
            if (null != back)
            {
                back();
            }
        }
    }
}
