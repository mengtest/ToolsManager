/************************************************************
//     文件名      : UpdateConfig.cs
//     功能描述    : 定义母包和子包的更新信息结构
//     负责人      : guoliang
//     参考文档    : 无
//     创建日期    : 03/15/2018
//     Copyright  : 
**************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UpdateDefineSpace
{
    public enum ErrorCode : int
    {
        Success = 0,

        NetworkError = 1,
        TimeOut = 2,
        FetchConfigFailed = 3,
        InvalidConfig = 4,
        DownloadResourceFailed = 5,


        MAX
    }

    public enum State
    {
        Idle,
        CheckUpgrade,
        Upgrading,
    }

    public enum UpdateResult : int
    {
        Success = 0,
        Failed = 1,
        Skip = 2,
        NeedRestart = 4,
        FailedNetError = 8,
    }


    public class BaseUpdateConfigInfo { }
    public class BaseUpdateConfigItem { }

    //更新信息描述基类
    public class BaseResInfo : BaseUpdateConfigItem
    {
        public BaseResInfo() { }

        public string type;
        public int size;
        public string md5;
        public string packageName;  //新加别名 用于文件加随机数 防止运营商串包（目前移动踩到过）
    }



    //母包更新配置描述结构
    public class ParentUpdateConfigInfo : BaseUpdateConfigInfo
    {
        public ParentUpdateConfigInfo() { }

        public List<ParentServerInfo> serverInfo;
        public List<ParentPackageUpdateInfo> packageUpdateInfo;
        public List<ParentDynamicUpdateInfo> dynamicUpdateInfo;
    }

    //母包服务端配置信息
    public class ParentServerInfo : BaseUpdateConfigItem
    {
        public ParentServerInfo() { }

        public int platform;
        public string version;
    }

    //母包的整包版本更新信息
    public class ParentPackageUpdateInfo : BaseUpdateConfigItem
    {
        public ParentPackageUpdateInfo() { }

        public int platform;
        public string version;
        public string maxVersion;
        public string updateUrl;
        public string updateDesc;
    }
    //母包的动态增量更新信息
    public class ParentDynamicUpdateInfo : BaseUpdateConfigItem
    {
        public ParentDynamicUpdateInfo() { }

        public string baseVersion;

        public List<ParentResVersionInfo> resVersionInfo;
    }

    //母包的增量更新的版本描述
    public class ParentResVersionInfo : BaseUpdateConfigItem
    {
        public ParentResVersionInfo() { }

        public int basePlatform;
        public int testSvrId;
        public string resVersion;
        public string appVersion;
        public bool forceUpdate;
        public List<ParentResInfo> resInfo;


        public string downloadVersion;
    }
    //单个母包更新文件的描述
    public class ParentResInfo : BaseResInfo
    {
        public ParentResInfo() { }
        public ParentResVersionInfo versionInfo;
    }

    //单个子包更新文件的描述
    public class ChildResInfo : BaseResInfo
    {
        public ChildResInfo() { }
        public ChildeDynamicUpdateInfo versionInfo;
    }


    //子包 更新配置描述结构
    public class ChildUpdateConfigInfo : BaseUpdateConfigInfo
    {
        public ChildUpdateConfigInfo() { }

        public List<ChildUpdateInfo> localUpdateInfo;


    }

    //子包更新集信息
    public class ChildUpdateInfo : BaseUpdateConfigItem
    {
        public ChildUpdateInfo() { }
        public int areaID;
        public string localName;
        public string baseVersion;

        public List<ChildeDynamicUpdateInfo> dynamicUpdateInfo;
    }
    //单个子包更新信息
    public class ChildeDynamicUpdateInfo : BaseUpdateConfigItem
    {
        public ChildeDynamicUpdateInfo() { }

        public int basePlatform;
        public string resVersion;
        public List<BaseResInfo> resInfo;
    }
}

