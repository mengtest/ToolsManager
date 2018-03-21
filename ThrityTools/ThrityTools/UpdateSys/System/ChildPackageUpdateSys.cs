/************************************************************
//     文件名      : GameUpdateSys.cs
//     功能描述    : guoliang
//     负责人      : 
//     参考文档    : 无
//     创建日期    : 03/16/2018
//     Copyright  : 
**************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UpdateDefineSpace;
using System.IO;

public class ChildPackageUpdateSys : BaseUpdateSys
{
    public ChildPackageUpdateSys(MonoBehaviour gb)
    {
        m_CoroutineJobQueue = new CoroutineJobQueue(gb);
        m_context = new ChildUpdateContext();
    }

    public override void Init(MonoBehaviour gb)
    {
        m_CoroutineJobQueue = new CoroutineJobQueue(gb);
        m_context = new ChildUpdateContext();
    }

    protected void CheckCacheDir(string localName)
    {
        DirectoryInfo dir = new DirectoryInfo(DWTools.CachePath + "/Area");
        if (!dir.Exists)
        {
            dir.Create();
        }
        dir = new DirectoryInfo(DWTools.CachePath + "/Area/" +localName);
        if (!dir.Exists)
        {
            dir.Create();
        }
    }

    protected override IEnumerator _DoCheckUpdate(IUpdateSysDelegate del, BaseUpdateResFilter filter, MonoBehaviour mono, BaseUpdateContext updateContext)
    {
        SetState(State.CheckUpgrade);

        //set context
        var ref_m_context = m_context as ChildUpdateContext;
        var childUpdateContext = updateContext as ChildUpdateContext;

        ref_m_context.BaseVersion = childUpdateContext.BaseVersion;
        ref_m_context.ResVersion = childUpdateContext.ResVersion;
        ref_m_context.PackageVersion = childUpdateContext.PackageVersion;

        ref_m_context.BasePlatform = childUpdateContext.BasePlatform;
        ref_m_context.Platform = childUpdateContext.Platform;
        ref_m_context.areaID = childUpdateContext.areaID;
        ref_m_context.localName = childUpdateContext.localName;




        Debug.LogError("Res.areaID: " + ref_m_context.areaID + " Res.localName: " + ref_m_context.localName + "  pack.Version:" + ref_m_context.ResVersion + " baseplatform:" + ref_m_context.BasePlatform + " platform:" + ref_m_context.Platform);
        //do check
        //bool checkFinish = false;

        CheckCacheDir(ref_m_context.localName);

        ChildPackageUpdateChecker checker = new ChildPackageUpdateChecker(ref_m_context, filter);
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
        var pInfo = info as ChildResInfo;
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

            m_delegate.OnUpdateSuccessed(UpdateResultEnum.Success, m_isReload);

            _UpdateFinished();
        }
    }
}
