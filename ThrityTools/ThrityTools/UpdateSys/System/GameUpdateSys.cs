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
    public GameUpdateSys(MonoBehaviour gb)
    {
        m_CoroutineJobQueue = new CoroutineJobQueue(gb);
        m_context = new ParentUpdateContext();
    }

    public override void Init(MonoBehaviour gb)
    {
        m_CoroutineJobQueue = new CoroutineJobQueue(gb);
        m_context = new ParentUpdateContext();
    }

    protected override IEnumerator _DoCheckUpdate(IUpdateSysDelegate del, BaseUpdateResFilter filter, MonoBehaviour mono, BaseUpdateContext updateContext)
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

        ParentUpdateChecker checker = new ParentUpdateChecker(m_context, filter);
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