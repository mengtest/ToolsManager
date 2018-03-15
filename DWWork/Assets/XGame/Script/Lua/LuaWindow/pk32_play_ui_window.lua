--------------------------------------------------------------------------------
--   File      : pk32_play_ui_window.lua
--   author    : zhisong
--   function   : 32张打牌主UI
--   date      : 2018年1月16日 11:51:19
--   copyright : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------
require "Logic.UICtrlLogic.UIPlayerHeadCtrl"
require "Logic.UICtrlLogic.UICommonOtherInfoCtrl"
require "Logic.UICtrlLogic.UIPKLimitCardShowCtrl"
require "Logic.UICtrlLogic.UIBetCtrl"
require "Logic.UICtrlLogic.UIBetShowCtrl"
require "Logic.UICtrlLogic.PK32UIKaiPaiCtrl"
require "Logic.UICtrlLogic.UICardShuffleCtrl"
require "Logic.UICtrlLogic.UICountDownPart"
require "Logic.UICtrlLogic.UICardAnimationCtrl"
require "Logic.UICtrlLogic.UICuoPaiCtrl"


require "Logic.UICtrlLogic.UISingleSettlementCtrl"


local m_luaWindowRoot
local m_state

-- --玩家自己手牌根节点
local m_handCardRootTrans
-- --出牌展示根节点
-- local m_outCardsShowRootTrans

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
--剩余牌墙牌数量
local m_labelRemainCardNumTrans
-- 翻牌所挂的节点
local m_fanpaiRootTrans

-- @mid 小局结算
local m_singleSettlementCtrl --逻辑控制器

--押注界面
local m_UIBetCtrl
--牌显示控制组件
local m_UIPKLimitCardShowCtrl
--押注积分显示界面
local m_UIBetShowCtrl
-- 开牌按钮控制
local m_UIKaiPaiCtrl
-- 洗牌控件
local m_ShuffleCtrl
--搓牌组建
local m_UICuoPaiCtrl

--玩家头部显示组件
local m_UIPlayerHeadCtrl
--动画播放组件
local m_UICardAnimationCtrl
-- 其他信息组件
local m_UICommonOtherInfoCtrl


--游戏逻辑
local m_cardPlayLogic
--提示结果是否准备好
local m_tipResultReady = false
--是否展示了房间设置界面
local  m_isShowRoomSet = nil

--房间是否打开
local m_open = false

pk32_play_ui_window = {

}

local _s = pk32_play_ui_window

function pk32_play_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function pk32_play_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	m_open = open
	if open then
		m_state = state
		_s.InitWindowDetail()
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, PlayGameSys.GetNowPlayId(), "chat_ui_window", false , nil)
	end
end

