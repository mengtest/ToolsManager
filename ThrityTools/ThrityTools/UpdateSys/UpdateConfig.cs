/************************************************************
//     文件名      : UpdateConfig.cs
//     功能描述    : 
//     负责人      : cai yang
//     参考文档    : 无
//     创建日期    : 05/11/2017
//     Copyright  : Copyright 2017-2018 EZFun.
**************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UpdateInfo
{
    public UpdateInfo() { }

    public List<ServerInfo> serverInfo;
    public List<PackageUpdateInfo> packageUpdateInfo;
    public List<DynamicUpdateInfo> dynamicUpdateInfo;

    public class ServerInfo
    {
        public ServerInfo() { }

        public int platform;
        public string version;
    }

    public class PackageUpdateInfo
    {
        public PackageUpdateInfo() { }

        public int platform;
        public string version;
        public string maxVersion;
        public string updateUrl;
        public string updateDesc;
    }

    public class DynamicUpdateInfo
    {
        public DynamicUpdateInfo() { }

        public string baseVersion;

        public List<ResVersionInfo> resVersionInfo;
    }

    public class ResVersionInfo
    {
        public ResVersionInfo() { }

        public int basePlatform;
        public int testSvrId;
        public string resVersion;
        public string appVersion;
        public bool forceUpdate;
        public List<ResInfo> resInfo;


        public string downloadVersion;
    }

    public class ResInfo
    {
        public ResInfo() { }

        public string type;
        public int size;
        public string md5;
        public string packageName;
        public UpdateInfo.ResVersionInfo versionInfo;
    }

}




