/************************************************************
//     文件名      : ParentUpdateExecutor.cs
//     功能描述    : 
//     负责人      : guoliang
//     参考文档    : 无
//     创建日期    : 03/15/2018
//     Copyright  : Copyright
**************************************************************/
using UpdateDefineSpace;
using UnityEngine;


public class ParentUpdateExecutor :BaseUpdateExecutor
{
    public override string GetUpdateType()
    {
        return "Parent";
    }


    protected override string GetDownloadUrl(BaseResInfo info, BaseUpdateContext context)
    {
        ParentResInfo pInfo = info as ParentResInfo;
        if (pInfo == null)
            return "";
        string version = context.BaseVersion;
        if (pInfo.versionInfo != null && !string.IsNullOrEmpty(pInfo.versionInfo.resVersion))
        {
            version = pInfo.versionInfo.downloadVersion;
        }

        var url = "";
        if (!string.IsNullOrEmpty(pInfo.packageName))
        {
            url = BaseUpdateSys.UPDATE_URL + version + "/" + ((int)pInfo.versionInfo.basePlatform) + "/" + pInfo.versionInfo.resVersion + "/" + pInfo.packageName + ".zip";
        }
        else
        {
            url = BaseUpdateSys.UPDATE_URL + version + "/" + ((int)pInfo.versionInfo.basePlatform) + "/" + pInfo.versionInfo.resVersion + "/" + pInfo.type + ".zip";
        }

        if (!BaseUpdateSys.RELEASE)
            url += "?" + Random.Range(0, 9999999);

        return url;
    }
    protected override string GetDownloadPath(BaseResInfo info, BaseUpdateContext context)
    {
        ParentResInfo pInfo = info as ParentResInfo;
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

