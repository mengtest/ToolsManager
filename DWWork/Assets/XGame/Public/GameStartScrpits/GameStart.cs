//#define TEST_UPDATE
//#define RELEASE
using UnityEngine;
using System;
using System.Collections;
using System.IO;
using UpdateDefineSpace;

#if UNITY_5_6
using UnityEngine.Profiling;
#endif

public class GameStart : MonoBehaviour
{
    public const string PACKAGE_VERSION = "1.4.0";

    public bool RUN_WITH_DLL = false;
    public bool RUN_WITH_DE_LUA = true;
    public bool RUN_WITH_AB = false;
    public bool ENABLE_LOG = true;
    public LogType FILTER_LOG_TYPE = LogType.Log;
    public bool EnableIM = false;

    private bool m_startGame = false;
    private System.Reflection.Assembly m_xgameData = null;

    public string IV = "";
    public string Key = "";

    void Start()
    {
#if UNITY_EDITOR
        Profiler.maxNumberOfSamplesPerFrame = -1;
#endif

#if UNITY_IPHONE
        RUN_WITH_DLL = false;
#endif
        if (Application.platform == RuntimePlatform.Android)
            RUN_WITH_DLL = true;

        GlobalCrypto.InitCry(IV, Key);
        LoadDLL();
        FMODUnity.RuntimeUtils.EnforceLibraryOrder();
		if (RUN_WITH_DLL) {
            Type t = m_xgameData.GetType("Constants");
            var releaseField = t.GetField("RELEASE");
#if RELEASE
			releaseField.SetValue(System.Activator.CreateInstance(t), true);
#else
			releaseField.SetValue(System.Activator.CreateInstance(t), false);
#endif
		} else {
            Type t = m_xgameData.GetType("Constants");
            var releaseField = t.GetField("RELEASE");
#if RELEASE
			releaseField.SetValue(System.Activator.CreateInstance(t), true);
#else
            releaseField.SetValue(System.Activator.CreateInstance(t), false);
#endif
        }

#if UNITY_EDITOR
        Debug.logger.logEnabled = ENABLE_LOG;
		Debug.logger.filterLogType = FILTER_LOG_TYPE;

        Type constants = m_xgameData.GetType("Constants");
        var _EnableIM = constants.GetField("EnableIM");
        _EnableIM.SetValue(System.Activator.CreateInstance(constants), EnableIM);
#else
		CallMethod("SetupDeviceLogger", "GameRoot", null, null);
#endif

        SetSDKPlatform();

        if (RUN_WITH_AB || RUN_WITH_DLL)
        {
            StartABLoadSys();
        }
        else
        {
            InitVersion();
            CheckUpdate();
        }
    }

    private void StartABLoadSys()
    {
        //CallMethod("InitUpdateSys", "CABLoadSys", null, new object[] { RUN_WITH_AB });
        InitVersion();
        CheckUpdate();
    }

    private void ClearAllObjects()
    {
        GameObject gameRoot = GameObject.Find("_GameRoot");
        var scripts = gameRoot.GetComponents(typeof(MonoBehaviour));
        for (int i = 0; i < scripts.Length; ++i)
        {
            if (scripts[i].GetType() != typeof(GameStart))
            {
                MonoBehaviour.Destroy(scripts[i]);
                scripts[i] = null;
            }
        }
        scripts = null;
    }

    private void InitVersion()
    {
        InitVersionByDLL();
    }

    private void InitVersionByDLL()
    {
        var property = GetProperty("Version", "Instance");
        CallMethod("Init", null, property);
        CallMethod("SetPackageVersion", null, property, new object[] { PACKAGE_VERSION });

        GlobalCrypto.InitCry(IV, Key);
    }

    private object CallMethod(string methodName, string targetName = null, object obj = null, object[] parameters = null)
    {
        object obje = null;
        System.Reflection.MethodInfo method = null;
        if (obj == null)
        {
            var targetType = m_xgameData.GetType(targetName);
            if (targetType.GetField("Instance") != null)
            {
                obje = targetType.GetField("Instance").GetValue(null);
            }
            method = targetType.GetMethod(methodName);
        }
        else
        {
            obje = obj;
            method = obj.GetType().GetMethod(methodName);
        }

        return method.Invoke(obje, parameters);
    }

