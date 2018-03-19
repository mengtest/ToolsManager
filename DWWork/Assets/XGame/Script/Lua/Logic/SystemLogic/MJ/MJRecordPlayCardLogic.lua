--------------------------------------------------------------------------------
-- 	 File      : MJRecordPlayCardLogic.lua
--   author    : guoliang
--   function   : 麻将游戏回放逻辑
--   date      : 2017-11-13
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.SystemLogic.BaseRecordPlayCardLogic"
require "Logic.MJCardLogic.CMJCard"
require "Logic.RoomLogic.CPlayer"
require "Logic.RoomLogic.Room.CMJRoom"
require	"LuaSys.AnimManager"

MJRecordPlayCardLogic = class("MJRecordPlayCardLogic", BaseRecordPlayCardLogic)
function MJRecordPlayCardLogic:ctor()
	self:BaseCtor()
end

function MJRecordPlayCardLogic:Init()
	self:BaseInit()
	self.playInterval = 0.25
	--初始化玩法所需PB
	-- ProtoManager.InitMJProto(Common_PlayID.chongRen_MJ)

	--房间管理
	self.roomObj = CMJRoom.New()
	self.roomObj:Init(self)

	self.isRegisterUpdate = false
end


function MJRecordPlayCardLogic:Destroy()
	self:CleanRoom()

	self:BaseDestroy()

	self.roomObj:Destroy()
	self.isRegisterUpdate = false
	self.roomObj = nil

end

function MJRecordPlayCardLogic:GetType()
	return PlayLogicTypeEnum.MJ_Record
end


function MJRecordPlayCardLogic:StartPlay(recordBody)
	BaseRecordPlayCardLogic.StartPlay(self, recordBody)
end


-------------------------------------------基础播放功能--------------------------------------------------------


-- 查找一局开始的消息编号
function MJRecordPlayCardLogic:FindRoundStartIndex(targetRound)
	for k,v in pairs(self.recordBody.cells) do
		if v.eventId == GAME_CMD.SC_ROOM_INFO then
			local eventId,roomInfo = self:DecordRecordItem(k)
			if roomInfo and roomInfo.currentGamepos == targetRound then
				return k
			end
		end
	end
end

-- 广播消息
function MJRecordPlayCardLogic:PublicEvent(eventId,rsp)
	if eventId == GAME_CMD.SC_ROOM_INFO then
		self:RecordRoomInfoPush(rsp)
	elseif eventId == GAME_CMD.SC_GAME_INFO_PUSH then
		self:RecordGameInfoPush(rsp)
	elseif eventId == GAME_CMD.CS_SC_GAMEPLAY_PLAY_CARD then
		self:RecordPlayCardPush(rsp)
	elseif eventId == GAME_CMD.SC_GAMEPLAY_CARD_PUSH then
		self:RecordPushCardPush(rsp)
	elseif eventId == GAME_CMD.SC_GAMEPLAY_WHOS_TURN_PUSH then
		self:RecordWhoTurnPush(rsp)
	elseif eventId == GAME_CMD.CS_SC_GAMEPLAY_HU then
		self:RecordHuPush(rsp)
	elseif eventId == GAME_CMD.CS_SC_GAMEPLAY_GANG then
		self:RecordGangPush(rsp)
	elseif eventId == GAME_CMD.CS_SC_GAMEPLAY_PENG then
		self:RecordPengPush(rsp)
	elseif eventId == GAME_CMD.CS_SC_GAMEPLAY_CHI then
		self:RecordChiPush(rsp)
	elseif eventId == GAME_CMD.SC_SCORE_PUSH then
		self:RecordScorePush(rsp)
	elseif eventId == GAME_CMD.SC_SMALL_RESULT_PUSH then
		self:RecordSmallResultPush(rsp)
	end
end


function MJRecordPlayCardLogic:CleanRoom()
	-- 清理房间显示
	self.roomObj:CleanRoom()

end


