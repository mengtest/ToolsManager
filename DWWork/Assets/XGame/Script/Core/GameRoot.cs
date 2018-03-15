using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
using ezfun_resource;
using System.Reflection;
using NIM;

public class GameRoot : MonoBehaviour
{

    private bool m_isPause = false;
    /// <summary>
    /// 受暂停影响  
    /// </summary>
    public static float m_gameTime = 0f; //游戏运行时间 将这个的计时放到 GameActorManager里面去，这个时间大部分用处都在哪儿

    /// <summary>
    /// 不受暂停影响
    /// </summary>
    public static float m_realTime = 0f;

    public static int ms_uiFPS = 30;
    public static int ms_gameFPS = 30;

    private bool _m_initSysDone = false;
    public bool m_initSysDone { get { return _m_initSysDone; } }
    static public GameRoot Instance;

    public static void StartMonoCoroutine(IEnumerator routine)
    {
        if (Instance != null && routine != null)
        {
            Instance.StartCoroutine(routine);
        }
    }

    public static void StartMonoCoroutine(string routine, object value)
    {
        if (Instance != null && routine != null)
        {
            Instance.StartCoroutine(routine, value);
        }
    }

    public static void StopMonoCoroutine(IEnumerator routine)
    {
        if (Instance != null && routine != null)
        {
            Instance.StopCoroutine(routine);
        }
    }

    public static int screen_width = 0;
    public static int screen_height = 0;

    void Awake()
    {
        Instance = this;

        Util.Init();
        DWSDKRoot.Instance.Init();

        if (!Constants.RELEASE)
        {
            NetLog.SetNetLogLevel(((int)CNetLogLevel.LogWarning) + 1);
        }

        screen_width = Screen.currentResolution.width;
        screen_height = Screen.currentResolution.height;

        CBuglyPlugin.Init(Constants.BUGLY_APPID);

        Debug.Log("Ezfun GameRoot Awake: " + DateTime.Now.ToString());
        Debug.Log("Ezfun GameRoot Awake:" + DateTime.Now.ToFileTime().ToString());

        //ChartboostSDK.CBExternal.init(Constants.CB_APPID, Constants.CB_APP_TOKEN);
        PrintConfig();

        //在ResourceMagr里面 加载图片的时候使用 由于ResourceMagr是静态工具类 这里初始化
        Util.CreateFolder(Util.m_persistPath + "/LocalDownImg");
        //MemoryWarnReporter.Setup ();
    }

	public static void SetupDeviceLogger()
    {
        //if (Application.platform == RuntimePlatform.WindowsEditor)
        //{
        //    Debug.logger.logEnabled = true;
        //}
        //else
        //{
        //    Debug.logger.logEnabled = true;
        //}

        var disableRemoteConsole = string.IsNullOrEmpty(PlayerPrefs.GetString("__LogPersistent"));

		if (!Constants.RELEASE || !disableRemoteConsole)
		{
            Debug.logger.filterLogType = LogType.Error;

			var remoteConsole = new GameObject("Remote Console");
			GameObject.DontDestroyOnLoad(remoteConsole);

			remoteConsole.AddComponent<FlyingWormConsole3.ConsoleProRemoteServer>();

			Debug.Log("Remote console service started");
		}
		else
		{
            Debug.logger.filterLogType = LogType.Error;
		}

        Debug.Log ("Setup Device Logger finished");

    }

    public static void PrintConfig()
    {
        Constants.InitValue();

        string path = Application.persistentDataPath + "/pre_config.cf";
        FileInfo file = new FileInfo(path);
        if (file.Exists)
        {
            byte[] b = EZFunTools.ReadFileStream(path);
            string str = System.Text.Encoding.Default.GetString(b);
            string[] strArry = str.Split(';');
            string appVersion = strArry[0];
            if (appVersion == Constants.APP_VERSION || !Constants.RELEASE)
            {
                if (strArry[1] == "true")
                    Constants.RELEASE = true;
                else
                    Constants.RELEASE = false;

                Constants.UPDATE_URL = strArry[2];

                if (strArry.Length > 3)
                {
                    if (strArry[3] == "true")
                    {
                        Constants.FORCE_DEBUG_PLATFORM = true;
                    }
                }
                if (strArry.Length > 4)
                {
                    Constants.UPDATE_CONFIG_FILE = strArry[4];
                }
                if (strArry.Length > 5)
                {
                    Constants.WITH_RELEASE_EXP = (strArry[5] == "true");
                }
                if (strArry.Length > 6)
                {
                    Constants.M_IS_IOS_PRE = (strArry[6] == "true");
                }
                if (strArry.Length > 7)
                {
                    Constants.DATAEYE_ID = strArry[7];
                }
            }
        }

        if (!Constants.RELEASE)
        {
            Constants.WITH_RELEASE_EXP = false;
        }
        //        Debug.LogError("本程序为" + (Constants.RELEASE ? "发布版本" : "测试版本"));
        //        Debug.LogError("本程序为" + (Constants.WITH_RELEASE_EXP ? "正式环境" : "沙盒环境"));
        //        Debug.LogError("DataEyeID=" + Constants.DATAEYE_ID);
        //        Debug.LogError("更新地址=" + Constants.UPDATE_URL);
        ////        if(Constants.RELEASE)
        //        {
        //            Debug.LogError("服务器域名=" + Constants.SVR_DOMAIN_NAME);
        //            Debug.LogError("服务器IP=" + Constants.SVR_IP);
        //        }
        //        Debug.LogError("程序版本号=" + Constants.APP_VERSION);
        //        Debug.LogError("资源版本号=" + Constants.RESS_VERSION);
    }

