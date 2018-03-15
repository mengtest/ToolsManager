--------------------------------------------------------------------------------
--   File	  : mj_round_over_ui_window.lua
--   author	: zx
--   function  : 麻将轮次结束界面
--   date	  : 2018-01-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

mj_round_over_ui_window = {

}

local _s = mj_round_over_ui_window

local m_luaWindowRoot
local m_state

local m_clubData = nil

function mj_round_over_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function mj_round_over_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		m_state = state
		_s.InitWindowDetail()
	end
end

function mj_round_over_ui_window.InitWindowDetail()
	_s.playLogicObj = PlayGameSys.GetPlayLogic()
	if not _s.playLogicObj then
		DwDebug.Log("not playLogic")
		return
	end
	_s.roomObj = _s.playLogicObj.roomObj
	if not _s.roomObj then
		DwDebug.Log("not roomObj")
		return
	end

	local is_show_total = _s.roomObj:GetBigResult() and true or false
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("btn_total_settlement"), is_show_total)
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("btn_continue"), not is_show_total)
	m_luaWindowRoot:PlayTweensInChildren(m_luaWindowRoot:GetTrans("animation_ui_root_temp"), true)
end

-- 点击事件处理
function mj_round_over_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name

	if click_name == "btn_continue" then
		PlayGameSys.GetPlayLogic():SendPrepare(0)
		-- 刷新轮次信息
		-- local uiData = _s.roomObj:GetCurrSmallResult()
		-- local players = uiData.playerMes
		-- local total = uiData.totalGameNum
		-- local next_pos = uiData.nextGamepos
		-- for i = 1, #players do
		-- 	local player = players[i]
		-- 	if next(player) ~= nil then
		-- 		local m_player = PlayGameSys.GetPlayerByPlayerID(player.userId)
		-- 		if m_player then
		-- 			LuaEvent.AddEvent(EEventType.RefreshPlayerTotalScore, m_player.seatPos, player.zongJifen)
		-- 		end
		-- 	end
		-- end
		_s.roomObj:ChangeState(RoomStateEnum.Idle)

	elseif click_name == "btn_total_settlement" then
		local roomObj = _s.roomObj
		if roomObj:GetBigResult() then
			roomObj:ChangeState(RoomStateEnum.GameOver)
		else
			local uiData = _s.roomObj:GetCurrSmallResult()
			if uiData.currentGamepos >= uiData.totalGameNum then
				WindowUtil.LuaShowTips("总结算信息准备中,请稍候...")
			end
		end
	elseif click_name == "btn_settlement" then
		_s.roomObj.playerMgr:AllPlayerIdle()
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, m_state, "mj_single_settlement_ui_window", false, nil)
	end

	_s.InitWindow(false, 0)
end

function mj_round_over_ui_window.RegisterEventHandle()
end

function mj_round_over_ui_window.UnRegisterEventHandle()
end

function mj_round_over_ui_window.CreateWindow()
	_s.RegisterEventHandle()
end

function mj_round_over_ui_window.OnDestroy()
	_s.UnRegisterEventHandle()

	m_luaWindowRoot = nil
end