--回放房间信息
function MJRecordPlayCardLogic:RecordRoomInfoPush(rsp)
	if rsp then
		-- 回放没有离线概念
		for i,v in ipairs(rsp.playerInfo or {}) do
			if v then
				v.onlineStatus = true
			end
		end
		self.roomInfo = rsp
		self.roomObj:InitConfigBySvr(rsp)
		LuaEvent.AddEvent(EEventType.RefreshRoomRoundNum,rsp.currentGamepos,rsp.totalGameNum)
		LuaEvent.AddEventNow(EEventType.MJShowDeskCenterCtrl,true)
		--房间信息播放后需要立即播放游戏信息发牌
		self.loopTimeDelta = 3

		if rsp and rsp.playerNum and rsp.playerNum > 0 then
			self.roomObj.playerMgr:SetMaxSize(rsp.playerNum)
			LuaEvent.AddEventNow(EEventType.InitHeadByPlayerNum,rsp.playerNum)
		else
			LuaEvent.AddEventNow(EEventType.InitHeadByPlayerNum,4)
		end
	end
end
-- 回放游戏信息推送
function MJRecordPlayCardLogic:RecordGameInfoPush(rsp)
	self.gameInfo = rsp
	local gameInfo = rsp
	if gameInfo then
		local player
		-- 初始化各个玩家打出的牌和自己吃碰杠牌
		for k,v in pairs(gameInfo.playerCard) do
			player = self.roomObj.playerMgr:GetPlayerByPlayerID(v.userId)
			if player then
				-- 初始化各个玩家手牌
				self.roomObj.playerMgr:InitPlayerHandCards(v.userId, v)

				-- 初始化派牌
				if v.paiPai ~= 0 then
					local mjCard = CMJCard.New()
					mjCard:Init(v.paiPai)
					local cardList = {mjCard}
					player.cardMgr:AddCards(cardList)
				end

				for k1,v1 in pairs(v.outputInfo) do
					LuaEvent.AddEventNow(EEventType.MJ_PlayNormalOutMJCardShow, player.seatPos, {v1, false})
				end

				for k1,v1 in pairs(v.opeList) do
					if v1.opeId == 1 then
						--牌展示
						local provider = self.roomObj.playerMgr:GetPlayerByPlayerID(v1.chuPai.chiWithPlayerId)
						LuaEvent.AddEventNow(EEventType.MJ_PlayOwnOutMJCardsShow,{player.seatPos,provider.seatPos},v1.chuPai.pai)
						--吃谁的

					elseif v1.opeId == 2 then
						--牌展示
						local provider = self.roomObj.playerMgr:GetPlayerByPlayerID(v1.pengPai.pengWithPlayerId)
						LuaEvent.AddEventNow(EEventType.MJ_PlayOwnOutMJCardsShow,{player.seatPos,provider.seatPos},v1.pengPai.pai)
						--碰谁的

					elseif v1.opeId == 3 then
						--牌展示
						local provider = self.roomObj.playerMgr:GetPlayerByPlayerID(v1.gangPai.gangWithPlayerId)
						LuaEvent.AddEventNow(EEventType.MJ_PlayOwnOutMJCardsShow,{player.seatPos,provider.seatPos},v1.gangPai.pai)
						--杠谁的

					end
				end
			end

			-- 吃碰胡通知

		end

		--庄家
		player = self.roomObj.playerMgr:GetPlayerByPlayerID(gameInfo.zhuangJiaId)
		if player then
			LuaEvent.AddEventNow(EEventType.PlayerShowZhuangJia,player.seatPos,true)
		end
		--待出牌和风向提示
		if gameInfo.outCardNotice then
			player = self.roomObj.playerMgr:GetPlayerByPlayerID(gameInfo.outCardNotice.fengXiangUserId)
			if player then
				AnimManager.PlayPlayerHeadAnim(true, player.seatPos)
				LuaEvent.AddEventNow(EEventType.MJUpdateWindDir,player.seatPos)
			end
		end
		--牌桌剩余牌张数
		LuaEvent.AddEventNow(EEventType.MJ_ShowRemainCardNum,true,gameInfo.remainPaiNum)

		--开始发牌
		LuaEvent.AddEventNow(EEventType.MJRecordInitHandCards)
		--房间进入游戏状态
		self.roomObj:ChangeState(RoomStateEnum.Playing)

	end
end

-- 玩家出牌推送
function MJRecordPlayCardLogic:RecordPlayCardPush(rsp)
	if rsp then

		local player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.userId)
		self.roomObj.playerMgr:PlayerWaiting(rsp.userId)

		AudioManager.PlayCommonSound(UIAudioEnum.chuPai)
		AudioManager.MJ_PlayBaoPai(player.seatInfo.sex,rsp.pai)
		--消息通知（玩家手牌消失，出牌展示）
		if player then
			local cardIdList = {rsp.pai}
			player.cardMgr:DeleteCards(cardIdList,1)
			LuaEvent.AddEventNow(EEventType.MJRecordRemoveCards,player,cardIdList)--手牌消失
			LuaEvent.AddEventNow(EEventType.MJ_PlayNormalOutMJCardShow,player.seatPos, {rsp.pai, true})--出牌展示
		end
	end
