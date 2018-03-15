/************************************************************
//     文件名      : ModuleInfo.cs
//     功能描述    : 
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2016-08-30 15:13:56.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;

public class ModuleInfo
{
    public ModuleFieldAttribute m_moduleField;

    public ModuleType m_moduleType;

    public string m_nodeName;

    public bool m_isNeedLoadRes = false;

    public ModuleRoot m_moduleRoot;
}
