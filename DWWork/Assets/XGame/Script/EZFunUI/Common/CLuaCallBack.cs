/************************************************************
    File      : CLuaCallBack.cs
    brief     : Lua 回调C#   
    author    : JanusLiu   janusliu@ezfun.cn
    version   : 1.0
    date      : 2015/1/23 16:20:8
    copyright : Copyright 2014 EZFun Inc.
**************************************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

public class CLuaCallBack 
{
	int m_luaFuncIndex = 0;

	void CallBack(EEventType evendID, object p1, object p2)
	{
		if(p1 != null && p2 != null)
		{
			WindowBaseLua.m_luaMgr.CallLuaFunction("call_func", new object[]{m_luaFuncIndex, evendID, p1, p2});
		}
		else if(p1 != null)
		{
			WindowBaseLua.m_luaMgr.CallLuaFunction("call_func", new object[]{m_luaFuncIndex, evendID, p1});
		}
		else
		{
			WindowBaseLua.m_luaMgr.CallLuaFunction("call_func", new object[]{m_luaFuncIndex, evendID});
		}
	}

	void CallBack(object p1, object p2)
	{
		if(p1 != null && p2 != null)
		{
			WindowBaseLua.m_luaMgr.CallLuaFunction("call_func", new object[]{m_luaFuncIndex, p1, p2});
		}
		else if(p1 != null)
		{
			WindowBaseLua.m_luaMgr.CallLuaFunction("call_func", new object[]{m_luaFuncIndex, p1});
		}
		else
		{
			WindowBaseLua.m_luaMgr.CallLuaFunction("call_func", new object[]{m_luaFuncIndex});
		}
	}

	void CallBack()
	{
		WindowBaseLua.m_luaMgr.CallLuaFunction("call_func", new object[]{m_luaFuncIndex});
	}

	void CallBack(GamePackage msg)
	{
		WindowBaseLua.m_luaMgr.CallLuaFunction("call_func", new object[]{m_luaFuncIndex, msg});
	}

    void CallBack(int err)
    {
        WindowBaseLua.m_luaMgr.CallLuaFunction("call_func", new object[] { m_luaFuncIndex, err });
    }

    void CallBack(string str)
    {
        WindowBaseLua.m_luaMgr.CallLuaFunction("call_func", new object[] { m_luaFuncIndex, str });
    }

	public CLuaCallBack(int luaFuncIndex)
	{
		m_luaFuncIndex = luaFuncIndex;
	}

	public object GetCallBack(Type type)
	{
		object callBack = Delegate.CreateDelegate(type, this, "CallBack");

		return callBack;
	}

   
}
[LuaWrap]
public class CLuaCallBackMgr
{
	List<int> m_callBackList = new List<int>();

	public CLuaCallBackMgr()
	{
	}

	public void AddCallBack(object target, string functionName, params object[] paramArray)
	{
		MethodInfo method = null;

		try 
		{
			System.Type type = (System.Type)target;
            method = type.GetMethod(functionName);
		}
		catch(Exception)
		{
			Type t = target.GetType();
			method = t.GetMethod(functionName);
			while(method == null && t.BaseType != null)
			{
				t = t.BaseType;
				method = t.GetMethod(functionName);
			}
		}

		if(method != null)
		{
			ParameterInfo[] parameterArray = method.GetParameters();
			for(int paramIndex = 0; paramArray != null && paramIndex < paramArray.Length; paramIndex ++)
			{
				if(paramArray[paramIndex].GetType().ToString() == "LuaInterface.LuaFunction") // lua function
				{
					object[] obA = WindowBaseLua.m_luaMgr.CallLuaFunction("register_func", new []{paramArray[paramIndex]});
					int luaIndex = int.Parse(obA[0].ToString());
					CLuaCallBack luaCallBack = new CLuaCallBack(luaIndex);
					m_callBackList.Add(luaIndex);
					Type type = parameterArray[paramIndex].ParameterType;
					paramArray[paramIndex] = luaCallBack.GetCallBack(type);
				}
			}
			
			method.Invoke(target, paramArray);
		}
	}

	public void ClearCallBack()
	{
		for(int callBackLsIndex = 0; callBackLsIndex < m_callBackList.Count; callBackLsIndex ++)
		{
			int funIndex = m_callBackList[callBackLsIndex];
			WindowBaseLua.m_luaMgr.CallLuaFunction("unregister_func", funIndex);
		}
		m_callBackList.Clear();
	}
}
