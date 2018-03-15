--------------------------------------------------------------------------------
-- 	 File      : mj_mj_play_ui_window.lua
--   author    : guoliang
--   function   : 麻将打牌主UI
--   date      : 2017-9-28
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.UICtrlLogic.UIPlayerHeadCtrl"
require "Logic.UICtrlLogic.UIMJOutCardShowCtrl"
require "Logic.UICtrlLogic.UIMJHandCardCtrl"
require "Logic.UICtrlLogic.UIMJOpCtrl"
require "Logic.UICtrlLogic.UIMJCardAnimationCtrl"
require "Logic.UICtrlLogic.UIMJDeskCenterCtrl"
require "Logic.UICtrlLogic.UIMJScoreCtrl"
require "Logic.UICtrlLogic.UICommonOtherInfoCtrl"
require "Logic.UICtrlLogic.MJAction.MJGameActionManager"

local m_luaWindowRoot
local m_state

--玩家自己手牌根节点
local m_handCardRootTrans
--出牌展示根节点
local m_outCardsShowRootTrans

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
-- 操作节点
local m_OpeRootTrans
-- 桌面中心节点
local m_deskCenterTrans
-- 飘分组件节点
local m_scoreRoot
--剩余牌墙牌数量
local m_labelRemainCardNumTrans
-- 流局烧庄节点
local m_gameEffectRoot

--手牌显示控制组件
local m_UIHandCardCtrl
--出牌显示控制
local m_UIOutCardShowCtrl
--玩家头部显示组件
local m_UIPlayerHeadCtrl
--动画播放组件
local m_UICardAnimationCtrl
-- 操作提醒组件
local m_UIOpCtrl
-- 桌面中心控制器组件
local m_UIMJDeskCenterCtrl
-- 飘分组件
local m_UIMJScoreCtrl
-- 其他信息组件
local m_UICommonOtherInfoCtrl

--游戏逻辑
local m_cardPlayLogic
--提示结果是否准备好
local m_tipResultReady = false
--是否展示了房间设置界面
local  m_isShowRoomSet = nil

--是否显示过房间详情
local m_showFirstDetail = false

mj_play_ui_window = {

}

local _s = mj_play_ui_window

function mj_play_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function mj_play_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		m_state = state
		_s.InitWindowDetail()
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, PlayGameSys.GetNowPlayId(), "chat_ui_window", false , nil)
	end
end

