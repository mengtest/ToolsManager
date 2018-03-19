using System;
using LuaInterface;

public class UILuaExtendsWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("SetLuaOnDrag", SetLuaOnDrag),
		new LuaMethod("SetLuaOnDragEnd", SetLuaOnDragEnd),
		new LuaMethod("New", _CreateUILuaExtends),
		new LuaMethod("GetClassType", GetClassType),
	};


	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateUILuaExtends(IntPtr L)
	{
		LuaDLL.luaL_error(L, "UILuaExtends class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(UILuaExtends));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "UILuaExtends", regs);
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLuaOnDrag(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UIEventListener arg0 = LuaScriptMgr.GetNetObject<UIEventListener>(L, 1);
		LuaFunction arg1 = LuaScriptMgr.GetLuaFunction(L, 2);
		UILuaExtends.SetLuaOnDrag(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLuaOnDragEnd(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UIEventListener arg0 = LuaScriptMgr.GetNetObject<UIEventListener>(L, 1);
		LuaFunction arg1 = LuaScriptMgr.GetLuaFunction(L, 2);
		UILuaExtends.SetLuaOnDragEnd(arg0,arg1);
		return 0;
	}
}

