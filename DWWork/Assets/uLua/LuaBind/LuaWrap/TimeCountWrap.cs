using System;
using LuaInterface;

public class TimeCountWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("New", _CreateTimeCount),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("m_isRealTime", get_m_isRealTime, set_m_isRealTime),
		new LuaField("Value", get_Value, set_Value),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateTimeCount(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			TimeCount obj = new TimeCount();
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else if (count == 1)
		{
			bool arg0 = LuaScriptMgr.GetBoolean(L, 1);
			TimeCount obj = new TimeCount(arg0);
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: TimeCount.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(TimeCount));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "TimeCount", typeof(TimeCount), regs, fields, "System.Object");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_isRealTime(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_isRealTime");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_isRealTime on a nil value");
			}
		}

		TimeCount obj = (TimeCount)o;
		LuaScriptMgr.Push(L, obj.m_isRealTime);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Value(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name Value");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index Value on a nil value");
			}
		}

		TimeCount obj = (TimeCount)o;
		LuaScriptMgr.Push(L, obj.Value);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_isRealTime(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_isRealTime");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_isRealTime on a nil value");
			}
		}

		TimeCount obj = (TimeCount)o;
		obj.m_isRealTime = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_Value(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name Value");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index Value on a nil value");
			}
		}

		TimeCount obj = (TimeCount)o;
		obj.Value = (double)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}
}

