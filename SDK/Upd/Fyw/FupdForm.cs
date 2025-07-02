using UnityEngine;

namespace Fyw
{
    /// <summary>
    /// 强制更新弹窗
    /// </summary>
    public class FupdForm : MonoBehaviour
    {
        /// <summary>
        /// 更新数据
        /// </summary>
        private UpdData data;

        /// <summary>
        /// 显示数据
        /// </summary>
        /// <param name="data"></param>
        public void ShowData(UpdData data)
        {
            this.data = data;
        }

        /// <summary>
        /// 更新处理
        /// </summary>
        public void UpdClick()
        {
            Application.OpenURL(this.data.UpdUrl);
        }
    }
}