    void Start()
    {
        DontDestroyOnLoad(this);
        StartCoroutine(ChangeLoginStateCoroutine());
        Application.targetFrameRate = ms_gameFPS;
    }
    void InitGameStartNeedSys()
    {
        TimeProfiler.BeginTimer("InitGameStartNeedSys");
        // TimeProfiler.BeginTimer("InitGameStartNeedSys PrepareSerializer");
        //EZFunTools.WarmupSerializer();
        //TimeProfiler.EndTimerAndLog("InitGameStartNeedSys PrepareSerializer");
        TimeProfiler.BeginTimer("InitGameStartNeedSys LoadingSystem");
        SystemManager.Instance.LoadingSystem(true);
        SystemManager.Instance.ModifySystemModuleExecuteOrder();
        TimeProfiler.EndTimerAndLog("InitGameStartNeedSys LoadingSystem");
        TimeProfiler.BeginTimer("InitGameStartNeedSys OnInit");
        SystemManager.Instance.OnInit(true);
        //sdk 平台接口类初始化
        PlatInterface.Instance.Init();

        TimeProfiler.EndTimerAndLog("InitGameStartNeedSys OnInit");
        PreLoadTable();
        TimeProfiler.EndTimerAndLog("InitGameStartNeedSys");
    }

    public void PreLoadTable()
    {
        TableLoader.Instance.m_preloadTable = true;
        //TableLoader.Instance.m_preloadTable = false;
        //DaemonSys.Instance.AddDaemonTask(() =>
        //{
        //    TimeProfiler.BeginTimer("InitGameStartNeedSys LoadAllTable");
        //    TableLoader.Instance.LoadAllTable();
        //    TimeProfiler.EndTimerAndLog("InitGameStartNeedSys LoadAllTable");
        //});
    }

    public void PreLoadAllLua()
    {
        LuaRootSys.Instance.m_isAsyncTaskComplete = false;
        DaemonSys.Instance.AddDaemonTask(() =>
        {
            TimeProfiler.BeginTimer("InitGameStartNeedSys PreLoadAllLua");
            LuaRootSys.Instance.LoadAllLuaFile();
            TimeProfiler.EndTimerAndLog("InitGameStartNeedSys PreLoadAllLua");
        });
        //LuaRootSys.Instance.LoadAllLuaFile();
    }

    //public void InitOtherSys()
    //{
    //    if (m_hasInitSys)
    //    {
    //        return;
    //    }
    //    m_hasInitSys = true;
    //    EZFunSysInfo.Init();
    //    StartCoroutine(InitNeedSys());
    //}

    /// <summary>
    /// 这里是给加载线程用
    /// </summary>
    public IEnumerator InitNeedSys()
    {
        TimeProfiler.BeginTimer("InitNeedSys");

        try
        {
            WindowBaseLua.m_luaMgr = LuaRootSys.Instance.LuaMgr;
            WindowBaseLua.m_luaMgr.Start();
        }
        catch (System.Exception e)
        {
            Debug.LogError("InitOtherSysAsync Error:" + e.ToString());
        }
        SystemManager.Instance.LoadingSystem(false);
        SystemManager.Instance.ModifySystemModuleExecuteOrder();
        while (!TableLoader.Instance.m_preloadTable)
        {
            yield return null;
        }
        SystemManager.Instance.OnInit(false);

        PreLoadAllLua();

        while (!LuaRootSys.Instance.m_isAsyncTaskComplete)
        {
            yield return null;
        }

        LuaRootSys.Instance.LuaInit();
        
        while (!LuaRootSys.Instance.m_isLoadFinished)
        {
            yield return null;
        }
        _m_initSysDone = true;
        TimeProfiler.EndTimerAndLog("InitNeedSys");
    }

    IEnumerator ChangeLoginStateCoroutine()
    {
        while (!EZFunWindowMgr.Instance.m_hasInitWinMgrDone)
        {
            yield return null;
        }

        TimeProfiler.BeginTimer("Game Loading Time");
        InitGameStartNeedSys();

        EZFunSysInfo.Init();
        yield return StartCoroutine(InitNeedSys());
        yield return StartCoroutine(GameStateMgr.Instance.ChangeStateForce(EGameStateType.LoginState));
        TimeProfiler.EndTimerAndLog("Game Loading Time");
    }

