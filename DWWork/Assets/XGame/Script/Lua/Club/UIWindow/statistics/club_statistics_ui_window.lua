--------------------------------------------------------------------------------
-- 	 File      : club_statistics_ui_window.lua
--   author    : zhisong
--   function  : 俱乐部数据统计窗口
--   date      : 2018年1月29日
--   copyright : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------
require "Club.UIWindow.statistics.club_rounds_statistics_ctrl"
require "Club.UIWindow.statistics.club_replay_statistics_ctrl"
require "Club.UIWindow.statistics.club_turnover_statistics_ctrl"

local m_luaWindowRoot
local m_state

-- 局数统计显示控制组件
local m_rounds_ctrl
-- 回放统计显示控制组件
local m_replay_ctrl
-- 流水统计显示控制组件
local m_turnover_ctrl
-- 当前俱乐部
local m_club_id
-- 当前页签名称
local m_cur_menu
-- 当前页签索引
local m_cur_index

club_statistics_ui_window = {

}
local this = club_statistics_ui_window

function club_statistics_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
	m_rounds_ctrl = club_rounds_statistics_ctrl.New(m_luaWindowRoot)
	m_replay_ctrl = club_replay_statistics_ctrl.New(m_luaWindowRoot)
	m_turnover_ctrl	= club_turnover_statistics_ctrl.New(m_luaWindowRoot)
end

function club_statistics_ui_window.InitWindow(open, state, params)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		m_state = state
		m_club_id = params
		this.InitWindowDetail(params)
	end
end

function club_statistics_ui_window.CreateWindow()
end

function club_statistics_ui_window.UnRegister()
    m_luaWindowRoot = nil
end

function club_statistics_ui_window.OnDestroy()
	if m_rounds_ctrl then
		m_rounds_ctrl:Destroy()
		m_rounds_ctrl = nil
	end
	if m_replay_ctrl then
		m_replay_ctrl:Destroy()
		m_replay_ctrl = nil
	end
	if m_turnover_ctrl then
		m_turnover_ctrl:Destroy()
		m_turnover_ctrl = nil
	end
end

function club_statistics_ui_window.InitWindowDetail()
	if not m_club_id then
		this.InitWindow(false, 0)
		return
	end
	this.ResetMenu()
	this.ChooseTab(m_state)
end

function club_statistics_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name
	if click_name == "btn_close" then
		this.InitWindow(false, 0)
	elseif click_name == "game_record" then
		if m_cur_index ~= 1 then
			this.ChooseTab(1)
		end
	elseif click_name == "game_replay" then
		if m_cur_index ~= 2 then
			this.ChooseTab(2)
		end
	elseif click_name == "charge_record" then
		if m_cur_index ~= 3 then
			this.ChooseTab(3)
		end
	end
	if m_cur_index == 1 then
		m_rounds_ctrl:HandleWidgetClick(gb)
	elseif m_cur_index == 2 then
		m_replay_ctrl:HandleWidgetClick(gb)
	elseif m_cur_index == 3 then
		m_turnover_ctrl:HandleWidgetClick(gb)
	end
end

function club_statistics_ui_window.HighLightMenu(menu_btn)
	if m_cur_menu then
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_cur_menu, "high_light"), false)
	end

	m_cur_menu = menu_btn
	if m_cur_menu then
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_cur_menu, "high_light"), true)
	end
end

function club_statistics_ui_window.ChooseTab(index)
	m_cur_index = index
	local list_root = m_luaWindowRoot:GetTrans("list_view")
	if index == 1 then
		this.HighLightMenu(m_luaWindowRoot:GetTrans("game_record"))
		m_luaWindowRoot:ShowChild(list_root, "round_statistics", true)
		m_rounds_ctrl:InitUI(m_club_id)
	elseif index == 2 then
		this.HighLightMenu(m_luaWindowRoot:GetTrans("game_replay"))
		m_luaWindowRoot:ShowChild(list_root, "game_statistics", true)
		m_replay_ctrl:InitUI(m_club_id)
	elseif index == 3 then
		this.HighLightMenu(m_luaWindowRoot:GetTrans("charge_record"))
		m_luaWindowRoot:ShowChild(list_root, "charge_statistics", true)
		m_turnover_ctrl:InitUI(m_club_id)
	else
		DwDebug.LogError("club_statistics_ui_window get wrong state")
	end
end

function club_statistics_ui_window.ResetMenu()
	this.HighLightMenu(m_luaWindowRoot:GetTrans("game_record"))
	this.HighLightMenu(m_luaWindowRoot:GetTrans("game_replay"))
	this.HighLightMenu(m_luaWindowRoot:GetTrans("charge_record"))
end