function mj_play_ui_window.CreateWindow()
	m_dismissRootTrans = m_luaWindowRoot:GetTrans("dismiss_root")
	m_handCardRootTrans = m_luaWindowRoot:GetTrans("hand_card_root")
	m_outCardsShowRootTrans = m_luaWindowRoot:GetTrans("out_card_show_root")
	m_otherOperateTipTrans = m_luaWindowRoot:GetTrans("label_other_operate_tips")
	m_playerHeadRootTrans = m_luaWindowRoot:GetTrans("player_icon_root")
	m_prepareRootTrans = m_luaWindowRoot:GetTrans("prepare_root")
	m_preparedRootTrans = m_luaWindowRoot:GetTrans("prepared_root")
	m_preparePlayBtnTrans = m_luaWindowRoot:GetTrans("btn_prepare_play")
	m_prepareInviteBtnTrans = m_luaWindowRoot:GetTrans("btn_prepare_invite")
	m_unprepareInviteBtnTrans = m_luaWindowRoot:GetTrans("btn_canelprepare_invite")
	m_animationRootTrans = m_luaWindowRoot:GetTrans("animation_root")

	m_roundRootTrans = m_luaWindowRoot:GetTrans("round_root")
	m_topLeftRoomSetRootTrans = m_luaWindowRoot:GetTrans("room_set_root")
	m_bottomChatRootTrans = m_luaWindowRoot:GetTrans("chat_root")
	m_OpeRootTrans = m_luaWindowRoot:GetTrans("ope_root")
	m_deskCenterTrans = m_luaWindowRoot:GetTrans("desk_center_root")
	m_scoreRoot = m_luaWindowRoot:GetTrans("score_root")
	m_gameEffectRoot = m_luaWindowRoot:GetTrans("game_effect_root")

	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_gameEffectRoot,"liuju"),false)
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_gameEffectRoot,"shaozhuang"),false)

	m_labelRemainCardNumTrans = m_luaWindowRoot:GetTrans("label_remain_cardNum")
	-- LogicUtil.ChangeSingleMJPaiBg(m_luaWindowRoot,LogicUtil.GetMJPaiType(), SeatPosEnum.North, m_luaWindowRoot:GetTrans(m_labelRemainCardNumTrans,"bg_remain_num"),1)

	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("chat_root"),false)
	m_luaWindowRoot:SetActive(m_topLeftRoomSetRootTrans,false)
	m_luaWindowRoot:SetActive(m_otherOperateTipTrans,false)
	m_luaWindowRoot:SetActive(m_labelRemainCardNumTrans,false)
	m_luaWindowRoot:SetActive(m_bottomChatRootTrans,true)

	-- 初始化邀请按钮是否变灰
	_s.RefreshRoomFull(nil, false)

	_s.RegisterEventHandle()

	m_UIPlayerHeadCtrl = UIPlayerHeadCtrl.New()
	m_UIPlayerHeadCtrl:Init(m_playerHeadRootTrans,m_luaWindowRoot)

	m_UIOutCardShowCtrl = UIMJOutCardShowCtrl.New()
	m_UIOutCardShowCtrl:Init(m_outCardsShowRootTrans,m_luaWindowRoot)

	m_UIHandCardCtrl = UIMJHandCardCtrl.New()
	m_UIHandCardCtrl:Init(m_handCardRootTrans,m_luaWindowRoot)

	m_UIOpCtrl = UIMJOpCtrl.New()
	m_UIOpCtrl:Init(m_OpeRootTrans,m_luaWindowRoot)

	m_UICardAnimationCtrl = UIMJCardAnimationCtrl.New()
	m_UICardAnimationCtrl:Init(m_animationRootTrans,m_luaWindowRoot)

	m_UIMJDeskCenterCtrl = UIMJDeskCenterCtrl.New()
	m_UIMJDeskCenterCtrl:Init(m_deskCenterTrans,m_luaWindowRoot)

	m_UIMJScoreCtrl = UIMJScoreCtrl.New()
	m_UIMJScoreCtrl:Init(m_scoreRoot,m_luaWindowRoot)

	m_UICommonOtherInfoCtrl = UICommonOtherInfoCtrl.New()
	m_UICommonOtherInfoCtrl:Init(m_topLeftRoomSetRootTrans,m_luaWindowRoot)
	MJGameActionManager.Init(m_luaWindowRoot)

	local btn_speak = m_luaWindowRoot:GetTrans("btn_speak").gameObject
	GetOrAddLuaComponent(btn_speak, "LuaWindow.Chat.ChatVoiceBtn", true)
end

function mj_play_ui_window.SetWaterMark()
	local waterConfig =
	{
		[Common_PlayID.chongRen_MJ] = "watermark_chongrenmajiang",
		[Common_PlayID.leAn_MJ] = "watermark_leanmajiang",
		[Common_PlayID.yiHuang_MJ] = "watermark_yihuangmajiang",
	}
	local name = waterConfig[PlayGameSys.GetNowPlayId()]
	if nil == name then
		DwDebug.LogError("DwDebug.LogError", PlayGameSys.GetNowPlayId())
		return
	end

	m_luaWindowRoot:LoadImag(m_luaWindowRoot:GetTrans("bg_water"), "", name, false, RessStorgeType.RST_Never)
end

function mj_play_ui_window.InitWindowDetail()
	m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans("bg_windows"), "bg_window_" .. LogicUtil.GetMJBgType(), true)
	m_cardPlayLogic = PlayGameSys.GetPlayLogic()
	_s.SetWaterMark()

	--隐藏玩家自己的操作节点
	m_luaWindowRoot:SetActive(m_dismissRootTrans,false)
	m_luaWindowRoot:SetActive(m_prepareRootTrans,false)
	m_luaWindowRoot:SetActive(m_preparedRootTrans,false)
	m_luaWindowRoot:SetActive(m_preparePlayBtnTrans,false)
	-- 隐藏流局烧庄节点
	m_luaWindowRoot:SetActive(m_gameEffectRoot,false)


end

function mj_play_ui_window.ChangeWindowShowState(isPlayNow)
	m_luaWindowRoot:SetActive(m_prepareRootTrans,not isPlayNow)
	m_luaWindowRoot:SetActive(m_preparedRootTrans,isPlayNow)
	m_luaWindowRoot:SetActive(m_otherOperateTipTrans,false)

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
			if roomInfo.currentGamepos == 0 then
				if selfPlayer and selfPlayer:IsState(PlayerStateEnum.Prepared) then
					m_luaWindowRoot:SetActive(m_prepareInviteBtnTrans, false)
					m_luaWindowRoot:SetActive(m_unprepareInviteBtnTrans, true)
				else
					m_luaWindowRoot:SetActive(m_prepareInviteBtnTrans, true)
					m_luaWindowRoot:SetActive(m_unprepareInviteBtnTrans, false)
				end
			else
				--显示这个界面的时候 先不出现准备按钮
				if selfPlayer and selfPlayer:IsState(PlayerStateEnum.Prepared) then
					m_luaWindowRoot:SetActive(m_preparePlayBtnTrans, false)
				else
				end
			end
			local labelRoomIdTrans = m_luaWindowRoot:GetTrans(m_prepareRootTrans,"label_room_id")
			m_luaWindowRoot:SetLabel(labelRoomIdTrans,"房间号 ".. roomInfo.roomId)
		end

		LuaEvent.AddEvent(EEventType.PlayerHeadPosChange, m_UIPlayerHeadCtrl:GetHeadPos(true))
	end
