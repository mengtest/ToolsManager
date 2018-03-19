using System;
using UnityEngine;
using LuaInterface;
using Object = UnityEngine.Object;

public class WindowBaseLuaWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("InitLuaFile", InitLuaFile),
		new LuaMethod("InitWindow", InitWindow),
		new LuaMethod("InitParam", InitParam),
		new LuaMethod("OnFocus", OnFocus),
		new LuaMethod("PreCreateWindow", PreCreateWindow),
		new LuaMethod("CreateWindow", CreateWindow),
		new LuaMethod("SendWidgetClick", SendWidgetClick),
		new LuaMethod("SendWidgetDrag", SendWidgetDrag),
		new LuaMethod("SendWidgetPress", SendWidgetPress),
		new LuaMethod("SendWidgetRelease", SendWidgetRelease),
		new LuaMethod("Update", Update),
		new LuaMethod("OnDestroy", OnDestroy),
		new LuaMethod("SetScrollViewCanDrag", SetScrollViewCanDrag),
		new LuaMethod("GetValueFromLua", GetValueFromLua),
		new LuaMethod("New", _CreateWindowBaseLua),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("m_luaMgr", get_m_luaMgr, set_m_luaMgr),
		new LuaField("m_fileName", get_m_fileName, set_m_fileName),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateWindowBaseLua(IntPtr L)
	{
		LuaDLL.luaL_error(L, "WindowBaseLua class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(WindowBaseLua));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "WindowBaseLua", typeof(WindowBaseLua), regs, fields, "UnityEngine.MonoBehaviour");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_luaMgr(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, WindowBaseLua.m_luaMgr);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_fileName(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_fileName");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_fileName on a nil value");
			}
		}

		WindowBaseLua obj = (WindowBaseLua)o;
		LuaScriptMgr.Push(L, obj.m_fileName);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_luaMgr(IntPtr L)
	{
		WindowBaseLua.m_luaMgr = LuaScriptMgr.GetNetObject<LuaScriptMgr>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_fileName(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_fileName");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_fileName on a nil value");
			}
		}

		WindowBaseLua obj = (WindowBaseLua)o;
		obj.m_fileName = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitLuaFile(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 5);
		WindowBaseLua obj = LuaScriptMgr.GetNetObject<WindowBaseLua>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		string arg1 = LuaScriptMgr.GetLuaString(L, 3);
		WindowRoot arg2 = LuaScriptMgr.GetNetObject<WindowRoot>(L, 4);
		bool arg3 = LuaScriptMgr.GetBoolean(L, 5);
		obj.InitLuaFile(arg0,arg1,arg2,arg3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitWindow(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		WindowBaseLua obj = LuaScriptMgr.GetNetObject<WindowBaseLua>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		object arg2 = LuaScriptMgr.GetVarObject(L, 4);
		obj.InitWindow(arg0,arg1,arg2);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitParam(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		WindowBaseLua obj = LuaScriptMgr.GetNetObject<WindowBaseLua>(L, 1);
		object arg0 = LuaScriptMgr.GetVarObject(L, 2);
		obj.InitParam(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnFocus(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		WindowBaseLua obj = LuaScriptMgr.GetNetObject<WindowBaseLua>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		obj.OnFocus(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PreCreateWindow(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		WindowBaseLua obj = LuaScriptMgr.GetNetObject<WindowBaseLua>(L, 1);
		obj.PreCreateWindow();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CreateWindow(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		WindowBaseLua obj = LuaScriptMgr.GetNetObject<WindowBaseLua>(L, 1);
		obj.CreateWindow();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SendWidgetClick(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		WindowBaseLua obj = LuaScriptMgr.GetNetObject<WindowBaseLua>(L, 1);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 2);
		obj.SendWidgetClick(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SendWidgetDrag(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		WindowBaseLua obj = LuaScriptMgr.GetNetObject<WindowBaseLua>(L, 1);
		Vector2 arg0 = LuaScriptMgr.GetNetObject<Vector2>(L, 2);
		obj.SendWidgetDrag(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SendWidgetPress(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		WindowBaseLua obj = LuaScriptMgr.GetNetObject<WindowBaseLua>(L, 1);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		obj.SendWidgetPress(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SendWidgetRelease(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		WindowBaseLua obj = LuaScriptMgr.GetNetObject<WindowBaseLua>(L, 1);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		obj.SendWidgetRelease(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Update(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		WindowBaseLua obj = LuaScriptMgr.GetNetObject<WindowBaseLua>(L, 1);
		obj.Update();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnDestroy(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		WindowBaseLua obj = LuaScriptMgr.GetNetObject<WindowBaseLua>(L, 1);
		obj.OnDestroy();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetScrollViewCanDrag(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		WindowBaseLua obj = LuaScriptMgr.GetNetObject<WindowBaseLua>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		obj.SetScrollViewCanDrag(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetValueFromLua(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		bool arg2 = LuaScriptMgr.GetBoolean(L, 3);
		object[] objs3 = LuaScriptMgr.GetParamsObject(L, 4, count - 3);
		object[] o = WindowBaseLua.GetValueFromLua(arg0,arg1,arg2,objs3);
		LuaScriptMgr.PushArray(L, o);
		return 1;
	}
}

