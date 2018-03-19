--------------------------------------------------------------------------------
--   File     : pk32_pk32_record_play_ui_window.lua
--   author : zx
--   function   : 32张回放主UI
--   date     : 2018-01-16
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

require "Logic.UICtrlLogic.UIRecordHandCardCtrl"
require "Logic.UICtrlLogic.UIOutCardShowCtrl"
require "Logic.UICtrlLogic.UIPlayerHeadCtrl"
require "Logic.UICtrlLogic.UICardAnimationCtrl"
require "Logic.UICtrlLogic.UICommonOtherInfoCtrl"
require "Logic.UICtrlLogic.UISingleSettlementCtrl"
require "Logic.UICtrlLogic.UIBetShowCtrl"
require "Logic.UICtrlLogic.UIPKLimitCardShowCtrl"
require "Logic.UICtrlLogic.UICardShuffleCtrl"

local m_luaWindowRoot
local m_state

--玩家操作根节点
local m_playRootTrans

--出牌展示根节点
local m_outCardsShowRootTrans

--玩家头像根节点
local m_playerHeadRootTrans
--玩家进入游戏显示根节点
local m_preparedRootTrans

-- 顶部右边房间轮数显示节点
local m_roundRootTrans

--播放速度显示文本
local m_labelPlaySpeedTrans
--暂停文本显示
local m_labelPauseTrans
-- 顶部左边设置根节点
local m_topLeftRoomSetRootTrans
-- 翻牌所挂的节点
local m_fanpaiRootTrans

-- @mid 小局结算节点
local m_singleSettlementCtrl --逻辑控制器


--玩家头部显示组件
local m_UIPlayerHeadCtrl
-- 其他信息组件
local m_UICommonOtherInfoCtrl
--押注积分显示界面
local m_UIBetShowCtrl
-- 手牌显示
local m_UIPKLimitCardShowCtrl
-- 下注显示
local m_UIBetShowCtrl
-- 洗牌
local m_ShuffleCtrl
--动画播放组件
local m_UICardAnimationCtrl

--游戏逻辑
local m_cardPlayLogic
--播放速度
local m_playSpeed = 1
--是否暂停
local m_isPause = false
--窗口是否打开
local m_open = false

pk32_record_play_ui_window = {

}
local _s = pk32_record_play_ui_window
_s.m_showBtnRoot = true
_s.m_timeCount = 0

function pk32_record_play_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function pk32_record_play_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	m_open = open
	if open then
		m_state = state
		_s.InitWindowDetail()
		UpdateSecond:Add(_s.UpdateSecond)
	else
		UpdateSecond:Remove(_s.UpdateSecond)
	end
end

function pk32_record_play_ui_window.CreateWindow()
	m_playRootTrans = m_luaWindowRoot:GetTrans("play_root")
	m_outCardsShowRootTrans = m_luaWindowRoot:GetTrans("out_card_show_root")
	m_playerHeadRootTrans = m_luaWindowRoot:GetTrans("player_icon_root")
	m_preparedRootTrans = m_luaWindowRoot:GetTrans("prepared_root")
	m_roundRootTrans = m_luaWindowRoot:GetTrans("round_root")
	m_labelPlaySpeedTrans = m_luaWindowRoot:GetTrans(m_playRootTrans,"label_speed")
	m_labelPauseTrans = m_luaWindowRoot:GetTrans(m_playRootTrans,"label_pause")
	m_topLeftRoomSetRootTrans = m_luaWindowRoot:GetTrans("room_set_root")
	m_fanpaiRootTrans = m_luaWindowRoot:GetTrans("fanpai_root")

	m_luaWindowRoot:SetActive(m_topLeftRoomSetRootTrans,true)

	_s.RegisterEventHandle()

	m_UIPlayerHeadCtrl = UIPlayerHeadCtrl.New()
	m_UIPlayerHeadCtrl:Init(m_playerHeadRootTrans,m_luaWindowRoot)

	local m_animationRootTrans = m_luaWindowRoot:GetTrans("animation_root")
	m_UICardAnimationCtrl = UICardAnimationCtrl.New()
	m_UICardAnimationCtrl:Init(m_animationRootTrans,m_luaWindowRoot)

	m_UICommonOtherInfoCtrl = UICommonOtherInfoCtrl.New()
	m_UICommonOtherInfoCtrl:Init(m_topLeftRoomSetRootTrans,m_luaWindowRoot)

	m_singleSettlementCtrl = UISingleSettlementCtrl.New()
	m_singleSettlementCtrl:Init(m_luaWindowRoot)

	m_UIPKLimitCardShowCtrl = UIPKLimitCardShowCtrl.New(m_outCardsShowRootTrans, m_luaWindowRoot, PlayGameSys.GetNowPlayId())
	m_UIBetShowCtrl = UIBetShowCtrl.New(m_playerHeadRootTrans, m_luaWindowRoot)
	m_ShuffleCtrl = UICardShuffleCtrl.New(m_luaWindowRoot:GetTrans("ShuffleRoot"), m_luaWindowRoot, PlayGameSys.GetNowPlayId())

	m_labelRestNumTrans = m_luaWindowRoot:GetTrans("txt_restNum")
	m_luaWindowRoot:SetActive(m_labelRestNumTrans, false)
