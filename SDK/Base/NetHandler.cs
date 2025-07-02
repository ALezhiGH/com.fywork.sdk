using System.Collections;
using UnityEngine.Networking;

namespace SDK
{
    /// <summary>
    /// 网络处理器
    /// </summary>
    public abstract class NetHandler : ServHandler
    {
        /// <summary>
        /// 访问路径
        /// </summary>
        protected string url;

        /// <summary>
        /// 唤醒处理
        /// </summary>
        private void Awake()
        {
            this.url = string.Empty;
        }

        /// <summary>
        /// Http异步Get
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        protected IEnumerator Get(string uri)
        {
            //LogTool.Norm($"WebRequest start:{uri}");
            //进行Http访问
            UnityWebRequest request = UnityWebRequest.Get(uri);
            request.timeout = 60;
            yield return request.SendWebRequest();

            //判断是否出现错误
            if (request.result != UnityWebRequest.Result.Success)
            {
                //打印错误信息
                LogTool.Error("Server Data Load " + request.error);
            }
            else
            {
                //判断是否返回200
                if (request.responseCode == 200)
                {
                    //回调处理
                    this.Parse(request.downloadHandler.text);
                }
                else
                {
                    //打印错误信息
                    LogTool.Error("Server Data Load " + request.responseCode);
                }
            }
            this.Back();
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="data"></param>
        protected virtual void Parse(string data)
        {

        }
    }
}