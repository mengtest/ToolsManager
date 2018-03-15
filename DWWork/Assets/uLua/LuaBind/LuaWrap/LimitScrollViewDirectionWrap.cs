using System;
using LuaInterface;

public class LimitScrollViewDirectionWrap
{
	static LuaEnum[] enums = new LuaEnum[]
	{
		new LuaEnum("SVD_Horizontal", LimitScrollViewDirection.SVD_Horizontal),
		new LuaEnum("SVD_Vertical", LimitScrollViewDirection.SVD_Vertical),
	};

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "LimitScrollViewDirection", enums);
		LuaScriptMgr.RegisterFunc(L, "LimitScrollViewDirection", IntToEnum, "IntToEnum");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IntToEnum(IntPtr L)
	{
		int arg0 = (int)LuaDLL.lua_tonumber(L, 1);
		LimitScrollViewDirection o = (LimitScrollViewDirection)arg0;
		LuaScriptMgr.PushEnum(L, o);
		return 1;
	}
}

