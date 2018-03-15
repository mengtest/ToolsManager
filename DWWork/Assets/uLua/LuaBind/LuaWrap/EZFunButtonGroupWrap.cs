using System;
using UnityEngine;
using LuaInterface;

public class EZFunButtonGroupWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("AddItem", AddItem),
		new LuaMethod("ClickButton", ClickButton),
		new LuaMethod("GetLastClick", GetLastClick),
		new LuaMethod("New", _CreateEZFunButtonGroup),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateEZFunButtonGroup(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			EZFunButtonGroup obj = new EZFunButtonGroup();
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else if (count == 3)
		{
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 2);
			bool arg2 = LuaScriptMgr.GetBoolean(L, 3);
			EZFunButtonGroup obj = new EZFunButtonGroup(arg0,arg1,arg2);
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: EZFunButtonGroup.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(EZFunButtonGroup));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "EZFunButtonGroup", typeof(EZFunButtonGroup), regs, fields, "System.Object");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddItem(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 2)
		{
			EZFunButtonGroup obj = LuaScriptMgr.GetNetObject<EZFunButtonGroup>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			obj.AddItem(arg0);
			return 0;
		}
		else if (count == 3)
		{
			EZFunButtonGroup obj = LuaScriptMgr.GetNetObject<EZFunButtonGroup>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			Transform arg1 = LuaScriptMgr.GetNetObject<Transform>(L, 3);
			obj.AddItem(arg0,arg1);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: EZFunButtonGroup.AddItem");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ClickButton(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		EZFunButtonGroup obj = LuaScriptMgr.GetNetObject<EZFunButtonGroup>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		bool o = obj.ClickButton(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLastClick(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZFunButtonGroup obj = LuaScriptMgr.GetNetObject<EZFunButtonGroup>(L, 1);
		Transform o = obj.GetLastClick();
		LuaScriptMgr.Push(L, o);
		return 1;
	}
}

