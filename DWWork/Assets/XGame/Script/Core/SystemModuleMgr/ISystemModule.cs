/************************************************************
//     文件名      : ISystemModule.cs
//     功能描述    : 系统模块接口
//     负责人      : lezen   lezenzeng@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2017-07-15 12:28:20.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System;
public abstract class ISystemModule
{
    public abstract void Init();
    public abstract void SetParent(object instance);
    public abstract void Reset();
    public abstract void Release();
    public abstract void Update();
}
