using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogController : MonoBehaviour 
{
    /// <summary>
    /// 当前窗口
    /// </summary>
    private static DialogController currentDialogController;

    /// <summary>
    /// 对话框结束回调
    /// </summary>
    /// <param name="p"></param>
    public delegate void OnPopupComplete(int p);

    /// <summary>
    /// 应用id
    /// </summary>
    private string appleId;

    /// <summary>
    /// 回调
    /// </summary>
    public OnPopupComplete onPopupComplete;

    /// <summary>
    /// 弹出提示框
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static void ShowPingLun(string appId, OnPopupComplete complete)
    {
        string title = "提  示";
        string message = "程序员哥哥已经加班到即将吐血身亡，你不来个好评鼓励一下？";

        if (currentDialogController != null) Destroy(currentDialogController.gameObject);
        currentDialogController = new GameObject("IOSRateUsPopUp").AddComponent<DialogController>();
        currentDialogController.appleId = appId; //记录appid
        currentDialogController.onPopupComplete = complete; //回调

        DialogNative.ShowRateUsPopUP(title, message, "五星好评", "稍后评价", "让他去死");
    }

    /// <summary>
    /// 显示对话框
    /// </summary>
    public static void ShowDialog(string message, string leftButton, string rightButton, OnPopupComplete complete)
    {
        Debug.LogError("~~~~~~~~~~~~BBB");
        if (currentDialogController != null) Destroy(currentDialogController.gameObject);
        currentDialogController = new GameObject("IOSDialogPopUp").AddComponent<DialogController>();
        currentDialogController.onPopupComplete = complete; //回调

        DialogNative.ShowDialog(message, leftButton, rightButton);
    }

	// Use this for initialization
	void Start ()
    {
      
	}
	
	// Update is called once per frame
	void Update () 
    {
		
	}

    #region 选择处理
    /// <summary>
    /// 评论回调
    /// </summary>
    /// <param name="buttonIndex"></param>
    public void OnRatePopUpCallBack(string buttonIndex)
    {
        Debug.LogError("~~~~~~~~~~~~ccc:" + buttonIndex);
        int index = System.Convert.ToInt16(buttonIndex);

        switch (index)
        {
            case 0:  //五星好评
                Application.OpenURL("https://itunes.apple.com/cn/app/id" + appleId + "?action=write-review");
                RaiseOnOnPopupComplete(0);
                break;
            case 1: //稍后评价

                break;
            case 2: //吐槽
                Application.OpenURL("https://itunes.apple.com/cn/app/id" + appleId + "?action=write-review");
                RaiseOnOnPopupComplete(0);
                break;

            case 3: //直接调用的评分界面
                RaiseOnOnPopupComplete(0);
                break;
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// 对话框回调
    /// </summary>
    /// <param name="buttonIndex"></param>
    public void OnDialogPopUpCallBack(string buttonIndex)
    {
        Debug.Log("buttonIndex=" + buttonIndex);
        Debug.LogError("~~~~~~~~~~~~DDD:" + buttonIndex);
        int index = System.Convert.ToInt16(buttonIndex);

        switch (index)
        {
            case 0: //取消

                break;

            case 1:  //确认
                RaiseOnOnPopupComplete(1);
                break;
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// 处理对话框结束
    /// </summary>
    /// <param name="p"></param>
    private void RaiseOnOnPopupComplete(int p)
    {
        Debug.LogError("~~~~~~~~~~~~E:" + p);
        if (onPopupComplete != null)
        {
            onPopupComplete(p);
        }
    }
    #endregion
}
