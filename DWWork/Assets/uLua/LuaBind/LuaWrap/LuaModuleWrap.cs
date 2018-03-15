using System;
using LuaInterface;

public class LuaModuleWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("GetLuaObj", GetLuaObj),
		new LuaMethod("GetLuaObjForLua", GetLuaObjForLua),
		new LuaMethod("Destroy", Destroy),
		new LuaMethod("New", _CreateLuaModule),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("m_luaCallBackMgr", get_m_luaCallBackMgr, set_m_luaCallBackMgr),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateLuaModule(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			LuaModule obj = new LuaModule();
			LuaScriptMgr.Push(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: LuaModule.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(LuaModule));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "LuaModule", typeof(LuaModule), regs, fields, "ModuleRoot");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_luaCallBackMgr(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_luaCallBackMgr");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_luaCallBackMgr on a nil value");
			}
		}

		LuaModule obj = (LuaModule)o;
		LuaScriptMgr.PushObject(L, obj.m_luaCallBackMgr);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_luaCallBackMgr(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_luaCallBackMgr");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_luaCallBackMgr on a nil value");
			}
		}

		LuaModule obj = (LuaModule)o;
		obj.m_luaCallBackMgr = LuaScriptMgr.GetNetObject<CLuaCallBackMgr>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLuaObj(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaModule obj = LuaScriptMgr.GetNetObject<LuaModule>(L, 1);
		LuaObj o = obj.GetLuaObj();
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLuaObjForLua(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaModule obj = LuaScriptMgr.GetNetObject<LuaModule>(L, 1);
		LuaInterface.LuaTable o = obj.GetLuaObjForLua();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Destroy(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaModule obj = LuaScriptMgr.GetNetObject<LuaModule>(L, 1);
		obj.Destroy();
		return 0;
	}
}

