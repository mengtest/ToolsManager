/************************************************************
//     文件名      : IUpdateExecutor.cs
//     功能描述    : 
//     负责人      : guoliang
//     参考文档    : 无
//     创建日期    : 05/09/2017
//     Copyright  : 
**************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UpdateDefineSpace;

public interface IUpdateExecutor {

	string GetUpdateType();

	void CleanCachedResource();

	IEnumerator GetUpdateCoroutine(BaseResInfo info, BaseUpdateContext context, IUpdateExecutorDelegate del);

	//string GetPathForResource(UpdateConfig.ResInfo info);
}

public enum DownloadState
{
    DownloadState,
    UnZipState,
}

public interface IUpdateExecutorDelegate
{
	void OnUpdateError(IUpdateExecutor executor, ErrorCode errCode, BaseResInfo info);
	void OnUpdateFinish(IUpdateExecutor executor, BaseResInfo info, List<string> fileList, bool isNeedReload = false);
    void UpdateStateNotice(BaseResInfo res, UpdateProgressInfo.Phase updateState, int totalSize, int curSize);
}

public interface IUpdateFilter
{
    void CheckNeedUpdate(List<BaseResInfo> list, BaseResInfo info);
}