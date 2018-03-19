/************************************************************
//     文件名      : ModuleRoot.cs
//     功能描述    : 一个大页面的部分小功能聚集块
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2016-08-30 14:26:38.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;

[LuaWrap]
public class ModuleRoot : BaseUI
{
    protected ModuleInfo m_moduleInfo;

    public bool m_isCreated = false;

    protected int m_oldState;
    protected int m_state;

    public void BaseCreateModule(WindowRoot m_parentWindow, ModuleInfo moduelInfo)
    {
        this.m_currentWindowEnum = m_parentWindow.m_currentWindowEnum;
        this.m_windowRoot = m_parentWindow;
        this.m_moduleInfo = moduelInfo;
        if (!m_isCreated)
        {
            CreateUI();
            CreateModule();
            m_isCreated = true;
        }
    }

    public WindowRoot windowRoot
    {
        get { return m_windowRoot; }
    }

    protected virtual void CreateModule()
    {

    }

    public void BaseInitModule(bool isOpen, int state)
    {
        m_open = isOpen;
        if (m_open)
        {
            m_oldState = m_state;
            m_state = state;
        }
        InitModule(isOpen, state);
    }

    protected virtual void InitModule(bool isOpen, int state)
    {

    }

    public virtual void SetScrollViewCanDrag(bool canDrag)
    {

    }

    protected bool IsWindowActive()
    {
        return null != m_windowRoot && m_windowRoot.m_open;
    }

    protected ModuleRoot GetModuleByType(ModuleType moduleType)
    {
        return m_windowRoot.GetModuleByType(moduleType);
    }

    public virtual void Destroy()
    {
        EventSys.Instance.RemoveHander(this);
        CNetSys.Instance.RemoveAllHandler(this);
        if (m_eventProxy != null)
        {
            m_eventProxy.m_PreClickHandleCallBack = null;
            m_eventProxy.m_ClickHandleCallBack = null;
        }
    }
}
