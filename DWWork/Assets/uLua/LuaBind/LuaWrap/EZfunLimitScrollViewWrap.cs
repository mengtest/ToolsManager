using System;
using UnityEngine;
using LuaInterface;
using Object = UnityEngine.Object;

public class EZfunLimitScrollViewWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("Init", Init),
		new LuaMethod("InitForLua", InitForLua),
		new LuaMethod("GetOrAddLimitScr", GetOrAddLimitScr),
		new LuaMethod("SetInitItemCall", SetInitItemCall),
		new LuaMethod("SetInitItemCallLua", SetInitItemCallLua),
		new LuaMethod("SetHideItemCallLua", SetHideItemCallLua),
		new LuaMethod("SetSelectOrUnSelectFuncByName", SetSelectOrUnSelectFuncByName),
		new LuaMethod("SetSelectOrUnSelectFunc", SetSelectOrUnSelectFunc),
		new LuaMethod("SetDepth", SetDepth),
		new LuaMethod("ResetCameraBoxCollider", ResetCameraBoxCollider),
		new LuaMethod("UpdateCameraRect", UpdateCameraRect),
		new LuaMethod("InitItemCount", InitItemCount),
		new LuaMethod("InitScrollBar", InitScrollBar),
		new LuaMethod("SetLookPosition", SetLookPosition),
		new LuaMethod("SetCanDrag", SetCanDrag),
		new LuaMethod("SetCanClickItem", SetCanClickItem),
		new LuaMethod("getDraggableCamera", getDraggableCamera),
		new LuaMethod("InitDragTogglePage", InitDragTogglePage),
		new LuaMethod("InitDragTogglePageForLua", InitDragTogglePageForLua),
		new LuaMethod("RegisterPageFunctions", RegisterPageFunctions),
		new LuaMethod("InitPreLoadData", InitPreLoadData),
		new LuaMethod("InitPreLoadDataForLua", InitPreLoadDataForLua),
		new LuaMethod("ResetCameraRect", ResetCameraRect),
		new LuaMethod("GetUseTransformByIndex", GetUseTransformByIndex),
		new LuaMethod("New", _CreateEZfunLimitScrollView),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("m_layerGetType", get_m_layerGetType, set_m_layerGetType),
		new LuaField("m_specialScrollType", get_m_specialScrollType, set_m_specialScrollType),
		new LuaField("m_springToCallBack", get_m_springToCallBack, set_m_springToCallBack),
		new LuaField("m_isStableCamera", get_m_isStableCamera, set_m_isStableCamera),
		new LuaField("m_dataStartPageIndex", get_m_dataStartPageIndex, set_m_dataStartPageIndex),
		new LuaField("Direction", get_Direction, null),
		new LuaField("offestScrollViewSpringPosition", get_offestScrollViewSpringPosition, set_offestScrollViewSpringPosition),
		new LuaField("SelectIndex", get_SelectIndex, set_SelectIndex),
		new LuaField("StartIndex", get_StartIndex, set_StartIndex),
		new LuaField("EndIndex", get_EndIndex, set_EndIndex),
		new LuaField("FocusIndex", null, set_FocusIndex),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateEZfunLimitScrollView(IntPtr L)
	{
		LuaDLL.luaL_error(L, "EZfunLimitScrollView class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(EZfunLimitScrollView));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "EZfunLimitScrollView", typeof(EZfunLimitScrollView), regs, fields, "UnityEngine.MonoBehaviour");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_layerGetType(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_layerGetType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_layerGetType on a nil value");
			}
		}

		EZfunLimitScrollView obj = (EZfunLimitScrollView)o;
		LuaScriptMgr.Push(L, obj.m_layerGetType);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_specialScrollType(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_specialScrollType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_specialScrollType on a nil value");
			}
		}

		EZfunLimitScrollView obj = (EZfunLimitScrollView)o;
		LuaScriptMgr.Push(L, obj.m_specialScrollType);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_springToCallBack(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_springToCallBack");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_springToCallBack on a nil value");
			}
		}

		EZfunLimitScrollView obj = (EZfunLimitScrollView)o;
		LuaScriptMgr.PushObject(L, obj.m_springToCallBack);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_isStableCamera(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_isStableCamera");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_isStableCamera on a nil value");
			}
		}

		EZfunLimitScrollView obj = (EZfunLimitScrollView)o;
		LuaScriptMgr.Push(L, obj.m_isStableCamera);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_dataStartPageIndex(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_dataStartPageIndex");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_dataStartPageIndex on a nil value");
			}
		}

		EZfunLimitScrollView obj = (EZfunLimitScrollView)o;
		LuaScriptMgr.Push(L, obj.m_dataStartPageIndex);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Direction(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name Direction");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index Direction on a nil value");
			}
		}

		EZfunLimitScrollView obj = (EZfunLimitScrollView)o;
		LuaScriptMgr.PushEnum(L, obj.Direction);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_offestScrollViewSpringPosition(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name offestScrollViewSpringPosition");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index offestScrollViewSpringPosition on a nil value");
			}
		}

		EZfunLimitScrollView obj = (EZfunLimitScrollView)o;
		LuaScriptMgr.Push(L, obj.offestScrollViewSpringPosition);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_SelectIndex(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name SelectIndex");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index SelectIndex on a nil value");
			}
		}

		EZfunLimitScrollView obj = (EZfunLimitScrollView)o;
		LuaScriptMgr.Push(L, obj.SelectIndex);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_StartIndex(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name StartIndex");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index StartIndex on a nil value");
			}
		}

		EZfunLimitScrollView obj = (EZfunLimitScrollView)o;
		LuaScriptMgr.Push(L, obj.StartIndex);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_EndIndex(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name EndIndex");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index EndIndex on a nil value");
			}
		}

		EZfunLimitScrollView obj = (EZfunLimitScrollView)o;
		LuaScriptMgr.Push(L, obj.EndIndex);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_layerGetType(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_layerGetType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_layerGetType on a nil value");
			}
		}

		EZfunLimitScrollView obj = (EZfunLimitScrollView)o;
		obj.m_layerGetType = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_specialScrollType(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_specialScrollType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_specialScrollType on a nil value");
			}
		}

		EZfunLimitScrollView obj = (EZfunLimitScrollView)o;
		obj.m_specialScrollType = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_springToCallBack(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_springToCallBack");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_springToCallBack on a nil value");
			}
		}

		EZfunLimitScrollView obj = (EZfunLimitScrollView)o;
		obj.m_springToCallBack = LuaScriptMgr.GetNetObject<ItemShowCallBack>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_isStableCamera(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_isStableCamera");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_isStableCamera on a nil value");
			}
		}

		EZfunLimitScrollView obj = (EZfunLimitScrollView)o;
		obj.m_isStableCamera = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_dataStartPageIndex(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_dataStartPageIndex");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_dataStartPageIndex on a nil value");
			}
		}

		EZfunLimitScrollView obj = (EZfunLimitScrollView)o;
		obj.m_dataStartPageIndex = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_offestScrollViewSpringPosition(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name offestScrollViewSpringPosition");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index offestScrollViewSpringPosition on a nil value");
			}
		}

		EZfunLimitScrollView obj = (EZfunLimitScrollView)o;
		obj.offestScrollViewSpringPosition = (float)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_SelectIndex(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name SelectIndex");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index SelectIndex on a nil value");
			}
		}

		EZfunLimitScrollView obj = (EZfunLimitScrollView)o;
		obj.SelectIndex = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_StartIndex(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name StartIndex");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index StartIndex on a nil value");
			}
		}

		EZfunLimitScrollView obj = (EZfunLimitScrollView)o;
		obj.StartIndex = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_EndIndex(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name EndIndex");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index EndIndex on a nil value");
			}
		}

		EZfunLimitScrollView obj = (EZfunLimitScrollView)o;
		obj.EndIndex = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_FocusIndex(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name FocusIndex");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index FocusIndex on a nil value");
			}
		}

		EZfunLimitScrollView obj = (EZfunLimitScrollView)o;
		obj.FocusIndex = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Init(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 12);
		EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		GameObject arg1 = LuaScriptMgr.GetNetObject<GameObject>(L, 3);
		InitItemCall arg2 = LuaScriptMgr.GetNetObject<InitItemCall>(L, 4);
		SelectItemCall arg3 = LuaScriptMgr.GetNetObject<SelectItemCall>(L, 5);
		UnSelectItemCall arg4 = LuaScriptMgr.GetNetObject<UnSelectItemCall>(L, 6);
		Vector2 arg5 = LuaScriptMgr.GetNetObject<Vector2>(L, 7);
		Vector2 arg6 = LuaScriptMgr.GetNetObject<Vector2>(L, 8);
		LimitScrollViewDirection arg7 = LuaScriptMgr.GetNetObject<LimitScrollViewDirection>(L, 9);
		bool arg8 = LuaScriptMgr.GetBoolean(L, 10);
		ScrItemDrag arg9 = LuaScriptMgr.GetNetObject<ScrItemDrag>(L, 11);
		float arg10 = (float)LuaScriptMgr.GetNumber(L, 12);
		obj.Init(arg0,arg1,arg2,arg3,arg4,arg5,arg6,arg7,arg8,arg9,arg10);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitForLua(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 7);
		EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		GameObject arg1 = LuaScriptMgr.GetNetObject<GameObject>(L, 3);
		Vector2 arg2 = LuaScriptMgr.GetNetObject<Vector2>(L, 4);
		Vector2 arg3 = LuaScriptMgr.GetNetObject<Vector2>(L, 5);
		LimitScrollViewDirection arg4 = LuaScriptMgr.GetNetObject<LimitScrollViewDirection>(L, 6);
		bool arg5 = LuaScriptMgr.GetBoolean(L, 7);
		obj.InitForLua(arg0,arg1,arg2,arg3,arg4,arg5);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetOrAddLimitScr(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		EZfunLimitScrollView o = EZfunLimitScrollView.GetOrAddLimitScr(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetInitItemCall(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
		InitItemCall arg0 = LuaScriptMgr.GetNetObject<InitItemCall>(L, 2);
		obj.SetInitItemCall(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetInitItemCallLua(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(EZfunLimitScrollView), typeof(int)};
		Type[] types1 = {typeof(EZfunLimitScrollView), typeof(string)};

		if (count == 2 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
			int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
			obj.SetInitItemCallLua(arg0);
			return 0;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
			string arg0 = LuaScriptMgr.GetString(L, 2);
			obj.SetInitItemCallLua(arg0);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: EZfunLimitScrollView.SetInitItemCallLua");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetHideItemCallLua(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		obj.SetHideItemCallLua(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetSelectOrUnSelectFuncByName(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		obj.SetSelectOrUnSelectFuncByName(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetSelectOrUnSelectFunc(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		obj.SetSelectOrUnSelectFunc(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetDepth(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
		obj.SetDepth();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ResetCameraBoxCollider(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
		obj.ResetCameraBoxCollider();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UpdateCameraRect(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
		float arg0 = (float)LuaScriptMgr.GetNumber(L, 2);
		obj.UpdateCameraRect(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitItemCount(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		obj.InitItemCount(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitScrollBar(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
		UIScrollBar arg0 = LuaScriptMgr.GetNetObject<UIScrollBar>(L, 2);
		obj.InitScrollBar(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLookPosition(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
		Vector3 arg0 = LuaScriptMgr.GetNetObject<Vector3>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		obj.SetLookPosition(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetCanDrag(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		obj.SetCanDrag(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetCanClickItem(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		string arg1 = LuaScriptMgr.GetLuaString(L, 3);
		obj.SetCanClickItem(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int getDraggableCamera(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
		UIDraggableCamera o = obj.getDraggableCamera();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitDragTogglePage(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
		float arg0 = (float)LuaScriptMgr.GetNumber(L, 2);
		Action<int> arg1 = LuaScriptMgr.GetNetObject<Action<int>>(L, 3);
		obj.InitDragTogglePage(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitDragTogglePageForLua(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
		float arg0 = (float)LuaScriptMgr.GetNumber(L, 2);
		string arg1 = LuaScriptMgr.GetLuaString(L, 3);
		obj.InitDragTogglePageForLua(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RegisterPageFunctions(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 7);
		EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		int arg2 = (int)LuaScriptMgr.GetNumber(L, 4);
		string arg3 = LuaScriptMgr.GetLuaString(L, 5);
		string arg4 = LuaScriptMgr.GetLuaString(L, 6);
		string arg5 = LuaScriptMgr.GetLuaString(L, 7);
		obj.RegisterPageFunctions(arg0,arg1,arg2,arg3,arg4,arg5);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitPreLoadData(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		Action arg1 = LuaScriptMgr.GetNetObject<Action>(L, 3);
		obj.InitPreLoadData(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitPreLoadDataForLua(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(EZfunLimitScrollView), typeof(int), typeof(int)};
		Type[] types1 = {typeof(EZfunLimitScrollView), typeof(int), typeof(string)};

		if (count == 3 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
			int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
			obj.InitPreLoadDataForLua(arg0,arg1);
			return 0;
		}
		else if (count == 3 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
			int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
			string arg1 = LuaScriptMgr.GetString(L, 3);
			obj.InitPreLoadDataForLua(arg0,arg1);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: EZfunLimitScrollView.InitPreLoadDataForLua");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ResetCameraRect(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
		obj.ResetCameraRect();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetUseTransformByIndex(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		EZfunLimitScrollView obj = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		Transform o = obj.GetUseTransformByIndex(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}
}