end

function pk32_record_play_ui_window.InitWindowDetail()
	m_cardPlayLogic = PlayGameSys.GetPlayLogic()

	m_playSpeed = 1
	--显示玩家自己的操作节点
	m_luaWindowRoot:SetActive(m_playRootTrans, true)
	m_luaWindowRoot:SetActive(m_preparedRootTrans, true)

	_s.m_showBtnRoot = false
	_s.ShowPlayBtnRoot(true,false)

	m_luaWindowRoot:SetSprite(m_labelPlaySpeedTrans,_s.GetSpeedSpriteName(m_playSpeed))
	m_luaWindowRoot:MakePixelPerfect(m_labelPlaySpeedTrans)

	if m_isPause then
		m_luaWindowRoot:SetSprite(m_labelPauseTrans,"replayLayer_text_play")
	else
		m_luaWindowRoot:SetSprite(m_labelPauseTrans,"replayLayer_text_stop")
	end

	_s.RefreshBtns()

	LuaEvent.AddEventNow(EEventType.RefreshOtherInfo)
end

-- 显示播放、暂停
function pk32_record_play_ui_window.RefreshBtns(eventId, p1, p2)
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_playRootTrans, "btn_last_round"), PlayRecordSys.CheckLastRecordReplayInfo())
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_playRootTrans, "btn_next_round"), PlayRecordSys.CheckNextRecordReplayInfo())
end

-- 显示速度
function pk32_record_play_ui_window.GetSpeedSpriteName(speed)
	local spriteNum = speed
	return "replayLayer_text_"..spriteNum
end

-- 设置房间id
function pk32_record_play_ui_window.ChangeWindowShowState()
	local labelRoomIdTrans = m_luaWindowRoot:GetTrans(m_preparedRootTrans,"label_roomID")
	m_luaWindowRoot:SetLabel(labelRoomIdTrans, "房间号 "..m_cardPlayLogic.roomObj.roomInfo.roomId)
end

----------------------------Event  start----------------------------------------


function pk32_record_play_ui_window.RegisterEventHandle()
	LuaEvent.AddHandle(EEventType.RefreshPlayerHead,_s.RefreshPlayerHead)
	LuaEvent.AddHandle(EEventType.PlaySceneInit,_s.PlaySceneInit)
	LuaEvent.AddHandle(EEventType.ShowRemainCardNum, _s.ShowRestCardNum)
	LuaEvent.AddHandle(EEventType.RefreshRoomRoundNum,_s.RefreshRoomRoundNum)
	LuaEvent.AddHandle(EEventType.RefreshRecordBtns,_s.RefreshBtns)
	LuaEvent.AddHandle(EEventType.RefreshRecordPauseStatus,_s.RefreshRecordPauseStatus)
	LuaEvent.AddHandle(EEventType.RefreshShowPlayBtnRoot, _s.RefreshShowPlayBtnRoot)
	LuaEvent.AddHandle(EEventType.PK32FanPai,_s.FanPai)
	LuaEvent.AddHandle(EEventType.PK32HideFanPai,_s.HideFanPai)
end