end

----------------------------Event  start----------------------------------------


function mj_play_ui_window.RegisterEventHandle()
	LuaEvent.AddHandle(EEventType.RefreshPlayerHead,_s.RefreshPlayerHead,nil)
	LuaEvent.AddHandle(EEventType.RefreshPlayWindowStatus,_s.PlaySceneInit,nil)
	LuaEvent.AddHandle(EEventType.PlaySceneInit,_s.PlaySceneInit,nil)
	LuaEvent.AddHandle(EEventType.RefreshRoomRoundNum,_s.RefreshRoomRoundNum,nil)
	LuaEvent.AddHandle(EEventType.PlayNotPrepare,_s.PlayNotPrepare,nil)
	LuaEvent.AddHandle(EEventType.ChangePlayCanvas,_s.ChangePlayCanvas,nil)
	LuaEvent.AddHandle(EEventType.RefreshRoomFull, _s.RefreshRoomFull)
	LuaEvent.AddHandle(EEventType.MJ_ShowRemainCardNum,_s.ShowRemainCardNum)
	LuaEvent.AddHandle(EEventType.MJ_LiuJuShaoZhuang,_s.onMJ_LiuJuShaoZhuang)
	LuaEvent.AddHandle(EEventType.RefreshRoomSetRoot,_s.RefreshRoomSetRoot)
end

function mj_play_ui_window.UnRegisterEventHandle()
	LuaEvent.RemoveHandle(EEventType.RefreshPlayerHead,_s.RefreshPlayerHead,nil)
	LuaEvent.RemoveHandle(EEventType.RefreshPlayWindowStatus,_s.PlaySceneInit,nil)
	LuaEvent.RemoveHandle(EEventType.PlaySceneInit,_s.PlaySceneInit,nil)
	LuaEvent.RemoveHandle(EEventType.RefreshRoomRoundNum,_s.RefreshRoomRoundNum,nil)
	LuaEvent.RemoveHandle(EEventType.PlayNotPrepare,_s.PlayNotPrepare,nil)
	LuaEvent.RemoveHandle(EEventType.ChangePlayCanvas,_s.ChangePlayCanvas,nil)
	LuaEvent.RemoveHandle(EEventType.RefreshRoomFull, _s.RefreshRoomFull)
	LuaEvent.RemoveHandle(EEventType.MJ_ShowRemainCardNum,_s.ShowRemainCardNum)
	LuaEvent.RemoveHandle(EEventType.MJ_LiuJuShaoZhuang,_s.onMJ_LiuJuShaoZhuang)
	LuaEvent.RemoveHandle(EEventType.RefreshRoomSetRoot,_s.RefreshRoomSetRoot)
end


function mj_play_ui_window.OtherPlayerOperateTipShow(eventId, p1, p2)
	local showText = p1
	if showText then
		m_luaWindowRoot:SetActive(m_otherOperateTipTrans,true)
		m_luaWindowRoot:SetLabel(m_otherOperateTipTrans,showText)
	else
		m_luaWindowRoot:SetActive(m_otherOperateTipTrans,false)
	end
end


function mj_play_ui_window.RefreshPlayerHead(eventId,p1,p2)
	local seatPos = p1
	local seatInfo = p2
	m_UIPlayerHeadCtrl:RefreshPlayerHead(seatPos,seatInfo)
end

function mj_play_ui_window.PlaySceneInit(eventId,p1,p2)
	local isPlayNow = p1
	-- 刷新头像位置
	-- m_UIPlayerHeadCtrl:RefreshPlayerHeadPos(SeatPosEnum.South)
	-- m_UIPlayerHeadCtrl:RefreshPlayerHeadPos(SeatPosEnum.East)
	-- m_UIPlayerHeadCtrl:RefreshPlayerHeadPos(SeatPosEnum.North)
	-- m_UIPlayerHeadCtrl:RefreshPlayerHeadPos(SeatPosEnum.West)
	--开启相关节点显示
	m_luaWindowRoot:SetActive(m_topLeftRoomSetRootTrans,isPlayNow)
	_s.ChangeWindowShowState(isPlayNow)
	if isPlayNow then
		_s.RefreshRoomSetRoot(nil, false)
	end
