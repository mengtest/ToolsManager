--------------------------------------------------------------------------------
-- 	 File      : AnimManager.lua
--   author    : zhisong
--   function   : 动画管理器
--   date      : 2017年11月2日
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

AnimManager = {}
MJAnimEnum = 
{
	PENG = "peng",
	GANG = "gang",
	HU = "hu",
	ZIMO = "zimo",
	--WIND = "wind",
	WAITSELFTIP = "waitselftip",
	WAITOTHERTIP = "waitothertip",
}

function AnimManager.PlayCardClassAnimByCardType(cardType)
	if cardType == CardTypeEnum.CT_THREE_LINE_TAKE_TWO then
		-- 三带二 
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "threetwo")
	elseif cardType == CardTypeEnum.CT_SINGLE_LINE then
		-- 顺子
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "shunzi")
	elseif cardType == CardTypeEnum.CT_DOUBLE_LINE then
		-- 连对
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "liandui")
	elseif cardType == CardTypeEnum.CT_THREE_LINE then
		-- 飞机
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "airplane")
	elseif cardType == CardTypeEnum.CT_FIVE_TEN_K or
		   cardType == CardTypeEnum.CT_FIVE_TEN_K_BIG then
		--510K
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "510K")
	elseif cardType >= CardTypeEnum.CT_BOMB_FOUR 
		and cardType <= CardTypeEnum.CT_FOUR_KING then
		-- 炸弹
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "thebomb")
	elseif cardType == CardTypeEnum.CT_DOUBLE_SAME_HongXin_5 then
		-- 红心5
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "redheart")
	end
end

function AnimManager.PlayCardClassAnim(rsp, playID)
	if rsp.paiClass == 5 then
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "threetwo")
	elseif rsp.paiClass == 6 then
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "shunzi")
	elseif rsp.paiClass == 7 then
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "liandui")
	elseif rsp.paiClass == 8 then
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "airplane")
	elseif rsp.paiClass == 9 then
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "510K")
	elseif rsp.paiClass ~= 15 and rsp.paiClass >= 10 and rsp.paiClass <= 25 then
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "thebomb")
	end
	
	if playID == Common_PlayID.yiHuang_510K then
		-- 宜黄红心5
		if rsp.paiClass == 27 then
			LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "redheart")
		end
	end
end

function AnimManager.DDZPlayAnimByCardType(cardType)
	if cardType == NewCardTypeEnum.NCCT_ROCKET then
		-- 火箭
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "DDZKing")
	elseif cardType == NewCardTypeEnum.NCCT_SINGLE_LINE then
		-- 顺子
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "shunzi")
	elseif cardType == NewCardTypeEnum.NCCT_DOUBLE_LINE then
		-- 连对
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "liandui")
	elseif cardType == NewCardTypeEnum.NCCT_BOMB then
		-- 炸弹
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "thebomb")
	elseif cardType == NewCardTypeEnum.NCCT_THREE_LINE or
			cardType == NewCardTypeEnum.NCCT_THREE_LINE_ADD_SAME_ONE or
			cardType == NewCardTypeEnum.NCCT_THREE_LINE_ADD_SAME_DOUBLE then
		-- 飞机
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "airplane")
	end
end

function AnimManager.DDZPlayAnimByPaiClass(rsp, playID)
	if rsp.paiClass == 34 then
		-- 火箭
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "DDZKing")
	elseif rsp.paiClass == 6 then
		-- 顺子
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "shunzi")
	elseif rsp.paiClass == 7 then
		-- 连对
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "liandui")
	elseif rsp.paiClass == 10 then
		-- 炸弹
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "thebomb")
		elseif rsp.paiClass == 31 or rsp.paiClass == 32 or rsp.paiClass  == 33 then
		-- 飞机
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "airplane")
	end
end

--播放斗地主春天，反春天动画(rsp为下结算信息)
function AnimManager.DDZPlaySpringAnim(rsp)
	local overState = rsp.overStatus

	if overState == 4 then
		-- 春天
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "DDZSpring")
	elseif overState == 5 then
		-- 反春天
		LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "DDZFanSpring")
	end
end

function AnimManager.PlayMJCardAnim(enum, seatPos)
	-- if MJAnimEnum.GANG == enum then
	-- 	LuaEvent.AddEventNow(EEventType.MJPlayCardClassAnim, MJAnimEnum.WIND, "Center")
	-- end
	LuaEvent.AddEventNow(EEventType.MJPlayCardClassAnim, enum, seatPos)
end

function AnimManager.PlayChuPaiAnim(mjCard, seatPos)
	-- LuaEvent.AddEventNow(EEventType.MJPlayCardChuPaiAnim, mjCard, seatPos)
end

function AnimManager.PlayArrowAnim(show, pos)
	LuaEvent.AddEventNow(EEventType.MJPlayCardArrowAnim, show, pos)
end

function AnimManager.PlayPlayerHeadAnim(show, seatPos)
	LuaEvent.AddEventNow(EEventType.PlayPlayerHeadAinm, show, seatPos)
end

function AnimManager.PlayWaitTip(show)
	if show then
		local selfPlayer = PlayGameSys.GetPlayLogic().roomObj.playerMgr:GetPlayerByPlayerID(PlayGameSys.GetPlayLogic().roomObj:GetSouthUID())
		DwDebug.LogError("PlayerState", "Play Wait Tip ", tostring(show), selfPlayer.playStateMgr:GetType())
		if nil ~= selfPlayer and selfPlayer:IsState(PlayerStateEnum.Playing) then
			print("------------------ show self tips")
			LuaEvent.AddEventNow(EEventType.MJPlayWaitTip, MJAnimEnum.WAITSELFTIP, true)
		else
			print("------------------ show other tips")
			LuaEvent.AddEventNow(EEventType.MJPlayWaitTip, MJAnimEnum.WAITOTHERTIP, true)
		end
	else
		LuaEvent.AddEventNow(EEventType.MJPlayWaitTip, MJAnimEnum.WAITSELFTIP, false)
		LuaEvent.AddEventNow(EEventType.MJPlayWaitTip, MJAnimEnum.WAITOTHERTIP, false)
	end
end

function AnimManager.PlayShuangweiAnim()
	LuaEvent.AddEventNow(EEventType.PlayCardClassAnim, "shuangwei")
end