function pk32_play_ui_window.CreateWindow()
	m_dismissRootTrans = m_luaWindowRoot:GetTrans("dismiss_root")
	m_handCardRootTrans = m_luaWindowRoot:GetTrans("out_card_pos")
	m_playerHeadRootTrans = m_luaWindowRoot:GetTrans("player_icon_root")
	m_prepareRootTrans = m_luaWindowRoot:GetTrans("prepare_root")
	m_preparedRootTrans = m_luaWindowRoot:GetTrans("prepared_root")
	m_preparePlayBtnTrans = m_luaWindowRoot:GetTrans("prepare_btn")
	m_prepareInviteBtnTrans = m_luaWindowRoot:GetTrans("btn_prepare_invite")
	m_unprepareInviteBtnTrans = m_luaWindowRoot:GetTrans("btn_canelprepare_invite")
	m_animationRootTrans = m_luaWindowRoot:GetTrans("animation_root")

	m_roundRootTrans = m_luaWindowRoot:GetTrans("round_root")
	m_topLeftRoomSetRootTrans = m_luaWindowRoot:GetTrans("room_set_root")
	m_bottomChatRootTrans = m_luaWindowRoot:GetTrans("chat_root")
	m_labelRemainCardNumTrans = m_luaWindowRoot:GetTrans("label_remain_cardNum")
	m_fanpaiRootTrans = m_luaWindowRoot:GetTrans("fanpai_root")

	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("chat_root"),false)
	m_luaWindowRoot:SetActive(m_topLeftRoomSetRootTrans,false)
	m_luaWindowRoot:SetActive(m_labelRemainCardNumTrans.parent,false)
	m_luaWindowRoot:SetActive(m_bottomChatRootTrans,true)

	-- 初始化邀请按钮是否变灰
	_s.RefreshRoomFull(nil, false)

	_s.RegisterEventHandle()

	m_UIPlayerHeadCtrl = UIPlayerHeadCtrl.New()
	m_UIPlayerHeadCtrl:Init(m_playerHeadRootTrans,m_luaWindowRoot)

	local m_btnRoot = m_luaWindowRoot:GetTrans("op_btn_root")
	m_UIBetCtrl = UIBetCtrl.New(m_luaWindowRoot:GetTrans(m_btnRoot, "1"),m_luaWindowRoot,m_luaWindowRoot:GetTrans(m_btnRoot, "count_down_root"),PlayGameSys.GetNowPlayId())
	m_UIPKLimitCardShowCtrl = UIPKLimitCardShowCtrl.New(m_handCardRootTrans,m_luaWindowRoot,PlayGameSys.GetNowPlayId())
	m_UIBetShowCtrl = UIBetShowCtrl.New(m_playerHeadRootTrans, m_luaWindowRoot)

	m_UIKaiPaiCtrl = PK32UIKaiPaiCtrl.New(m_luaWindowRoot:GetTrans("kaipai_root"), m_luaWindowRoot)

	m_UICardAnimationCtrl = UICardAnimationCtrl.New()
	m_UICardAnimationCtrl:Init(m_animationRootTrans,m_luaWindowRoot)

	m_UICommonOtherInfoCtrl = UICommonOtherInfoCtrl.New()
	m_UICommonOtherInfoCtrl:Init(m_topLeftRoomSetRootTrans,m_luaWindowRoot)

	--@mid
	m_singleSettlementCtrl = UISingleSettlementCtrl.New()
	m_singleSettlementCtrl:Init(m_luaWindowRoot)

	local btn_speak = m_luaWindowRoot:GetTrans("btn_speak").gameObject
	GetOrAddLuaComponent(btn_speak, "LuaWindow.Chat.ChatVoiceBtn", true)

	m_ShuffleCtrl = UICardShuffleCtrl.New(m_luaWindowRoot:GetTrans("ShuffleRoot"), m_luaWindowRoot, PlayGameSys.GetNowPlayId())
	--搓牌
	m_UICuoPaiCtrl = UICuoPaiCtrl.New(m_luaWindowRoot:GetTrans("CuoPaiRoot"), m_luaWindowRoot)
end

-- function pk32_play_ui_window.SetWaterMark()
-- 	local waterConfig = 
-- 	{
-- 		[Common_PlayID.ThirtyTwo] = "pk32_waterMark",
-- 	}
-- 	local name = waterConfig[PlayGameSys.GetNowPlayId()]
-- 	if nil == name then
-- 		DwDebug.LogError("no found water mark", PlayGameSys.GetNowPlayId())
-- 		return
-- 	end

-- 	m_luaWindowRoot:LoadImag(m_luaWindowRoot:GetTrans("bg_water"), "", name, false, RessStorgeType.RST_Never)
-- end

function pk32_play_ui_window.InitWindowDetail()
	-- m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans("bg_windows"), "bg_window_" .. LogicUtil.GetMJBgType(), true)
	m_cardPlayLogic = PlayGameSys.GetPlayLogic()
	--_s.SetWaterMark()

	--隐藏玩家自己的操作节点
	m_luaWindowRoot:SetActive(m_dismissRootTrans,false)
	m_luaWindowRoot:SetActive(m_prepareRootTrans,false)
	m_luaWindowRoot:SetActive(m_preparedRootTrans,false)
	m_luaWindowRoot:SetActive(m_preparePlayBtnTrans,false)
	m_luaWindowRoot:SetActive(m_labelRemainCardNumTrans.parent,false)
