using System;
using LuaInterface;

public class AdaptTypeWrap
{
	static LuaEnum[] enums = new LuaEnum[]
	{
		new LuaEnum("none", AdaptType.none),
		new LuaEnum("normal", AdaptType.normal),
		new LuaEnum("rigid", AdaptType.rigid),
	};

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "AdaptType", enums);
		LuaScriptMgr.RegisterFunc(L, "AdaptType", IntToEnum, "IntToEnum");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IntToEnum(IntPtr L)
	{
		int arg0 = (int)LuaDLL.lua_tonumber(L, 1);
		AdaptType o = (AdaptType)arg0;
		LuaScriptMgr.PushEnum(L, o);
		return 1;
	}
}

