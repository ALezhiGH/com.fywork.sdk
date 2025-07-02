using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Fyw
{
    /// <summary>
    /// 正常更新弹窗
    /// </summary>
    public class NupdForm : MonoBehaviour
    {
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public Button CloseBtn;

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
        /// 设置关闭回调
        /// </summary>
        /// <param name="call"></param>
        public void SetClose(UnityAction call)
        {
            this.CloseBtn.onClick.RemoveAllListeners();
            this.CloseBtn.onClick.AddListener(call);
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