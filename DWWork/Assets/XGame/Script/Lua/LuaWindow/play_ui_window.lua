--------------------------------------------------------------------------------
-- 	 File	  : play_ui_window.lua
--   author	: guoliang
--   function   : 510k 打牌主UI
--   date	  : 2017-9-28
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.UICtrlLogic.UIHandCardCtrl"
require "Logic.UICtrlLogic.UIOutCardShowCtrl"
require "Logic.UICtrlLogic.UIPlayerHeadCtrl"
require "Logic.UICtrlLogic.UICardAnimationCtrl"
require "Logic.UICtrlLogic.UICommonOtherInfoCtrl"

local m_luaWindowRoot
local m_state
--玩家叫牌打独根节点
local m_aloneRootTrans
--玩家找朋友根节点
local m_findFriendRootTrans
--玩家出牌根节点
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
--玩家邀请准备展示根节点
local m_prepareRootTrans
--玩家进入游戏显示根节点
local m_preparedRootTrans

-- 底部聊天根节点
local m_bottomChatRootTrans
-- 顶部左边设置根节点
local m_topLeftRoomSetRootTrans
-- 顶部右边房间轮数显示节点
local m_roundRootTrans
-- 开始后准备按钮节点
local m_preparePlayBtnTrans
-- 自己准备按钮节点
local m_prepareInviteBtnTrans
-- 自己准备按钮节点
local m_unprepareInviteBtnTrans
-- 动画节点
local m_animationRootTrans
--出牌节点
local followTrans,forceTrans

-- 其他信息组件
local m_UICommonOtherInfoCtrl

--手牌显示控制组件
local m_UIHandCardCtrl
--出牌显示控制
local m_UIOutCardShowCtrl
--玩家头部显示组件
local m_UIPlayerHeadCtrl
--动画播放组件
local m_UICardAnimationCtrl



--游戏逻辑
local m_cardPlayLogic
--提示结果是否准备好
local m_tipResultReady = false
--是否展示了房间设置界面
local  m_isShowRoomSet = nil

--排序根节点
local sortContent
-- 手牌排序状态按钮
local m_sortTypeBtn
local btn_CardTag,btn_CardTag_back	--理牌按钮
--提示和不出按钮
local btn_tips,btn_out_card
--是否在找朋友中
local isFindFriend = false

play_ui_window = {

}
local _s = play_ui_window

function play_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function play_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		m_state = state
		_s.InitWindowDetail()
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, Common_PlayID.chongRen_510K, "chat_ui_window", false , nil)
	end
end

function play_ui_window.CreateWindow()
	m_aloneRootTrans = m_luaWindowRoot:GetTrans("alone_root")
	m_findFriendRootTrans = m_luaWindowRoot:GetTrans("findFriend_root")
	m_playRootTrans = m_luaWindowRoot:GetTrans("play_root")
	m_handCardRootTrans = m_luaWindowRoot:GetTrans("hand_card_root")
	m_outCardsShowRootTrans = m_luaWindowRoot:GetTrans("out_card_show_root")
	m_otherOperateTipTrans = m_luaWindowRoot:GetTrans("label_other_operate_tips")
	m_curTabelPointTrans = m_luaWindowRoot:GetTrans("label_cur_table_point")
	m_playerHeadRootTrans = m_luaWindowRoot:GetTrans("player_icon_root")
	m_prepareRootTrans = m_luaWindowRoot:GetTrans("prepare_root")
	m_preparedRootTrans = m_luaWindowRoot:GetTrans("prepared_root")
	m_friendCardRootTrans =  m_luaWindowRoot:GetTrans(m_preparedRootTrans,"friend_card_root")
	m_preparePlayBtnTrans = m_luaWindowRoot:GetTrans("btn_prepare_play")
	m_prepareInviteBtnTrans = m_luaWindowRoot:GetTrans("btn_prepare_invite")
	m_unprepareInviteBtnTrans = m_luaWindowRoot:GetTrans("btn_canelprepare_invite")
	m_animationRootTrans = m_luaWindowRoot:GetTrans("animation_root")

	m_roundRootTrans = m_luaWindowRoot:GetTrans("round_root")
	m_topLeftRoomSetRootTrans = m_luaWindowRoot:GetTrans("room_set_root")
	m_bottomChatRootTrans = m_luaWindowRoot:GetTrans("chat_root")

	followTrans = m_luaWindowRoot:GetTrans(m_playRootTrans,"follow_play")
	forceTrans = m_luaWindowRoot:GetTrans(m_playRootTrans,"force_play")

	m_luaWindowRoot:SetActive(m_topLeftRoomSetRootTrans,false)
	m_luaWindowRoot:SetActive(m_bottomChatRootTrans,true)

	m_luaWindowRoot:SetActive(m_curTabelPointTrans,false)
	m_luaWindowRoot:SetActive(m_otherOperateTipTrans,false)
	m_luaWindowRoot:SetActive(m_friendCardRootTrans,false)

	-- 初始化邀请按钮是否变灰
	_s.RefreshRoomFull(nil, false)

	_s.RegisterEventHandle()

	m_UIPlayerHeadCtrl = UIPlayerHeadCtrl.New()
	m_UIPlayerHeadCtrl:Init(m_playerHeadRootTrans,m_luaWindowRoot)

	m_UIOutCardShowCtrl = UIOutCardShowCtrl.New()
	m_UIOutCardShowCtrl:Init(m_outCardsShowRootTrans,m_luaWindowRoot)

	m_UIHandCardCtrl = UIHandCardCtrl.New()
	m_UIHandCardCtrl:Init(m_handCardRootTrans,m_luaWindowRoot)

	m_UICardAnimationCtrl = UICardAnimationCtrl.New()
	m_UICardAnimationCtrl:Init(m_animationRootTrans,m_luaWindowRoot)

	m_UICommonOtherInfoCtrl = UICommonOtherInfoCtrl.New()
	m_UICommonOtherInfoCtrl:Init(m_topLeftRoomSetRootTrans,m_luaWindowRoot)

	local btn_speak = m_luaWindowRoot:GetTrans("btn_speak").gameObject
	GetOrAddLuaComponent(btn_speak, "LuaWindow.Chat.ChatVoiceBtn", true)
	
	sortContent = m_luaWindowRoot:GetTrans("sortContent")
	m_sortTypeBtn =  m_luaWindowRoot:GetTrans("btn_sort_type")
	btn_tips = m_luaWindowRoot:GetTrans("btn_tips")
	btn_out_card = m_luaWindowRoot:GetTrans("btn_out_card")

	btn_CardTag = m_luaWindowRoot:GetTrans(sortContent,"btn_CardTag")
	btn_CardTag_back = m_luaWindowRoot:GetTrans(sortContent,"btn_CardTag_back")
	
