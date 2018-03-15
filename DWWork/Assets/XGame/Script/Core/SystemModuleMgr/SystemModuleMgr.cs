/************************************************************
//     文件名      : SystemModelMgr.cs
//     功能描述    : 系统模块管理类，单个系统持有一个实例，管理自己的模块
//     负责人      : lezen   lezenzeng@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2017-07-15 12:26:45.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System;
public class SystemModuleMgr {

    public SystemModuleMgr() { }
    private object m_parentInstance = null;
    private EZFunDictionary<Type, ISystemModule> m_modules = new EZFunDictionary<Type, ISystemModule>();
    public void SetParent(object instance)
    {
        m_parentInstance = instance;
    }
    public void RegisterModule<T>()
        where T : ISystemModule, new()
    {
        T t = new T();
        var type = typeof(T);
        if (!m_modules.ContainsKey(type))
        {
            m_modules.Add(type, t);
            t.Init();
            t.SetParent(m_parentInstance);
        }
    }
    public T GetModule<T>()
        where T : ISystemModule
    {
        var type = typeof(T);
        ISystemModule value;
        if (m_modules.TryGetValue(type,out value))
        {
            return (T)value;
        }
        return null;
    }

    public void UnRegsiter<T>()
        where T : ISystemModule
    {
        var type = typeof(T);
        if (m_modules.ContainsKey(type))
            m_modules[type].Release();
        m_modules.Remove(type);
    }

    public void ModuleReset()
    {
        for (int i = 0; i < m_modules.Count; i++)
        {
            var value = m_modules.GetValue(i);
            value.Reset();
        }
    }

    public void ModuleUpdate()
    {
        for (int i = 0; i < m_modules.Count; i++)
        {
            var value = m_modules.GetValue(i);
            value.Update();
        }
    }
}
