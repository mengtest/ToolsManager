--------------------------------------------------------------------------------
--   File	  : clubmanager_ui_window.lua
--   author	: zx
--   function  : 俱乐部玩法管理
--   date	  : 2018-01-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Club.ClubSys.ClubUtil"
require "LuaWindow.Module.UINetTipsCtrl"

clubmanager_ui_window = {

}

local _s = clubmanager_ui_window

local m_luaWindowRoot
local m_state
local m_clubInfo = nil
local m_open = false
local m_netTipsCtrl = nil

function clubmanager_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function clubmanager_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	m_open = open
	if open then
		m_state = state
		_s.InitWindowDetail()
	end
end

-- 界面设置
function clubmanager_ui_window.InitClubInfo(data)
	m_clubInfo = data
	-- 设置俱乐部Icon
	local iconIndex = (nil == data or nil == data.g_img) and 1 or data.g_img
	ClubUtil.SetClubIcon(m_luaWindowRoot, m_luaWindowRoot:GetTrans("icon"), iconIndex)

	-- 设置人数
	local count = (nil == data or nil == data.user_number) and 0 or data.user_number
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("member"), tostring(count))

	-- 区分是否首领
	local isClubChief = ClubUtil.IsClubChief(data and data.owner_uid)
	m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans("chiefRoot"), "isChief", isClubChief)

	-- 设置名称
	local club = (nil == data or nil == data.name) and ClubUtil.GetDefultInfo(1, isClubChief) or data.name
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("club"), club)

	-- 设置微信号
	local weixin = (nil == data or nil == data.owner_weixin) and ClubUtil.GetDefultInfo(2, isClubChief) or data.owner_weixin
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("weixin"), weixin)

	-- 设置电话号码
	local phone = (nil == data or nil == data.owner_mobile) and ClubUtil.GetDefultInfo(3, isClubChief) or data.owner_mobile
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("phone"), phone)
end

function clubmanager_ui_window.InitWindowDetail(data)
	-- 出事化界面
	_s.InitClubInfo(nil)
	-- 获取数据
	_s.GetClubData()
end

-- 点击事件处理
function clubmanager_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name

	if click_name == "btnIcon" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, m_state, "Club.UIWindow.clubChangeHeadImage_ui_window", false , m_clubInfo)

	elseif click_name == "btnMember" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, m_state, "Club.UIWindow.clubmember_ui_window", false , nil)

	elseif click_name == "btnInvite" then
		ClubUtil.OpenShare(m_luaWindowRoot:GetTrans("animation_ui_root"), m_luaWindowRoot, m_clubInfo.id, m_clubInfo)

	elseif click_name == "btnDisband" then
		if nil ~= m_clubInfo then
			-- 解散俱乐部
			WindowUtil.ShowErrorWindow(4, string.format("\n确认解散%s俱乐部", m_clubInfo.name), nil, function()
				LuaEvent.AddEventNow(EEventType.ShowWaitWindow, true)
				ClubSys.DeleteClub(m_state, function (body, head)

					LuaEvent.AddEventNow(EEventType.ShowWaitWindow, false)
					if 0 == body.errcode then
						WindowUtil.LuaShowTips("解散俱乐部成功！")
						-- _s.InitWindow(false, 0)
						-- LuaEvent.AddEventNow(EEventType.ClubQuieOrDisBand, m_state)
						-- LuaEvent.AddEventNow(EEventType.RefreshClubHallWindow, m_state)
					else
						WindowUtil.LuaShowTips(body.errmsg)
					end
				end,
				function (body, head)
					LuaEvent.AddEventNow(EEventType.ShowWaitWindow, false)
					if nil ~= body and 0 ~= body.errcode and "" ~= body.errmsg then
						WindowUtil.LuaShowTips(body.errmsg)
					else
						WindowUtil.LuaShowTips("退出俱乐部超时,请稍候重试")
					end
				end)

			end, nil, "")
		end

	elseif click_name == "btnQuit" then
		if nil ~= m_clubInfo then
			-- 退出俱乐部
			WindowUtil.ShowErrorWindow(4, string.format("\n确认退出%s俱乐部", m_clubInfo.name), nil, function()
				LuaEvent.AddEventNow(EEventType.ShowWaitWindow, true)
				ClubSys.QuitClub(m_state, function (body, head)
					LuaEvent.AddEventNow(EEventType.ShowWaitWindow, false)
					if 0 == body.errcode then
						WindowUtil.LuaShowTips("退出俱乐部成功!")
						-- _s.InitWindow(false, 0)
						LuaEvent.AddEventNow(EEventType.ClubQuieOrDisBand, m_state)
						-- LuaEvent.AddEventNow(EEventType.RefreshClubHallWindow, m_state)
					else
						WindowUtil.LuaShowTips(body.errmsg)
					end
				end,
				function (body, head)
					LuaEvent.AddEventNow(EEventType.ShowWaitWindow, false)
					if nil ~= body and 0 ~= body.errcode and "" ~= body.errmsg then
						WindowUtil.LuaShowTips(body.errmsg)
					else
						WindowUtil.LuaShowTips("退出俱乐部,请稍候重试")
					end
				end)

			end, nil, "")
		end

	elseif click_name == "btnGame" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, m_state, "Club.UIWindow.clubgame_ui_window", false , nil)

	elseif click_name == "btnClub" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "Club.UIWindow.clubmodify_ui_window", false , m_clubInfo)

	elseif click_name == "btnWeiXin" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 2, "Club.UIWindow.clubmodify_ui_window", false , m_clubInfo)

	elseif click_name == "btnPhone" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 3, "Club.UIWindow.clubmodify_ui_window", false , m_clubInfo)

	elseif click_name == "btnClose" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		_s.InitWindow(false, 0)
	else
		if m_netTipsCtrl then
			m_netTipsCtrl:HandleWidgetClick(gb)
		end
	end