end

function pk32_play_ui_window.ChangeWindowShowState(isPlayNow)
	m_luaWindowRoot:SetActive(m_prepareRootTrans,not isPlayNow)
	m_luaWindowRoot:SetActive(m_preparedRootTrans,isPlayNow)

	if isPlayNow then
		local labelRoomIdTrans = m_luaWindowRoot:GetTrans(m_preparedRootTrans,"label_roomID")
		m_luaWindowRoot:SetLabel(labelRoomIdTrans,"房间号 "..m_cardPlayLogic.roomObj.roomInfo.roomId)
		-- 刷新wifi 电量 时间
		LuaEvent.AddEventNow(EEventType.RefreshOtherInfo)

		LuaEvent.AddEvent(EEventType.PlayerHeadPosChange, m_UIPlayerHeadCtrl:GetHeadPos(false))
	else-- 邀请准备阶段
		local labelQuitTrans = m_luaWindowRoot:GetTrans(m_prepareRootTrans,"label_quit")
		if m_cardPlayLogic.roomObj:CheckSelfIsRoomOwner() then
			m_luaWindowRoot:SetSprite(labelQuitTrans,"gameui_disbandroom")
		else
			m_luaWindowRoot:SetSprite(labelQuitTrans,"gameui_text_leveroom")
		end

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
			m_luaWindowRoot:SetLabel(labelRoomIdTrans,"房间号 ".. roomInfo.roomId)
		end

		LuaEvent.AddEvent(EEventType.PlayerHeadPosChange, m_UIPlayerHeadCtrl:GetHeadPos(true))
	end
end

----------------------------Event  start----------------------------------------


function pk32_play_ui_window.RegisterEventHandle()
	LuaEvent.AddHandle(EEventType.RefreshPlayerHead,_s.RefreshPlayerHead,nil)
	LuaEvent.AddHandle(EEventType.RefreshPlayWindowStatus,_s.PlaySceneInit,nil)
	LuaEvent.AddHandle(EEventType.PlaySceneInit,_s.PlaySceneInit,nil)
	LuaEvent.AddHandle(EEventType.RefreshRoomRoundNum,_s.RefreshRoomRoundNum,nil)
	LuaEvent.AddHandle(EEventType.PlayNotPrepare,_s.PlayNotPrepare,nil)
	-- LuaEvent.AddHandle(EEventType.ChangePlayCanvas,_s.ChangePlayCanvas,nil)
	LuaEvent.AddHandle(EEventType.RefreshRoomFull, _s.RefreshRoomFull)
	LuaEvent.AddHandle(EEventType.ShowRemainCardNum,_s.ShowRemainCardNum)
	LuaEvent.AddHandle(EEventType.PK32FanPai,_s.FanPai)
	LuaEvent.AddHandle(EEventType.PK32HideFanPai,_s.HideFanPai)
	LuaEvent.AddHandle(EEventType.PK32ShowCountDown,_s.CreateCountDown)
	LuaEvent.AddHandle(EEventType.PK32HideCountDown,_s.HideCountDown)
end

function pk32_play_ui_window.UnRegisterEventHandle()
	LuaEvent.RemoveHandle(EEventType.RefreshPlayerHead,_s.RefreshPlayerHead,nil)
	LuaEvent.RemoveHandle(EEventType.RefreshPlayWindowStatus,_s.PlaySceneInit,nil)
	LuaEvent.RemoveHandle(EEventType.PlaySceneInit,_s.PlaySceneInit,nil)
	LuaEvent.RemoveHandle(EEventType.RefreshRoomRoundNum,_s.RefreshRoomRoundNum,nil)
	LuaEvent.RemoveHandle(EEventType.PlayNotPrepare,_s.PlayNotPrepare,nil)
	-- LuaEvent.RemoveHandle(EEventType.ChangePlayCanvas,_s.ChangePlayCanvas,nil)
	LuaEvent.RemoveHandle(EEventType.RefreshRoomFull, _s.RefreshRoomFull)
	LuaEvent.RemoveHandle(EEventType.ShowRemainCardNum,_s.ShowRemainCardNum)
	LuaEvent.RemoveHandle(EEventType.PK32FanPai,_s.FanPai)
	LuaEvent.RemoveHandle(EEventType.PK32HideFanPai,_s.HideFanPai)
	LuaEvent.RemoveHandle(EEventType.PK32ShowCountDown,_s.CreateCountDown)
	LuaEvent.RemoveHandle(EEventType.PK32HideCountDown,_s.HideCountDown)
