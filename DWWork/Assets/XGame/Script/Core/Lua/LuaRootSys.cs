/************************************************************
     File      : LuaRootSys.cs
     author    : shandong   shandong@ezfun.cn
     version   : 1.0
     date      : 6/27/2016.
     copyright : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using System;
using System.Runtime.InteropServices;

[RegisterSystem(typeof(LuaRootSys), true)]
public class LuaRootSys : TCoreSystem<LuaRootSys>, IResetable, IInitializeable, IUpdateable, ILateUpdateable, IFixedUpdateable
{
    private LuaScriptMgr m_luaMgr;

    private Dictionary<string, LuaSystemRoot> m_luaCtrls = new Dictionary<string, LuaSystemRoot>();

    //private LuaFunction m_luaGetCtrl;

    private LuaFunction m_luaCreateLuaObj;

    public bool m_isLoadFinished = false;
    public bool m_isAsyncTaskComplete = false;

    public LuaScriptMgr LuaMgr
    {
        get
        {
            if (m_luaMgr == null)
            {
                Debug.Log("LuaRootSys.Instance.LuaMgr == null");
                m_luaMgr = new LuaScriptMgr();
                Util.Init();
            }
            return m_luaMgr;
        }
    }

    public object CreateLuaObject(params object[] objects)
    {
        if (m_luaCreateLuaObj != null)
        {
            var rets = m_luaCreateLuaObj.Call(objects);
            if (rets != null && rets.Length > 0)
            {
                return rets[0];
            }
        }
        return null;
    }
    /// <summary>
    /// 无gc
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="gameObj"></param>
    /// <returns></returns>
    public object RequireAndNew(string fileName, object gameObj)
    {
        if (m_luaCreateLuaObj != null)
        {
            var rets = m_luaCreateLuaObj.Call2Args(fileName, gameObj);
            if (rets != null && rets.Length > 0)
            {
                return rets[0];
            }
        }
        return null;
    }


    public void AddLuaSystem(string luaObjectName)
    {
        if (!m_luaCtrls.ContainsKey(luaObjectName))
        {
            m_luaCtrls.Add(luaObjectName, new LuaSystemRoot());
            m_luaCtrls[luaObjectName].SystemName = luaObjectName;
            m_luaCtrls[luaObjectName].InitByLua();
            CallFunction("AddLuaGameSys", luaObjectName);
        }
    }

    public void Init()
    {
        //主线程创建
        var scriptMgr = LuaMgr;
    }

    public void LoadAllLuaFile()
    {
//        Debug.LogError("LoadAllLuaFile start");
        m_luaMgr.DoFile("GameManager");
//        Debug.LogError("GameManager do file");
        m_luaMgr.InitLuaScriptCallBack();
        PBDLL.RegisterLuaPB(m_luaMgr.lua.L);
//        Debug.LogError("RegisterLuaPB do file");
        RegisterEnum();
        m_luaCreateLuaObj = m_luaMgr.GetLuaFunction("CreateObject");
        m_isAsyncTaskComplete = true;
//        Debug.LogError("LoadAllLuaFile end");
    }

    public void LuaInit()
    {
//        Debug.LogError("LuaInit");
        CallFunction("Init");
        m_isLoadFinished = true;
    }

    /// <summary>
    /// 凌晨5点刷新，因为大部分系统都会在这个时候重置
    /// </summary>
    void RefreshDataAM5(EEventType evntid, object p1, object p2)
    {
        int count = 0;
       
       
    }

    public void InitLuaData()
    {
        var enumatrator = m_luaCtrls.GetEnumerator();
        while (enumatrator.MoveNext())
        {
            enumatrator.Current.Value.InitLuaData();
        }
    }

    public void Update()
    {
        if (m_luaMgr != null && m_isLoadFinished)
        {
            m_luaMgr.Update();
        }
    }

    public void LateUpdate()
    {
        if (m_luaMgr != null && m_isLoadFinished)
        {
            m_luaMgr.LateUpate();
        }
    }

    public void FixedUpdate()
    {
        if (m_luaMgr != null && m_isLoadFinished)
        {
            m_luaMgr.FixedUpdate();
        }
    }

    public LuaSystemRoot GetLuaSystem(string luaSysName)
    {
        if (string.IsNullOrEmpty(luaSysName))
        {
            return null;
        }
        if (m_luaCtrls.ContainsKey(luaSysName))
        {
            return m_luaCtrls[luaSysName];
        }
        var objs = LuaMgr.CallLuaFunction("GameManager.GetLuaGameSys", luaSysName);
        if (objs != null && objs[0] != null)
        {
            var luaSys = new LuaSystemRoot();
            m_luaCtrls[luaSysName] = luaSys;
            m_luaCtrls[luaSysName].SystemName = luaSysName;
            m_luaCtrls[luaSysName].InitByLua();
            return luaSys;
        }
        return null;
    }

    public void Release()
    {
        if (m_luaCreateLuaObj != null)
            m_luaCreateLuaObj.Release();
        CallFunction("Release");
        OnDestroy();
    }

    public void Reset()
    {
        CallFunction("Reset");
    }

    private void CallFunction(string functionName, params object[] args)
    {
        LuaMgr.CallLuaFunction("GameManager." + functionName, args);
    }

    public void AddEventHandler(int eventid)
    {
        if (EventSys.Instance != null)
        {
            EventSys.Instance.AddHandler((EEventType)eventid, HandleEvent);
        }
    }

    public void RemoveEventHandle(int eventId)
    {
        if (EventSys.Instance != null)
        {
            EventSys.Instance.RemoveHandleByEventType((EEventType)eventId, HandleEvent);
        }
    }

    private LuaFunction m_handleEventFunction = null;
    private void HandleEvent(EEventType eventid, object p1, object p2)
    {
        if (m_handleEventFunction == null)
        {
            m_handleEventFunction = this.LuaMgr.GetLuaFunction("LuaEvent.HandleEvent");
        }
        m_handleEventFunction.Call3Args((int)eventid, p1, p2);
    }

    public int RegisterPBFlie(string fileName)
    {
        int length = 0;
        byte[] datas = this.LuaMgr.Loader(fileName, out length);
        return PBDLL.Lpb_load_byte(LuaMgr.lua.L, datas, length);
    }

    public void OnDestroy()
    {
        if (m_handleEventFunction != null)
        {
            m_handleEventFunction.Release();
            m_handleEventFunction = null;
        }
        m_luaCtrls.Clear();
        LuaMgr.Destroy();
    }


    void RegisterEnum()
    {
        RegisterEnum(typeof(EEventType));
    }

    void RegisterEnum(Type type)
    {
        var names = Enum.GetNames(type);
        var values = new int[names.Length];
        for (int i = 0; i < names.Length; i++)
        {
            values[i] = ((int)Enum.Parse(type, names[i]));
        }
        LuaScriptMgr.RegisterNewEnumLib(LuaMgr.lua.L, type.Name, names, values);
    }

    #region 针对网路的byte传递优化

    public void SendNetMsgByLua(string netName,int areaID,int cmd, int seq, bool iscb, bool needLockScreen,LuaStringBuffer data)
    {
        GamePackage msg = new GamePackage();
        msg.areaID = areaID;
        msg.cmd = cmd;
        msg.seq = seq;
        msg.needLockScreen = needLockScreen;
        if (data != null)
        {
            msg.body = data.buffer;
        }
        //CNetSys.Instance.AddLuaCMD(cmd);

        CNetSys.Instance.SendNetMsg(netName,cmd, msg, HandleLuaSuccCallBack, false, HandleLuaSuccCallBack,
        iscb ? false : true, false, null,
        true, false, null, seq);
    }

    private LuaFunction m_handleFunc;
    private LuaFunction m_userNetHandleFunc;
    private LuaFunction m_httpHandleFunc;

    private void CallLuaSuccCallBack(string netName,GamePackage package)
    {
        LuaFunction activeFunc = null;
        switch(netName)
        {
            case "GameLogicNet":
                if (m_handleFunc == null)
                {
                    m_handleFunc = LuaRootSys.Instance.LuaMgr.GetLuaFunction("LuaNetWork.HandleMsg");
                }
                activeFunc = m_handleFunc;
                break;
            case "UserLogicNet":
                if (m_userNetHandleFunc == null)
                {
                    m_userNetHandleFunc = LuaRootSys.Instance.LuaMgr.GetLuaFunction("LuaUserNetWork.HandleMsg");
                }
                activeFunc = m_userNetHandleFunc;
                break;
        }

        if (activeFunc != null)
        {
            activeFunc.SendNetMessage(netName,package.cmd, package.seq, package.areaID,package.errno,true,package.body);
        }
    }

    private void CallHttpLuaSuccCallBack(WebSCPackage scPackage)
    {
        if (m_httpHandleFunc == null)
        {
            m_httpHandleFunc = LuaRootSys.Instance.LuaMgr.GetLuaFunction("LuaHttpNetWork.HandleMsg");
        }
        if (m_httpHandleFunc != null)
        {
            m_httpHandleFunc.SendHttpNetMessage(scPackage.m_msgKey,scPackage.m_pack.cmd, scPackage.m_pack.errno,scPackage.m_seq, scPackage.m_pack.body);
        }
    }

    public void HandleLuaSuccCallBack(string netName,GamePackage package)
    {
        CallLuaSuccCallBack(netName,package);
    }

    public void HandleHttpLuaSuccCallBack(WebSCPackage package)
    {
        CallHttpLuaSuccCallBack(package);
    }


    public void CallLuaFunc(string funcName)
    {
        m_luaMgr.CallLuaFunction(funcName);
    }

    public void CallLuaFunc(string funcName,params object[] args)
    {
        m_luaMgr.CallLuaFunction(funcName, args);
    }


    //socket重连进游戏
    public void NetReconnectToGame(string netName)
    {
        EventSys.Instance.AddEvent(EEventType.NetReconnectStart,netName);
    }
    //http重连
    public void HttpReconnect()
    {
//        WindowRoot.ShowTips("数据请求超时");
    }

    //建立游戏服连接
    public void ConnectGameSvr(string netName,string ip,int port)
    {
        CNetSys.Instance.CreateConnect(netName,ip, port,ConnectSucCallBack, null);
    }
    private void ConnectSucCallBack(string netName,bool isConnected)
    {
        EventSys.Instance.AddEvent(EEventType.GameSvrConnectSuc,isConnected, netName);
    }
    #endregion
}