    private object GetProperty(string targetName, string propertyName, object obj = null)
    {
        var targetType = m_xgameData.GetType(targetName);
        return targetType.GetProperty(propertyName).GetValue(obj, null);
    }

    private object GetField(string targetName, string fieldName, object obj = null)
    {
        var targetType = m_xgameData.GetType(targetName);
        return targetType.GetField(fieldName).GetValue(obj);
    }

    private void SetField(string targetName, string fieldName, object obj, object value)
    {
        var targetType = m_xgameData.GetType(targetName);
        targetType.GetField(fieldName).SetValue(obj, value);
    }

    private void CheckUpdate()
    {

        AddUpdateInfo();
        StartCoroutine(CheckUpdateByDLL());
    }

    private void AddUpdateInfo() 
    {
        ParentUpdateConfigInfo updateInfo = new ParentUpdateConfigInfo();
        ParentDynamicUpdateInfo DynamicUpdateInfo = new ParentDynamicUpdateInfo();
        ParentPackageUpdateInfo PackageUpdateInfo = new ParentPackageUpdateInfo();
        ParentServerInfo serverInfo = new ParentServerInfo();
        ParentResVersionInfo resVersionInfo = new ParentResVersionInfo();
        ParentResInfo resInfo = new ParentResInfo();

        ChildUpdateConfigInfo childUpdateConfigInfo = new ChildUpdateConfigInfo();
        ChildUpdateInfo childUpdateInfo = new ChildUpdateInfo();
        ChildResInfo childResInfo = new ChildResInfo();
        ChildeDynamicUpdateInfo childDynamicInfo = new ChildeDynamicUpdateInfo();

    }

    private IEnumerator CheckUpdateByDLL()
    {
		CallMethod("Setup", "MemoryWarnReporter", null, null);

        SetField("X2UpdateSys", "m_isUpdating", null, true);
        var wmgr = m_xgameData.GetType("EZFunWindowMgr");
        SetField("EZFunWindowMgr", "m_isUpdateSys", null, true);
        var wmg = gameObject.AddComponent(wmgr);

        while (!(bool)GetField("EZFunWindowMgr", "m_hasInitWinMgrDone", wmg))
        {
            yield return null;
        }
        

		#if UNITY_EDITOR && !TEST_UPDATE

		SetField("EZFunWindowMgr", "m_isUpdateSys", null, false);
		OnEndUpdate(false);

#else
		var a = new Action<bool>((bool needRestart) =>
		{
			SetField("EZFunWindowMgr", "m_isUpdateSys", null, false);
			OnEndUpdate(needRestart);
		});

        CallMethod("StartUpdate", "X2UpdateSys", null, new object[] { a });
#endif
    }

    private void OnEndUpdate(bool needRestart)
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#if !BUILD_EXP
        SetField("X2UpdateSys", "m_isUpdating", null, false);
        var wmgr = m_xgameData.GetType("EZFunWindowMgr");
        MonoBehaviour.Destroy(gameObject.GetComponent(wmgr));
#endif
        CallMethod("DestroyUpdate", "X2UpdateSys", null, null);
#elif UNITY_ANDROID
            var wmgr = m_xgameData.GetType("EZFunWindowMgr");
            MonoBehaviour.Destroy(gameObject.GetComponent(wmgr));
      
#elif UNITY_IPHONE
        MonoBehaviour.Destroy(gameObject.GetComponent<EZFunWindowMgr>());
#endif
        Debug.Log("update suc");

