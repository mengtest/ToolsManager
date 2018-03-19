using System.Collections.Generic;
using System.Collections;
using UpdateDefineSpace;



public class UpdateCheckResult
{
    public bool Success;
    public ErrorCode ErrorCode;

    public bool NeedUpdate
    {
        get
        {
            return IsPackageUpdateAvailable || IsResourceUpdateAvailable;
        }
    }
    public bool IsPackageForceUpdate = false;
    public bool IsPackageUpdateAvailable = false;
    public bool IsResourceUpdateAvailable = false;
    public bool IsResourceForceUpdate = false;


    public string PackageUpdateUrl;
    public string PackageUpdateDesc;

    public string PublishedVersion;

    public List<BaseResInfo> ResInfoList;

    public void AddResInfo(BaseResInfo info)
    {
        if (ResInfoList == null)
        {
            ResInfoList = new List<BaseResInfo>();
        }

        ResInfoList.Add(info);
    }
}


public class BaseUpdateChecker
{

}
