--------------------------------------------------------------------------------
--   File       : ddz_record_play_ui_window.lua
--   author     : zhanghaochun
--   function   : 回放主UI
--   date       : 2018-1-30
--   copyright  : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------

require "Logic.UICtrlLogic.UIRecordHandCardCtrl"
require "Logic.UICtrlLogic.UIOutCardShowCtrl"
require "Logic.UICtrlLogic.UIPlayerHeadCtrl"
require "Logic.UICtrlLogic.UICardAnimationCtrl"
require "Logic.UICtrlLogic.UICommonOtherInfoCtrl"

local AddDoubleCtrl = require "Area.DouDiZhu.UIWindow.UICtrl.UIDDZAddDoubleCtrl"
local BaseCardsCtrl = require "Area.DouDiZhu.UIWindow.UICtrl.UIDDZBaseCardsCtrl"
local LordPlayCtrl = require "Area.DouDiZhu.UIWindow.UICtrl.UIDDZLordOpenPlayCtrl"
local WantLordCtrl = require "Area.DouDiZhu.UIWindow.UICtrl.UIDDZWantLordCtrl"
local LightCtrl = require "Area.DouDiZhu.UIWindow.UICtrl.UIDDZLightCtrl"
local DoubleShowCtrl = require "Area.DouDiZhu.UIWindow.UICtrl.UIDDZDoubleShowCtrl"

local m_luaWindowRoot
local m_state

-- 斗地主亮灯节点
local m_redLightRootTrans
-- 斗地主双倍显示节点
local m_doubleShowRootTrans
--玩家加倍按钮
local m_doubleRootTrans
--底牌节点
local m_BaseCardRootTrans
-- 是否明牌根节点
local m_LordOpenRootTrans
-- 叫地主根节点
local m_WantLordRootTrans
--玩家操作根节点
local m_playRootTrans
--玩家自己手牌根节点
local m_handCardRootTrans
--出牌展示根节点
local m_outCardsShowRootTrans
--其他玩家操作提示文本节点
local m_otherOperateTipTrans
--当前桌面分显示文本
local m_curTabelPointTrans
-- 朋友亮牌节点
local m_friendCardRootTrans

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
-- 动画节点
local m_animationRootTrans
-- 顶部左边设置根节点
local m_topLeftRoomSetRootTrans
-- 倍数信息显示节点
local m_multipleNumShowTrans
-- 详细信息节点
local m_detailRootTrans


-- 其他信息组件
local m_UICommonOtherInfoCtrl
--手牌显示控制组件
local m_UIRecordHandCardCtrl
--出牌显示控制
local m_UIOutCardShowCtrl
--玩家头部显示组件
local m_UIPlayerHeadCtrl

--游戏逻辑
local m_cardPlayLogic
--播放速度
local m_playSpeed = 1
--是否暂停
local m_isPause = false

-- 斗地主加倍控件
local m_UIAddDoubleCtrl
-- 斗地主底牌控件
local m_UIBaseCardsCtrl
-- 斗地主地主是否明牌控件
local m_UILordOpenPlayCtrl
-- 斗地主叫地主控件
local m_UIWantLordCtrl
-- 斗地主亮灯控件
local m_UIRedLightCtrl

ddz_record_play_ui_window = {

}
local _s = ddz_record_play_ui_window
_s.m_showBtnRoot = true
_s.m_timeCount = 0

function ddz_record_play_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function ddz_record_play_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		m_state = state
		_s.InitWindowDetail()
		UpdateSecond:Add(_s.UpdateSecond)
	else
		UpdateSecond:Remove(_s.UpdateSecond)
	end
end