end

function play_ui_window.SetWaterMark()
	local waterConfig = 
	{
		[Common_PlayID.chongRen_510K] = "watermark_chongren510K",
		[Common_PlayID.leAn_510K] = "watermark_lean510K",
		[Common_PlayID.yiHuang_510K] = "watermark_yihuang510K",
	}
	local name = waterConfig[PlayGameSys.GetNowPlayId()]
	if nil == name then
		DwDebug.LogError("no found water mark", PlayGameSys.GetNowPlayId())
		return
	end

	m_luaWindowRoot:LoadImag(m_luaWindowRoot:GetTrans("bg_water"), "", name, false, RessStorgeType.RST_Never)
end

function play_ui_window.InitWindowDetail()
	m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans("bg_windows"), "bg_window_" .. LogicUtil.GetPKBgType(), true)
	m_cardPlayLogic = PlayGameSys.GetPlayLogic()
	_s.SetWaterMark()

	--隐藏玩家自己的操作节点
	m_luaWindowRoot:SetActive(m_aloneRootTrans,false)
	m_luaWindowRoot:SetActive(m_findFriendRootTrans,false)
	m_luaWindowRoot:SetActive(m_playRootTrans,false)
	m_luaWindowRoot:SetActive(m_prepareRootTrans,false)
	m_luaWindowRoot:SetActive(m_preparedRootTrans,false)
	m_luaWindowRoot:SetActive(m_preparePlayBtnTrans,false)

	_s.RefreshSortContent()
end

function play_ui_window.ChangeWindowShowState(isPlayNow)
	m_luaWindowRoot:SetActive(m_prepareRootTrans,not isPlayNow)
	m_luaWindowRoot:SetActive(m_preparedRootTrans,isPlayNow)
	m_luaWindowRoot:SetActive(m_friendCardRootTrans,false)
	m_luaWindowRoot:SetActive(m_curTabelPointTrans,false)
	m_luaWindowRoot:SetActive(m_otherOperateTipTrans,false)

	if isPlayNow then
		local labelRoomIdTrans = m_luaWindowRoot:GetTrans(m_preparedRootTrans,"label_roomID")
		m_luaWindowRoot:SetLabel(labelRoomIdTrans,"房间号 "..m_cardPlayLogic.roomObj.roomInfo.roomId)

		-- 刷新wifi 电量 时间
		DwDebug.Log(" 刷新wifi 电量 时间 ")
		LuaEvent.AddEventNow(EEventType.RefreshOtherInfo)

		LuaEvent.AddEvent(EEventType.PlayerHeadPosChange, m_UIPlayerHeadCtrl:GetHeadPos(false))
	else-- 邀请准备阶段
		local labelQuitTrans = m_luaWindowRoot:GetTrans(m_prepareRootTrans,"label_quit")
		if m_cardPlayLogic.roomObj:CheckSelfIsRoomOwner() then
			m_luaWindowRoot:SetSprite(labelQuitTrans,"gameui_disbandroom")
		else
			m_luaWindowRoot:SetSprite(labelQuitTrans,"gameui_text_leveroom")
		end

		-- 显示邀请按钮
		if DataManager.isPrePublish then
			m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_prepareRootTrans,"btn_invite"),false)
		else
			m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_prepareRootTrans,"btn_invite"),true)
		end

		local roomInfo = m_cardPlayLogic.roomObj.roomInfo
		if roomInfo then
			local selfPlayer = m_cardPlayLogic.roomObj.playerMgr:GetPlayerByPlayerID(DataManager.GetUserID())
			if roomInfo.currentGamepos == 1 then
				if selfPlayer and selfPlayer:IsState(PlayerStateEnum.Prepared) then
					m_luaWindowRoot:SetActive(m_prepareInviteBtnTrans, false)
					m_luaWindowRoot:SetActive(m_unprepareInviteBtnTrans, true)
				else
					m_luaWindowRoot:SetActive(m_prepareInviteBtnTrans, true)
					m_luaWindowRoot:SetActive(m_unprepareInviteBtnTrans, false)
				end
			else
				if selfPlayer and selfPlayer:IsState(PlayerStateEnum.Prepared) then
					m_luaWindowRoot:SetActive(m_preparePlayBtnTrans, false)
				else
					m_luaWindowRoot:SetActive(m_preparePlayBtnTrans, true)
				end
			end

			local labelRoomIdTrans = m_luaWindowRoot:GetTrans(m_prepareRootTrans,"label_room_id")
			m_luaWindowRoot:SetLabel(labelRoomIdTrans,"房间号 " .. roomInfo.roomId)
		end

		LuaEvent.AddEvent(EEventType.PlayerHeadPosChange, m_UIPlayerHeadCtrl:GetHeadPos(true))
	end
