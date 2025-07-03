using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Fyw
{
    /// <summary>
    /// 推荐页
    /// </summary>
    public class RecPage : MonoBehaviour
    {
        /// <summary>
        /// 名称文本
        /// </summary>
        public Text NameText;

        /// <summary>
        /// 描述1文本
        /// </summary>
        public Text DescText;

        /// <summary>
        /// 图标
        /// </summary>
        public Image IconImage;

        /// <summary>
        /// 数据
        /// </summary>
        private RecData data;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        public void Init(RecData data)
        {
            this.data = data;
            this.NameText.text = data.Name;
            this.DescText.text = data.Desc;
            if (null == data.Image)
            {
                this.StartCoroutine(this.LoadImage());
            }
            else
            {
                this.IconImage.sprite = data.Image;
            }
        }

        /// <summary>
        /// 下载处理
        /// </summary>
        public void DownClick()
        {
            Application.OpenURL(this.data.Url);
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
                Debug.LogError("Server Data Load Fail: " + request.error);
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
                    Debug.LogError("Server Data Load Error: " + request.responseCode);
                }
            }
        }
    }
}