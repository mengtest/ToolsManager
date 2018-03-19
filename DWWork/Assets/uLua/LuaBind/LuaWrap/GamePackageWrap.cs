using System;
using LuaInterface;

public class GamePackageWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("New", _CreateGamePackage),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("areaID", get_areaID, set_areaID),
		new LuaField("cmd", get_cmd, set_cmd),
		new LuaField("seq", get_seq, set_seq),
		new LuaField("errno", get_errno, set_errno),
		new LuaField("body", get_body, set_body),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateGamePackage(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			GamePackage obj = new GamePackage();
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: GamePackage.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(GamePackage));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "GamePackage", typeof(GamePackage), regs, fields, "System.Object");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_areaID(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name areaID");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index areaID on a nil value");
			}
		}

		GamePackage obj = (GamePackage)o;
		LuaScriptMgr.Push(L, obj.areaID);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_cmd(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name cmd");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index cmd on a nil value");
			}
		}

		GamePackage obj = (GamePackage)o;
		LuaScriptMgr.Push(L, obj.cmd);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_seq(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name seq");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index seq on a nil value");
			}
		}

		GamePackage obj = (GamePackage)o;
		LuaScriptMgr.Push(L, obj.seq);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_errno(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name errno");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index errno on a nil value");
			}
		}

		GamePackage obj = (GamePackage)o;
		LuaScriptMgr.Push(L, obj.errno);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_body(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name body");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index body on a nil value");
			}
		}

		GamePackage obj = (GamePackage)o;
		LuaScriptMgr.PushArray(L, obj.body);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_areaID(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name areaID");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index areaID on a nil value");
			}
		}

		GamePackage obj = (GamePackage)o;
		obj.areaID = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_cmd(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name cmd");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index cmd on a nil value");
			}
		}

		GamePackage obj = (GamePackage)o;
		obj.cmd = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_seq(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name seq");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index seq on a nil value");
			}
		}

		GamePackage obj = (GamePackage)o;
		obj.seq = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_errno(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name errno");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index errno on a nil value");
			}
		}

		GamePackage obj = (GamePackage)o;
		obj.errno = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_body(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name body");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index body on a nil value");
			}
		}

		GamePackage obj = (GamePackage)o;
		obj.body = LuaScriptMgr.GetNetObject<Byte[]>(L, 3);
		return 0;
	}
}

