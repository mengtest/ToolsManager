using System;
using UnityEngine;
using LuaInterface;
using Object = UnityEngine.Object;

public class NGUIToolsWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("PlaySound", PlaySound),
		new LuaMethod("RandomRange", RandomRange),
		new LuaMethod("GetHierarchy", GetHierarchy),
		new LuaMethod("FindActive", FindActive),
		new LuaMethod("InsertNguiItem", InsertNguiItem),
		new LuaMethod("RemoveNguiItem", RemoveNguiItem),
		new LuaMethod("InsertCamera", InsertCamera),
		new LuaMethod("RemoveCamera", RemoveCamera),
		new LuaMethod("FindCameraForLayer", FindCameraForLayer),
		new LuaMethod("AddWidgetCollider", AddWidgetCollider),
		new LuaMethod("UpdateWidgetCollider", UpdateWidgetCollider),
		new LuaMethod("GetTypeName", GetTypeName),
		new LuaMethod("RegisterUndo", RegisterUndo),
		new LuaMethod("SetDirty", SetDirty),
		new LuaMethod("AddChild", AddChild),
		new LuaMethod("CalculateRaycastDepth", CalculateRaycastDepth),
		new LuaMethod("CalculateNextDepth", CalculateNextDepth),
		new LuaMethod("AdjustDepth", AdjustDepth),
		new LuaMethod("BringForward", BringForward),
		new LuaMethod("PushBack", PushBack),
		new LuaMethod("NormalizeDepths", NormalizeDepths),
		new LuaMethod("NormalizeWidgetDepths", NormalizeWidgetDepths),
		new LuaMethod("NormalizePanelDepths", NormalizePanelDepths),
		new LuaMethod("CreateUI", CreateUI),
		new LuaMethod("SetChildLayer", SetChildLayer),
		new LuaMethod("SetPanelLayer", SetPanelLayer),
		new LuaMethod("AddSprite", AddSprite),
		new LuaMethod("GetRoot", GetRoot),
		new LuaMethod("Destroy", Destroy),
		new LuaMethod("DestroyChildren", DestroyChildren),
		new LuaMethod("DestroyImmediate", DestroyImmediate),
		new LuaMethod("Broadcast", Broadcast),
		new LuaMethod("IsChild", IsChild),
		new LuaMethod("SetActive", SetActive),
		new LuaMethod("SetActiveChildren", SetActiveChildren),
		new LuaMethod("GetActive", GetActive),
		new LuaMethod("SetActiveSelf", SetActiveSelf),
		new LuaMethod("SetLayer", SetLayer),
		new LuaMethod("Round", Round),
		new LuaMethod("MakePixelPerfect", MakePixelPerfect),
		new LuaMethod("Save", Save),
		new LuaMethod("Load", Load),
		new LuaMethod("ApplyPMA", ApplyPMA),
		new LuaMethod("MarkParentAsChanged", MarkParentAsChanged),
		new LuaMethod("GetSides", GetSides),
		new LuaMethod("GetWorldCorners", GetWorldCorners),
		new LuaMethod("GetFuncName", GetFuncName),
		new LuaMethod("ClearDynamicUIDic", ClearDynamicUIDic),
		new LuaMethod("GetFontByName", GetFontByName),
		new LuaMethod("GetAtlasByName", GetAtlasByName),
		new LuaMethod("GetUIFontByName", GetUIFontByName),
		new LuaMethod("GetTextureByName", GetTextureByName),
		new LuaMethod("ImmediatelyCreateDrawCalls", ImmediatelyCreateDrawCalls),
		new LuaMethod("KeyToCaption", KeyToCaption),
		new LuaMethod("New", _CreateNGUITools),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("mDefaultFontName", get_mDefaultFontName, set_mDefaultFontName),
		new LuaField("mGetFontCB", get_mGetFontCB, set_mGetFontCB),
		new LuaField("mGetAtalsCB", get_mGetAtalsCB, set_mGetAtalsCB),
		new LuaField("mGetUIFontCB", get_mGetUIFontCB, set_mGetUIFontCB),
		new LuaField("mGetTextureCB", get_mGetTextureCB, set_mGetTextureCB),
		new LuaField("keys", get_keys, set_keys),
		new LuaField("soundVolume", get_soundVolume, set_soundVolume),
		new LuaField("fileAccess", get_fileAccess, null),
		new LuaField("clipboard", get_clipboard, set_clipboard),
		new LuaField("screenSize", get_screenSize, null),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateNGUITools(IntPtr L)
	{
		LuaDLL.luaL_error(L, "NGUITools class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(NGUITools));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "NGUITools", typeof(NGUITools), regs, fields, null);
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_mDefaultFontName(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUITools.mDefaultFontName);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_mGetFontCB(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, NGUITools.mGetFontCB);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_mGetAtalsCB(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, NGUITools.mGetAtalsCB);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_mGetUIFontCB(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, NGUITools.mGetUIFontCB);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_mGetTextureCB(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, NGUITools.mGetTextureCB);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_keys(IntPtr L)
	{
		LuaScriptMgr.PushArray(L, NGUITools.keys);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_soundVolume(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUITools.soundVolume);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_fileAccess(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUITools.fileAccess);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_clipboard(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUITools.clipboard);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_screenSize(IntPtr L)
	{
		LuaScriptMgr.PushValue(L, NGUITools.screenSize);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_mDefaultFontName(IntPtr L)
	{
		NGUITools.mDefaultFontName = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_mGetFontCB(IntPtr L)
	{
		NGUITools.mGetFontCB = LuaScriptMgr.GetNetObject<NGUITools.GetFontCB>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_mGetAtalsCB(IntPtr L)
	{
		NGUITools.mGetAtalsCB = LuaScriptMgr.GetNetObject<NGUITools.GetAtlasCB>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_mGetUIFontCB(IntPtr L)
	{
		NGUITools.mGetUIFontCB = LuaScriptMgr.GetNetObject<NGUITools.GetUIFontCB>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_mGetTextureCB(IntPtr L)
	{
		NGUITools.mGetTextureCB = LuaScriptMgr.GetNetObject<NGUITools.GetTextureCB>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_keys(IntPtr L)
	{
		NGUITools.keys = LuaScriptMgr.GetNetObject<KeyCode[]>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_soundVolume(IntPtr L)
	{
		NGUITools.soundVolume = (float)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_clipboard(IntPtr L)
	{
		NGUITools.clipboard = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlaySound(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 1)
		{
			AudioClip arg0 = LuaScriptMgr.GetNetObject<AudioClip>(L, 1);
			AudioSource o = NGUITools.PlaySound(arg0);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2)
		{
			AudioClip arg0 = LuaScriptMgr.GetNetObject<AudioClip>(L, 1);
			float arg1 = (float)LuaScriptMgr.GetNumber(L, 2);
			AudioSource o = NGUITools.PlaySound(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 3)
		{
			AudioClip arg0 = LuaScriptMgr.GetNetObject<AudioClip>(L, 1);
			float arg1 = (float)LuaScriptMgr.GetNumber(L, 2);
			float arg2 = (float)LuaScriptMgr.GetNumber(L, 3);
			AudioSource o = NGUITools.PlaySound(arg0,arg1,arg2);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: NGUITools.PlaySound");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RandomRange(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		int o = NGUITools.RandomRange(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetHierarchy(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
		string o = NGUITools.GetHierarchy(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int FindActive(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
		MonoBehaviour[] o = NGUITools.FindActive(arg0);
		LuaScriptMgr.PushArray(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InsertNguiItem(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		MonoBehaviour arg0 = LuaScriptMgr.GetNetObject<MonoBehaviour>(L, 1);
		NGUITools.InsertNguiItem(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RemoveNguiItem(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		MonoBehaviour arg0 = LuaScriptMgr.GetNetObject<MonoBehaviour>(L, 1);
		NGUITools.RemoveNguiItem(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InsertCamera(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Camera arg0 = LuaScriptMgr.GetNetObject<Camera>(L, 1);
		NGUITools.InsertCamera(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RemoveCamera(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Camera arg0 = LuaScriptMgr.GetNetObject<Camera>(L, 1);
		NGUITools.RemoveCamera(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int FindCameraForLayer(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
		Camera o = NGUITools.FindCameraForLayer(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddWidgetCollider(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 1)
		{
			GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
			NGUITools.AddWidgetCollider(arg0);
			return 0;
		}
		else if (count == 2)
		{
			GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 2);
			NGUITools.AddWidgetCollider(arg0,arg1);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: NGUITools.AddWidgetCollider");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UpdateWidgetCollider(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types1 = {typeof(BoxCollider2D), typeof(bool)};
		Type[] types2 = {typeof(BoxCollider), typeof(bool)};
		Type[] types3 = {typeof(GameObject), typeof(bool)};

		if (count == 1)
		{
			GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
			NGUITools.UpdateWidgetCollider(arg0);
			return 0;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			BoxCollider2D arg0 = LuaScriptMgr.GetNetObject<BoxCollider2D>(L, 1);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 2);
			NGUITools.UpdateWidgetCollider(arg0,arg1);
			return 0;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types2, 1))
		{
			BoxCollider arg0 = LuaScriptMgr.GetNetObject<BoxCollider>(L, 1);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 2);
			NGUITools.UpdateWidgetCollider(arg0,arg1);
			return 0;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types3, 1))
		{
			GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 2);
			NGUITools.UpdateWidgetCollider(arg0,arg1);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: NGUITools.UpdateWidgetCollider");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetTypeName(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Object arg0 = LuaScriptMgr.GetNetObject<Object>(L, 1);
		string o = NGUITools.GetTypeName(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RegisterUndo(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Object arg0 = LuaScriptMgr.GetNetObject<Object>(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		NGUITools.RegisterUndo(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetDirty(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Object arg0 = LuaScriptMgr.GetNetObject<Object>(L, 1);
		NGUITools.SetDirty(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddChild(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types1 = {typeof(GameObject), typeof(GameObject)};
		Type[] types2 = {typeof(GameObject), typeof(bool)};

		if (count == 1)
		{
			GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
			GameObject o = NGUITools.AddChild(arg0);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
			GameObject arg1 = LuaScriptMgr.GetNetObject<GameObject>(L, 2);
			GameObject o = NGUITools.AddChild(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types2, 1))
		{
			GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 2);
			GameObject o = NGUITools.AddChild(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: NGUITools.AddChild");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CalculateRaycastDepth(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
		int o = NGUITools.CalculateRaycastDepth(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CalculateNextDepth(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 1)
		{
			GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
			int o = NGUITools.CalculateNextDepth(arg0);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2)
		{
			GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 2);
			int o = NGUITools.CalculateNextDepth(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: NGUITools.CalculateNextDepth");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AdjustDepth(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		int o = NGUITools.AdjustDepth(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int BringForward(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
		NGUITools.BringForward(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PushBack(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
		NGUITools.PushBack(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int NormalizeDepths(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		NGUITools.NormalizeDepths();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int NormalizeWidgetDepths(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types1 = {typeof(UIWidget[])};
		Type[] types2 = {typeof(GameObject)};

		if (count == 0)
		{
			NGUITools.NormalizeWidgetDepths();
			return 0;
		}
		else if (count == 1 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			UIWidget[] objs0 = LuaScriptMgr.GetArrayObject<UIWidget>(L, 1);
			NGUITools.NormalizeWidgetDepths(objs0);
			return 0;
		}
		else if (count == 1 && LuaScriptMgr.CheckTypes(L, types2, 1))
		{
			GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
			NGUITools.NormalizeWidgetDepths(arg0);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: NGUITools.NormalizeWidgetDepths");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int NormalizePanelDepths(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		NGUITools.NormalizePanelDepths();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CreateUI(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 1)
		{
			bool arg0 = LuaScriptMgr.GetBoolean(L, 1);
			UIPanel o = NGUITools.CreateUI(arg0);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2)
		{
			bool arg0 = LuaScriptMgr.GetBoolean(L, 1);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
			UIPanel o = NGUITools.CreateUI(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 3)
		{
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 2);
			int arg2 = (int)LuaScriptMgr.GetNumber(L, 3);
			UIPanel o = NGUITools.CreateUI(arg0,arg1,arg2);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: NGUITools.CreateUI");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetChildLayer(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		NGUITools.SetChildLayer(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetPanelLayer(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		NGUITools.SetPanelLayer(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddSprite(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
		UIAtlas arg1 = LuaScriptMgr.GetNetObject<UIAtlas>(L, 2);
		string arg2 = LuaScriptMgr.GetLuaString(L, 3);
		int arg3 = (int)LuaScriptMgr.GetNumber(L, 4);
		UISprite o = NGUITools.AddSprite(arg0,arg1,arg2,arg3);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetRoot(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
		GameObject o = NGUITools.GetRoot(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Destroy(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Object arg0 = LuaScriptMgr.GetNetObject<Object>(L, 1);
		NGUITools.Destroy(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DestroyChildren(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		NGUITools.DestroyChildren(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DestroyImmediate(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Object arg0 = LuaScriptMgr.GetNetObject<Object>(L, 1);
		NGUITools.DestroyImmediate(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Broadcast(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 1)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			NGUITools.Broadcast(arg0);
			return 0;
		}
		else if (count == 2)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			object arg1 = LuaScriptMgr.GetVarObject(L, 2);
			NGUITools.Broadcast(arg0,arg1);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: NGUITools.Broadcast");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IsChild(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		Transform arg1 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		bool o = NGUITools.IsChild(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetActive(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 2)
		{
			GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 2);
			NGUITools.SetActive(arg0,arg1);
			return 0;
		}
		else if (count == 3)
		{
			GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 2);
			bool arg2 = LuaScriptMgr.GetBoolean(L, 3);
			NGUITools.SetActive(arg0,arg1,arg2);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: NGUITools.SetActive");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetActiveChildren(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 2);
		NGUITools.SetActiveChildren(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetActive(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(GameObject)};
		Type[] types1 = {typeof(Behaviour)};

		if (count == 1 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
			bool o = NGUITools.GetActive(arg0);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 1 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			Behaviour arg0 = LuaScriptMgr.GetNetObject<Behaviour>(L, 1);
			bool o = NGUITools.GetActive(arg0);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: NGUITools.GetActive");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetActiveSelf(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 2);
		NGUITools.SetActiveSelf(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLayer(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		NGUITools.SetLayer(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Round(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Vector3 arg0 = LuaScriptMgr.GetNetObject<Vector3>(L, 1);
		Vector3 o = NGUITools.Round(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int MakePixelPerfect(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		NGUITools.MakePixelPerfect(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Save(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		byte[] objs1 = LuaScriptMgr.GetArrayNumber<byte>(L, 2);
		bool o = NGUITools.Save(arg0,objs1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Load(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		byte[] o = NGUITools.Load(arg0);
		LuaScriptMgr.PushArray(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ApplyPMA(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Color arg0 = LuaScriptMgr.GetNetObject<Color>(L, 1);
		Color o = NGUITools.ApplyPMA(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int MarkParentAsChanged(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
		NGUITools.MarkParentAsChanged(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetSides(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types1 = {typeof(Camera), typeof(Transform)};
		Type[] types2 = {typeof(Camera), typeof(float)};

		if (count == 1)
		{
			Camera arg0 = LuaScriptMgr.GetNetObject<Camera>(L, 1);
			Vector3[] o = NGUITools.GetSides(arg0);
			LuaScriptMgr.PushArray(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			Camera arg0 = LuaScriptMgr.GetNetObject<Camera>(L, 1);
			Transform arg1 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			Vector3[] o = NGUITools.GetSides(arg0,arg1);
			LuaScriptMgr.PushArray(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types2, 1))
		{
			Camera arg0 = LuaScriptMgr.GetNetObject<Camera>(L, 1);
			float arg1 = (float)LuaScriptMgr.GetNumber(L, 2);
			Vector3[] o = NGUITools.GetSides(arg0,arg1);
			LuaScriptMgr.PushArray(L, o);
			return 1;
		}
		else if (count == 3)
		{
			Camera arg0 = LuaScriptMgr.GetNetObject<Camera>(L, 1);
			float arg1 = (float)LuaScriptMgr.GetNumber(L, 2);
			Transform arg2 = LuaScriptMgr.GetNetObject<Transform>(L, 3);
			Vector3[] o = NGUITools.GetSides(arg0,arg1,arg2);
			LuaScriptMgr.PushArray(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: NGUITools.GetSides");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetWorldCorners(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types1 = {typeof(Camera), typeof(Transform)};
		Type[] types2 = {typeof(Camera), typeof(float)};

		if (count == 1)
		{
			Camera arg0 = LuaScriptMgr.GetNetObject<Camera>(L, 1);
			Vector3[] o = NGUITools.GetWorldCorners(arg0);
			LuaScriptMgr.PushArray(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			Camera arg0 = LuaScriptMgr.GetNetObject<Camera>(L, 1);
			Transform arg1 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
			Vector3[] o = NGUITools.GetWorldCorners(arg0,arg1);
			LuaScriptMgr.PushArray(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types2, 1))
		{
			Camera arg0 = LuaScriptMgr.GetNetObject<Camera>(L, 1);
			float arg1 = (float)LuaScriptMgr.GetNumber(L, 2);
			Vector3[] o = NGUITools.GetWorldCorners(arg0,arg1);
			LuaScriptMgr.PushArray(L, o);
			return 1;
		}
		else if (count == 3)
		{
			Camera arg0 = LuaScriptMgr.GetNetObject<Camera>(L, 1);
			float arg1 = (float)LuaScriptMgr.GetNumber(L, 2);
			Transform arg2 = LuaScriptMgr.GetNetObject<Transform>(L, 3);
			Vector3[] o = NGUITools.GetWorldCorners(arg0,arg1,arg2);
			LuaScriptMgr.PushArray(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: NGUITools.GetWorldCorners");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetFuncName(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		object arg0 = LuaScriptMgr.GetVarObject(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		string o = NGUITools.GetFuncName(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ClearDynamicUIDic(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		NGUITools.ClearDynamicUIDic();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetFontByName(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		Font o = NGUITools.GetFontByName(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetAtlasByName(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		UIAtlas o = NGUITools.GetAtlasByName(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetUIFontByName(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		UIFont o = NGUITools.GetUIFontByName(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetTextureByName(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		Texture o = NGUITools.GetTextureByName(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ImmediatelyCreateDrawCalls(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
		NGUITools.ImmediatelyCreateDrawCalls(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int KeyToCaption(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		KeyCode arg0 = LuaScriptMgr.GetNetObject<KeyCode>(L, 1);
		string o = NGUITools.KeyToCaption(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}
}

