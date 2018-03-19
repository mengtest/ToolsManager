using System;
using UnityEngine;
using LuaInterface;
using Object = UnityEngine.Object;

public class WrapSysWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("OpenURL", OpenURL),
		new LuaMethod("ComputeHash", ComputeHash),
		new LuaMethod("CNetSys_GetInstance", CNetSys_GetInstance),
		new LuaMethod("CNetSys_GetServerName", CNetSys_GetServerName),
		new LuaMethod("CNetSys_AddNetConnectInstance", CNetSys_AddNetConnectInstance),
		new LuaMethod("CNetSys_RemoveNetConnectInstance", CNetSys_RemoveNetConnectInstance),
		new LuaMethod("CNetSys_ConnectSvr", CNetSys_ConnectSvr),
		new LuaMethod("CNetSys_SendLuaNet", CNetSys_SendLuaNet),
		new LuaMethod("CNetSys_SendHttpLuaNet", CNetSys_SendHttpLuaNet),
		new LuaMethod("CNetSys_SetAccessToken", CNetSys_SetAccessToken),
		new LuaMethod("CNetSys_GiveUpAutoConnect", CNetSys_GiveUpAutoConnect),
		new LuaMethod("CNetSys_ResetAutoConnectt", CNetSys_ResetAutoConnectt),
		new LuaMethod("CNetSys_RestartHttpWork", CNetSys_RestartHttpWork),
		new LuaMethod("CNetSys_CheckNetWorkIsReachable", CNetSys_CheckNetWorkIsReachable),
		new LuaMethod("CNetSys_CloseHttpWork", CNetSys_CloseHttpWork),
		new LuaMethod("EventSys_AddEvent", EventSys_AddEvent),
		new LuaMethod("EventSys_AddEventNow", EventSys_AddEventNow),
		new LuaMethod("EventSys_GetInstance", EventSys_GetInstance),
		new LuaMethod("LuaRootSys_AddLuaEvent", LuaRootSys_AddLuaEvent),
		new LuaMethod("LuaRootSys_RemoveLuaEvent", LuaRootSys_RemoveLuaEvent),
		new LuaMethod("LuaRootSys_LoadPB", LuaRootSys_LoadPB),
		new LuaMethod("EZFunWindowMgr_SetWindowStatusForLua", EZFunWindowMgr_SetWindowStatusForLua),
		new LuaMethod("EZFunWindowMgr_SetWindowStatus", EZFunWindowMgr_SetWindowStatus),
		new LuaMethod("EZFunWindowMgr_GetScreenWidth", EZFunWindowMgr_GetScreenWidth),
		new LuaMethod("EZFunWindowMgr_GetScreenHeight", EZFunWindowMgr_GetScreenHeight),
		new LuaMethod("EZFunWindowMgr_getUnusedLayer", EZFunWindowMgr_getUnusedLayer),
		new LuaMethod("EZFunWindowMgr_CheckWindowOpen", EZFunWindowMgr_CheckWindowOpen),
		new LuaMethod("EZFunWindowMgr_InitWindowDic", EZFunWindowMgr_InitWindowDic),
		new LuaMethod("EZFunWindowMgr_RegisterWindowInfo", EZFunWindowMgr_RegisterWindowInfo),
		new LuaMethod("ResourceMgr_GetInstantiateAsset", ResourceMgr_GetInstantiateAsset),
		new LuaMethod("ResourceMgr_GetInsAssetInParent", ResourceMgr_GetInsAssetInParent),
		new LuaMethod("GameRoot_GetInstance", GameRoot_GetInstance),
		new LuaMethod("GameRoot_Screen_width", GameRoot_Screen_width),
		new LuaMethod("GameRoot_Screen_height", GameRoot_Screen_height),
		new LuaMethod("GameRoot_GetCurStateType", GameRoot_GetCurStateType),
		new LuaMethod("TextData_GetText", TextData_GetText),
		new LuaMethod("Util_Log", Util_Log),
		new LuaMethod("Util_LogError", Util_LogError),
		new LuaMethod("Util_LogWarning", Util_LogWarning),
		new LuaMethod("Util_SetParent", Util_SetParent),
		new LuaMethod("Version_CheckIsPrePublish", Version_CheckIsPrePublish),
		new LuaMethod("Version_GetQuestionnaireBegineTime", Version_GetQuestionnaireBegineTime),
		new LuaMethod("Version_SetQuestionnaireBegineTime", Version_SetQuestionnaireBegineTime),
		new LuaMethod("Version_GetQuestionnaireClicked", Version_GetQuestionnaireClicked),
		new LuaMethod("Version_SetQuestionnaireClicked", Version_SetQuestionnaireClicked),
		new LuaMethod("WindowBaseLua_LoadLua", WindowBaseLua_LoadLua),
		new LuaMethod("NGUITools_SetLayer", NGUITools_SetLayer),
		new LuaMethod("NGUITools_GetActive", NGUITools_GetActive),
		new LuaMethod("NGUITools_ActiveOutLine", NGUITools_ActiveOutLine),
		new LuaMethod("EZFunTools_ReadLua", EZFunTools_ReadLua),
		new LuaMethod("EZFunTools_GetSecToStr", EZFunTools_GetSecToStr),
		new LuaMethod("EZFunTools_GetSecToStrWithoutHour", EZFunTools_GetSecToStrWithoutHour),
		new LuaMethod("EZFunTools_LuaDecryptDES", EZFunTools_LuaDecryptDES),
		new LuaMethod("EZFunTools_LuaEncryptDES", EZFunTools_LuaEncryptDES),
		new LuaMethod("EZFunTools_GetCharacterCount", EZFunTools_GetCharacterCount),
		new LuaMethod("EZFunTools_SetCameraDepth", EZFunTools_SetCameraDepth),
		new LuaMethod("EZFunTools_GetLeftTimeStr", EZFunTools_GetLeftTimeStr),
		new LuaMethod("GetRealTime", GetRealTime),
		new LuaMethod("EZFunTools_GetOrAddComponent", EZFunTools_GetOrAddComponent),
		new LuaMethod("PortingForLua_GetClassType", PortingForLua_GetClassType),
		new LuaMethod("PortingForLua_GetOrAddCamera", PortingForLua_GetOrAddCamera),
		new LuaMethod("PortingForLua_GetOrAddUICamera", PortingForLua_GetOrAddUICamera),
		new LuaMethod("PortingForLua_BinaryMoveLeft", PortingForLua_BinaryMoveLeft),
		new LuaMethod("PortingForLua_BinaryMoveRight", PortingForLua_BinaryMoveRight),
		new LuaMethod("PortingForLua_PlayAnimInAnimator", PortingForLua_PlayAnimInAnimator),
		new LuaMethod("PortingForLua_PlayAnimEndPlayIdle", PortingForLua_PlayAnimEndPlayIdle),
		new LuaMethod("PortingForLua_PlayTweensInChildren", PortingForLua_PlayTweensInChildren),
		new LuaMethod("PortingForLua_PlayAnimationByName", PortingForLua_PlayAnimationByName),
		new LuaMethod("ScreenToWorldPoint", ScreenToWorldPoint),
		new LuaMethod("WorldToScreenPoint", WorldToScreenPoint),
		new LuaMethod("GetCurrentTouchPos", GetCurrentTouchPos),
		new LuaMethod("AudioSys_LoadBanks", AudioSys_LoadBanks),
		new LuaMethod("AudioSys_UnLoadBank", AudioSys_UnLoadBank),
		new LuaMethod("GetFmodAudioSys", GetFmodAudioSys),
		new LuaMethod("AudioSys_PlayEffect", AudioSys_PlayEffect),
		new LuaMethod("AudioSys_PauseMusic", AudioSys_PauseMusic),
		new LuaMethod("AudioSys_ResumeMusic", AudioSys_ResumeMusic),
		new LuaMethod("AudioSys_SetMusicVolumn", AudioSys_SetMusicVolumn),
		new LuaMethod("AudioSys_SetEffectVolumn", AudioSys_SetEffectVolumn),
		new LuaMethod("AudioSys_GetEffectVolumn", AudioSys_GetEffectVolumn),
		new LuaMethod("AudioSys_GetMusicVolumn", AudioSys_GetMusicVolumn),
		new LuaMethod("AudioSys_GetSaveEffectVolumn", AudioSys_GetSaveEffectVolumn),
		new LuaMethod("AudioSys_GetSaveMusicVolumn", AudioSys_GetSaveMusicVolumn),
		new LuaMethod("PauseVoice", PauseVoice),
		new LuaMethod("AudioSys_PlayMusic", AudioSys_PlayMusic),
		new LuaMethod("InitMusicVolumnSlider", InitMusicVolumnSlider),
		new LuaMethod("InitEffectVolumnSlider", InitEffectVolumnSlider),
		new LuaMethod("BeginDetecation", BeginDetecation),
		new LuaMethod("isMuted", isMuted),
		new LuaMethod("NetSys_SetForceOut", NetSys_SetForceOut),
		new LuaMethod("GetUILabelText", GetUILabelText),
		new LuaMethod("GetOrAddComponentBaseLua", GetOrAddComponentBaseLua),
		new LuaMethod("AddTimerEVentByLeftTime", AddTimerEVentByLeftTime),
		new LuaMethod("GetCurrentDateTime", GetCurrentDateTime),
		new LuaMethod("TimerSys_GetCurrentTimeStrHM", TimerSys_GetCurrentTimeStrHM),
		new LuaMethod("HandleErrorWindow_GetClassType", HandleErrorWindow_GetClassType),
		new LuaMethod("HandleErrorWindow_SetOkCallBack", HandleErrorWindow_SetOkCallBack),
		new LuaMethod("HandleErrorWindow_SetNoCallBack", HandleErrorWindow_SetNoCallBack),
		new LuaMethod("HandleErrorWindow_SetCancelCallBack", HandleErrorWindow_SetCancelCallBack),
		new LuaMethod("WeiXinLogin", WeiXinLogin),
		new LuaMethod("WeiXinLogout", WeiXinLogout),
		new LuaMethod("LoadPic", LoadPic),
		new LuaMethod("WeiXinShareLocalUrl", WeiXinShareLocalUrl),
		new LuaMethod("GetScreenTexturePic", GetScreenTexturePic),
		new LuaMethod("SetScreenShotWaitTime", SetScreenShotWaitTime),
		new LuaMethod("GetUmengSys", GetUmengSys),
		new LuaMethod("GetWifiRssi", GetWifiRssi),
		new LuaMethod("GetNetType", GetNetType),
		new LuaMethod("GetBattery", GetBattery),
		new LuaMethod("GetDeviceID", GetDeviceID),
		new LuaMethod("GetLatitude", GetLatitude),
		new LuaMethod("GetLongitude", GetLongitude),
		new LuaMethod("GetCountry", GetCountry),
		new LuaMethod("GetCountryCode", GetCountryCode),
		new LuaMethod("GetState", GetState),
		new LuaMethod("GetCity", GetCity),
		new LuaMethod("GetSubLocality", GetSubLocality),
		new LuaMethod("GetStreet", GetStreet),
		new LuaMethod("GetThoroughfare", GetThoroughfare),
		new LuaMethod("GetSubThoroughfare", GetSubThoroughfare),
		new LuaMethod("GetLocationName", GetLocationName),
		new LuaMethod("GetFormattedAddressLine", GetFormattedAddressLine),
		new LuaMethod("PlatInterface_RefreshBattery", PlatInterface_RefreshBattery),
		new LuaMethod("PlatInterface_RefreshLocation", PlatInterface_RefreshLocation),
		new LuaMethod("PlatInterface_RefreshNetStatus", PlatInterface_RefreshNetStatus),
		new LuaMethod("PlatInterface_CopyStr", PlatInterface_CopyStr),
		new LuaMethod("CIAPSys_InitUnlegalCountry", CIAPSys_InitUnlegalCountry),
		new LuaMethod("CIAPSys_PurchaseSucceed", CIAPSys_PurchaseSucceed),
		new LuaMethod("CIAPSys_SendBuyDiamond", CIAPSys_SendBuyDiamond),
		new LuaMethod("Bugly_SetUserID", Bugly_SetUserID),
		new LuaMethod("TouchBegin", TouchBegin),
		new LuaMethod("TouchEnd", TouchEnd),
		new LuaMethod("TouchIng", TouchIng),
		new LuaMethod("ConversAction", ConversAction),
		new LuaMethod("GetTimeValue", GetTimeValue),
		new LuaMethod("Reflection_CallMethod", Reflection_CallMethod),
		new LuaMethod("Reflection_CallMethodByObjName", Reflection_CallMethodByObjName),
		new LuaMethod("Reflection_GetProperty", Reflection_GetProperty),
		new LuaMethod("Reflection_GetField", Reflection_GetField),
		new LuaMethod("Reflection_SetField", Reflection_SetField),
		new LuaMethod("TestReflect", TestReflect),
		new LuaMethod("StringSafeConvert", StringSafeConvert),
		new LuaMethod("New", _CreateWrapSys),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("IsAndroid", get_IsAndroid, null),
		new LuaField("IsIOS", get_IsIOS, null),
		new LuaField("IsEditor", get_IsEditor, null),
		new LuaField("LuaPersistPath", get_LuaPersistPath, null),
		new LuaField("ResourceMgr_CacheTextureNum", get_ResourceMgr_CacheTextureNum, set_ResourceMgr_CacheTextureNum),
		new LuaField("GameRoot_GameTime", get_GameRoot_GameTime, null),
		new LuaField("Constants_RELEASE", get_Constants_RELEASE, null),
		new LuaField("UnityScreen_width", get_UnityScreen_width, null),
		new LuaField("UnityScreen_height", get_UnityScreen_height, null),
		new LuaField("HandleErrorWindow_TitleStr", null, set_HandleErrorWindow_TitleStr),
		new LuaField("HandleErrorWindow_ContentSt", null, set_HandleErrorWindow_ContentSt),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateWrapSys(IntPtr L)
	{
		LuaDLL.luaL_error(L, "WrapSys class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(WrapSys));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "WrapSys", typeof(WrapSys), regs, fields, null);
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_IsAndroid(IntPtr L)
	{
		LuaScriptMgr.Push(L, WrapSys.IsAndroid);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_IsIOS(IntPtr L)
	{
		LuaScriptMgr.Push(L, WrapSys.IsIOS);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_IsEditor(IntPtr L)
	{
		LuaScriptMgr.Push(L, WrapSys.IsEditor);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_LuaPersistPath(IntPtr L)
	{
		LuaScriptMgr.Push(L, WrapSys.LuaPersistPath);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ResourceMgr_CacheTextureNum(IntPtr L)
	{
		LuaScriptMgr.Push(L, WrapSys.ResourceMgr_CacheTextureNum);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_GameRoot_GameTime(IntPtr L)
	{
		LuaScriptMgr.Push(L, WrapSys.GameRoot_GameTime);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Constants_RELEASE(IntPtr L)
	{
		LuaScriptMgr.Push(L, WrapSys.Constants_RELEASE);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_UnityScreen_width(IntPtr L)
	{
		LuaScriptMgr.Push(L, WrapSys.UnityScreen_width);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_UnityScreen_height(IntPtr L)
	{
		LuaScriptMgr.Push(L, WrapSys.UnityScreen_height);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ResourceMgr_CacheTextureNum(IntPtr L)
	{
		WrapSys.ResourceMgr_CacheTextureNum = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_HandleErrorWindow_TitleStr(IntPtr L)
	{
		WrapSys.HandleErrorWindow_TitleStr = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_HandleErrorWindow_ContentSt(IntPtr L)
	{
		WrapSys.HandleErrorWindow_ContentSt = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OpenURL(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		WrapSys.OpenURL(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ComputeHash(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string o = WrapSys.ComputeHash(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CNetSys_GetInstance(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		object o = WrapSys.CNetSys_GetInstance();
		LuaScriptMgr.PushVarObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CNetSys_GetServerName(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string o = WrapSys.CNetSys_GetServerName();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CNetSys_AddNetConnectInstance(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		WrapSys.CNetSys_AddNetConnectInstance(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CNetSys_RemoveNetConnectInstance(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		WrapSys.CNetSys_RemoveNetConnectInstance(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CNetSys_ConnectSvr(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		int arg2 = (int)LuaScriptMgr.GetNumber(L, 3);
		WrapSys.CNetSys_ConnectSvr(arg0,arg1,arg2);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CNetSys_SendLuaNet(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 6)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
			int arg2 = (int)LuaScriptMgr.GetNumber(L, 3);
			int arg3 = (int)LuaScriptMgr.GetNumber(L, 4);
			bool arg4 = LuaScriptMgr.GetBoolean(L, 5);
			bool arg5 = LuaScriptMgr.GetBoolean(L, 6);
			WrapSys.CNetSys_SendLuaNet(arg0,arg1,arg2,arg3,arg4,arg5);
			return 0;
		}
		else if (count == 7)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
			int arg2 = (int)LuaScriptMgr.GetNumber(L, 3);
			int arg3 = (int)LuaScriptMgr.GetNumber(L, 4);
			bool arg4 = LuaScriptMgr.GetBoolean(L, 5);
			bool arg5 = LuaScriptMgr.GetBoolean(L, 6);
			LuaStringBuffer arg6 = LuaScriptMgr.GetStringBuffer(L, 7);
			WrapSys.CNetSys_SendLuaNet(arg0,arg1,arg2,arg3,arg4,arg5,arg6);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: WrapSys.CNetSys_SendLuaNet");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CNetSys_SendHttpLuaNet(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 6);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		string arg2 = LuaScriptMgr.GetLuaString(L, 3);
		bool arg3 = LuaScriptMgr.GetBoolean(L, 4);
		int arg4 = (int)LuaScriptMgr.GetNumber(L, 5);
		int arg5 = (int)LuaScriptMgr.GetNumber(L, 6);
		WrapSys.CNetSys_SendHttpLuaNet(arg0,arg1,arg2,arg3,arg4,arg5);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CNetSys_SetAccessToken(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		WrapSys.CNetSys_SetAccessToken(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CNetSys_GiveUpAutoConnect(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		WrapSys.CNetSys_GiveUpAutoConnect(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CNetSys_ResetAutoConnectt(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		WrapSys.CNetSys_ResetAutoConnectt(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CNetSys_RestartHttpWork(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		WrapSys.CNetSys_RestartHttpWork();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CNetSys_CheckNetWorkIsReachable(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		bool o = WrapSys.CNetSys_CheckNetWorkIsReachable();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CNetSys_CloseHttpWork(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		WrapSys.CNetSys_CloseHttpWork();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EventSys_AddEvent(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(int), typeof(object), typeof(object)};
		Type[] types1 = {typeof(EEventType), typeof(object), typeof(object)};

		if (count == 3 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
			object arg1 = LuaScriptMgr.GetVarObject(L, 2);
			object arg2 = LuaScriptMgr.GetVarObject(L, 3);
			WrapSys.EventSys_AddEvent(arg0,arg1,arg2);
			return 0;
		}
		else if (count == 3 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			EEventType arg0 = LuaScriptMgr.GetNetObject<EEventType>(L, 1);
			object arg1 = LuaScriptMgr.GetVarObject(L, 2);
			object arg2 = LuaScriptMgr.GetVarObject(L, 3);
			WrapSys.EventSys_AddEvent(arg0,arg1,arg2);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: WrapSys.EventSys_AddEvent");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EventSys_AddEventNow(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(int), typeof(object), typeof(object)};
		Type[] types1 = {typeof(EEventType), typeof(object), typeof(object)};

		if (count == 3 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
			object arg1 = LuaScriptMgr.GetVarObject(L, 2);
			object arg2 = LuaScriptMgr.GetVarObject(L, 3);
			WrapSys.EventSys_AddEventNow(arg0,arg1,arg2);
			return 0;
		}
		else if (count == 3 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			EEventType arg0 = LuaScriptMgr.GetNetObject<EEventType>(L, 1);
			object arg1 = LuaScriptMgr.GetVarObject(L, 2);
			object arg2 = LuaScriptMgr.GetVarObject(L, 3);
			WrapSys.EventSys_AddEventNow(arg0,arg1,arg2);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: WrapSys.EventSys_AddEventNow");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EventSys_GetInstance(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		object o = WrapSys.EventSys_GetInstance();
		LuaScriptMgr.PushVarObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LuaRootSys_AddLuaEvent(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
		WrapSys.LuaRootSys_AddLuaEvent(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LuaRootSys_RemoveLuaEvent(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
		WrapSys.LuaRootSys_RemoveLuaEvent(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LuaRootSys_LoadPB(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		int o = WrapSys.LuaRootSys_LoadPB(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EZFunWindowMgr_SetWindowStatusForLua(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 5)
		{
			EZFunWindowEnum arg0 = LuaScriptMgr.GetNetObject<EZFunWindowEnum>(L, 1);
			RessType arg1 = LuaScriptMgr.GetNetObject<RessType>(L, 2);
			bool arg2 = LuaScriptMgr.GetBoolean(L, 3);
			int arg3 = (int)LuaScriptMgr.GetNumber(L, 4);
			string arg4 = LuaScriptMgr.GetLuaString(L, 5);
			WrapSys.EZFunWindowMgr_SetWindowStatusForLua(arg0,arg1,arg2,arg3,arg4);
			return 0;
		}
		else if (count == 6)
		{
			EZFunWindowEnum arg0 = LuaScriptMgr.GetNetObject<EZFunWindowEnum>(L, 1);
			RessType arg1 = LuaScriptMgr.GetNetObject<RessType>(L, 2);
			bool arg2 = LuaScriptMgr.GetBoolean(L, 3);
			int arg3 = (int)LuaScriptMgr.GetNumber(L, 4);
			string arg4 = LuaScriptMgr.GetLuaString(L, 5);
			object arg5 = LuaScriptMgr.GetVarObject(L, 6);
			WrapSys.EZFunWindowMgr_SetWindowStatusForLua(arg0,arg1,arg2,arg3,arg4,arg5);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: WrapSys.EZFunWindowMgr_SetWindowStatusForLua");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EZFunWindowMgr_SetWindowStatus(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 4)
		{
			EZFunWindowEnum arg0 = LuaScriptMgr.GetNetObject<EZFunWindowEnum>(L, 1);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 2);
			int arg2 = (int)LuaScriptMgr.GetNumber(L, 3);
			string arg3 = LuaScriptMgr.GetLuaString(L, 4);
			WrapSys.EZFunWindowMgr_SetWindowStatus(arg0,arg1,arg2,arg3);
			return 0;
		}
		else if (count == 5)
		{
			EZFunWindowEnum arg0 = LuaScriptMgr.GetNetObject<EZFunWindowEnum>(L, 1);
			RessType arg1 = LuaScriptMgr.GetNetObject<RessType>(L, 2);
			bool arg2 = LuaScriptMgr.GetBoolean(L, 3);
			int arg3 = (int)LuaScriptMgr.GetNumber(L, 4);
			string arg4 = LuaScriptMgr.GetLuaString(L, 5);
			WrapSys.EZFunWindowMgr_SetWindowStatus(arg0,arg1,arg2,arg3,arg4);
			return 0;
		}
		else if (count == 6)
		{
			EZFunWindowEnum arg0 = LuaScriptMgr.GetNetObject<EZFunWindowEnum>(L, 1);
			bool arg1 = LuaScriptMgr.GetBoolean(L, 2);
			int arg2 = (int)LuaScriptMgr.GetNumber(L, 3);
			string arg3 = LuaScriptMgr.GetLuaString(L, 4);
			bool arg4 = LuaScriptMgr.GetBoolean(L, 5);
			object arg5 = LuaScriptMgr.GetVarObject(L, 6);
			WrapSys.EZFunWindowMgr_SetWindowStatus(arg0,arg1,arg2,arg3,arg4,arg5);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: WrapSys.EZFunWindowMgr_SetWindowStatus");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EZFunWindowMgr_GetScreenWidth(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		int o = WrapSys.EZFunWindowMgr_GetScreenWidth();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EZFunWindowMgr_GetScreenHeight(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		int o = WrapSys.EZFunWindowMgr_GetScreenHeight();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EZFunWindowMgr_getUnusedLayer(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
		Transform arg1 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		int o = WrapSys.EZFunWindowMgr_getUnusedLayer(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EZFunWindowMgr_CheckWindowOpen(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		EZFunWindowEnum arg0 = LuaScriptMgr.GetNetObject<EZFunWindowEnum>(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		bool o = WrapSys.EZFunWindowMgr_CheckWindowOpen(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EZFunWindowMgr_InitWindowDic(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		EZFunWindowEnum arg0 = LuaScriptMgr.GetNetObject<EZFunWindowEnum>(L, 1);
		RessType arg1 = LuaScriptMgr.GetNetObject<RessType>(L, 2);
		RessStorgeType arg2 = LuaScriptMgr.GetNetObject<RessStorgeType>(L, 3);
		string arg3 = LuaScriptMgr.GetLuaString(L, 4);
		bool o = WrapSys.EZFunWindowMgr_InitWindowDic(arg0,arg1,arg2,arg3);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EZFunWindowMgr_RegisterWindowInfo(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 5);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		string arg2 = LuaScriptMgr.GetLuaString(L, 3);
		bool arg3 = LuaScriptMgr.GetBoolean(L, 4);
		bool arg4 = LuaScriptMgr.GetBoolean(L, 5);
		WrapSys.EZFunWindowMgr_RegisterWindowInfo(arg0,arg1,arg2,arg3,arg4);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ResourceMgr_GetInstantiateAsset(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		RessType arg0 = LuaScriptMgr.GetNetObject<RessType>(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		RessStorgeType arg2 = LuaScriptMgr.GetNetObject<RessStorgeType>(L, 3);
		bool arg3 = LuaScriptMgr.GetBoolean(L, 4);
		GameObject o = WrapSys.ResourceMgr_GetInstantiateAsset(arg0,arg1,arg2,arg3);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ResourceMgr_GetInsAssetInParent(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		RessType arg0 = LuaScriptMgr.GetNetObject<RessType>(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		Transform arg2 = LuaScriptMgr.GetNetObject<Transform>(L, 3);
		RessStorgeType arg3 = LuaScriptMgr.GetNetObject<RessStorgeType>(L, 4);
		GameObject o = WrapSys.ResourceMgr_GetInsAssetInParent(arg0,arg1,arg2,arg3);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GameRoot_GetInstance(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		object o = WrapSys.GameRoot_GetInstance();
		LuaScriptMgr.PushVarObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GameRoot_Screen_width(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		float o = WrapSys.GameRoot_Screen_width();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GameRoot_Screen_height(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		float o = WrapSys.GameRoot_Screen_height();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GameRoot_GetCurStateType(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		EGameStateType o = WrapSys.GameRoot_GetCurStateType();
		LuaScriptMgr.PushEnum(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int TextData_GetText(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types1 = {typeof(string)};
		Type[] types2 = {typeof(int)};

		if (count == 1)
		{
			int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
			string o = WrapSys.TextData_GetText(arg0);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (LuaScriptMgr.CheckTypes(L, types1, 1) && LuaScriptMgr.CheckParamsType(L, typeof(string), 2, count - 1))
		{
			string arg0 = LuaScriptMgr.GetString(L, 1);
			string[] objs1 = LuaScriptMgr.GetParamsString(L, 2, count - 1);
			string o = WrapSys.TextData_GetText(arg0,objs1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (LuaScriptMgr.CheckTypes(L, types2, 1) && LuaScriptMgr.CheckParamsType(L, typeof(string), 2, count - 1))
		{
			int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
			string[] objs1 = LuaScriptMgr.GetParamsString(L, 2, count - 1);
			string o = WrapSys.TextData_GetText(arg0,objs1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: WrapSys.TextData_GetText");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Util_Log(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		WrapSys.Util_Log(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Util_LogError(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		WrapSys.Util_LogError(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Util_LogWarning(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		WrapSys.Util_LogWarning(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Util_SetParent(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		Transform arg1 = LuaScriptMgr.GetNetObject<Transform>(L, 2);
		WrapSys.Util_SetParent(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Version_CheckIsPrePublish(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		bool o = WrapSys.Version_CheckIsPrePublish();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Version_GetQuestionnaireBegineTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		int o = WrapSys.Version_GetQuestionnaireBegineTime();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Version_SetQuestionnaireBegineTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		WrapSys.Version_SetQuestionnaireBegineTime();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Version_GetQuestionnaireClicked(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		bool o = WrapSys.Version_GetQuestionnaireClicked();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Version_SetQuestionnaireClicked(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 1);
		WrapSys.Version_SetQuestionnaireClicked(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WindowBaseLua_LoadLua(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		WrapSys.WindowBaseLua_LoadLua(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int NGUITools_SetLayer(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		WrapSys.NGUITools_SetLayer(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int NGUITools_GetActive(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		bool o = WrapSys.NGUITools_GetActive(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int NGUITools_ActiveOutLine(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		bool arg1 = LuaScriptMgr.GetBoolean(L, 2);
		WrapSys.NGUITools_ActiveOutLine(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EZFunTools_ReadLua(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string o = WrapSys.EZFunTools_ReadLua(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EZFunTools_GetSecToStr(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
		string o = WrapSys.EZFunTools_GetSecToStr(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EZFunTools_GetSecToStrWithoutHour(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
		string o = WrapSys.EZFunTools_GetSecToStrWithoutHour(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EZFunTools_LuaDecryptDES(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		double o = WrapSys.EZFunTools_LuaDecryptDES(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EZFunTools_LuaEncryptDES(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		double arg0 = (double)LuaScriptMgr.GetNumber(L, 1);
		double o = WrapSys.EZFunTools_LuaEncryptDES(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EZFunTools_GetCharacterCount(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		int o = WrapSys.EZFunTools_GetCharacterCount(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EZFunTools_SetCameraDepth(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Camera arg0 = LuaScriptMgr.GetNetObject<Camera>(L, 1);
		float arg1 = (float)LuaScriptMgr.GetNumber(L, 2);
		WrapSys.EZFunTools_SetCameraDepth(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EZFunTools_GetLeftTimeStr(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
		string o = WrapSys.EZFunTools_GetLeftTimeStr(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetRealTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		float o = WrapSys.GetRealTime();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EZFunTools_GetOrAddComponent(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		Component o = WrapSys.EZFunTools_GetOrAddComponent(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PortingForLua_GetClassType(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		object o = WrapSys.PortingForLua_GetClassType();
		LuaScriptMgr.PushVarObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PortingForLua_GetOrAddCamera(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		Camera o = WrapSys.PortingForLua_GetOrAddCamera(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PortingForLua_GetOrAddUICamera(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		UICamera o = WrapSys.PortingForLua_GetOrAddUICamera(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PortingForLua_BinaryMoveLeft(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		int o = WrapSys.PortingForLua_BinaryMoveLeft(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PortingForLua_BinaryMoveRight(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		int o = WrapSys.PortingForLua_BinaryMoveRight(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PortingForLua_PlayAnimInAnimator(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		WrapSys.PortingForLua_PlayAnimInAnimator(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PortingForLua_PlayAnimEndPlayIdle(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		WrapSys.PortingForLua_PlayAnimEndPlayIdle(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PortingForLua_PlayTweensInChildren(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		WrapSys.PortingForLua_PlayTweensInChildren(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PortingForLua_PlayAnimationByName(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		WrapSys.PortingForLua_PlayAnimationByName(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ScreenToWorldPoint(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Camera arg0 = LuaScriptMgr.GetNetObject<Camera>(L, 1);
		Vector3 arg1 = LuaScriptMgr.GetNetObject<Vector3>(L, 2);
		Vector3 o = WrapSys.ScreenToWorldPoint(arg0,arg1);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WorldToScreenPoint(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Camera arg0 = LuaScriptMgr.GetNetObject<Camera>(L, 1);
		Vector3 arg1 = LuaScriptMgr.GetNetObject<Vector3>(L, 2);
		Vector3 o = WrapSys.WorldToScreenPoint(arg0,arg1);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrentTouchPos(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
		Vector3 o = WrapSys.GetCurrentTouchPos(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AudioSys_LoadBanks(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		WrapSys.AudioSys_LoadBanks(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AudioSys_UnLoadBank(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		WrapSys.AudioSys_UnLoadBank(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetFmodAudioSys(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		FmodAudioSys o = WrapSys.GetFmodAudioSys();
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AudioSys_PlayEffect(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		FMOD.Studio.EventInstance o = WrapSys.AudioSys_PlayEffect(arg0);
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AudioSys_PauseMusic(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		WrapSys.AudioSys_PauseMusic();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AudioSys_ResumeMusic(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		WrapSys.AudioSys_ResumeMusic();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AudioSys_SetMusicVolumn(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		float arg0 = (float)LuaScriptMgr.GetNumber(L, 1);
		WrapSys.AudioSys_SetMusicVolumn(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AudioSys_SetEffectVolumn(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		float arg0 = (float)LuaScriptMgr.GetNumber(L, 1);
		WrapSys.AudioSys_SetEffectVolumn(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AudioSys_GetEffectVolumn(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		float o = WrapSys.AudioSys_GetEffectVolumn();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AudioSys_GetMusicVolumn(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		float o = WrapSys.AudioSys_GetMusicVolumn();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AudioSys_GetSaveEffectVolumn(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		float o = WrapSys.AudioSys_GetSaveEffectVolumn();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AudioSys_GetSaveMusicVolumn(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		float o = WrapSys.AudioSys_GetSaveMusicVolumn();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PauseVoice(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 1);
		WrapSys.PauseVoice(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AudioSys_PlayMusic(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		WrapSys.AudioSys_PlayMusic(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitMusicVolumnSlider(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		WrapSys.InitMusicVolumnSlider(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitEffectVolumnSlider(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		WrapSys.InitEffectVolumnSlider(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int BeginDetecation(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		WrapSys.BeginDetecation();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int isMuted(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		bool o = WrapSys.isMuted();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int NetSys_SetForceOut(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 1);
		WrapSys.NetSys_SetForceOut(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetUILabelText(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		string o = WrapSys.GetUILabelText(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetOrAddComponentBaseLua(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
		ComponentBaseLua o = WrapSys.GetOrAddComponentBaseLua(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddTimerEVentByLeftTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		float arg0 = (float)LuaScriptMgr.GetNumber(L, 1);
		LuaFunction arg1 = LuaScriptMgr.GetLuaFunction(L, 2);
		WrapSys.AddTimerEVentByLeftTime(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrentDateTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		int o = WrapSys.GetCurrentDateTime();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int TimerSys_GetCurrentTimeStrHM(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string o = WrapSys.TimerSys_GetCurrentTimeStrHM();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int HandleErrorWindow_GetClassType(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		object o = WrapSys.HandleErrorWindow_GetClassType();
		LuaScriptMgr.PushVarObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int HandleErrorWindow_SetOkCallBack(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaFunction arg0 = LuaScriptMgr.GetLuaFunction(L, 1);
		WrapSys.HandleErrorWindow_SetOkCallBack(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int HandleErrorWindow_SetNoCallBack(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaFunction arg0 = LuaScriptMgr.GetLuaFunction(L, 1);
		WrapSys.HandleErrorWindow_SetNoCallBack(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int HandleErrorWindow_SetCancelCallBack(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaFunction arg0 = LuaScriptMgr.GetLuaFunction(L, 1);
		WrapSys.HandleErrorWindow_SetCancelCallBack(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WeiXinLogin(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		WrapSys.WeiXinLogin();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WeiXinLogout(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		WrapSys.WeiXinLogout();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadPic(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		LuaFunction arg2 = LuaScriptMgr.GetLuaFunction(L, 3);
		WrapSys.LoadPic(arg0,arg1,arg2);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WeiXinShareLocalUrl(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 6);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		string arg2 = LuaScriptMgr.GetLuaString(L, 3);
		string arg3 = LuaScriptMgr.GetLuaString(L, 4);
		string arg4 = LuaScriptMgr.GetLuaString(L, 5);
		LuaFunction arg5 = LuaScriptMgr.GetLuaFunction(L, 6);
		WrapSys.WeiXinShareLocalUrl(arg0,arg1,arg2,arg3,arg4,arg5);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetScreenTexturePic(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 6)
		{
			int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
			int arg2 = (int)LuaScriptMgr.GetNumber(L, 3);
			int arg3 = (int)LuaScriptMgr.GetNumber(L, 4);
			string arg4 = LuaScriptMgr.GetLuaString(L, 5);
			LuaFunction arg5 = LuaScriptMgr.GetLuaFunction(L, 6);
			WrapSys.GetScreenTexturePic(arg0,arg1,arg2,arg3,arg4,arg5);
			return 0;
		}
		else if (count == 7)
		{
			Camera arg0 = LuaScriptMgr.GetNetObject<Camera>(L, 1);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
			int arg2 = (int)LuaScriptMgr.GetNumber(L, 3);
			int arg3 = (int)LuaScriptMgr.GetNumber(L, 4);
			int arg4 = (int)LuaScriptMgr.GetNumber(L, 5);
			string arg5 = LuaScriptMgr.GetLuaString(L, 6);
			LuaFunction arg6 = LuaScriptMgr.GetLuaFunction(L, 7);
			WrapSys.GetScreenTexturePic(arg0,arg1,arg2,arg3,arg4,arg5,arg6);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: WrapSys.GetScreenTexturePic");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetScreenShotWaitTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		float arg0 = (float)LuaScriptMgr.GetNumber(L, 1);
		WrapSys.SetScreenShotWaitTime(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetUmengSys(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		UMengSys o = WrapSys.GetUmengSys();
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetWifiRssi(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		int o = WrapSys.GetWifiRssi();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetNetType(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		int o = WrapSys.GetNetType();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetBattery(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		double o = WrapSys.GetBattery();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetDeviceID(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string o = WrapSys.GetDeviceID();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLatitude(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string o = WrapSys.GetLatitude();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLongitude(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string o = WrapSys.GetLongitude();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCountry(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string o = WrapSys.GetCountry();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCountryCode(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string o = WrapSys.GetCountryCode();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetState(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string o = WrapSys.GetState();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCity(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string o = WrapSys.GetCity();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetSubLocality(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string o = WrapSys.GetSubLocality();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetStreet(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string o = WrapSys.GetStreet();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetThoroughfare(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string o = WrapSys.GetThoroughfare();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetSubThoroughfare(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string o = WrapSys.GetSubThoroughfare();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLocationName(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string o = WrapSys.GetLocationName();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetFormattedAddressLine(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string o = WrapSys.GetFormattedAddressLine();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlatInterface_RefreshBattery(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		WrapSys.PlatInterface_RefreshBattery();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlatInterface_RefreshLocation(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		WrapSys.PlatInterface_RefreshLocation();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlatInterface_RefreshNetStatus(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		WrapSys.PlatInterface_RefreshNetStatus();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlatInterface_CopyStr(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		WrapSys.PlatInterface_CopyStr(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CIAPSys_InitUnlegalCountry(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		WrapSys.CIAPSys_InitUnlegalCountry();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CIAPSys_PurchaseSucceed(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		WrapSys.CIAPSys_PurchaseSucceed(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CIAPSys_SendBuyDiamond(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		WrapSys.CIAPSys_SendBuyDiamond(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Bugly_SetUserID(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
		WrapSys.Bugly_SetUserID(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int TouchBegin(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		bool o = WrapSys.TouchBegin();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int TouchEnd(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		bool o = WrapSys.TouchEnd();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int TouchIng(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		bool o = WrapSys.TouchIng();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ConversAction(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		LuaFunction arg0 = LuaScriptMgr.GetLuaFunction(L, 1);
		object arg1 = LuaScriptMgr.GetVarObject(L, 2);
		Action o = WrapSys.ConversAction(arg0,arg1);
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetTimeValue(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		TimeValue o = WrapSys.GetTimeValue();
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Reflection_CallMethod(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		object arg2 = LuaScriptMgr.GetVarObject(L, 3);
		DwObject[] objs3 = LuaScriptMgr.GetArrayObject<DwObject>(L, 4);
		object o = WrapSys.Reflection_CallMethod(arg0,arg1,arg2,objs3);
		LuaScriptMgr.PushVarObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Reflection_CallMethodByObjName(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		string arg2 = LuaScriptMgr.GetLuaString(L, 3);
		DwObject[] objs3 = LuaScriptMgr.GetArrayObject<DwObject>(L, 4);
		object o = WrapSys.Reflection_CallMethodByObjName(arg0,arg1,arg2,objs3);
		LuaScriptMgr.PushVarObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Reflection_GetProperty(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		object arg2 = LuaScriptMgr.GetVarObject(L, 3);
		object o = WrapSys.Reflection_GetProperty(arg0,arg1,arg2);
		LuaScriptMgr.PushVarObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Reflection_GetField(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		object arg2 = LuaScriptMgr.GetVarObject(L, 3);
		object o = WrapSys.Reflection_GetField(arg0,arg1,arg2);
		LuaScriptMgr.PushVarObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Reflection_SetField(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		object arg2 = LuaScriptMgr.GetVarObject(L, 3);
		object arg3 = LuaScriptMgr.GetVarObject(L, 4);
		WrapSys.Reflection_SetField(arg0,arg1,arg2,arg3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int TestReflect(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		WrapSys.TestReflect(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int StringSafeConvert(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaStringBuffer arg0 = LuaScriptMgr.GetStringBuffer(L, 1);
		string o = WrapSys.StringSafeConvert(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}
}

