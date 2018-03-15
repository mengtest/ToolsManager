using System;
using LuaInterface;

public class EGameStateTypeWrap
{
	static LuaEnum[] enums = new LuaEnum[]
	{
		new LuaEnum("StateNone", EGameStateType.StateNone),
		new LuaEnum("LoginState", EGameStateType.LoginState),
		new LuaEnum("LoadingState", EGameStateType.LoadingState),
		new LuaEnum("MainCityState", EGameStateType.MainCityState),
		new LuaEnum("GameState", EGameStateType.GameState),
	};

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "EGameStateType", enums);
		LuaScriptMgr.RegisterFunc(L, "EGameStateType", IntToEnum, "IntToEnum");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IntToEnum(IntPtr L)
	{
		int arg0 = (int)LuaDLL.lua_tonumber(L, 1);
		EGameStateType o = (EGameStateType)arg0;
		LuaScriptMgr.PushEnum(L, o);
		return 1;
	}
}

