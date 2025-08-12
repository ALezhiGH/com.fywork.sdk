using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if FY_TAKU
using AnyThinkAds.Api;
using AnyThinkAds.ThirdParty.LitJson;
#endif

namespace SDK
{
    /// <summary>
    /// Taku聚合广告处理器
    /// </summary>
    public class TakuUnAdsHandler : AdsHandler
    {
        /// <summary>
        /// 插屏视频加载标识
        /// </summary>
        private int insert_load = -1;

        /// <summary>
        /// 激励视频加载标识
        /// </summary>
        private int reward_load = -1;

        private string id;
        private Coroutine loadInterstitialAdCoroutine;
        private Coroutine loadRewardAdCoroutine;

        private void OnDestroy()
        {
            StopAllCoroutines();
            loadInterstitialAdCoroutine = null;
            loadRewardAdCoroutine = null;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="args"></param>
        protected override void Init(Hashtable args)
        {
            this.insert_load = -1;
            this.reward_load = -1;
            this.id = args["id"].ToString();
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
        public override bool ShowInsert(AdCallback call)
        {
            this.ad_call = call;
            if (this.insert_load > 0)
            {
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
            //重试中
            if (loadInterstitialAdCoroutine != null)
            {
                return;
            }
            
            //标记已调用加载
            this.insert_load = 0;
            //调用加载 
            LoadInterstitialAd();
        }

        /// <summary>
        /// 加载激励广告
        /// </summary>
        private void loadRewardAd()
        {
            //重试中
            if (loadRewardAdCoroutine != null)
            {
                return;
            }

            //标记已调用加载
            this.reward_load = 0;
            //调用加载 
            LoadRewardAd();
        }

        /// <summary>
        /// 显示插屏广告
        /// </summary>
        private void showInsertAd()
        {
            // 设置展示阶段的监听器
            
            showInterstitialAd();
        }

        /// <summary>
        /// 显示激励广告
        /// </summary>
        private void showRewardAd()
        {
            // 设置展示阶段的监听器
            
            ShowRewardAd();
        }

        /// <summary>
        /// SDK初始化回调
        /// </summary>
        private void SdkInitCallback(bool success, string message)
        {
            // 注意：在初始化回调成功后再请求广告
            LogTool.Norm("[TakuUn SdkInitCallback] success: " + success + ", message: " + message);
            if (this.insert_id != string.Empty && !Application.isEditor)
            {
                InitializeInterstitialAd();//this.loadInsertAd();//加载插屏缓存
            }
            if (this.reward_id != string.Empty && !Application.isEditor)
            {
                InitializeRewardAd();//this.loadRewardAd();//加载视频缓存
            }
            this.Back();
        }
        
        /// <summary>
        /// 初始化SDK.
        /// https://help.takuad.com/docs/NjQbyD
        /// </summary>
        protected virtual void InitSDK()
        {
            if (Application.isEditor)
            {
                SdkInitCallback(false, "编辑器下");
            }
            else
            {
#if FY_TAKU
                //（可选配置）设置自定义的Map信息，可匹配后台配置的广告商顺序的列表（App纬度）.
                //自定义key-value，可用于App维度匹配后端下发的广告列表信息App的自定义规则为全局设置，对全部Placement有效.
                //开发者可以在Taku后台的流量分组中设置相应的自定义规则.
                //注意：调用此方法会清除setChannel()、setSubChannel()方法设置的信息，如果有设置这些信息，请在调用此方法后重新设置
                ATSDKAPI.initCustomMap(new Dictionary<string, string> { { "unity3d_data", "test_data" } }); 
                
                //（可选配置）设置自定义的Map信息，可匹配后台配置的广告商顺序的列表（Placement纬度）
                //自定义key-value，可用于广告位维度匹配后端下发的广告列表信息.仅对当前Placement有效，您可以在App自定义规则基础上增加Placement特有的自定义规则.
                //开发者可以在Taku后台的流量分组中设置相应的自定义规则.
                //ATSDKAPI.setCustomDataForPlacementID(new Dictionary<string, string> { { "unity3d_data_pl", "test_data_pl" } } ,PlacementId);

                #if Fanyu
                var channel = "Fywork";
                #elif HYKB
                var channel = "HYKB";
                #elif TAPTAP
                var channel = "TAPTAP";
                #else
                var channel = "Fywork";
                #endif
                //（可选配置）设置渠道的信息，开发者可以通过该渠道信息在后台来区分看各个渠道的广告数据.
                //只允许设置字符的规则：[A-Za-z0-9_]
                //注意：如果有使用initCustomMap()方法，必须在initCustomMap()方法之后调用此方法
                ATSDKAPI.setChannel(channel);//("unity3d_test_channel"); 

                //（可选配置）设置子渠道的信息，开发者可以通过该渠道信息在后台来区分看各个渠道的子渠道广告数据.
                //只允许设置字符的规则：[A-Za-z0-9_]
                //注意：如果有使用initCustomMap()方法，必须在initCustomMap()方法之后调用此方法
                //ATSDKAPI.setSubChannel("unity3d_test_subchannel"); 

                //设置开启Debug日志（强烈建议测试阶段开启，方便排查问题）
                ATSDKAPI.setLogDebug(true);
                
                //获取当前隐私数据的上报等级
                //Debug.Log("Developer DataConsent: " + ATSDKAPI.getGDPRLevel());
                //Debug.Log("Developer isEUTrafic: " + ATSDKAPI.isEUTraffic());
                //判断当前网络是否在欧盟地区
                //ATSDKAPI.getUserLocation(new GetLocationListener());//ATGetUserLocationListener listener

                //Only for Android China SDK (CSJ)
                // #if UNITY_ANDROID
                // ATDownloadManager.Instance.setListener(new ATDownloadListener());
                // #endif
                
                //（必须配置）SDK的初始化 (其中appId和appKey需要在Taku后台创建应用之后获取。)
                ATSDKAPI.initSDK(this.id, this.key, new SDKInitListerner(SdkInitCallback));//Use your own app_id & app_key here
#else
                SdkInitCallback(false, "Cannot find FY_TAKU");
#endif
            }
        }

#if FY_TAKU
        public class SDKInitListerner : ATSDKInitListener
        {
            private Action<bool, string> callback;

            public SDKInitListerner(Action<bool, string> _callback)
            {
                callback = _callback;
            }
            
            /// <summary>
            /// This method will be called back after the SDK is initialized successfully
            /// </summary>
            public void initSuccess()
            {
                Debug.Log("[SDKInitListerner] initSuccess");
                callback?.Invoke(true, string.Empty);
            }
    
            /// <summary>
            /// This method will be called back after the SDK is initialized failed
            /// </summary>
            /// <param name="msg"></param>
            public void initFail(string msg)
            {
                Debug.Log("[SDKInitListerner] initFail:" + msg);
                callback?.Invoke(false, msg);
            }
        }

        /// <summary>
        /// 判断当前网络是否在欧盟地区 的回调
        /// </summary>
        private class GetLocationListener : ATGetUserLocationListener
        {
            public void didGetUserLocation(int location)
            {
                Debug.Log("[GetLocationListener] didGetUserLocation(): " + location);
                if (location == ATSDKAPI.kATUserLocationInEU && ATSDKAPI.getGDPRLevel() == ATSDKAPI.UNKNOWN)
                {
                    //展示GDPR授权页面. 
                    //ATSDKAPI.showGDPRAuth();//注意：v6.2.87开始废弃，请使用showGDPRConsentDialog代替
                    ATSDKAPI.showGDPRConsentDialog(new ATConsentListener());//（v6.2.87新增）展示Google UMP GDPR授权页面
                }
            }
        }

        /// <summary>
        /// 展示Google UMP GDPR授权页面 的回调
        /// </summary>
        private class ATConsentListener : ATConsentDismissListener
        {
            public void onConsentDismiss()
            {
                Debug.Log("[ATConsentListener] onConsentDismiss()");
            }
        }

        /// <summary>
        /// Only for Android China SDK (Pangle)
        /// </summary>
        private class ATDownloadListener : ATDownloadAdListener
        {
            public void onDownloadFail(string placementId, ATCallbackInfo callbackInfo, long totalBytes, long currBytes,
                string fileName, string appName)
            {
                Debug.Log("[ATDownloadListener] onDownloadFail------->" + JsonMapper.ToJson(callbackInfo.toDictionary())
                                                             + "\n, totalBytes: " + totalBytes + ", currBytes:" + currBytes
                                                             + "\n, fileName: " + fileName + ", appName: " + appName);
            }

            public void onDownloadFinish(string placementId, ATCallbackInfo callbackInfo, long totalBytes, string fileName,
                string appName)
            {
                Debug.Log("[ATDownloadListener] onDownloadFinish------->" + JsonMapper.ToJson(callbackInfo.toDictionary())
                                                               + "\n, totalBytes: " + totalBytes
                                                               + "\n, fileName: " + fileName + ", appName: " + appName);
            }

            public void onDownloadPause(string placementId, ATCallbackInfo callbackInfo, long totalBytes, long currBytes,
                string fileName, string appName)
            {
                Debug.Log("[ATDownloadListener] onDownloadPause------->" + JsonMapper.ToJson(callbackInfo.toDictionary())
                                                              + "\n, totalBytes: " + totalBytes + ", currBytes:" + currBytes
                                                              + "\n, fileName: " + fileName + ", appName: " + appName);
            }

            public void onDownloadStart(string placementId, ATCallbackInfo callbackInfo, long totalBytes, long currBytes,
                string fileName, string appName)
            {
                Debug.Log("[ATDownloadListener] onDownloadStart------->" + JsonMapper.ToJson(callbackInfo.toDictionary())
                                                              + "\n, totalBytes: " + totalBytes + ", currBytes:" + currBytes
                                                              + "\n, fileName: " + fileName + ", appName: " + appName);
            }

            public void onDownloadUpdate(string placementId, ATCallbackInfo callbackInfo, long totalBytes, long currBytes,
                string fileName, string appName)
            {
                Debug.Log("[ATDownloadListener] onDownloadUpdate------->" + JsonMapper.ToJson(callbackInfo.toDictionary())
                                                               + "\n, totalBytes: " + totalBytes + ", currBytes:" +
                                                               currBytes
                                                               + "\n, fileName: " + fileName + ", appName: " + appName);
            }

            public void onInstalled(string placementId, ATCallbackInfo callbackInfo, string fileName, string appName)
            {
                Debug.Log("[ATDownloadListener] onInstalled------->" + JsonMapper.ToJson(callbackInfo.toDictionary())
                                                          + "\n, fileName: " + fileName + ", appName: " + appName);
            }
        }
#endif
        
#region InterstitialAd
        /// <summary>
        /// 初始化插屏广告.
        /// 方法定义参数: sender 为广告类型对象，erg为返回信息
        /// </summary>
        private void InitializeInterstitialAd()
        {
#if FY_TAKU
            //广告加载成功
            ATInterstitialAd.Instance.client.onAdLoadEvent += onInterstitialAdLoad;
            //广告加载失败
            ATInterstitialAd.Instance.client.onAdLoadFailureEvent += onInterstitialAdLoadFail;
            //广告展示（可以依赖该回调进行展示统计）
            ATInterstitialAd.Instance.client.onAdShowEvent += onInterstitialAdShow;
            //广告展示失败
            ATInterstitialAd.Instance.client.onAdShowFailureEvent += onInterstitialAdShowFail;
            //广告点击
            ATInterstitialAd.Instance.client.onAdClickEvent += onInterstitialAdClick;
            //广告关闭
            ATInterstitialAd.Instance.client.onAdCloseEvent += onInterstitialAdClose;
            //视频广告播放开始
            ATInterstitialAd.Instance.client.onAdVideoStartEvent += startVideoPlayback;
            //视频广告播放结束
            ATInterstitialAd.Instance.client.onAdVideoEndEvent += endVideoPlayback;
            //视频广告播放失败
            ATInterstitialAd.Instance.client.onAdVideoFailureEvent += failVideoPlayback;
            
            //进阶监听设置
            //广告源启动请求
            ATInterstitialAd.Instance.client.onAdSourceAttemptEvent += startLoadingADSource;
            //广告源加载成功
            ATInterstitialAd.Instance.client.onAdSourceFilledEvent += finishLoadingADSource;
            //广告源加载失败
            ATInterstitialAd.Instance.client.onAdSourceLoadFailureEvent += failToLoadADSource;
            //广告源Bidding启动
            ATInterstitialAd.Instance.client.onAdSourceBiddingAttemptEvent += startBiddingADSource;
            //广告源Bidding成功
            ATInterstitialAd.Instance.client.onAdSourceBiddingFilledEvent += finishBiddingADSource;
            //广告源Bidding失败
            ATInterstitialAd.Instance.client.onAdSourceBiddingFailureEvent += failBiddingADSource;

            LoadInterstitialAd();
#endif
        }

        private void DestroyInterstitialAd()
        {
#if FY_TAKU
            ATInterstitialAd.Instance.client.onAdLoadEvent -= onInterstitialAdLoad;
            ATInterstitialAd.Instance.client.onAdLoadFailureEvent -= onInterstitialAdLoadFail;
            ATInterstitialAd.Instance.client.onAdShowEvent -= onInterstitialAdShow;
            ATInterstitialAd.Instance.client.onAdShowFailureEvent -= onInterstitialAdShowFail;
            ATInterstitialAd.Instance.client.onAdClickEvent -= onInterstitialAdClick;
            ATInterstitialAd.Instance.client.onAdCloseEvent -= onInterstitialAdClose;
            ATInterstitialAd.Instance.client.onAdVideoStartEvent -= startVideoPlayback;
            ATInterstitialAd.Instance.client.onAdVideoEndEvent -= endVideoPlayback;
            ATInterstitialAd.Instance.client.onAdVideoFailureEvent -= failVideoPlayback;
            
            ATInterstitialAd.Instance.client.onAdSourceAttemptEvent -= startLoadingADSource;
            ATInterstitialAd.Instance.client.onAdSourceFilledEvent -= finishLoadingADSource;
            ATInterstitialAd.Instance.client.onAdSourceLoadFailureEvent -= failToLoadADSource;
            ATInterstitialAd.Instance.client.onAdSourceBiddingAttemptEvent -= startBiddingADSource;
            ATInterstitialAd.Instance.client.onAdSourceBiddingFilledEvent -= finishBiddingADSource;
            ATInterstitialAd.Instance.client.onAdSourceBiddingFailureEvent -= failBiddingADSource;
#endif
        }

        /// <summary>
        /// 启动应用时通过ATInterstitialAd.loadInterstitialAd来执行加载广告
        /// </summary>
        public void LoadInterstitialAd()
        {
#if FY_TAKU
            Debug.Log("[TakuUnAds] LoadInterstitialAd() >>> ");
            Dictionary<string, object> jsonmap = new Dictionary<string, object>();
            jsonmap.Add(AnyThinkAds.Api.ATConst.USE_REWARDED_VIDEO_AS_INTERSTITIAL, AnyThinkAds.Api.ATConst.USE_REWARDED_VIDEO_AS_INTERSTITIAL_NO);
            //jsonmap.Add(AnyThinkAds.Api.ATConst.USE_REWARDED_VIDEO_AS_INTERSTITIAL, AnyThinkAds.Api.ATConst.USE_REWARDED_VIDEO_AS_INTERSTITIAL_YES);
            ATInterstitialAd.Instance.loadInterstitialAd(this.insert_id, jsonmap);//加载广告
#endif
        }
        
        /// <summary>
        /// 在需要展示广告的位置通过ATInterstitialAd.hasInterstitialAdReady判断是否能展示
        /// false：重新执行ATInterstitialAd.loadInterstitialAd来加载广告
        /// true：执行ATInterstitialAd.showInterstitialAd展示广告，
        /// 在onInterstitialAdClose的回调中再执行ATInterstitialAd.loadInterstitialAd来预加载下一次的广告
        /// （在onInterstitialAdClose的回调里可直接调用loadInterstitialAd不需要经过hasInterstitialAdReady的判断，
        /// 有助于提升优先级比较高的广告源的展示量.）
        /// </summary>
        public void showInterstitialAd()
        {
#if FY_TAKU
            Debug.Log("[TakuUnAds] showInterstitialAd() >>> ");
            bool isAdReady = ATInterstitialAd.Instance.hasInterstitialAdReady(this.insert_id);//判断是否有广告缓存
            if (isAdReady)
            {
                //与激励视频相同，插屏广告只要调用展示api并传递展示广告位ID作为参数：
                ATInterstitialAd.Instance.showInterstitialAd(this.insert_id);//显示广告
                
                //当用到 场景 功能时：
                // Dictionary<string, string> jsonmap = new Dictionary<string, string>();
                // jsonmap.Add(AnyThinkAds.Api.ATConst.SCENARIO, showingScenario);
                // ATInterstitialAd.Instance.showInterstitialAd(this.insert_id, jsonmap);//显示广告
            }
            else
            {
                LoadInterstitialAd();
            }
#endif
        }

#if FY_TAKU
        private void onInterstitialAdLoad(object sender, ATAdEventArgs erg)
        {
            Debug.Log("[TakuUnAds] onInterstitialAdLoad: " + erg.placementId);
            this.insert_load = 1;
            retryInterstitialAdAttemptCount = 0;
        }

        public void onInterstitialAdLoadFail(object sender, ATAdErrorEventArgs erg)
        {
            Debug.Log("[TakuUnAds] onInterstitialAdLoadFail --erg.placementId:" + erg.placementId + " --erg.errorCode:" + erg.errorCode + " --msg:" + erg.errorMessage);
            
            this.insert_load = -1;
            // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            retryInterstitialAdAttempt();
        }

        public void onInterstitialAdShow(object sender, ATAdEventArgs erg)
        {
            Debug.Log("[TakuUnAds] onInterstitialAdShow: " + erg.placementId);
        }

        public void onInterstitialAdShowFail(object sender, ATAdErrorEventArgs erg)
        {
            //Interstitial ad failed to display. We recommend loading the next ad
            Debug.Log("[TakuUnAds] onInterstitialAdShowFail: " + erg.placementId);
            
            this.insert_load = -1;
            this.DoShowback(AdvertCode.None);
            this.loadInsertAd();
        }

        public void onInterstitialAdClick(object sender, ATAdEventArgs erg)
        {
            Debug.Log("[TakuUnAds] onInterstitialAdClick: " + erg.placementId + "->" + JsonMapper.ToJson(erg.callbackInfo.toDictionary()));
        }

        public void onInterstitialAdClose(object sender, ATAdEventArgs erg)
        {
            // Interstitial ad is hidden. Pre-load the next ad
            Debug.Log("[TakuUnAds] onInterstitialAdClose: " + erg.placementId);
            
            this.insert_load = -1;
            this.DoShowback(AdvertCode.Insert);
            this.loadInsertAd();
        }

        /// <summary>
        /// 广告视频开始播放，部分平台有此回调
        /// </summary>
        public void startVideoPlayback(object sender, ATAdEventArgs erg)
        {
            Debug.Log("[TakuUnAds] startVideoPlayback------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toAdsourceDictionary()));
        }

        /// <summary>
        /// 广告视频播放结束，部分广告平台有此回调
        /// </summary>
        public void endVideoPlayback(object sender, ATAdEventArgs erg)
        {
            Debug.Log("[TakuUnAds] endVideoPlayback------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toAdsourceDictionary()));
        }

        /// <summary>
        /// 广告视频播放失败，部分广告平台有此回调
        /// </summary>
        public void failVideoPlayback(object sender, ATAdEventArgs erg)
        {
            Debug.Log("[TakuUnAds] failVideoPlayback------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toAdsourceDictionary()));
        }

        public int retryInterstitialAdAttemptCount;
        
        private void retryInterstitialAdAttempt()
        {
            //ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            retryInterstitialAdAttemptCount++;
            double retryDelay = Math.Pow(2, Math.Min(6, retryInterstitialAdAttemptCount));
            // Debug.Log("setLoadFailed() >>> retryDelay: " + retryDelay + " retryLoadAdAttemptEvent: " + retryLoadAdAttemptEvent);
            loadInterstitialAdCoroutine = StartCoroutine(CorLoadinterstitalAd((float)retryDelay));
        }

        private IEnumerator CorLoadinterstitalAd(float delay)
        {
            var timer = 0f;
            while (timer < delay)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            loadInterstitialAdCoroutine = null;
            loadInsertAd();
        }
#endif
#endregion

#region 视频奖励广告
        /// <summary>
        /// 初始化奖励广告
        /// </summary>
        public void InitializeRewardAd()
        {
#if FY_TAKU
            //广告加载成功
            ATRewardedVideo.Instance.client.onAdLoadEvent += onRewardAdLoad;
            //广告加载失败
            ATRewardedVideo.Instance.client.onAdLoadFailureEvent += onRewardAdLoadFail;
            //广告展示的回调（可依赖这个回调统计展示数据）
            ATRewardedVideo.Instance.client.onAdVideoStartEvent += onAdVideoStartEvent;
            //广告播放结束
            ATRewardedVideo.Instance.client.onAdVideoEndEvent += onAdVideoEndEvent;
            //广告视频播放失败
            ATRewardedVideo.Instance.client.onAdVideoFailureEvent += onAdVideoPlayFailure;
            //广告点击
            ATRewardedVideo.Instance.client.onAdClickEvent += onRewardAdClick;
            //广告激励回调（可依赖该监听下发游戏激励）
            ATRewardedVideo.Instance.client.onRewardEvent += onReward;
            //广告被关闭
            ATRewardedVideo.Instance.client.onAdVideoCloseEvent += onAdVideoClosedEvent;
            
            //进阶监听设置：
            //广告源启动请求
            ATRewardedVideo.Instance.client.onAdSourceAttemptEvent += startLoadingADSource;
            //广告源加载成功
            ATRewardedVideo.Instance.client.onAdSourceFilledEvent += finishLoadingADSource;
            //广告源加载失败
            ATRewardedVideo.Instance.client.onAdSourceLoadFailureEvent += failToLoadADSource;
            //广告源Bidding启动
            ATRewardedVideo.Instance.client.onAdSourceBiddingAttemptEvent += startBiddingADSource;
            //广告源Bidding成功
            ATRewardedVideo.Instance.client.onAdSourceBiddingFilledEvent += finishBiddingADSource;
            //广告源Bidding失败
            ATRewardedVideo.Instance.client.onAdSourceBiddingFailureEvent += failBiddingADSource;
            
            //再看一个回调：仅穿山甲支持再看一个回调
            // 激励视频再看一次视频开始播放
            ATRewardedVideo.Instance.client.onPlayAgainStart += onRewardedVideoAdAgainPlayStart;
            // 激励视频再看一次视频播放失败
            ATRewardedVideo.Instance.client.onPlayAgainFailure += onRewardedVideoAdAgainPlayFail;
            // 激励视频再看一次视频播放结束
            ATRewardedVideo.Instance.client.onPlayAgainEnd += onRewardedVideoAdAgainPlayEnd;
            // 激励视频再看一次视频点击
            ATRewardedVideo.Instance.client.onPlayAgainClick += onRewardedVideoAdAgainPlayClicked;
            // 激励视频再看一次视频激励下发
            ATRewardedVideo.Instance.client.onPlayAgainReward += onAgainReward;

            LoadRewardAd();
#endif
        }

        public void DestroyRewardAd()
        {
#if FY_TAKU
            ATRewardedVideo.Instance.client.onAdLoadEvent -= onRewardAdLoad;
            ATRewardedVideo.Instance.client.onAdLoadFailureEvent -= onRewardAdLoadFail;
            ATRewardedVideo.Instance.client.onRewardEvent -= onReward;
            ATRewardedVideo.Instance.client.onAdVideoCloseEvent -= onAdVideoClosedEvent;
            ATRewardedVideo.Instance.client.onAdVideoEndEvent -= onAdVideoEndEvent;
            ATRewardedVideo.Instance.client.onAdVideoStartEvent -= onAdVideoStartEvent;
            ATRewardedVideo.Instance.client.onAdVideoFailureEvent -= onAdVideoPlayFailure;
            ATRewardedVideo.Instance.client.onAdClickEvent -= onRewardAdClick;
            
            //进阶监听设置：
            ATRewardedVideo.Instance.client.onAdSourceAttemptEvent -= startLoadingADSource;
            ATRewardedVideo.Instance.client.onAdSourceFilledEvent -= finishLoadingADSource;
            ATRewardedVideo.Instance.client.onAdSourceLoadFailureEvent -= failToLoadADSource;
            ATRewardedVideo.Instance.client.onAdSourceBiddingAttemptEvent -= startBiddingADSource;
            ATRewardedVideo.Instance.client.onAdSourceBiddingFilledEvent -= finishBiddingADSource;
            ATRewardedVideo.Instance.client.onAdSourceBiddingFailureEvent -= failBiddingADSource;
            
            //再看一个回调：仅穿山甲支持再看一个回调
            ATRewardedVideo.Instance.client.onPlayAgainStart -= onRewardedVideoAdAgainPlayStart;
            ATRewardedVideo.Instance.client.onPlayAgainFailure -= onRewardedVideoAdAgainPlayFail;
            ATRewardedVideo.Instance.client.onPlayAgainEnd -= onRewardedVideoAdAgainPlayEnd;
            ATRewardedVideo.Instance.client.onPlayAgainClick -= onRewardedVideoAdAgainPlayClicked;
            ATRewardedVideo.Instance.client.onPlayAgainReward -= onAgainReward;
#endif
        }

        public void LoadRewardAd()
        {
#if FY_TAKU
            Debug.Log("[TakuUnAds] LoadRewardAd() >>> ");
            //ATSDKAPI.setCustomDataForPlacementID(new Dictionary<string, string> { { "placement_custom_key", "placement_custom" } }, this.reward_id);

            Dictionary<string, string> jsonmap = new Dictionary<string, string>();
            //如果需要通过开发者的服务器进行奖励的下发（部分广告平台支持此服务器激励），则需要传递下面两个key
            //ATConst.USERID_KEY必传，用于标识每个用户;ATConst.USER_EXTRA_DATA为可选参数，传入后将透传到开发者的服务器
            jsonmap.Add(ATConst.USERID_KEY, "test_user_id");
            jsonmap.Add(ATConst.USER_EXTRA_DATA, "test_user_extra_data");

            ATRewardedVideo.Instance.loadVideoAd(this.reward_id, jsonmap);//加载广告
            // ATRewardedVideo.Instance.addAutoLoadAdPlacementID(this.reward_id);
#endif
        }

        public void ShowRewardAd()
        {
#if FY_TAKU
            Debug.Log("[TakuUnAds] ShowRewardAd() >>> ");
            bool isAdReady = ATRewardedVideo.Instance.hasAdReady(this.reward_id);//判断是否有广告缓存
            if (isAdReady)
            {
                //显示广告
                ATRewardedVideo.Instance.showAd(this.reward_id);
                
                //使用场景功能显示广告
                // Dictionary<string, string> jsonmap = new Dictionary<string, string>();
                // jsonmap.Add(AnyThinkAds.Api.ATConst.SCENARIO, showingScenario);
                // ATRewardedVideo.Instance.showAd(this.reward_id, jsonmap);
            }
            else
            {
                LoadRewardAd();
            }
#endif
        }

#if FY_TAKU
        public void onRewardAdLoad(object sender, ATAdEventArgs erg)
        {
            Debug.Log("[TakuUnAds] onRewardAdLoad :" + erg.placementId);
            this.reward_load = 1;
            retryRewardAdAttemptCount = 0;
        }

        public void onRewardAdLoadFail(object sender, ATAdErrorEventArgs erg)
        {
            Debug.Log("[TakuUnAds] onRewardAdLoadFail : : " + erg.placementId + "--erg.errorCode:" + erg.errorCode + "--msg:" + erg.errorMessage);
            this.reward_load = -1;
            // Reward ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            retryRewardAdAttempt();
        }

        public void onRewardAdClick(object sender, ATAdEventArgs erg)
        {
            Debug.Log("[TakuUnAds] onRewardAdClick :" + erg.placementId + "->" + JsonMapper.ToJson(erg.callbackInfo.toDictionary()));
        }

        public void onRewardAdClose(object sender, ATAdRewardEventArgs erg)
        {
            Debug.Log("[TakuUnAds] onRewardAdClose :" + erg.placementId);
            
        }
        
        public void onAdVideoStartEvent(object sender, ATAdEventArgs erg)
        {
            Debug.Log("[TakuUnAds] onAdVideoStartEvent------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toDictionary()));
        }

        public void onAdVideoEndEvent(object sender, ATAdEventArgs erg)
        {
            Debug.Log("[TakuUnAds] onAdVideoEndEvent------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toDictionary()));
        }

        public void onAdVideoClosedEvent(object sender, ATAdEventArgs erg)
        {
            // Rewarded ad is hidden. Pre-load the next ad
            Debug.Log("[TakuUnAds] onAdVideoClosedEvent------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toDictionary()));
            LoadRewardAd();
        }

        public void onAdVideoPlayFailure(object sender, ATAdErrorEventArgs erg)
        {
            // Rewarded ad failed to display. We recommend loading the next ad
            Debug.Log("[TakuUnAds] onAdVideoClosedEvent------" + "->" + JsonMapper.ToJson(erg.errorMessage));
            
            this.reward_load = -1;
            this.DoShowback(AdvertCode.None);
            this.LoadRewardAd();
        }

        public void onReward(object sender, ATAdEventArgs erg)
        {
            Debug.Log("[TakuUnAds] onReward------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toDictionary()));
            
            this.reward_load = -1;
            this.DoShowback(AdvertCode.Reward);
            this.LoadRewardAd();
        }

        public void onRewardedVideoAdAgainPlayStart(object sender, ATAdEventArgs erg)
        {
            Debug.Log("[TakuUnAds] onRewardedVideoAdAgainPlayStart------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toDictionary()));
        }

        public void onRewardedVideoAdAgainPlayEnd(object sender, ATAdEventArgs erg)
        {
            Debug.Log("[TakuUnAds] onRewardedVideoAdAgainPlayEnd------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toDictionary()));
        }

        public void onRewardedVideoAdAgainPlayFail(object sender, ATAdErrorEventArgs erg)
        {
            Debug.Log("[TakuUnAds] onRewardedVideoAdAgainPlayFail------code:" + erg.errorCode + "---message:" + erg.errorMessage);
        }

        public void onRewardedVideoAdAgainPlayClicked(object sender, ATAdEventArgs erg)
        {
            Debug.Log("[TakuUnAds] onRewardedVideoAdAgainPlayClicked------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toDictionary()));

        }

        public void onAgainReward(object sender, ATAdEventArgs erg)
        {
            Debug.Log("[TakuUnAds] onAgainReward------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toDictionary()));
        }

        public int retryRewardAdAttemptCount;
        public void retryRewardAdAttempt()
        {
            //ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            retryRewardAdAttemptCount++;
            double retryDelay = Math.Pow(2, Math.Min(6, retryRewardAdAttemptCount));
            // Debug.Log("setLoadFailed() >>> retryDelay: " + retryDelay + " retryLoadAdAttemptEvent: " + retryLoadAdAttemptEvent);
            loadRewardAdCoroutine = StartCoroutine(CorLoadRewardAd((float)retryDelay));
        }

        private IEnumerator CorLoadRewardAd(float delay)
        {
            var timer = 0f;
            while (timer < delay)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            loadRewardAdCoroutine = null;
            loadRewardAd();
        }
#endif
#endregion


#region Common
#if FY_TAKU
        // v5.8.10 新增广告源层级回调
        /// <summary>
        /// 广告源开始加载
        /// </summary>
        public void startLoadingADSource(object sender, ATAdEventArgs erg)
        {
            Debug.Log("[TakuUnAds] startLoadingADSource------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toAdsourceDictionary()));
        }

        /// <summary>
        /// 广告源加载完成
        /// </summary>
        public void finishLoadingADSource(object sender, ATAdEventArgs erg)
        {
            Debug.Log("[TakuUnAds] finishLoadingADSource------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toAdsourceDictionary()));
        }

        /// <summary>
        /// 广告源失败
        /// </summary>
        public void failToLoadADSource(object sender, ATAdErrorEventArgs erg)
        {
            Debug.Log("[TakuUnAds] failToLoadADSource------erg.errorCode:" + erg.errorCode + "---erg.errorMessage:" + erg.errorMessage + "->" + JsonMapper.ToJson(erg.callbackInfo.toAdsourceDictionary()));
        }

        /// <summary>
        /// 广告源开始bidding
        /// </summary>
        public void startBiddingADSource(object sender, ATAdEventArgs erg)
        {
            Debug.Log("[TakuUnAds] startBiddingADSource------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toAdsourceDictionary()));
        }

        /// <summary>
        /// 广告源bidding成功
        /// </summary>
        public void finishBiddingADSource(object sender, ATAdEventArgs erg)
        {
            Debug.Log("[TakuUnAds] finishBiddingADSource------" + "->" + JsonMapper.ToJson(erg.callbackInfo.toAdsourceDictionary()));
        }

        /// <summary>
        /// 广告源bidding失败
        /// </summary>
        public void failBiddingADSource(object sender, ATAdErrorEventArgs erg)
        {
            Debug.Log("[TakuUnAds] failBiddingADSource------erg.errorCode:" + erg.errorCode + "---erg.errorMessage:" + erg.errorMessage + "->" + JsonMapper.ToJson(erg.callbackInfo.toAdsourceDictionary()));
        }
#endif
#endregion
    }
}