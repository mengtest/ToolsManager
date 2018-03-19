using System;
using UnityEngine;
using System.Collections.Generic;
using LuaInterface;
using Object = UnityEngine.Object;

public class EZFunScollViewWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("AddCScrollConstruct", AddCScrollConstruct),
		new LuaMethod("SetLuaShowCallBack", SetLuaShowCallBack),
		new LuaMethod("GetScrollView", GetScrollView),
		new LuaMethod("CreateFourceFirstAdaptSCR", CreateFourceFirstAdaptSCR),
		new LuaMethod("SetItemRefenceTrans", SetItemRefenceTrans),
		new LuaMethod("InitScrollViewInBatch", InitScrollViewInBatch),
		new LuaMethod("setUICameraEnable", setUICameraEnable),
		new LuaMethod("Clear", Clear),
		new LuaMethod("Destory", Destory),
		new LuaMethod("UpdateTransform", UpdateTransform),
		new LuaMethod("UpdateSpacing", UpdateSpacing),
		new LuaMethod("UpdateProgressBar", UpdateProgressBar),
		new LuaMethod("SetDragCamera", SetDragCamera),
		new LuaMethod("SetScrollDicInPage", SetScrollDicInPage),
		new LuaMethod("ResetDragCameraRect", ResetDragCameraRect),
		new LuaMethod("ResetCameraRect", ResetCameraRect),
		new LuaMethod("SetDepth", SetDepth),
		new LuaMethod("SetLayer", SetLayer),
		new LuaMethod("getDraggableCamera", getDraggableCamera),
		new LuaMethod("GetCenterOnChild", GetCenterOnChild),
		new LuaMethod("ResetScrollView", ResetScrollView),
		new LuaMethod("ResetCameraPos", ResetCameraPos),
		new LuaMethod("SetCameraAtEnd", SetCameraAtEnd),
		new LuaMethod("SetIndexFirst", SetIndexFirst),
		new LuaMethod("getItemStartPos", getItemStartPos),
		new LuaMethod("SetLookPosition", SetLookPosition),
		new LuaMethod("SetCanDrag", SetCanDrag),
		new LuaMethod("SetCanClickItem", SetCanClickItem),
		new LuaMethod("GetMaxRect", GetMaxRect),
		new LuaMethod("GetRootBoxSize", GetRootBoxSize),
		new LuaMethod("SetLayerGetType", SetLayerGetType),
		new LuaMethod("JumpPage", JumpPage),
		new LuaMethod("GetParentTrans", GetParentTrans),
		new LuaMethod("GetCurrLineMaxRect", GetCurrLineMaxRect),
		new LuaMethod("New", _CreateEZFunScollView),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("m_barTrans", get_m_barTrans, set_m_barTrans),
		new LuaField("m_direction", get_m_direction, set_m_direction),
		new LuaField("m_noClickWhileDrag", get_m_noClickWhileDrag, set_m_noClickWhileDrag),
		new LuaField("m_SelectTrans", get_m_SelectTrans, set_m_SelectTrans),
		new LuaField("m_layer", get_m_layer, set_m_layer),
		new LuaField("m_hasShowd", get_m_hasShowd, set_m_hasShowd),
		new LuaField("m_needClearName", get_m_needClearName, set_m_needClearName),
		new LuaField("m_parentCamera", get_m_parentCamera, set_m_parentCamera),
		new LuaField("m_layerGetType", get_m_layerGetType, set_m_layerGetType),
		new LuaField("m_isStableCamera", get_m_isStableCamera, set_m_isStableCamera),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateEZFunScollView(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types2 = {typeof(Transform), typeof(OBContainer), typeof(Vector2), typeof(ScrollViewDirection), typeof(int), typeof(Transform)};
		Type[] types3 = {typeof(Transform), typeof(GameObject), typeof(Vector2), typeof(ScrollViewDirection), typeof(int), typeof(bool)};
		Type[] types4 = {typeof(Transform), typeof(GameObject), typeof(Vector2), typeof(ScrollViewDirection), typeof(int), typeof(Transform)};
		Type[] types5 = {typeof(Transform), typeof(GameObject), typeof(Vector2), typeof(ScrollViewDirection), typeof(int), typeof(int)};
		Type[] types6 = {typeof(Transform), typeof(GameObject), typeof(Vector2), typeof(ScrollViewDirection), typeof(int), typeof(ItemShowCallBack), typeof(int)};
		Type[] types7 = {typeof(Transform), typeof(GameObject), typeof(Vector2), typeof(int), typeof(ScrollViewDirection), typeof(int), typeof(int)};
		Type[] types8 = {typeof(Transform), typeof(List<CScrollConstruct>), typeof(ScrollViewDirection), typeof(Transform), typeof(bool), typeof(Camera), typeof(bool), typeof(int), typeof(int)};
		Type[] types9 = {typeof(Transform), typeof(GameObject), typeof(Vector2), typeof(ScrollViewDirection), typeof(int), typeof(ItemShowCallBack), typeof(Transform), typeof(bool), typeof(int)};

		if (count == 3)
		{
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
			ScrollViewDirection arg1 = LuaScriptMgr.GetNetObject<ScrollViewDirection>(L, 2);
			bool arg2 = LuaScriptMgr.GetBoolean(L, 3);
			EZFunScollView obj = new EZFunScollView(arg0,arg1,arg2);
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else if (count == 5)
		{
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
			OBContainer arg1 = LuaScriptMgr.GetNetObject<OBContainer>(L, 2);
			Vector2 arg2 = LuaScriptMgr.GetNetObject<Vector2>(L, 3);
			ScrollViewDirection arg3 = LuaScriptMgr.GetNetObject<ScrollViewDirection>(L, 4);
			int arg4 = (int)LuaScriptMgr.GetNumber(L, 5);
			EZFunScollView obj = new EZFunScollView(arg0,arg1,arg2,arg3,arg4);
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else if (count == 6 && LuaScriptMgr.CheckTypes(L, types2, 1))
		{
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
			OBContainer arg1 = LuaScriptMgr.GetNetObject<OBContainer>(L, 2);
			Vector2 arg2 = LuaScriptMgr.GetNetObject<Vector2>(L, 3);
			ScrollViewDirection arg3 = LuaScriptMgr.GetNetObject<ScrollViewDirection>(L, 4);
			int arg4 = (int)LuaScriptMgr.GetNumber(L, 5);
			Transform arg5 = LuaScriptMgr.GetNetObject<Transform>(L, 6);
			EZFunScollView obj = new EZFunScollView(arg0,arg1,arg2,arg3,arg4,arg5);
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else if (count == 6 && LuaScriptMgr.CheckTypes(L, types3, 1))
		{
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
			GameObject arg1 = LuaScriptMgr.GetNetObject<GameObject>(L, 2);
			Vector2 arg2 = LuaScriptMgr.GetNetObject<Vector2>(L, 3);
			ScrollViewDirection arg3 = LuaScriptMgr.GetNetObject<ScrollViewDirection>(L, 4);
			int arg4 = (int)LuaScriptMgr.GetNumber(L, 5);
			bool arg5 = LuaScriptMgr.GetBoolean(L, 6);
			EZFunScollView obj = new EZFunScollView(arg0,arg1,arg2,arg3,arg4,arg5);
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else if (count == 6 && LuaScriptMgr.CheckTypes(L, types4, 1))
		{
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
			GameObject arg1 = LuaScriptMgr.GetNetObject<GameObject>(L, 2);
			Vector2 arg2 = LuaScriptMgr.GetNetObject<Vector2>(L, 3);
			ScrollViewDirection arg3 = LuaScriptMgr.GetNetObject<ScrollViewDirection>(L, 4);
			int arg4 = (int)LuaScriptMgr.GetNumber(L, 5);
			Transform arg5 = LuaScriptMgr.GetNetObject<Transform>(L, 6);
			EZFunScollView obj = new EZFunScollView(arg0,arg1,arg2,arg3,arg4,arg5);
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else if (count == 6 && LuaScriptMgr.CheckTypes(L, types5, 1))
		{
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
			GameObject arg1 = LuaScriptMgr.GetNetObject<GameObject>(L, 2);
			Vector2 arg2 = LuaScriptMgr.GetNetObject<Vector2>(L, 3);
			ScrollViewDirection arg3 = LuaScriptMgr.GetNetObject<ScrollViewDirection>(L, 4);
			int arg4 = (int)LuaScriptMgr.GetNumber(L, 5);
			int arg5 = (int)LuaScriptMgr.GetNumber(L, 6);
			EZFunScollView obj = new EZFunScollView(arg0,arg1,arg2,arg3,arg4,arg5);
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else if (count == 7 && LuaScriptMgr.CheckTypes(L, types6, 1))
		{
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
			GameObject arg1 = LuaScriptMgr.GetNetObject<GameObject>(L, 2);
			Vector2 arg2 = LuaScriptMgr.GetNetObject<Vector2>(L, 3);
			ScrollViewDirection arg3 = LuaScriptMgr.GetNetObject<ScrollViewDirection>(L, 4);
			int arg4 = (int)LuaScriptMgr.GetNumber(L, 5);
			ItemShowCallBack arg5 = LuaScriptMgr.GetNetObject<ItemShowCallBack>(L, 6);
			int arg6 = (int)LuaScriptMgr.GetNumber(L, 7);
			EZFunScollView obj = new EZFunScollView(arg0,arg1,arg2,arg3,arg4,arg5,arg6);
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else if (count == 7 && LuaScriptMgr.CheckTypes(L, types7, 1))
		{
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
			GameObject arg1 = LuaScriptMgr.GetNetObject<GameObject>(L, 2);
			Vector2 arg2 = LuaScriptMgr.GetNetObject<Vector2>(L, 3);
			int arg3 = (int)LuaScriptMgr.GetNumber(L, 4);
			ScrollViewDirection arg4 = LuaScriptMgr.GetNetObject<ScrollViewDirection>(L, 5);
			int arg5 = (int)LuaScriptMgr.GetNumber(L, 6);
			int arg6 = (int)LuaScriptMgr.GetNumber(L, 7);
			EZFunScollView obj = new EZFunScollView(arg0,arg1,arg2,arg3,arg4,arg5,arg6);
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else if (count == 9 && LuaScriptMgr.CheckTypes(L, types8, 1))
		{
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
			List<CScrollConstruct> arg1 = LuaScriptMgr.GetNetObject<List<CScrollConstruct>>(L, 2);
			ScrollViewDirection arg2 = LuaScriptMgr.GetNetObject<ScrollViewDirection>(L, 3);
			Transform arg3 = LuaScriptMgr.GetNetObject<Transform>(L, 4);
			bool arg4 = LuaScriptMgr.GetBoolean(L, 5);
			Camera arg5 = LuaScriptMgr.GetNetObject<Camera>(L, 6);
			bool arg6 = LuaScriptMgr.GetBoolean(L, 7);
			int arg7 = (int)LuaScriptMgr.GetNumber(L, 8);
			int arg8 = (int)LuaScriptMgr.GetNumber(L, 9);
			EZFunScollView obj = new EZFunScollView(arg0,arg1,arg2,arg3,arg4,arg5,arg6,arg7,arg8);
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else if (count == 9 && LuaScriptMgr.CheckTypes(L, types9, 1))
		{
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
			GameObject arg1 = LuaScriptMgr.GetNetObject<GameObject>(L, 2);
			Vector2 arg2 = LuaScriptMgr.GetNetObject<Vector2>(L, 3);
			ScrollViewDirection arg3 = LuaScriptMgr.GetNetObject<ScrollViewDirection>(L, 4);
			int arg4 = (int)LuaScriptMgr.GetNumber(L, 5);
			ItemShowCallBack arg5 = LuaScriptMgr.GetNetObject<ItemShowCallBack>(L, 6);
			Transform arg6 = LuaScriptMgr.GetNetObject<Transform>(L, 7);
			bool arg7 = LuaScriptMgr.GetBoolean(L, 8);
			int arg8 = (int)LuaScriptMgr.GetNumber(L, 9);
			EZFunScollView obj = new EZFunScollView(arg0,arg1,arg2,arg3,arg4,arg5,arg6,arg7,arg8);
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: EZFunScollView.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(EZFunScollView));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "EZFunScollView", typeof(EZFunScollView), regs, fields, "System.Object");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_barTrans(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_barTrans");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_barTrans on a nil value");
			}
		}

		EZFunScollView obj = (EZFunScollView)o;
		LuaScriptMgr.Push(L, obj.m_barTrans);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_direction(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_direction");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_direction on a nil value");
			}
		}

		EZFunScollView obj = (EZFunScollView)o;
		LuaScriptMgr.PushEnum(L, obj.m_direction);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_noClickWhileDrag(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_noClickWhileDrag");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_noClickWhileDrag on a nil value");
			}
		}

		EZFunScollView obj = (EZFunScollView)o;
		LuaScriptMgr.Push(L, obj.m_noClickWhileDrag);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_SelectTrans(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_SelectTrans");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_SelectTrans on a nil value");
			}
		}

		EZFunScollView obj = (EZFunScollView)o;
		LuaScriptMgr.PushObject(L, obj.m_SelectTrans);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_layer(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_layer");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_layer on a nil value");
			}
		}

		EZFunScollView obj = (EZFunScollView)o;
		LuaScriptMgr.Push(L, obj.m_layer);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_hasShowd(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_hasShowd");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_hasShowd on a nil value");
			}
		}

		EZFunScollView obj = (EZFunScollView)o;
		LuaScriptMgr.Push(L, obj.m_hasShowd);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_needClearName(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_needClearName");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_needClearName on a nil value");
			}
		}

		EZFunScollView obj = (EZFunScollView)o;
		LuaScriptMgr.Push(L, obj.m_needClearName);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_parentCamera(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_parentCamera");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_parentCamera on a nil value");
			}
		}

		EZFunScollView obj = (EZFunScollView)o;
		LuaScriptMgr.Push(L, obj.m_parentCamera);
		return 1;
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

		EZFunScollView obj = (EZFunScollView)o;
		LuaScriptMgr.Push(L, obj.m_layerGetType);
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

		EZFunScollView obj = (EZFunScollView)o;
		LuaScriptMgr.Push(L, obj.m_isStableCamera);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_barTrans(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_barTrans");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_barTrans on a nil value");
			}
		}

		EZFunScollView obj = (EZFunScollView)o;
		obj.m_barTrans = LuaScriptMgr.GetNetObject<Transform>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_direction(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_direction");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_direction on a nil value");
			}
		}

		EZFunScollView obj = (EZFunScollView)o;
		obj.m_direction = LuaScriptMgr.GetNetObject<ScrollViewDirection>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_noClickWhileDrag(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_noClickWhileDrag");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_noClickWhileDrag on a nil value");
			}
		}

		EZFunScollView obj = (EZFunScollView)o;
		obj.m_noClickWhileDrag = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_SelectTrans(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_SelectTrans");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_SelectTrans on a nil value");
			}
		}

		EZFunScollView obj = (EZFunScollView)o;
		obj.m_SelectTrans = LuaScriptMgr.GetNetObject<List<Transform>>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_layer(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_layer");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_layer on a nil value");
			}
		}

		EZFunScollView obj = (EZFunScollView)o;
		obj.m_layer = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_hasShowd(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_hasShowd");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_hasShowd on a nil value");
			}
		}

		EZFunScollView obj = (EZFunScollView)o;
		obj.m_hasShowd = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_needClearName(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_needClearName");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_needClearName on a nil value");
			}
		}

		EZFunScollView obj = (EZFunScollView)o;
		obj.m_needClearName = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_parentCamera(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_parentCamera");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_parentCamera on a nil value");
			}
		}

		EZFunScollView obj = (EZFunScollView)o;
		obj.m_parentCamera = LuaScriptMgr.GetNetObject<Camera>(L, 3);
		return 0;
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

		EZFunScollView obj = (EZFunScollView)o;
		obj.m_layerGetType = (int)LuaScriptMgr.GetNumber(L, 3);
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

		EZFunScollView obj = (EZFunScollView)o;
		obj.m_isStableCamera = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddCScrollConstruct(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 2);
		Vector2 arg1 = LuaScriptMgr.GetNetObject<Vector2>(L, 3);
		int arg2 = (int)LuaScriptMgr.GetNumber(L, 4);
		obj.AddCScrollConstruct(arg0,arg1,arg2);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLuaShowCallBack(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(EZFunScollView), typeof(LuaInterface.LuaFunction)};
		Type[] types1 = {typeof(EZFunScollView), typeof(string)};

		if (count == 2 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
			LuaFunction arg0 = LuaScriptMgr.GetLuaFunction(L, 2);
			obj.SetLuaShowCallBack(arg0);
			return 0;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
			string arg0 = LuaScriptMgr.GetString(L, 2);
			obj.SetLuaShowCallBack(arg0);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: EZFunScollView.SetLuaShowCallBack");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetScrollView(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types1 = {typeof(EZFunScollView), typeof(List<CScrollGet>), typeof(bool)};
		Type[] types2 = {typeof(EZFunScollView), typeof(string), typeof(bool)};

		if (count == 1)
		{
			EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
			UIScrollView o = obj.GetScrollView();
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 3 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
			List<CScrollGet> arg0 = LuaScriptMgr.GetNetObject<List<CScrollGet>>(L, 2);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
			Dictionary<int,List<Transform>> o = obj.GetScrollView(arg0,arg1);
			LuaScriptMgr.PushObject(L, o);
			return 1;
		}
		else if (count == 3 && LuaScriptMgr.CheckTypes(L, types2, 1))
		{
			EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
			string arg0 = LuaScriptMgr.GetString(L, 2);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
			obj.GetScrollView(arg0,arg1);
			return 0;
		}
		else if (count == 4)
		{
			EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
			int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
			int arg2 = (int)LuaScriptMgr.GetNumber(L, 4);
			List<Transform> o = obj.GetScrollView(arg0,arg1,arg2);
			LuaScriptMgr.PushObject(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: EZFunScollView.GetScrollView");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CreateFourceFirstAdaptSCR(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 5);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		List<CScrollConstruct> arg1 = LuaScriptMgr.GetNetObject<List<CScrollConstruct>>(L, 2);
		ScrollViewDirection arg2 = LuaScriptMgr.GetNetObject<ScrollViewDirection>(L, 3);
		Transform arg3 = LuaScriptMgr.GetNetObject<Transform>(L, 4);
		bool arg4 = LuaScriptMgr.GetBoolean(L, 5);
		EZFunScollView o = EZFunScollView.CreateFourceFirstAdaptSCR(arg0,arg1,arg2,arg3,arg4);
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetItemRefenceTrans(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		obj.SetItemRefenceTrans(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitScrollViewInBatch(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		List<CScrollGet> arg0 = LuaScriptMgr.GetNetObject<List<CScrollGet>>(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		bool arg2 = LuaScriptMgr.GetBoolean(L, 4);
		obj.InitScrollViewInBatch(arg0,arg1,arg2);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int setUICameraEnable(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		obj.setUICameraEnable(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Clear(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		obj.Clear();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Destory(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		obj.Destory();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UpdateTransform(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		obj.UpdateTransform();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UpdateSpacing(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		obj.UpdateSpacing();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UpdateProgressBar(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		obj.UpdateProgressBar();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetDragCamera(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		obj.SetDragCamera(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetScrollDicInPage(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		ScrollViewDirection arg0 = LuaScriptMgr.GetNetObject<ScrollViewDirection>(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		obj.SetScrollDicInPage(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ResetDragCameraRect(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		obj.ResetDragCameraRect();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ResetCameraRect(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		obj.ResetCameraRect();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetDepth(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		obj.SetDepth();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLayer(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		EZFunScollView.SetLayer(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int getDraggableCamera(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		UIDraggableCamera o = obj.getDraggableCamera();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCenterOnChild(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		UICenterOnChild o = obj.GetCenterOnChild();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ResetScrollView(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		obj.ResetScrollView(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ResetCameraPos(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		obj.ResetCameraPos();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetCameraAtEnd(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		obj.SetCameraAtEnd();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetIndexFirst(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		obj.SetIndexFirst(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int getItemStartPos(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		List<Vector3> o = obj.getItemStartPos();
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLookPosition(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		Vector3 arg0 = LuaScriptMgr.GetNetObject<Vector3>(L, 2);
		obj.SetLookPosition(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetCanDrag(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		obj.SetCanDrag(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetCanClickItem(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		string arg1 = LuaScriptMgr.GetLuaString(L, 3);
		obj.SetCanClickItem(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetMaxRect(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		Rect o = obj.GetMaxRect();
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetRootBoxSize(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		Vector2 o = obj.GetRootBoxSize();
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLayerGetType(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		obj.SetLayerGetType(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int JumpPage(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		obj.JumpPage(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetParentTrans(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		Transform o = obj.GetParentTrans();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrLineMaxRect(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		EZFunScollView obj = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 1);
		Rect o = obj.GetCurrLineMaxRect();
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}
}

