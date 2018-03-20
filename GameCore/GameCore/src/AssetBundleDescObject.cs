/************************************************************
//     文件名      : AssetBundleDescObject.cs
//     功能描述    : ab描述对象
//     负责人      : blackzhou
//     参考文档    : 无
//     创建日期    : 2016/09/20.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AssetBundleDescObject : ScriptableObject
{
    [System.Serializable]
    public class AssetBundleDesc
    {
        public string m_Name;
        public List<string> m_Assets = new List<string>();
    }
    [System.Serializable]
    public class SceneBundleDesc
    {
        public string m_BundleName;
        public string m_SceneName;
    }
    public List<AssetBundleDesc> m_AssetBundleDesc = new List<AssetBundleDesc>();
    public List<SceneBundleDesc> m_SceneBundleDesc = new List<SceneBundleDesc>();
}