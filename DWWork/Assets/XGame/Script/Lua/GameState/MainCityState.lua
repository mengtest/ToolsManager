--------------------------------------------------------------------------------
--   File      : MainCityState.lua
--   author    : guoliang
--   function   : 大厅场景
--   date      : 2017-09-20
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

MainCityState = {}
local _s = MainCityState

local tickTime = 0
local m_recover_window_cb

function MainCityState.Init()

end

function MainCityState.Enter()
	-- logError("enter main city state,time = " .. TimerSys.Instance:GetCurrentDateMTimes())
	WrapSys.AudioSys_PlayMusic("Music/backMusic")
	WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "hall_ui_window", false , nil)
	HallSys.RefreshUserInfo()

	--刷新定位信息
	WrapSys.PlatInterface_RefreshLocation()

	-- 派发刷新系统通知事件 （喇叭和广告）
	LuaEvent.AddEventNow(EEventType.RefreshNoticeSysHorn)
	LuaEvent.AddEventNow(EEventType.RefreshNoticeSysAd)
	LuaEvent.AddEventNow(EEventType.RefreshNoticeActivity)

	--查询解散退出IM群
	NimChatSys.QueryAllMyTeamAndDismiss()

	GameStateQueueTask.EnterEnd()
	_s.DealEnterCallBack()
end

function MainCityState.Update()
	tickTime = tickTime + UnityEngine.Time.deltaTime
	if tickTime > 60 then
		tickTime = 0

		HallSys.RefreshUserInfo()
	end
end

function MainCityState.Leave()
	--关闭用户通讯服连接
	UserNetSys.CloseNetWork()
end

function MainCityState.GetType()
	return EGameStateType.MainCityState
end

function MainCityState.AddEnterCallBack(callback)
	if not _s.m_enter_callback then
		_s.m_enter_callback = {}
	end

	table.insert( _s.m_enter_callback, callback)
end

function MainCityState.DealEnterCallBack()
	for i,v in ipairs(_s.m_enter_callback or {}) do
		if v then
			v()
		end
	end
	_s.m_enter_callback = {}
end

