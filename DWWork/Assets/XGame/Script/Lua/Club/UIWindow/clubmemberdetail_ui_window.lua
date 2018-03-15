--------------------------------------------------------------------------------
--   File	  : clubmemberdetail_ui_window.lua
--   author	: zx
--   function  : 俱乐部玩法管理
--   date	  : 2018-01-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "LuaWindow.Module.UINetTipsCtrl"

clubmemberdetail_ui_window = {

}

local _s = clubmemberdetail_ui_window

local m_luaWindowRoot
local m_state
local m_open = false
local m_memberData = nil
local m_netTipsCtrl = nil

function clubmemberdetail_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function clubmemberdetail_ui_window.InitWindow(open, state, params)
	m_luaWindowRoot:InitCamera(open, false, false, false, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	m_open = open
	if open then
		m_state = state
		_s.InitWindowDetail(params)
	end
end

function clubmemberdetail_ui_window.InitMemberInfo(data)
	m_memberData = data
	-- 设置头像
	local url = (nil == data or nil == data.wx_headimgurl) and "" or data.wx_headimgurl
	local sex = (nil == data or nil == data.wx_sex) and DataManager.GetUserGender() or data.wx_sex
	WindowUtil.LoadHeadIcon(m_luaWindowRoot, m_luaWindowRoot:GetTrans("head"), url, sex, false, RessStorgeType.RST_Never)
	-- 设置名字
	local name = (nil == data or nil == data.wx_nickname) and "" or data.wx_nickname
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("name"), utf8sub(name, 1, 10))
	-- 设置性别
	if sex == 1 then
		m_luaWindowRoot:SetSprite(m_luaWindowRoot:GetTrans("sex"), "userino_icon_male")
	else
		m_luaWindowRoot:SetSprite(m_luaWindowRoot:GetTrans("sex"), "userino_icon_female")
	end

	-- 设置id
	local id = (nil == data or nil == data.uid) and "" or data.uid
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("id"), data.uid)

	-- 设置局数
	-- DwDebug.LogError("detail data:", data)
	local count = (nil == data or nil == data.game_number) and "" or data.game_number
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("count"), count)

	-- 设置时间
	local time = (nil == data or nil == data.join_time) and os.time() or data.join_time
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("time"), os.date("%Y-%m-%d   %H:%M", time))

	if data.owner_uid == DataManager.GetUserID() and DataManager.GetUserID() ~= id then
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("btnKickOut"), true)
	else
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("btnKickOut"), false)
	end
end

function clubmemberdetail_ui_window.InitWindowDetail(params)
	-- 重置界面
	_s.InitMemberInfo(params)

	-- DwDebug.LogError("m_state:", m_state, "params:", params)
	if not m_netTipsCtrl then
		m_netTipsCtrl = UINetTipsCtrl.New()
	end

	m_netTipsCtrl:Init(
		m_luaWindowRoot,
		m_luaWindowRoot:GetTrans("animation_ui_root"),
		function()
			WebNetHelper.RequestClubMemDetail(m_state, params.uid, function (body, head)
				if 0 == body.errcode then
					_s.InitMemberInfo(body.data)
					m_netTipsCtrl:StopWork(true)
				else
					WindowUtil.LuaShowTips(body.errmsg)
					m_netTipsCtrl:StopWork(false)
				end
			end,
			function (body, head)
				_s.InitWindow(false, 0)
				if nil ~= body and 0 ~= body.errcode and "" ~= body.errmsg then
					WindowUtil.LuaShowTips(body.errmsg)
				end
				m_netTipsCtrl:StopWork(false)
			end)
		end
	)

	m_netTipsCtrl:StartWork()
end


-- 踢出玩家
function clubmemberdetail_ui_window._handKickOut()
	if nil == m_memberData then
		return
	end

	WindowUtil.ShowErrorWindow(4, string.format("\n确认踢出 \"%s\" 玩家?", utf8sub(m_memberData.wx_nickname, 1, 10)), nil, function()
		LuaEvent.AddEventNow(EEventType.ShowWaitWindow, true)
		_s.InitWindow(false, 0)
		WebNetHelper.RequestKickoutClub(m_state, m_memberData.uid, function (body, head)
			LuaEvent.AddEventNow(EEventType.ShowWaitWindow, false)
			if 0 == body.errcode then
				-- 踢出玩家
				LuaEvent.AddEventNow(EEventType.UI_RefreshClubList)
				LuaEvent.AddEventNow(EEventType.UI_ChangeClubData)
				_s.InitWindow(false, 0)
			else
				WindowUtil.LuaShowTips(body.errmsg)
			end
		end,
		function (body, head)
			LuaEvent.AddEventNow(EEventType.ShowWaitWindow, false)
			if nil ~= body and 0 ~= body.errcode and "" ~= body.errmsg then
				WindowUtil.LuaShowTips(body.errmsg)
			else
				WindowUtil.LuaShowTips("踢出请求超时,请稍候重试")
			end
		end)

	end, nil, "")
end

-- 点击事件处理
function clubmemberdetail_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name

	if click_name == "btnKickOut" then
		_s._handKickOut()

	elseif click_name == "btnClose" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		_s.InitWindow(false, 0)
	else
		if m_netTipsCtrl then
			m_netTipsCtrl:HandleWidgetClick(gb)
		end
	end
end

-- 解散或退出，关闭界面
function clubmemberdetail_ui_window.ClubQuieOrDisBand(eventID, clubID)
	if nil == clubID or m_state == clubID then
		_s.InitWindow(false, 0)
	end
end

function clubmemberdetail_ui_window.RegisterEventHandle()
	LuaEvent.AddHandle(EEventType.ClubQuieOrDisBand, _s.ClubQuieOrDisBand)
	LuaEvent.AddHandle(EEventType.UserNetReconnectSuccess, _s.ClubQuieOrDisBand)
end

function clubmemberdetail_ui_window.UnRegisterEventHandle()
	LuaEvent.RemoveHandle(EEventType.ClubQuieOrDisBand, _s.ClubQuieOrDisBand)
	LuaEvent.RemoveHandle(EEventType.UserNetReconnectSuccess, _s.ClubQuieOrDisBand)
end

function clubmemberdetail_ui_window.CreateWindow()
	_s.RegisterEventHandle()
end

function clubmemberdetail_ui_window.OnDestroy()
	_s.UnRegisterEventHandle()

	if m_netTipsCtrl then
		m_netTipsCtrl:Destroy()
		m_netTipsCtrl = nil
	end

	m_luaWindowRoot = nil
end
