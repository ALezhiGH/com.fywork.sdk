# 编辑器报错处理
## 删除otherSDK文件,用本地package代替
    com.fywork.csj@1.0.0
    com.fywork.hykb@1.4.2
    com.fywork.sdk@1.0.0
    com.fywork.umeng@1.0.0

## Package下的Manifest.Json引入Tap sdk
    "dependencies": {
        "com.google.external-dependency-manager": "1.2.179",
        "com.taptap.sdk.compliance": "4.6.3",
        "com.taptap.sdk.core": "4.6.3",
        "com.taptap.sdk.login": "4.6.3"
    },
    "scopedRegistries": [
        {
            "name": "package.openupm.com",
            "url": "https://package.openupm.com",
            "scopes": [
            "com.google.external-dependency-manager"
            ]
        },
        {
            "name": "NPMJS",
            "url": "https://registry.npmjs.org/",
            "scopes": [
            "com.taptap"
            ]
        }
    ]

## 直接删掉,不再使用的脚本或方法
    不再需要的脚本:
    VersonControl.cs, 同时要删除界面上的相关按钮、面板等,慎重!

    已集成到SDK里的流程: 渠道初始化、统计初始化、协议初始化
    CallServer.Instance.AsyncLoad
    WordManager.Instance.AsyncLoad

## 添加新命名空间 using SDK;
    引用不到的类可以自动添加命名空间SDK即可.
    比如ChnlControl、AdvertCode、

## CallCmnt() 改为 CallCmt()
    ChnlControl.Instance.CallCmnt();
    改为:
    ChnlControl.Instance.CallCmt();

## ServerSDK.CallServer.cs 用 SDK.ChnlControl.cs 替换
    Application.OpenURL(CallServer.Instance.UpdUrl) 改为 ChnlControl.Instance.CallUpd()
    CallServer.Instance.CanPay 改为 ChnlControl.Instance.CanPay
    CallServer.Instance.RankUrl 改为 ChnlControl.Instance.RankUrl
    CallServer.Instance.SaveUrl 改为 ChnlControl.Instance.SaveUrl
    CallServer.Instance.ShowGameList 改为 ChnlControl.Instance.CallRec (推荐服务就是这个功能，只不过自带了统一界面，游戏中就不需要界面了，直接CallRec就行了)
    CallServer.Instance.ShowVersion 暂无替换,可以移除

## SdkControl.cs 用 ChnlControl.cs 替代, 修改URL
    SdkControl.Instance.PrivacyUrl 改为 ChnlControl.Instance.PrivUrl
    SdkControl.Instance.ServiceUrl 改为 ChnlControl.Instance.ServUrl

## CopyControy.cs 用 PlatGeneric.cs 替代    
    CopyControy.GetPowe() 改为 PlatGeneric.CallPerm(null)//调用ios权限
    CopyControy.OnClickCopyText 改为 PlatGeneric.CopyText
    CallContact.JoinQQGroup("") 改为 PlatGeneric.JoinQQ("", "")

## 论坛按钮处理
    Application.OpenURL(InfoControl.Instance.FrmUrl) 改为 SDK.ChnlControl.Instance.CallFrm()

# SDK 初始化
## LoadControl.cs 增加 SDK 初始化工作
    PlatGeneric.CallPerm(null);//获取权限
    SdkManager.Instance.AddIgnoredFlowServer("agrt");
    SdkManager.Instance.Init();
    SdkManager.Instance.Load(() =>
    {
        SceneManager.LoadSceneAsync("main");
    });

## main 场景的 GameControl.cs 增加 SDK 工作流程
* #1 Awake 可以增加 SDK 初始化工作, 用于编辑器下从main场景启动游戏
    //直接从main场景开始游戏,则需要初始化SDK
    if (!SdkManager.Instance.IsInited)
    {
        PlatGeneric.CallPerm(null);//获取权限
        SdkManager.Instance.AddIgnoredFlowServer("agrt");
        SdkManager.Instance.Init();
        SdkManager.Instance.Load(() => SdkManager.Instance.Work());
    }
* #2 调整 ChnlID 为运行时读取配置
    //设置ChnlID
    if (SdkManager.Instance.IsInited)
    {
        GameConfig.ChnlID = ChnlControl.Instance.Chnl;
    }
    else
    {
        Debug.LogError("SDK 还没初始化");
    }

* #3 在 Start 中增加 SdkManager.Instance.Work() 方法
    //如果load完成(在load场景中已调用),则直接执行work
    if (SdkManager.Instance.LoadCompleted)
    {
        SdkManager.Instance.Work();
    }

## main 场景的 GameControl.cs 也可以增加 跳转 load 场景代码, 但要注意其他脚本在 Awake 里的回调是否会报空
    if (null == GameObject.Find("Loader"))
    {
        SceneManager.LoadSceneAsync("load");
        return;
    }

## supplierconfig.json 文件移到Assets\Resources 文件夹下
    同时删除Plugins\Android文件夹下的 assets、libs、res等文件夹

## PlayerSetting 里的 Allow downloads over HTTP 改为 Always allowed

# 打包错误处理
## Target API Level 选择 32

## Publishing Settings 修改
    去掉Custom Main Manifest、Custom Launcher Manifest、Custom Launcher Gradle Template;
    剩下的全部用Framework的模板替换.

    否则会报错:    
A problem occurred configuring project ':launcher'.
> Could not resolve all dependencies for configuration ':launcher:classpath'.
   > Using insecure protocols with repositories, without explicit opt-in, is unsupported. Switch Maven repository 'maven(http://maven.aliyun.com/nexus/content/groups/public)' to redirect to a secure protocol (like HTTPS) or allow insecure protocols. See https://docs.gradle.org/7.5.1/dsl/org.gradle.api.artifacts.repositories.UrlArtifactRepository.html#org.gradle.api.artifacts.repositories.UrlArtifactRepository:allowInsecureProtocol for more details. 

## 因为 AndroidPrivacyAgreementPostBuildProcessor.cs 里面没有删除重复的内容而报错, 直接使用SDK里的即可.
* What went wrong:
Execution failed for task ':unityLibrary:compileReleaseJavaWithJavac'.
> Compilation failed; see the compiler error output for details.

* What went wrong:
java.lang.StackOverflowError (no error message)

## 重启 Unity
## 删掉 Library 再重启 Unity