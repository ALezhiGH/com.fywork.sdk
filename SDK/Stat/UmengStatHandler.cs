using System;
using System.Collections;
using Umeng;
using UnityEngine;

namespace SDK
{
        /// <summary>
        /// 友盟统计处理器
        /// </summary>
        public class UmengStatHandler : StatHandler
        {
                /// <summary>
                /// 初始化
                /// </summary>
                /// <param name="args"></param>
                protected override void Init(Hashtable args)
                {
                        if (!Application.isEditor)
                        {
                                try
                                {
                                        this.key = args["key"].ToString();
                                        GA.StartWithAppKeyAndChannelId(this.key, this.chnl_ctrl.Chnl.ToString());
                                        this.Back();
                                }
                                catch (Exception e)
                                {
                                        Debug.LogError(e.StackTrace);
                                        Debug.LogError(e.Message);
                                }
                        }
                        else
                        {
                                this.Back();
                        }
                }

                /// <summary>
                /// 增加统计
                /// </summary>
                /// <param name="key"></param>
                public override void AppendStat(string key)
                {
                        if (!Application.isEditor)
                        {
                                GA.Event(key);
                        }
                }

                /// <summary>
                /// 增加统计
                /// </summary>
                /// <param name="key"></param>
                /// <param name="value"></param>
                public override void AppendStat(string key, int value)
                {
                        if (!Application.isEditor)
                        {
                                GA.Event(key, value.ToString());
                        }
                }

                /// <summary>
                /// 增加统计
                /// </summary>
                /// <param name="key"></param>
                /// <param name="value"></param>
                public override void AppendStat(string key, string value)
                {
                        if (!Application.isEditor)
                        {
                                GA.Event(key, value);
                        }
                }

                /// <summary>
                /// 打开面板
                /// </summary>
                /// <param name="panel_name"></param>
                public override void OpenPanel(string panel_name)
                {
                        if (!Application.isEditor)
                        {
                                GA.PageBegin(panel_name);
                        }
                }

                /// <summary>
                /// 关闭面板
                /// </summary>
                /// <param name="panel_name"></param>
                public override void ClosePanel(string panel_name)
                {
                        if (!Application.isEditor)
                        {
                                GA.PageEnd(panel_name);
                        }
                }
        }
}