function pk32_record_play_ui_window.UnRegisterEventHandle()
	LuaEvent.RemoveHandle(EEventType.RefreshPlayerHead,_s.RefreshPlayerHead)
	LuaEvent.RemoveHandle(EEventType.PlaySceneInit,_s.PlaySceneInit)
	LuaEvent.RemoveHandle(EEventType.ShowRemainCardNum, _s.ShowRestCardNum)
	LuaEvent.RemoveHandle(EEventType.RefreshRoomRoundNum,_s.RefreshRoomRoundNum)
	LuaEvent.RemoveHandle(EEventType.RefreshRecordBtns,_s.RefreshBtns)
	LuaEvent.RemoveHandle(EEventType.RefreshRecordPauseStatus,_s.RefreshRecordPauseStatus)
	LuaEvent.RemoveHandle(EEventType.RefreshShowPlayBtnRoot, _s.RefreshShowPlayBtnRoot)
	LuaEvent.RemoveHandle(EEventType.PK32FanPai,_s.FanPai)
	LuaEvent.RemoveHandle(EEventType.PK32HideFanPai,_s.HideFanPai)
end

function pk32_record_play_ui_window.RefreshPlayerHead(eventId, seatPos, seatInfo)
	m_UIPlayerHeadCtrl:RefreshPlayerHead(seatPos,seatInfo)
end

function pk32_record_play_ui_window.PlaySceneInit(eventId,p1,p2)
	--开启相关节点显示
	_s.ChangeWindowShowState()
end

--刷新房间轮数
function pk32_record_play_ui_window.RefreshRoomRoundNum(eventId, p1, p2)
	local curRound = p1
	local totalRound = p2
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_roundRootTrans, "label_cur_round"), curRound)
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_roundRootTrans, "label_total_round"), "/"..totalRound)
end

-- 刷新回放暂停、播放
function pk32_record_play_ui_window.RefreshRecordPauseStatus(eventId, p1, p2)
	m_isPause = p1
	if m_isPause then
		m_luaWindowRoot:SetSprite(m_labelPauseTrans, "replayLayer_text_play")
	else
		m_luaWindowRoot:SetSprite(m_labelPauseTrans, "replayLayer_text_stop")
	end
end

--显示剩余牌数
function pk32_record_play_ui_window.ShowRestCardNum(eventId, isShow, cardNum)
	m_luaWindowRoot:SetActive(m_labelRestNumTrans, isShow)
	if isShow then
		m_luaWindowRoot:SetLabel(m_labelRestNumTrans, cardNum)
	end
end

-- 刷新显示
function pk32_record_play_ui_window.RefreshShowPlayBtnRoot(eventId)
	_s.RefreshBtns()
	_s.ShowPlayBtnRoot(true, true)
end

function pk32_record_play_ui_window.HideFanPai(event_id, p1, p2)
	if m_fanpaiRootTrans then
		m_luaWindowRoot:SetActiveChildren(m_fanpaiRootTrans, false)
	end
end

function pk32_record_play_ui_window.FanPai(event_id,p1,p2)
	LuaEvent.AddEventNow(EEventType.ShowCardFriendCardAction,true)
	TimerTaskSys.AddTimerEventByLeftTime(function()
		if m_open then
			_s.DealyFanPai(event_id,p1,p2)
			LuaEvent.AddEventNow(EEventType.ShowCardFriendCardAction,false)
		end
	end,1)
end

function pk32_record_play_ui_window.DealyFanPai(event_id, p1, p2)
	if p1 == nil or type(p1) ~= "number" then
		return
	end
	local fanpai_card
	if m_fanpaiRootTrans.childCount > 0 then
		fanpai_card = m_fanpaiRootTrans:GetChild(0)
	else
		local resObj = WrapSys.ResourceMgr_GetInsAssetInParent(RessType.RT_UIItem, "card_item", m_fanpaiRootTrans, RessStorgeType.RST_Never)
		if not resObj then
			return
		end
		fanpai_card = resObj.transform
	end

	m_luaWindowRoot:SetActive(fanpai_card, true)
	local cardItem = CCard.New()
	cardItem:Init(p1, 0)
	LogicUtil.InitCardItem(m_luaWindowRoot, fanpai_card, cardItem, 1)

	TimerTaskSys.AddTimerEventByLeftTime(function()
		if m_open then
			if m_fanpaiRootTrans then
				m_luaWindowRoot:SetActiveChildren(m_fanpaiRootTrans, false)
			end
		end
	end,0.5)
end
-----------------------------Event End -------------------------------------------------


--------------------------button click -------------------------------------------------

