using System;
using LuaInterface;

public class VersionWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("Init", Init),
		new LuaMethod("CheckIsPrePublish", CheckIsPrePublish),
		new LuaMethod("GetRessVersion", GetRessVersion),
		new LuaMethod("GetVersion", GetVersion),
		new LuaMethod("GetVersionCode", GetVersionCode),
		new LuaMethod("VersionIntToStr", VersionIntToStr),
		new LuaMethod("StoreActivityPopVersion", StoreActivityPopVersion),
		new LuaMethod("SetActivityPopVersion", SetActivityPopVersion),
		new LuaMethod("CheckActivityPopVersion", CheckActivityPopVersion),
		new LuaMethod("CheckActivityVersion", CheckActivityVersion),
		new LuaMethod("StoreActivityVersion", StoreActivityVersion),
		new LuaMethod("SetNetActivityVersion", SetNetActivityVersion),
		new LuaMethod("GetNetActivityVersion", GetNetActivityVersion),
		new LuaMethod("SetVersion", SetVersion),
		new LuaMethod("SetNoticeURL", SetNoticeURL),
		new LuaMethod("SetPlatformID", SetPlatformID),
		new LuaMethod("getMSDKPlatformName", getMSDKPlatformName),
		new LuaMethod("GetCurrentSdkPlatform", GetCurrentSdkPlatform),
		new LuaMethod("GetPublishSDKPlatform", GetPublishSDKPlatform),
		new LuaMethod("GetAppName", GetAppName),
		new LuaMethod("GetAppNameEnum", GetAppNameEnum),
		new LuaMethod("SetAnnounce", SetAnnounce),
		new LuaMethod("GetAnnounce", GetAnnounce),
		new LuaMethod("CompareVersion", CompareVersion),
		new LuaMethod("SetPackageVersion", SetPackageVersion),
		new LuaMethod("GetPackagetVersion", GetPackagetVersion),
		new LuaMethod("GetPackageVersionInt", GetPackageVersionInt),
		new LuaMethod("SetPackageInfo", SetPackageInfo),
		new LuaMethod("GetPackageURL", GetPackageURL),
		new LuaMethod("GetQuestionnaireBegineTime", GetQuestionnaireBegineTime),
		new LuaMethod("SetQuestionnaireBegineTime", SetQuestionnaireBegineTime),
		new LuaMethod("GetQuestionnaireClicked", GetQuestionnaireClicked),
		new LuaMethod("SetQuestionnaireClicked", SetQuestionnaireClicked),
		new LuaMethod("New", _CreateVersion),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("m_isPrePublish", get_m_isPrePublish, null),
		new LuaField("Instance", get_Instance, null),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateVersion(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			Version obj = new Version();
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: Version.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(Version));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "Version", typeof(Version), regs, fields, "System.Object");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_isPrePublish(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_isPrePublish");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_isPrePublish on a nil value");
			}
		}

		Version obj = (Version)o;
		LuaScriptMgr.Push(L, obj.m_isPrePublish);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Instance(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, Version.Instance);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Init(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		obj.Init();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CheckIsPrePublish(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		bool o = obj.CheckIsPrePublish();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetRessVersion(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		int o = obj.GetRessVersion();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetVersion(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types1 = {typeof(Version), typeof(int)};
		Type[] types2 = {typeof(Version), typeof(VersionType)};

		if (count == 1)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			int o = Version.GetVersion(arg0);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
			int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
			string o = obj.GetVersion(arg0);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types2, 1))
		{
			Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
			VersionType arg0 = LuaScriptMgr.GetNetObject<VersionType>(L, 2);
			string o = obj.GetVersion(arg0);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: Version.GetVersion");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetVersionCode(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		int o = Version.GetVersionCode(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int VersionIntToStr(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
		string o = Version.VersionIntToStr(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int StoreActivityPopVersion(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		obj.StoreActivityPopVersion();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetActivityPopVersion(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		obj.SetActivityPopVersion(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CheckActivityPopVersion(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		bool o = obj.CheckActivityPopVersion(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CheckActivityVersion(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		bool o = obj.CheckActivityVersion();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int StoreActivityVersion(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		obj.StoreActivityVersion();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetNetActivityVersion(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		obj.SetNetActivityVersion(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetNetActivityVersion(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		int o = obj.GetNetActivityVersion();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetVersion(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		VersionType arg0 = LuaScriptMgr.GetNetObject<VersionType>(L, 2);
		string arg1 = LuaScriptMgr.GetLuaString(L, 3);
		obj.SetVersion(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetNoticeURL(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		obj.SetNoticeURL();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetPlatformID(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		obj.SetPlatformID(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int getMSDKPlatformName(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		string o = obj.getMSDKPlatformName();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrentSdkPlatform(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		ENUM_SDK_PLATFORM o = obj.GetCurrentSdkPlatform();
		LuaScriptMgr.PushEnum(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetPublishSDKPlatform(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		ENUM_SDK_PLATFORM o = obj.GetPublishSDKPlatform();
		LuaScriptMgr.PushEnum(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetAppName(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		string o = obj.GetAppName();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetAppNameEnum(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		EnumAppName o = obj.GetAppNameEnum();
		LuaScriptMgr.PushEnum(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetAnnounce(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		obj.SetAnnounce(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetAnnounce(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		string o = obj.GetAnnounce();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CompareVersion(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		int o = Version.CompareVersion(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetPackageVersion(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		obj.SetPackageVersion(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetPackagetVersion(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		string o = obj.GetPackagetVersion();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetPackageVersionInt(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		int o = obj.GetPackageVersionInt();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetPackageInfo(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		string arg1 = LuaScriptMgr.GetLuaString(L, 3);
		obj.SetPackageInfo(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetPackageURL(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		string o = obj.GetPackageURL();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetQuestionnaireBegineTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		int o = obj.GetQuestionnaireBegineTime();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetQuestionnaireBegineTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		obj.SetQuestionnaireBegineTime();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetQuestionnaireClicked(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		bool o = obj.GetQuestionnaireClicked();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetQuestionnaireClicked(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Version obj = LuaScriptMgr.GetNetObject<Version>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		obj.SetQuestionnaireClicked(arg0);
		return 0;
	}
}

