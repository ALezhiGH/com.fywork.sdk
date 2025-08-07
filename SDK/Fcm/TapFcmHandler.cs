using System.Collections;
using System;
using TapSDK.Core;
using TapSDK.Compliance;
using UnityEngine;

namespace SDK
{
    /// <summary>
    /// Tap防沉迷处理器
    /// </summary>
    public class TapFcmHandler : FcmHandler
    {
        /// <summary>
        /// Token
        /// </summary>
        protected string token;

        /// <summary>
        /// 保存key
        /// </summary>
        private string save = "";

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="args"></param>
        protected override void Init(Hashtable args)
        {
            this.key = args["key"].ToString();
            this.token = args["token"].ToString();
            this.save = args["save"].ToString();
            this.InitSDK();
        }

        /// <summary>
        /// 唤起
        /// </summary>
        protected override void Call()
        {
            // LogTool.Norm($"[TapFcmHandler] Call.  chnl_ctrl.OpenID:{chnl_ctrl.OpenID}");
            string stamp;
            if (!PlayerPrefs.HasKey(this.save))
            {
                stamp = ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds).ToString();
                PlayerPrefs.SetString(this.save, stamp);
                PlayerPrefs.Save();
            }
            else
            {
                stamp = PlayerPrefs.GetString(this.save);
            }
            TapTapCompliance.Startup(this.chnl_ctrl.OpenID + "_" + stamp);
        }

        /// <summary>
        /// 初始化防沉迷
        /// </summary>
        protected void InitSDK()
        {
            // 核心配置
            TapTapSdkOptions coreOptions = new TapTapSdkOptions
            {
                clientId = this.key,
                clientToken = this.token
            };
            // 合规认证配置
            TapTapComplianceOption complianceOption = new TapTapComplianceOption
            {
                showSwitchAccount = false, // 是否显示切换账号按钮
                useAgeRange = false // 是否使用年龄段信息
            };
            // 其他模块配置项
            TapTapSdkBaseOptions[] otherOptions = new TapTapSdkBaseOptions[]
            {
                complianceOption
            };
            // TapSDK 初始化
            TapTapSDK.Init(coreOptions, otherOptions);
            Action<int, string> callback = (code, errorMsg) =>
            {
                // 根据回调返回的参数 code 添加不同情况的处理
                switch (code)
                {
                    case 500: // 玩家未受限制，可正常开始
                        this._complianced = true;
                        this.Back(); //Call回调
                        break;
                    case 1000: // 防沉迷认证凭证无效时触发
                        PlatGeneric.ExitApp();
                        break;
                    case 1001: // 切换账号处理 因不允许切换 故不处理
                        break;
                    case 1030: // 用户当前时间无法进行游戏 Tap处理
                    case 1050: // 用户无可玩时长 Tap处理
                        break;
                    case 1100: // 当前用户因触发应用设置的年龄限制无法进入游戏
                        PlatGeneric.ExitApp();
                        break;
                    case 1200: // 数据请求失败，应用信息错误或网络连接异常
                        break;
                    case 9002: // 实名认证过程中玩家关闭了实名窗口
                        PlatGeneric.ExitApp();
                        break;
                }
                LogTool.Norm($"TapFcm code: {code} error Message: {errorMsg}");
            };
            TapTapCompliance.RegisterComplianceCallback(callback);
            this.Back(); //Load回调
        }
    }
}