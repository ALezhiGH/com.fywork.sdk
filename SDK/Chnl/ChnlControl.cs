using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace SDK
{
    /// <summary>
    /// 渠道控制器
    /// </summary>
    public class ChnlControl : MonoBehaviour
    {
        /// <summary>
        /// 控制接口
        /// </summary>
        public static ChnlControl Instance;

        /// <summary>
        /// 平台ID
        /// </summary>
        protected int plat;

        /// <summary>
        /// 渠道ID
        /// </summary>
        protected int chnl;

        /// <summary>
        /// 游戏ID
        /// </summary>
        protected int game;

        /// <summary>
        /// 渠道参数
        /// </summary>
        private Hashtable args;

        /// <summary>
        /// 所有服务
        /// </summary>
        private Dictionary<string, ServHandler> servs;

        /// <summary>
        /// 平台ID
        /// </summary>
        public int Plat { get => this.plat; }

        /// <summary>
        /// 渠道ID
        /// </summary>
        public int Chnl { get => this.chnl; }

        /// <summary>
        /// 游戏ID
        /// </summary>
        public int Game { get => this.game; }

        /// <summary>
        /// 唤醒
        /// </summary>
        void Awake()
        {
            Instance = this;
            this.servs = new Dictionary<string, ServHandler>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="args"></param>
        public void Init(Hashtable args)
        {
            this.args = args;
            this.plat = int.Parse(args["plat"].ToString());
            this.chnl = int.Parse(args["chnl"].ToString());
            this.game = int.Parse(args["game"].ToString());
            //LogTool.Norm($"[ChnlControl] Init. plat:{plat}, chnl:{chnl}, game:{game}");
        }

        /// <summary>
        /// 初始化服务
        /// </summary>
        /// <param name="back"></param>
        public void Init(string serv, UnityAction back)
        {
            Hashtable conf;
            string cnty_serv = serv + "_" + this.Cnty.ToLower();
            //LogTool.Norm($"[ChnlControl] InitServer.  serv:{serv}  cnty_serv:{cnty_serv}");
            if (this.args.ContainsKey(cnty_serv))
            {
                conf = args[cnty_serv] as Hashtable;
            }
            else if (this.args.ContainsKey(serv))
            {
                conf = args[serv] as Hashtable;
            }
            else
            {
                back?.Invoke();
                return;
            }
            string name = conf["hdlr"].ToString();
            Assembly assembly = Assembly.GetExecutingAssembly();
            ServHandler hdlr = this.gameObject.AddComponent(assembly.GetType("SDK." + name)) as ServHandler;
            if (null == hdlr)
            {
                back?.Invoke();
                return;
            }
            this.servs[serv] = hdlr;
            //LogTool.Norm($"[ChnlControl] --Add serv:{serv}  name:{name}");
            hdlr.Init(this, conf, back);
        }

        /// <summary>
        /// 调用服务
        /// </summary>
        /// <param name="serv"></param>
        /// <param name="back"></param>
        public void Call(string serv, UnityAction back)
        {
            ServHandler hdlr = this.Get(serv);
            if (null == hdlr)
            {
                back?.Invoke();
            }
            else
            {
                hdlr.Call(back);
            }
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="serv"></param>
        /// <returns></returns>
        public ServHandler Get(string serv)
        {
            if (!this.servs.ContainsKey(serv))
            {
                return null;
            }
            else
            {
                return this.servs[serv];
            }
        }

        #region 属性

        /// <summary>
        /// 隐私链接
        /// </summary>
        public string PrivUrl
        {
            get
            {
                AgrtHandler hdlr = this.Get("agrt") as AgrtHandler;
                if (null == hdlr)
                {
                    return string.Empty;
                }
                else
                {
                    return hdlr.PrivUrl;
                }
            }
        }

        /// <summary>
        /// 服务链接
        /// </summary>
        public string ServUrl
        {
            get
            {
                AgrtHandler hdlr = this.Get("agrt") as AgrtHandler;
                if (null == hdlr)
                {
                    return string.Empty;
                }
                else
                {
                    return hdlr.ServUrl;
                }
            }
        }

        /// <summary>
        /// IP
        /// </summary>
        public string IP
        {
            get
            {
                PosHandler hdlr = this.Get("pos") as PosHandler;
                if (null == hdlr)
                {
                    return string.Empty;
                }
                else
                {
                    return hdlr.PosData.IP;
                }
            }
        }

        /// <summary>
        /// 城市
        /// </summary>
        public string City
        {
            get
            {
                PosHandler hdlr = this.Get("pos") as PosHandler;
                if (null == hdlr)
                {
                    return string.Empty;
                }
                else
                {
                    return hdlr.PosData.Cnty;
                }
            }
        }

        /// <summary>
        /// 地区
        /// </summary>
        public string Area
        {
            get
            {
                PosHandler hdlr = this.Get("pos") as PosHandler;
                if (null == hdlr)
                {
                    return string.Empty;
                }
                else
                {
                    return hdlr.PosData.Area;
                }
            }
        }

        /// <summary>
        /// 国区
        /// </summary>
        public string Cnty
        {
            get
            {
                PosHandler hdlr = this.Get("pos") as PosHandler;
                if (null == hdlr)
                {
                    return RegionInfo.CurrentRegion.Name;
                }
                else
                {
                    return hdlr.PosData.Cnty;
                }
            }
        }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName
        {
            get
            {
                LogHandler hdlr = this.Get("log") as LogHandler;
                if (null == hdlr)
                {
                    return string.Empty;
                }
                else
                {
                    return hdlr.LogData.Name;
                }
            }
        }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string UserHead
        {
            get
            {
                LogHandler hdlr = this.Get("log") as LogHandler;
                if (null == hdlr)
                {
                    return string.Empty;
                }
                else
                {
                    return hdlr.LogData.Head;
                }
            }
        }

        /// <summary>
        /// 访问token
        /// </summary>
        public string UserToken
        {
            get
            {
                LogHandler hdlr = this.Get("log") as LogHandler;
                if (null == hdlr)
                {
                    return string.Empty;
                }
                else
                {
                    return hdlr.LogData.Token;
                }
            }
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID
        {
            get
            {
                LogHandler hdlr = this.Get("log") as LogHandler;
                if (null == hdlr)
                {
                    return string.Empty;
                }
                else
                {
                    return hdlr.LogData.UserID;
                }
            }
        }

        /// <summary>
        /// 用户游戏ID
        /// </summary>
        public string OpenID
        {
            get
            {
                LogHandler hdlr = this.Get("log") as LogHandler;
                if (null == hdlr)
                {
                    return string.Empty;
                }
                else
                {
                    return hdlr.LogData.OpenID;
                }
            }
        }

        /// <summary>
        /// 用户厂商ID
        /// </summary>
        public string UnionID
        {
            get
            {
                LogHandler hdlr = this.Get("log") as LogHandler;
                if (null == hdlr)
                {
                    return string.Empty;
                }
                else
                {
                    return hdlr.LogData.UnionID;
                }
            }
        }

        /// <summary>
        /// 能否进行付费
        /// </summary>
        public bool CanPay
        {
            get
            {
                CtrlHandler hdlr = this.Get("ctrl") as CtrlHandler;
                if (null == hdlr)
                {
                    return false;
                }
                else
                {
                    return hdlr.CtrlData.CanPay;
                }
            }
        }

        /// <summary>
        /// 能否显示广告
        /// </summary>
        public bool CanAds
        {
            get
            {
                CtrlHandler hdlr = this.Get("ctrl") as CtrlHandler;
                if (null == hdlr)
                {
                    return false;
                }
                else
                {
                    return hdlr.CtrlData.CanAds;
                }
            }
        }

        /// <summary>
        /// 能否分享
        /// </summary>
        public bool CanShr
        {
            get
            {
                CtrlHandler hdlr = this.Get("ctrl") as CtrlHandler;
                if (null == hdlr)
                {
                    return false;
                }
                else
                {
                    return hdlr.CtrlData.CanShr;
                }
            }
        }

        /// <summary>
        /// 保存链接
        /// </summary>
        public string SaveUrl
        {
            get
            {
                CtrlHandler hdlr = this.Get("ctrl") as CtrlHandler;
                if (null == hdlr)
                {
                    return string.Empty;
                }
                else
                {
                    return hdlr.CtrlData.SaveUrl;
                }
            }
        }

        /// <summary>
        /// 排行链接
        /// </summary>
        public string RankUrl
        {
            get
            {
                CtrlHandler hdlr = this.Get("ctrl") as CtrlHandler;
                if (null == hdlr)
                {
                    return string.Empty;
                }
                else
                {
                    return hdlr.CtrlData.RankUrl;
                }
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 唤起协议
        /// </summary>
        /// <param name="back"></param>
        public void CallAgrt(UnityAction back = null)
        {
            this.Call("Agrt", back);
        }

        /// <summary>
        /// 唤起防沉迷
        /// </summary>
        /// <param name="back"></param>
        public void CallFcm(UnityAction back = null)
        {
            this.Call("fcm", back);
        }

        /// <summary>
        /// 唤起登录
        /// </summary>
        /// <param name="back"></param>
        public void CallReg(UnityAction back = null)
        {
            this.Call("reg", back);
        }

        /// <summary>
        /// 能否显示插屏广告 用于外部状态显示
        /// </summary>
        /// <returns></returns>
        public bool CanInsert()
        {
            AdsHandler hdlr = this.Get("ads") as AdsHandler;
            if (null == hdlr)
            {
                return false;
            }
            else
            {
                return hdlr.CanInsert();
            }
        }

        /// <summary>
        /// 能否显示激励广告 用于外部状态显示
        /// </summary>
        /// <returns></returns>
        public bool CanReward()
        {
            AdsHandler hdlr = this.Get("ads") as AdsHandler;
            if (null == hdlr)
            {
                return false;
            }
            else
            {
                return hdlr.CanReward();
            }
        }

        /// <summary>
        /// 显示插屏广告 用于外部直接调用
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        public bool ShowInsert(AdCallback call)
        {
            AdsHandler hdlr = this.Get("ads") as AdsHandler;
            if (null == hdlr)
            {
                if (null != call)
                {
                    call(AdvertCode.None);
                }
                return false;
            }
            else
            {
                return hdlr.ShowInsert(call);
            }
        }

        /// <summary>
        /// 显示激励广告 用于外部直接调用
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        public bool ShowReward(AdCallback call)
        {
            AdsHandler hdlr = this.Get("ads") as AdsHandler;
            if (null == hdlr)
            {
                if (null != call)
                {
                    call(AdvertCode.None);
                }
                return false;
            }
            else
            {
                return hdlr.ShowReward(call);
            }
        }

        /// <summary>
        /// 检测是否包含敏感词
        /// </summary>
        /// <param name="info"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool CheckWord(string info, out string word)
        {
            PbzHandler hdlr = this.Get("pbz") as PbzHandler;
            if (null == hdlr)
            {
                word = "";
                return false;
            }
            else
            {
                return hdlr.CheckWord(info, out word);
            }
        }

        /// <summary>
        /// 唤起更新
        /// </summary>
        /// <param name="back"></param>
        public void CallUpd(UnityAction back = null)
        {
            this.Call("upd", back);
        }

        /// <summary>
        /// 唤起评论
        /// </summary>
        /// <param name="back"></param>
        public void CallCmt(UnityAction back = null)
        {
            this.Call("cmt", back);
        }

        /// <summary>
        /// 唤起论坛
        /// </summary>
        /// <param name="back"></param>
        public void CallFrm(UnityAction back = null)
        {
            this.Call("frm", back);
        }

        /// <summary>
        /// 唤起推荐
        /// </summary>
        /// <param name="back"></param>
        public void CallRec(UnityAction back = null)
        {
            this.Call("rec", back);
        }

        /// <summary>
        /// 唤起联系
        /// </summary>
        /// <param name="back"></param>
        public void CallLink(UnityAction back = null)
        {
            this.Call("link", back);
        }

        /// <summary>
        /// 增加统计
        /// </summary>
        /// <param name="key"></param>
        public void AppendStat(string key)
        {
            StatHandler hdlr = this.Get("stat") as StatHandler;
            if (null == hdlr)
            {
                return;
            }
            hdlr.AppendStat(key);
        }

        /// <summary>
        /// 增加统计
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AppendStat(string key, int value)
        {
            StatHandler hdlr = this.Get("stat") as StatHandler;
            if (null == hdlr)
            {
                return;
            }
            hdlr.AppendStat(key, value);
        }

        /// <summary>
        /// 增加统计
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AppendStat(string key, string value)
        {
            StatHandler hdlr = this.Get("stat") as StatHandler;
            if (null == hdlr)
            {
                return;
            }
            hdlr.AppendStat(key, value);
        }

        /// <summary>
        /// 打开面板
        /// </summary>
        /// <param name="panel_name"></param>
        public void OpenPanel(string panel_name)
        {
            StatHandler hdlr = this.Get("stat") as StatHandler;
            if (null == hdlr)
            {
                return;
            }
            hdlr.OpenPanel(panel_name);
        }

        /// <summary>
        /// 关闭面板
        /// </summary>
        /// <param name="panel_name"></param>
        public void ClosePanel(string panel_name)
        {
            StatHandler hdlr = this.Get("stat") as StatHandler;
            if (null == hdlr)
            {
                return;
            }
            hdlr.ClosePanel(panel_name);
        }
        #endregion
    }
}