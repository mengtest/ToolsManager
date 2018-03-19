--------------------------------------------------------------------------------
--   File	  : createRoom_ui_window.lua
--   author	: jianing
--   function  : 创建房间
--   date	  : 2017-10-21
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Tools.CreateRoomCtrl"
require "LuaSys.CreateRoomConfig"

createRoom_ui_window = {

}

local _s = createRoom_ui_window

local m_luaWindowRoot
local m_state
-- 创建房间控件
local m_createRoomCtrl

function createRoom_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function createRoom_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		m_state = state
		_s.InitWindowDetail()
	end
end

function createRoom_ui_window.InitWindowDetail()
	local gameGroup = CreateRoomConfig.GameGroup[m_state]
	if nil == gameGroup then
		DwDebug.LogError("create room no found game group")
		return
	end

	local gameTabList = {}
	for i,v in ipairs(gameGroup) do
		local tabConfig = CreateRoomConfig.GameTabConfig[v]
		table.insert(gameTabList, {playID = v, sortOrder = tabConfig.sortOrder })
	end

	if 1 < #gameTabList then
		table.sort(gameTabList, function (a, b)
			return a.sortOrder < b.sortOrder
		end)
	end

	if nil == m_createRoomCtrl then
		m_createRoomCtrl = CreateRoomCtrl.New(m_luaWindowRoot)
	end
	m_createRoomCtrl:Init(0, gameTabList, gameTabList[1].playID, UnityEngine.Vector2.New(275, 100), UnityEngine.Vector2.New(1, 5))
end

-- 刷新按钮状态
function createRoom_ui_window.RefreshCreateBtnStatus(eventID, p1, p2)
-- 	if m_luaWindowRoot then
--		m_luaWindowRoot:SetGray(m_luaWindowRoot:GetTrans("btnCreate"), false, true)
-- 	end
end
local function LocalCreateRoom()
	if m_createRoomCtrl then
		local playID, config, webConfig = m_createRoomCtrl:SaveCreateRoom()
		local playerNum = webConfig.playerNum or 4
		if playID == Common_PlayID.DW_DouDiZhu then
			-- 斗地主
			playerNum = 3
		end
		HallSys.CreateRoom(playID, webConfig.gameNum, playerNum, webConfig.zhiFu, webConfig.clubGroupId, config)
	end
end

-- 点击事件处理
function createRoom_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name

	if click_name == "btnCreate" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		LocalCreateRoom()
	elseif click_name == "btnClose" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		_s.InitWindow(false, 0)
	end

	if m_createRoomCtrl then
		m_createRoomCtrl:HandleWidgetClick(gb, click_name)
	end
end

function createRoom_ui_window.RegisterEventHandle()
	LuaEvent.AddHandle(EEventType.UI_CreateRoomResult, _s.RefreshCreateBtnStatus)
end

function createRoom_ui_window.UnRegisterEventHandle()
	LuaEvent.RemoveHandle(EEventType.UI_CreateRoomResult, _s.RefreshCreateBtnStatus)
end

function createRoom_ui_window.CreateWindow()
	_s.RegisterEventHandle()
end

function createRoom_ui_window.OnDestroy()
	if m_createRoomCtrl then
		m_createRoomCtrl:Destroy()
		m_createRoomCtrl = nil
	end
	_s.UnRegisterEventHandle()

	m_luaWindowRoot = nil
end