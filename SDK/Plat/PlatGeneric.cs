using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SDK
{
    /// <summary>
    /// 平台通用
    /// </summary>
    public class PlatGeneric
    {
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void _copyTextToClipboard(string text);
        [DllImport("__Internal")]
        private static extern void GetIDFA();
	    [DllImport ("__Internal")]
	    private static extern double GetStartUpTime ();
        [DllImport("__Internal")]
        private static extern bool iOSJoinQQGroup(string key, string uid);
#endif

        /// <summary>
        /// 申请权限
        /// </summary>
        /// <param name="perm"></param>
        public static void CallPerm(string perm)
        {
#if UNITY_IOS
            GetIDFA();
#endif
        }

        /// <summary>
        /// 复制文本
        /// </summary>
        /// <param name="text"></param>
        public static void CopyText(string text)
        {
#if UNITY_EDITOR
            TextEditor te = new TextEditor();
            te.text = text;
            te.SelectAll();
            te.Copy();
#elif UNITY_ANDROID
            AndroidJavaClass Agent = new AndroidJavaClass("com.pj.yfgame.MainActivity");
            Agent.CallStatic("copyTextToClipboard", text, AdrContext.Instance.Activity);
#elif UNITY_IOS
            _copyTextToClipboard(text);
#endif
        }

        /// <summary>
        /// 开机时间
        /// </summary>
        /// <returns></returns>
        public static TimeSpan OpenTime()
        {
#if UNITY_ANDROID
            AndroidJavaClass jc = new AndroidJavaClass("com.pj.yfgame.MainActivity");
            long nValue = jc.CallStatic<long>("GetStartUpTime");
            return new TimeSpan(nValue);
#elif UNITY_IOS
            double nValue = GetStartUpTime();
            return new TimeSpan((long)nValue);
#elif UNITY_EDITOR
            return new TimeSpan(Environment.TickCount);
#endif
        }

        /// <summary>
        /// 加入QQ
        /// </summary>
        /// <param name="key"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static bool JoinQQ(string key, string uid)
        {
#if UNITY_ANDROID
            AndroidJavaClass jc = new AndroidJavaClass("com.pj.yfgame.MainActivity");
            return jc.CallStatic<bool>("joinQQGroup", key, AdrContext.Instance.Activity);
#elif UNITY_IOS
            return iOSJoinQQGroup(key, uid);
#elif UNITY_EDITOR
            return false;
#endif
        }

        /// <summary>
        /// 跳转应用
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool JumpApp(string path)
        {
            return false;
        }

        /// <summary>
        /// 检查App是否存在 1表示存在 -1表示不存在 0表示未知
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static int CheckApp(string code)
        {
            int flag = 0;
#if UNITY_IOS && !UNITY_EDITOR

#elif UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                if (null == up)
                {
                    return flag;
                }
                AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
                if (null == ca)
                {
                    return flag;
                }
                AndroidJavaObject mgr = ca.Call<AndroidJavaObject>("getPackageManager");
                if (null == mgr)
                {
                    return flag;
                }
                try
                {
                    AndroidJavaObject info = mgr.Call<AndroidJavaObject>("getPackageInfo", code, 0);//找不到会报错
                    if (null != info)
                    {
                        flag = 1;
                    }
                    else
                    {
                        flag = -1;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    flag = -1;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
#endif
            return flag;
        }

        /// <summary>
        /// 退出应用
        /// </summary>
        public static void ExitApp()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}