end
----------------------------Event  start----------------------------------------
function play_ui_window.RegisterEventHandle()
	LuaEvent.AddHandle(EEventType.OtherPlayerOperateTipShow,_s.OtherPlayerOperateTipShow,nil)
	LuaEvent.AddHandle(EEventType.RefreshCurTablePoint,_s.RefreshCurTablePoint,nil)
	LuaEvent.AddHandle(EEventType.RefreshPlayerHead,_s.RefreshPlayerHead,nil)
	LuaEvent.AddHandle(EEventType.RefreshPlayWindowStatus,_s.RefreshPlayWindowStatus,nil)
	LuaEvent.AddHandle(EEventType.PlaySceneInit,_s.PlaySceneInit,nil)
	LuaEvent.AddHandle(EEventType.PlayNotPrepare,_s.PlayNotPrepare,nil)
	LuaEvent.AddHandle(EEventType.SelfCallAlone,_s.SelfCallAlone,nil)
	LuaEvent.AddHandle(EEventType.SelfFindFriend,_s.SelfFindFriend,nil)
	LuaEvent.AddHandle(EEventType.SelfOutCard,_s.SelfOutCard,nil)
	LuaEvent.AddHandle(EEventType.PlayOutCardAction,_s.PlayOutCardAction,nil)
	LuaEvent.AddHandle(EEventType.ClearDesk,_s.ClearDesk,nil)
	LuaEvent.AddHandle(EEventType.ClearPlayerOutCard,_s.ClearPlayerOutCard,nil)
	LuaEvent.AddHandle(EEventType.PlayerShowFriendCard,_s.PlayerShowFriendCard,nil)
	LuaEvent.AddHandle(EEventType.RefreshRoomRoundNum,_s.RefreshRoomRoundNum,nil)
	LuaEvent.AddHandle(EEventType.ChangePlayCanvas,_s.ChangePlayCanvas,nil)
	LuaEvent.AddHandle(EEventType.RefreshRoomFull, _s.RefreshRoomFull)
	LuaEvent.AddHandle(EEventType.EndDistributCard, _s.EndDistributeCard, nil)
	LuaEvent.AddHandle(EEventType.SelfHandCardClear, _s.DealPlayerHandCardClear, nil)
	LuaEvent.AddHandle(EEventType.PlayerShowSort, _s.DealPlayerShowSort, nil)
	LuaEvent.AddHandle(EEventType.RefreshRoomSetRoot,_s.RefreshRoomSetRoot)

	LuaEvent.AddHandle(EEventType.CardDownUpChange,_s.RefreshTagBtnState)
end

function play_ui_window.UnRegisterEventHandle()
	LuaEvent.RemoveHandle(EEventType.OtherPlayerOperateTipShow,_s.OtherPlayerOperateTipShow,nil)
	LuaEvent.RemoveHandle(EEventType.RefreshCurTablePoint,_s.RefreshCurTablePoint,nil)
	LuaEvent.RemoveHandle(EEventType.RefreshPlayerHead,_s.RefreshPlayerHead,nil)
	LuaEvent.RemoveHandle(EEventType.RefreshPlayWindowStatus,_s.RefreshPlayWindowStatus,nil)
	LuaEvent.RemoveHandle(EEventType.PlaySceneInit,_s.PlaySceneInit,nil)
	LuaEvent.RemoveHandle(EEventType.PlayNotPrepare,_s.PlayNotPrepare,nil)
	LuaEvent.RemoveHandle(EEventType.SelfCallAlone,_s.SelfCallAlone,nil)
	LuaEvent.RemoveHandle(EEventType.SelfFindFriend,_s.SelfFindFriend,nil)
	LuaEvent.RemoveHandle(EEventType.SelfOutCard,_s.SelfOutCard,nil)
	LuaEvent.RemoveHandle(EEventType.PlayOutCardAction,_s.PlayOutCardAction,nil)
	LuaEvent.RemoveHandle(EEventType.ClearDesk,_s.ClearDesk,nil)
	LuaEvent.RemoveHandle(EEventType.ClearPlayerOutCard,_s.ClearPlayerOutCard,nil)
	LuaEvent.RemoveHandle(EEventType.PlayerShowFriendCard,_s.PlayerShowFriendCard,nil)
	LuaEvent.RemoveHandle(EEventType.RefreshRoomRoundNum,_s.RefreshRoomRoundNum,nil)
	LuaEvent.RemoveHandle(EEventType.ChangePlayCanvas,_s.ChangePlayCanvas,nil)
	LuaEvent.RemoveHandle(EEventType.RefreshRoomFull, _s.RefreshRoomFull)
	LuaEvent.RemoveHandle(EEventType.EndDistributCard, _s.EndDistributeCard, nil)
	LuaEvent.RemoveHandle(EEventType.SelfHandCardClear, _s.DealPlayerHandCardClear, nil)
	LuaEvent.RemoveHandle(EEventType.PlayerShowSort, _s.DealPlayerShowSort, nil)
	LuaEvent.RemoveHandle(EEventType.RefreshRoomSetRoot,_s.RefreshRoomSetRoot)

	LuaEvent.RemoveHandle(EEventType.CardDownUpChange,_s.RefreshTagBtnState)
