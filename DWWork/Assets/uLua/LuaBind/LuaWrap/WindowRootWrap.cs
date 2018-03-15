using System;
using UnityEngine;
using LuaInterface;
using Object = UnityEngine.Object;

public class WindowRootWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("InitWindow", InitWindow),
		new LuaMethod("OnReceiveMemoryWarning", OnReceiveMemoryWarning),
		new LuaMethod("RegisterModule", RegisterModule),
		new LuaMethod("GetModuleByType", GetModuleByType),
		new LuaMethod("RegisterUpdate", RegisterUpdate),
		new LuaMethod("RegisterPerSecondUpdate", RegisterPerSecondUpdate),
		new LuaMethod("UnRegisterUpdate", UnRegisterUpdate),
		new LuaMethod("UnRegisterPerSecondUpdate", UnRegisterPerSecondUpdate),
		new LuaMethod("TriggerNewPlayerGuideUIWindow", TriggerNewPlayerGuideUIWindow),
		new LuaMethod("CloseWindowDelayProcess", CloseWindowDelayProcess),
		new LuaMethod("InitDic", InitDic),
		new LuaMethod("GetLayer", GetLayer),
		new LuaMethod("OnWillDestroy", OnWillDestroy),
		new LuaMethod("OnDestroy", OnDestroy),
		new LuaMethod("ClearEventHandler", ClearEventHandler),
		new LuaMethod("ClearTempData", ClearTempData),
		new LuaMethod("AddScrollView", AddScrollView),
		new LuaMethod("AddLimitScrollView", AddLimitScrollView),
		new LuaMethod("SetWindowActive", SetWindowActive),
		new LuaMethod("InitCamera", InitCamera),
		new LuaMethod("GetCameraStruct", GetCameraStruct),
		new LuaMethod("CheckBtnCanClickForLua", CheckBtnCanClickForLua),
		new LuaMethod("CheckBtnClickDiff", CheckBtnClickDiff),
		new LuaMethod("CheckBtnCanClickSrc", CheckBtnCanClickSrc),
		new LuaMethod("CheckNeedClickBtn", CheckNeedClickBtn),
		new LuaMethod("SetLabelSelfActive", SetLabelSelfActive),
		new LuaMethod("ResetSpriteSize", ResetSpriteSize),
		new LuaMethod("ClickPointInCollider", ClickPointInCollider),
		new LuaMethod("ScaleParticle", ScaleParticle),
		new LuaMethod("AutoSetBgScale", AutoSetBgScale),
		new LuaMethod("GetWidgetBounds", GetWidgetBounds),
		new LuaMethod("RemoveColorStr", RemoveColorStr),
		new LuaMethod("SetLayer", SetLayer),
		new LuaMethod("SetParentAndReset", SetParentAndReset),
		new LuaMethod("SetMiddle", SetMiddle),
		new LuaMethod("AdaptChildPosition", AdaptChildPosition),
		new LuaMethod("AdaptChildPositionLimitMaxNum", AdaptChildPositionLimitMaxNum),
		new LuaMethod("SetScrollViewCanDrag", SetScrollViewCanDrag),
		new LuaMethod("AutoSizeParticle", AutoSizeParticle),
		new LuaMethod("GetTrueWindowWidth", GetTrueWindowWidth),
		new LuaMethod("EnableTween", EnableTween),
		new LuaMethod("GetLableSize", GetLableSize),
		new LuaMethod("GenTipsIndependently", GenTipsIndependently),
		new LuaMethod("GetColor", GetColor),
		new LuaMethod("FloatInLimit", FloatInLimit),
		new LuaMethod("SetLabelGradient", SetLabelGradient),
		new LuaMethod("GetCloseBehaviour", GetCloseBehaviour),
		new LuaMethod("GetSceneChangeBehaviour", GetSceneChangeBehaviour),
		new LuaMethod("PlayTweenAlpha", PlayTweenAlpha),
		new LuaMethod("PlayTweenPos", PlayTweenPos),
		new LuaMethod("PlayAndStopTweens", PlayAndStopTweens),
		new LuaMethod("SetDesAndPlayTweenRotation", SetDesAndPlayTweenRotation),
		new LuaMethod("LoadImag", LoadImag),
		new LuaMethod("New", _CreateWindowRoot),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("SYS_AUTO_CLOSE_STATE", get_SYS_AUTO_CLOSE_STATE, null),
		new LuaField("m_needClickBtnName", get_m_needClickBtnName, set_m_needClickBtnName),
		new LuaField("m_needClickWindow", get_m_needClickWindow, set_m_needClickWindow),
		new LuaField("m_IsTipWindow", get_m_IsTipWindow, set_m_IsTipWindow),
		new LuaField("s_argc1", get_s_argc1, set_s_argc1),
		new LuaField("m_selfOpenState", get_m_selfOpenState, set_m_selfOpenState),
		new LuaField("m_needOpenMainWindow", get_m_needOpenMainWindow, set_m_needOpenMainWindow),
		new LuaField("m_hasWinAnim", get_m_hasWinAnim, set_m_hasWinAnim),
		new LuaField("m_animationState", get_m_animationState, set_m_animationState),
		new LuaField("m_cameraStruct", get_m_cameraStruct, set_m_cameraStruct),
		new LuaField("ignoreDeltaTime", get_ignoreDeltaTime, set_ignoreDeltaTime),
		new LuaField("m_scrollViewCnt", get_m_scrollViewCnt, set_m_scrollViewCnt),
		new LuaField("playAnimOnOpen", get_playAnimOnOpen, null),
		new LuaField("playAnimOnClose", get_playAnimOnClose, null),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateWindowRoot(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			WindowRoot obj = new WindowRoot();
			LuaScriptMgr.Push(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: WindowRoot.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(WindowRoot));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "WindowRoot", typeof(WindowRoot), regs, fields, "BaseUI");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_SYS_AUTO_CLOSE_STATE(IntPtr L)
	{
		LuaScriptMgr.Push(L, WindowRoot.SYS_AUTO_CLOSE_STATE);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_needClickBtnName(IntPtr L)
	{
		LuaScriptMgr.Push(L, WindowRoot.m_needClickBtnName);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_needClickWindow(IntPtr L)
	{
		LuaScriptMgr.PushEnum(L, WindowRoot.m_needClickWindow);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_IsTipWindow(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_IsTipWindow");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_IsTipWindow on a nil value");
			}
		}

		WindowRoot obj = (WindowRoot)o;
		LuaScriptMgr.Push(L, obj.m_IsTipWindow);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_s_argc1(IntPtr L)
	{
		LuaScriptMgr.Push(L, WindowRoot.s_argc1);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_selfOpenState(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_selfOpenState");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_selfOpenState on a nil value");
			}
		}

		WindowRoot obj = (WindowRoot)o;
		LuaScriptMgr.Push(L, obj.m_selfOpenState);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_needOpenMainWindow(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_needOpenMainWindow");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_needOpenMainWindow on a nil value");
			}
		}

		WindowRoot obj = (WindowRoot)o;
		LuaScriptMgr.Push(L, obj.m_needOpenMainWindow);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_hasWinAnim(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_hasWinAnim");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_hasWinAnim on a nil value");
			}
		}

		WindowRoot obj = (WindowRoot)o;
		LuaScriptMgr.Push(L, obj.m_hasWinAnim);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_animationState(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_animationState");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_animationState on a nil value");
			}
		}

		WindowRoot obj = (WindowRoot)o;
		LuaScriptMgr.Push(L, obj.m_animationState);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_cameraStruct(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_cameraStruct");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_cameraStruct on a nil value");
			}
		}

		WindowRoot obj = (WindowRoot)o;
		LuaScriptMgr.PushObject(L, obj.m_cameraStruct);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ignoreDeltaTime(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name ignoreDeltaTime");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index ignoreDeltaTime on a nil value");
			}
		}

		WindowRoot obj = (WindowRoot)o;
		LuaScriptMgr.Push(L, obj.ignoreDeltaTime);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_scrollViewCnt(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_scrollViewCnt");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_scrollViewCnt on a nil value");
			}
		}

		WindowRoot obj = (WindowRoot)o;
		LuaScriptMgr.Push(L, obj.m_scrollViewCnt);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_playAnimOnOpen(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name playAnimOnOpen");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index playAnimOnOpen on a nil value");
			}
		}

		WindowRoot obj = (WindowRoot)o;
		LuaScriptMgr.Push(L, obj.playAnimOnOpen);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_playAnimOnClose(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name playAnimOnClose");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index playAnimOnClose on a nil value");
			}
		}

		WindowRoot obj = (WindowRoot)o;
		LuaScriptMgr.Push(L, obj.playAnimOnClose);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_needClickBtnName(IntPtr L)
	{
		WindowRoot.m_needClickBtnName = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_needClickWindow(IntPtr L)
	{
		WindowRoot.m_needClickWindow = LuaScriptMgr.GetNetObject<EZFunWindowEnum>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_IsTipWindow(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_IsTipWindow");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_IsTipWindow on a nil value");
			}
		}

		WindowRoot obj = (WindowRoot)o;
		obj.m_IsTipWindow = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_s_argc1(IntPtr L)
	{
		WindowRoot.s_argc1 = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_selfOpenState(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_selfOpenState");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_selfOpenState on a nil value");
			}
		}

		WindowRoot obj = (WindowRoot)o;
		obj.m_selfOpenState = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_needOpenMainWindow(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_needOpenMainWindow");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_needOpenMainWindow on a nil value");
			}
		}

		WindowRoot obj = (WindowRoot)o;
		obj.m_needOpenMainWindow = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_hasWinAnim(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_hasWinAnim");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_hasWinAnim on a nil value");
			}
		}

		WindowRoot obj = (WindowRoot)o;
		obj.m_hasWinAnim = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_animationState(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_animationState");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_animationState on a nil value");
			}
		}

		WindowRoot obj = (WindowRoot)o;
		obj.m_animationState = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_cameraStruct(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_cameraStruct");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_cameraStruct on a nil value");
			}
		}

		WindowRoot obj = (WindowRoot)o;
		obj.m_cameraStruct = LuaScriptMgr.GetNetObject<EZFunWindowMgr.UICameraStruct>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ignoreDeltaTime(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name ignoreDeltaTime");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index ignoreDeltaTime on a nil value");
			}
		}

		WindowRoot obj = (WindowRoot)o;
		obj.ignoreDeltaTime = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_scrollViewCnt(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_scrollViewCnt");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_scrollViewCnt on a nil value");
			}
		}

		WindowRoot obj = (WindowRoot)o;
		obj.m_scrollViewCnt = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitWindow(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 3)
		{
			WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
			bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
			obj.InitWindow(arg0,arg1);
			return 0;
		}
		else if (count == 4)
		{
			WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
			bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
			object arg2 = LuaScriptMgr.GetVarObject(L, 4);
			obj.InitWindow(arg0,arg1,arg2);
			return 0;
		}
		else if (count == 5)
		{
			WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
			bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
			bool arg2 = LuaScriptMgr.GetBoolean(L, 4);
			object arg3 = LuaScriptMgr.GetVarObject(L, 5);
			obj.InitWindow(arg0,arg1,arg2,arg3);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: WindowRoot.InitWindow");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnReceiveMemoryWarning(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		obj.OnReceiveMemoryWarning();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RegisterModule(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 5);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		ModuleType arg0 = LuaScriptMgr.GetNetObject<ModuleType>(L, 2);
		string arg1 = LuaScriptMgr.GetLuaString(L, 3);
		string arg2 = LuaScriptMgr.GetLuaString(L, 4);
		bool arg3 = LuaScriptMgr.GetBoolean(L, 5);
		obj.RegisterModule(arg0,arg1,arg2,arg3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetModuleByType(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		ModuleType arg0 = LuaScriptMgr.GetNetObject<ModuleType>(L, 2);
		ModuleRoot o = obj.GetModuleByType(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RegisterUpdate(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		Action arg0 = LuaScriptMgr.GetNetObject<Action>(L, 2);
		obj.RegisterUpdate(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RegisterPerSecondUpdate(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		Action arg0 = LuaScriptMgr.GetNetObject<Action>(L, 2);
		obj.RegisterPerSecondUpdate(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UnRegisterUpdate(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		Action arg0 = LuaScriptMgr.GetNetObject<Action>(L, 2);
		obj.UnRegisterUpdate(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UnRegisterPerSecondUpdate(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		Action arg0 = LuaScriptMgr.GetNetObject<Action>(L, 2);
		obj.UnRegisterPerSecondUpdate(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int TriggerNewPlayerGuideUIWindow(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		obj.TriggerNewPlayerGuideUIWindow();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CloseWindowDelayProcess(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		obj.CloseWindowDelayProcess();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitDic(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		obj.InitDic();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLayer(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		int o = obj.GetLayer();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnWillDestroy(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		obj.OnWillDestroy();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnDestroy(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		obj.OnDestroy();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ClearEventHandler(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		obj.ClearEventHandler();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ClearTempData(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		obj.ClearTempData();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddScrollView(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		EZFunScollView arg0 = LuaScriptMgr.GetNetObject<EZFunScollView>(L, 2);
		obj.AddScrollView(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddLimitScrollView(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		EZfunLimitScrollView arg0 = LuaScriptMgr.GetNetObject<EZfunLimitScrollView>(L, 2);
		obj.AddLimitScrollView(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetWindowActive(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		obj.SetWindowActive(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitCamera(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 5)
		{
			WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
			bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
			int arg2 = (int)LuaScriptMgr.GetNumber(L, 4);
			bool arg3 = LuaScriptMgr.GetBoolean(L, 5);
			obj.InitCamera(arg0,arg1,arg2,arg3);
			return 0;
		}
		else if (count == 6)
		{
			WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
			bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
			bool arg2 = LuaScriptMgr.GetBoolean(L, 4);
			bool arg3 = LuaScriptMgr.GetBoolean(L, 5);
			int arg4 = (int)LuaScriptMgr.GetNumber(L, 6);
			obj.InitCamera(arg0,arg1,arg2,arg3,arg4);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: WindowRoot.InitCamera");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCameraStruct(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		EZFunWindowMgr.UICameraStruct o = obj.GetCameraStruct();
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CheckBtnCanClickForLua(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		bool o = obj.CheckBtnCanClickForLua(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CheckBtnClickDiff(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		bool o = obj.CheckBtnClickDiff();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CheckBtnCanClickSrc(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		bool o = obj.CheckBtnCanClickSrc(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CheckNeedClickBtn(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		bool o = WindowRoot.CheckNeedClickBtn();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLabelSelfActive(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		string arg1 = LuaScriptMgr.GetLuaString(L, 3);
		obj.SetLabelSelfActive(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ResetSpriteSize(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(WindowRoot), typeof(Transform), typeof(int)};
		Type[] types1 = {typeof(WindowRoot), typeof(Transform), typeof(Vector2)};

		if (count == 3 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
			obj.ResetSpriteSize(arg0,arg1);
			return 0;
		}
		else if (count == 3 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			Vector2 arg1 = LuaScriptMgr.GetNetObject<Vector2>(L, 3);
			obj.ResetSpriteSize(arg0,arg1);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: WindowRoot.ResetSpriteSize");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ClickPointInCollider(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		bool o = WindowRoot.ClickPointInCollider(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ScaleParticle(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 2);
		Vector3 arg1 = LuaScriptMgr.GetNetObject<Vector3>(L, 3);
		obj.ScaleParticle(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AutoSetBgScale(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		Transform arg1 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		Vector4 arg2 = LuaScriptMgr.GetNetObject<Vector4>(L, 3);
		WindowRoot.AutoSetBgScale(arg0,arg1,arg2);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetWidgetBounds(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UIWidget arg0 = LuaScriptMgr.GetNetObject<UIWidget>(L, 1);
		Transform arg1 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		Vector4 o = WindowRoot.GetWidgetBounds(arg0,arg1);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RemoveColorStr(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		string o = obj.RemoveColorStr(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLayer(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(Transform), typeof(int)};
		Type[] types1 = {typeof(GameObject), typeof(int)};

		if (count == 2 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
			WindowRoot.SetLayer(arg0,arg1);
			return 0;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
			WindowRoot.SetLayer(arg0,arg1);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: WindowRoot.SetLayer");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetParentAndReset(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		Transform arg1 = LuaScriptMgr.GetNetObject<Transform>(L, 3);
		obj.SetParentAndReset(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetMiddle(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		Vector2 arg1 = LuaScriptMgr.GetNetObject<Vector2>(L, 3);
		int arg2 = (int)LuaScriptMgr.GetNumber(L, 4);
		obj.SetMiddle(arg0,arg1,arg2);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AdaptChildPosition(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 5);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		int arg2 = (int)LuaScriptMgr.GetNumber(L, 4);
		int arg3 = (int)LuaScriptMgr.GetNumber(L, 5);
		obj.AdaptChildPosition(arg0,arg1,arg2,arg3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AdaptChildPositionLimitMaxNum(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 7);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		int arg2 = (int)LuaScriptMgr.GetNumber(L, 4);
		int arg3 = (int)LuaScriptMgr.GetNumber(L, 5);
		int arg4 = (int)LuaScriptMgr.GetNumber(L, 6);
		int arg5 = (int)LuaScriptMgr.GetNumber(L, 7);
		obj.AdaptChildPositionLimitMaxNum(arg0,arg1,arg2,arg3,arg4,arg5);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetScrollViewCanDrag(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		obj.SetScrollViewCanDrag(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AutoSizeParticle(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
		float arg1 = (float)LuaScriptMgr.GetNumber(L, 2);
		WindowRoot.AutoSizeParticle(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetTrueWindowWidth(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		float o = obj.GetTrueWindowWidth();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EnableTween(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		obj.EnableTween(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLableSize(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		Vector2 o = obj.GetLableSize(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GenTipsIndependently(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		WindowRoot.GenTipsIndependently(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetColor(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		float arg0 = (float)LuaScriptMgr.GetNumber(L, 2);
		float arg1 = (float)LuaScriptMgr.GetNumber(L, 3);
		float arg2 = (float)LuaScriptMgr.GetNumber(L, 4);
		Color o = obj.GetColor(arg0,arg1,arg2);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int FloatInLimit(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		float arg0 = (float)LuaScriptMgr.GetNumber(L, 2);
		float arg1 = (float)LuaScriptMgr.GetNumber(L, 3);
		float arg2 = (float)LuaScriptMgr.GetNumber(L, 4);
		float o = obj.FloatInLimit(arg0,arg1,arg2);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLabelGradient(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		Color arg1 = LuaScriptMgr.GetNetObject<Color>(L, 2);
		Color arg2 = LuaScriptMgr.GetNetObject<Color>(L, 3);
		WindowRoot.SetLabelGradient(arg0,arg1,arg2);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCloseBehaviour(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		WindowCloseBehaviour o = obj.GetCloseBehaviour();
		LuaScriptMgr.PushEnum(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetSceneChangeBehaviour(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		WindowBehaviourOnSceneChange o = obj.GetSceneChangeBehaviour();
		LuaScriptMgr.PushEnum(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlayTweenAlpha(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 5);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		float arg1 = (float)LuaScriptMgr.GetNumber(L, 3);
		float arg2 = (float)LuaScriptMgr.GetNumber(L, 4);
		double arg3 = (double)LuaScriptMgr.GetNumber(L, 5);
		obj.PlayTweenAlpha(arg0,arg1,arg2,arg3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlayTweenPos(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(WindowRoot), typeof(Transform), typeof(Vector3), typeof(Vector3), typeof(double), typeof(bool)};
		Type[] types1 = {typeof(WindowRoot), typeof(Transform), typeof(float), typeof(bool), typeof(float), typeof(float)};

		if (count == 6 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			Vector3 arg1 = LuaScriptMgr.GetNetObject<Vector3>(L, 3);
			Vector3 arg2 = LuaScriptMgr.GetNetObject<Vector3>(L, 4);
			double arg3 = (double)LuaScriptMgr.GetNumber(L, 5);
			bool arg4 = LuaScriptMgr.GetBoolean(L, 6);
			obj.PlayTweenPos(arg0,arg1,arg2,arg3,arg4);
			return 0;
		}
		else if (count == 6 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			float arg1 = (float)LuaScriptMgr.GetNumber(L, 3);
			bool arg2 = LuaScriptMgr.GetBoolean(L, 4);
			float arg3 = (float)LuaScriptMgr.GetNumber(L, 5);
			float arg4 = (float)LuaScriptMgr.GetNumber(L, 6);
			obj.PlayTweenPos(arg0,arg1,arg2,arg3,arg4);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: WindowRoot.PlayTweenPos");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlayAndStopTweens(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		obj.PlayAndStopTweens(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetDesAndPlayTweenRotation(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		Vector3 arg1 = LuaScriptMgr.GetNetObject<Vector3>(L, 3);
		obj.SetDesAndPlayTweenRotation(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadImag(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 6)
		{
			WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			string arg1 = LuaScriptMgr.GetLuaString(L, 3);
			string arg2 = LuaScriptMgr.GetLuaString(L, 4);
			bool arg3 = LuaScriptMgr.GetBoolean(L, 5);
			RessStorgeType arg4 = LuaScriptMgr.GetNetObject<RessStorgeType>(L, 6);
			obj.LoadImag(arg0,arg1,arg2,arg3,arg4);
			return 0;
		}
		else if (count == 7)
		{
			WindowRoot obj = LuaScriptMgr.GetNetObject<WindowRoot>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			string arg1 = LuaScriptMgr.GetLuaString(L, 3);
			string arg2 = LuaScriptMgr.GetLuaString(L, 4);
			bool arg3 = LuaScriptMgr.GetBoolean(L, 5);
			RessStorgeType arg4 = LuaScriptMgr.GetNetObject<RessStorgeType>(L, 6);
			bool arg5 = LuaScriptMgr.GetBoolean(L, 7);
			obj.LoadImag(arg0,arg1,arg2,arg3,arg4,arg5);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: WindowRoot.LoadImag");
		}

		return 0;
	}
}

