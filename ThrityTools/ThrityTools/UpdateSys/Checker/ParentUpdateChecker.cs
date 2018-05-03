/************************************************************
//     文件名      : ChildUpdateExecutor.cs
//     功能描述    : 
//     负责人      : guoliang
//     参考文档    : 无
//     创建日期    : 03/15/2018
//     Copyright  : Copyright
**************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UpdateDefineSpace;


public class ParentUpdateChecker :BaseUpdateChecker
{
    public ParentUpdateChecker() { }
    public ParentUpdateChecker(BaseUpdateContext context, BaseUpdateResFilter filter)
    {
        m_context = context;
        m_filter = filter;
    }

    protected override string GetUpdateConfigUrl()
    {
        string url = BaseUpdateSys.UPDATE_URL +  BaseUpdateSys.UPDATE_CONFIG_FILE + "?" + Random.Range(0, 9999999);
        Debug.LogError(url);
        return url;
    }

    protected override IEnumerator GetVersionInfoFile(System.Action<BaseUpdateCheckResult> callback)
    {
        var result = new BaseUpdateCheckResult();
        ParentUpdateConfigInfo cfg = null;

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
                timeOut = (Time.realtimeSinceStartup - beginTime) >= 8;
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
                cfg = JsonMapper.ToObject<ParentUpdateConfigInfo>(w.text);

                w.Dispose();
            }
            //m_downloadRetryTimes++;
        }

        //开关窗口 有0.4S动画 这里统一加一下
        yield return new WaitForSeconds(0.5f);
        if (isDownloadSuc)
        {
            Debug.Log("拉取更新配置成功");
            result = CheckIfNeedsUpdate(cfg);

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
    public override BaseUpdateCheckResult CheckIfNeedsUpdate(BaseUpdateConfigInfo cfg_para)
    {
        ParentUpdateConfigInfo cfg = cfg_para as ParentUpdateConfigInfo;
        ParentUpdateCheckResult result = new ParentUpdateCheckResult();

        if (cfg == null)
        {
            Debug.LogError("invalid cfg, cfg == null");

            result.Success = false;
            result.ErrorCode = ErrorCode.InvalidConfig;

            return result;

        }

        var currPlatform = (int)m_context.Platform;
        string publishedVersion = null;
        for (int i = 0; i < cfg.serverInfo.Count; i++)
        {
            var info = cfg.serverInfo[i];
            //版本发布根据小平台走，如果有配相应平台去相应平台，如果没有取通用平台 "0"
            if (info.platform == currPlatform)
            {
                publishedVersion = info.version;
                break;
            }
            else if (info.platform == (int)ENUM_SDK_PLATFORM.ESP_General)
            {
                publishedVersion = info.version;
            }
        }

        result.Success = true;
        result.PublishedVersion = publishedVersion;

        //当本地版本号比预发布版本号大时，表明正在提审阶段，不需要更新内容
        var packageVerNum = GetVersionCode(m_context.PackageVersion);
        var publishedVerNum = GetVersionCode(publishedVersion);
        if (publishedVerNum < packageVerNum)
        {
            Debug.Log("本地版本号比预发布版本号大, packageVer = " + packageVerNum + ", publishedVer = " + publishedVerNum);
            result.IsPackageUpdateAvailable = false;
            result.IsResourceUpdateAvailable = false;

            return result;
        }

        if (cfg.packageUpdateInfo != null)
        {
            for (int i = 0; i < cfg.packageUpdateInfo.Count; i++)
            {
                var pinfo = cfg.packageUpdateInfo[i];

                if (pinfo.platform == currPlatform || pinfo.platform == (int)ENUM_SDK_PLATFORM.ESP_General)
                {
                    //当自己版本好号比最小版本号小时，需要强制更新
                    if (packageVerNum < GetVersionCode(pinfo.version))
                    {
                        result.IsPackageUpdateAvailable = true;
                        result.IsPackageForceUpdate = true;
                    }
                    //当自己版本号比最小版本号大，但比最大版本号小时，建议更新
                    else if (GetVersionCode(pinfo.maxVersion) > packageVerNum)
                    {
                        result.IsPackageUpdateAvailable = true;
                        result.IsPackageForceUpdate = false;
                    }
                    else
                    {
                        //自己版本号大于或等于最大版本号，无需更新
                    }

                    if (result.IsPackageUpdateAvailable)
                    {
                        result.PackageUpdateUrl = pinfo.updateUrl;
                        result.PackageUpdateDesc = pinfo.updateDesc;
                    }

                    if (pinfo.platform == currPlatform)
                    {
                        break;
                    }
                }
            }
        }

        //如果是强制更新，直接告诉用户需要更新才能进行游戏，后续检测不再需要。
        if (result.IsPackageForceUpdate)
        {
            return result;
        }

        //collect dynamic update info

        if (cfg.dynamicUpdateInfo == null)
        {
            return result;
        }
        Queue<string> appQueue = new Queue<string>();
        var versionInfoList = new List<ParentResVersionInfo>();
        appQueue.Enqueue(m_context.BaseVersion);
        while (appQueue.Count > 0)
        {
            var curVersion = appQueue.Dequeue();
            for (int i = 0; i < cfg.dynamicUpdateInfo.Count; i++)
            {
                var info = cfg.dynamicUpdateInfo[i];

                if (info.baseVersion.Equals(curVersion))
                {
                    for (int j = 0; j < info.resVersionInfo.Count; j++)
                    {
                        var versionInfo = info.resVersionInfo[j];

                        bool _added = false;
                        if (versionInfo.basePlatform == (int)m_context.BasePlatform)
                        {
                            _added = AddResVersionInfo(versionInfoList, versionInfo);
                        }

                        if (versionInfo.basePlatform == (int)EnumAppName.general)
                        {
                            _added = AddResVersionInfo(versionInfoList, versionInfo);
                        }
                        // 这个appVersion 是用来更新DLL
                        //比如当前版本是0.7.2 0.7.3版本更新了DLL 并把版本提升到了0.7.3 
                        //那么按原来的逻辑它就不会去拉新的更新了，而这里的逻辑上再把0.7.3的更细也拉下来

                        //这里是和刀锋逻辑不一样的地方
                        if (!string.IsNullOrEmpty(versionInfo.appVersion))
                        {
                            appQueue.Enqueue(versionInfo.appVersion);
                            versionInfo.forceUpdate = true;
                        }
                        versionInfo.downloadVersion = curVersion;
                        if (_added)
                        {
                            result.IsResourceForceUpdate = result.IsResourceForceUpdate || versionInfo.forceUpdate;
                        }
                    }
                }
            }
        }

        //collect res update info for all this baseVersion
        List<ParentResInfo> tmpResInfoList = new List<ParentResInfo>();
        for (int i = 0; i < versionInfoList.Count; i++)
        {
            var info = versionInfoList[i];
            var list = info.resInfo;
            for (int j = 0; j < list.Count; j++)
            {
                var resInfo = list[j];
                resInfo.versionInfo = info;

                AddResInfo(tmpResInfoList, resInfo);
            }
        }

        tmpResInfoList.Sort((a, b) =>
        {
            return GetVersionCode(a.versionInfo.resVersion) - GetVersionCode(b.versionInfo.resVersion);

        });

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
            result.IsResourceUpdateAvailable = true;
        }

        return result;
    }

    protected void AddResInfo(List<ParentResInfo> list, ParentResInfo info)
    {
        //TODO Filter logic here
        if (m_filter is ParentUpdateResFilter)
        {
            (m_filter as ParentUpdateResFilter).CheckNeedUpdate(list, info);
        }
        else
        {
            list.Add(info);
        }
    }


    protected bool AddResVersionInfo(List<ParentResVersionInfo> list, ParentResVersionInfo platInfo)
    {
        //这里不要对比
        //var resVerNum = Version.GetVersionCode(m_context.ResVersion);
        //if (resVerNum >= Version.GetVersionCode(platInfo.resVersion))
        //{
        //    return false;
        //}

        var item = list.Find((ParentResVersionInfo findValue) =>
        {
            return findValue.resVersion == platInfo.resVersion;
        });

        if (item == null)
        {
            list.Add(platInfo);
        }
        else
        {
            var generalPlatVer = (int)EnumAppName.general;
            // 0.7.2 通用平台一个Table  然后平台1 也有一个Table，那么就把通用平台的删掉
            if (platInfo.basePlatform != generalPlatVer && item.basePlatform == generalPlatVer)
            {
                list.Remove(item);
                list.Add(platInfo);
            }
        }

        return true;
    }
}