end


function play_ui_window.OtherPlayerOperateTipShow(eventId, p1, p2)
	local showText = p1
	if showText then
		m_luaWindowRoot:SetActive(m_otherOperateTipTrans,true)
		m_luaWindowRoot:SetLabel(m_otherOperateTipTrans,showText)
	else
		m_luaWindowRoot:SetActive(m_otherOperateTipTrans,false)
	end
end

function play_ui_window.RefreshCurTablePoint(eventId, p1, p2)
	local pointNum = p1
	if pointNum > 0 then
		m_luaWindowRoot:SetActive(m_curTabelPointTrans,true)
		m_luaWindowRoot:SetLabel(m_curTabelPointTrans,"dp"..pointNum)
	else
		m_luaWindowRoot:SetActive(m_curTabelPointTrans,false)
	end
end

function play_ui_window.RefreshPlayerHead(eventId,p1,p2)
	local seatPos = p1
	local seatInfo = p2
	m_UIPlayerHeadCtrl:RefreshPlayerHead(seatPos,seatInfo)
end

function play_ui_window.PlaySceneInit(eventId,p1,p2)
	local isPlayNow = p1
	-- 刷新头像位置
	-- m_UIPlayerHeadCtrl:RefreshPlayerHeadPos(SeatPosEnum.South)
	-- m_UIPlayerHeadCtrl:RefreshPlayerHeadPos(SeatPosEnum.East)
	-- m_UIPlayerHeadCtrl:RefreshPlayerHeadPos(SeatPosEnum.North)
	-- m_UIPlayerHeadCtrl:RefreshPlayerHeadPos(SeatPosEnum.West)
	--开启相关节点显示
	m_luaWindowRoot:SetActive(m_topLeftRoomSetRootTrans,isPlayNow)
	m_luaWindowRoot:SetActive(m_topLeftRoomSetRootTrans,isPlayNow)
	_s.ChangeWindowShowState(isPlayNow)
	if isPlayNow then
		_s.RefreshRoomSetRoot(nil, false)
	end
end

function play_ui_window.RefreshPlayWindowStatus(eventId,p1,p2)
	local isPlayNow = p1
	-- 刷新头像位置
	-- m_UIPlayerHeadCtrl:RefreshPlayerHeadPos(SeatPosEnum.South)
	-- m_UIPlayerHeadCtrl:RefreshPlayerHeadPos(SeatPosEnum.East)
	-- m_UIPlayerHeadCtrl:RefreshPlayerHeadPos(SeatPosEnum.North)
	-- m_UIPlayerHeadCtrl:RefreshPlayerHeadPos(SeatPosEnum.West)
	-- 切换准备状态与游戏状态
	_s.ChangeWindowShowState(isPlayNow)
end

function play_ui_window.RefreshRoomSetRoot(eventId, isDown)
	if m_isShowRoomSet == isDown then
		return
	end
	DwDebug.Log("play_ui_window.RefreshRoomSetRoot:", isDown)
	m_isShowRoomSet = isDown
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_topLeftRoomSetRootTrans,"set_root"),isDown)
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_topLeftRoomSetRootTrans,"btn_set_down"),not isDown)
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_topLeftRoomSetRootTrans,"btn_set_up"),isDown)
end


function play_ui_window.SelfCallAlone(eventId,p1,p2)
	local isOn = p1
	m_luaWindowRoot:SetActive(m_aloneRootTrans,isOn)
end

function play_ui_window.SelfFindFriend(eventId,p1,p2)
	local isOn = p1
	m_luaWindowRoot:SetActive(m_findFriendRootTrans,isOn)
	isFindFriend = isOn
end

function play_ui_window.SelfOutCard(eventId,p1,p2)
	local isOn = p1
	local isForce = p2
	m_luaWindowRoot:SetActive(m_playRootTrans,isOn)
	if isOn then
		m_luaWindowRoot:SetActive(followTrans,not isForce)
		m_luaWindowRoot:SetActive(forceTrans,isForce)

		if not isForce then
			_s.SetPlayCardBtnState()
		end
	end
end

--获得当前是否可以出牌
function play_ui_window.GetCanOutCard()
    local logicCtrl = PlayGameSys.GetPlayLogic()
    if logicCtrl and logicCtrl.roomObj then
        return logicCtrl.roomObj.canOutCard
    end
    return nil
