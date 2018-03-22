using System;
using LuaInterface;

public class ConstantsWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("InitValue", InitValue),
		new LuaMethod("New", _CreateConstants),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("NATIVE_IMPORT", get_NATIVE_IMPORT, null),
		new LuaField("CryptoIV", get_CryptoIV, set_CryptoIV),
		new LuaField("CryptoKey", get_CryptoKey, set_CryptoKey),
		new LuaField("RELEASE", get_RELEASE, set_RELEASE),
		new LuaField("WITH_RELEASE_EXP", get_WITH_RELEASE_EXP, set_WITH_RELEASE_EXP),
		new LuaField("USE_PRE_VERSION_2", get_USE_PRE_VERSION_2, set_USE_PRE_VERSION_2),
		new LuaField("APP_VERSION", get_APP_VERSION, set_APP_VERSION),
		new LuaField("RESS_VERSION", get_RESS_VERSION, set_RESS_VERSION),
		new LuaField("DATAEYE_ID", get_DATAEYE_ID, set_DATAEYE_ID),
		new LuaField("SERVICE_KEY", get_SERVICE_KEY, set_SERVICE_KEY),
		new LuaField("BUGLY_APPID", get_BUGLY_APPID, set_BUGLY_APPID),
		new LuaField("UPDATE_URL", get_UPDATE_URL, set_UPDATE_URL),
		new LuaField("UPDATE_CONFIG_FILE", get_UPDATE_CONFIG_FILE, set_UPDATE_CONFIG_FILE),
		new LuaField("M_IS_IOS_PRE", get_M_IS_IOS_PRE, set_M_IS_IOS_PRE),
		new LuaField("SCREEN_HEIGHT", get_SCREEN_HEIGHT, set_SCREEN_HEIGHT),
		new LuaField("FORCE_LOAD_AB", get_FORCE_LOAD_AB, set_FORCE_LOAD_AB),
		new LuaField("RUN_WITH_EN_LUA", get_RUN_WITH_EN_LUA, set_RUN_WITH_EN_LUA),
		new LuaField("FORCE_DEBUG_PLATFORM", get_FORCE_DEBUG_PLATFORM, set_FORCE_DEBUG_PLATFORM),
		new LuaField("EnableIM", get_EnableIM, set_EnableIM),
		new LuaField("LOCAL_UPDATE_URL", get_LOCAL_UPDATE_URL, set_LOCAL_UPDATE_URL),
		new LuaField("LOCAL_UPDATE_CONFIG_FILE", get_LOCAL_UPDATE_CONFIG_FILE, set_LOCAL_UPDATE_CONFIG_FILE),
		new LuaField("TEMP_CLOSE", get_TEMP_CLOSE, set_TEMP_CLOSE),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateConstants(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			Constants obj = new Constants();
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: Constants.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(Constants));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "Constants", typeof(Constants), regs, fields, "System.Object");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_NATIVE_IMPORT(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.NATIVE_IMPORT);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_CryptoIV(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.CryptoIV);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_CryptoKey(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.CryptoKey);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_RELEASE(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.RELEASE);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_WITH_RELEASE_EXP(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.WITH_RELEASE_EXP);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_USE_PRE_VERSION_2(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.USE_PRE_VERSION_2);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_APP_VERSION(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.APP_VERSION);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_RESS_VERSION(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.RESS_VERSION);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_DATAEYE_ID(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.DATAEYE_ID);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_SERVICE_KEY(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.SERVICE_KEY);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_BUGLY_APPID(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.BUGLY_APPID);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_UPDATE_URL(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.UPDATE_URL);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_UPDATE_CONFIG_FILE(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.UPDATE_CONFIG_FILE);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_M_IS_IOS_PRE(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.M_IS_IOS_PRE);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_SCREEN_HEIGHT(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.SCREEN_HEIGHT);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_FORCE_LOAD_AB(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.FORCE_LOAD_AB);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_RUN_WITH_EN_LUA(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.RUN_WITH_EN_LUA);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_FORCE_DEBUG_PLATFORM(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.FORCE_DEBUG_PLATFORM);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_EnableIM(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.EnableIM);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_LOCAL_UPDATE_URL(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.LOCAL_UPDATE_URL);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_LOCAL_UPDATE_CONFIG_FILE(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.LOCAL_UPDATE_CONFIG_FILE);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_TEMP_CLOSE(IntPtr L)
	{
		LuaScriptMgr.Push(L, Constants.TEMP_CLOSE);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_CryptoIV(IntPtr L)
	{
		Constants.CryptoIV = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_CryptoKey(IntPtr L)
	{
		Constants.CryptoKey = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_RELEASE(IntPtr L)
	{
		Constants.RELEASE = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_WITH_RELEASE_EXP(IntPtr L)
	{
		Constants.WITH_RELEASE_EXP = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_USE_PRE_VERSION_2(IntPtr L)
	{
		Constants.USE_PRE_VERSION_2 = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_APP_VERSION(IntPtr L)
	{
		Constants.APP_VERSION = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_RESS_VERSION(IntPtr L)
	{
		Constants.RESS_VERSION = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_DATAEYE_ID(IntPtr L)
	{
		Constants.DATAEYE_ID = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_SERVICE_KEY(IntPtr L)
	{
		Constants.SERVICE_KEY = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_BUGLY_APPID(IntPtr L)
	{
		Constants.BUGLY_APPID = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_UPDATE_URL(IntPtr L)
	{
		Constants.UPDATE_URL = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_UPDATE_CONFIG_FILE(IntPtr L)
	{
		Constants.UPDATE_CONFIG_FILE = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_M_IS_IOS_PRE(IntPtr L)
	{
		Constants.M_IS_IOS_PRE = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_SCREEN_HEIGHT(IntPtr L)
	{
		Constants.SCREEN_HEIGHT = (float)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_FORCE_LOAD_AB(IntPtr L)
	{
		Constants.FORCE_LOAD_AB = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_RUN_WITH_EN_LUA(IntPtr L)
	{
		Constants.RUN_WITH_EN_LUA = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_FORCE_DEBUG_PLATFORM(IntPtr L)
	{
		Constants.FORCE_DEBUG_PLATFORM = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_EnableIM(IntPtr L)
	{
		Constants.EnableIM = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_LOCAL_UPDATE_URL(IntPtr L)
	{
		Constants.LOCAL_UPDATE_URL = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_LOCAL_UPDATE_CONFIG_FILE(IntPtr L)
	{
		Constants.LOCAL_UPDATE_CONFIG_FILE = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_TEMP_CLOSE(IntPtr L)
	{
		Constants.TEMP_CLOSE = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitValue(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		Constants.InitValue();
		return 0;
	}
}