function ddz_record_play_ui_window.CreateWindow()
	m_doubleShowRootTrans = m_luaWindowRoot:GetTrans("DoubleShowRoot")
	m_redLightRootTrans = m_luaWindowRoot:GetTrans("ligth_root")
	m_doubleRootTrans = m_luaWindowRoot:GetTrans("double_root")
	m_BaseCardRootTrans = m_luaWindowRoot:GetTrans("base_card_root")
	m_LordOpenRootTrans = m_luaWindowRoot:GetTrans("isopen_root")
	m_WantLordRootTrans = m_luaWindowRoot:GetTrans("wantlord_root")
	m_playRootTrans = m_luaWindowRoot:GetTrans("play_root")
	m_handCardRootTrans = m_luaWindowRoot:GetTrans("hand_card_root")
	m_outCardsShowRootTrans = m_luaWindowRoot:GetTrans("out_card_show_root")
	m_otherOperateTipTrans = m_luaWindowRoot:GetTrans("label_other_operate_tips")
	m_curTabelPointTrans = m_luaWindowRoot:GetTrans("label_cur_table_point")
	m_playerHeadRootTrans = m_luaWindowRoot:GetTrans("player_icon_root")
	m_preparedRootTrans = m_luaWindowRoot:GetTrans("prepared_root")
	m_friendCardRootTrans =  m_luaWindowRoot:GetTrans(m_preparedRootTrans,"friend_card_root")
	m_roundRootTrans = m_luaWindowRoot:GetTrans("round_root")
	m_labelPlaySpeedTrans = m_luaWindowRoot:GetTrans(m_playRootTrans,"label_speed")
	m_labelPauseTrans = m_luaWindowRoot:GetTrans(m_playRootTrans,"label_pause")
	m_animationRootTrans = m_luaWindowRoot:GetTrans("animation_root")
	m_topLeftRoomSetRootTrans = m_luaWindowRoot:GetTrans("room_set_root")

	m_multipleNumShowTrans = m_luaWindowRoot:GetTrans("multiple_num_show")
	m_detailRootTrans = m_luaWindowRoot:GetTrans("DetailRoot")

	m_luaWindowRoot:SetActive(m_topLeftRoomSetRootTrans,true)
	m_luaWindowRoot:SetActive(m_curTabelPointTrans,false)
	m_luaWindowRoot:SetActive(m_otherOperateTipTrans,false)
	m_luaWindowRoot:SetActive(m_friendCardRootTrans,false)

	_s.RegisterEventHandle()


	m_UIPlayerHeadCtrl = UIPlayerHeadCtrl.New()
	m_UIPlayerHeadCtrl:Init(m_playerHeadRootTrans,m_luaWindowRoot)

	m_UIOutCardShowCtrl = UIOutCardShowCtrl.New()
	m_UIOutCardShowCtrl:Init(m_outCardsShowRootTrans,m_luaWindowRoot)

	m_UIHandCardCtrl = UIRecordHandCardCtrl.New()
	m_UIHandCardCtrl:Init(m_handCardRootTrans,m_luaWindowRoot)

	m_UICardAnimationCtrl = UICardAnimationCtrl.New()
	m_UICardAnimationCtrl:Init(m_animationRootTrans,m_luaWindowRoot)

	m_UICommonOtherInfoCtrl = UICommonOtherInfoCtrl.New()
	m_UICommonOtherInfoCtrl:Init(m_topLeftRoomSetRootTrans,m_luaWindowRoot)

	m_UIAddDoubleCtrl = AddDoubleCtrl.New(m_doubleRootTrans, m_luaWindowRoot)
	m_UIBaseCardsCtrl = BaseCardsCtrl.New(m_BaseCardRootTrans, m_luaWindowRoot, PlayGameSys.GetNowPlayId())
	m_UILordOpenPlayCtrl = LordPlayCtrl.New(m_LordOpenRootTrans, m_luaWindowRoot)
	m_UIWantLordCtrl = WantLordCtrl.New(m_WantLordRootTrans, m_luaWindowRoot)
	m_UIRedLightCtrl = LightCtrl.New(m_redLightRootTrans, m_luaWindowRoot, PlayGameSys.GetNowPlayId())
	m_UIDoubleShowCtrl = DoubleShowCtrl.New(m_doubleShowRootTrans, m_luaWindowRoot)
end

function ddz_record_play_ui_window.InitWindowDetail()
	-- m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans("bg_root"), "bg_window_" .. LogicUtil.GetPKBgType(), true)
	m_cardPlayLogic = PlayGameSys.GetPlayLogic()
	m_playSpeed = 1
	--显示玩家自己的操作节点
	m_luaWindowRoot:SetActive(m_playRootTrans,true)
	m_luaWindowRoot:SetActive(m_preparedRootTrans,true)

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

