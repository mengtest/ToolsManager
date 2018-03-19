/************************************************************
//     文件名      : LuaModule.cs
//     功能描述    : lua的module持有
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2016-08-30 14:37:19.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;

[LuaWrap]
public class LuaModule : ModuleRoot
{
    public CLuaCallBackMgr m_luaCallBackMgr = null;
    private LuaObj m_luaModule;

    protected override void CreateModule()
    {
        m_luaCallBackMgr = new CLuaCallBackMgr();
        var field = m_moduleInfo.m_moduleField;//.GetAttributeOfType<ModuleField>();
        if (field == null || string.IsNullOrEmpty(field.m_luaFile))
        {
            return;
        }

        var ret = LuaRootSys.Instance.RequireAndNew("LuaWindow.Module." + field.m_luaFile, this.gameObject);
        if (ret != null && ret is LuaInterface.LuaTable)
        {
            m_luaModule = new LuaObj(ret as LuaInterface.LuaTable, field.m_luaFile);
        }
        if (m_luaModule != null)
        {
            m_luaModule.CallFunction("BaseCreateModule");
        }
    }

    protected override void InitModule(bool isOpen, int state)
    {
        if (m_luaModule != null)
        {
            m_luaModule.CallFunction("BaseInitModule", isOpen, state);
        }
    }

    protected override void HandleWidgetClick(GameObject gameObj)
    {
        if (m_luaModule != null)
        {
            base.HandleWidgetClick(gameObj);
            m_luaModule.CallFunction("BaseHandleWidgetClick", gameObj);
        }
    }


    public LuaObj GetLuaObj()
    {
        return m_luaModule;
    }

    public LuaInterface.LuaTable GetLuaObjForLua()
    {
        if (m_luaModule != null)
        {
            return m_luaModule.GetLua();
        }
        return null;
    }

    public override void Destroy()
    {
        if (m_luaModule != null && LuaScriptMgr.Instance != null)
        {
            m_luaModule.CallFunction("BaseDestroyModule");
        }
        m_luaModule = null;
        base.Destroy();
    }
}
