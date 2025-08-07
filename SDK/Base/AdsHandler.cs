using System.Collections;
using UnityEngine;

namespace SDK
{
    /// <summary>
    /// 广告码
    /// </summary>
    public enum AdvertCode
    {
        None = 1,//没有广告
        Error,//播放错误
        Close,//中途关闭
        Banner,//横幅广告
        Insert,//插页广告
        Reward,//激励广告
        Native,//原生广告
    }

    /// <summary>
    /// 广告显示代理
    /// </summary>
    /// <param name="code"></param>
    public delegate void AdCallback(AdvertCode code);

    /// <summary>
    /// 广告处理器
    /// </summary>
    public abstract class AdsHandler : ServHandler
    {
        /// <summary>
        /// 广告回调事件
        /// </summary>
        protected AdCallback ad_call;

        /// <summary>
        /// 插页广告ID
        /// </summary>
        protected string insert_id = "";

        /// <summary>
        /// 激励广告ID
        /// </summary>
        protected string reward_id = "";

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void Call()
        {
            this.Back();
        }

        /// <summary>
        /// 能否显示插屏广告 用于外部状态显示
        /// </summary>
        /// <returns></returns>
        public abstract bool CanInsert();

        /// <summary>
        /// 能否显示激励广告 用于外部状态显示
        /// </summary>
        /// <returns></returns>
        public abstract bool CanReward();

        /// <summary>
        /// 显示插屏广告 用于外部直接调用
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        public abstract bool ShowInsert(AdCallback call);

        /// <summary>
        /// 显示激励广告 用于外部直接调用
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        public abstract bool ShowReward(AdCallback call);

        /// <summary>
        /// 执行回调
        /// </summary>
        /// <param name="code"></param>
        protected virtual void DoShowback(AdvertCode code)
        {
            this.ad_call?.Invoke(code);
            this.ad_call = null;
        }
    }
}