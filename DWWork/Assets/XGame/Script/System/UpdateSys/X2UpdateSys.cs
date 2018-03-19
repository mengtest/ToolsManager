/************************************************************
//     文件名      : X2UpdateSys.cs
//     功能描述    : 
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2017-05-19 17:40:12.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System;
using LitJson;
public enum UpdateType : int
{
    Table = 0,
    AssetBundles,
    DLL,
    Lua,
    MapFile,
    Audio,

    MAX,
}
public class X2UpdateSys : MonoBehaviour, IUpdateSysDelegate
{
    public static bool m_isUpdating = false;
    enum UpdateState
    {
        PreState,
        CheckState,
        DownloadState,
        FinishState,
        ErrorState,
    }

    enum UpdateEvent
    {
        StartCheckEvent,
        CheckFinishEvent,
        DownLoadEvent,
        ErrorEvent,
        FinishEvent,
    }

    private static GameObject m_updateGb;

    private StateMachine<UpdateState, UpdateEvent> m_fsm;

    private Action<bool> m_endAction;

    private static JsonData m_updateJson = null;

    private GameUpdateSys m_updateSys = null;

    private BaseUpdateExecutor m_reloadDLLExecutor = null;

    private bool m_isNeedReload = false;

    static public string[] UpdateFileSuffix = new string[5]     // 更新文件后缀名
    {"bytes",
    "assetbundle",
    "ezfun",
    "lua",
    "mapFile"};

    public static void StartUpdate(Action<bool> aciton)
    {
        if (m_updateGb == null)
        {
            m_updateGb = new GameObject("UpdateRoot");
        }
        ResourceManager.Instance.Init();
        DWLoom.Initialize();
        //在更新系统初始化之前 给系统可能的值修改
        GameRoot.PrintConfig();
        var updateSys = m_updateGb.AddComponent<X2UpdateSys>();
        InitUpdateJson();
        EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.update_ui_window, RessType.RT_LoadingUI, true);
        //EZFunWindowMgr.Instance.InitWindowDic(EZFunWindowEnum.error_ui_window, RessType.RT_CommonWindow, RessStorgeType.RST_Always);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        updateSys.InitUpdate(aciton);
    }


    public void InitUpdate(Action<bool> action)
    {
        m_endAction = action;
        m_fsm = new StateMachine<UpdateState, UpdateEvent>();
        m_updateSys = new GameUpdateSys(this);
        m_updateSys.Initialize();
        m_updateSys.m_updatefinishCB = (VersionType type,string resVersion) => {
            Version.Instance.SetVersion(VersionType.Resource, resVersion);
        };



        GameUpdateSys.UPDATE_URL = Constants.UPDATE_URL;
        GameUpdateSys.UPDATE_CONFIG_FILE = Constants.UPDATE_CONFIG_FILE;
        GameUpdateSys.RELEASE = Constants.RELEASE;

    m_reloadDLLExecutor = new DLLUpdateExecutor();
        m_updateSys.RegisterUpdateExecutor(m_reloadDLLExecutor);
        m_updateSys.RegisterUpdateExecutor(new AssetsBundleUpdateExecutor());
        m_updateSys.RegisterUpdateExecutor(new MapFileUpdateExecutor());
        m_updateSys.RegisterUpdateExecutor(new LuaUpdateExecutor());
        m_updateSys.RegisterUpdateExecutor(new TableUpdateExecutor());
		m_updateSys.RegisterUpdateExecutor(new AudioUpdateExecutor());

        m_fsm.In(UpdateState.PreState)
            .On(UpdateEvent.StartCheckEvent)
            .GoTo(UpdateState.CheckState)
            .ExecuteOnEnter(EnterPreEnter);

        m_fsm.In(UpdateState.CheckState)
            .On(UpdateEvent.DownLoadEvent)
            .GoTo(UpdateState.DownloadState)
              .On(UpdateEvent.ErrorEvent)
            .GoTo(UpdateState.ErrorState)
            .On(UpdateEvent.FinishEvent)
            .GoTo(UpdateState.FinishState)
            .ExecuteOnEnter(EnterCheck);

        m_fsm.In(UpdateState.DownloadState)
            .On(UpdateEvent.ErrorEvent)
            .GoTo(UpdateState.ErrorState)
             .On(UpdateEvent.FinishEvent)
            .GoTo(UpdateState.FinishState)
            .ExecuteOnEnter(EnterDownLoad);

        m_fsm.In(UpdateState.ErrorState)
            .On(UpdateEvent.StartCheckEvent)
            .GoTo(UpdateState.CheckState)
             .On(UpdateEvent.DownLoadEvent)
            .GoTo(UpdateState.DownloadState)
            .ExecuteOnEnter(EnterCheckErrorState);

        m_fsm.In(UpdateState.FinishState)
            .ExecuteOnEnter(EnterFinish);

        m_fsm.Initialize(UpdateState.PreState);
        m_fsm.Start();
    }

    void EnterCheckErrorState()
    {
        Debug.Log("EnterCheckErrorState");
        if (m_updateCheckResult != null)
        {
            HandleErrorWindow.m_contentStr = GetUpdateTxt(10) + "(" + m_updateCheckResult.ErrorCode + ")";
            HandleErrorWindow.m_okCallBack = (object p1, object p2) =>
            {
                DWLoom.QueueOnMainThread(() =>
                {
                    m_fsm.Fire(UpdateEvent.DownLoadEvent);
                });
            };
            EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.error_ui_window, true, 0);
        }
        else
        {
            HandleErrorWindow.m_contentStr = GetUpdateTxt(10) + "(0)";
            HandleErrorWindow.m_okCallBack = (object p1, object p2) =>
            {
                DWLoom.QueueOnMainThread(() =>
                {
                    m_fsm.Fire(UpdateEvent.StartCheckEvent);
                });
            };
            EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.error_ui_window, true, 0);
        }
        EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.wait_ui_window, false, 0);
    }

    void EnterPreEnter()
    {
        Debug.Log("Enter_PreEnter");
        //HandleErrorWindow.m_contentStr = GetUpdateTxt(8);
        //EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.error_ui_window, RessType.RT_CommonWindow, true, 2);
        HandleUpdateWindow.Instance.ShowLogin(true, GetUpdateTxt(12));
        m_fsm.Fire(UpdateEvent.StartCheckEvent);
    }

    void EnterCheck()
    {
        EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.wait_ui_window, true, 0);
        var ver = Version.Instance;
        var context = new GameUpdateSys.UpdateContext();
        context.BaseVersion = ver.GetVersion(VersionType.App);
        context.ResVersion = ver.GetVersion(VersionType.Resource);
        context.PackageVersion = ver.GetVersion(VersionType.PackageVersion);

        context.BasePlatform = ver.GetAppNameEnum();
        context.Platform = ver.GetPublishSDKPlatform();
        m_updateSys.CheckUpdate(this, new UpdateResFilter(this.m_reloadDLLExecutor), this, context);
        Debug.Log("EnterCheck");
    }


    void EnterDownLoad()
    {
        //HandleUpdateWindow.Instance.ShowUI(true);
        HandleUpdateWindow.Instance.ShowLogin(false, GetUpdateTxt(12));
        Debug.Log("EnterDownLoad");
        m_updateSys.StartResourceUpdate(m_updateCheckResult, this);
    }

    void EnterFinish()
    {
        Debug.Log("EnterFinish");
        HandleUpdateWindow.Instance.ShowUI(false);
        //HandleErrorWindow.m_contentStr = GetUpdateTxt(8);
        //EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.error_ui_window, RessType.RT_CommonWindow, true, 2);
        if (m_endAction != null)
        {
            m_endAction(m_isNeedReload);
        }
    }


    public static void DestroyUpdate()
    {
        if (m_updateGb != null)
        {
            GameObject.Destroy(m_updateGb);
        }
    }

    #region IUpdateSysDelegate implementation
    public void OnCheckUpdateError(GameUpdateSys.ErrorCode c)
    {
        m_fsm.Fire(UpdateEvent.ErrorEvent);
    }

    public void OnCheckUpdateSuccess(UpdateCheckResult result)
    {
        EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.wait_ui_window, false, 0);
        if (result.NeedUpdate)
        {
            if (result.IsPackageUpdateAvailable)
            {
                var desc = result.PackageUpdateDesc;
                var packageUrl = result.PackageUpdateUrl;
                //tip for download new package
                if (result.IsPackageForceUpdate)
                {
                    // 弹框提示更新游戏
                    if (string.IsNullOrEmpty(desc))
                    {
                        HandleErrorWindow.m_contentStr = GetUpdateTxt(5);
                    }
                    else
                    {
                        HandleErrorWindow.m_contentStr = desc;
                    }
                    HandleErrorWindow.m_okCallBack = (object p1, object p2) =>
                    {
                        OpenGameUpdateURL(packageUrl);
                    };
                    EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.error_ui_window, true, 0);
                    HandleErrorWindow.m_needCloseWindow = false;
                }
                else
                {
                    if (string.IsNullOrEmpty(desc))
                    {
                        HandleErrorWindow.m_contentStr = GetUpdateTxt(6);
                    }
                    else
                    {
                        HandleErrorWindow.m_contentStr = desc;
                    }
                    HandleErrorWindow.m_okCallBack = (object p1, object p2) =>
                    {
                        OpenGameUpdateURL(packageUrl);
                    };
                    HandleErrorWindow.m_noCallBack = (object p1, object p2) =>
                    {
                        HandleCheckResVersion(result);
                    };
                    EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.error_ui_window, true, 1);
                    HandleErrorWindow.m_needCloseWindow = false;
                }

            }
            else
            {
                HandleCheckResVersion(result);
            }

        }
        else
        {
            m_fsm.Fire(UpdateEvent.FinishEvent);
        }

        //检测publishedVersion
        var packageVer = Version.Instance.GetVersion(VersionType.PackageVersion);
        Version.Instance.SetVersion(VersionType.PublishedAppVersion, result.PublishedVersion);
        Version.Instance.SetNoticeURL();
        if (Version.GetVersionCode(packageVer) > Version.GetVersionCode(result.PublishedVersion))
        {
            Debug.Log("此版本为正在提审的版本");
        }

    }

    private UpdateCheckResult m_updateCheckResult;

    private void HandleCheckResVersion(UpdateCheckResult result)
    {
        m_updateCheckResult = result;
        var m_updateMgrLs = m_updateCheckResult.ResInfoList;
        int totalSize = 0;
        string maxVersion = "";
        for (int index = 0; m_updateMgrLs != null && index < m_updateMgrLs.Count; index++)
        {
            totalSize += m_updateMgrLs[index].size;
            if (string.IsNullOrEmpty(maxVersion) ||
            Version.GetVersion(m_updateMgrLs[index].versionInfo.resVersion) > Version.GetVersion(maxVersion))
            {
                maxVersion = m_updateMgrLs[index].versionInfo.resVersion;
            }
        }

        float size = totalSize * 1f / 1024 / 1024;
        if (size < 0.01f)
        {
            size = 0.01f;
        }
        string sizeTxt = size.ToString("0.00") + "MB";

        if (result.IsResourceUpdateAvailable)
        {
            if (!result.IsResourceForceUpdate)
            {
                // 弹框提示是否更新资源
                string str = GetUpdateTxt(3);
                str = TextData.GetText(str, maxVersion, sizeTxt);
                HandleErrorWindow.m_contentStr = str;
                HandleErrorWindow.m_okCallBack = (object p1, object p2) =>
                {
                    m_fsm.Fire(UpdateEvent.DownLoadEvent);
                };
                HandleErrorWindow.m_noCallBack = (object p1, object p2) =>
                {
                    //EndResUpdate();
                    m_fsm.Fire(UpdateEvent.FinishEvent);
                };
                EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.error_ui_window, true, 1);
            }
            else
            {
                // 弹框提示更新资源
                string str = GetUpdateTxt(2);
                str = TextData.GetText(str, maxVersion, sizeTxt);
                HandleErrorWindow.m_contentStr = str;
                HandleErrorWindow.m_okCallBack = (object p1, object p2) =>
                {
                    //BeginResUpdate();
                    m_fsm.Fire(UpdateEvent.DownLoadEvent);
                };
                EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.error_ui_window, true, 0);
            }
        }
        else
        {
            m_fsm.Fire(UpdateEvent.FinishEvent);
        }
    }
    private float m_lastProcess = -1f;
    public void OnUpdateProgress(UpdateProgressInfo info)
    {
        string headStr = null;
        float process = 0;
        string text = null;
        if (info.phase == UpdateProgressInfo.Phase.Downloading)
        {
            process = info.curSize * 0.7f / info.totalSize;
        }
        else
        {
            process = 0.7f + info.curSize * 0.7f / info.totalSize * 0.3f;
        }
        //进度大于1才动  
        if (Mathf.Abs(process - m_lastProcess) > 0.01)
        {
            if (info.phase == UpdateProgressInfo.Phase.Downloading)
            {
                headStr = GetUpdateTxt(4);
            }
            else
            {
                headStr = GetUpdateTxt(11);
            }

            if (info.totalSize < 1024 * 1024)
            {
                text = string.Format("{0} {1:0.00}KB/{2:0.00}KB    ({3}/{4})",
                    headStr, info.curSize / 1024,
                    info.totalSize / 1024, info.curIndex + 1, info.totalCount);
            }
            else
            {
                text = string.Format("{0} {1:0.00}M/{2:0.00}M    ({3}/{4})",
                    headStr, info.curSize / (1024 * 1024),
                    info.totalSize / (1024 * 1024), info.curIndex + 1, info.totalCount);
            }

            m_lastProcess = process;
            ShowText(text, process);
        }
    }

    public void OnUpdateError(GameUpdateSys.ErrorCode c)
    {
        m_fsm.Fire(UpdateEvent.ErrorEvent);
        //m_fsm.Fire(LoadEvent.ErrorOccured);

        //UISys.Instance.ShowConfirm("更新失败，请重试", () =>
        //{

        //    m_fsm.Fire(LoadEvent.RetryUpgrade);

        //}, () =>
        //{

        //    Application.Quit();

        //});

    }

    public void OnUpdateSuccessed(GameUpdateSys.UpdateResult result, bool isNeedReload = false)
    {
        m_isNeedReload = isNeedReload;
        m_fsm.Fire(UpdateEvent.FinishEvent);
    }

    #endregion

    #region tools
    private void OpenGameUpdateURL(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
    }


    private static void InitUpdateJson()
    {
        string update_path = Application.persistentDataPath + "/Table/ResUpdateTxtList.json";
        if (!System.IO.File.Exists(update_path))
        {
            update_path = EZFunTools.StreamPath + "/Table/ResUpdateTxtList.json";
        }

        byte[] b = EZFunTools.ReadFile(update_path);
        if (b != null)
        {
            string str = System.Text.Encoding.UTF8.GetString(b);
            Debug.Log(str);
            m_updateJson = JsonMapper.ToObject(str)["list"];
        }
    }

    public static string GetUpdateTxt(int id)
    {
        if (m_updateJson == null)
        {
            return "";
        }
        for (int i = 0; i < m_updateJson.Count; i++)
        {
            JsonData item = m_updateJson[i];
            if (item["ID"].ToString() == id.ToString())
            {
                string str = item["text"].ToString();
                str = str.Replace("\\n", "\n");
                return str;
            }
        }

        return "";
    }


    #endregion

    private void ShowText(string text, float process)
    {
        var window = EZFunWindowMgr.Instance.GetWindowRoot(EZFunWindowEnum.update_ui_window) as HandleUpdateWindow;
        window.ShowUI(true);
        if (window != null)
        {
            window.SetText(text);
            window.SetProgress(process);
        }
    }
}
