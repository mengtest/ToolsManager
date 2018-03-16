/************************************************************
//     文件名      : ChildUpdateExecutor.cs
//     功能描述    : 
//     负责人      : guoliang
//     参考文档    : 无
//     创建日期    : 03/15/2018
//     Copyright  : Copyright
**************************************************************/

using UpdateDefineSpace;
using UnityEngine;


public class ChildUpdateExecutor : BaseUpdateExecutor
{
    public override string GetUpdateType()
    {
        return "Child";
    }


    protected override string GetDownloadUrl(BaseResInfo info, BaseUpdateContext context)
    {
        ChildResInfo pInfo = info as ChildResInfo;
        if (pInfo == null)
            return "";
        string version = context.BaseVersion;

        var url = "";
        if (!string.IsNullOrEmpty(pInfo.packageName))
        {
            url = GameUpdateSys.UPDATE_URL + version + "/" + ((int)pInfo.versionInfo.basePlatform) + "/" + pInfo.versionInfo.resVersion + "/" + pInfo.packageName + ".zip";
        }
        else
        {
            url = GameUpdateSys.UPDATE_URL + version + "/" + ((int)pInfo.versionInfo.basePlatform) + "/" + pInfo.versionInfo.resVersion + "/" + pInfo.type + ".zip";
        }

        if (!GameUpdateSys.RELEASE)
            url += "?" + Random.Range(0, 9999999);

        return url;
    }
    protected override string GetDownloadPath(BaseResInfo info, BaseUpdateContext context)
    {
        ChildResInfo pInfo = info as ChildResInfo;
        if (pInfo == null)
            return "";
        if (!string.IsNullOrEmpty(pInfo.packageName))
        {
            return DWTools.CachePath + "/" + pInfo.packageName + ".zip";
        }
        else
        {
            return DWTools.CachePath + "/" + pInfo.type + ".zip";
        }
    }
}