end

-- 获取俱乐部详细数据
function clubmanager_ui_window.GetClubData( ... )
	-- 请求数据详情
	if not m_netTipsCtrl then
		m_netTipsCtrl = UINetTipsCtrl.New()
		m_netTipsCtrl:Init(
			m_luaWindowRoot,
			m_luaWindowRoot:GetTrans("animation_ui_root"),
			function()
				WebNetHelper.RequestClubDetail(m_state, function (body, head)
					if not m_open then
						return
					end
					if 0 == body.errcode then
						_s.InitClubInfo(body.data)
						m_netTipsCtrl:StopWork(true)
					else
						WindowUtil.LuaShowTips(body.errmsg)
						m_netTipsCtrl:StopWork(false)
					end
				end,
				function (body, head)
					if nil ~= body and 0 ~= body.errcode and "" ~= body.errmsg then
						WindowUtil.LuaShowTips(body.errmsg)
					end
					m_netTipsCtrl:StopWork(false)
				end)
			end
		)
	end

	m_netTipsCtrl:StartWork()
end

-- 解散或退出，关闭界面
function clubmanager_ui_window.ClubQuieOrDisBand(eventID, clubID)
	if m_state == clubID then
		_s.InitWindow(false, 0)
	end
end

function clubmanager_ui_window.RegisterEventHandle()
	LuaEvent.AddHandle(EEventType.ClubQuieOrDisBand, _s.ClubQuieOrDisBand)
	LuaEvent.AddHandle(EEventType.UI_ChangeClubData, _s.GetClubData)
	LuaEvent.AddHandle(EEventType.UserNetReconnectSuccess, _s.GetClubData)
end

function clubmanager_ui_window.UnRegisterEventHandle()
	LuaEvent.RemoveHandle(EEventType.ClubQuieOrDisBand, _s.ClubQuieOrDisBand)
	LuaEvent.RemoveHandle(EEventType.UI_ChangeClubData, _s.GetClubData)
	LuaEvent.RemoveHandle(EEventType.UserNetReconnectSuccess, _s.GetClubData)
end

function clubmanager_ui_window.CreateWindow()
	_s.RegisterEventHandle()
end

function clubmanager_ui_window.OnDestroy()
	_s.UnRegisterEventHandle()

	if m_netTipsCtrl then
		m_netTipsCtrl:Destroy()
		m_netTipsCtrl = nil
	end

	m_luaWindowRoot = nil
end
