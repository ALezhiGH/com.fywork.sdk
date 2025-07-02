using System.Collections;
using UnityEngine;
using com.m3839.sdk;
using com.m3839.sdk.single;

namespace SDK
{
    /// <summary>
    /// 好游快爆防沉迷处理器
    /// </summary>
    public class HykbFcmHandler : FcmHandler
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="args"></param>
        protected override void Init(Hashtable args)
        {
            this.key = args["key"].ToString();
            this.Back();
        }

        /// <summary>
        /// 唤起防沉迷
        /// </summary>
        protected override void Call()
        {
            if (Application.isEditor)
            {
                this.Back();
            }
            else
            {
                //游戏屏幕方向 （Game Screen Orientation）
                int screen = HykbContext.SCREEN_PORTRAIT;
                //初始化回调监听（Init callback）
                //HykbInitListenerProxy proxy = new HykbInitListenerProxy(this);
                //HykbLogin.Init(this.game_id, screen, proxy);
                HykbFcmListenerProxy proxy = new HykbFcmListenerProxy(this);
                UnionFcmSDK.Init(this.key, screen, proxy);
            }
        }

        /// <summary>
        /// 防沉迷监听
        /// </summary>
        private class HykbFcmListenerProxy : UnionV2FcmListener
        {
            /// <summary>
            /// 处理器
            /// </summary>
            private HykbFcmHandler handler;

            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="handler"></param>
            public HykbFcmListenerProxy(HykbFcmHandler handler)
            {
                this.handler = handler;
            }

            /// <summary>
            /// 认证失败
            /// </summary>
            /// <param name="code"></param>
            /// <param name="message"></param>
            public override void OnFailed(int code, string message)
            {
                switch (code)
                {
                    case 2001:
                    case 2002:
                    case 2003:
                    case 2004:
                    case 2005:
                        Application.Quit();
                        break;

                }
            }

            /// <summary>
            /// 认证成功
            /// </summary>
            /// <param name="user"></param>
            public override void OnSucceed(UnionFcmUser user)
            {
                this.handler.Back();
            }
        }
    }
}