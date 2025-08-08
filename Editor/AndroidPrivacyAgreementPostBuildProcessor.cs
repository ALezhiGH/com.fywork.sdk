#if UNITY_ANDROID
using System.Collections;
using UnityEditor.Android;
using UnityEngine;
using System.IO;
using SDK;

/// <summary>
/// 处理安卓的隐私协议
/// </summary>
class AndroidPrivacyAgreementPostBuildProcessor : IPostGenerateGradleAndroidProject
{
    private string privacyAppName = Application.productName;
    private string privacyEmail = "2082502204@qq.com";

    public int callbackOrder { get { return 999; } }
    public void OnPostGenerateGradleAndroidProject(string path)
    {
        Debug.Log("AndroidPostBuildProcessor.OnPostGenerateGradleAndroidProject at path " + path);

        string unityPlayerJavaFilePath = path + "/src/main/java/com/unity3d/player/UnityPlayerActivity.java";
        string content = File.ReadAllText(unityPlayerJavaFilePath);

		//读取后台配置表的链接地址
        var asset = Resources.Load<TextAsset>("chnl");
        Hashtable chnl_args = JsonTool.jsonDecode(asset.text) as Hashtable;
        //"agrt":{"hdlr":"FywAgrtHandler", "serv":"https://www.fanyu.work/home/service.html",
		//		  "cnty":0, "save":"Agreement", "priv":"https://www.fanyu.work/home/privacy.html"},
        var args = chnl_args["agrt"] as Hashtable;
        var PrivUrl = args["priv"].ToString();//"priv":"https://www.fanyu.work/home/privacy.html"
        var ServUrl = args["serv"].ToString();//"serv":"https://www.fanyu.work/home/service.html"
        //替换地址
        var privacyContent = privacyString.Replace("{PrivUrl}", PrivUrl).Replace("{ServUrl}", ServUrl) + "\n";
        
        //1.先刪除重复的
        if (content.Contains(privacyImport))
        {
            content = content.Replace(privacyImport, "");
        }
        if (content.Contains(privacyContent))
        {
            content = content.Replace(privacyContent, "");
        }
        
		//2.再更新内容
        content = content.Replace("import android.os.Process;", "import android.os.Process;" + privacyImport);
        content = content.Replace("mUnityPlayer = new UnityPlayer(this, this);", privacyContent + "mUnityPlayer = new UnityPlayer(this, this);");

        File.WriteAllText(unityPlayerJavaFilePath, content);
    }

    private string privacyImport = @"
//启动前弹出隐私协议弹窗 新增库
import android.app.AlertDialog;//新增
import android.content.DialogInterface;//新增
import android.content.SharedPreferences;//新增
import android.text.Html;//新增
import android.text.Spanned;//新增
import android.text.SpannableStringBuilder;//新增
import android.text.SpannableString;//新增
import android.text.style.ForegroundColorSpan;//新增
import android.text.style.URLSpan;//新增
import android.graphics.Color;//新增
import android.widget.TextView;//新增
import android.text.method.LinkMovementMethod;//新增";

    private string privacyString = @"
        // 先展示隐私政策
        SharedPreferences base = getSharedPreferences(""base"", MODE_PRIVATE);
        Boolean privacyFlag = base.getBoolean(""PrivacyFlag"", true);

        if (privacyFlag == true) {
            // 隐私协议内容
            String message = ""欢迎下载本游戏！\n"" +
                    ""1.本游戏客户端是由泛鱼工作室运营的游戏类应用；保护用户隐私是本游戏的一项基本政策；我们不会泄露您的个人信息；\n"" +
                    ""2.为了提供更好的游戏体验，我们会根据您使用的具体功能需要，收集必要的用户信息（如申请设备信息、存储等相关权限）；\n"" +
                    ""3.您可以阅读完整版《游戏隐私保护协议》、《游戏用户服务协议》了解我们申请使用相关权限的情况，以及对您的个人隐私保护政策。"";
            
            String splitText = ""《游戏隐私保护协议》、《游戏用户服务协议》"";
            String[] messages = message.split(splitText);
            SpannableStringBuilder stringBuilder = new SpannableStringBuilder();
            //前部分文本
            stringBuilder.append(messages[0]);
            //《游戏隐私保护协议》链接
            String privacyText = ""《游戏隐私保护协议》"";
            String url_privacy = ""{PrivUrl}"";
            SpannableString privacySpan = new SpannableString(privacyText);
            privacySpan.setSpan(new ForegroundColorSpan(Color.GREEN),0,privacyText.length(), Spanned.SPAN_EXCLUSIVE_EXCLUSIVE);
            privacySpan.setSpan(new URLSpan(url_privacy),0,privacyText.length(), Spanned.SPAN_EXCLUSIVE_EXCLUSIVE);
            stringBuilder.append(privacySpan);
            //顿号
            stringBuilder.append(""、"");
            //《游戏用户服务协议》链接
            String userText = ""《游戏用户服务协议》"";
            String url_user = ""{ServUrl}"";
            SpannableString userSpan = new SpannableString(userText);
            userSpan.setSpan(new ForegroundColorSpan(Color.GREEN),0,userText.length(), Spanned.SPAN_EXCLUSIVE_EXCLUSIVE);
            userSpan.setSpan(new URLSpan(url_user),0,userText.length(), Spanned.SPAN_EXCLUSIVE_EXCLUSIVE);
            stringBuilder.append(userSpan);
            //后部分文本
            stringBuilder.append(messages[1]);

            AlertDialog.Builder builder = new AlertDialog.Builder(UnityPlayerActivity.this);
            builder.setTitle(""个人信息保护政策"");  //设置标题
            builder.setMessage(stringBuilder);
            builder.setCancelable(false);  //是否可以取消
            builder.setNegativeButton(""拒绝"", new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialogInterface, int i) {
                    dialogInterface.dismiss();
                    android.os.Process.killProcess(android.os.Process.myPid());
                }
            });

            builder.setPositiveButton(""同意"", new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialog, int which) {
                    SharedPreferences.Editor editor = base.edit();
                    editor.putBoolean(""PrivacyFlag"", false);
                    editor.commit();
                }
            });
            //builder.show();// builder.show().getWindow().setLayout(1000, 1300);

            AlertDialog alert = builder.create();
            alert.show();
            //设置弹窗文本可点击
            TextView msgTxt = (TextView) alert.findViewById(android.R.id.message);
            msgTxt.setMovementMethod(LinkMovementMethod.getInstance());
        }
	";
}
#endif
