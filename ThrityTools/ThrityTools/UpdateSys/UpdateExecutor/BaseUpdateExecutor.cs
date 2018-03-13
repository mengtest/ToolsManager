/************************************************************
//     文件名      : BaseUpdateExecutor.cs
//     功能描述    : 
//     负责人      : cai yang
//     参考文档    : 无
//     创建日期    : 05/14/2017
//     Copyright  : Copyright 2017-2018 EZFun.
**************************************************************/

using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;

public abstract class BaseUpdateExecutor : IUpdateExecutor {


    #region IUpdateExecutor implementation

    public abstract string GetUpdateType ();

    protected virtual string GetDownloadUrl(UpdateInfo.ResInfo info, GameUpdateSys.UpdateContext context)
    {
        string version = context.BaseVersion;
        if (info.versionInfo != null && !string.IsNullOrEmpty(info.versionInfo.resVersion))
        {
            version = info.versionInfo.downloadVersion;
        }

        var url = "";
        if (!string.IsNullOrEmpty(info.packageName))
        {
            url = GameUpdateSys.UPDATE_URL + version + "/" + ((int)info.versionInfo.basePlatform) + "/" + info.versionInfo.resVersion + "/" + info.packageName + ".zip";
        }
        else 
        {
            url = GameUpdateSys.UPDATE_URL + version + "/" + ((int)info.versionInfo.basePlatform) + "/" + info.versionInfo.resVersion + "/" + info.type + ".zip";
        }

        if (!GameUpdateSys.RELEASE)
            url += "?" + Random.Range(0, 9999999);
  
        return url;
    }
    protected virtual string GetDownloadPath(UpdateInfo.ResInfo info, GameUpdateSys.UpdateContext context)
    {
        if (!string.IsNullOrEmpty(info.packageName))
        {
            return DWTools.CachePath + "/" + info.packageName + ".zip";
        }
        else
        {
            return DWTools.CachePath + "/" + info.type + ".zip";
        }
    }

    protected virtual string GetUnzipDir()
    {
        return DWTools.CachePath +"/" + GetUpdateType();
    }

    protected virtual bool IsNeedReloadDLL()
    {
        return false;
    }

    public void CleanCachedResource ()
	{
		var path = GetUnzipDir();
		var dir = new System.IO.DirectoryInfo(path);
		if(dir.Exists)
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

	public IEnumerator GetUpdateCoroutine (UpdateInfo.ResInfo info, GameUpdateSys.UpdateContext context, IUpdateExecutorDelegate del)
	{
		Debug.LogWarning("begin update:" + GetUpdateType());

		if (info == null)
		{
			Debug.LogError("info is null");
			del.OnUpdateError(this, GameUpdateSys.ErrorCode.InvalidConfig, info);

			yield break;
		}

		if (string.IsNullOrEmpty(info.md5) || info.size <= 0)
		{
			Debug.LogError("invalid res update info");
			del.OnUpdateError(this, GameUpdateSys.ErrorCode.InvalidConfig, info);
		}

		bool updateSucc = false;

		var path = GetDownloadPath(info, context);
		var url = GetDownloadUrl(info, context);
		int retryCount = 0;

        List<string> fileList = null;
        bool downLoadSucc = false;
        while (retryCount++ < context.DownloadRetryMaxCount && !downLoadSucc)
		{
			Debug.Log("download to path:" + path);
			Debug.Log("download from url:" + url);
			CDownLoadFile downLoadFile = new CDownLoadFile(path, url, info.md5);
            if (del != null)
            {
                del.UpdateStateNotice(info, UpdateProgressInfo.Phase.Downloading, (int)info.size,0);
            }
          
            ThreadPool.QueueUserWorkItem((object state)=>{
				downLoadFile.BeginDownLoad();
			});

			while (downLoadFile.m_downLoadStatus == CDownLoadFile.EnumDownLoadStatus.EDLS_NONE)
			{
                if (del != null)
                {
                    del.UpdateStateNotice(info, UpdateProgressInfo.Phase.Downloading, (int)info.size,
                     (int)downLoadFile.m_hasDownLoadBits);
                }
                yield return null;
			}
			if (downLoadFile.m_downLoadStatus != CDownLoadFile.EnumDownLoadStatus.EDLS_SUCCESS)
			{
				Debug.LogError("download error:" + downLoadFile.m_downLoadStatus);
				if(retryCount >= context.DownloadRetryMaxCount)
				{
					del.OnUpdateError(this, GameUpdateSys.ErrorCode.DownloadResourceFailed, info);
					yield break;
				}
				else
				{
					Debug.Log("retry download");
				}
				continue;
			}
			else
			{
				downLoadSucc = true;
			}

			var unzipDir = GetUnzipDir();
			Debug.Log("begin unzip, to dir:" + unzipDir);
			CUnzipFile unzip = new CUnzipFile(unzipDir, path, downLoadFile.m_hasDownLoadBits);
            if (del != null)
            {
                del.UpdateStateNotice(info, UpdateProgressInfo.Phase.Unpacking, (int)downLoadFile.m_hasDownLoadBits,0 );
            }
            ThreadPool.QueueUserWorkItem((object state)=>{

				unzip.BeginUnzip();

			});

			while(unzip.m_unzipState == CUnzipFile.EnumUnzipStatus.EUS_NONE)
			{
                //TODO notify progress
                if (del != null)
                {
                    del.UpdateStateNotice(info, UpdateProgressInfo.Phase.Unpacking, (int)downLoadFile.m_hasDownLoadBits,
                       (int) (unzip.m_rate * downLoadFile.m_hasDownLoadBits));
                }
                yield return null;
			}

			switch(unzip.m_unzipState)
			{
			case CUnzipFile.EnumUnzipStatus.EUS_SUCCESS:
				{
					Debug.Log("unzip successed");
					updateSucc = true;
                    fileList = unzip.m_unpackFileList;
                    break;
				}
			default:
				updateSucc = false;
				break;
			}
		}

		if (updateSucc)
		{
			del.OnUpdateFinish(this, info, fileList, this.IsNeedReloadDLL());
		}
		else
		{
			del.OnUpdateError(this, GameUpdateSys.ErrorCode.DownloadResourceFailed, info);
		}
	}

	#endregion




}