end

--设置按钮状态
function play_ui_window.SetPlayCardBtnState()
	local canOut = _s.GetCanOutCard()
	--为了灰色颜色一致 用这种方式
	m_luaWindowRoot:SetVisible(m_luaWindowRoot:GetTrans(btn_tips,"btn_bg_gray"),not canOut)
	m_luaWindowRoot:SetVisible(m_luaWindowRoot:GetTrans(btn_out_card,"btn_bg_gray"),not canOut)
	m_luaWindowRoot:SetCanTouched(btn_tips,canOut,false)
	m_luaWindowRoot:SetCanTouched(btn_out_card,canOut,false)
end

function play_ui_window.PlayOutCardAction(eventId,p1,p2)
	local rsp = p1
	local isReconnect = p2
	if rsp then
		local player = m_cardPlayLogic.roomObj.playerMgr:GetPlayerByPlayerID(rsp.userId)
		if player then
			if rsp.isSkip then
				if rsp.userId == DataManager.GetUserID() then
					-- 出牌后重置AI提示结果
					m_cardPlayLogic.cardLogicContainer:ResetAIResult()
					m_UIHandCardCtrl:DownUpCards()
				end
				-- 移除牌桌牌
				m_UIOutCardShowCtrl:ClearCards(player.seatPos) --考虑到重连后服务端下发的一轮出牌信息中，玩家会参与多次出牌
				-- 显示不出
				m_UIPlayerHeadCtrl:ShowNotOutTips(player.seatPos,true)
				_s.OutCardHideBombTip(player.seatPos)
			else
				--先清理提示
				m_UIPlayerHeadCtrl:ShowNotOutTips(player.seatPos,false)--考虑到重连后服务端下发的一轮出牌信息中，玩家会参与多次出牌
				--显示桌牌
				if rsp.userId == DataManager.GetUserID() and not isReconnect then
					-- local chooseCards = m_UIHandCardCtrl:GetUpCards()
					-- if #chooseCards > 0 then
					-- 	m_UIHandCardCtrl:OutCards()
					-- 	m_UIOutCardShowCtrl:PlayOutCardsShow(player.seatPos,chooseCards)
					-- 	_s.SetBombTipShowByRsp(rsp, player.seatPos)
					-- end
				else
					local srcCards = {}
					local cardItem
					for k,v in pairs(rsp.pai) do
						cardItem = CCard.New()
						cardItem:Init(v,k)
						table.insert(srcCards,cardItem)
					end
					m_UIOutCardShowCtrl:PlayOutCardsShow(player.seatPos,srcCards)
					_s.SetBombTipShowByRsp(rsp, player.seatPos)
				end
			end
		end
	end
end

function play_ui_window.PlayNotPrepare(eventId,p1,p2)
	local seatPos = p1
	local isOn = p2
	if seatPos == SeatPosEnum.South then
		local roomInfo = m_cardPlayLogic.roomObj.roomInfo
		if roomInfo then
			DwDebug.Log("PlayNotPrepare currentGamepos = "..roomInfo.currentGamepos)
			if 1 == roomInfo.currentGamepos then
				m_luaWindowRoot:SetActive(m_prepareInviteBtnTrans, isOn)
				m_luaWindowRoot:SetActive(m_unprepareInviteBtnTrans, not isOn)
			else
				m_luaWindowRoot:SetActive(m_preparePlayBtnTrans, isOn)
			end
		end
	end
end

function play_ui_window.ClearDesk(eventId,p1,p2)
	m_UIOutCardShowCtrl:ClearAllCards()
	m_UIPlayerHeadCtrl:ClearAllNotOutTips()
	m_UIOutCardShowCtrl:HideAllBombTip()
end


function play_ui_window.ClearPlayerOutCard(eventId,p1,p2)
	local seatPos = p1
	-- 移除不出提示
	m_UIPlayerHeadCtrl:ShowNotOutTips(seatPos,false)
	-- 移除牌桌牌
	m_UIOutCardShowCtrl:ClearCards(seatPos)
	_s.OutCardHideBombTip(seatPos)
end

--刷新房间轮数
function play_ui_window.RefreshRoomRoundNum(eventId,p1,p2)
	local curRound = p1
	local totalRound = p2
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_roundRootTrans,"label_cur_round"),curRound)
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_roundRootTrans,"label_total_round"),"/"..totalRound)
end
--展示找朋友亮牌

function play_ui_window.PlayerShowFriendCard(eventId,p1,p2)
	print("PlayerShowFriendCard")
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

		if isShow then
			m_UIHandCardCtrl:RefreshAllCardShow()
		end
	end
end

--更换背景
function play_ui_window.ChangePlayCanvas(eventId, p1, p2)
	local index = p1

	if index then
		m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans("bg_windows"), "bg_window_" .. index, true)
	end
end

-- 显示是否满员
function play_ui_window.RefreshRoomFull(eventId, isFull, p2)
	m_luaWindowRoot:SetGray(m_luaWindowRoot:GetTrans(m_prepareRootTrans,"btn_invite"), isFull, true)
end

