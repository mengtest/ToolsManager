--------------------------------------------------------------------------------
--   File	  : clubcreateroom_ui_window.lua
--   author	: zx
--   function  : 俱乐部创建玩法
--   date	  : 2018-01-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Tools.CreateRoomCtrl"
require "LuaSys.CreateRoomConfig"

clubcreateroom_ui_window = {

}

local _s = clubcreateroom_ui_window

local m_luaWindowRoot
local m_state
local m_playData
local m_open = false
-- 创建房间控件
local m_createRoomCtrl

function clubcreateroom_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function clubcreateroom_ui_window.InitWindow(open, state, params)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	m_open = open
	if open then
		m_state = state or 0
		_s.InitWindowDetail(params)
	end
end

function clubcreateroom_ui_window.InitWindowDetail(params)
	local gameGroup = CreateRoomConfig.GameGroup[0]
	if nil == gameGroup then
		DwDebug.LogError("create room no found game group")
		return
	end

	m_playData = params
	if nil == m_playData or nil == m_playData.clubID then
		DwDebug.LogError("nil == m_playData or nil == m_playData.clubID:", m_playData)
		return
	end

	local gameTabList = {}
	for i,v in ipairs(gameGroup) do
		local tabConfig = CreateRoomConfig.GameTabConfig[v]
		table.insert(gameTabList, {playID = v, sortOrder = tabConfig.sortOrder })
	end

	if 1 < #gameTabList then
		table.sort(gameTabList, function (a, b)
			if a.playID == m_playData.play_id then
				return true
			elseif b.playID == m_playData.play_id then
				return false
			end

			return a.sortOrder < b.sortOrder
		end)
	end

	if nil == m_createRoomCtrl then
		m_createRoomCtrl = CreateRoomCtrl.New(m_luaWindowRoot, true)
	end
	m_createRoomCtrl:Init(m_playData.clubID, gameTabList, gameTabList[1].playID, UnityEngine.Vector2.New(236, 90), UnityEngine.Vector2.New(1, 6))
end

-- -- 创建房间
local function LocalCreateRoom()
	if m_createRoomCtrl then
		local playID, config, webConfig = m_createRoomCtrl:SaveCreateRoom()
		--
		if 0 == m_state then
			local playerNum = webConfig.playerNum or 4
			if playID == Common_PlayID.DW_DouDiZhu then
				-- 斗地主
				playerNum = 3
			end
			HallSys.CreateRoom(playID, webConfig.gameNum, playerNum, webConfig.zhiFu, webConfig.clubGroupId, config, m_playData.succ_cb)
		else
			local succCB = function (body, head)
					LuaEvent.AddEventNow(EEventType.ShowWaitWindow, false)
					if 0 == body.errcode then
						LuaEvent.AddEventNow(EEventType.UI_RefreshClubGameList)
						if not m_open then
							return
						end
					else
						WindowUtil.LuaShowTips(body.errmsg)
					end
					_s.InitWindow(false, 0)
					LuaEvent.AddEventNow(EEventType.UI_RefreshClubList)
				end

			LuaEvent.AddEventNow(EEventType.ShowWaitWindow, true)
			ProtoManager.InitProtoByPlayID(playID)
			local pbData = ProtoManager.Encode(GAME_CMD.CS_CREATE_ROOM, config)
			local base64Str = WrapSys.StringSafeConvert(pbData)

			if 1 == m_state then
				WebNetHelper.RequestCreateTemplate(m_playData.clubID, playID, base64Str,
					succCB,
					function (body, head)
						LuaEvent.AddEventNow(EEventType.ShowWaitWindow, false)
						if nil ~= body and 0 ~= body.errcode and "" ~= body.errmsg then
							WindowUtil.LuaShowTips(body.errmsg)
						else
							WindowUtil.LuaShowTips("创建玩法超时，请稍后重试！")
						end
						_s.InitWindow(false, 0)
					end)
			else
				WebNetHelper.RequestUpdateTemplate(m_playData.clubID, m_playData.id, playID, base64Str,
					succCB,
					function (body, head)
						LuaEvent.AddEventNow(EEventType.ShowWaitWindow, false)
						if nil ~= body and 0 ~= body.errcode and "" ~= body.errmsg then
							WindowUtil.LuaShowTips(body.errmsg)
						else
							WindowUtil.LuaShowTips("修改玩法超时，请稍后重试！")
						end
					end)
			end

		end
	end
end

-- 点击事件处理
function clubcreateroom_ui_window.HandleWidgetClick(gb)
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

-- 解散或退出，关闭界面
function clubcreateroom_ui_window.ClubQuieOrDisBand(eventID, clubID)
	if nil == m_playData or nil == clubID or m_playData.clubID == clubID then
		_s.InitWindow(false, 0)
	end
end

function clubcreateroom_ui_window.RegisterEventHandle()
	LuaEvent.AddHandle(EEventType.ClubQuieOrDisBand, _s.ClubQuieOrDisBand)
	-- LuaEvent.AddHandle(EEventType.UserNetReconnectSuccess, _s.ClubQuieOrDisBand)
end

function clubcreateroom_ui_window.UnRegisterEventHandle()
	LuaEvent.RemoveHandle(EEventType.ClubQuieOrDisBand, _s.ClubQuieOrDisBand)
	-- LuaEvent.RemoveHandle(EEventType.UserNetReconnectSuccess, _s.ClubQuieOrDisBand)
end

function clubcreateroom_ui_window.CreateWindow()
	_s.RegisterEventHandle()
end

function clubcreateroom_ui_window.OnDestroy()
	if m_createRoomCtrl then
		m_createRoomCtrl:Destroy()
		m_createRoomCtrl = nil
	end
	_s.UnRegisterEventHandle()

	m_luaWindowRoot = nil
end
