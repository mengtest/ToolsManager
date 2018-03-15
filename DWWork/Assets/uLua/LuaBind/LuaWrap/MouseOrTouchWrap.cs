using System;
using UnityEngine;
using LuaInterface;
using Object = UnityEngine.Object;

public class MouseOrTouchWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("New", _CreateMouseOrTouch),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("key", get_key, set_key),
		new LuaField("pos", get_pos, set_pos),
		new LuaField("lastPos", get_lastPos, set_lastPos),
		new LuaField("delta", get_delta, set_delta),
		new LuaField("totalDelta", get_totalDelta, set_totalDelta),
		new LuaField("pressedCam", get_pressedCam, set_pressedCam),
		new LuaField("last", get_last, set_last),
		new LuaField("current", get_current, set_current),
		new LuaField("pressed", get_pressed, set_pressed),
		new LuaField("dragged", get_dragged, set_dragged),
		new LuaField("pressTime", get_pressTime, set_pressTime),
		new LuaField("clickTime", get_clickTime, set_clickTime),
		new LuaField("clickNotification", get_clickNotification, set_clickNotification),
		new LuaField("touchBegan", get_touchBegan, set_touchBegan),
		new LuaField("pressStarted", get_pressStarted, set_pressStarted),
		new LuaField("dragStarted", get_dragStarted, set_dragStarted),
		new LuaField("ignoreDelta", get_ignoreDelta, set_ignoreDelta),
		new LuaField("deltaTime", get_deltaTime, null),
		new LuaField("isOverUI", get_isOverUI, null),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateMouseOrTouch(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			UICamera.MouseOrTouch obj = new UICamera.MouseOrTouch();
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: MouseOrTouch.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(UICamera.MouseOrTouch));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "UICamera.MouseOrTouch", typeof(UICamera.MouseOrTouch), regs, fields, "System.Object");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_key(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name key");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index key on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		LuaScriptMgr.PushEnum(L, obj.key);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_pos(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pos");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pos on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		LuaScriptMgr.PushValue(L, obj.pos);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_lastPos(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name lastPos");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index lastPos on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		LuaScriptMgr.PushValue(L, obj.lastPos);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_delta(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name delta");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index delta on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		LuaScriptMgr.PushValue(L, obj.delta);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_totalDelta(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name totalDelta");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index totalDelta on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		LuaScriptMgr.PushValue(L, obj.totalDelta);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_pressedCam(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pressedCam");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pressedCam on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		LuaScriptMgr.Push(L, obj.pressedCam);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_last(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name last");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index last on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		LuaScriptMgr.Push(L, obj.last);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_current(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name current");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index current on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		LuaScriptMgr.Push(L, obj.current);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_pressed(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pressed");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pressed on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		LuaScriptMgr.Push(L, obj.pressed);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_dragged(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name dragged");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index dragged on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		LuaScriptMgr.Push(L, obj.dragged);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_pressTime(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pressTime");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pressTime on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		LuaScriptMgr.Push(L, obj.pressTime);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_clickTime(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name clickTime");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index clickTime on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		LuaScriptMgr.Push(L, obj.clickTime);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_clickNotification(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name clickNotification");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index clickNotification on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		LuaScriptMgr.PushEnum(L, obj.clickNotification);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_touchBegan(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name touchBegan");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index touchBegan on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		LuaScriptMgr.Push(L, obj.touchBegan);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_pressStarted(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pressStarted");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pressStarted on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		LuaScriptMgr.Push(L, obj.pressStarted);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_dragStarted(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name dragStarted");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index dragStarted on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		LuaScriptMgr.Push(L, obj.dragStarted);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ignoreDelta(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name ignoreDelta");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index ignoreDelta on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		LuaScriptMgr.Push(L, obj.ignoreDelta);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_deltaTime(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name deltaTime");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index deltaTime on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		LuaScriptMgr.Push(L, obj.deltaTime);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_isOverUI(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name isOverUI");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index isOverUI on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		LuaScriptMgr.Push(L, obj.isOverUI);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_key(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name key");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index key on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		obj.key = LuaScriptMgr.GetNetObject<KeyCode>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_pos(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pos");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pos on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		obj.pos = LuaScriptMgr.GetNetObject<Vector2>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_lastPos(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name lastPos");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index lastPos on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		obj.lastPos = LuaScriptMgr.GetNetObject<Vector2>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_delta(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name delta");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index delta on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		obj.delta = LuaScriptMgr.GetNetObject<Vector2>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_totalDelta(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name totalDelta");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index totalDelta on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		obj.totalDelta = LuaScriptMgr.GetNetObject<Vector2>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_pressedCam(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pressedCam");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pressedCam on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		obj.pressedCam = LuaScriptMgr.GetNetObject<Camera>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_last(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name last");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index last on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		obj.last = LuaScriptMgr.GetNetObject<GameObject>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_current(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name current");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index current on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		obj.current = LuaScriptMgr.GetNetObject<GameObject>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_pressed(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pressed");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pressed on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		obj.pressed = LuaScriptMgr.GetNetObject<GameObject>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_dragged(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name dragged");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index dragged on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		obj.dragged = LuaScriptMgr.GetNetObject<GameObject>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_pressTime(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pressTime");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pressTime on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		obj.pressTime = (float)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_clickTime(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name clickTime");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index clickTime on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		obj.clickTime = (float)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_clickNotification(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name clickNotification");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index clickNotification on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		obj.clickNotification = LuaScriptMgr.GetNetObject<UICamera.ClickNotification>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_touchBegan(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name touchBegan");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index touchBegan on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		obj.touchBegan = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_pressStarted(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name pressStarted");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index pressStarted on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		obj.pressStarted = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_dragStarted(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name dragStarted");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index dragStarted on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		obj.dragStarted = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ignoreDelta(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name ignoreDelta");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index ignoreDelta on a nil value");
			}
		}

		UICamera.MouseOrTouch obj = (UICamera.MouseOrTouch)o;
		obj.ignoreDelta = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}
}