end

function mj_play_ui_window.RefreshPlayWindowStatus(eventId,p1,p2)
	local isPlayNow = p1
	-- 刷新头像位置
	-- m_UIPlayerHeadCtrl:RefreshPlayerHeadPos(SeatPosEnum.South)
	-- m_UIPlayerHeadCtrl:RefreshPlayerHeadPos(SeatPosEnum.East)
	-- m_UIPlayerHeadCtrl:RefreshPlayerHeadPos(SeatPosEnum.North)
	-- m_UIPlayerHeadCtrl:RefreshPlayerHeadPos(SeatPosEnum.West)
	-- 切换准备状态与游戏状态
	_s.ChangeWindowShowState(isPlayNow)
end

function mj_play_ui_window.RefreshRoomSetRoot(eventId, isDown)
	if m_isShowRoomSet == isDown then
		return
	end
	DwDebug.Log("mj_play_ui_window.RefreshRoomSetRoot:", isDown)
	m_isShowRoomSet = isDown
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_topLeftRoomSetRootTrans,"set_root"),isDown)
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_topLeftRoomSetRootTrans,"btn_set_down"),not isDown)
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_topLeftRoomSetRootTrans,"btn_set_up"),isDown)
end


function mj_play_ui_window.AskDismissRoom(eventId,p1,p2)
	local isOn = p1
	m_luaWindowRoot:SetActive(m_dismissRootTrans,isOn)
end


function mj_play_ui_window.PlayNotPrepare(eventId,p1,p2)
	local seatPos = p1
	local isOn = p2
	if seatPos == SeatPosEnum.South then
		local roomInfo = m_cardPlayLogic.roomObj.roomInfo
		if roomInfo then
			if 0 == roomInfo.currentGamepos then
				m_luaWindowRoot:SetActive(m_prepareInviteBtnTrans, isOn)
				m_luaWindowRoot:SetActive(m_unprepareInviteBtnTrans, not isOn)
			else
				--特殊要求 这个阶段 重连不能显示准备按钮
				-- local showRoundOverUI = WrapSys.EZFunWindowMgr_CheckWindowOpen(EZFunWindowEnum.luaWindow, "mj_round_over_ui_window")
				-- if showRoundOverUI then
				-- 	isOn = false
				-- end
				m_luaWindowRoot:SetActive(m_preparePlayBtnTrans, isOn)
			end
		end
	end
end

--检查是否是在首次进入房间状态
function mj_play_ui_window.CheckIsIdleAndFirst()
	--m_cardPlayLogic = PlayGameSys.GetPlayLogic()
	if m_cardPlayLogic and m_cardPlayLogic.roomObj and m_cardPlayLogic.roomObj.roomStateMgr then
		local curStateType = m_cardPlayLogic.roomObj.roomStateMgr:GetCurStateType()
		local isFirstRound = m_cardPlayLogic.roomObj.roomInfo.currentGamepos == 0
		return curStateType == RoomStateEnum.Idle and isFirstRound
	end
	return false
end

--刷新房间轮数
function mj_play_ui_window.RefreshRoomRoundNum(eventId,p1,p2)
	local curRound = p1
	local totalRound = p2
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_deskCenterTrans,"label_round"), "[b]局数:"..curRound.."/"..totalRound,false)
	
	--获取到信息 会走刷新轮数 才能显示界面
	_s.CheckNeedOpenRDWindow()
end

--更换背景
function mj_play_ui_window.ChangePlayCanvas(eventId, p1, p2)
	local index = p1

	if index then
		m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans("bg_windows"), "bg_window_" .. index, true)
	end
	-- LogicUtil.ChangeSingleMJPaiBg(m_luaWindowRoot,LogicUtil.GetMJPaiType(), SeatPosEnum.North, m_luaWindowRoot:GetTrans(m_labelRemainCardNumTrans,"bg_remain_num"),1)
end

-- 显示是否满员
function mj_play_ui_window.RefreshRoomFull(eventId, isFull, p2)
	m_luaWindowRoot:SetGray(m_luaWindowRoot:GetTrans(m_prepareRootTrans,"btn_invite"), isFull, true)
end

