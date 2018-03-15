using System;
using LuaInterface;

public class RessTypeWrap
{
	static LuaEnum[] enums = new LuaEnum[]
	{
		new LuaEnum("RT_Window", RessType.RT_Window),
		new LuaEnum("RT_UIItem", RessType.RT_UIItem),
		new LuaEnum("RT_UICamera", RessType.RT_UICamera),
		new LuaEnum("RT_Emoji", RessType.RT_Emoji),
		new LuaEnum("RT_Animation", RessType.RT_Animation),
		new LuaEnum("RT_CommonWindow", RessType.RT_CommonWindow),
		new LuaEnum("RT_GuideUIItem", RessType.RT_GuideUIItem),
		new LuaEnum("RT_FightUIItem", RessType.RT_FightUIItem),
		new LuaEnum("RT_CommonUItem", RessType.RT_CommonUItem),
		new LuaEnum("RT_LoadingUI", RessType.RT_LoadingUI),
		new LuaEnum("RT_None", RessType.RT_None),
	};

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "RessType", enums);
		LuaScriptMgr.RegisterFunc(L, "RessType", IntToEnum, "IntToEnum");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IntToEnum(IntPtr L)
	{
		int arg0 = (int)LuaDLL.lua_tonumber(L, 1);
		RessType o = (RessType)arg0;
		LuaScriptMgr.PushEnum(L, o);
		return 1;
	}
}

