/************************************************************
//     文件名      : GameUpdateSys.cs
//     功能描述    : 
//     负责人      : cai yang
//     参考文档    : 无
//     创建日期    : 05/09/2017
//     Copyright  : Copyright 2017-2018 EZFun.
**************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public delegate void UpdateFinishCallBack(VersionType type, string resVersion);

public class GameUpdateSys : IUpdateExecutorDelegate
{
    public static string UPDATE_URL = "";
    public static string UPDATE_CONFIG_FILE = "";
    public static bool RELEASE = true;

    public enum ErrorCode : int
    {
        Success = 0,

        NetworkError = 1,
        TimeOut = 2,
        FetchConfigFailed = 3,
        InvalidConfig = 4,
        DownloadResourceFailed = 5,


        MAX
    }

    public enum State
    {
        Idle,
        CheckUpgrade,
        Upgrading,
    }

    public enum UpdateResult : int
    {
        Success = 0,
        Failed = 1,
        Skip = 2,
        NeedRestart = 4,
        FailedNetError = 8,
    }



    public GameUpdateSys(MonoBehaviour gb)
    {
        m_CoroutineJobQueue = new CoroutineJobQueue(gb);
    }

    public class UpdateContext
    {
        public string PackageVersion;
        public string BaseVersion;  //C#代码 版本
        public string ResVersion;   //资源版本

        public ENUM_SDK_PLATFORM Platform;
        public EnumAppName BasePlatform;

        public int DownloadRetryMaxCount = 3;

        public ResourceMd5Mgr ResMd5Mgr = new ResourceMd5Mgr();
    }

    private UpdateContext m_context;
    private State m_state;
    private void SetState(State s)
    {
        m_state = s;

        if (s == State.Idle)
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
    public UpdateFinishCallBack m_updatefinishCB = null;


    #region IInitializeable implementation
    public void Initialize()
    {
        SetState(State.Idle);

        m_context = new UpdateContext();
        m_context.ResMd5Mgr.Load();
    }

    #endregion

    private IDictionary<string, IUpdateExecutor> m_updaterDict =
        new Dictionary<string, IUpdateExecutor>(8);

    public void RegisterUpdateExecutor(IUpdateExecutor exec)
    {
        var t = exec.GetUpdateType();
        if (m_updaterDict.ContainsKey(t))
        {
            Debug.LogError("already registerd this type, ignore:" + t.ToString());
            return;
        }

        m_updaterDict.Add(t, exec);
    }

    private IUpdateExecutor GetUpdateExecutor(string type)
    {
        if (m_updaterDict == null)
        {
            return null;
        }
        else
        {
            if (m_updaterDict.ContainsKey(type))
            {
                return m_updaterDict[type];
            }
            else
            {
                return null;
            }
        }
    }

    private Coroutine m_checkUpdateCor = null;

    public void CheckUpdate(IUpdateSysDelegate del, IUpdateFilter filter, MonoBehaviour mono,UpdateContext updateContext)
    {
        if (m_state != State.Idle)
            return;

        if (mono != null) 
        {
            if (m_checkUpdateCor != null)
                mono.StopCoroutine(m_checkUpdateCor);

            m_checkUpdateCor = mono.StartCoroutine(_DoCheckUpdate(del, filter, mono, updateContext));
        }
    }

    private IEnumerator _DoCheckUpdate(IUpdateSysDelegate del, IUpdateFilter filter, MonoBehaviour mono, UpdateContext updateContext)
    {
        SetState(State.CheckUpgrade);

        //set context

        m_context.BaseVersion = updateContext.BaseVersion;
        m_context.ResVersion = updateContext.ResVersion;
        m_context.PackageVersion = updateContext.PackageVersion;

        m_context.BasePlatform = updateContext.BasePlatform;
        m_context.Platform = updateContext.Platform;


        Debug.LogError (" App.version:" + m_context.BaseVersion + " Res.Version:" + m_context.ResVersion + "  pack.Version:" + m_context.PackageVersion + " baseplatform:" + m_context.BasePlatform + " platform:" + m_context.Platform);
        //do check
        //bool checkFinish = false;

        UpdateChecker checker = new UpdateChecker(m_context, filter);
        checker.StartCheck((result) =>
        {
            SetState(State.Idle);

            //checkFinish = true;

            if (!result.Success)
            {
                //notify error
                if (del != null)
                {
                    del.OnCheckUpdateError(result.ErrorCode);
                }
            }
            else
            {
                if (del != null)
                {
                    del.OnCheckUpdateSuccess(result);
                }
            }

        }, mono);

        //while (!checkFinish)
        //{
           
        //}
        yield return null;
    }


    public void CleanCachedResource()
    {
        var ite = m_updaterDict.GetEnumerator();
        while (ite.MoveNext())
        {
            var kvp = ite.Current;
            var executor = kvp.Value;

			executor.CleanCachedResource();
        }

        m_context.ResMd5Mgr.Clean();
    }

    private CoroutineJobQueue m_CoroutineJobQueue;

    internal class UpdateResourceParam
    {
        public UpdateInfo.ResInfo info;
        public GameUpdateSys.UpdateContext context;
        public IUpdateExecutor executor;
    }

    private IUpdateSysDelegate m_delegate;
    private int m_updateCount = 0;
    private int m_currIndex = 0;
    public void StartResourceUpdate(UpdateCheckResult result, IUpdateSysDelegate del)
    {
        if (m_state == State.Upgrading)
        {
            Debug.LogError("already in upgrading state");
            return;
        }

        if (!result.NeedUpdate)
        {
            Debug.LogError("donot need update");
            return;
        }

        var list = result.ResInfoList;
        if (list == null || list.Count == 0)
        {
            Debug.LogError("empty res list");
            return;
        }

        m_delegate = del;
        _StartUpdate();

        for (int i = 0; i < list.Count; i++)
        {
            var info = list[i];

            IUpdateExecutor executor = GetUpdateExecutor(info.type);
            if (executor != null)
            {
                //JobQueueCoroutine需要重构下更易用
                UpdateResourceParam param = new UpdateResourceParam();
                param.executor = executor;
                param.info = info;
                param.context = m_context;

                m_updateCount++;
                m_CoroutineJobQueue.PushJob(_UpdateResCoroutine, param);
            }
            else
            {
                Debug.LogError("UpdateExecutor not registered for type:" + info.type);
            }
        }

        m_CoroutineJobQueue.StartJobCoroutine();
    }

    public IEnumerator _UpdateResCoroutine(object jobParam)
    {
        var param = jobParam as UpdateResourceParam;
        IEnumerator coroutine = param.executor.GetUpdateCoroutine(param.info, param.context, this);
        while (true)
        {
            if (!coroutine.MoveNext())
            {
                yield break;
            }

            yield return coroutine.Current;
        }
    }

    private void _StartUpdate()
    {
        SetState(State.Upgrading);
    }

    private void _UpdateFinished()
    {
        m_updateCount = 0;
        m_currIndex = 0;
        m_CoroutineJobQueue.StopJobCoroutine();
        m_CoroutineJobQueue.ClearAllJobs();
        m_delegate = null;
        SetState(State.Idle);
    }

    #region IUpdateExecutorDelegate implementation

    public void UpdateStateNotice(UpdateInfo.ResInfo res, UpdateProgressInfo.Phase downloadState, int totalSize, int curSize)
    {
        var data = new UpdateProgressInfo();
        data.totalCount = m_updateCount;
        data.curIndex = m_currIndex;
        data.totalSize = totalSize;
        data.curSize = curSize;
        data.phase = downloadState;
        data.resInfo = res;
        if (m_delegate != null)
            m_delegate.OnUpdateProgress(data);
    }

    public void OnUpdateError(IUpdateExecutor executor, ErrorCode errCode, UpdateInfo.ResInfo info)
    {
        m_delegate.OnUpdateError(errCode);

        _UpdateFinished();

    }
    private bool m_isReload = false;
    public void OnUpdateFinish(IUpdateExecutor executor, UpdateInfo.ResInfo info, List<string> fileList, bool isNeedReload = false)
    {
        Debug.Log("update " + info.type + " finished, progress:" + (m_currIndex + 1) + "/" + m_updateCount);
        if (!m_isReload && isNeedReload)
        {
            m_isReload = true;
        }


        //flag file existed
        m_context.ResMd5Mgr.Set(info, true, fileList);
        m_currIndex++;
        bool allFinished = ((m_currIndex) == m_updateCount);
        if (allFinished)
        {
            //Version.Instance.SetVersion(VersionType.Resource, info.versionInfo.resVersion);

            if(m_updatefinishCB != null)
            {
                m_updatefinishCB(VersionType.Resource, info.versionInfo.resVersion);
            }

            m_delegate.OnUpdateSuccessed(GameUpdateSys.UpdateResult.Success, m_isReload);

            _UpdateFinished();
        }
    }

    #endregion

}

public struct UpdateProgressInfo
{
    public enum Phase
    {
        Downloading,
        Unpacking
    }

    public int totalCount;
    public int curIndex;

    public int totalSize;
    public int curSize;

    public UpdateInfo.ResInfo resInfo;

    public Phase phase;
}

public interface IUpdateSysDelegate
{
    void OnCheckUpdateError(GameUpdateSys.ErrorCode c);
    void OnCheckUpdateSuccess(UpdateCheckResult result);

    //	void On

    void OnUpdateProgress(UpdateProgressInfo info);
    void OnUpdateError(GameUpdateSys.ErrorCode c);
    void OnUpdateSuccessed(GameUpdateSys.UpdateResult result, bool isNeedReload = false);
}

