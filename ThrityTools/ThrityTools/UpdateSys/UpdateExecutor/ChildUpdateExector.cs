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

    protected override string GetUnzipDir(BaseUpdateContext context)
    {
        ChildUpdateContext cuContext = context as ChildUpdateContext;
        return DWTools.CachePath + "/" + "Area/" + cuContext.localName + "/" + GetUpdateType();
    }


    protected override string GetDownloadUrl(BaseResInfo info, BaseUpdateContext context)
    {
        ChildResInfo pInfo = info as ChildResInfo;
        ChildUpdateContext cuContext = context as ChildUpdateContext;
        if (pInfo == null)
            return "";
        string version = context.BaseVersion;

        var url = "";
        if (!string.IsNullOrEmpty(pInfo.packageName))
        {
            url = BaseUpdateSys.LOCAL_UPDATE_URL + cuContext.areaID + "/" + ((int)pInfo.versionInfo.basePlatform) + "/"  + pInfo.packageName + ".zip";
        }
        else
        {
            url = BaseUpdateSys.LOCAL_UPDATE_URL + cuContext.areaID + "/" + ((int)pInfo.versionInfo.basePlatform)  + "/" + pInfo.type + ".zip";
        }

        if (!BaseUpdateSys.RELEASE)
            url += "?" + Random.Range(0, 9999999);

        return url;
    }
    protected override string GetDownloadPath(BaseResInfo info, BaseUpdateContext context)
    {
        ChildResInfo pInfo = info as ChildResInfo;
        ChildUpdateContext cuContext = context as ChildUpdateContext;
        if (pInfo == null)
            return "";
        if (!string.IsNullOrEmpty(pInfo.packageName))
        {
            return DWTools.CachePath + "/"+"Area/" + cuContext.localName +"/" + pInfo.packageName + ".zip";
        }
        else
        {
            return DWTools.CachePath + "/" + "Area/" + cuContext.localName + "/" + pInfo.type + ".zip";
        }
    }
}