end

--玩家派牌推送
function MJRecordPlayCardLogic:RecordPushCardPush(rsp, head)
	if rsp then
		local player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.userId)
		self.roomObj.playerMgr:PlayerWaiting(rsp.userId)
		if player then
			--派牌 手牌显示
			local mjCard = CMJCard.New()
			mjCard:Init(rsp.pai)
			local cardList = {mjCard}
			player.cardMgr:AddCards(cardList)
			LuaEvent.AddEventNow(EEventType.MJRecordInCard, player, mjCard)
			--操作提示

			--牌桌剩余牌张数
			LuaEvent.AddEventNow(EEventType.MJ_ShowRemainCardNum, true, rsp.remainPaiNum)
		end
	end
end


--谁家出牌
function MJRecordPlayCardLogic:RecordWhoTurnPush(rsp,head)
	if rsp then
		--谁家出牌
		local player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.outCardUserId)
		--风向标指向
		player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.fengXiangUserId)
		if player then
			AnimManager.PlayPlayerHeadAnim(true, player.seatPos)
			LuaEvent.AddEventNow(EEventType.MJUpdateWindDir,player.seatPos)
		else

		end
	end
end

function MJRecordPlayCardLogic:RecordHuPush(rsp,head)
	if rsp then
		--谁胡
		local player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.userId)
		if player then
			--胡牌类型
			if nil == MJHuPaiType.IsMJZiMo then
				print("nil == MJHuPaiType.IsMJZiMo")
			end
			if MJHuPaiType.IsMJZiMo(rsp.huType) then
				AudioManager.MJ_PlayCaoZuo(player.seatInfo.sex,MJ_CaoZuoEnum.ziMo)
				AnimManager.PlayMJCardAnim(MJAnimEnum.ZIMO, player.seatPos)
			else
				AudioManager.MJ_PlayCaoZuo(player.seatInfo.sex,MJ_CaoZuoEnum.huLe)
				AnimManager.PlayMJCardAnim(MJAnimEnum.HU, player.seatPos)

				if not MJHuPaiType.IsMJNoneOrLiuJu(rsp.huType) and rsp.huPai then
					local card_item = CMJCard.New()
					card_item:Init(rsp.huPai, 1)
					local cardList = {card_item}
					player.cardMgr:AddCards(cardList)
					LuaEvent.AddEventNow(EEventType.MJRecordInCard, player, card_item)

					if rsp.huType == MJHuPaiType.Hu and rsp.actionerId then
						-- 出牌显示：去掉别人放炮的那张牌
						local other = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.actionerId)
						LuaEvent.AddEventNow(EEventType.MJ_NormalOutMJCardCatch,other.seatPos,rsp.huPai)
					elseif rsp.huType == MJHuPaiType.QiangGang and rsp.actionerId then
						-- 抢杠胡，把杠的那张牌去掉
						local other = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.actionerId)
						LuaEvent.AddEventNow(EEventType.MJ_PlayQiangGangShow, other.seatPos, rsp.huPai)
					end
				end
			end
		end
	end
end

function MJRecordPlayCardLogic:RecordChiPush(rsp,head)
	if rsp then
		--谁吃牌
		local player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.userId)
		if player then
			player.cardMgr:DeleteCards(rsp.chuPai.pai,2)
			AudioManager.MJ_PlayCaoZuo(player.seatInfo.sex,MJ_CaoZuoEnum.chi)
			--牌展示
			LuaEvent.AddEventNow(EEventType.MJRecordRemoveCards,player,rsp.chuPai.pai)--手牌消失
			local provider = self.roomObj.playerMgr:GetPlayerByPlayerID(v1.chuPai.chiWithPlayerId)
			LuaEvent.AddEventNow(EEventType.MJ_PlayOwnOutMJCardsShow,{player.seatPos,provider.seatPos},rsp.chuPai.pai)--出牌展示
			--吃谁的
			player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.chuPai.chiWithPlayerId)
			LuaEvent.AddEventNow(EEventType.MJ_NormalOutMJCardCatch,player.seatPos,rsp.chuPai.pai[1])
		end
	end
end

