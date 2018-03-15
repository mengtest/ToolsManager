--------------------------------------------------------------------------------
--   File	  : clubmodify_ui_window.lua
--   author	: zx
--   function  : 俱乐部玩法管理
--   date	  : 2018-01-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Club.ClubSys.ClubUtil"

clubmodify_ui_window = {

}

local _s = clubmodify_ui_window

local m_luaWindowRoot
local m_state
local m_UIInputComponent
local m_open = false
local m_clubInfo = nil

function clubmodify_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function clubmodify_ui_window.InitWindow(open, state, params)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	m_open = open
	if open then
		m_state = state
		_s.InitWindowDetail(params)
	end
end

-- 获取对应字段
function clubmodify_ui_window.GetWebKey()
	-- 名字
	if 1 == m_state then
		return "name"
	-- 微信号
	elseif 2 == m_state then
		return "owner_weixin"
	-- 手机号
	elseif 3 == m_state then
		return "owner_mobile"
	end

	return "error"
end

-- 获取对应字段数据
function clubmodify_ui_window.GetWebValue(state, inmsg)
	-- local webKey = _s.GetWebKey()
	if state == m_state then
		return inmsg
	end

	return nil
end

-- 获取对应字段的长度限制
function clubmodify_ui_window.GetInMsgLength()
	-- 名字
	if 1 == m_state then
		return 5
	-- 微信号
	elseif 2 == m_state then
		return 20
	-- 手机号
	elseif 3 == m_state then
		return 20
	end

	return 20
end

function clubmodify_ui_window.InitWindowDetail(params)
	m_clubInfo = params
	-- 设置基础信息
	m_UIInputComponent = m_luaWindowRoot:GetTrans("msgInput"):GetComponent("UIInput")

	local key = _s.GetWebKey()
	local value
	if nil ~= m_clubInfo and nil ~= m_clubInfo[key] then
		value = m_clubInfo[key]
	else
		value = ClubUtil.GetDefultInfo(m_state, true)
	end
	m_UIInputComponent.value = value
end

-- 关闭界面
function clubmodify_ui_window.CloseWindow()
	WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
	_s.InitWindow(false, 0)
end

-- 保存数据
function clubmodify_ui_window._handModifyData()
	local inmsg = m_UIInputComponent.value

	local count = utf8len(inmsg)
	local length = _s.GetInMsgLength()
	if count > length then
		inmsg = string.sub(inmsg, 0, length)
	elseif count <= 0 then
		-- WindowUtil.LuaShowTips("请先输入内容！")
		-- return
	else
		local wUtils = require "Global.SensitiveWordUtils"
		if not wUtils:checkString(inmsg) then
			WindowUtil.LuaShowTips("输入内容非法，请先检查后重新输入！")
			return
		end
	end

	-- local key = _s.GetWebKey()
	-- m_clubInfo[key] = inmsg

	_s.InitWindow(false, 0)

	-- 发送修改数据
	LuaEvent.AddEventNow(EEventType.ShowWaitWindow, true)
	local name, weixin, mobile = _s.GetWebValue(1, inmsg), _s.GetWebValue(2, inmsg), _s.GetWebValue(3, inmsg)
	ClubSys.UpdateClub(m_clubInfo.id, name, nil, weixin, mobile,
		function (body, head)
			LuaEvent.AddEventNow(EEventType.ShowWaitWindow, false)
			if 0 == body.errcode then
				-- 发送事件，界面重新拉数据
				LuaEvent.AddEventNow(EEventType.UI_ChangeClubData, false)
				LuaEvent.AddEventNow(EEventType.UI_RefreshClubList)
			else
				WindowUtil.LuaShowTips(body.errmsg)
			end
		end,
		function (body, head)
			LuaEvent.AddEventNow(EEventType.ShowWaitWindow, false)
			if nil ~= body and 0 ~= body.errcode and "" ~= body.errmsg then
				WindowUtil.LuaShowTips(body.errmsg)
			else
				WindowUtil.LuaShowTips("请求修改数据超时, 请稍候重试")
			end
		end)
end

-- 点击事件处理
function clubmodify_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name

	if click_name == "btnSure" then
		_s._handModifyData()

	elseif click_name == "btnClose" then
		_s.CloseWindow()
	end
end

-- 解散或退出，关闭界面
function clubmodify_ui_window.ClubQuieOrDisBand(eventID, clubID)
	if nil == m_clubInfo or m_clubInfo.id == clubID then
		_s.InitWindow(false, 0)
	end
end

function clubmodify_ui_window.RegisterEventHandle()
	LuaEvent.AddHandle(EEventType.ClubQuieOrDisBand, _s.ClubQuieOrDisBand)
	LuaEvent.AddHandle(EEventType.UserNetReconnectSuccess, _s.ClubQuieOrDisBand)
end

function clubmodify_ui_window.UnRegisterEventHandle()
	LuaEvent.RemoveHandle(EEventType.ClubQuieOrDisBand, _s.ClubQuieOrDisBand)
	LuaEvent.RemoveHandle(EEventType.UserNetReconnectSuccess, _s.ClubQuieOrDisBand)
end

function clubmodify_ui_window.CreateWindow()
	_s.RegisterEventHandle()
end

function clubmodify_ui_window.OnDestroy()
	_s.UnRegisterEventHandle()

	m_panelScrollView = nil
	m_luaWindowRoot = nil
end