--刷新理牌按钮状态
function play_ui_window.RefreshSortContent( )
	if m_cardPlayLogic ~= nil and PlayGameSys.GetPlayLogicType() == PlayLogicTypeEnum.WSK_Normal then
		if m_UIHandCardCtrl and m_UIHandCardCtrl.handCards and #m_UIHandCardCtrl.handCards > 0 then
			m_luaWindowRoot:SetActive(sortContent, true)
			_s.RefreshTagBtnState()

			--宜黄没有510K的牌
			local btn_510K_up = m_luaWindowRoot:GetTrans(sortContent,"btn_510K_up")
			m_luaWindowRoot:SetActive(btn_510K_up, PlayGameSys.GetNowPlayId() ~= Common_PlayID.yiHuang_510K)
			return
		end
	end
	m_luaWindowRoot:SetActive(sortContent, false)
end

--刷新理牌按钮状态
function play_ui_window.RefreshTagBtnState()
	if m_cardPlayLogic ~= nil and PlayGameSys.GetPlayLogicType() == PlayLogicTypeEnum.WSK_Normal then

		local showLiPai = false
		showLiPai = m_UIHandCardCtrl:IsInLiPaiState()

		m_luaWindowRoot:SetVisible(btn_CardTag,showLiPai)
		m_luaWindowRoot:SetVisible(btn_CardTag_back,not showLiPai)
	end
end

-- 发牌完毕
function play_ui_window.EndDistributeCard(eventId, p1, p2)
	_s.RefreshSortContent()
end

-- 玩家摊牌，表示一小局结束
function play_ui_window.DealPlayerHandCardClear(eventId, p1, p2)
	if m_cardPlayLogic ~= nil and PlayGameSys.GetPlayLogicType() == PlayLogicTypeEnum.WSK_Normal then
		m_luaWindowRoot:SetActive(sortContent, false)
	end
end

-- 当我出玩牌了，把排序按钮隐藏
function play_ui_window.DealPlayerShowSort(eventId, p1, p2)
	local seatPos = p1
	if seatPos == SeatPosEnum.South then
		_s.RefreshSortContent()
	end
end
-----------------------------Event End -------------------------------------------------

--------------------------button click -------------------------------------------------

local function FindCardIndex(transName)
	if string.find(transName, "_") ~= nil then
		local firstSplit = string.find(transName, "_")
		local id = tonumber(string.sub(transName, 1, firstSplit - 1))
		return id
	end
end

function play_ui_window.HandleWidgetClick(gb)
	_s.RefreshRoomSetRoot(nil, false)
	local click_name = gb.name
	if click_name == "btn_out_card" then
		_s.HandleClickOutCard()
	elseif click_name == "btn_tips" then
		_s.HandleClickTipsCard()
	elseif click_name == "btn_no_out" then
		_s.HandleClickNoOutCard()
	elseif click_name == "btn_alone" then
		_s.HandleClickAlone()
	elseif click_name == "btn_no_alone" then
		m_cardPlayLogic:SendReplyIsAlone(false)
	elseif click_name == "btn_prepare_play" or click_name == "btn_prepare_invite" then
		WrapSys.AudioSys_PlayEffect("Common/UI/UIclick_1")
		m_cardPlayLogic:SendPrepare(0)
	elseif click_name == "btn_canelprepare_invite" then
		WrapSys.AudioSys_PlayEffect("Common/UI/UIclick_1")
		m_cardPlayLogic:SendPrepare(1)
	elseif click_name == "btn_showFriendCard" then
		_s.HandleShowFriendCard()
	elseif click_name == "btn_set_down" then
		_s.RefreshRoomSetRoot(nil, true)
	elseif click_name == "btn_set_up" then
		-- _s.RefreshRoomSetRoot(false)
	elseif click_name == "btn_room_bg" then
		-- _s.RefreshRoomSetRoot(not m_isShowRoomSet)
	elseif click_name == "btn_location" then
		local logic = PlayGameSys.GetPlayLogic()
		if logic then
			logic:SendAskLBS()
		end
	elseif click_name == "btn_chat" then
		LuaEvent.AddEvent(EEventType.Chat_ui_window_Show)
	elseif click_name == "btn_quit" then
		_s.HandleClickQuit()
	elseif click_name == "btn_set" then
		_s.HandleClickSet()
	elseif click_name == "btn_help" then
		_s.HandleClickHelp()
	elseif click_name == "btn_detail" then
		
		if m_cardPlayLogic and m_cardPlayLogic:CheckRoomInfoLegal() then
			WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "roomDetail_ui_window", false , nil)
		end
	elseif click_name == "btn_invite" then --邀请
		HallSys.ShareRoom(m_cardPlayLogic.roomObj.roomInfo.roomId)
	elseif click_name == "btn_sort_type" then
		if _s.IsInFindFriend() then
			return
		end
		m_UIHandCardCtrl:ChangeCardSort()
	elseif click_name == "btn_510K_up" then 	-- 抬起510K牌型
		if _s.IsInFindFriend() then
			return
		end

		m_UIHandCardCtrl:Up510KCard()
	elseif click_name == "btn_CardTag" or click_name == "btn_CardTag_back" then 	-- 标记选牌 并重新排序
		if _s.IsInFindFriend() then
			return
		end
		m_UIHandCardCtrl:CardTag()
	elseif click_name == "CopyBtn" then
		_s.OnCopyBtnClick()
	elseif string.find(click_name, "headCollider_") then
		m_UIPlayerHeadCtrl:HandleWidgetClick(gb)
	else
		-- local parent = gb.transform.parent
		-- if parent and parent.name == m_handCardRootTrans.name then
		-- 	local cardIndex = FindCardIndex(click_name)
		-- 	if cardIndex and m_UIHandCardCtrl then
		-- 		m_UIHandCardCtrl:ClickCard(gb.transform,cardIndex)
		-- 	end
		-- end
	end
