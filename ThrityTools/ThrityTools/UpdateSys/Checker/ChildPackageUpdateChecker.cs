/************************************************************
//     文件名      : ChildPackageUpdateChecker.cs
//     功能描述    : 子包更新检查
//     负责人      : guoliang
//     参考文档    : 无
//     创建日期    : 03/16/2018
//     Copyright  : 
**************************************************************/
using System.Collections.Generic;
using System.Collections;
using UpdateDefineSpace;
using UnityEngine;
using LitJson;


public class ChildPackageUpdateChecker :BaseUpdateChecker
{
    private ChildUpdateContext m_childContext;
    public ChildPackageUpdateChecker() { }
    public ChildPackageUpdateChecker(BaseUpdateContext context, BaseUpdateResFilter filter)
    {
        m_context = context;
        m_filter = filter;
        m_childContext = context as ChildUpdateContext;
    }

    protected override string GetUpdateConfigUrl()
    {
        string url = BaseUpdateSys.LOCAL_UPDATE_URL + m_childContext.areaID  + "/"+ BaseUpdateSys.LOCAL_UPDATE_CONFIG_FILE + "?" + Random.Range(0, 9999999);
        Debug.LogError(url);
        return url;
    }

    protected override IEnumerator GetVersionInfoFile(System.Action<BaseUpdateCheckResult> callback)
    {
        var result = new ChildUpdateCheckResult();
        ChildUpdateConfigInfo cfg = null;

        var url = GetUpdateConfigUrl();
        bool isDownloadSuc = false;

        //先判断网络
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            Debug.Log("fetch config url:" + url);
            WWW w = new WWW(url);
            float beginTime = Time.realtimeSinceStartup;
            bool timeOut = false;
            while (!w.isDone && !timeOut)
            {
                timeOut = (Time.realtimeSinceStartup - beginTime) >= 3;
                yield return null;
            }

            if (w.error != null)
            {
                Debug.LogError(w.error);
                w.Dispose();
            }
            else if (timeOut)
            {
                Debug.LogError("fetch config timeout");
                w.Dispose();
            }
            else
            {
                isDownloadSuc = true;
                Debug.Log("w.text:" + w.text);
                //parse config
                cfg = JsonMapper.ToObject<ChildUpdateConfigInfo>(w.text);

                w.Dispose();
            }
            //m_downloadRetryTimes++;
        }

        //开关窗口 有0.4S动画 这里统一加一下
        yield return new WaitForSeconds(0.5f);
        if (isDownloadSuc)
        {
            Debug.Log("拉取更新配置成功");
            result = CheckIfNeedsUpdate(cfg) as ChildUpdateCheckResult;

            callback(result);
        }
        else
        {
            Debug.LogError("拉取更新失败！");

            result.Success = isDownloadSuc;
            result.ErrorCode = ErrorCode.FetchConfigFailed;

            callback(result);
        }

    }



    //here is public for Unit Test
    public override BaseUpdateCheckResult CheckIfNeedsUpdate(BaseUpdateConfigInfo cfg)
    {
        ChildUpdateCheckResult result = new ChildUpdateCheckResult();
        ChildUpdateConfigInfo child_cfg = cfg as ChildUpdateConfigInfo;

        if (child_cfg == null)
        {
            Debug.LogError("invalid cfg, cfg == null");

            result.Success = false;
            result.ErrorCode = ErrorCode.InvalidConfig;

            return result;

        }


        //collect dynamic update info

        if (child_cfg.localUpdateInfo == null)
        {
            Debug.LogError("invalid cfg localUpdateInfo, localUpdateInfo == null");

            result.Success = false;
            result.ErrorCode = ErrorCode.InvalidConfig;

            return result;
        }

        result.Success = true;
        result.PublishedVersion = child_cfg.localUpdateInfo.baseVersion;
        var tmpResInfoList = new List<ChildResInfo>();
        for (int i = 0; i < child_cfg.localUpdateInfo.dynamicUpdateInfo.Count; i++)
        {
            var info = child_cfg.localUpdateInfo.dynamicUpdateInfo[i];
            if (info.basePlatform == (int)m_context.BasePlatform || info.basePlatform == (int)EnumAppName.general)
                for (int j = 0; j < info.resInfo.Count; j++)
                {
                    var resInfo = info.resInfo[j];
                    resInfo.versionInfo = info;
                    bool _added = AddResInfo(tmpResInfoList, resInfo);
                }
        }

        //check existing resource files, and filter them
        for (int i = 0; i < tmpResInfoList.Count; i++)
        {
            var info = tmpResInfoList[i];

            if (!m_context.ResMd5Mgr.IsResourceExisted(info))
            {
                result.AddResInfo(info);
            }
        }

        if (result.ResInfoList != null && result.ResInfoList.Count > 0)
        {
            result.IsResourceForceUpdate = true;
        }


        return result;
    }

    protected bool AddResInfo(List<ChildResInfo> list, ChildResInfo info)
    {
        //TODO Filter logic here
        if (m_filter is ChildUpdateResFilter)
        {
            (m_filter as ChildUpdateResFilter).CheckNeedUpdate(list, info);
        }
        else
        {
            list.Add(info);
        }
        return true;
    }

}

