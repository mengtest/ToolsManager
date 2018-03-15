using System;
using LuaInterface;

public class RessStorgeTypeWrap
{
	static LuaEnum[] enums = new LuaEnum[]
	{
		new LuaEnum("RST_Never", RessStorgeType.RST_Never),
		new LuaEnum("RST_Always", RessStorgeType.RST_Always),
		new LuaEnum("RST_NONE", RessStorgeType.RST_NONE),
	};

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "RessStorgeType", enums);
		LuaScriptMgr.RegisterFunc(L, "RessStorgeType", IntToEnum, "IntToEnum");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IntToEnum(IntPtr L)
	{
		int arg0 = (int)LuaDLL.lua_tonumber(L, 1);
		RessStorgeType o = (RessStorgeType)arg0;
		LuaScriptMgr.PushEnum(L, o);
		return 1;
	}
}