end

function play_ui_window.IsInFindFriend()
	if isFindFriend then
		WindowUtil.LuaShowTips("找朋友中，不能理牌")
	end
	return isFindFriend
end


--打独
function play_ui_window.HandleClickAlone()
	local contentStr = "您是否确定打独 ？"
	WrapSys.HandleErrorWindow_ContentSt = contentStr
	WrapSys.HandleErrorWindow_SetOkCallBack(function()
		m_cardPlayLogic:SendReplyIsAlone(true)
	end)
	WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.error_ui_window, true, 1, "")
end

--出牌
function play_ui_window.HandleClickOutCard()
	local curOutInfo = m_cardPlayLogic.roomObj.selfPlayer.outInfo
	local handCardCount = #m_UIHandCardCtrl.handCards
	local chooseCards = m_UIHandCardCtrl:GetUpCards()
	local chooseCardIds = {}
	for k,v in pairs(chooseCards) do
		table.insert(chooseCardIds,v.ID)
	end
	if #chooseCards > 0 then
		if curOutInfo.isForce then
			local canOut, cardType = m_cardPlayLogic.cardLogicContainer:CheckCanOutCard(chooseCards,handCardCount)
			if canOut then
				m_cardPlayLogic:SendOutCard(chooseCardIds,curOutInfo.token)
				--先出牌
			 	_s.PlayOutCardActionPre(false, chooseCards, cardType)
			else
				WindowUtil.LuaShowTips("你选择的牌不符合规则")
			end
		else
			local lastOutCardInfo = m_cardPlayLogic.cardLogicContainer:GetLastOutCardInfo()
			if lastOutCardInfo then
				local srcCardIds = lastOutCardInfo.pai
				local srcCards = {}
--				local printStr = "last player out cardIds "
				for k,v in pairs(srcCardIds) do
					cardItem = CCard.New()
					cardItem:Init(v,k)
					table.insert(srcCards,cardItem)
--					printStr = printStr .. " " .. v
				end
				--print(printStr)
				local canOut, cardType = m_cardPlayLogic.cardLogicContainer:CompareCard(srcCards,chooseCards,handCardCount)
				if canOut then
				 	m_cardPlayLogic:SendOutCard(chooseCardIds,curOutInfo.token)

					--先出牌
					_s.PlayOutCardActionPre(false, chooseCards, cardType)
				else
					WindowUtil.LuaShowTips("你选择的牌不符合规则")
				end
			else
				WindowUtil.LuaShowTips("没有上家出牌")
			end
		end
	end
end

--提前出牌
function play_ui_window.PlayOutCardActionPre(isSkip,chooseCards, chooseCardsType)
	-- 出牌后重置AI提示结果
	m_cardPlayLogic.cardLogicContainer:ResetAIResult()
	-- 清理按钮和倒计时
	LuaEvent.AddEventNow(EEventType.SelfOutCard,false,nil)
	LuaEvent.AddEventNow(EEventType.RefreshPlayerStatusTimer,SeatPosEnum.South,PlayerStateEnum.Idle)

	if isSkip then
		m_UIHandCardCtrl:DownUpCards()
		-- 移除牌桌牌
		m_UIOutCardShowCtrl:ClearCards(SeatPosEnum.South) --考虑到重连后服务端下发的一轮出牌信息中，玩家会参与多次出牌
		-- 显示不出
		m_UIPlayerHeadCtrl:ShowNotOutTips(SeatPosEnum.South,true)
		_s.OutCardHideBombTip(SeatPosEnum.South)
	else
		--先清理提示
		m_UIPlayerHeadCtrl:ShowNotOutTips(SeatPosEnum.South,false)--考虑到重连后服务端下发的一轮出牌信息中，玩家会参与多次出牌
		--显示桌牌
		if #chooseCards > 0 then
			m_UIHandCardCtrl:OutCards()
			m_UIOutCardShowCtrl:PlayOutCardsShow(SeatPosEnum.South,chooseCards)
			_s.SetBombTipShowByCardEnum(chooseCardsType, SeatPosEnum.South)
		end
		AudioManager.PlayCommonSound(UIAudioEnum.chuPai)
		AudioManager.WSK_PlayBaoPai_2(chooseCards, chooseCardsType, DataManager.GetUserID())
		AnimManager.PlayCardClassAnimByCardType(chooseCardsType)
	end
end

function play_ui_window.HandleClickTipsCard()
	local lastOutCardInfo = m_cardPlayLogic.cardLogicContainer:GetLastOutCardInfo()
	if lastOutCardInfo then
		local srcCardIds = lastOutCardInfo.pai
		local srcCards = {}
		for k,v in pairs(srcCardIds) do
			cardItem = CCard.New()
			cardItem:Init(v,k)
			table.insert(srcCards,cardItem)
		end

		local nextCards = m_cardPlayLogic.cardLogicContainer:FindIntroduceCards(m_UIHandCardCtrl.handCards,srcCards)

		if nextCards and #nextCards > 0 then
			m_UIHandCardCtrl:TipCardsUp(nextCards)
		else
			--没有提示可出的牌
			WindowUtil.LuaShowTips("未能找到合适的推荐牌型")
		end
	end