function ddz_record_play_ui_window.RefreshBtns(eventId, p1, p2)
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_playRootTrans, "btn_last_round"), PlayRecordSys.CheckLastRecordReplayInfo())
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_playRootTrans, "btn_next_round"),PlayRecordSys.CheckNextRecordReplayInfo())
end

function ddz_record_play_ui_window.GetSpeedSpriteName(speed)
	local spriteNum = speed
	return "replayLayer_text_"..spriteNum
end


function ddz_record_play_ui_window.ChangeWindowShowState()
	m_luaWindowRoot:SetActive(m_friendCardRootTrans,false)
	m_luaWindowRoot:SetActive(m_curTabelPointTrans,false)
	m_luaWindowRoot:SetActive(m_otherOperateTipTrans,false)

	local labelRoomIdTrans = m_luaWindowRoot:GetTrans(m_preparedRootTrans,"label_roomID")
	m_luaWindowRoot:SetLabel(labelRoomIdTrans,"房间号 "..m_cardPlayLogic.roomObj.roomInfo.roomId)
	-- wifi

	-- 电池

	--时间


end

----------------------------Event  start----------------------------------------


function ddz_record_play_ui_window.RegisterEventHandle()
	LuaEvent.AddHandle(EEventType.OtherPlayerOperateTipShow,_s.OtherPlayerOperateTipShow,nil)
	LuaEvent.AddHandle(EEventType.RefreshCurTablePoint,_s.RefreshCurTablePoint,nil)
	LuaEvent.AddHandle(EEventType.RefreshPlayerHead,_s.RefreshPlayerHead,nil)
	LuaEvent.AddHandle(EEventType.PlaySceneInit,_s.PlaySceneInit,nil)
	LuaEvent.AddHandle(EEventType.PlayOutCardAction,_s.PlayOutCardAction,nil)
	LuaEvent.AddHandle(EEventType.ClearDesk,_s.ClearDesk,nil)
	LuaEvent.AddHandle(EEventType.ClearPlayerOutCard,_s.ClearPlayerOutCard,nil)
	LuaEvent.AddHandle(EEventType.PlayerShowFriendCard,_s.PlayerShowFriendCard,nil)
	LuaEvent.AddHandle(EEventType.RefreshRoomRoundNum,_s.RefreshRoomRoundNum,nil)
	LuaEvent.AddHandle(EEventType.RefreshRecordBtns,_s.RefreshBtns,nil)
	LuaEvent.AddHandle(EEventType.RefreshRecordPauseStatus,_s.RefreshRecordPauseStatus,nil)
	LuaEvent.AddHandle(EEventType.DDZMultiplePBInfo, _s.DealDDZMultipleNum, nil)
end

function ddz_record_play_ui_window.UnRegisterEventHandle()
	LuaEvent.RemoveHandle(EEventType.OtherPlayerOperateTipShow,_s.OtherPlayerOperateTipShow,nil)
	LuaEvent.RemoveHandle(EEventType.RefreshCurTablePoint,_s.RefreshCurTablePoint,nil)
	LuaEvent.RemoveHandle(EEventType.RefreshPlayerHead,_s.RefreshPlayerHead,nil)
	LuaEvent.RemoveHandle(EEventType.PlaySceneInit,_s.PlaySceneInit,nil)
	LuaEvent.RemoveHandle(EEventType.PlayOutCardAction,_s.PlayOutCardAction,nil)
	LuaEvent.RemoveHandle(EEventType.ClearDesk,_s.ClearDesk,nil)
	LuaEvent.RemoveHandle(EEventType.ClearPlayerOutCard,_s.ClearPlayerOutCard,nil)
	LuaEvent.RemoveHandle(EEventType.PlayerShowFriendCard,_s.PlayerShowFriendCard,nil)
	LuaEvent.RemoveHandle(EEventType.RefreshRoomRoundNum,_s.RefreshRoomRoundNum,nil)
	LuaEvent.RemoveHandle(EEventType.RefreshRecordBtns,_s.RefreshBtns,nil)
	LuaEvent.RemoveHandle(EEventType.RefreshRecordPauseStatus,_s.RefreshRecordPauseStatus,nil)
	LuaEvent.RemoveHandle(EEventType.DDZMultiplePBInfo, _s.DealDDZMultipleNum, nil)
end


