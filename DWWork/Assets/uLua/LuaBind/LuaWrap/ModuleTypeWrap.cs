using System;
using LuaInterface;

public class ModuleTypeWrap
{
	static LuaEnum[] enums = new LuaEnum[]
	{
		new LuaEnum("LuaModuleType", ModuleType.LuaModuleType),
	};

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "ModuleType", enums);
		LuaScriptMgr.RegisterFunc(L, "ModuleType", IntToEnum, "IntToEnum");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IntToEnum(IntPtr L)
	{
		int arg0 = (int)LuaDLL.lua_tonumber(L, 1);
		ModuleType o = (ModuleType)arg0;
		LuaScriptMgr.PushEnum(L, o);
		return 1;
	}
}

