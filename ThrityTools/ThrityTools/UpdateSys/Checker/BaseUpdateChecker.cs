using System.Collections.Generic;
using System.Collections;
using UpdateDefineSpace;
using UnityEngine;
using LitJson;

public class BaseUpdateCheckResult
{
    public bool Success;
    public ErrorCode ErrorCode;

    public string PackageUpdateUrl;
    public string PackageUpdateDesc;

    public string PublishedVersion;

    public virtual bool NeedUpdate
    {
        get { return false; }
    }

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

public class ParentUpdateCheckResult :BaseUpdateCheckResult
{
    public override bool NeedUpdate
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

}

public class ChildUpdateCheckResult : BaseUpdateCheckResult
{
    public int areaID;
    public string localName;
    public bool IsResourceForceUpdate = false;
    public override bool NeedUpdate
    {
        get { return IsResourceForceUpdate; }
    }
}



public class BaseUpdateChecker
{
    protected BaseUpdateContext m_context;

    protected BaseUpdateResFilter m_filter;
    public BaseUpdateChecker()
    { }
    public BaseUpdateChecker(BaseUpdateContext context, BaseUpdateResFilter filter)
    {
        m_context = context;
        m_filter = filter;
    }

    protected virtual string GetUpdateConfigUrl()
    {
        return "";
    }

    protected Coroutine m_checkCor = null;

    public void StartCheck(System.Action<BaseUpdateCheckResult> callback, MonoBehaviour mono)
    {
        if (mono != null)
        {
            if (m_checkCor != null)
                mono.StopCoroutine(m_checkCor);

            m_checkCor = mono.StartCoroutine(GetVersionInfoFile(callback));
        }
    }

    //private int m_downloadRetryTimes = 0;
    //private int m_downloadRetryMaxCount = 1;
    protected virtual IEnumerator GetVersionInfoFile(System.Action<BaseUpdateCheckResult> callback)
    {
        yield return null;
    }

    //here is public for Unit Test
    public virtual BaseUpdateCheckResult CheckIfNeedsUpdate(BaseUpdateConfigInfo cfg)
    {
        BaseUpdateCheckResult result = new BaseUpdateCheckResult();

        return result;
    }


    public static int GetVersionCode(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return 0;
        }
        string[] strArray = str.Split('.');
        int version = int.Parse(strArray[0]) * 10000 + int.Parse(strArray[1]) * 100 + int.Parse(strArray[2]);
        return version;
    }
}
