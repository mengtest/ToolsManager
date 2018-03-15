using System;
using LuaInterface;

public class EZFunWindowEnumWrap
{
	static LuaEnum[] enums = new LuaEnum[]
	{
		new LuaEnum("None", EZFunWindowEnum.None),
		new LuaEnum("cover_ui_window", EZFunWindowEnum.cover_ui_window),
		new LuaEnum("loading_ui_window", EZFunWindowEnum.loading_ui_window),
		new LuaEnum("FULL_WINDOW_SPLIT", EZFunWindowEnum.FULL_WINDOW_SPLIT),
		new LuaEnum("wait_ui_window", EZFunWindowEnum.wait_ui_window),
		new LuaEnum("error_ui_window", EZFunWindowEnum.error_ui_window),
		new LuaEnum("err_tips_ui_window", EZFunWindowEnum.err_tips_ui_window),
		new LuaEnum("update_ui_window", EZFunWindowEnum.update_ui_window),
		new LuaEnum("reconnect_ui_window", EZFunWindowEnum.reconnect_ui_window),
		new LuaEnum("gm_ui_window", EZFunWindowEnum.gm_ui_window),
		new LuaEnum("luaWindow", EZFunWindowEnum.luaWindow),
	};

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "EZFunWindowEnum", enums);
		LuaScriptMgr.RegisterFunc(L, "EZFunWindowEnum", IntToEnum, "IntToEnum");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IntToEnum(IntPtr L)
	{
		int arg0 = (int)LuaDLL.lua_tonumber(L, 1);
		EZFunWindowEnum o = (EZFunWindowEnum)arg0;
		LuaScriptMgr.PushEnum(L, o);
		return 1;
	}
}

