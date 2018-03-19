/************************************************************
//     文件名      : GameUpdateSys.cs
//     功能描述    : 
//     负责人      : 
//     参考文档    : 无
//     创建日期    : 05/09/2017
//     Copyright  : 
**************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UpdateDefineSpace;


public delegate void UpdateFinishCallBack(VersionType type, string resVersion);

public class BaseUpdateContext
{
    public string PackageVersion;
    public string BaseVersion;  //C#代码 版本
    public string ResVersion;   //资源版本

    public ENUM_SDK_PLATFORM Platform;
    public EnumAppName BasePlatform;

    public int DownloadRetryMaxCount = 3;

    public BaseResourceMd5Mgr ResMd5Mgr;
}

public class ParentUpdateContext : BaseUpdateContext
{
    public ParentUpdateContext()
    {
        ResMd5Mgr = new ParentResourceMd5Mgr();
    }
}

public class ChildUpdateContext : BaseUpdateContext
{
    public int areaID;
    public string localName;
    public ChildUpdateContext()
    {
        ResMd5Mgr = new ChildResourceMd5Mgr();
    }
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

    public BaseResInfo resInfo;

    public Phase phase;
}

public interface IUpdateSysDelegate
{
    void OnCheckUpdateError(ErrorCode c);
    void OnCheckUpdateSuccess(BaseUpdateCheckResult result);

    //	void On

    void OnUpdateProgress(UpdateProgressInfo info);
    void OnUpdateError(ErrorCode c);
    void OnUpdateSuccessed(UpdateResult result, bool isNeedReload = false);
}



public class BaseUpdateSys : IUpdateExecutorDelegate
{
    public static string UPDATE_URL = "";
    public static string UPDATE_CONFIG_FILE = "";
    public static bool RELEASE = true;

    public BaseUpdateSys()
    {

    }

    public BaseUpdateSys(MonoBehaviour gb)
    {
        m_CoroutineJobQueue = new CoroutineJobQueue(gb);
        m_context = new BaseUpdateContext();
    }


    protected BaseUpdateContext m_context;
    protected State m_state;
    protected void SetState(State s)
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


    public virtual void Init(MonoBehaviour gb)
    {
        m_CoroutineJobQueue = new CoroutineJobQueue(gb);
        m_context = new BaseUpdateContext();
    }

    #region IInitializeable implementation
    public void Initialize()
    {
        SetState(State.Idle);

        m_context.ResMd5Mgr.Load();
    }

    #endregion

    protected IDictionary<string, IUpdateExecutor> m_updaterDict =
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

    protected IUpdateExecutor GetUpdateExecutor(string type)
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

    public virtual void CheckUpdate(IUpdateSysDelegate del, BaseUpdateResFilter filter, MonoBehaviour mono, BaseUpdateContext updateContext)
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

    protected virtual IEnumerator _DoCheckUpdate(IUpdateSysDelegate del, BaseUpdateResFilter filter, MonoBehaviour mono, BaseUpdateContext updateContext)
    {
        SetState(State.CheckUpgrade);
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

    protected CoroutineJobQueue m_CoroutineJobQueue;

    protected class UpdateResourceParam
    {
        public BaseResInfo info;
        public BaseUpdateContext context;
        public IUpdateExecutor executor;
    }

    protected IUpdateSysDelegate m_delegate;
    protected int m_updateCount = 0;
    protected int m_currIndex = 0;

    public virtual void StartResourceUpdate(BaseUpdateCheckResult result, IUpdateSysDelegate del)
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

    protected void _StartUpdate()
    {
        SetState(State.Upgrading);
    }

    protected void _UpdateFinished()
    {
        m_updateCount = 0;
        m_currIndex = 0;
        m_CoroutineJobQueue.StopJobCoroutine();
        m_CoroutineJobQueue.ClearAllJobs();
        m_delegate = null;
        SetState(State.Idle);
    }

    #region IUpdateExecutorDelegate implementation

    public void UpdateStateNotice(BaseResInfo res, UpdateProgressInfo.Phase downloadState, int totalSize, int curSize)
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

    public void OnUpdateError(IUpdateExecutor executor, ErrorCode errCode, BaseResInfo info)
    {
        m_delegate.OnUpdateError(errCode);

        _UpdateFinished();

    }
    protected bool m_isReload = false;
    public virtual void OnUpdateFinish(IUpdateExecutor executor, BaseResInfo info, List<string> fileList, bool isNeedReload = false)
    {

    }

    #endregion

}

