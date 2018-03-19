using System;
using LuaInterface;

public class ComponentBaseLuaWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("Init", Init),
		new LuaMethod("New", _CreateComponentBaseLua),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("m_luaMgr", get_m_luaMgr, set_m_luaMgr),
		new LuaField("m_fileName", get_m_fileName, set_m_fileName),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateComponentBaseLua(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			ComponentBaseLua obj = new ComponentBaseLua();
			LuaScriptMgr.Push(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: ComponentBaseLua.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(ComponentBaseLua));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "ComponentBaseLua", typeof(ComponentBaseLua), regs, fields, "BaseUI");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_luaMgr(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, ComponentBaseLua.m_luaMgr);
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

		ComponentBaseLua obj = (ComponentBaseLua)o;
		LuaScriptMgr.Push(L, obj.m_fileName);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_luaMgr(IntPtr L)
	{
		ComponentBaseLua.m_luaMgr = LuaScriptMgr.GetNetObject<LuaScriptMgr>(L, 3);
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

		ComponentBaseLua obj = (ComponentBaseLua)o;
		obj.m_fileName = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Init(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		ComponentBaseLua obj = LuaScriptMgr.GetNetObject<ComponentBaseLua>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		obj.Init(arg0,arg1);
		return 0;
	}
}

