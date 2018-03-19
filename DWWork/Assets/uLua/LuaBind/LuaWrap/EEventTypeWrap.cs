using System;
using LuaInterface;

public class EEventTypeWrap
{
	static LuaEnum[] enums = new LuaEnum[]
	{
		new LuaEnum("EventTypeNull", EEventType.EventTypeNull),
		new LuaEnum("GamePause", EEventType.GamePause),
		new LuaEnum("ChangeGameState", EEventType.ChangeGameState),
		new LuaEnum("AccGuestLogin", EEventType.AccGuestLogin),
		new LuaEnum("LuaNetCallBack", EEventType.LuaNetCallBack),
		new LuaEnum("GmExecute", EEventType.GmExecute),
		new LuaEnum("ChangeGameStateForce", EEventType.ChangeGameStateForce),
		new LuaEnum("RefreshServerList", EEventType.RefreshServerList),
		new LuaEnum("Msg_Quit_Fuben", EEventType.Msg_Quit_Fuben),
		new LuaEnum("ActorRest", EEventType.ActorRest),
		new LuaEnum("OpenNewWin", EEventType.OpenNewWin),
		new LuaEnum("UI_Msg_ClearCamera", EEventType.UI_Msg_ClearCamera),
		new LuaEnum("UI_Msg_CloseWindow", EEventType.UI_Msg_CloseWindow),
		new LuaEnum("CloseWin", EEventType.CloseWin),
		new LuaEnum("UI_Msg_RefreshErrorContent", EEventType.UI_Msg_RefreshErrorContent),
		new LuaEnum("LoadingStateFinish", EEventType.LoadingStateFinish),
		new LuaEnum("LuaWindowEv", EEventType.LuaWindowEv),
		new LuaEnum("EnterStateFinish", EEventType.EnterStateFinish),
		new LuaEnum("ChangeScene", EEventType.ChangeScene),
		new LuaEnum("ShowWaitWindow", EEventType.ShowWaitWindow),
		new LuaEnum("OpenOrCloseWindow", EEventType.OpenOrCloseWindow),
		new LuaEnum("ScreenShotEnd", EEventType.ScreenShotEnd),
		new LuaEnum("WeiXinLoginEnd", EEventType.WeiXinLoginEnd),
		new LuaEnum("ShareFail", EEventType.ShareFail),
		new LuaEnum("GameSvrConnectSuc", EEventType.GameSvrConnectSuc),
		new LuaEnum("NetReconnectStart", EEventType.NetReconnectStart),
		new LuaEnum("NetReconnectSuccess", EEventType.NetReconnectSuccess),
		new LuaEnum("NetResetAutoConnect", EEventType.NetResetAutoConnect),
		new LuaEnum("UI_Msg_LoginOut", EEventType.UI_Msg_LoginOut),
		new LuaEnum("NetWorkTypeRefresh", EEventType.NetWorkTypeRefresh),
		new LuaEnum("NetWorkStrengthRefresh", EEventType.NetWorkStrengthRefresh),
		new LuaEnum("LocationRefresh", EEventType.LocationRefresh),
		new LuaEnum("BatteryRefreh", EEventType.BatteryRefreh),
		new LuaEnum("DeviceIDRefresh", EEventType.DeviceIDRefresh),
		new LuaEnum("NimIsNotInTeam", EEventType.NimIsNotInTeam),
		new LuaEnum("ApplicationAwakeEvent", EEventType.ApplicationAwakeEvent),
		new LuaEnum("ApplicationPauseEvent", EEventType.ApplicationPauseEvent),
		new LuaEnum("Max", EEventType.Max),
	};

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "EEventType", enums);
		LuaScriptMgr.RegisterFunc(L, "EEventType", IntToEnum, "IntToEnum");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IntToEnum(IntPtr L)
	{
		int arg0 = (int)LuaDLL.lua_tonumber(L, 1);
		EEventType o = (EEventType)arg0;
		LuaScriptMgr.PushEnum(L, o);
		return 1;
	}
}

