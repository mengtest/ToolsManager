--------------------------------------------------------------------------------
-- 	 File       : tips_ui_window.lua
--   author     : zhagnhaochun
--   function   : 提示弹窗窗口
--   date       : 2018-01-14
--   copyright  : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------

tips_ui_window = {}

local _s = tips_ui_window

local m_luaWindowRoot
local m_state

local m_position
local m_position_1
local m_BG

function tips_ui_window.Init(luaWindowRoot)
	m_luaWindowRoot = luaWindowRoot
end

function tips_ui_window.CreateWindow()
	m_position = m_luaWindowRoot:GetTrans("position")
	m_position_1 = m_luaWindowRoot:GetTrans("position_1")
	m_BG = m_luaWindowRoot:GetTrans("BG")
end

function tips_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, 930, false)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		m_state = state
		_s.InitWindowDetail()
	else
	end
end

function tips_ui_window.InitWindowDetail()
	if m_state == 0 then
		m_luaWindowRoot:SetActive(m_position, true)
		m_luaWindowRoot:SetActive(m_position_1, false)
		m_luaWindowRoot:SetActive(m_BG, false)

		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_position, "valueLabel"), WindowUtil.ShowTipsStr)
		m_luaWindowRoot:PlayAndStopTweens(m_position, true)
	elseif m_state == 1 then
		m_luaWindowRoot:SetActive(m_position, false)
		m_luaWindowRoot:SetActive(m_position_1, true)
		m_luaWindowRoot:SetActive(m_BG, true)

		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_position_1, "valueLabel"), WindowUtil.ShowTipsStr)
		m_luaWindowRoot:PlayAndStopTweens(m_position_1, true)
	else
	end
end

function tips_ui_window.HandleWidgetClick(go)
	local click_name = go.name
	if click_name == "BG" then
		m_luaWindowRoot:SetActive(m_position_1, false)
		m_luaWindowRoot:SetActive(m_BG, false)
	else
	end
end