function ddz_record_play_ui_window.OtherPlayerOperateTipShow(eventId, p1, p2)
	local showText = p1
	if showText then
		m_luaWindowRoot:SetActive(m_otherOperateTipTrans,true)
		m_luaWindowRoot:SetLabel(m_otherOperateTipTrans,showText)
	else
		m_luaWindowRoot:SetActive(m_otherOperateTipTrans,false)
	end
end

function ddz_record_play_ui_window.RefreshCurTablePoint(eventId, p1, p2)
	local pointNum = p1
	if pointNum > 0 then
		m_luaWindowRoot:SetActive(m_curTabelPointTrans,true)
		m_luaWindowRoot:SetLabel(m_curTabelPointTrans,"dp"..pointNum)
	else
		m_luaWindowRoot:SetActive(m_curTabelPointTrans,false)
	end
end

function ddz_record_play_ui_window.RefreshPlayerHead(eventId,p1,p2)
	local seatPos = p1
	local seatInfo = p2
	m_UIPlayerHeadCtrl:RefreshPlayerHead(seatPos,seatInfo)
end

function ddz_record_play_ui_window.PlaySceneInit(eventId,p1,p2)
	local isPlayNow = p1
	-- 刷新头像位置
	-- m_UIPlayerHeadCtrl:RefreshPlayerHeadPos(SeatPosEnum.South)
	-- m_UIPlayerHeadCtrl:RefreshPlayerHeadPos(SeatPosEnum.East)
	-- m_UIPlayerHeadCtrl:RefreshPlayerHeadPos(SeatPosEnum.North)
	-- m_UIPlayerHeadCtrl:RefreshPlayerHeadPos(SeatPosEnum.West)
	--开启相关节点显示
	_s.ChangeWindowShowState()
end

function ddz_record_play_ui_window.DealDDZMultipleNum(eventId,p1,p2)
	local pb = p1
	m_luaWindowRoot:SetLabel(m_multipleNumShowTrans, pb.sum)

	_s.SetDetailInfo()
end


function ddz_record_play_ui_window.PlayOutCardAction(eventId,p1,p2)
	local rsp = p1
	local isReconnect = p2
	if rsp then
		local player = m_cardPlayLogic.roomObj.playerMgr:GetPlayerByPlayerID(rsp.userId)
		if player then

			if rsp.isSkip then
				-- 移除牌桌牌
				m_UIOutCardShowCtrl:ClearCards(player.seatPos) --考虑到重连后服务端下发的一轮出牌信息中，玩家会参与多次出牌
				-- 显示不出
				m_UIPlayerHeadCtrl:ShowNotOutTips(player.seatPos,true)
			else
				--先清理提示
				m_UIPlayerHeadCtrl:ShowNotOutTips(player.seatPos,false)--考虑到重连后服务端下发的一轮出牌信息中，玩家会参与多次出牌
				--显示桌牌
				m_UIHandCardCtrl:OutCards(player,rsp.pai)
				local srcCards = {}
				local cardItem
				for k,v in pairs(rsp.pai) do
					cardItem = CCard.New()
					cardItem:Init(v,k)
					table.insert(srcCards,cardItem)
				end
				m_UIOutCardShowCtrl:PlayOutCardsShow(player.seatPos,srcCards)
			end
		end
	end
end


function ddz_record_play_ui_window.ClearDesk(eventId,p1,p2)
	m_UIOutCardShowCtrl:ClearAllCards()
	m_UIPlayerHeadCtrl:ClearAllNotOutTips()
end


function ddz_record_play_ui_window.ClearPlayerOutCard(eventId,p1,p2)
	local seatPos = p1
	-- 移除不出提示
	m_UIPlayerHeadCtrl:ShowNotOutTips(seatPos,false)
	-- 移除牌桌牌
	m_UIOutCardShowCtrl:ClearCards(seatPos)

end

--刷新房间轮数
function ddz_record_play_ui_window.RefreshRoomRoundNum(eventId,p1,p2)
	local curRound = p1
	local totalRound = p2
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_roundRootTrans,"label_cur_round"),curRound)
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_roundRootTrans,"label_total_round"),"/"..totalRound)
end
--展示找朋友亮牌

