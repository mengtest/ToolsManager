using System;
using UnityEngine;
using LuaInterface;
using Object = UnityEngine.Object;

public class LuaWindowRootWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("InitLua", InitLua),
		new LuaMethod("InitWindow", InitWindow),
		new LuaMethod("BaseIniwWindow", BaseIniwWindow),
		new LuaMethod("GetName", GetName),
		new LuaMethod("TopBarCreateWindow", TopBarCreateWindow),
		new LuaMethod("TopBarHandleWidgetClick", TopBarHandleWidgetClick),
		new LuaMethod("OnDestroy", OnDestroy),
		new LuaMethod("SetScrollViewCanDrag", SetScrollViewCanDrag),
		new LuaMethod("SetOpenWinFunction", SetOpenWinFunction),
		new LuaMethod("New", _CreateLuaWindowRoot),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateLuaWindowRoot(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			LuaWindowRoot obj = new LuaWindowRoot();
			LuaScriptMgr.Push(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: LuaWindowRoot.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(LuaWindowRoot));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "LuaWindowRoot", typeof(LuaWindowRoot), regs, fields, "WindowRoot");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitLua(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		LuaWindowRoot obj = LuaScriptMgr.GetNetObject<LuaWindowRoot>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		obj.InitLua(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitWindow(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 3)
		{
			LuaWindowRoot obj = LuaScriptMgr.GetNetObject<LuaWindowRoot>(L, 1);
			bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
			obj.InitWindow(arg0,arg1);
			return 0;
		}
		else if (count == 4)
		{
			LuaWindowRoot obj = LuaScriptMgr.GetNetObject<LuaWindowRoot>(L, 1);
			bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
			object arg2 = LuaScriptMgr.GetVarObject(L, 4);
			obj.InitWindow(arg0,arg1,arg2);
			return 0;
		}
		else if (count == 5)
		{
			LuaWindowRoot obj = LuaScriptMgr.GetNetObject<LuaWindowRoot>(L, 1);
			bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
			bool arg2 = LuaScriptMgr.GetBoolean(L, 4);
			object arg3 = LuaScriptMgr.GetVarObject(L, 5);
			obj.InitWindow(arg0,arg1,arg2,arg3);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: LuaWindowRoot.InitWindow");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int BaseIniwWindow(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		LuaWindowRoot obj = LuaScriptMgr.GetNetObject<LuaWindowRoot>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		obj.BaseIniwWindow(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetName(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaWindowRoot obj = LuaScriptMgr.GetNetObject<LuaWindowRoot>(L, 1);
		string o = obj.GetName();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int TopBarCreateWindow(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaWindowRoot obj = LuaScriptMgr.GetNetObject<LuaWindowRoot>(L, 1);
		obj.TopBarCreateWindow();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int TopBarHandleWidgetClick(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		LuaWindowRoot obj = LuaScriptMgr.GetNetObject<LuaWindowRoot>(L, 1);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 2);
		obj.TopBarHandleWidgetClick(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnDestroy(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaWindowRoot obj = LuaScriptMgr.GetNetObject<LuaWindowRoot>(L, 1);
		obj.OnDestroy();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetScrollViewCanDrag(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		LuaWindowRoot obj = LuaScriptMgr.GetNetObject<LuaWindowRoot>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		obj.SetScrollViewCanDrag(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetOpenWinFunction(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		LuaWindowRoot obj = LuaScriptMgr.GetNetObject<LuaWindowRoot>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		obj.SetOpenWinFunction(arg0);
		return 0;
	}
}