        ClearAllObjects();
        if (needRestart)
        {
            LoadDLL();
        }
        StartGame();
    }

    void StartGame()
    {
        if (m_startGame)
        {
            return;
        }
        m_startGame = true;

        InitVersion();
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        print("run editor");
        Start_WIN_EDITOR();
#elif UNITY_ANDROID
		Start_Android();
#elif UNITY_IPHONE
        Start_iOS();
#endif
    }

    private void LoadDLL()
    {
        if (RUN_WITH_DLL)
        {
            string path = Application.streamingAssetsPath + "/xgame.ezfun";
            string persistentPath = Application.persistentDataPath + "/DLL/xgame.ezfun";
            if (CheckUseUpdate())
            {
                using (Stream file = File.OpenRead(persistentPath))
                {
                    byte[] xgameData = GlobalCrypto.LoadDLL(file);
                    m_xgameData = System.Reflection.Assembly.Load(xgameData);
                }
            }
            else
            {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                using (Stream file = File.OpenRead(path))
                {
                    byte[] xgameData = GlobalCrypto.LoadDLL(file);
                    m_xgameData = System.Reflection.Assembly.Load(xgameData);
                }
#elif UNITY_ANDROID
                WWW www = new WWW(path);  
                while(!www.isDone);
                Stream file = new MemoryStream(www.bytes);
                byte[] xgameData = GlobalCrypto.LoadDLL(file);
                m_xgameData = System.Reflection.Assembly.Load(xgameData);
#endif
            }
        }
        else
        {
            m_xgameData = this.GetType().Assembly;
        }

		if (m_xgameData != null)
        {
		#if UNITY_ANDROID || UNITY_EDITOR
		Debug.LogError("Cur DLL Verison:" + m_xgameData.ImageRuntimeVersion);
		#endif
        }
    }
    private bool CheckUseUpdate()
    {
        bool useUpdate = false;
        string persistentPath = Application.persistentDataPath + "/DLL/xgame.ezfun";
        string appPath = Application.persistentDataPath + "/localAppVer.cfg";

        if (File.Exists(persistentPath))
        {
            if (File.Exists(appPath))
            {
                using (Stream file = File.OpenRead(appPath))
                {
                    var b = new byte[file.Length];
                    file.Read(b, 0, b.Length);
                    file.Close();

                    string appVer = System.Text.Encoding.Default.GetString(b);
                    if (!string.IsNullOrEmpty(appVer))
                    {
                        if (GetVerInit(appVer) > GetVerInit(PACKAGE_VERSION))
                        {
                            useUpdate = true;
                        }
                        else
                        {
                            DeleteFile(persistentPath);
                        }
                    }
                    else
                    {
                        DeleteFile(persistentPath);
                    }
                }
            }
            else
            {
                DeleteFile(persistentPath);
            }
        }

        Debug.LogError("UseUpdate=" + (useUpdate).ToString());
        return useUpdate;
    }

    private void DeleteFile(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch
        {
        }
    }

    private int GetVerInit(string str)
    {
        string[] strArray = str.Split('.');
        int version = int.Parse(strArray[0]) * 10000 + int.Parse(strArray[1]) * 100 + int.Parse(strArray[2]);
        return version;
    }

    private void Start_WIN_EDITOR()
    {
        GlobalCrypto.InitCry(IV, Key);

        ResourceManager.Instance.Init();

        Type t = m_xgameData.GetType("Constants");
        var pp = t.GetField("FORCE_LOAD_AB");
        pp.SetValue(System.Activator.CreateInstance(t), RUN_WITH_AB);
        pp = t.GetField("RUN_WITH_EN_LUA");
        pp.SetValue(System.Activator.CreateInstance(t), RUN_WITH_DE_LUA);

        gameObject.AddComponent(m_xgameData.GetType("GameRoot"));
        gameObject.AddComponent(m_xgameData.GetType("EZFunWindowMgr"));
        gameObject.AddComponent(m_xgameData.GetType("ShowFPS"));
    }

    private void Start_iOS()
    {
#if UNITY_IOS
        gameObject.AddComponent<GameRoot>();
        gameObject.AddComponent<EZFunWindowMgr>();
        gameObject.AddComponent<ShowFPS>();
#endif
    }

    private void Start_Android()
    {
        GlobalCrypto.InitCry(IV, Key);

        gameObject.AddComponent(m_xgameData.GetType("GameRoot"));
        gameObject.AddComponent(m_xgameData.GetType("EZFunWindowMgr"));
        gameObject.AddComponent(m_xgameData.GetType("ShowFPS"));
    }

    private void SetSDKPlatform()
    {
#if DW_IOS
            SetPlat(100);
#elif DW_Android
            SetPlat(200);
#else
            SetPlat(200);
#endif
    }

    private void SetPlat(int plat)
    {
        //Debug.LogError("set  plat id: " + plat);
        var property = GetProperty("Version", "Instance");
        CallMethod("SetPlatformID", null, property, new object[] { plat });
    }


}