function ddz_record_play_ui_window.PlayerShowFriendCard(eventId,p1,p2)
	local friendCardID = p1
	local isShow = p2
	local resCardItem = LuaTableSys.GetEntry("ResPKCardList",friendCardID)
	if resCardItem then
		m_luaWindowRoot:SetActive(m_friendCardRootTrans,isShow)
		local numTrans = m_luaWindowRoot:GetTrans(m_friendCardRootTrans,"ico_num")
		local smallIcoTrans = m_luaWindowRoot:GetTrans(m_friendCardRootTrans,"ico_cardType")
		if resCardItem.ID < 83 then
			m_luaWindowRoot:SetSprite(numTrans,resCardItem.ico_num)
			m_luaWindowRoot:SetSprite(smallIcoTrans,resCardItem.ico_big)
		elseif resCardItem.ID == 83 then
			m_luaWindowRoot:SetSprite(numTrans,"gameui_text_king")
			m_luaWindowRoot:SetSprite(smallIcoTrans,"gameui_text_two")
		elseif resCardItem.ID == 84 then
			m_luaWindowRoot:SetSprite(numTrans,"gameui_text_king")
			m_luaWindowRoot:SetSprite(smallIcoTrans,"gameui_text_one")
		end
	end
end

function ddz_record_play_ui_window.RefreshRecordPauseStatus(eventId,p1,p2)
	local isPause = p1
	m_isPause = isPause
	if m_isPause then
		m_luaWindowRoot:SetSprite(m_labelPauseTrans,"replayLayer_text_play")
	else
		m_luaWindowRoot:SetSprite(m_labelPauseTrans,"replayLayer_text_stop")
	end
end

-----------------------------Event End -------------------------------------------------


--------------------------button click -------------------------------------------------


function ddz_record_play_ui_window.HandleWidgetClick(gb)
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
	elseif click_name == "Show_Root" then
		m_luaWindowRoot:SetActive(m_detailRootTrans, true)
		_s.SetDetailInfo()
	elseif click_name == "DetailBG" then
		m_luaWindowRoot:SetActive(m_detailRootTrans, false)
	end
end

function ddz_record_play_ui_window.SetDetailInfo()
	local multipleInfo = m_cardPlayLogic.MyPBMultiple
	local emptyStr = "- -"
	-- 初始分
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_detailRootTrans, "d_init"), "x " .. multipleInfo.base)
	-- 炸弹倍数
	if multipleInfo.bomb > 1 then
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_detailRootTrans, "d_bomb"), "x " .. multipleInfo.bomb)
	else
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_detailRootTrans, "d_bomb"), emptyStr)
	end
	-- 炸弹是否达到上限
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_detailRootTrans, "d_bombLimit"), multipleInfo.isBombLimit)
	-- 明牌
	local openText = ""
	if multipleInfo.open > 1 then
		openText = "x " .. multipleInfo.open
	else
		openText = emptyStr
	end
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_detailRootTrans, "d_open"), openText)
	-- 春天反春天	
	local springText = ""
	if multipleInfo.spring > 1 then
		springText = "x " .. multipleInfo.spring
	elseif multipleInfo.antiSpring > 1 then
		springText = "x " .. multipleInfo.antiSpring
	else
		springText = emptyStr
	end
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_detailRootTrans, "d_spring"), springText)
	-- 公共倍数
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_detailRootTrans, "d_commonplus"), "x " .. multipleInfo.common)
	-- 农民加倍
	if multipleInfo.plus > 0 then
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_detailRootTrans, "d_famerplus"), "x " .. multipleInfo.plus)
	else
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_detailRootTrans, "d_famerplus"), emptyStr)
	end
	
	-- 地主加倍
	if multipleInfo.back > 0 then
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_detailRootTrans, "d_lordplus"), "x " .. multipleInfo.back)
	else
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_detailRootTrans, "d_lordplus"), emptyStr)
	end
	--总倍数
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_detailRootTrans, "d_totalplus"), "" .. multipleInfo.sum)
end

function ddz_record_play_ui_window.HandleClickLastRound()
	LuaEvent.AddEventNow(EEventType.RecordPlayLastRound)
end

function ddz_record_play_ui_window.HandleClickReply()
	LuaEvent.AddEventNow(EEventType.RecordReplayCurRound)
end

