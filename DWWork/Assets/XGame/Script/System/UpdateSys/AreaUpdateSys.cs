/************************************************************
//     文件名      : AreaUpdateSys.cs
//     功能描述    : 
//     负责人      : guoliang
//     参考文档    : 无
//     创建日期    : 2017-05-19 17:40:12.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System;
using LitJson;
using UpdateDefineSpace;

public class AreaUpdateSys : MonoBehaviour, IUpdateSysDelegate
{
    public static bool m_isUpdating = false;

    private static GameObject m_updateGb;

    private StateMachine<UpdateState, UpdateEvent> m_fsm;

    private Action<bool> m_endAction;

    private static JsonData m_updateJson = null;

    private ChildPackageUpdateSys m_updateSys = null;

    private BaseUpdateExecutor m_reloadDLLExecutor = null;

    private bool m_isNeedReload = false;

    static public string[] UpdateFileSuffix = new string[5]     // 更新文件后缀名
    {"bytes",
    "assetbundle",
    "ezfun",
    "lua",
    "mapFile"};

    //设置地区玩法
    public static void SetAreaPlay(int areaID,string localName)
    {
        PlayerPrefs.SetInt("localPackage_areaID", areaID);
        PlayerPrefs.SetString("localPackage_localName",localName);
    }


    public static void StartUpdate(Action<bool> aciton)
    {
        if (m_updateGb == null)
        {
            m_updateGb = new GameObject("AreaUpdateRoot");
        }

        ////test
        //ResourceManager.Instance.Init();
        ////在更新系统初始化之前 给系统可能的值修改
        //GameRoot.PrintConfig();
        //X2UpdateSys.InitUpdateJson();
        ////--------
        var updateSys = m_updateGb.AddComponent<AreaUpdateSys>();
        EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.update_ui_window, RessType.RT_LoadingUI, true);
        updateSys.InitUpdate(aciton);
    }


    public void InitUpdate(Action<bool> action)
    {
        m_endAction = action;
        m_fsm = new StateMachine<UpdateState, UpdateEvent>();
        m_updateSys = new ChildPackageUpdateSys(this);
        m_updateSys.Initialize();
        m_updateSys.m_updatefinishCB = (VersionType type, string resVersion) =>
        {
 //           Version.Instance.SetVersion(VersionType.Resource, resVersion);

        };



        BaseUpdateSys.LOCAL_UPDATE_URL = Constants.LOCAL_UPDATE_URL;
        BaseUpdateSys.LOCAL_UPDATE_CONFIG_FILE = Constants.LOCAL_UPDATE_CONFIG_FILE;

        m_reloadDLLExecutor = new DLLUpdateExecutor();

        m_updateSys.RegisterUpdateExecutor(new ChildAssetBundleUpdateExecutor());
        m_updateSys.RegisterUpdateExecutor(new ChildLuaUpdateExecutor());
        m_updateSys.RegisterUpdateExecutor(new ChildAudioUpdateExecutor());

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
            HandleErrorWindow.m_contentStr = X2UpdateSys.GetUpdateTxt(10) + "(" + m_updateCheckResult.ErrorCode + ")";
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
            HandleErrorWindow.m_contentStr = X2UpdateSys.GetUpdateTxt(10) + "(0)";
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
        HandleUpdateWindow.Instance.ShowLogin(true, X2UpdateSys.GetUpdateTxt(12));
        m_fsm.Fire(UpdateEvent.StartCheckEvent);
    }

    void EnterCheck()
    {
        EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.wait_ui_window, true, 0);
        var ver = Version.Instance;
        var context = new ChildUpdateContext();
        context.BasePlatform = ver.GetAppNameEnum();
        context.Platform = ver.GetPublishSDKPlatform();
        context.areaID = PlayerPrefs.GetInt("localPackage_areaID", 10002);
        context.localName = PlayerPrefs.GetString("localPackage_localName","DouDiZhu");

        m_updateSys.CheckUpdate(this, new ChildUpdateResFilter(this.m_reloadDLLExecutor), this, context);
        Debug.Log("EnterCheck");
    }


    void EnterDownLoad()
    {
        //HandleUpdateWindow.Instance.ShowUI(true);
        HandleUpdateWindow.Instance.ShowLogin(false, X2UpdateSys.GetUpdateTxt(12));
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
    public void OnCheckUpdateError(ErrorCode c)
    {
        m_fsm.Fire(UpdateEvent.ErrorEvent);
    }

    public void OnCheckUpdateSuccess(BaseUpdateCheckResult result)
    {
        EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.wait_ui_window, false, 0);
        if (result.NeedUpdate)
        {
           HandleCheckResVersion(result);

        }
        else
        {
            m_fsm.Fire(UpdateEvent.FinishEvent);
        }
    }

    private ChildUpdateCheckResult m_updateCheckResult;

    private void HandleCheckResVersion(BaseUpdateCheckResult result)
    {
        m_updateCheckResult = result as ChildUpdateCheckResult;
        var m_updateMgrLs = m_updateCheckResult.ResInfoList;
        int totalSize = 0;
        string maxVersion = m_updateCheckResult.PublishedVersion;
        for (int index = 0; m_updateMgrLs != null && index < m_updateMgrLs.Count; index++)
        {
            totalSize += m_updateMgrLs[index].size;
        }

        float size = totalSize * 1f / 1024 / 1024;
        if (size < 0.01f)
        {
            size = 0.01f;
        }
        string sizeTxt = size.ToString("0.00") + "MB";

        if (m_updateCheckResult.IsResourceForceUpdate)
        {
            // 弹框提示更新资源
            string str = X2UpdateSys.GetUpdateTxt(2);
            str = TextData.GetText(str, maxVersion, sizeTxt);
            HandleErrorWindow.m_contentStr = str;
            HandleErrorWindow.m_okCallBack = (object p1, object p2) =>
            {
                //BeginResUpdate();
                m_fsm.Fire(UpdateEvent.DownLoadEvent);
            };
            EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.error_ui_window, true, 0);
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
                headStr = X2UpdateSys.GetUpdateTxt(4);
            }
            else
            {
                headStr = X2UpdateSys.GetUpdateTxt(11);
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

    public void OnUpdateError(ErrorCode c)
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

    public void OnUpdateSuccessed(UpdateResultEnum result, bool isNeedReload = false)
    {
        m_isNeedReload = isNeedReload;
        m_fsm.Fire(UpdateEvent.FinishEvent);
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
