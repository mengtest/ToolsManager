--[[
文件名     : 	timeout_tips_ui_window.lua
功能描述   :    任务超时提示窗口
负责人     :	sand0uzhuang   sand0uzhuang@ezfun.cn
参考文档   :    无
创建日期   :	2017/05/11 11:20:28
Copyright  :	Copyright 2017 EZFUN Inc
--]]
local m_luaWindowRoot
local m_state
local m_task
local m_content
local m_content_Trans
local m_itemObject
local m_ico_Trans
local m_ico_num_Trans
local m_ico_quality_Trans
local m_commitItemTrans
local m_resetTaskTrans
local m_greenStr
local m_redStr
----------------------

local function InitWindowDetail(state)
	m_state = state
	m_task = WrapSys.TaskSys_GetTask(m_state)
	m_greenStr = "[75d16e]"
	m_redStr = "[ff4a4a]"
	m_itemObject = WrapSys.PackSys_GetItem(m_task.overTimeItem)
	local colorStr = WrapSys.CPlayerTitleSys_GetQualityColor(m_itemObject.m_resItem.quality)
	m_content = WrapSys.TextData_GetText(1600205, colorStr..m_itemObject.m_resItem.name)
	
	if m_itemObject ~= nil then
		m_luaWindowRoot:InitClickToolShowItem(m_ico_Trans, m_itemObject.m_resItem, false, false)
	end
	m_luaWindowRoot:SetLabel(m_content_Trans, m_content)
	m_luaWindowRoot:SetLabel(m_commitItemTrans, WrapSys.TextData_GetText(1600206))
	m_luaWindowRoot:SetLabel(m_resetTaskTrans, WrapSys.TextData_GetText(1600207))
	m_luaWindowRoot:SetSprite(m_ico_Trans, m_itemObject.m_resItem.icon)
	if m_itemObject.m_own >= m_task.overTimeItNum then
		m_luaWindowRoot:SetLabel(m_ico_num_Trans, m_greenStr .. m_itemObject.m_own.."/"..m_task.overTimeItNum)
	else
		m_luaWindowRoot:SetLabel(m_ico_num_Trans, m_redStr .. m_itemObject.m_own.."/"..m_task.overTimeItNum)
	end
	WrapSys.BaseUI_SetQulity(m_ico_quality_Trans, m_itemObject.m_resItem.quality, false)
end

local function LocalInitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		InitWindowDetail(state)
	end
end

local function LocalCreateWindow()
	m_luaWindowRoot:TopBarCreateWindow()
	m_content_Trans = m_luaWindowRoot:GetTrans("content")
	m_ico_Trans = m_luaWindowRoot:GetTrans("ico")
	m_ico_num_Trans = m_luaWindowRoot:GetTrans(m_luaWindowRoot:GetTrans(m_ico_Trans, "label2"), "num")
	m_ico_quality_Trans = m_luaWindowRoot:GetTrans(m_ico_Trans, "bg2")
	m_commitItemTrans = m_luaWindowRoot:GetTrans(m_luaWindowRoot:GetTrans("cancelBtn"), "label")
	m_resetTaskTrans = m_luaWindowRoot:GetTrans(m_luaWindowRoot:GetTrans("confirm2Btn"), "label")
end

local function LocalHandleWidgetClick(gb)
	local click_name = gb.name
	if click_name == "btn_close" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		m_luaWindowRoot:InitWindow(false, m_state)
	elseif click_name == "cancelBtn" then
		WrapSys.TaskSys_SendQuickFinishReq(m_state)
		m_luaWindowRoot:InitWindow(false, m_state)
	elseif click_name == "confirm2Btn" then
		WrapSys.TaskSys_SendResetTaskReq(m_state)
		m_luaWindowRoot:InitWindow(false, m_state)
	else
	end
end

timeout_tips_ui_window = {
Init = function (luaWindowRoot)
	m_luaWindowRoot = luaWindowRoot
end
,
InitWindow = function (open, state)
	LocalInitWindow(open, state)
end
,
CreateWindow = function ()
	LocalCreateWindow()
end
,
HandleWidgetClick = function (gb)
	LocalHandleWidgetClick(gb)
end
}


