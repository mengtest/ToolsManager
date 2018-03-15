using NIM;
using NIM.DataSync;
using NimUtility;
using System.IO;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class DWChatLogin
{
    public static string _nimAppKey = "3364b7a1dac9ee0a82415e937dfd4499";
    public static string _nimAppSecret = "87196bd8b591";
    public static bool _loginSuccess = false;
    private string _appDataPath = "";
    private const string ConfigFileName = "countinfo.json";
    private AccountInfo _accountInfo = new AccountInfo();
    public static bool _sdkInit = false;

    private volatile static DWChatLogin _instance = null;
    private static readonly object lockHelper = new object();
    private DWChatLogin() { }
    public static DWChatLogin getInstance()
    {
        if (_instance == null)
        {
            lock (lockHelper)
            {
                if (_instance == null)
                    _instance = new DWChatLogin();
            }
        }
        return _instance;
    }

    public class AccountInfo : NimJsonObject<AccountInfo>
    {
        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }

    private string AppDataPath
    {
        get
        {
            if (string.IsNullOrEmpty(_appDataPath))
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    _appDataPath = Application.persistentDataPath + "/jxqp";
                }
                else if (Application.platform == RuntimePlatform.Android)
                {
                    string androidPathName = "com.dawang.jxqp";
                    if (Directory.Exists("/sdcard"))
                        _appDataPath = Path.Combine("/sdcard", androidPathName);
                    else
                        _appDataPath = Path.Combine(Application.persistentDataPath, androidPathName);
                }
                else if (Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    _appDataPath = Application.persistentDataPath;
                }
                DWChatUtils.DebugLog("AppDataPath:" + _appDataPath);
            }
            return _appDataPath;
        }
    }

    private string _accountRecordPath
    {
        get
        {
            string path = "";
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
                path = "";
            else
                path = AppDataPath;
            path = Path.Combine(path, "chataccount.json");

            return path;
        }
    }

    public void DW_setAccountInfo(string count, string token)
    {
        _accountInfo.Account = count;
        _accountInfo.Password = token;
    }

    //清理语音资源 放在初始化之前
    public void ClearVoice() 
    {
        //DeleteFolder(Application.persistentDataPath + "/audio");
        DeleteFolderFile(Application.persistentDataPath + "/audio");
        DeleteFolder(Application.persistentDataPath + "/NIM");
    }

    public void DeleteFolderFile(string path)
    {
        var dir = new DirectoryInfo(path);
        if (dir.Exists)
        {
            var files = dir.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (!file.FullName.Contains(".bytes"))
                {
                    Util.DeleteFile(file.FullName);
                }
            }
        }
    }

    private void DeleteFolder(string path)
    {
        var dir = new DirectoryInfo(path);
        if (dir.Exists)
        {
            try
            {
                dir.Delete(true);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    public bool initSDK(string nimAppKey, string nimAppSecret)
    {
        if (_sdkInit)
        {
            return true;
        }
        //_sdkInit = true;

        ClearVoice();

        NimConfig config = new NimConfig();
        _nimAppKey = nimAppKey;
        _nimAppSecret = nimAppSecret;
        config.AppKey = _nimAppKey;
        var ret = false;
        if (!ClientAPI.SdkInitialized)
           ret  = ClientAPI.Init(Application.persistentDataPath, "", config);

        DWChatUtils.DebugLog(ret ? "sdk 初始化成功" : "sdk 初始化失败");
        bool audioinit = NIMAudio.AudioAPI.InitModule(Application.persistentDataPath);
        if (!audioinit)
        {
            DWChatUtils.DebugLog("init audio module failed===========");
        }
        _sdkInit = ret && audioinit;
  //      UnityEngine.Debug.Log("initSDK!");    //要现在ui主线程注册一下
        return ret&&audioinit;
    }

    public void DW_login()
    {
        DW_login(_nimAppKey, _nimAppSecret);
    }

    public void DW_login(string nimAppKey, string nimAppSecret)
    {
        if (!Constants.EnableIM)
            return;

        DWLoom.Initialize();
        int lstate = (int)ClientAPI.GetLoginState();
        Debug.LogFormat("login state ======== {0}", lstate);
        //GlobalAPI.SetSdkLogCallback(DumpLog);

        //if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor
        //    || Application.platform == RuntimePlatform.IPhonePlayer)
        //    GlobalAPI.SetSdkLogCallback(DumpLog);

        _sdkInit = initSDK(nimAppKey, nimAppSecret);
        DWChatUtils.DebugLog("_sdkInit :" + _sdkInit + ",lstate :" + lstate);
        if (_sdkInit)
        {
            if (lstate == 2) //未登录
            { 
                AccountInfo info = DW_getAccountInfo();
                if (info != null && info.Account != null && info.Password != null)
                {
                    ClientAPI.Login(nimAppKey, info.Account, info.Password, loginCallBack);
                }
                else
                {
                    Debug.LogError("can not login, please check account or password or sdk initialized=======");
                }
            }
            else if (lstate == 1) //已登录
            {
                loginSuccess();
            }
        }
        else
        {
            DWChatUtils.DebugLog("sdk is not init+++++++++");
        }
    }

    private static void loginSuccess() 
    {
        if (_loginSuccess)
            return;

        DWChatUtils.DebugLog("login success============");
        _loginSuccess = true;

        DWChatTalk.getInstance().DW_initTalk();
        DWChatTeam.getInstance().DW_initTeam();

        DW_SetCallbacks();
        ChatInterface.getInstance().imLoginCallBack(true);
        
    }

    [AOT.MonoPInvokeCallback(typeof(NIM.ClientAPI.LoginResultDelegate))]
    private static void loginCallBack(NIMLoginResult res)
    {
        if (res.Code == ResponseCode.kNIMResSuccess && res.LoginStep == NIMLoginStep.kNIMLoginStepLogin)
        {
            loginSuccess();
        }
        else if(res.LoginStep == NIMLoginStep.kNIMLoginStepLink || res.LoginStep == NIMLoginStep.kNIMLoginStepLinking || res.LoginStep == NIMLoginStep.kNIMLoginStepLogining)
        {
            DWChatUtils.DebugLog("logining==========");
        }
        else
        {
            DWChatUtils.DebugLogFormat("nim login failed, error code = {0}:{1}, --------", res.LoginStep, res.Code);
            //DW_nim_login (); 
            _loginSuccess = false;
            ChatInterface.getInstance().imLoginCallBack(false);
        }

    }

    [AOT.MonoPInvokeCallback(typeof(NimWriteLogDelegate))]
    static void DumpLog(int level, string log)
    {
 //       UnityEngine.Debug.Log(log);
    }

    public static void DW_SetCallbacks()
    {
        ClientAPI.RegDisconnectedCb(OnDisconnected);
        ClientAPI.RegAutoReloginCb(OnAutoRelogin);
        DataSyncAPI.RegCompleteCb(OnDataSyncCompleted);
        System.AppDomain.CurrentDomain.UnhandledException += OnReceiveUnhandledException;
    }

    public AccountInfo DW_getAccountInfo()
    {
        //优先查找内存
        if (_accountInfo != null && _accountInfo.Account != null && _accountInfo.Password != null)
        {
            return _accountInfo;
        }
        return null;
    }

    public void DW_logout()
    {
        if (_loginSuccess && ClientAPI.GetLoginState() == NIMLoginState.kNIMLoginStateLogin)
        {
            DWChatUtils.DebugLog("========logout im===============");
            //DW_setAccountInfo("", "");
            ClientAPI.Logout(NIMLogoutType.kNIMLogoutChangeAccout, LogoutCallBack);
        }
    }

    [AOT.MonoPInvokeCallback(typeof(NIM.ClientAPI.LogoutResultDelegate))]
    public static void LogoutCallBack(NIMLogoutResult res)
    {
        DWChatUtils.DebugLog("LogoutCallBack ===" + res.Code.ToString());
        //if (res.Code == ResponseCode.kNIMResSuccess)
        //{
            DWChatUtils.DebugLog("========logout im success===============");
            _loginSuccess = false;
            ChatInterface.getInstance().imLoginOutCallBack();
            //DWChatTeam.getInstance().DW_dismissTeam("");
            getInstance().DW_setAccountInfo("", "");
            //ClientAPI.Cleanup();
        //}
        //else
        //{
        //    ChatInterface.getInstance().imLoginOutCallBack(false);
        //    Debug.LogError("nim logout failed, error code =  --------" + res.Code);
        //}
    }

    private static void OnDataSyncCompleted(NIMDataSyncType syncType, NIMDataSyncStatus status, string jsonAttachment)
    {
        DWChatUtils.DebugLog(string.Format("Sync completed:{0},{1}", syncType, status));
    }

    private static void OnDisconnected()
    {
        DWChatUtils.DebugLog("网络连接断开，已掉线");
    }

    private static void OnAutoRelogin(NIMLoginResult result)
    {
        DWChatUtils.DebugLog(string.Format("自动重连:{0}", result.Serialize()));
    }

    private static void OnReceiveUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Debug.LogError(e.ToString());
        if (e.ExceptionObject != null)
        {
            Debug.LogError(e.ExceptionObject.ToString());
        }
    }

}