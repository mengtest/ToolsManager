using System;
using LuaInterface;

public class CLuaCallBackMgrWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("AddCallBack", AddCallBack),
		new LuaMethod("ClearCallBack", ClearCallBack),
		new LuaMethod("New", _CreateCLuaCallBackMgr),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateCLuaCallBackMgr(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			CLuaCallBackMgr obj = new CLuaCallBackMgr();
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: CLuaCallBackMgr.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(CLuaCallBackMgr));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "CLuaCallBackMgr", typeof(CLuaCallBackMgr), regs, fields, "System.Object");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddCallBack(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);
		CLuaCallBackMgr obj = LuaScriptMgr.GetNetObject<CLuaCallBackMgr>(L, 1);
		object arg0 = LuaScriptMgr.GetVarObject(L, 2);
		string arg1 = LuaScriptMgr.GetLuaString(L, 3);
		object[] objs2 = LuaScriptMgr.GetParamsObject(L, 4, count - 3);
		obj.AddCallBack(arg0,arg1,objs2);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ClearCallBack(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		CLuaCallBackMgr obj = LuaScriptMgr.GetNetObject<CLuaCallBackMgr>(L, 1);
		obj.ClearCallBack();
		return 0;
	}
}