    private void OnDestroy()
    {
        DaemonSys.Instance.Close();
        //FastShadowReceiver坑爹，它的后台线程自己不杀
#if NETFX_CORE
		//这里是 Windows Store App. 暂时不处理
#else
        if (Nyahoon.ThreadPool.Instance != null)
        {
            Nyahoon.ThreadPool.Instance.Close();
        }
#endif
        SystemManager.Instance.OnRelease();
        ResourceMgr.Clear(true);
        Resources.UnloadUnusedAssets();
    }

    private float m_pauseTime = 0;

    // applicatoin pause do
    private void OnApplicationPause(bool pause)
    {
        m_isPause = pause;
        if (m_isPause) // pause do
        {
            m_pauseTime = Time.realtimeSinceStartup;
            EventSys.Instance.AddEvent(EEventType.ApplicationAwakeEvent,false);
        }
        else
        {
            if (m_pauseTime != 0 && Time.realtimeSinceStartup - m_pauseTime >= 1)
            {
                m_pauseTime = 0;
                EventSys.Instance.AddEvent(EEventType.ApplicationAwakeEvent,true);
            }
        }
        EventSys.Instance.AddEvent(EEventType.ApplicationPauseEvent, m_isPause);
    }

    System.Threading.ManualResetEvent _resetEvnet = new System.Threading.ManualResetEvent(false);
    private void OnApplicationQuit()
    {
        Debug.Log("Constants.EnableIM : " + Constants.EnableIM + ", DWChatLogin._sdkInit :" + DWChatLogin._sdkInit);
        if (Constants.EnableIM && DWChatLogin._sdkInit)
        {
            NIM.ClientAPI.Logout(NIM.NIMLogoutType.kNIMLogoutAppExit, OnAppExitLogoutCompleted);
 //           _resetEvnet.WaitOne(TimeSpan.FromSeconds(5));
            UnityEngine.Debug.Log("OnApplicationQuit completed");
        }
    }

    private void OnAppExitLogoutCompleted(NIMLogoutResult result)
    {
        Debug.Log("NIM.ClientAPI.Cleanup begin");
        ClientAPI.Cleanup();
        UnityEngine.Debug.Log("NIM.ClientAPI.Cleanup end");
        NIMAudio.AudioAPI.UninitModule();
        _resetEvnet.Set();
    }

    public void HandleAppPauseAction(bool isNeedLowFPS = false)
    {

    }

    public void ResetGameSys()
    {
        SystemManager.Instance.OnReset();
    }

    public void FixedUpdate()
    {
        SystemManager.Instance.OnFixedUpdate();
    }
#if UNITY_IOS
    private Mesh m_mesh = null;
    private float m_delayTime = 0;
#endif
    public void Update()
    {
        GameRoot.m_realTime += Time.deltaTime;
        SystemManager.Instance.OnUpdate();

        CheckClickExitGame();

    }

    public void LateUpdate()
    {
        SystemManager.Instance.OnLateUpdate();
    }

    /// <summary>
    /// 是否有返回功能
    /// </summary>
    protected void CheckClickExitGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && EventSys.Instance != null)
        {
            // 实现SDK的登出
            if (HandleLoadingWindow.m_loadingOver)
            {
                HandleAppPauseAction();
                OnExitGame("1");
            }
        }
    }


    public string GetChannelName()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return "AppStore";
        }
        else
        {
            if (Version.Instance.GetCurrentSdkPlatform() == ENUM_SDK_PLATFORM.ESP_Debug)
            {
                return "GooglePlay";
            }
            else
            {
                return Version.Instance.GetCurrentSdkPlatform().ToString();
            }
        }
    }

    public void OnSwitchAccount(string str)
    {
        EventSys.Instance.AddEvent(EEventType.UI_Msg_LoginOut);
    }

    public void OnExitGame(string strExit)
    {
        if (strExit == "1")
        {
            HandleErrorWindow.m_titleStr = "提示";
            HandleErrorWindow.m_contentStr = "是否退出游戏？";
            HandleErrorWindow.m_okCallBack = (object p1, object p2) =>
            {
                Application.Quit();
            };
            HandleErrorWindow.m_noCallBack = (object p1, object p2) =>
            {
                if (EventSys.Instance != null)
                {
                    EventSys.Instance.AddEventNow(EEventType.GamePause, false);
                }

            };
          EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.error_ui_window, true, 1);
        }
    }

    public static void RemoveHandle(object obj)
    {
        if (CNetSys.Instance != null)
        {
            CNetSys.Instance.RemoveAllHandler(obj);
        }
    }
}
