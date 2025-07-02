using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Fyw
{
    /// <summary>
    /// 资质弹窗
    /// </summary>
    public class CredForm : MonoBehaviour
    {
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public Button CloseBtn;

        /// <summary>
        /// 列表面板
        /// </summary>
        public Transform ListPanel;

        /// <summary>
        /// 显示数据
        /// </summary>
        /// <param name="datas"></param>
        public void ShowData(List<CredData> datas)
        {
            int index = 0;
            for (; index < this.ListPanel.childCount; index++)
            {
                this.ListPanel.GetChild(index).gameObject.SetActive(false);
            }
            if (null == datas || datas.Count == 0)
            {
                return;
            }
            index = 0;
            Transform child;
            foreach (var item in datas)
            {
                if (index >= this.ListPanel.childCount)
                {
                    GameObject copy = this.ListPanel.GetChild(0).gameObject;
                    child = GameObject.Instantiate(copy).transform;
                    child.SetParent(this.ListPanel);
                    child.localScale = new Vector3(1, 1, 1);
                    child.localPosition = new Vector3(0, 0, 0);
                }
                else
                {
                    child = this.ListPanel.GetChild(index);
                }
                child.gameObject.SetActive(true);
                child.GetComponent<CredPage>().Init(item);
                index += 1;
            }
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
    }
}