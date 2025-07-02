using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

namespace Fyw
{
    /// <summary>
    /// 屏蔽字编辑器
    /// </summary>
    public class PbzEditor : EditorWindow
    {
        /// <summary>
        /// 添加内容
        /// </summary>
        private string content;

        /// <summary>
        /// 滑动区域
        /// </summary>
        private Vector2 scroll;

        /// <summary>
        /// 更新字库
        /// </summary>
        [MenuItem("泛鱼工具/屏蔽字库/更新", false, 100)]
        public static void UpdateWord()
        {
            Debug.Log("更新屏蔽字库");
            string data = SyncGet("https://www.onebiji.com/qudao/kuaibao/words.json");
            SaveFile(Application.dataPath.Replace("/Assets", "") + "/Packages/com.fywork.sdk/SDK/Pbz/Fyw/Resources/words.txt", data);
            Debug.Log("更新操作完成");
        }

        /// <summary>
        /// 添加字库
        /// </summary>
        [MenuItem("泛鱼工具/屏蔽字库/添加", false, 100)]
        public static void AppendWord()
        {
            EditorWindow.GetWindow(typeof(PbzEditor)).Show();
        }

        /// <summary>
        /// 可用处理
        /// </summary>
        private void OnEnable()
        {
            Debug.Log("字库编辑已打开");
            TextAsset asset = Resources.Load<TextAsset>("extra");
            if (null != asset)
            {
                this.content = asset.text;
            }
            else
            {
                this.content = "";
            }
        }

        /// <summary>
        /// 禁用处理
        /// </summary>
        private void OnDisable()
        {
            SaveFile(Application.dataPath.Replace("/Assets", "") + "/Packages/com.fywork.sdk/SDK/Pbz/Fyw/Resources/extra.txt", this.content);
            Debug.Log("添加字库已保存");
        }

        /// <summary>
        /// 显示处理
        /// </summary>
        private void OnGUI()
        {
            this.scroll = EditorGUILayout.BeginScrollView(this.scroll);
            this.content = EditorGUILayout.TextArea(this.content, GUILayout.Height(this.position.height - 5));
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Http同步Get
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string SyncGet(string url)
        {
            //进行Http访问
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.timeout = 60;
            request.SendWebRequest();

            //等待完成
            while (!request.isDone)
            {

            }

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
                    //返回数据
                    return request.downloadHandler.text;
                }
                else
                {
                    //打印错误信息
                    Debug.Log("Server Data Load " + request.responseCode);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        private static void SaveFile(string path, string data)
        {
            try
            {
                StreamWriter streamWriter = File.CreateText(path);
                streamWriter.Write(data);
                streamWriter.Flush();
                streamWriter.Close();
                streamWriter.Dispose();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.StackTrace);
            }
        }
    }
}