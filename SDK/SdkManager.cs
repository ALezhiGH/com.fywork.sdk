using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SDK
{
    /// <summary>
    /// SDK管理器
    /// </summary>
    public class SdkManager
    {
        /// <summary>
        /// 流程类型
        /// </summary>
        protected enum FlowType
        {
            Init = 1,
            Call = 2,
        }

        /// <summary>
        /// 唯一句柄
        /// </summary>
        private static SdkManager instance;

        /// <summary>
        /// 获取实例
        /// </summary>
        public static SdkManager Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = new SdkManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// 流程参数
        /// </summary>
        private Hashtable flow_args;

        /// <summary>
        /// 渠道控制器
        /// </summary>
        private ChnlControl chnl_ctrl;

        /// <summary>
        /// 跳过执行的流程
        /// </summary>
        /// <returns></returns>
        private HashSet<string> ignoredFlowServers;

        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool IsInited { get; private set; }

        /// <summary>
        /// 是否已执行Load
        /// </summary>
        public bool LoadExecuted { get; private set; }
        
        /// <summary>
        /// 是否已加载完成
        /// </summary>
        public bool LoadCompleted { get; private set; }
        
        /// <summary>
        /// 是否已执行Work
        /// </summary>
        public bool WorkExecuted { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        private SdkManager()
        {
            this.flow_args = null;
            this.chnl_ctrl = null;
            this.ignoredFlowServers = new HashSet<string>();
        }

        /// <summary>
        /// 添加忽略服务
        /// </summary>
        /// <param name="server"></param>
        public void AddIgnoredFlowServer(string server)
        {
            this.ignoredFlowServers.Add(server);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            //流程配置加载
            TextAsset asset = Resources.Load<TextAsset>("flow");
            this.flow_args = JsonTool.jsonDecode(asset.text) as Hashtable;
            if (null == flow_args)
            {
                LogTool.Error("流程配置错误");
                return;
            }

            //渠道配置加载
            asset = Resources.Load<TextAsset>("chnl");
            Hashtable chnl_args = JsonTool.jsonDecode(asset.text) as Hashtable;
            if (null == asset)
            {
                LogTool.Error("渠道配置错误");
                return;
            }

            //物体初始化
            GameObject obj = new GameObject("SDK");
            Object.DontDestroyOnLoad(obj);//防止销毁

            //渠道初始化
            this.chnl_ctrl = obj.AddComponent<ChnlControl>();
            this.chnl_ctrl.Init(chnl_args);
            this.IsInited = true;
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="end"></param>
        public void Load(UnityAction end = null)
        {
            if (!this.LoadExecuted)
            {
                //LogTool.Norm("[SdkManager] Start Load");
                this.LoadExecuted = true;
                ArrayList list = this.flow_args["load"] as ArrayList;
                this.Exec(list, 0, () =>
                {
                    //LogTool.Norm("[SdkManager] LoadCompleted");
                    this.LoadCompleted = true;
                    end?.Invoke();
                });
            }
        }

        /// <summary>
        /// 工作
        /// </summary>
        public void Work(UnityAction end = null)
        {
            if (!this.WorkExecuted)
            {
                //LogTool.Norm("[SdkManager] Start Work");
                this.WorkExecuted = true;
                ArrayList list = this.flow_args["work"] as ArrayList;
                this.Exec(list, 0, () =>
                {
                    //LogTool.Norm("[SdkManager] WorkCompleted");
                    end?.Invoke();
                });
            }
        }

        /// <summary>
        /// 执行服务流
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <param name="end"></param>
        protected void Exec(ArrayList list, int index, UnityAction end)
        {
            if (list.Count <= index)
            {
                end?.Invoke();
                return;
            }
            string serv_name;
            Hashtable temp = list[index] as Hashtable;
            ArrayList servs = temp["servs"] as ArrayList;
            int serv_count = servs.Count, serv_index = 0;
            UnityAction back = () =>
            {
                serv_index++;
                if (serv_index >= serv_count)
                {
                    this.Exec(list, index + 1, end);
                }
            };
            switch ((FlowType)int.Parse(temp["kind"].ToString()))
            {
                case FlowType.Init:
                    foreach (var serv in servs)
                    {
                        serv_name = serv.ToString();
                        // LogTool.Norm($"Init: {serv_name}");
                        this.chnl_ctrl.Init(serv_name, back);
                    }
                    break;
                default:
                    foreach (var serv in servs)
                    {
                        serv_name = serv.ToString();
                        // LogTool.Norm($"Call: {serv_name}");
                        //TODO 跳过忽略的流程
                        if (this.ignoredFlowServers.Contains(serv_name))
                        {
                            LogTool.Norm($"跳过流程 Call: {serv_name}");
                            back();
                            return;
                        }
                        this.chnl_ctrl.Call(serv_name, back);
                    }
                    break;
            }
        }
    }
}