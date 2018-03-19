using System;
using LuaInterface;

public class EventSysWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("Init", Init),
		new LuaMethod("Release", Release),
		new LuaMethod("Update", Update),
		new LuaMethod("AddHandler", AddHandler),
		new LuaMethod("RemoveHandleByEventType", RemoveHandleByEventType),
		new LuaMethod("RemoveHander", RemoveHander),
		new LuaMethod("AddEvent", AddEvent),
		new LuaMethod("AddEventNow", AddEventNow),
		new LuaMethod("New", _CreateEventSys),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateEventSys(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			EventSys obj = new EventSys();
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: EventSys.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(EventSys));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "EventSys", typeof(EventSys), regs, fields, "TCoreSystem`1[EventSys]");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Init(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EventSys obj = LuaScriptMgr.GetNetObject<EventSys>(L, 1);
		obj.Init();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Release(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EventSys obj = LuaScriptMgr.GetNetObject<EventSys>(L, 1);
		obj.Release();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Update(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EventSys obj = LuaScriptMgr.GetNetObject<EventSys>(L, 1);
		obj.Update();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddHandler(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		EventSys obj = LuaScriptMgr.GetNetObject<EventSys>(L, 1);
		EEventType arg0 = LuaScriptMgr.GetNetObject<EEventType>(L, 2);
		EventSysCallBack arg1 = LuaScriptMgr.GetNetObject<EventSysCallBack>(L, 3);
		obj.AddHandler(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RemoveHandleByEventType(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		EventSys obj = LuaScriptMgr.GetNetObject<EventSys>(L, 1);
		EEventType arg0 = LuaScriptMgr.GetNetObject<EEventType>(L, 2);
		EventSysCallBack arg1 = LuaScriptMgr.GetNetObject<EventSysCallBack>(L, 3);
		obj.RemoveHandleByEventType(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RemoveHander(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		EventSys obj = LuaScriptMgr.GetNetObject<EventSys>(L, 1);
		object arg0 = LuaScriptMgr.GetVarObject(L, 2);
		obj.RemoveHander(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddEvent(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		EventSys obj = LuaScriptMgr.GetNetObject<EventSys>(L, 1);
		EEventType arg0 = LuaScriptMgr.GetNetObject<EEventType>(L, 2);
		object arg1 = LuaScriptMgr.GetVarObject(L, 3);
		object arg2 = LuaScriptMgr.GetVarObject(L, 4);
		obj.AddEvent(arg0,arg1,arg2);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddEventNow(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		EventSys obj = LuaScriptMgr.GetNetObject<EventSys>(L, 1);
		EEventType arg0 = LuaScriptMgr.GetNetObject<EEventType>(L, 2);
		object arg1 = LuaScriptMgr.GetVarObject(L, 3);
		object arg2 = LuaScriptMgr.GetVarObject(L, 4);
		obj.AddEventNow(arg0,arg1,arg2);
		return 0;
	}
}

