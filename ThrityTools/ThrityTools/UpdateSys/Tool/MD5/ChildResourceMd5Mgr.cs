using UnityEngine;
using System;
using UpdateDefineSpace;


 public class ChildResourceMd5Mgr : BaseResourceMd5Mgr
{
    protected override string GetFilePath()
    {
        string path = Application.persistentDataPath + "/ress_child.cf";
        return path;
    }

    protected override string GetKeyForRes(BaseResInfo resInfo)
    {
        var parentResInfo = resInfo as ChildResInfo;

        return parentResInfo.versionInfo.resVersion + "-" + resInfo.type;
    }
}

