﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ByteDance.Union;
using ByteDance.Union.Mediation;
using System.Threading;

namespace SDK
{
    /// <summary>
    /// 穿山甲聚合处理器
    /// </summary>
    public class CsjUnAdsHandler : AdsHandler
    {
        /// <summary>
        /// 插屏视频加载标识
        /// </summary>
        private int insert_load = -1;

        /// <summary>
        /// 激励视频加载标识
        /// </summary>
        private int reward_load = -1;

        /// <summary>
        /// 插全屏和新插屏，支持csj和融合
        /// </summary>
        public FullScreenVideoAd insert_ad;

        /// <summary>
        /// 激励视频，支持csj和融合
        /// </summary>
        public RewardVideoAd reward_ad;

        /// <summary>
        /// 主线程
        /// </summary>
        public int MainThread { get; protected set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="args"></param>
        protected override void Init(Hashtable args)
        {
            this.insert_load = -1;
            this.reward_load = -1;
            this.MainThread = Thread.CurrentThread.ManagedThreadId;
            this.key = args["key"].ToString();
            this.insert_id = args["insert"].ToString();
            this.reward_id = args["reward"].ToString();
            this.InitSDK();
        }

        /// <summary>
        /// 能否显示插屏广告 用于外部状态显示
        /// </summary>
        /// <returns></returns>
        public override bool CanInsert()
        {
            if (this.insert_load > 0)
            {
                return true;
            }
            else
            {
                if (this.insert_load < 0)
                {
                    this.loadInsertAd();
                }
                return false;
            }
        }

        /// <summary>
        /// 能否显示激励广告 用于外部状态显示
        /// </summary>
        /// <returns></returns>
        public override bool CanReward()
        {
            if (this.reward_load > 0)
            {
                return true;
            }
            else
            {
                if (this.reward_load < 0)
                {
                    this.loadRewardAd();
                }
                return false;
            }
        }

        /// <summary>
        /// 显示插屏广告 用于外部直接调用
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        public override bool ShowInsert(AdCallback call)
        {
            if (this.insert_load > 0)
            {
                this.ad_call = call;
                this.showInsertAd();
                return true;
            }
            else
            {
                if (this.insert_load < 0)
                {
                    this.loadInsertAd();
                }
                this.DoShowback(AdvertCode.None);//触发无广告回调
                return false;
            }
        }

        /// <summary>
        /// 显示激励广告 用于外部直接调用
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        public override bool ShowReward(AdCallback call)
        {
            this.ad_call = call;
            if (this.reward_load > 0)
            {
                this.showRewardAd();
                return true;
            }
            else
            {
                if (this.reward_load < 0)
                {
                    this.loadRewardAd();
                }
                this.DoShowback(AdvertCode.None);//触发无广告回调
                return false;
            }
        }

        /// <summary>
        /// 加载插屏广告
        /// </summary>
        private void loadInsertAd()
        {
            // 释放上一次广告
            if (this.insert_ad != null)
            {
                this.insert_ad.Dispose();
                this.insert_ad = null;
            }
            this.insert_load = 0;

            // 创造广告参数对象
            var adSlot = new AdSlot.Builder()
                .SetCodeId(this.insert_id) // 必传
                .SetOrientation(AdOrientation.Vertical)
                .SetMediationAdSlot(new MediationAdSlot.Builder()
                    .SetScenarioId("ScenarioId") // 可选
                    .SetUseSurfaceView(false) // 可选
                    .SetBidNotify(true) // 可选
                    .Build())
                .Build();

            // 加载广告
            ByteDance.Union.SDK.CreateAdNative().LoadFullScreenVideoAd(adSlot, new InsertAdLoadListener(this));
        }

        /// <summary>
        /// 加载激励广告
        /// </summary>
        private void loadRewardAd()
        {
            // 释放上一次广告
            if (this.reward_ad != null)
            {
                this.reward_ad.Dispose();
                this.reward_ad = null;
            }
            this.reward_load = 0;

            // 创造广告参数对象
            var adSlot = new AdSlot.Builder()
                .SetCodeId(this.reward_id) // 必传
                .SetUserID(SystemInfo.deviceUniqueIdentifier) // 用户id,必传参数
                .SetOrientation(AdOrientation.Vertical) // 必填参数，期望视频的播放方向
                .SetRewardName("金币") // 可选
                .SetRewardAmount(1) // 可选
                .SetMediaExtra("media_extra") //⚠️设置透传信息(穿山甲广告 或 聚合维度iOS广告时)，需可序列化
                .SetMediationAdSlot(
                    new MediationAdSlot.Builder()
#if UNITY_ANDROID  //⚠️设置透传信息(当加载聚合维度Android广告时)
                    .SetExtraObject(AdConst.KEY_GROMORE_EXTRA, "gromore-server-reward-extra-unity") // 可选，设置gromore服务端验证的透传参数
                        .SetExtraObject("pangle", "pangleCustomData") // 可选，不是gromore服务端验证时，用于各个adn的参数透传
#endif
                    .SetScenarioId("reward-m-scenarioId") // 可选
                        .SetBidNotify(true) // 可选
                        .SetUseSurfaceView(false) // 可选
                        .Build()
                        )
                .Build();

            // 加载广告
            ByteDance.Union.SDK.CreateAdNative().LoadRewardVideoAd(adSlot, new RewardAdLoadListener(this));
        }

        /// <summary>
        /// 显示插屏广告
        /// </summary>
        private void showInsertAd()
        {
            // 设置展示阶段的监听器
            this.insert_ad.SetFullScreenVideoAdInteractionListener(new InsertAdPlayListener(this));
            this.insert_ad.SetDownloadListener(new AppDownloadListener(this));
            this.insert_ad.SetAdInteractionListener(new AdInteractListener());
            this.insert_ad.ShowFullScreenVideoAd();
        }

        /// <summary>
        /// 显示激励广告
        /// </summary>
        private void showRewardAd()
        {
            // 设置展示阶段的监听器
            this.reward_ad.SetRewardAdInteractionListener(new RewardAdPlayListener(this));
            this.reward_ad.SetAgainRewardAdInteractionListener(null);//屏蔽再一次观看
            this.reward_ad.SetDownloadListener(new AppDownloadListener(this));
            this.reward_ad.SetAdInteractionListener(new AdInteractListener());
#if UNITY_ANDROID
            this.reward_ad.SetRewardPlayAgainController(null);//屏蔽再一次观看
#endif
            this.reward_ad.ShowRewardVideoAd();
        }

        /// <summary>
        /// 初始化SDK
        /// </summary>
        protected virtual void InitSDK()
        {
            if (Application.isEditor)
            {
                SdkInitCallback(false, "编辑器下");
            }
            else
            {
                // sdk初始化
                SDKConfiguration sdkConfiguration = new SDKConfiguration.Builder()
                    .SetAppId(this.key)
                    .SetAppName(Application.productName)
                    .SetUseMediation(true) // 是否使用融合功能，置为false，可不初始化聚合广告相关模块
                    .SetDebug(true) // debug日志开关，app发版时记得关闭
                    .SetMediationConfig(GetMediationConfig())
                    .SetPrivacyConfigurationn(GetPrivacyConfiguration())
                    .SetAgeGroup(0)
                    .SetPaid(false) // 是否是付费用户
                    .SetTitleBarTheme(AdConst.TITLE_BAR_THEME_LIGHT) // 设置落地页主题
                    .SetKeyWords("") // 设置用户画像关键词列表
                    .Build();

                Pangle.Init(sdkConfiguration); // 合规要求，初始化分为2步，第一步先调用init
                Pangle.Start(this.SdkInitCallback); // 第二步再调用start。注意在初始化回调成功后再请求广告
            }
        }

        /// <summary>
        /// SDK初始化回调
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        private void SdkInitCallback(bool success, string message)
        {
            // 注意：在初始化回调成功后再请求广告
            LogTool.Norm("[CsjUn SdkInitCallback] success: " + success + ", message: " + message);
            if (this.insert_id != string.Empty && !Application.isEditor)
            {
                this.loadInsertAd();//加载插屏缓存
            }
            if (this.reward_id != string.Empty && !Application.isEditor)
            {
                this.loadRewardAd();//加载视频缓存
            }
            this.Back();
        }

        /// <summary>
        /// 初始化时进行隐私合规相关配置。不设置的将使用默认值
        /// </summary>
        /// <returns></returns>
        private PrivacyConfiguration GetPrivacyConfiguration()
        {
            // 这里仅展示了部分设置，开发者根据自己需要进行设置，不设置的将使用默认值，默认值可能不合规。
            PrivacyConfiguration privacyConfig = new PrivacyConfiguration();
            privacyConfig.CanUsePhoneState = false;
            privacyConfig.CanUseLocation = false;
            privacyConfig.Longitude = 115.7;
            privacyConfig.Latitude = 39.4;
            //privacyConfig.CustomIdfa = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";

            // 融合相关配置示例
            privacyConfig.MediationPrivacyConfig = new MediationPrivacyConfig();
            privacyConfig.MediationPrivacyConfig.LimitPersonalAds = false;
            privacyConfig.MediationPrivacyConfig.ProgrammaticRecommend = false;
            privacyConfig.MediationPrivacyConfig.forbiddenCAID = false;//20250723@Lezhi CSJ_M_SDK_6800 已移除本字段
            privacyConfig.MediationPrivacyConfig.CanUseOaid = false;

            return privacyConfig;
        }

        /// <summary>
        /// 使用融合功能时，初始化时进行相关配置
        /// </summary>
        /// <returns></returns>
        private MediationConfig GetMediationConfig()
        {
            MediationConfig mediationConfig = new MediationConfig();

            // 聚合配置json字符串（从gromore平台下载），用于首次安装时作为兜底配置使用。可选
            mediationConfig.CustomLocalConfig = "{\"cypher\":2,\"message\":\"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\"}";

            // 流量分组功能，可选
            MediationConfigUserInfoForSegment segment = new MediationConfigUserInfoForSegment();
            segment.Age = 18;
            segment.Gender = AdConst.GENDER_MALE;
            segment.Channel = "mediation-unity";
            segment.SubChannel = "mediation-sub-unity";
            segment.UserId = "mediation-userId-unity";
            segment.UserValueGroup = "mediation-user-value-unity";
            segment.CustomInfos = new Dictionary<string, string>
        {
            { "customKey", "customValue" }
        };
            mediationConfig.MediationConfigUserInfoForSegment = segment;

            return mediationConfig;
        }

        /// <summary>
        /// 插屏广告加载监听器
        /// </summary>
        private class InsertAdLoadListener : IFullScreenVideoAdListener
        {
            /// <summary>
            /// 处理器
            /// </summary>
            private CsjUnAdsHandler handler;

            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="handler"></param>
            public InsertAdLoadListener(CsjUnAdsHandler handler)
            {
                this.handler = handler;
            }

            /// <summary>
            /// 加载错误
            /// </summary>
            /// <param name="code"></param>
            /// <param name="message"></param>
            public void OnError(int code, string message)
            {
                LogTool.Error($"CsjUn OnFullScreenError: {message}  on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
                this.handler.insert_load = -1;
            }

            /// <summary>
            /// 加载成功
            /// </summary>
            /// <param name="ad"></param>
            public void OnFullScreenVideoAdLoad(FullScreenVideoAd ad)
            {
                LogTool.Norm($"CsjUn OnFullScreenAdLoad  on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
                this.handler.insert_load = 1;
                this.handler.insert_ad = ad;
            }

            /// <summary>
            /// 即将废弃
            /// </summary>
            public void OnFullScreenVideoCached()
            {
                LogTool.Norm($"CsjUn OnFullScreenVideoCached  on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
            }

            /// <summary>
            /// 加载缓存
            /// </summary>
            /// <param name="ad"></param>
            public void OnFullScreenVideoCached(FullScreenVideoAd ad)
            {
                LogTool.Norm($"CsjUn OnFullScreenVideoCached  on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
            }
        }

        /// <summary>
        /// 插屏广告播放监听器
        /// </summary>
        private class InsertAdPlayListener : IFullScreenVideoAdInteractionListener
        {
            /// <summary>
            /// 处理器
            /// </summary>
            private CsjUnAdsHandler handler;

            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="handler"></param>
            public InsertAdPlayListener(CsjUnAdsHandler handler)
            {
                this.handler = handler;
            }

            /// <summary>
            /// 获得Ecpm信息
            /// </summary>
            /// <returns></returns>
            private string GetEcpmInfo()
            {
                MediationAdEcpmInfo showEcpm = this.handler.insert_ad.GetMediationManager().GetShowEcpm();
                if (showEcpm != null)
                {
                    return showEcpm.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            /// <summary>
            /// 广告展示
            /// </summary>
            public void OnAdShow()
            {
                LogTool.Norm($"CsjUn OnFullScreenShow ecpm:{this.GetEcpmInfo()} on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
            }

            /// <summary>
            /// 广告点击
            /// </summary>
            public void OnAdVideoBarClick()
            {
                LogTool.Norm($"CsjUn OnFullScreenBarClick on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
            }


            /// <summary>
            /// 广告关闭
            /// </summary>
            public void OnAdClose()
            {
                LogTool.Norm($"CsjUn OnFullScreenClose on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
                this.handler.insert_load = -1;
                this.handler.DoShowback(AdvertCode.Insert);
                this.handler.loadInsertAd();
            }

            /// <summary>
            /// 播放完成
            /// </summary>
            public void OnVideoComplete()
            {
                LogTool.Norm($"CsjUn OnFullScreenComplete on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
            }

            /// <summary>
            /// 播放错误
            /// </summary>
            public void OnVideoError()
            {
                LogTool.Error($"CsjUn OnFullScreenError on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
            }

            /// <summary>
            /// 跳过视频
            /// </summary>
            public void OnSkippedVideo()
            {
                LogTool.Norm($"CsjUn OnFullScreenSkip on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
            }
        }

        /// <summary>
        /// 激励广告监听器
        /// </summary>
        private class RewardAdLoadListener : IRewardVideoAdListener
        {
            /// <summary>
            /// 处理器
            /// </summary>
            private CsjUnAdsHandler handler;

            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="handler"></param>
            public RewardAdLoadListener(CsjUnAdsHandler handler)
            {
                this.handler = handler;
            }

            /// <summary>
            /// 加载错误
            /// </summary>
            /// <param name="code"></param>
            /// <param name="message"></param>
            public void OnError(int code, string message)
            {
                LogTool.Error($"CsjUn OnRewardError:{message} on main thread:{Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
                this.handler.reward_load = -1;
            }

            /// <summary>
            /// 加载成功
            /// </summary>
            /// <param name="ad"></param>
            public void OnRewardVideoAdLoad(RewardVideoAd ad)
            {
                LogTool.Norm($"CsjUn OnRewardVideoAdLoad on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
                this.handler.reward_ad = ad;
                this.handler.reward_load = 1;
            }

            /// <summary>
            /// 即将废弃
            /// </summary>
            public void OnRewardVideoCached()
            {
                LogTool.Norm($"CsjUn OnRewardVideoCached on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
            }

            /// <summary>
            /// 加载缓存
            /// </summary>
            /// <param name="ad"></param>
            public void OnRewardVideoCached(RewardVideoAd ad)
            {
                LogTool.Norm($"CsjUn OnRewardVideoCached RewardVideoAd ad on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
            }
        }

        /// <summary>
        /// 激励广告监听器
        /// </summary>
        private class RewardAdPlayListener : IRewardAdInteractionListener
        {
            /// <summary>
            /// 处理器
            /// </summary>
            private CsjUnAdsHandler handler;

            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="handler"></param>
            public RewardAdPlayListener(CsjUnAdsHandler handler)
            {
                this.handler = handler;
            }

            /// <summary>
            /// 获得Ecpm信息
            /// </summary>
            /// <returns></returns>
            private string GetEcpmInfo()
            {
                MediationAdEcpmInfo showEcpm = this.handler.reward_ad.GetMediationManager().GetShowEcpm();
                if (showEcpm != null)
                {
                    return showEcpm.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            /// <summary>
            /// 广告展示
            /// </summary>
            public void OnAdShow()
            {
                LogTool.Norm($"CsjUn OnRewardShow ecpm:{this.GetEcpmInfo()} on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
            }

            /// <summary>
            /// 广告点击
            /// </summary>
            public void OnAdVideoBarClick()
            {
                LogTool.Norm($"CsjUn OnRewardBarClick on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
            }

            /// <summary>
            /// 广告关闭
            /// </summary>
            public void OnAdClose()
            {
                LogTool.Norm($"CsjUn OnRewardClose on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
                this.handler.reward_load = -1;
                this.handler.DoShowback(AdvertCode.None);
                this.handler.loadRewardAd();
            }

            /// <summary>
            /// 跳过视频
            /// </summary>
            public void OnVideoSkip()
            {
                LogTool.Norm($"CsjUn OnRewardSkip on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
            }

            /// <summary>
            /// 视频播放完成
            /// </summary>
            public void OnVideoComplete()
            {
                LogTool.Norm($"CsjUn OnRewardComplete on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
            }

            /// <summary>
            /// 视频播放错误
            /// </summary>
            public void OnVideoError()
            {
                LogTool.Error($"CsjUn OnRewardError on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
            }

            /// <summary>
            /// 奖励下发
            /// </summary>
            /// <param name="isRewardValid"></param>
            /// <param name="rewardType"></param>
            /// <param name="extraInfo"></param>
            public void OnRewardArrived(bool isRewardValid, int rewardType, IRewardBundleModel extraInfo)
            {
                LogTool.Norm($"CsjUn OnRewardVerify:{isRewardValid} rewardType:{rewardType} extraInfo:{extraInfo.ToString()} on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
                this.handler.DoShowback(AdvertCode.Reward);
            }
        }

        /// <summary>
        /// 广告交互监听
        /// </summary>
        public class AdInteractListener : ITTAdInteractionListener
        {
            /// <summary>
            /// 广告事件
            /// </summary>
            /// <param name="code"></param>
            /// <param name="map"></param>
            public void OnAdEvent(int code, Dictionary<string, object> map)
            {
                LogTool.Norm($"CsjUn OnAdEvent code:{code}, map:{map.ToString()}");
            }
        }

        /// <summary>
        /// 应用下载监听
        /// </summary>
        public sealed class AppDownloadListener : IAppDownloadListener
        {
            /// <summary>
            /// 处理器
            /// </summary>
            private CsjUnAdsHandler handler;

            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="handler"></param>
            public AppDownloadListener(CsjUnAdsHandler handler)
            {
                this.handler = handler;
            }

            /// <summary>
            /// 未开始下载
            /// </summary>
            public void OnIdle()
            {
                LogTool.Norm($"CsjUn OnIdle 下载未开始 on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
            }

            /// <summary>
            /// 下载中
            /// </summary>
            /// <param name="totalBytes"></param>
            /// <param name="currBytes"></param>
            /// <param name="fileName"></param>
            /// <param name="appName"></param>
            public void OnDownloadActive(long totalBytes, long currBytes, string fileName, string appName)
            {
                LogTool.Norm($"CsjUn OnDownloadActive 下载中 on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
            }

            /// <summary>
            /// 下载暂停
            /// </summary>
            /// <param name="totalBytes"></param>
            /// <param name="currBytes"></param>
            /// <param name="fileName"></param>
            /// <param name="appName"></param>
            public void OnDownloadPaused(long totalBytes, long currBytes, string fileName, string appName)
            {
                LogTool.Norm($"CsjUn OnDownloadPaused 下载暂停 on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
            }

            /// <summary>
            /// 下载失败
            /// </summary>
            /// <param name="totalBytes"></param>
            /// <param name="currBytes"></param>
            /// <param name="fileName"></param>
            /// <param name="appName"></param>
            public void OnDownloadFailed(long totalBytes, long currBytes, string fileName, string appName)
            {
                LogTool.Norm($"CsjUn OnDownloadFailed 下载失败 on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
            }

            /// <summary>
            /// 下载完成
            /// </summary>
            /// <param name="totalBytes"></param>
            /// <param name="fileName"></param>
            /// <param name="appName"></param>
            public void OnDownloadFinished(long totalBytes, string fileName, string appName)
            {
                LogTool.Norm($"CsjUn OnDownloadFinished 下载完成 on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
            }

            /// <summary>
            /// 安装完成
            /// </summary>
            /// <param name="fileName"></param>
            /// <param name="appName"></param>
            public void OnInstalled(string fileName, string appName)
            {
                LogTool.Norm($"CsjUn OnInstalled 安装完成 on main thread: {Thread.CurrentThread.ManagedThreadId == this.handler.MainThread}");
            }
        }
    }
}