end

--不出
function play_ui_window.HandleClickNoOutCard()
	-- 出牌后重置AI提示结果
	m_cardPlayLogic.cardLogicContainer:ResetAIResult()
	
	local curOutInfo = m_cardPlayLogic.roomObj.selfPlayer.outInfo
	m_cardPlayLogic:SendNoOutCard(curOutInfo.token)

	play_ui_window.PlayOutCardActionPre(true)
end

function play_ui_window.HandleShowFriendCard()
	local chooseCards = m_UIHandCardCtrl:GetUpCards()
	local cardNum = #chooseCards
	if cardNum > 1 then
		WindowUtil.LuaShowTips("你只能选择一张牌亮牌")
	elseif cardNum == 1 then
		m_cardPlayLogic:SendFindFriendCard(chooseCards[1].ID)
	else
		WindowUtil.LuaShowTips("你必须选择一张牌亮牌")
	end
end

function play_ui_window.HandleClickQuit()
	if m_cardPlayLogic and m_cardPlayLogic.roomObj then
		local curStateType = m_cardPlayLogic.roomObj.roomStateMgr:GetCurStateType()
		local contentStr
		local okFunc
		local noFunc
		local secContentStr
		local isFirstRound = m_cardPlayLogic.roomObj.roomInfo.currentGamepos == 1

		if curStateType == RoomStateEnum.Idle and isFirstRound then
			if m_cardPlayLogic.roomObj:CheckSelfIsRoomOwner() then
				contentStr = "确定解散房间 ？"
				secContentStr = "开局前解散不消耗房卡"
			else
				contentStr = "确定离开房间 ？"
			end
			okFunc = function ()
				m_cardPlayLogic:SendQuitRoom()
			end
		else
			contentStr = "确定发起解散投票 ？"
			okFunc = function ()
				m_cardPlayLogic:SendAskDismiss()
			end
		end
		WindowUtil.ShowErrorWindow(2,contentStr,nil, okFunc,noFunc,secContentStr)
	end
end

function play_ui_window.HandleClickHelp()
	--获取游戏类型，然后填state
	WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, PlayGameSys.GetNowPlayId(), "help_ui_window", false , nil)
end

function play_ui_window.HandleClickSet()
	WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "setting_ui_window", false , nil)
end

function play_ui_window:OnCopyBtnClick()
	if m_cardPlayLogic then
		local roomStr = m_cardPlayLogic:GetRoomBaseInfoCopyStr()
		if roomStr == "" then
			WindowUtil.LuaShowTips("请重新复制")
		else
			WrapSys.PlatInterface_CopyStr(roomStr)
			WindowUtil.LuaShowTips("房间信息已复制")
		end
	else
		WindowUtil.LuaShowTips("请重新复制的!")
	end
	
end
------------------------------button click end -------------------------------------------------

function play_ui_window.SetBombTipShowByRsp(rsp, seatPos)
	if rsp then
		local bombNum = _s.GetBombNumByRsp(rsp)
		if bombNum >= 4 then
			m_UIOutCardShowCtrl:ShowBombTip(seatPos, bombNum)
		end
	else
		m_UIOutCardShowCtrl:HideBombTip(seatPos)
	end
end

function play_ui_window.SetBombTipShowByCardEnum(cardType, seatPos)
	if cardType then
		local bombNum = _s.GetBombNumByCardType(cardType)
		if bombNum >= 4 then
			m_UIOutCardShowCtrl:ShowBombTip(seatPos, bombNum)
		end
	else
		m_UIOutCardShowCtrl:HideBombTip(seatPos)
	end
end

function play_ui_window.OutCardHideBombTip(seatPos)
	m_UIOutCardShowCtrl:HideBombTip(seatPos)
end

function play_ui_window.GetBombNumByRsp(rsp)
	if rsp.paiClass == 10 or rsp.paiClass == 26 then
		return 4
	elseif rsp.paiClass == 11 or rsp.paiClass == 16 then
		return 5
	elseif rsp.paiClass == 12 or rsp.paiClass == 17 then
		return 6
	elseif rsp.paiClass == 13 then
		return 7
	elseif rsp.paiClass == 14 then
		return 8
	else
		return 0
	end
end

function play_ui_window.GetBombNumByCardType(cardType)
	if cardType == CardTypeEnum.CT_BOMB_FOUR then
		return 4
	elseif cardType == CardTypeEnum.CT_BOMB_FIVE then
		return 5
	elseif cardType == CardTypeEnum.CT_BOMB_SIX then
		return 6
	elseif cardType == CardTypeEnum.CT_BOMB_SEVEN then
		return 7
	elseif cardType == CardTypeEnum.CT_BOMB_EIGTH then
		return 8
	else
		return 0
	end
end

function play_ui_window.UnRegister()
	m_luaWindowRoot = nil
end

function play_ui_window.OnDestroy()
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
end
