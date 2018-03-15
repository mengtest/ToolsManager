using System;
using LuaInterface;

public class ModuleRootWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("BaseCreateModule", BaseCreateModule),
		new LuaMethod("BaseInitModule", BaseInitModule),
		new LuaMethod("SetScrollViewCanDrag", SetScrollViewCanDrag),
		new LuaMethod("Destroy", Destroy),
		new LuaMethod("New", _CreateModuleRoot),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("m_isCreated", get_m_isCreated, set_m_isCreated),
		new LuaField("windowRoot", get_windowRoot, null),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateModuleRoot(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			ModuleRoot obj = new ModuleRoot();
			LuaScriptMgr.Push(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: ModuleRoot.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(ModuleRoot));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "ModuleRoot", typeof(ModuleRoot), regs, fields, "BaseUI");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_isCreated(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_isCreated");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_isCreated on a nil value");
			}
		}

		ModuleRoot obj = (ModuleRoot)o;
		LuaScriptMgr.Push(L, obj.m_isCreated);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_windowRoot(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name windowRoot");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index windowRoot on a nil value");
			}
		}

		ModuleRoot obj = (ModuleRoot)o;
		LuaScriptMgr.Push(L, obj.windowRoot);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_isCreated(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_isCreated");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_isCreated on a nil value");
			}
		}

		ModuleRoot obj = (ModuleRoot)o;
		obj.m_isCreated = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int BaseCreateModule(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		ModuleRoot obj = LuaScriptMgr.GetNetObject<ModuleRoot>(L, 1);
		WindowRoot arg0 = LuaScriptMgr.GetNetObject<WindowRoot>(L, 2);
		ModuleInfo arg1 = LuaScriptMgr.GetNetObject<ModuleInfo>(L, 3);
		obj.BaseCreateModule(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int BaseInitModule(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		ModuleRoot obj = LuaScriptMgr.GetNetObject<ModuleRoot>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		obj.BaseInitModule(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetScrollViewCanDrag(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		ModuleRoot obj = LuaScriptMgr.GetNetObject<ModuleRoot>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		obj.SetScrollViewCanDrag(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Destroy(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		ModuleRoot obj = LuaScriptMgr.GetNetObject<ModuleRoot>(L, 1);
		obj.Destroy();
		return 0;
	}
}