--显示剩余牌数
function mj_play_ui_window.ShowRemainCardNum(eventId,p1,p2)
	local isShow,cardNum = p1,p2
	m_luaWindowRoot:SetActive(m_labelRemainCardNumTrans,isShow)
	if isShow then
		m_luaWindowRoot:SetLabel(m_labelRemainCardNumTrans, string.format("[b]余 %d张[-]", cardNum), false)
	end

end

-- 控制流局烧庄显示状态
function mj_play_ui_window.onMJ_LiuJuShaoZhuang(eventId,p1,p2 )
	local data = p1
	local isLiuJu = data[1]
	local isShaoZhuang = data[2]
	m_luaWindowRoot:SetActive(m_gameEffectRoot,true)
	if isLiuJu then
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans( m_gameEffectRoot,"liuju"),isLiuJu)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans( m_gameEffectRoot,"shaozhuang"),false)
		-- WrapSys.AddTimerEVentByLeftTime(2,function ( )
			-- m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_gameEffectRoot,"shaozhuang"),isShaoZhuang)
		-- end)
	else
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_gameEffectRoot,"shaozhuang"),isShaoZhuang)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_gameEffectRoot,"liuju"),isLiuJu)
	end

end


-----------------------------Event End -------------------------------------------------


--------------------------button click -------------------------------------------------


function mj_play_ui_window.HandleWidgetClick(gb)
	-- 打开了统一关闭
	_s.RefreshRoomSetRoot(nil, false)

	local click_name = gb.name
	if click_name == "btn_prepare" or click_name == "btn_prepare_invite" or click_name == "btn_prepare_play" then
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
		_s.OpenRoundDetailWindow()
	elseif click_name == "btn_invite" then --邀请
		HallSys.ShareRoom(m_cardPlayLogic.roomObj.roomInfo.roomId)
	elseif click_name == "CopyBtn" then
		_s.OnCopyBtnClick()
	elseif string.find(click_name, "headCollider_") then
		m_UIPlayerHeadCtrl:HandleWidgetClick(gb)
	elseif string.find(click_name, "mjOp_") then
		m_UIOpCtrl:DispatchEvent(gb)
	else

	end
end

-- 记录是否第一次进入房间来判断是否要弹出房间信息
function mj_play_ui_window.CheckNeedOpenRDWindow()
	if m_cardPlayLogic and m_cardPlayLogic.roomObj and m_cardPlayLogic.roomObj.roomInfo then
		local roomInfo = m_cardPlayLogic.roomObj.roomInfo
		if roomInfo.status == RoomStateEnum.Idle
			and roomInfo.currentGamepos == 0
			and DataManager.GetUserID() ~= roomInfo.fangZhu 
			and not m_showFirstDetail then
			_s.OpenRoundDetailWindow()
			m_showFirstDetail = true
		end
	end
end

-- 打开对局流水界面
function mj_play_ui_window.OpenRoundDetailWindow()
	--if m_cardPlayLogic and m_cardPlayLogic:CheckRoomInfoLegal() then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "roomDetail_ui_window", false , nil)
	--end
end

function mj_play_ui_window.HandleClickQuit()
	if m_cardPlayLogic and m_cardPlayLogic.roomObj then
		local contentStr
		local okFunc
		local noFunc
		local secContentStr

		if _s.CheckIsIdleAndFirst() then
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
		WindowUtil.ShowErrorWindow(2,contentStr,function ( ) end,okFunc,noFunc,secContentStr)
	end
end

function mj_play_ui_window.HandleClickHelp()
	--获取游戏类型，然后填state
	WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, PlayGameSys.GetNowPlayId(), "help_ui_window", false , nil)
end

function mj_play_ui_window.HandleClickSet()
	WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 2, "setting_ui_window", false , nil)
end

function mj_play_ui_window.OnCopyBtnClick()
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

function mj_play_ui_window.UnRegister()
	m_luaWindowRoot = nil
end

function mj_play_ui_window.OnDestroy()
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
	if m_UIOpCtrl then
		m_UIOpCtrl:Destroy()
		m_UIOpCtrl = nil
	end
	if m_UICardAnimationCtrl then
		m_UICardAnimationCtrl:Destroy()
		m_UICardAnimationCtrl = nil
	end
	if m_UIMJDeskCenterCtrl then
		m_UIMJDeskCenterCtrl:Destroy()
		m_UIMJDeskCenterCtrl = nil
	end
	if m_UIMJScoreCtrl then
		m_UIMJScoreCtrl:Destroy()
		m_UIMJScoreCtrl = nil
	end
	if m_UICommonOtherInfoCtrl then
		m_UICommonOtherInfoCtrl:Destroy()
		m_UICommonOtherInfoCtrl = nil
	end
	MJGameActionManager.UnInit()

	m_showFirstDetail = false
end