function ddz_record_play_ui_window.HandleClickPause()
	m_isPause = not m_isPause
	if m_isPause then
		m_luaWindowRoot:SetSprite(m_labelPauseTrans,"replayLayer_text_play")
	else
		m_luaWindowRoot:SetSprite(m_labelPauseTrans,"replayLayer_text_stop")
	end
	LuaEvent.AddEventNow(EEventType.RecordPausePlay,m_isPause)
end

function ddz_record_play_ui_window.HandleClickSpeed()
	m_playSpeed = m_playSpeed + 1
	if m_playSpeed > PlayRecordSys.playInterval then
		m_playSpeed = 1
	end

	m_luaWindowRoot:SetSprite(m_labelPlaySpeedTrans,_s.GetSpeedSpriteName(m_playSpeed))
	m_luaWindowRoot:MakePixelPerfect(m_labelPlaySpeedTrans)

	LuaEvent.AddEventNow(EEventType.RecordSetPlaySpeed,m_playSpeed)
end

function ddz_record_play_ui_window.HandleClickNextRound()
	LuaEvent.AddEventNow(EEventType.RecordPlayNextRound)
end


function ddz_record_play_ui_window.HandleClickQuit()
	local contentStr = "确定退出回放"
	local okFunc = function ()
			GameManager.SetGameTimeScale(1)
			PlayGameSys.QuitToMainCity()
		end
	WindowUtil.ShowErrorWindow(2,contentStr,nil,okFunc)
end

function ddz_record_play_ui_window.HanldeClickBgBtn()
	_s.ShowPlayBtnRoot(not _s.m_showBtnRoot,true)
end

-- 回放按钮隐藏与显示
function ddz_record_play_ui_window.ShowPlayBtnRoot(isFoword,needTweenAnim)
	if isFoword ~= _s.m_showBtnRoot then
		m_luaWindowRoot:PlayForceAnimation_Lua(m_playRootTrans,function ()
			_s.m_timeCount = 0
		end,isFoword,needTweenAnim)
		_s.m_showBtnRoot = isFoword
	end
end


------------------------------button click end -------------------------------------------------

function ddz_record_play_ui_window.UpdateSecond()
	if _s.m_showBtnRoot then
		_s.m_timeCount = _s.m_timeCount + 1
		-- DwDebug.Log("当前的时间缩放参数 "..tostring(UnityEngine.Time.timeScale))
		if _s.m_timeCount >= 4*(UnityEngine.Time.timeScale) then
			_s.m_timeCount = 0
			_s.ShowPlayBtnRoot(false,true)
		end
	end
end

function ddz_record_play_ui_window.UnRegister()
    m_luaWindowRoot = nil
end

function ddz_record_play_ui_window.OnDestroy()
    _s.UnRegisterEventHandle()
    if m_UIPlayerHeadCtrl then
        m_UIPlayerHeadCtrl:Destroy()
        m_UIPlayerHeadCtrl = nil
    end
     if m_UIHandCardCtrl then
        m_UIHandCardCtrl:Destroy()
        m_UIHandCardCtrl = nil
    end
     if m_UIOutCardShowCtrl then
        m_UIOutCardShowCtrl:Destroy()
        m_UIOutCardShowCtrl = nil
    end
    if m_UICardAnimationCtrl then
        m_UICardAnimationCtrl:Destroy()
        m_UICardAnimationCtrl = nil
    end
	if m_UICommonOtherInfoCtrl then
		m_UICommonOtherInfoCtrl:Destroy()
		m_UICommonOtherInfoCtrl = nil
	end
	
	if m_UIAddDoubleCtrl then
		m_UIAddDoubleCtrl:Destroy()
		m_UIAddDoubleCtrl = nil
	end

	if m_UIBaseCardsCtrl then
		m_UIBaseCardsCtrl:Destroy()
		m_UIBaseCardsCtrl = nil
	end

	if m_UILordOpenPlayCtrl then
		m_UILordOpenPlayCtrl:Destroy()
		m_UILordOpenPlayCtrl = nil
	end

	if m_UIWantLordCtrl then
		m_UIWantLordCtrl:Destroy()
		m_UIWantLordCtrl = nil
	end
	
	if m_UIRedLightCtrl then
		m_UIRedLightCtrl:Destroy()
		m_UIRedLightCtrl = nil
	end

	if m_UIDoubleShowCtrl then
		m_UIDoubleShowCtrl:Destroy()
		m_UIDoubleShowCtrl = nil
	end
end