end

function pk32_play_ui_window.CreateCountDown(event_id, remain_time)
	_s:CleanCountDown()
	
	if remain_time <= 0 then
		return
	end

	_s.countDown = UICountDownPart.New(m_luaWindowRoot, m_luaWindowRoot:GetTrans("count_down_root"), remain_time, function()
		_s.countDown = nil
    end)
end

function pk32_play_ui_window.HideCountDown(event_id, p1, p2)
	_s:CleanCountDown()
end

function pk32_play_ui_window.CleanCountDown()
	if _s.countDown ~= nil then
		_s.countDown:Destroy(false)
		_s.countDown = nil
	end
end

function pk32_play_ui_window.HideFanPai(event_id, p1, p2)
	if m_fanpaiRootTrans.childCount > 0 then
		m_luaWindowRoot:SetActiveChildren(m_fanpaiRootTrans, false)
	end	
end

function pk32_play_ui_window.FanPai(event_id,p1,p2)
	LuaEvent.AddEventNow(EEventType.ShowCardFriendCardAction,true)
	TimerTaskSys.AddTimerEventByLeftTime(function()
		if m_open then
			_s.DealyFanPai(event_id,p1,p2)
			LuaEvent.AddEventNow(EEventType.ShowCardFriendCardAction,false)
		end
	end,1)
end

function pk32_play_ui_window.DealyFanPai(event_id, p1, p2)
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
	cardItem:Init(p1,0)
	LogicUtil.InitCardItem(m_luaWindowRoot, fanpai_card,cardItem,1)
	_s.PlayFanpaiSound(cardItem.logicValue)
end

function pk32_play_ui_window.PlayFanpaiSound(dianshu)
	local play_logic = PlayGameSys.GetPlayLogic()
	if play_logic.roomObj and play_logic.roomObj.playerMgr and play_logic.bankerSeatId then
		local banker = play_logic.roomObj.playerMgr:GetPlayerBySeatID(play_logic.bankerSeatId)
		if banker then
			if dianshu >= 14 and dianshu <= 15 then
				dianshu = dianshu - 13
			elseif dianshu == 16 then
				dianshu = 3
			elseif dianshu == 17 then
				dianshu = 6
			end
			AudioManager.ThirtyTwo_PlayBaoDian(banker.seatInfo.sex, dianshu)
		end
	end
end

function pk32_play_ui_window.RefreshPlayerHead(eventId,p1,p2)
	local seatPos = p1
	local seatInfo = p2
	m_UIPlayerHeadCtrl:RefreshPlayerHead(seatPos,seatInfo)
end

function pk32_play_ui_window.PlaySceneInit(eventId,p1,p2)
	local isPlayNow = p1
	--开启相关节点显示
	m_luaWindowRoot:SetActive(m_topLeftRoomSetRootTrans,isPlayNow)
	_s.ChangeWindowShowState(isPlayNow)
	if isPlayNow then
		_s.RefreshRoomSetRoot(nil, false)
	end
end

function pk32_play_ui_window.RefreshPlayWindowStatus(eventId,p1,p2)
	local isPlayNow = p1
	-- 切换准备状态与游戏状态
	_s.ChangeWindowShowState(isPlayNow)
end

