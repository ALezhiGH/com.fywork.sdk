using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Fyw
{
    /// <summary>
    /// 推荐弹窗
    /// </summary>
    public class RecForm : MonoBehaviour
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
        public void ShowData(List<RecData> datas)
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
            foreach (var item in this.RandSort(datas))
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
                child.GetComponent<RecPage>().Init(item);
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

        /// <summary>
        /// 随机顺序
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        private List<RecData> RandSort(List<RecData> datas)
        {
            int total_weight = 0, temp_weight;
            List<int> temp = new List<int>();
            List<RecData> old_list = new List<RecData>();
            List<RecData> new_list = new List<RecData>();
            foreach (var item in datas)
            {
                if (item.Prio > 0)
                {
                    total_weight += item.Prio;
                    temp.Add(item.Prio);
                    old_list.Add(item);
                }
            }
            int random, index;
            while (total_weight > 0)
            {
                random = Random.Range(0, total_weight);
                for (index = 0; index < temp.Count; index++)
                {
                    temp_weight = temp[index];
                    if (random < temp_weight)
                    {
                        new_list.Add(old_list[index]);
                        total_weight -= temp_weight;
                        temp.RemoveAt(index);
                        old_list.RemoveAt(index);
                        break;
                    }
                    else
                    {
                        random -= temp_weight;
                    }
                }
            }
            return new_list;
        }
    }
}