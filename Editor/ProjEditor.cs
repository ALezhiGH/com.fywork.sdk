using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

namespace SDK.Editor
{
    /// <summary>
    /// 项目编辑器
    /// </summary>
    public class ProjEditor : EditorWindow
    {
        /// <summary>
        /// 渠道数据
        /// </summary>
        private class ChnlData
        {
            /// <summary>
            /// ID
            /// </summary>
            public int ID { get; protected set; }

            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; protected set; }

            /// <summary>
            /// 简称
            /// </summary>
            public string Abbr { get; protected set; }

            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="id"></param>
            /// <param name="name"></param>
            /// <param name="abbr"></param>
            public ChnlData(int id, string name, string abbr)
            {
                this.ID = id;
                this.Name = name;
                this.Abbr = abbr;
            }

            /// <summary>
            /// 解析数据
            /// </summary>
            /// <param name="table"></param>
            /// <returns></returns>
            public static ChnlData Parse(Hashtable table)
            {
                try
                {
                    if (null == table)
                    {
                        return null;
                    }
                    if (!table.ContainsKey("id"))
                    {
                        return null;
                    }
                    if (!table.ContainsKey("name"))
                    {
                        return null;
                    }
                    if (!table.ContainsKey("abbr"))
                    {
                        return null;
                    }
                    int id = int.Parse(table["id"].ToString());
                    string name = table["name"].ToString();
                    string abbr = table["abbr"].ToString();
                    return new ChnlData(id, name, abbr);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.StackTrace);
                    return null;
                }
            }
        }

        /// <summary>
        /// 版本信息
        /// </summary>
        private string version;

        /// <summary>
        /// 宏定义
        /// </summary>
        private string symbols;

        /// <summary>
        /// 渠道信息
        /// </summary>
        private Dictionary<int, ChnlData> chnls;

        /// <summary>
        /// 生成配置
        /// </summary>
        [MenuItem("泛鱼工具/生成配置", false, 12)]
        public static void BuildConfig()
        {
            Debug.Log("生成配置");
            string path = Application.dataPath;
            path = path.Replace("Assets", "") + "Export/ExeclExport.exe";
            ProcCmd(path, null);
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="arg"></param>
        private static void ProcCmd(string cmd, string arg)
        {
            ProcessStartInfo start = new ProcessStartInfo(cmd);
            start.Arguments = arg;
            start.CreateNoWindow = false;
            start.ErrorDialog = true;
            start.UseShellExecute = true;
            if (start.UseShellExecute)
            {
                start.RedirectStandardOutput = false;
                start.RedirectStandardError = false;
                start.RedirectStandardInput = false;
            }
            else
            {
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                start.RedirectStandardInput = true;
                start.StandardOutputEncoding = UTF8Encoding.UTF8;
                start.StandardErrorEncoding = UTF8Encoding.UTF8;
            }
            Process p = Process.Start(start);
            if (!start.UseShellExecute)
            {
                Debug.Log(p.StandardOutput);
                Debug.Log(p.StandardError);
            }
            p.WaitForExit();
            p.Close();
        }

        [MenuItem("泛鱼工具/清空PlayerPrefs")]
        private static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("已清空PlayerPrefs");
        }

        /// <summary>
        /// 一键打包
        /// </summary>
        [MenuItem("泛鱼工具/一键打包", false, 100)]
        public static void OneKeyPack()
        {
            EditorWindow.GetWindow(typeof(ProjEditor)).Show();
        }

        /// <summary>
        /// 可用处理
        /// </summary>
        private void OnEnable()
        {
            this.version = Application.version;
            this.chnls = new Dictionary<int, ChnlData>();
            string data = SyncGet($"https://www.fanyu.work/fywork/serv/chnl_pack.php?plat_id={this.GetPlat()}&game_code={Application.identifier}");
            Debug.Log($"<color=red>渠道列表返回:{data}</color>");
            if (data == string.Empty)
            {
                return;
            }
            Hashtable table = JsonTool.jsonDecode(data) as Hashtable;
            if (null == table)
            {
                return;
            }
            ArrayList list = table["data"] as ArrayList;
            if (null == list)
            {
                return;
            }
            ChnlData chnl;
            foreach (var item in list)
            {
                table = item as Hashtable;
                chnl = ChnlData.Parse(table);
                if (null != chnl)
                {
                    this.chnls[chnl.ID] = chnl;
                }
            }
            //宏定义相关处理
            var target = EditorUserBuildSettings.selectedBuildTargetGroup;
            var symbols = new HashSet<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(target).Split(";"));
            foreach (var item in this.chnls.Values)
            {
                symbols.Remove(item.Abbr);
            }
            this.symbols = string.Empty;
            foreach (var item in symbols)
            {
                this.symbols += item + ";";
            }
            Debug.Log("游戏渠道已加载");
        }

        /// <summary>
        /// 显示处理
        /// </summary>
        private void OnGUI()
        {
            GUILayout.Label("当前版本:");
            this.version = EditorGUILayout.TextField(this.version);
            foreach (var item in this.chnls.Values)
            {
                if (GUILayout.Button(item.Name))
                {
                    Debug.Log("开始打包:" + item.Name);
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, this.symbols + item.Abbr);//设置渠道宏定义
                    this.ChnlPack(item.ID, item.Name);
                }
            }
            
            GUILayout.Label(string.Empty);
            GUILayout.Label("更新配置:");
            foreach (var item in this.chnls.Values)
            {
                if (GUILayout.Button(item.Name))
                {
                    Debug.Log("更新配置:" + item.Name);
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, this.symbols + item.Abbr);//设置渠道宏定义
                    UpdateChnlConfig(item.ID, item.Name);
                }
            }
        }

        /// <summary>
        /// 获取平台ID
        /// </summary>
        /// <returns></returns>
        private int GetPlat()
        {
#if UNITY_ANDROID
            return 1;
#elif UNITY_IOS
            return 2;
#elif UNITY_STANDALONE
            return 5;
#endif
        }

        /// <summary>
        /// 更新渠道配置
        /// </summary>
        /// <param name="chnl"></param>
        /// <param name="name"></param>
        private void UpdateChnlConfig(int chnl, string name)
        {
            string data = SyncGet($"https://www.fanyu.work/fywork/serv/game_pack.php?chnl_id={chnl}&game_code={Application.identifier}");
            Debug.Log($"<color=red>{name} 渠道配置返回:{data}</color>");
            if (data == string.Empty)
            {
                Debug.Log("渠道配置错误");
                return;
            }
            Hashtable table = JsonTool.jsonDecode(data) as Hashtable;
            if (null == table)
            {
                Debug.Log("渠道配置错误");
                return;
            }
            table = table["data"] as Hashtable;
            if (null == table)
            {
                Debug.Log("渠道配置错误");
                return;
            }
            // 
            SaveFile(Path.Combine(Application.dataPath, "Resources"), "chnl.txt", JsonTool.jsonEncode(table));
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 指定渠道打包
        /// </summary>
        /// <param name="chnl">渠道Id</param>
        /// <param name="name">渠道名</param>
        private void ChnlPack(int chnl, string name)
        {
            UpdateChnlConfig(chnl, name);
            PlayerSettings.bundleVersion = this.version;
#if UNITY_ANDROID
            //设置签名
            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = Application.dataPath.Replace("/Assets", "/fywork.keystore");
            PlayerSettings.Android.keystorePass = "Fy880618";
            PlayerSettings.Android.keyaliasName = "fywork";
            PlayerSettings.Android.keyaliasPass = "Fy880618";
            
            //输入路径
            var directoryPath = $"{Directory.GetParent(Application.dataPath)}/Builds/";
            var report = BuildPipeline.BuildPlayer(this.GetScenes(), directoryPath + $"{Application.productName}_{name}_{PlayerSettings.bundleVersion}_{System.DateTime.Now:yyyyMMdd_HHmmss}.apk", BuildTarget.Android, BuildOptions.None);
            var summary = report.summary;
            if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log($"Build succeeded: {summary.totalSize} bytes   outputPath: {summary.outputPath}");
            
                //打开APK目录
                Process.Start("Explorer.exe", directoryPath.Replace('/', '\\'));
            }
            else if (summary.result == UnityEditor.Build.Reporting.BuildResult.Failed)
            {
                Debug.Log("Build failed");
            }
#elif UNITY_IOS
            BuildPipeline.BuildPlayer(this.GetScenes(), Application.dataPath + "/../Builds/"+ name + "/" + Application.productName + ".apk", BuildTarget.iOS, BuildOptions.None);
#elif UNITY_STANDALONE
            BuildPipeline.BuildPlayer(this.GetScenes(), Application.dataPath + "/../Builds/"+ name + "/" + Application.productName + ".exe", BuildTarget.StandaloneWindows, BuildOptions.None);
#endif
        }

        /// <summary>
        /// 获取所有场景路径
        /// </summary>
        /// <returns></returns>
        private string[] GetScenes()
        {
            var datas = new List<string>();
            for (int index = 0; index < EditorBuildSettings.scenes.Length; index++)
            {
                if (EditorBuildSettings.scenes[index].enabled)
                {
                    datas.Add(EditorBuildSettings.scenes[index].path);
                }
            }
            return datas.ToArray();
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
        /// <param name="path">文件夹路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="data">数据</param>
        private static void SaveFile(string path, string fileName, string data)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                
                StreamWriter streamWriter = File.CreateText(Path.Combine(path, fileName));
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