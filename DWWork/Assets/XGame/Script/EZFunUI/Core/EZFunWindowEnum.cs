/************************************************************
//     文件名      : EZFunWindowEnum.cs
//     功能描述    : 
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2016-08-30 11:35:18.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System;

public class WindowTypeAttribute : Attribute
{
    public Type m_classType;
    public string m_resPath;
    public string m_luaFile;
    public bool m_needClosePrimary;
    public bool m_needCreate;
	public WindowCloseBehaviour m_windowCloseBehaviour;
	public WindowBehaviourOnSceneChange m_windowBehaviourOnSceneChange;
    
	/// <summary>
    /// 类是c#的用这个
    /// </summary>
    /// <param name="type"></param>
	public WindowTypeAttribute(Type type, string resPath = "", bool needClosePrimary = false, WindowCloseBehaviour windowCloseBehaviour = WindowCloseBehaviour.SetActiveFalse, WindowBehaviourOnSceneChange windowBehaviourOnSceneChange = WindowBehaviourOnSceneChange.CloseButDontDestroy)
    {
        this.m_classType = type;
        this.m_resPath = resPath;
        this.m_needClosePrimary = needClosePrimary;
		this.m_windowCloseBehaviour = windowCloseBehaviour;
		this.m_windowBehaviourOnSceneChange = windowBehaviourOnSceneChange;
    }

    /// <summary>
    /// 具体代码是lua的用这个
    /// </summary>
    /// <param name="luaName"></param>
	public WindowTypeAttribute(string luaName, string resPath = "",bool needClosePrimary = false, WindowCloseBehaviour windowCloseBehaviour = WindowCloseBehaviour.SetActiveFalse,  WindowBehaviourOnSceneChange windowBehaviourOnSceneChange = WindowBehaviourOnSceneChange.CloseButDontDestroy)
    {
        this.m_classType = null;
        this.m_luaFile = luaName;
        this.m_resPath = resPath;
        this.m_needClosePrimary = needClosePrimary;
		this.m_windowCloseBehaviour = windowCloseBehaviour;
		this.m_windowBehaviourOnSceneChange = windowBehaviourOnSceneChange;
    }

}

[LuaWrap]
public enum EZFunWindowEnum
{
    None,

    [WindowType(typeof(HandleCoverWindow))]
    cover_ui_window,
    [WindowType(typeof(HandleLoadingWindow),"",false,WindowCloseBehaviour.SetActiveFalse,WindowBehaviourOnSceneChange.DontClose)]
    loading_ui_window,

    FULL_WINDOW_SPLIT, //分界线 在这之上的窗口打开需要黑屏，下面的不需要

    [WindowType(typeof(HandleWaitWindow), "", false, WindowCloseBehaviour.SetActiveFalse)]
    wait_ui_window,
    [WindowType(typeof(HandleErrorWindow))]
    error_ui_window,
    [WindowType(typeof(HandleErrTipsWindow))]
    err_tips_ui_window,
    [WindowType(typeof(HandleUpdateWindow), "", false, WindowCloseBehaviour.SetActiveFalse, WindowBehaviourOnSceneChange.DontClose)]
    update_ui_window,
    [WindowType(typeof(HandleNetTimeOutWindow))]
    reconnect_ui_window,
    [WindowType(typeof(GMUI))]
    gm_ui_window,

    luaWindow,
}
