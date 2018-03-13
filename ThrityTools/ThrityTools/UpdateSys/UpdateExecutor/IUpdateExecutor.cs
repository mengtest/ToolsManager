/************************************************************
//     文件名      : IUpdateExecutor.cs
//     功能描述    : 
//     负责人      : cai yang
//     参考文档    : 无
//     创建日期    : 05/09/2017
//     Copyright  : Copyright 2017-2018 EZFun.
**************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IUpdateExecutor {

	string GetUpdateType();

	void CleanCachedResource();

	IEnumerator GetUpdateCoroutine(UpdateInfo.ResInfo info, GameUpdateSys.UpdateContext context, IUpdateExecutorDelegate del);

	//string GetPathForResource(UpdateConfig.ResInfo info);
}

public enum DownloadState
{
    DownloadState,
    UnZipState,
}

public interface IUpdateExecutorDelegate
{
	void OnUpdateError(IUpdateExecutor executor, GameUpdateSys.ErrorCode errCode, UpdateInfo.ResInfo info);
	void OnUpdateFinish(IUpdateExecutor executor, UpdateInfo.ResInfo info, List<string> fileList, bool isNeedReload = false);
    void UpdateStateNotice(UpdateInfo.ResInfo  res, UpdateProgressInfo.Phase updateState, int totalSize, int curSize);
}

public interface IUpdateFilter
{
    void CheckNeedUpdate(List<UpdateInfo.ResInfo> list, UpdateInfo.ResInfo info);
}