using System;
using UpdateDefineSpace;


 public class ParentResourceMd5Mgr : BaseResourceMd5Mgr
{

    protected override string GetKeyForRes(BaseResInfo resInfo)
    {
        var parentResInfo = resInfo as ParentResInfo;

        return parentResInfo.versionInfo.resVersion + "-" + resInfo.type;
    }
}

