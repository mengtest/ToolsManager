/************************************************************
//     文件名      : GameUpdateSys.cs
//     功能描述    : guoliang
//     负责人      : 
//     参考文档    : 无
//     创建日期    : 03/16/2018
//     Copyright  : 
**************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UpdateDefineSpace;


public class GameUpdateSys : BaseUpdateSys
{

    protected override IEnumerator _DoCheckUpdate(IUpdateSysDelegate del, IUpdateFilter filter, MonoBehaviour mono, BaseUpdateContext updateContext)
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


    public override void StartResourceUpdate(UpdateCheckResult result, IUpdateSysDelegate del)
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


    public override void OnUpdateFinish(IUpdateExecutor executor, BaseResInfo info, List<string> fileList, bool isNeedReload = false)
    {
        var pInfo = info as ParentResInfo;
        Debug.Log("update " + pInfo.type + " finished, progress:" + (m_currIndex + 1) + "/" + m_updateCount);
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

            if (m_updatefinishCB != null)
            {
                m_updatefinishCB(VersionType.Resource, pInfo.versionInfo.resVersion);
            }

            m_delegate.OnUpdateSuccessed(UpdateResult.Success, m_isReload);

            _UpdateFinished();
        }
    }
}