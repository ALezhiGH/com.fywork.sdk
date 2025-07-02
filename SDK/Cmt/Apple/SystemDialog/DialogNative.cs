using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class DialogNative
{
#if UNITY_EDITOR

#elif UNITY_ANDROID
    private static AndroidJavaClass mJc;
    private static AndroidJavaObject mJo;

#elif UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern void _TAG_ShowRateUsPopUp(string title, string message, string rate, string remind, string declined);

    [DllImport("__Internal")]
    private static extern void _TAG_ShowDialog(string title, string message, string yes, string no);

    [DllImport("__Internal")]
    private static extern void _TAG_ShowMessage(string title, string message, string ok);

    [DllImport("__Internal")]
    private static extern void _TAG_RedirectToAppStoreRatingPage(string appId);

    [DllImport("__Internal")]
    private static extern void _TAG_RedirectToWebPage(string urlString);

    [DllImport("__Internal")]
    private static extern void _TAG_DismissCurrentAlert();
#endif

    /// <summary>
    /// 获取mjo
    /// </summary>
    /// <returns></returns>
    public static AndroidJavaObject GetMjo()
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID
        if (mJo == null)
        {
            mJc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            mJo = mJc.GetStatic<AndroidJavaObject>("currentActivity");
        }

        return mJo;
#elif UNITY_IPHONE

#endif

        return null;
    }

    /// <summary>
    /// 显示评论界面
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="rate"></param>
    /// <param name="remind"></param>
    /// <param name="declined"></param>
    public static void ShowRateUsPopUP(string title, string message, string rate, string remind, string declined)
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID

#elif UNITY_IPHONE
        _TAG_ShowRateUsPopUp(title, message, rate, remind, declined);
#endif
    }

    /// <summary>
    /// 显示对话框
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="yes"></param>
    /// <param name="no"></param>
    public static void ShowDialog(string message, string leftButton, string rightButton)
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID
        GetMjo().Call("ShowDialog", message, leftButton, rightButton);
#elif UNITY_IPHONE
        _TAG_ShowDialog("提 示", message, leftButton, rightButton);
#endif
    }
}