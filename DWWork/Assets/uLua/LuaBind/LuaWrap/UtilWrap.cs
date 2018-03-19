using System;
using UnityEngine;
using LuaInterface;
using Object = UnityEngine.Object;

public class UtilWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("Init", Init),
		new LuaMethod("Int", Int),
		new LuaMethod("Float", Float),
		new LuaMethod("Long", Long),
		new LuaMethod("Random", Random),
		new LuaMethod("Uid", Uid),
		new LuaMethod("GetTime", GetTime),
		new LuaMethod("Child", Child),
		new LuaMethod("Peer", Peer),
		new LuaMethod("Vibrate", Vibrate),
		new LuaMethod("Encode", Encode),
		new LuaMethod("Decode", Decode),
		new LuaMethod("IsNumeric", IsNumeric),
		new LuaMethod("HashToMD5Hex", HashToMD5Hex),
		new LuaMethod("md5", md5),
		new LuaMethod("md5file", md5file),
		new LuaMethod("CompressFile", CompressFile),
		new LuaMethod("DecompressFile", DecompressFile),
		new LuaMethod("Compress", Compress),
		new LuaMethod("Decompress", Decompress),
		new LuaMethod("ClearChild", ClearChild),
		new LuaMethod("ClearMemory", ClearMemory),
		new LuaMethod("IsNumber", IsNumber),
		new LuaMethod("GetFileText", GetFileText),
		new LuaMethod("AddClick", AddClick),
		new LuaMethod("AppContentPathA", AppContentPathA),
		new LuaMethod("LuaPath", LuaPath),
		new LuaMethod("DeleteFile", DeleteFile),
		new LuaMethod("DeleteFolder", DeleteFolder),
		new LuaMethod("CheckFileExists", CheckFileExists),
		new LuaMethod("CreateFolder", CreateFolder),
		new LuaMethod("DeleteFolderFile", DeleteFolderFile),
		new LuaMethod("New", _CreateUtil),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("m_platformId", get_m_platformId, set_m_platformId),
		new LuaField("m_dataPath", get_m_dataPath, set_m_dataPath),
		new LuaField("m_persistPath", get_m_persistPath, set_m_persistPath),
		new LuaField("m_streamPath", get_m_streamPath, set_m_streamPath),
		new LuaField("AppContentDataUri", get_AppContentDataUri, null),
		new LuaField("NetAvailable", get_NetAvailable, null),
		new LuaField("IsWifi", get_IsWifi, null),
		new LuaField("isApplePlatform", get_isApplePlatform, null),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateUtil(IntPtr L)
	{
		LuaDLL.luaL_error(L, "Util class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(Util));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "Util", typeof(Util), regs, fields, "UnityEngine.MonoBehaviour");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_platformId(IntPtr L)
	{
		LuaScriptMgr.PushEnum(L, Util.m_platformId);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_dataPath(IntPtr L)
	{
		LuaScriptMgr.Push(L, Util.m_dataPath);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_persistPath(IntPtr L)
	{
		LuaScriptMgr.Push(L, Util.m_persistPath);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_streamPath(IntPtr L)
	{
		LuaScriptMgr.Push(L, Util.m_streamPath);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_AppContentDataUri(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, Util.AppContentDataUri);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_NetAvailable(IntPtr L)
	{
		LuaScriptMgr.Push(L, Util.NetAvailable);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_IsWifi(IntPtr L)
	{
		LuaScriptMgr.Push(L, Util.IsWifi);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_isApplePlatform(IntPtr L)
	{
		LuaScriptMgr.Push(L, Util.isApplePlatform);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_platformId(IntPtr L)
	{
		Util.m_platformId = LuaScriptMgr.GetNetObject<RuntimePlatform>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_dataPath(IntPtr L)
	{
		Util.m_dataPath = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_persistPath(IntPtr L)
	{
		Util.m_persistPath = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_streamPath(IntPtr L)
	{
		Util.m_streamPath = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Init(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		Util.Init();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Int(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		object arg0 = LuaScriptMgr.GetVarObject(L, 1);
		int o = Util.Int(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Float(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		object arg0 = LuaScriptMgr.GetVarObject(L, 1);
		float o = Util.Float(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Long(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		object arg0 = LuaScriptMgr.GetVarObject(L, 1);
		long o = Util.Long(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Random(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(float), typeof(float)};
		Type[] types1 = {typeof(int), typeof(int)};

		if (count == 2 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			float arg0 = (float)LuaScriptMgr.GetNumber(L, 1);
			float arg1 = (float)LuaScriptMgr.GetNumber(L, 2);
			float o = Util.Random(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
			int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
			int o = Util.Random(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: Util.Random");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Uid(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string o = Util.Uid(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		long o = Util.GetTime();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Child(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(Transform), typeof(string)};
		Type[] types1 = {typeof(GameObject), typeof(string)};

		if (count == 2 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
			string arg1 = LuaScriptMgr.GetString(L, 2);
			GameObject o = Util.Child(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
			string arg1 = LuaScriptMgr.GetString(L, 2);
			GameObject o = Util.Child(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: Util.Child");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Peer(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		Type[] types0 = {typeof(Transform), typeof(string)};
		Type[] types1 = {typeof(GameObject), typeof(string)};

		if (count == 2 && LuaScriptMgr.CheckTypes(L, types0, 1))
		{
			Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
			string arg1 = LuaScriptMgr.GetString(L, 2);
			GameObject o = Util.Peer(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2 && LuaScriptMgr.CheckTypes(L, types1, 1))
		{
			GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
			string arg1 = LuaScriptMgr.GetString(L, 2);
			GameObject o = Util.Peer(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: Util.Peer");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Vibrate(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		Util.Vibrate();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Encode(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string o = Util.Encode(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Decode(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string o = Util.Decode(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IsNumeric(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		bool o = Util.IsNumeric(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int HashToMD5Hex(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string o = Util.HashToMD5Hex(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int md5(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string o = Util.md5(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int md5file(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string o = Util.md5file(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CompressFile(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string arg1 = LuaScriptMgr.GetLuaString(L, 2);
		Util.CompressFile(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DecompressFile(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string o = Util.DecompressFile(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Compress(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string o = Util.Compress(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Decompress(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string o = Util.Decompress(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ClearChild(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Transform arg0 = LuaScriptMgr.GetNetObject<Transform>(L, 1);
		Util.ClearChild(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ClearMemory(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		Util.ClearMemory();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IsNumber(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		bool o = Util.IsNumber(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetFileText(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string o = Util.GetFileText(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddClick(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		GameObject arg0 = LuaScriptMgr.GetNetObject<GameObject>(L, 1);
		object arg1 = LuaScriptMgr.GetVarObject(L, 2);
		Util.AddClick(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AppContentPathA(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		string o = Util.AppContentPathA();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LuaPath(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 5);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		bool arg1 = LuaScriptMgr.GetNetObject<bool>(L, 2);
		bool arg2 = LuaScriptMgr.GetBoolean(L, 3);
		string arg3 = LuaScriptMgr.GetLuaString(L, 4);
		string arg4 = LuaScriptMgr.GetLuaString(L, 5);
		string o = Util.LuaPath(arg0,out arg1,arg2,arg3,arg4);
		LuaScriptMgr.Push(L, o);
		LuaScriptMgr.Push(L, arg1);
		return 2;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DeleteFile(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		Util.DeleteFile(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DeleteFolder(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		Util.DeleteFolder(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CheckFileExists(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		bool o = Util.CheckFileExists(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CreateFolder(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		Util.CreateFolder(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DeleteFolderFile(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		Util.DeleteFolderFile(arg0);
		return 0;
	}
}

