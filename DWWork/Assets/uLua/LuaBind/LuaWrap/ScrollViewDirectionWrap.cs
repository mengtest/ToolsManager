using System;
using LuaInterface;

public class ScrollViewDirectionWrap
{
	static LuaEnum[] enums = new LuaEnum[]
	{
		new LuaEnum("SVD_Horizontal", ScrollViewDirection.SVD_Horizontal),
		new LuaEnum("SVD_Vertical", ScrollViewDirection.SVD_Vertical),
	};

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "ScrollViewDirection", enums);
		LuaScriptMgr.RegisterFunc(L, "ScrollViewDirection", IntToEnum, "IntToEnum");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IntToEnum(IntPtr L)
	{
		int arg0 = (int)LuaDLL.lua_tonumber(L, 1);
		ScrollViewDirection o = (ScrollViewDirection)arg0;
		LuaScriptMgr.PushEnum(L, o);
		return 1;
	}
}

