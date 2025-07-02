using SDK;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Fyw
{
    /// <summary>
    /// 联系页
    /// </summary>
    public class LinkPage : MonoBehaviour
    {
        /// <summary>
        /// 名称文本
        /// </summary>
        public Text NameText;

        /// <summary>
        /// Key文本
        /// </summary>
        public Text KeyText;

        /// <summary>
        /// 图标
        /// </summary>
        public Image IconImage;

        /// <summary>
        /// 跳转对象
        /// </summary>
        public GameObject JumpObj;

        /// <summary>
        /// 数据
        /// </summary>
        private LinkData data;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        public void Init(LinkData data)
        {
            this.data = data;
            this.NameText.text = data.Name;
            this.KeyText.text = data.Key;
            if (null == data.Image)
            {
                this.StartCoroutine(this.LoadImage());
            }
            else
            {
                this.IconImage.sprite = data.Image;
            }
            this.JumpObj.SetActive(data.Path != string.Empty);
        }

        /// <summary>
        /// 跳转处理
        /// </summary>
        public void JumpClick()
        {
            PlatGeneric.JumpApp(this.data.Path);
        }

        /// <summary>
        /// 复制处理
        /// </summary>
        public void CopyClick()
        {
            PlatGeneric.CopyText(this.data.Key);
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadImage()
        {
            //进行Http访问
            UnityWebRequest request = UnityWebRequest.Get(this.data.Icon);
            request.downloadHandler = new DownloadHandlerTexture(true);
            request.timeout = 60;
            yield return request.SendWebRequest();

            //判断是否出现错误
            if (request.result != UnityWebRequest.Result.Success)
            {
                //打印错误信息
                Debug.Log("Server Data Load " + request.error);
            }
            else
            {
                //判断是否返回200
                if (request.responseCode == 200)
                {
                    //回调处理
                    Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    Sprite image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    this.IconImage.sprite = image;
                    this.data.Image = image;
                }
                else
                {
                    //打印错误信息
                    Debug.Log("Server Data Load " + request.responseCode);
                }
            }
        }
    }
}