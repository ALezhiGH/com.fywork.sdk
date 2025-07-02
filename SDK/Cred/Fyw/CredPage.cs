using SDK;
using UnityEngine;
using UnityEngine.UI;

namespace Fyw
{
    /// <summary>
    /// 资质页
    /// </summary>
    public class CredPage : MonoBehaviour
    {
        /// <summary>
        /// 名称文本
        /// </summary>
        public Text NameText;

        /// <summary>
        /// 值文本
        /// </summary>
        public Text ValText;

        /// <summary>
        /// 查询对象
        /// </summary>
        public GameObject SeekObj;

        /// <summary>
        /// 数据
        /// </summary>
        private CredData data;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        public void Init(CredData data)
        {
            this.data = data;
            this.NameText.text = data.Name;
            this.ValText.text = data.Val;
            this.SeekObj.SetActive(data.Url != string.Empty);
        }

        /// <summary>
        /// 查询处理
        /// </summary>
        public void SeekClick()
        {
            Application.OpenURL(this.data.Url);
        }

        /// <summary>
        /// 复制处理
        /// </summary>
        public void CopyClick()
        {
            PlatGeneric.CopyText(this.data.Val);
        }
    }
}