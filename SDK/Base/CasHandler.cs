
using UnityEngine.Events;

namespace SDK
{
    /// <summary>
    /// 云存储处理器
    /// </summary>
    public abstract class CasHandler : NetHandler
    {
        /// <summary>
        /// 调用处理
        /// </summary>
        /// <param name="call"></param>
        protected override void Call()
        {
            this.Back();
        }

        /// <summary>
        /// 上传数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="back"></param>
        /// <returns></returns>
        public abstract void UploadData(string data, UnityAction<bool, string> back);

        /// <summary>
        /// 下载数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="back"></param>
        /// <returns></returns>
        public abstract void DwloadData(string data, UnityAction<bool, string> back);
    }
}