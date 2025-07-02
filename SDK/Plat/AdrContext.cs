using UnityEngine;

namespace SDK
{
    /// <summary>
    /// 安卓上下文
    /// </summary>
    public class AdrContext
    {
        /// <summary>
        /// 上下文实例
        /// </summary>
        private static AdrContext instance;

        /// <summary>
        /// 安卓主入口
        /// </summary>
        private AndroidJavaObject activity;

        /// <summary>
        /// 上下文实例
        /// </summary>
        public static AdrContext Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = new AdrContext();
                }
                return instance;
            }
        }

        /// <summary>
        /// 安卓主入口
        /// </summary>
        public AndroidJavaObject Activity
        {
            get
            {
                if (null == this.activity)
                {
                    AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    this.activity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
                }
                return this.activity;
            }
        }

        /// <summary>
        /// 运行在主UI线程
        /// </summary>
        /// <param name="runnable"></param>
        public void RunOnUIThread(AndroidJavaRunnable runnable)
        {
            this.Activity.Call("runOnUiThread", runnable);
        }
    }
}