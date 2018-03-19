using System;
using UnityEngine;
using System.Collections.Generic;
using LuaInterface;
using Object = UnityEngine.Object;

public class BaseUIWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("CreateUI", CreateUI),
		new LuaMethod("OnDestroy", OnDestroy),
		new LuaMethod("CheckBtnCanClick", CheckBtnCanClick),
		new LuaMethod("CheckBtnCanClickBase", CheckBtnCanClickBase),
		new LuaMethod("HandleWidgetBaseClick", HandleWidgetBaseClick),
		new LuaMethod("SetOnClickCallback", SetOnClickCallback),
		new LuaMethod("TransformScreenSize", TransformScreenSize),
		new LuaMethod("GetBoardDistance", GetBoardDistance),
		new LuaMethod("SelfAdaptScale", SelfAdaptScale),
		new LuaMethod("SelfAdaptItem", SelfAdaptItem),
		new LuaMethod("GetShowNumberForLua", GetShowNumberForLua),
		new LuaMethod("GetShowNumber", GetShowNumber),
		new LuaMethod("SetEffectColor", SetEffectColor),
		new LuaMethod("ResetTweenAnim", ResetTweenAnim),
		new LuaMethod("PlayTweenAnim", PlayTweenAnim),
		new LuaMethod("WorldToScreenPos", WorldToScreenPos),
		new LuaMethod("PlayTweensInChildren", PlayTweensInChildren),
		new LuaMethod("StopTweensInChildren", StopTweensInChildren),
		new LuaMethod("GetTrans", GetTrans),
		new LuaMethod("LoadUISprite", LoadUISprite),
		new LuaMethod("SetActive", SetActive),
		new LuaMethod("SetActiveChildren", SetActiveChildren),
		new LuaMethod("SetVisible", SetVisible),
		new LuaMethod("SetSprite", SetSprite),
		new LuaMethod("SetLabelShadow", SetLabelShadow),
		new LuaMethod("SetLabelColor", SetLabelColor),
		new LuaMethod("MakePixelPerfect", MakePixelPerfect),
		new LuaMethod("GetTransByDepth", GetTransByDepth),
		new LuaMethod("SetLabel", SetLabel),
		new LuaMethod("SetNum", SetNum),
		new LuaMethod("SetFightLable", SetFightLable),
		new LuaMethod("SetLabelActive", SetLabelActive),
		new LuaMethod("SetChildTransQueue", SetChildTransQueue),
		new LuaMethod("GetStarString", GetStarString),
		new LuaMethod("SetGray", SetGray),
		new LuaMethod("SetActiveTween", SetActiveTween),
		new LuaMethod("SetCanTouched", SetCanTouched),
		new LuaMethod("Settouched", Settouched),
		new LuaMethod("SetUntouchable", SetUntouchable),
		new LuaMethod("SetFillSprite", SetFillSprite),
		new LuaMethod("SetColor", SetColor),
		new LuaMethod("GetColor", GetColor),
		new LuaMethod("SetSlider", SetSlider),
		new LuaMethod("SetAllActive", SetAllActive),
		new LuaMethod("ShowChild", ShowChild),
		new LuaMethod("SetClickable", SetClickable),
		new LuaMethod("ShowConfirm", ShowConfirm),
		new LuaMethod("ShowTips", ShowTips),
		new LuaMethod("PlayForceAnimation_Lua", PlayForceAnimation_Lua),
		new LuaMethod("LoadFx", LoadFx),
		new LuaMethod("AddParticleMask", AddParticleMask),
		new LuaMethod("LoadFxWork", LoadFxWork),
		new LuaMethod("GetUIWidgetLocalSize", GetUIWidgetLocalSize),
		new LuaMethod("RepositionGrid", RepositionGrid),
		new LuaMethod("SetDepth", SetDepth),
		new LuaMethod("ChangeDepth", ChangeDepth),
		new LuaMethod("New", _CreateBaseUI),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("CLOSE_STATE", get_CLOSE_STATE, null),
		new LuaField("m_currentWindowEnum", get_m_currentWindowEnum, set_m_currentWindowEnum),
		new LuaField("m_windowName", get_m_windowName, set_m_windowName),
		new LuaField("m_open", get_m_open, set_m_open),
		new LuaField("grayTitleStr", get_grayTitleStr, set_grayTitleStr),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateBaseUI(IntPtr L)
	{
		LuaDLL.luaL_error(L, "BaseUI class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(BaseUI));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "BaseUI", typeof(BaseUI), regs, fields, "UnityEngine.MonoBehaviour");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_CLOSE_STATE(IntPtr L)
	{
		LuaScriptMgr.Push(L, BaseUI.CLOSE_STATE);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_currentWindowEnum(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_currentWindowEnum");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_currentWindowEnum on a nil value");
			}
		}

		BaseUI obj = (BaseUI)o;
		LuaScriptMgr.PushEnum(L, obj.m_currentWindowEnum);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_windowName(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_windowName");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_windowName on a nil value");
			}
		}

		BaseUI obj = (BaseUI)o;
		LuaScriptMgr.Push(L, obj.m_windowName);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_open(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_open");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_open on a nil value");
			}
		}

		BaseUI obj = (BaseUI)o;
		LuaScriptMgr.Push(L, obj.m_open);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_grayTitleStr(IntPtr L)
	{
		LuaScriptMgr.Push(L, BaseUI.grayTitleStr);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_currentWindowEnum(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_currentWindowEnum");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_currentWindowEnum on a nil value");
			}
		}

		BaseUI obj = (BaseUI)o;
		obj.m_currentWindowEnum = LuaScriptMgr.GetNetObject<EZFunWindowEnum>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_windowName(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_windowName");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_windowName on a nil value");
			}
		}

		BaseUI obj = (BaseUI)o;
		obj.m_windowName = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_open(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_open");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_open on a nil value");
			}
		}

		BaseUI obj = (BaseUI)o;
		obj.m_open = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_grayTitleStr(IntPtr L)
	{
		BaseUI.grayTitleStr = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CreateUI(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		obj.CreateUI();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnDestroy(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		obj.OnDestroy();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CheckBtnCanClick(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		bool o = obj.CheckBtnCanClick(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CheckBtnCanClickBase(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		bool o = obj.CheckBtnCanClickBase(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int HandleWidgetBaseClick(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 2);
		obj.HandleWidgetBaseClick(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetOnClickCallback(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Action<GameObject> arg0 = LuaScriptMgr.GetNetObject<Action<GameObject>>(L, 1);
		BaseUI.SetOnClickCallback(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int TransformScreenSize(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		Vector2 o = obj.TransformScreenSize(arg0,arg1);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetBoardDistance(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		Vector4 o = obj.GetBoardDistance(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SelfAdaptScale(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 6);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		AdaptType arg1 = LuaScriptMgr.GetNetObject<AdaptType>(L, 3);
		AdaptType arg2 = LuaScriptMgr.GetNetObject<AdaptType>(L, 4);
		AdaptType arg3 = LuaScriptMgr.GetNetObject<AdaptType>(L, 5);
		AdaptType arg4 = LuaScriptMgr.GetNetObject<AdaptType>(L, 6);
		Vector3 o = obj.SelfAdaptScale(arg0,arg1,arg2,arg3,arg4);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SelfAdaptItem(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		obj.SelfAdaptItem(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetShowNumberForLua(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		string o = obj.GetShowNumberForLua(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetShowNumber(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		string o = obj.GetShowNumber(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetEffectColor(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 6);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		float arg1 = (float)LuaScriptMgr.GetNumber(L, 3);
		float arg2 = (float)LuaScriptMgr.GetNumber(L, 4);
		float arg3 = (float)LuaScriptMgr.GetNumber(L, 5);
		float arg4 = (float)LuaScriptMgr.GetNumber(L, 6);
		obj.SetEffectColor(arg0,arg1,arg2,arg3,arg4);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ResetTweenAnim(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		obj.ResetTweenAnim(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlayTweenAnim(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		obj.PlayTweenAnim(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WorldToScreenPos(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Vector3 arg0 = LuaScriptMgr.GetNetObject<Vector3>(L, 1);
		Vector3 o = BaseUI.WorldToScreenPos(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlayTweensInChildren(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		obj.PlayTweensInChildren(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int StopTweensInChildren(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		obj.StopTweensInChildren(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetTrans(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types1 = {typeof(BaseUI), typeof(Transform), typeof(string)};
		Type[] types2 = {typeof(BaseUI), typeof(string), typeof(bool)};
		Type[] types3 = {typeof(BaseUI), typeof(string), typeof(string)};

		if (count == 2)
		{
			BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
			string arg0 = LuaScriptMgr.GetLuaString(L, 2);
			Transform o = obj.GetTrans(arg0);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 3 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			string arg1 = LuaScriptMgr.GetString(L, 3);
			Transform o = obj.GetTrans(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 3 && LuaScriptMgr.CheckTypes(L, types2, 1))
		{
			BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
			string arg0 = LuaScriptMgr.GetString(L, 2);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
			Transform o = obj.GetTrans(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 3 && LuaScriptMgr.CheckTypes(L, types3, 1))
		{
			BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
			string arg0 = LuaScriptMgr.GetString(L, 2);
			string arg1 = LuaScriptMgr.GetString(L, 3);
			Transform o = obj.GetTrans(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: BaseUI.GetTrans");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadUISprite(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		string arg1 = LuaScriptMgr.GetLuaString(L, 3);
		obj.LoadUISprite(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetActive(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		obj.SetActive(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetActiveChildren(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		obj.SetActiveChildren(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetVisible(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		obj.SetVisible(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetSprite(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 3)
		{
			BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			string arg1 = LuaScriptMgr.GetLuaString(L, 3);
			obj.SetSprite(arg0,arg1);
			return 0;
		}
		else if (count == 4)
		{
			BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
			string arg0 = LuaScriptMgr.GetLuaString(L, 2);
			string arg1 = LuaScriptMgr.GetLuaString(L, 3);
			string arg2 = LuaScriptMgr.GetLuaString(L, 4);
			obj.SetSprite(arg0,arg1,arg2);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: BaseUI.SetSprite");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLabelShadow(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		Color arg1 = LuaScriptMgr.GetNetObject<Color>(L, 3);
		obj.SetLabelShadow(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLabelColor(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		Color arg1 = LuaScriptMgr.GetNetObject<Color>(L, 3);
		obj.SetLabelColor(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int MakePixelPerfect(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		obj.MakePixelPerfect(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetTransByDepth(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		Transform o = obj.GetTransByDepth(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLabel(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(BaseUI), typeof(Transform), typeof(int)};
		Type[] types1 = {typeof(BaseUI), typeof(Transform), typeof(string)};
		Type[] types2 = {typeof(BaseUI), typeof(Transform), typeof(string)};
		Type[] types3 = {typeof(BaseUI), typeof(string), typeof(string), typeof(string)};
		Type[] types4 = {typeof(BaseUI), typeof(Transform), typeof(string), typeof(bool)};

		if (count == 3 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
			obj.SetLabel(arg0,arg1);
			return 0;
		}
		else if (count == 3 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			string arg1 = LuaScriptMgr.GetString(L, 3);
			obj.SetLabel(arg0,arg1);
			return 0;
		}
		else if (LuaScriptMgr.CheckTypes(L, types2, 1) && LuaScriptMgr.CheckParamsType(L, typeof(string), 4, count - 3))
		{
			BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			string arg1 = LuaScriptMgr.GetString(L, 3);
			string[] objs2 = LuaScriptMgr.GetParamsString(L, 4, count - 3);
			obj.SetLabel(arg0,arg1,objs2);
			return 0;
		}
		else if (count == 4 && LuaScriptMgr.CheckTypes(L, types3, 1))
		{
			BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
			string arg0 = LuaScriptMgr.GetString(L, 2);
			string arg1 = LuaScriptMgr.GetString(L, 3);
			string arg2 = LuaScriptMgr.GetString(L, 4);
			obj.SetLabel(arg0,arg1,arg2);
			return 0;
		}
		else if (count == 4 && LuaScriptMgr.CheckTypes(L, types4, 1))
		{
			BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			string arg1 = LuaScriptMgr.GetString(L, 3);
			bool arg2 = LuaScriptMgr.GetBoolean(L, 4);
			obj.SetLabel(arg0,arg1,arg2);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: BaseUI.SetLabel");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetNum(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		obj.SetNum(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetFightLable(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		obj.SetFightLable(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLabelActive(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		string arg1 = LuaScriptMgr.GetLuaString(L, 3);
		obj.SetLabelActive(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetChildTransQueue(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		obj.SetChildTransQueue(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetStarString(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		string o = obj.GetStarString(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetGray(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 3)
		{
			BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
			UIWidget arg0 = LuaScriptMgr.GetNetObject<UIWidget>(L, 2);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
			obj.SetGray(arg0,arg1);
			return 0;
		}
		else if (count == 4)
		{
			BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
			bool arg2 = LuaScriptMgr.GetBoolean(L, 4);
			obj.SetGray(arg0,arg1,arg2);
			return 0;
		}
		else if (count == 5)
		{
			BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
			bool arg2 = LuaScriptMgr.GetBoolean(L, 4);
			bool arg3 = LuaScriptMgr.GetBoolean(L, 5);
			obj.SetGray(arg0,arg1,arg2,arg3);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: BaseUI.SetGray");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetActiveTween(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		obj.SetActiveTween(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetCanTouched(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		bool arg2 = LuaScriptMgr.GetBoolean(L, 4);
		obj.SetCanTouched(arg0,arg1,arg2);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Settouched(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		obj.Settouched(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetUntouchable(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		obj.SetUntouchable(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetFillSprite(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		float arg1 = (float)LuaScriptMgr.GetNumber(L, 3);
		bool arg2 = LuaScriptMgr.GetBoolean(L, 4);
		obj.SetFillSprite(arg0,arg1,arg2);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetColor(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(BaseUI), typeof(Transform), typeof(string)};
		Type[] types1 = {typeof(BaseUI), typeof(Transform), typeof(Color)};

		if (count == 3 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			string arg1 = LuaScriptMgr.GetString(L, 3);
			obj.SetColor(arg0,arg1);
			return 0;
		}
		else if (count == 3 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			Color arg1 = LuaScriptMgr.GetNetObject<Color>(L, 3);
			obj.SetColor(arg0,arg1);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: BaseUI.SetColor");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetColor(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		Color o = obj.GetColor(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetSlider(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		float arg1 = (float)LuaScriptMgr.GetNumber(L, 3);
		Transform arg2 = LuaScriptMgr.GetNetObject<Transform>(L, 4);
		obj.SetSlider(arg0,arg1,arg2);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetAllActive(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		obj.SetAllActive(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ShowChild(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(BaseUI), typeof(Transform), typeof(List<string>), typeof(bool)};
		Type[] types1 = {typeof(BaseUI), typeof(Transform), typeof(string), typeof(bool)};

		if (count == 4 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			List<string> arg1 = LuaScriptMgr.GetNetObject<List<string>>(L, 3);
			bool arg2 = LuaScriptMgr.GetBoolean(L, 4);
			obj.ShowChild(arg0,arg1,arg2);
			return 0;
		}
		else if (count == 4 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			string arg1 = LuaScriptMgr.GetString(L, 3);
			bool arg2 = LuaScriptMgr.GetBoolean(L, 4);
			obj.ShowChild(arg0,arg1,arg2);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: BaseUI.ShowChild");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetClickable(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		obj.SetClickable(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ShowConfirm(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		HandleErrorWindow.CallBack arg1 = LuaScriptMgr.GetNetObject<HandleErrorWindow.CallBack>(L, 2);
		HandleErrorWindow.CallBack arg2 = LuaScriptMgr.GetNetObject<HandleErrorWindow.CallBack>(L, 3);
		BaseUI.ShowConfirm(arg0,arg1,arg2);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ShowTips(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types1 = {typeof(int)};

		if (count == 1)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			BaseUI.ShowTips(arg0);
			return 0;
		}
		else if (LuaScriptMgr.CheckTypes(L, types1, 1) && LuaScriptMgr.CheckParamsType(L, typeof(string), 2, count - 1))
		{
			int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
			string[] objs1 = LuaScriptMgr.GetParamsString(L, 2, count - 1);
			BaseUI.ShowTips(arg0,objs1);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: BaseUI.ShowTips");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlayForceAnimation_Lua(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(BaseUI), typeof(Transform), typeof(LuaInterface.LuaFunction), typeof(bool), typeof(bool)};
		Type[] types1 = {typeof(BaseUI), typeof(Transform), typeof(string), typeof(bool), typeof(bool)};

		if (count == 5 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			LuaFunction arg1 = LuaScriptMgr.GetLuaFunction(L, 3);
			bool arg2 = LuaScriptMgr.GetBoolean(L, 4);
			bool arg3 = LuaScriptMgr.GetBoolean(L, 5);
			obj.PlayForceAnimation_Lua(arg0,arg1,arg2,arg3);
			return 0;
		}
		else if (count == 5 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			string arg1 = LuaScriptMgr.GetString(L, 3);
			bool arg2 = LuaScriptMgr.GetBoolean(L, 4);
			bool arg3 = LuaScriptMgr.GetBoolean(L, 5);
			obj.PlayForceAnimation_Lua(arg0,arg1,arg2,arg3);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: BaseUI.PlayForceAnimation_Lua");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadFx(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		Transform arg1 = LuaScriptMgr.GetNetObject<Transform>(L, 3);
		bool arg2 = LuaScriptMgr.GetBoolean(L, 4);
		Transform o = obj.LoadFx(arg0,arg1,arg2);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddParticleMask(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		obj.AddParticleMask(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadFxWork(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 6);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		Transform arg1 = LuaScriptMgr.GetNetObject<Transform>(L, 3);
		Vector3 arg2 = LuaScriptMgr.GetNetObject<Vector3>(L, 4);
		Vector3 arg3 = LuaScriptMgr.GetNetObject<Vector3>(L, 5);
		bool arg4 = LuaScriptMgr.GetBoolean(L, 6);
		Transform o = obj.LoadFxWork(arg0,arg1,arg2,arg3,arg4);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetUIWidgetLocalSize(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		Vector2 o = obj.GetUIWidgetLocalSize(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RepositionGrid(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 3);
		obj.RepositionGrid(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetDepth(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		obj.SetDepth(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ChangeDepth(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BaseUI obj = LuaScriptMgr.GetNetObject<BaseUI>(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		obj.ChangeDepth(arg0,arg1);
		return 0;
	}
}

