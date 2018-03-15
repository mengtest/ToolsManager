using System;
using UnityEngine;
using LuaInterface;
using Object = UnityEngine.Object;

public class UIDraggableCameraWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("CalculateConstrainOffset", CalculateConstrainOffset),
		new LuaMethod("ConstrainToBounds", ConstrainToBounds),
		new LuaMethod("CenterOn", CenterOn),
		new LuaMethod("Stop", Stop),
		new LuaMethod("Press", Press),
		new LuaMethod("Drag", Drag),
		new LuaMethod("Scroll", Scroll),
		new LuaMethod("New", _CreateUIDraggableCamera),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("rootForBounds", get_rootForBounds, set_rootForBounds),
		new LuaField("scale", get_scale, set_scale),
		new LuaField("scrollWheelFactor", get_scrollWheelFactor, set_scrollWheelFactor),
		new LuaField("dragEffect", get_dragEffect, set_dragEffect),
		new LuaField("smoothDragStart", get_smoothDragStart, set_smoothDragStart),
		new LuaField("momentumAmount", get_momentumAmount, set_momentumAmount),
		new LuaField("onfinished", get_onfinished, set_onfinished),
		new LuaField("currentMomentum", get_currentMomentum, set_currentMomentum),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateUIDraggableCamera(IntPtr L)
	{
		LuaDLL.luaL_error(L, "UIDraggableCamera class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(UIDraggableCamera));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "UIDraggableCamera", typeof(UIDraggableCamera), regs, fields, "UnityEngine.MonoBehaviour");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_rootForBounds(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name rootForBounds");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index rootForBounds on a nil value");
			}
		}

		UIDraggableCamera obj = (UIDraggableCamera)o;
		LuaScriptMgr.Push(L, obj.rootForBounds);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_scale(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name scale");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index scale on a nil value");
			}
		}

		UIDraggableCamera obj = (UIDraggableCamera)o;
		LuaScriptMgr.PushValue(L, obj.scale);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_scrollWheelFactor(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name scrollWheelFactor");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index scrollWheelFactor on a nil value");
			}
		}

		UIDraggableCamera obj = (UIDraggableCamera)o;
		LuaScriptMgr.Push(L, obj.scrollWheelFactor);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_dragEffect(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name dragEffect");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index dragEffect on a nil value");
			}
		}

		UIDraggableCamera obj = (UIDraggableCamera)o;
		LuaScriptMgr.PushEnum(L, obj.dragEffect);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_smoothDragStart(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name smoothDragStart");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index smoothDragStart on a nil value");
			}
		}

		UIDraggableCamera obj = (UIDraggableCamera)o;
		LuaScriptMgr.Push(L, obj.smoothDragStart);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_momentumAmount(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name momentumAmount");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index momentumAmount on a nil value");
			}
		}

		UIDraggableCamera obj = (UIDraggableCamera)o;
		LuaScriptMgr.Push(L, obj.momentumAmount);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_onfinished(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name onfinished");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index onfinished on a nil value");
			}
		}

		UIDraggableCamera obj = (UIDraggableCamera)o;
		LuaScriptMgr.PushObject(L, obj.onfinished);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_currentMomentum(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name currentMomentum");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index currentMomentum on a nil value");
			}
		}

		UIDraggableCamera obj = (UIDraggableCamera)o;
		LuaScriptMgr.PushValue(L, obj.currentMomentum);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_rootForBounds(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name rootForBounds");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index rootForBounds on a nil value");
			}
		}

		UIDraggableCamera obj = (UIDraggableCamera)o;
		obj.rootForBounds = LuaScriptMgr.GetNetObject<Transform>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_scale(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name scale");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index scale on a nil value");
			}
		}

		UIDraggableCamera obj = (UIDraggableCamera)o;
		obj.scale = LuaScriptMgr.GetNetObject<Vector2>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_scrollWheelFactor(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name scrollWheelFactor");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index scrollWheelFactor on a nil value");
			}
		}

		UIDraggableCamera obj = (UIDraggableCamera)o;
		obj.scrollWheelFactor = (float)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_dragEffect(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name dragEffect");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index dragEffect on a nil value");
			}
		}

		UIDraggableCamera obj = (UIDraggableCamera)o;
		obj.dragEffect = LuaScriptMgr.GetNetObject<UIDragObject.DragEffect>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_smoothDragStart(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name smoothDragStart");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index smoothDragStart on a nil value");
			}
		}

		UIDraggableCamera obj = (UIDraggableCamera)o;
		obj.smoothDragStart = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_momentumAmount(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name momentumAmount");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index momentumAmount on a nil value");
			}
		}

		UIDraggableCamera obj = (UIDraggableCamera)o;
		obj.momentumAmount = (float)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_onfinished(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name onfinished");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index onfinished on a nil value");
			}
		}

		UIDraggableCamera obj = (UIDraggableCamera)o;
		obj.onfinished = LuaScriptMgr.GetNetObject<UIDraggableCamera.OnStop>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_currentMomentum(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name currentMomentum");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index currentMomentum on a nil value");
			}
		}

		UIDraggableCamera obj = (UIDraggableCamera)o;
		obj.currentMomentum = LuaScriptMgr.GetNetObject<Vector2>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CalculateConstrainOffset(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		UIDraggableCamera obj = LuaScriptMgr.GetNetObject<UIDraggableCamera>(L, 1);
		Vector3 o = obj.CalculateConstrainOffset();
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ConstrainToBounds(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UIDraggableCamera obj = LuaScriptMgr.GetNetObject<UIDraggableCamera>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		bool o = obj.ConstrainToBounds(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CenterOn(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UIDraggableCamera obj = LuaScriptMgr.GetNetObject<UIDraggableCamera>(L, 1);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 2);
		obj.CenterOn(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Stop(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		UIDraggableCamera obj = LuaScriptMgr.GetNetObject<UIDraggableCamera>(L, 1);
		obj.Stop();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Press(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UIDraggableCamera obj = LuaScriptMgr.GetNetObject<UIDraggableCamera>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		obj.Press(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Drag(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UIDraggableCamera obj = LuaScriptMgr.GetNetObject<UIDraggableCamera>(L, 1);
		Vector2 arg0 = LuaScriptMgr.GetNetObject<Vector2>(L, 2);
		obj.Drag(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Scroll(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UIDraggableCamera obj = LuaScriptMgr.GetNetObject<UIDraggableCamera>(L, 1);
		float arg0 = (float)LuaScriptMgr.GetNumber(L, 2);
		obj.Scroll(arg0);
		return 0;
	}
}