function pk32_play_ui_window.RefreshRoomSetRoot(eventId, isDown)
	if m_isShowRoomSet == isDown then
		return
	end
	m_isShowRoomSet = isDown
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_topLeftRoomSetRootTrans,"set_root"),isDown)
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_topLeftRoomSetRootTrans,"btn_set_down"),not isDown)
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_topLeftRoomSetRootTrans,"btn_set_up"),isDown)
end


function pk32_play_ui_window.AskDismissRoom(eventId,p1,p2)
	local isOn = p1
	m_luaWindowRoot:SetActive(m_dismissRootTrans,isOn)
end


function pk32_play_ui_window.PlayNotPrepare(eventId,p1,p2)
	local seatPos = p1
	local isOn = p2
	if seatPos == SeatPosEnum.South then
		local roomInfo = m_cardPlayLogic.roomObj.roomInfo
		if roomInfo then
			if 1 == roomInfo.currentGamepos then
				m_luaWindowRoot:SetActive(m_prepareInviteBtnTrans, isOn)
				m_luaWindowRoot:SetActive(m_unprepareInviteBtnTrans, not isOn)
			else
				m_luaWindowRoot:SetActive(m_preparePlayBtnTrans, isOn)
			end
		end
	end
end

--刷新房间轮数
function pk32_play_ui_window.RefreshRoomRoundNum(eventId,p1,p2)
	local curRound = p1
	local totalRound = p2
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_roundRootTrans,"label_cur_round"),curRound)
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_roundRootTrans,"label_total_round"),"/"..totalRound)
end


--更换背景
-- function pk32_play_ui_window.ChangePlayCanvas(eventId, p1, p2)
-- 	local index = p1

-- 	if index then
-- 		-- m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans("bg_windows"), "bg_window_" .. index, true)
-- 	end
-- end

-- 显示是否满员
function pk32_play_ui_window.RefreshRoomFull(eventId, isFull, p2)
	m_luaWindowRoot:SetGray(m_luaWindowRoot:GetTrans(m_prepareRootTrans,"btn_invite"), isFull, true)
end

--显示剩余牌数
function pk32_play_ui_window.ShowRemainCardNum(eventId,p1,p2)
	local isShow,cardNum = p1,p2
	m_luaWindowRoot:SetActive(m_labelRemainCardNumTrans.parent,isShow)
	if isShow then
		m_luaWindowRoot:SetLabel(m_labelRemainCardNumTrans, tostring(cardNum), false)
	end

end



-----------------------------Event End -------------------------------------------------


--------------------------button click -------------------------------------------------


function pk32_play_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name

	--大结算的状态很尴尬 断网了 又停留在界面上 很多功能点击无效
	local is_dissmissed = false
	if PlayGameSys.GetPlayLogic() and PlayGameSys.GetPlayLogic().roomObj and PlayGameSys.GetPlayLogic().roomObj:GetBigResult() then
		is_dissmissed = true
	end
	if is_dissmissed then
		if click_name ~= "singleSettlement_btn_totalSettlement" 
			and click_name ~= "btn_set_down" and click_name ~= "btn_set_up"
			and click_name ~= "btn_room_bg"
			and click_name ~= "btn_help" and click_name ~= "btn_set" then
			WindowRoot.ShowTips("当前局已结束,请点击总结算按钮查看详情")
			return
		end
	end

	_s.RefreshRoomSetRoot(nil, false)
	if click_name == "btn_prepare" or click_name == "btn_prepare_invite" or click_name == "prepare_btn" then
		WrapSys.AudioSys_PlayEffect("Common/UI/UIclick_1")
		m_cardPlayLogic:SendPrepare(0)
	elseif click_name == "btn_canelprepare_invite" then
		WrapSys.AudioSys_PlayEffect("Common/UI/UIclick_1")
		m_cardPlayLogic:SendPrepare(1)
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
	elseif click_name == "btn_speak" then

	elseif click_name == "btn_quit" then
		_s.HandleClickQuit()
	elseif click_name == "btn_set" then
		_s.HandleClickSet()
	elseif click_name == "btn_help" then
		_s.HandleClickHelp()
	elseif click_name == "btn_detail" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "roomDetail_ui_window", false , nil)
	elseif click_name == "btn_invite" then --邀请
		HallSys.ShareRoom(m_cardPlayLogic.roomObj.roomInfo.roomId)
	elseif click_name == "CopyBtn" then
		_s.OnCopyBtnClick()
	elseif string.find(click_name, "headCollider_") then
		m_UIPlayerHeadCtrl:HandleWidgetClick(gb)
	elseif string.find(click_name, "mjOp_") then
		m_UIOpCtrl:DispatchEvent(gb)
	elseif string.find(click_name, "singleSettlement_") then
		m_singleSettlementCtrl:DispatchEvent(gb)
	elseif string.find(click_name, "btn_bet_") then
		m_UIBetCtrl:ClickBetBtn(gb)
	else
		m_UIKaiPaiCtrl:HandleWidgetClick(gb)
	end