function MJRecordPlayCardLogic:RecordPengPush(rsp,head)
	if rsp then
		--谁碰
		local player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.userId)
		if player then
			AudioManager.MJ_PlayCaoZuo(player.seatInfo.sex,MJ_CaoZuoEnum.peng)
			AnimManager.PlayMJCardAnim(MJAnimEnum.PENG, player.seatPos)

			player.cardMgr:DeleteCards(rsp.pengPai.pai,2)
			--牌展示
			LuaEvent.AddEventNow(EEventType.MJRecordRemoveCards,player,rsp.pengPai.pai)--手牌消失
			local provider = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.pengPai.pengWithPlayerId)
			LuaEvent.AddEventNow(EEventType.MJ_PlayOwnOutMJCardsShow,{player.seatPos,provider.seatPos},rsp.pengPai.pai)--出牌展示
		end
		--碰谁的
		if rsp.userId ~= rsp.pengPai.pengWithPlayerId then
			player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.pengPai.pengWithPlayerId)
			if player then
				LuaEvent.AddEventNow(EEventType.MJ_NormalOutMJCardCatch,player.seatPos,rsp.pengPai.pai[1])
			end
		end
	end
end

function MJRecordPlayCardLogic:RecordGangPush(rsp,head)
	if rsp then
		--谁杠
		local player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.userId)

		--杠的类型表现
		if rsp.userId ~= rsp.gangPai.gangWithPlayerId then
			if player then
				AudioManager.MJ_PlayCaoZuo(player.seatInfo.sex,MJ_CaoZuoEnum.gang)
				AnimManager.PlayMJCardAnim(MJAnimEnum.GANG, player.seatPos)

				player.cardMgr:DeleteCards(rsp.gangPai.pai,3)
				--牌展示
				LuaEvent.AddEventNow(EEventType.MJRecordRemoveCards,player,rsp.gangPai.pai)--手牌消失
				local provider = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.gangPai.gangWithPlayerId)
				LuaEvent.AddEventNow(EEventType.MJ_PlayOwnOutMJCardsShow,{player.seatPos,provider.seatPos},rsp.gangPai.pai)--出牌展示
			end
			--杠谁的
			player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.gangPai.gangWithPlayerId)
			if player then
				--被杠的牌消失
				LuaEvent.AddEventNow(EEventType.MJ_NormalOutMJCardCatch,player.seatPos,rsp.gangPai.pai[1])
			end
		else--自己抓牌杠
			--自己手牌消失
			if player then
				if rsp.gangPai.gangType == 3 then
					player.cardMgr:DeleteCards(rsp.gangPai.pai,1)
					AudioManager.MJ_PlayCaoZuo(player.seatInfo.sex,MJ_CaoZuoEnum.gang)
				else
					player.cardMgr:DeleteCards(rsp.gangPai.pai,4)
					AudioManager.MJ_PlayCaoZuo(player.seatInfo.sex,MJ_CaoZuoEnum.anGang)
				end
				AnimManager.PlayMJCardAnim(MJAnimEnum.GANG, player.seatPos)

				LuaEvent.AddEventNow(EEventType.MJRecordRemoveCards,player,rsp.gangPai.pai)--手牌消失
				--暗杠
				LuaEvent.AddEventNow(EEventType.MJ_PlayOwnSupplyGangShow,player.seatPos,rsp.gangPai.pai)
			end
		end
	end
end

function MJRecordPlayCardLogic:RecordScorePush(rsp,head)
	if rsp then
		local player
		for k,v in pairs(rsp.userJifen) do
		 	player = self.roomObj.playerMgr:GetPlayerByPlayerID(v.userId)
			LuaEvent.AddEventNow(EEventType.RefreshPlayerTotalScore,player.seatPos,v.jifen)
			--是否展示 本次积分效果
		end
	end
end

--小局结算推送
function MJRecordPlayCardLogic:RecordSmallResultPush(rsp,head)
	if rsp then
		self:PausePlay(true)
		self.roomObj.playerMgr:AllPlayerWaiting()
		self.roomObj:AddSmallResult(rsp.currentGamepos,rsp)
		self.roomObj:ChangeState(RoomStateEnum.SmallRoundOver)
	end
end

function MJRecordPlayCardLogic:WSK_PlayCaoZuoSound(seatId,caoZuoEnum)
	local player = self.roomObj.playerMgr:GetPlayerBySeatID(seatId)
	if player then
		AudioManager.WSK_PlayCaoZuo(player.seatInfo.sex,caoZuoEnum)
	end
end
