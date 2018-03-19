using System;
using LuaInterface;

public class ChatInterfaceWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("getInstance", getInstance),
		new LuaMethod("initLuaFunc", initLuaFunc),
		new LuaMethod("InitIMSDK", InitIMSDK),
		new LuaMethod("loginIm", loginIm),
		new LuaMethod("Relogin", Relogin),
		new LuaMethod("SDKCleanup", SDKCleanup),
		new LuaMethod("logoutIm", logoutIm),
		new LuaMethod("SetImCount", SetImCount),
		new LuaMethod("queryAllMyTeamAndDismiss", queryAllMyTeamAndDismiss),
		new LuaMethod("queryMyTeams", queryMyTeams),
		new LuaMethod("createChatTeam", createChatTeam),
		new LuaMethod("applyJoinTeam", applyJoinTeam),
		new LuaMethod("sendTextMessage", sendTextMessage),
		new LuaMethod("startRecordAudio", startRecordAudio),
		new LuaMethod("endRecordAudio", endRecordAudio),
		new LuaMethod("OnRecordAudioCancel", OnRecordAudioCancel),
		new LuaMethod("sendAudioMessage", sendAudioMessage),
		new LuaMethod("playVoiceAudio", playVoiceAudio),
		new LuaMethod("inviteTeamMember", inviteTeamMember),
		new LuaMethod("dismissTeam", dismissTeam),
		new LuaMethod("leaveTeam", leaveTeam),
		new LuaMethod("setTeamID", setTeamID),
		new LuaMethod("getCurrentTeamID", getCurrentTeamID),
		new LuaMethod("getIMLoginStatus", getIMLoginStatus),
		new LuaMethod("GetLoginState", GetLoginState),
		new LuaMethod("OnPlayAudioStop", OnPlayAudioStop),
		new LuaMethod("OnAudioSetSpeaker", OnAudioSetSpeaker),
		new LuaMethod("OnAudioGetSpeaker", OnAudioGetSpeaker),
		new LuaMethod("ClearAudio", ClearAudio),
		new LuaMethod("ClearVoice", ClearVoice),
		new LuaMethod("New", _CreateChatInterface),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateChatInterface(IntPtr L)
	{
		LuaDLL.luaL_error(L, "ChatInterface class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(ChatInterface));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "ChatInterface", typeof(ChatInterface), regs, fields, "System.Object");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int getInstance(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		ChatInterface o = ChatInterface.getInstance();
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int initLuaFunc(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		obj.initLuaFunc();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitIMSDK(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		string arg1 = LuaScriptMgr.GetLuaString(L, 3);
		obj.InitIMSDK(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int loginIm(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 5);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		string arg1 = LuaScriptMgr.GetLuaString(L, 3);
		string arg2 = LuaScriptMgr.GetLuaString(L, 4);
		string arg3 = LuaScriptMgr.GetLuaString(L, 5);
		obj.loginIm(arg0,arg1,arg2,arg3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Relogin(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		obj.Relogin();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SDKCleanup(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		obj.SDKCleanup();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int logoutIm(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		obj.logoutIm();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetImCount(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		string arg1 = LuaScriptMgr.GetLuaString(L, 3);
		obj.SetImCount(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int queryAllMyTeamAndDismiss(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		obj.queryAllMyTeamAndDismiss();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int queryMyTeams(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		obj.queryMyTeams();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int createChatTeam(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		obj.createChatTeam(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int applyJoinTeam(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		obj.applyJoinTeam(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int sendTextMessage(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		string arg1 = LuaScriptMgr.GetLuaString(L, 3);
		obj.sendTextMessage(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int startRecordAudio(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		obj.startRecordAudio();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int endRecordAudio(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		obj.endRecordAudio();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnRecordAudioCancel(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		obj.OnRecordAudioCancel();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int sendAudioMessage(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		string arg1 = LuaScriptMgr.GetLuaString(L, 3);
		obj.sendAudioMessage(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int playVoiceAudio(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		obj.playVoiceAudio(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int inviteTeamMember(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		string arg1 = LuaScriptMgr.GetLuaString(L, 3);
		obj.inviteTeamMember(arg0,arg1);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int dismissTeam(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		obj.dismissTeam(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int leaveTeam(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		obj.leaveTeam(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int setTeamID(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 2);
		obj.setTeamID(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int getCurrentTeamID(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		string o = obj.getCurrentTeamID();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int getIMLoginStatus(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		bool o = obj.getIMLoginStatus();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLoginState(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		bool o = obj.GetLoginState();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnPlayAudioStop(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		obj.OnPlayAudioStop();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnAudioSetSpeaker(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		obj.OnAudioSetSpeaker(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnAudioGetSpeaker(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		bool o = obj.OnAudioGetSpeaker();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ClearAudio(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		obj.ClearAudio();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ClearVoice(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		ChatInterface obj = LuaScriptMgr.GetNetObject<ChatInterface>(L, 1);
		obj.ClearVoice();
		return 0;
	}
}