end

function pk32_play_ui_window.HandleWidgetPressed(gb)
	m_UICuoPaiCtrl:HandleWidgetPressed(gb,true)
end

function pk32_play_ui_window.HandleWidgetRelease(gb)
	m_UICuoPaiCtrl:HandleWidgetPressed(gb,false)
end

function pk32_play_ui_window.Update()
	m_UICuoPaiCtrl:Update()
end

function pk32_play_ui_window.HandleClickQuit()
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
		WindowUtil.ShowErrorWindow(2, contentStr, function ( ) end, okFunc, noFunc, secContentStr)
	end
end

function pk32_play_ui_window.HandleClickHelp()
	--获取游戏类型，然后填state
	WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, PlayGameSys.GetNowPlayId(), "help_ui_window", false , nil)
end

function pk32_play_ui_window.HandleClickSet()
	WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 3, "setting_ui_window", false , nil)
end

function pk32_play_ui_window.OnCopyBtnClick()
	if m_cardPlayLogic then
		local roomStr = m_cardPlayLogic:GetRoomBaseInfoCopyStr()
		if roomStr == "" then
			WindowUtil.LuaShowTips("请重新复制")
		else
			WrapSys.PlatInterface_CopyStr(roomStr)
			WindowUtil.LuaShowTips("房间信息已复制")
		end
	else
		WindowUtil.LuaShowTips("请重新复制!")
	end
end
------------------------------button click end -------------------------------------------------

function pk32_play_ui_window.UnRegister()
	m_luaWindowRoot = nil
end

function pk32_play_ui_window.OnDestroy()
	_s.UnRegisterEventHandle()
	if m_UIPlayerHeadCtrl then
		m_UIPlayerHeadCtrl:Destroy()
		m_UIPlayerHeadCtrl = nil
	end

	if m_UICardAnimationCtrl then
		m_UICardAnimationCtrl:Destroy()
		m_UICardAnimationCtrl = nil
	end

	if m_UICommonOtherInfoCtrl then
		m_UICommonOtherInfoCtrl:Destroy()
		m_UICommonOtherInfoCtrl = nil
	end

	-- @mid
	if m_singleSettlementCtrl then
		m_singleSettlementCtrl:Destroy()
		m_singleSettlementCtrl = nil
	end

	if m_UIPKLimitCardShowCtrl then
		m_UIPKLimitCardShowCtrl:Destroy()
		m_UIPKLimitCardShowCtrl = nil
	end

	if m_UIBetCtrl then
		m_UIBetCtrl:Destroy()
		m_UIBetCtrl = nil
	end
	if m_UIBetShowCtrl then
		m_UIBetShowCtrl:Destroy()
		m_UIBetShowCtrl = nil
	end
	if m_UIKaiPaiCtrl then
		m_UIKaiPaiCtrl:Destroy()
		m_UIKaiPaiCtrl = nil
	end

	if m_ShuffleCtrl then
		m_ShuffleCtrl:Destroy()
		m_ShuffleCtrl = nil
	end

	if m_UICuoPaiCtrl then
		m_UICuoPaiCtrl:Destroy()
		m_UICuoPaiCtrl = nil
	end
end
