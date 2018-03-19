--------------------------------------------------------------------------------
-- 	 File      : club_hall_ui_window.lua
--   author    : zhisong
--   function  : 俱乐部大厅窗口
--   date      : 2018年1月29日 18:15:12
--   copyright : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------
require "Club.UIWindow.hall.club_title_render_ctrl"
require "Club.UIWindow.hall.club_member_render_ctrl"
require "Club.UIWindow.hall.club_room_render_ctrl"
require "Club.ClubSys.ClubUtil"

local m_luaWindowRoot
local m_state

-- 俱乐部列表显示控制组件
local m_title_ctrl
-- 俱乐部成员显示控制组件
local m_member_ctrl
-- 俱乐部房间列表显示控制组件
local m_room_ctrl

club_hall_ui_window = {

}
local this = club_hall_ui_window

function club_hall_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
	m_title_ctrl = club_title_render_ctrl.New(m_luaWindowRoot,this)
	m_member_ctrl = club_member_render_ctrl.New(m_luaWindowRoot)
	m_room_ctrl	= club_room_render_ctrl.New(m_luaWindowRoot)
end

function club_hall_ui_window.InitWindow(open, state, params)
	m_luaWindowRoot:InitCamera(open, false, true, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		-- DwDebug.LogError("xxx", "club_hall_ui_window open")
		this.RegisterEvent()
		m_state = state
		-- 必须保证“上报服务器进入俱乐部”在“上报服务器进入指定id俱乐部ui”前
		ClubSys.OnEnterHall()
		this.InitWindowDetail(state, params)
	else
		-- DwDebug.LogError("xxx", "club_hall_ui_window close")
		this.UnRegisterEvent()
		ClubSys.OnExitHall()
	end
end

function club_hall_ui_window.CreateWindow()
end

function club_hall_ui_window.UnRegister()
	-- DwDebug.LogError("xxx", "club_hall_ui_window unregister")
    m_luaWindowRoot = nil
end

function club_hall_ui_window.OnDestroy()
	-- DwDebug.LogError("xxx", "club_hall_ui_window destroy")
	if m_title_ctrl then
		m_title_ctrl:Destroy()
		m_title_ctrl = nil
	end
	if m_member_ctrl then
		m_member_ctrl:Destroy()
		m_member_ctrl = nil
	end
	if m_room_ctrl then
		m_room_ctrl:Destroy()
		m_room_ctrl = nil
	end
end

function club_hall_ui_window.RegisterEvent()
	LuaEvent.AddHandle(EEventType.RefreshClubHallWindow, this.RefreshWindow)
	LuaEvent.AddHandle(EEventType.UserNetReconnectSuccess, this.OnReconnect)
	-- LuaEvent.AddHandle(EEventType.NewMailNotify, this.RefreshClubMailRedPoint)
	LuaEvent.AddHandle(EEventType.RefreshClubMailRedPoint, this.RefreshClubMailRedPoint)
	m_title_ctrl:RegisterEvent()
	m_member_ctrl:RegisterEvent()
	m_room_ctrl:RegisterEvent()
end

function club_hall_ui_window.UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.RefreshClubHallWindow, this.RefreshWindow)
	LuaEvent.RemoveHandle(EEventType.UserNetReconnectSuccess, this.OnReconnect)
	-- LuaEvent.RemoveHandle(EEventType.NewMailNotify, this.RefreshClubMailRedPoint)
	LuaEvent.RemoveHandle(EEventType.RefreshClubMailRedPoint, this.RefreshClubMailRedPoint)
	m_title_ctrl:UnRegisterEvent()
	m_member_ctrl:UnRegisterEvent()
	m_room_ctrl:UnRegisterEvent()
end

function club_hall_ui_window.OnReconnect(event_id, p1, p2)
	ClubSys.SendEnterClubHall(true)
	this.InitWindowDetail(3)
end

function club_hall_ui_window.RefreshWindow(event_id, p1, p2)
	this.InitWindowDetail(4, p1)
end


-- 刷新邮件红点
function club_hall_ui_window.RefreshClubMailRedPoint()
	DwDebug.Log("刷新俱乐部邮件红点",tostring(DataManager.GetIsShowClubApplyMsgRedPoint()))
    m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("red_point"), DataManager.GetIsShowClubApplyMsgRedPoint())
end

function club_hall_ui_window.InitWindowDetail(call_path, group_id)
	DwDebug.LogWarning("club_hall_ui_window.InitWindowDetail ", call_path, group_id)
	-- 选择俱乐部页签会“上报服务器进入指定id俱乐部ui”
	m_title_ctrl:TryRenderUI(call_path, group_id)
	this.RefreshClubMailRedPoint()
end

function club_hall_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name
	if this.m_menu_opened then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("pop_menu"), false)
		this.m_menu_opened = false
	elseif click_name == "btn_menu" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("pop_menu"), true)
		this.m_menu_opened = true
	end
	
	if click_name == "btn_close" or click_name == "back_btn" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		this.InitWindow(false, 0)
	elseif click_name == "create_room_btn" then
		if nil ~= m_title_ctrl and nil ~= m_title_ctrl.m_cur_club and nil ~= m_title_ctrl.m_cur_club.m_id then
			local club_id = m_title_ctrl.m_cur_club.m_id
			local data = {}
			data.clubID = m_title_ctrl.m_cur_club.m_id
			data.succ_cb = function()
				MainCityState.AddEnterCallBack(
					function ()
						ClubSys.OpenClub(club_id)
					end
				)
			end
			WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 0, "Club.UIWindow.clubcreateroom_ui_window", false , data)
		end
	elseif click_name == "mine_club_btn" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 0, "Club.UIWindow.myclub_ui_window", false , nil)
	elseif click_name == "btn_Invite" then
		if nil ~= m_title_ctrl and nil ~= m_title_ctrl.m_cur_club and nil ~= m_title_ctrl.m_cur_club.m_id then
			ClubUtil.OpenShare(m_luaWindowRoot:GetTrans("animation_ui_root_tmp"), m_luaWindowRoot, m_title_ctrl.m_cur_club.m_id)
		end
	elseif click_name == "create_club_btn" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "Club.UIWindow.clubCreateJoin_ui_window", false , 1)
	elseif click_name == "join_club_btn" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "Club.UIWindow.clubCreateJoin_ui_window", false , 2)
	elseif click_name == "btn_requests" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "Club.UIWindow.clubApplyMsg_ui_window", false, nil)
		-- this.RefreshBtnMailRedPoint(false)
	end
	m_title_ctrl:HandleWidgetClick(gb)
	m_member_ctrl:HandleWidgetClick(gb)
	m_room_ctrl:HandleWidgetClick(gb)
end
