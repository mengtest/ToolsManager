using System;
using LuaInterface;

public class TestConfigWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("New", _CreateTestConfig),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("_UserID", get__UserID, set__UserID),
		new LuaField("_UserToken", get__UserToken, set__UserToken),
		new LuaField("_UserAccessToken", get__UserAccessToken, set__UserAccessToken),
		new LuaField("UserID", get_UserID, null),
		new LuaField("UserToken", get_UserToken, null),
		new LuaField("UserAccessToken", get_UserAccessToken, null),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateTestConfig(IntPtr L)
	{
		LuaDLL.luaL_error(L, "TestConfig class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(TestConfig));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "TestConfig", typeof(TestConfig), regs, fields, "UnityEngine.MonoBehaviour");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get__UserID(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name _UserID");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index _UserID on a nil value");
			}
		}

		TestConfig obj = (TestConfig)o;
		LuaScriptMgr.Push(L, obj._UserID);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get__UserToken(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name _UserToken");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index _UserToken on a nil value");
			}
		}

		TestConfig obj = (TestConfig)o;
		LuaScriptMgr.Push(L, obj._UserToken);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get__UserAccessToken(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name _UserAccessToken");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index _UserAccessToken on a nil value");
			}
		}

		TestConfig obj = (TestConfig)o;
		LuaScriptMgr.Push(L, obj._UserAccessToken);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_UserID(IntPtr L)
	{
		LuaScriptMgr.Push(L, TestConfig.UserID);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_UserToken(IntPtr L)
	{
		LuaScriptMgr.Push(L, TestConfig.UserToken);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_UserAccessToken(IntPtr L)
	{
		LuaScriptMgr.Push(L, TestConfig.UserAccessToken);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set__UserID(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name _UserID");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index _UserID on a nil value");
			}
		}

		TestConfig obj = (TestConfig)o;
		obj._UserID = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set__UserToken(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name _UserToken");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index _UserToken on a nil value");
			}
		}

		TestConfig obj = (TestConfig)o;
		obj._UserToken = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set__UserAccessToken(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name _UserAccessToken");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index _UserAccessToken on a nil value");
			}
		}

		TestConfig obj = (TestConfig)o;
		obj._UserAccessToken = LuaScriptMgr.GetString(L, 3);
		return 0;
	}
}