-- 点击事件
function pk32_record_play_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name
	_s.m_timeCount = 0
	if click_name == "btn_last_round" then
		_s.HandleClickLastRound()
	elseif click_name == "btn_replay" then
		_s.HandleClickReply()
	elseif click_name == "btn_pause" then
		_s.HandleClickPause()
	elseif click_name == "btn_speed" then
		_s.HandleClickSpeed()
	elseif click_name == "btn_next_round" then
		_s.HandleClickNextRound()
	elseif click_name == "btn_quit" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		_s.HandleClickQuit()
	elseif click_name == "bgBtn" then
		_s.HanldeClickBgBtn()
	end
end

function pk32_record_play_ui_window.HandleClickLastRound()
	LuaEvent.AddEventNow(EEventType.RecordPlayLastRound)
end

function pk32_record_play_ui_window.HandleClickReply()
	LuaEvent.AddEventNow(EEventType.RecordReplayCurRound)
end

function pk32_record_play_ui_window.HandleClickPause()
	_s.RefreshRecordPauseStatus(nil, not m_isPause)
	LuaEvent.AddEventNow(EEventType.RecordPausePlay,m_isPause)
end

function pk32_record_play_ui_window.HandleClickSpeed()
	m_playSpeed = m_playSpeed + 1
	if m_playSpeed > PlayRecordSys.playInterval then
		m_playSpeed = 1
	end

	m_luaWindowRoot:SetSprite(m_labelPlaySpeedTrans,_s.GetSpeedSpriteName(m_playSpeed))
	m_luaWindowRoot:MakePixelPerfect(m_labelPlaySpeedTrans)

	LuaEvent.AddEventNow(EEventType.RecordSetPlaySpeed,m_playSpeed)
end

function pk32_record_play_ui_window.HandleClickNextRound()
	LuaEvent.AddEventNow(EEventType.RecordPlayNextRound)
end

function pk32_record_play_ui_window.HandleClickQuit()
	local contentStr = "确定退出回放"
	local okFunc = function ()
			GameManager.SetGameTimeScale(1)
			PlayGameSys.QuitToMainCity()
		end
	WindowUtil.ShowErrorWindow(2,contentStr,nil,okFunc)
end

function pk32_record_play_ui_window.HanldeClickBgBtn()
	_s.ShowPlayBtnRoot(not _s.m_showBtnRoot,true)
end

-- 回放按钮隐藏与显示
function pk32_record_play_ui_window.ShowPlayBtnRoot(isFoword,needTweenAnim)
	if isFoword ~= _s.m_showBtnRoot then
		m_luaWindowRoot:PlayForceAnimation_Lua(m_playRootTrans,function ()
			_s.m_timeCount = 0
		end,isFoword,needTweenAnim)
		_s.m_showBtnRoot = isFoword
	end
end


------------------------------button click end -------------------------------------------------

function pk32_record_play_ui_window.UpdateSecond()
	if _s.m_showBtnRoot then
		_s.m_timeCount = _s.m_timeCount + 1
		-- DwDebug.Log("当前的时间缩放参数 "..tostring(UnityEngine.Time.timeScale))
		if _s.m_timeCount >= 4*(UnityEngine.Time.timeScale) then
			_s.m_timeCount = 0
			_s.ShowPlayBtnRoot(false,true)
		end
	end
end

function pk32_record_play_ui_window.UnRegister()
	m_luaWindowRoot = nil
end

function pk32_record_play_ui_window.OnDestroy()
	_s.UnRegisterEventHandle()

	if m_UIPlayerHeadCtrl then
		m_UIPlayerHeadCtrl:Destroy()
		m_UIPlayerHeadCtrl = nil
	end

	if m_UIPKLimitCardShowCtrl then
		m_UIPKLimitCardShowCtrl:Destroy()
		m_UIPKLimitCardShowCtrl = nil
	end

	if m_UICardAnimationCtrl then
		m_UICardAnimationCtrl:Destroy()
		m_UICardAnimationCtrl = nil
	end

	if m_UIBetShowCtrl then
		m_UIBetShowCtrl:Destroy()
		m_UIBetShowCtrl = nil
	end

	if m_ShuffleCtrl then
		m_ShuffleCtrl:Destroy()
		m_ShuffleCtrl = nil
	end

	if m_singleSettlementCtrl then
		m_singleSettlementCtrl:Destroy()
		m_singleSettlementCtrl = nil
	end

	if m_UICommonOtherInfoCtrl then
		m_UICommonOtherInfoCtrl:Destroy()
		m_UICommonOtherInfoCtrl = nil
	end
end
