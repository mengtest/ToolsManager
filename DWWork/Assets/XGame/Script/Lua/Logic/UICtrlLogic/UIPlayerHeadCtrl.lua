--------------------------------------------------------------------------------
-- 	 File      : UIPlayerHeadCtrl.lua
--   author    : guoliang
--   function   : 牌局玩家头像显示控制
--   date      : 2017-10-9
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
local CountDownClock = require "CommonProduct.CommonBase.UICtrl.UICountDownClock"

UIPlayerHeadCtrl = class("UIPlayerHeadCtrl",nil)

function UIPlayerHeadCtrl:Init(rootTrans,luaWindowRoot)
	self.rootTrans = rootTrans
	self.luaWindowRoot = luaWindowRoot
	-- 屏蔽准备中位置
	self.headPreparePos = {}
	self.headPlayPos = {}

	self.southNodeTrans = luaWindowRoot:GetTrans(rootTrans,"south_node")
	local southplayPosTrans = self.luaWindowRoot:GetTrans(self.southNodeTrans,"play_pos")
	self.southHeadTrans = self:LoadHeadItem(southplayPosTrans, "south")
	-- self.southHeadTrans = luaWindowRoot:GetTrans(self.southNodeTrans,"head_item")
	self:InitHeadItem(self.southHeadTrans,nil)
	self.luaWindowRoot:SetActive(self.southHeadTrans,false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.southNodeTrans, "SoreAnim_1"),false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.southNodeTrans, "SoreAnim_2"),false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.southNodeTrans, "BombAnim"),false)
	-- local southpreparePosTrans = self.luaWindowRoot:GetTrans(self.southNodeTrans,"prepare_pos")
	-- self.headPreparePos[1] = southpreparePosTrans.position
	self.headPlayPos[1] = southplayPosTrans.position

	self.eastNodeTrans = luaWindowRoot:GetTrans(rootTrans,"east_node")
	local eastplayPosTrans = self.luaWindowRoot:GetTrans(self.eastNodeTrans,"play_pos")
	self.eastHeadTrans = self:LoadHeadItem(eastplayPosTrans, "east")
	-- self.eastHeadTrans = luaWindowRoot:GetTrans(self.eastNodeTrans,"head_item")
	self:InitHeadItem(self.eastHeadTrans,nil)
	self.luaWindowRoot:SetActive(self.eastHeadTrans,false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.eastNodeTrans, "SoreAnim_1"),false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.eastNodeTrans, "SoreAnim_2"),false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.eastNodeTrans, "BombAnim"),false)
	-- local eastpreparePosTrans = self.luaWindowRoot:GetTrans(self.eastNodeTrans,"prepare_pos")
	-- self.headPreparePos[2] = eastpreparePosTrans.position
	self.headPlayPos[2] = eastplayPosTrans.position

	self.northNodeTrans = luaWindowRoot:GetTrans(rootTrans,"north_node")
	local northplayPosTrans = self.luaWindowRoot:GetTrans(self.northNodeTrans,"play_pos")
	self.northHeadTrans = self:LoadHeadItem(northplayPosTrans, "north")
	-- self.northHeadTrans = luaWindowRoot:GetTrans(self.northNodeTrans,"head_item")
	self:InitHeadItem(self.northHeadTrans,nil)
	self.luaWindowRoot:SetActive(self.northHeadTrans,false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.northNodeTrans, "SoreAnim_1"),false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.northNodeTrans, "SoreAnim_2"),false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.northNodeTrans, "BombAnim"),false)
	-- local northpreparePosTrans = self.luaWindowRoot:GetTrans(self.northNodeTrans,"prepare_pos")
	-- self.headPreparePos[3] = northpreparePosTrans.position
	self.headPlayPos[3] = northplayPosTrans.position

	self.westNodeTrans = luaWindowRoot:GetTrans(rootTrans,"west_node")
	local westplayPosTrans = self.luaWindowRoot:GetTrans(self.westNodeTrans,"play_pos")
	self.westHeadTrans = self:LoadHeadItem(westplayPosTrans, "west")
	-- luaWindowRoot:GetTrans(self.westNodeTrans,"head_item")
	self:InitHeadItem(self.westHeadTrans,nil)
	self.luaWindowRoot:SetActive(self.westHeadTrans,false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.westNodeTrans, "SoreAnim_1"),false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.westNodeTrans, "SoreAnim_2"),false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.westNodeTrans, "BombAnim"),false)
	-- local westpreparePosTrans = self.luaWindowRoot:GetTrans(self.westNodeTrans,"prepare_pos")
	-- self.headPreparePos[4] = westpreparePosTrans.position
	self.headPlayPos[4] = westplayPosTrans.position

	self:Destroy()
	self:RegisterEvent()
	UpdateSecond:Add(self.UpdateSec, self)
	self.timerList = {}
	self.countDownList = {}
end

function UIPlayerHeadCtrl:LoadHeadItem(parent, seatPos)
	local itemName = "head_item_" .. seatPos
	local resObj = WrapSys.ResourceMgr_GetInsAssetInParent(RessType.RT_UIItem, itemName, parent, RessStorgeType.RST_Never)
	if not resObj then
		return
	end
	resObj.name = "head_item"
	resObj:SetActive(false)

	return resObj.transform
end

function UIPlayerHeadCtrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.RefreshPlayerPoint,self.RefreshPlayerPoint,self)
	LuaEvent.AddHandle(EEventType.RefreshPlayerBombNum,self.RefreshPlayerBombNum,self)
	LuaEvent.AddHandle(EEventType.RefreshPlayerTotalScore,self.RefreshPlayerTotalScore,self)
	LuaEvent.AddHandle(EEventType.RefreshPlayerStatusTimer,self.RefreshPlayerStatusTimer,self)
	LuaEvent.AddHandle(EEventType.PlayNotPrepare, self.PlayNotPrepare, self)
	LuaEvent.AddHandle(EEventType.PlayerPrepared,self.PlayerPrepared,self)
	LuaEvent.AddHandle(EEventType.PlayerShowAlone,self.PlayerShowAlone,self)
	LuaEvent.AddHandle(EEventType.PlayerShowFriend,self.PlayerShowFriend,self)
	LuaEvent.AddHandle(EEventType.PlayerShowSort,self.PlayerShowSort,self)
	LuaEvent.AddHandle(EEventType.PlayerShowFangZhu,self.PlayerShowFangZhu,self)
	LuaEvent.AddHandle(EEventType.OnlineRefresh,self.OnlineRefresh,self)
	LuaEvent.AddHandle(EEventType.PlayerShowRemainHandCards,self.PlayerShowRemainHandCards,self)
	LuaEvent.AddHandle(EEventType.ResetPlayerGameHeadStatus,self.ResetPlayerGameHeadStatus,self)
	LuaEvent.AddHandle(EEventType.PlayerShowZhuangJia,self.PlayerShowZhuangJia,self)
	LuaEvent.AddHandle(EEventType.ShowPlayerHuType, self.ShowPlayerHuType, self)
	LuaEvent.AddHandle(EEventType.DDZLordFind, self.PlayerShowLord, self)

	LuaEvent.AddHandle(EEventType.PlayEmoji,self.PlayEmoji,self)
	LuaEvent.AddHandle(EEventType.ShowPlayerPointAnim, self.ShowPlayerPointAnimFunc, self)
	LuaEvent.AddHandle(EEventType.ShowPlayerBombAnim, self.ShowPlayerBombAnimFunc, self)
	LuaEvent.AddHandle(EEventType.InitHeadByPlayerNum, self.HandleInitHeadByPlayerNum, self)
end

function UIPlayerHeadCtrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.RefreshPlayerPoint,self.RefreshPlayerPoint,self)
	LuaEvent.RemoveHandle(EEventType.RefreshPlayerBombNum,self.RefreshPlayerBombNum,self)
	LuaEvent.RemoveHandle(EEventType.RefreshPlayerTotalScore,self.RefreshPlayerTotalScore,self)
	LuaEvent.RemoveHandle(EEventType.RefreshPlayerStatusTimer,self.RefreshPlayerStatusTimer,self)
	LuaEvent.RemoveHandle(EEventType.PlayNotPrepare, self.PlayNotPrepare, self)
	LuaEvent.RemoveHandle(EEventType.PlayerPrepared,self.PlayerPrepared,self)
	LuaEvent.RemoveHandle(EEventType.PlayerShowAlone,self.PlayerShowAlone,self)
	LuaEvent.RemoveHandle(EEventType.PlayerShowFriend,self.PlayerShowFriend,self)
	LuaEvent.RemoveHandle(EEventType.PlayerShowSort,self.PlayerShowSort,self)
	LuaEvent.RemoveHandle(EEventType.PlayerShowFangZhu,self.PlayerShowFangZhu,self)
	LuaEvent.RemoveHandle(EEventType.OnlineRefresh,self.OnlineRefresh,self)
	LuaEvent.RemoveHandle(EEventType.PlayerShowRemainHandCards,self.PlayerShowRemainHandCards,self)
	LuaEvent.RemoveHandle(EEventType.ResetPlayerGameHeadStatus,self.ResetPlayerGameHeadStatus,self)
	LuaEvent.RemoveHandle(EEventType.PlayerShowZhuangJia,self.PlayerShowZhuangJia,self)
	LuaEvent.RemoveHandle(EEventType.ShowPlayerHuType, self.ShowPlayerHuType, self)
	LuaEvent.RemoveHandle(EEventType.DDZLordFind, self.PlayerShowLord, self)

	LuaEvent.RemoveHandle(EEventType.PlayEmoji,self.PlayEmoji,self)
	LuaEvent.RemoveHandle(EEventType.ShowPlayerPointAnim, self.ShowPlayerPointAnimFunc, self)
	LuaEvent.RemoveHandle(EEventType.ShowPlayerBombAnim, self.ShowPlayerBombAnimFunc, self)
	LuaEvent.RemoveHandle(EEventType.InitHeadByPlayerNum, self.HandleInitHeadByPlayerNum, self)
end

function UIPlayerHeadCtrl:HandleInitHeadByPlayerNum(eventId,p1,p2)
	local player_num = p1
	local play_logic = PlayGameSys.GetPlayLogic()
	local self_player = play_logic.roomObj.playerMgr:GetPlayerByPlayerID(play_logic.roomObj:GetSouthUID())
	local self_seat_id = self_player and self_player.seatInfo and self_player.seatInfo.seatId or nil
	if player_num == 2 then
		self.luaWindowRoot:SetActive(self.eastHeadTrans, false)
		self.luaWindowRoot:SetActive(self.westHeadTrans, false)
		self.luaWindowRoot:SetActive(self.northHeadTrans, true)
		self.luaWindowRoot:SetActive(self.southHeadTrans, true)
	elseif player_num == 3 and self_seat_id then
		if self_seat_id == 0 then
			self.luaWindowRoot:SetActive(self.eastHeadTrans, true)
			self.luaWindowRoot:SetActive(self.westHeadTrans, false)
			self.luaWindowRoot:SetActive(self.northHeadTrans, true)
			self.luaWindowRoot:SetActive(self.southHeadTrans, true)
		elseif self_seat_id == 1 then
			self.luaWindowRoot:SetActive(self.eastHeadTrans, true)
			self.luaWindowRoot:SetActive(self.westHeadTrans, true)
			self.luaWindowRoot:SetActive(self.northHeadTrans, false)
			self.luaWindowRoot:SetActive(self.southHeadTrans, true)
		elseif self_seat_id == 2 then
			self.luaWindowRoot:SetActive(self.eastHeadTrans, false)
			self.luaWindowRoot:SetActive(self.westHeadTrans, true)
			self.luaWindowRoot:SetActive(self.northHeadTrans, true)
			self.luaWindowRoot:SetActive(self.southHeadTrans, true)
		end
	else
		self.luaWindowRoot:SetActive(self.eastHeadTrans, true)
		self.luaWindowRoot:SetActive(self.westHeadTrans, true)
		self.luaWindowRoot:SetActive(self.northHeadTrans, true)
		self.luaWindowRoot:SetActive(self.southHeadTrans, true)
	end
end

function UIPlayerHeadCtrl:RefreshPlayerPoint(eventId,p1,p2)
	local seatPos = p1
	local pointNum = p2
	self:PlayerPointRefresh(seatPos,pointNum)
end

function UIPlayerHeadCtrl:ShowPlayerPointAnimFunc(eventID, p1, p2)
	local seatPos = p1
	local showPointNum = p2
	
	local trans = self:GetSeatTransByPos(seatPos)
	self:ShowPlayerPointAnim(trans, showPointNum)
end

function UIPlayerHeadCtrl:ShowPlayerBombAnimFunc(eventID, p1, p2)
	local seatPos = p1
	local showBombNum = p2
	
	local trans = self:GetSeatTransByPos(seatPos)
	self:ShowPlayerBombAnim(trans, showBombNum)
end

function UIPlayerHeadCtrl:RefreshPlayerBombNum(eventId,p1,p2)
	local seatPos = p1
	local bombNum = p2
	self:PlayerBombNumRefresh(seatPos,bombNum)
end

function UIPlayerHeadCtrl:RefreshPlayerTotalScore(eventId,p1,p2)
	local seatPos = p1
	local bombNum = p2
	self:PlayerTotalScoreRefresh(seatPos,bombNum)
end

function UIPlayerHeadCtrl:RefreshPlayerStatusTimer(eventId,p1,p2)
	local seatPos = p1
	local status = p2
	local playID = PlayGameSys.GetNowPlayId()
	if playID == Common_PlayID.DW_DouDiZhu then
		self:PlayerStatusTipShow_2(seatPos, status)
	else
		self:PlayerStatusTipShow(seatPos,status)
	end
end

-- 显示准备中效果
function UIPlayerHeadCtrl:PlayNotPrepare(eventId, seatPos, isShow)
	local trans = self:GetSeatTransByPos(seatPos)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans, "label_unprepare"), isShow)
end

--显示准备icon
function UIPlayerHeadCtrl:PlayerPrepared(eventId,p1,p2)
	local is_record = PlayGameSys.GetIsRecord()
	if not is_record then
		local seatPos = p1
		local isShow = p2
		local trans = self:GetSeatTransByPos(seatPos)
		self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans,"label_prepare"), isShow)
	end
end

--打独ico显示
function UIPlayerHeadCtrl:PlayerShowAlone(eventId,p1,p2)
	local seatPos = p1
	local isShow = p2

	local trans = self:GetSeatTransByPos(seatPos)
	local headItemTrans = self.luaWindowRoot:GetTrans(trans,"head_item")
	local icoTrans = self.luaWindowRoot:GetTrans(headItemTrans,"ico_alone")
	self.luaWindowRoot:SetActive(icoTrans,isShow)
end
-- 朋友出现显示
function UIPlayerHeadCtrl:PlayerShowFriend(eventId,p1,p2)
	local seatPos = p1
	local isShow = p2

	local trans = self:GetSeatTransByPos(seatPos)
	local headItemTrans = self.luaWindowRoot:GetTrans(trans,"head_item")
	local icoTrans = self.luaWindowRoot:GetTrans(headItemTrans,"ico_friend")
	self.luaWindowRoot:SetActive(icoTrans,isShow)
end
--几游
function UIPlayerHeadCtrl:PlayerShowSort(eventId,p1,p2)
	local seatPos = p1
	local sort = p2
	local trans = self:GetSeatTransByPos(seatPos)
	local headItemTrans = self.luaWindowRoot:GetTrans(trans,"head_item")
	local icoTrans = self.luaWindowRoot:GetTrans(headItemTrans,"ico_sort")
	if sort == 1 then
		self.luaWindowRoot:SetActive(icoTrans,true)
		self.luaWindowRoot:SetSprite(icoTrans,"gameui_icon_first")
	elseif sort == 2 then
		self.luaWindowRoot:SetActive(icoTrans,true)
		self.luaWindowRoot:SetSprite(icoTrans,"gameui_icon_second")
	elseif sort == 3 then
		self.luaWindowRoot:SetActive(icoTrans,true)
		self.luaWindowRoot:SetSprite(icoTrans,"gameui_icon_three")
	elseif sort == 4 then
		self.luaWindowRoot:SetActive(icoTrans,true)
		self.luaWindowRoot:SetSprite(icoTrans,"gameui_icon_end")
	else
		self.luaWindowRoot:SetActive(icoTrans,false)
	end
end
--是否房主显示
function UIPlayerHeadCtrl:PlayerShowFangZhu(eventId,p1,p2)
	local seatPos = p1
	local isShow = p2
	local trans = self:GetSeatTransByPos(seatPos)
	local headItemTrans = self.luaWindowRoot:GetTrans(trans,"head_item")
	local icoTrans = self.luaWindowRoot:GetTrans(headItemTrans,"ico_fangZhu")
	self.luaWindowRoot:SetActive(icoTrans,isShow)
end

--在线状态刷新
function UIPlayerHeadCtrl:OnlineRefresh(eventId,p1,p2)
	local seatPos = p1
	local isOn = p2
	self:PlayerOnlineRefresh(seatPos,isOn)
end

--显示剩余手牌数量
function UIPlayerHeadCtrl:PlayerShowRemainHandCards(eventId,p1,p2)
	local seatPos = p1
	local remainNum = p2
	local trans = self:GetSeatTransByPos(seatPos)
	local headItemTrans = self.luaWindowRoot:GetTrans(trans,"head_item")
	local icoTrans = self.luaWindowRoot:GetTrans(headItemTrans,"label_remainCardNum")
	if remainNum > 0 then
		self.luaWindowRoot:SetActive(icoTrans,true)
		self.luaWindowRoot:SetLabel(icoTrans,remainNum)
	else
		self.luaWindowRoot:SetActive(icoTrans,false)
	end
end

-- 是否是地主显示
function UIPlayerHeadCtrl:PlayerShowLord(eventId, p1, p2)
	local seatPos = p1
	local isShow = p2
	
	self:_setPlayerShowLord(seatPos, isShow)
end

function UIPlayerHeadCtrl:_setPlayerShowLord(seatPos, isShow)
	local trans = self:GetSeatTransByPos(seatPos)
	if trans ~= nil then
		local headItemTrans = self.luaWindowRoot:GetTrans(trans, "head_item")
		local playID = PlayGameSys.GetNowPlayId()
		if playID and playID == Common_PlayID.DW_DouDiZhu then
			self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(headItemTrans, "ico_lord"), isShow)
		end
	end
end

-- 设置庄家显示
function UIPlayerHeadCtrl:_setPlayerShowZhuangJia(seatPos, isShow)
	local trans = self:GetSeatTransByPos(seatPos)
	local headItemTrans = self.luaWindowRoot:GetTrans(trans, "head_item")
	local icoTrans = self.luaWindowRoot:GetTrans(headItemTrans, "ico_zhuangjia")
	self.luaWindowRoot:SetActive(icoTrans, isShow)
	local play_id = PlayGameSys.GetNowPlayId()
	if play_id and play_id == Common_PlayID.ThirtyTwo then
		self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(headItemTrans, "ico_dealer"), true)
	end
end

-- 麻将庄家
function UIPlayerHeadCtrl:PlayerShowZhuangJia(eventId,p1,p2)
	local seatPos = p1
	-- local isShow = p2

	if nil ~= self.perMJZhuangJiaSeatPos then
		self:_setPlayerShowZhuangJia(self.perMJZhuangJiaSeatPos, false)
	end

	self:_setPlayerShowZhuangJia(seatPos, true)
	self.perMJZhuangJiaSeatPos = seatPos
end

-- 显示放炮
function UIPlayerHeadCtrl:ShowPlayerHuType(eventId, playerList)
	for k,data in pairs(playerList) do
		local trans, isFanPao = nil, false
		if nil ~= data.player then
			trans = self:GetSeatTransByPos(data.player.seatPos)
		elseif nil ~= data.other then
			isFanPao = true
			trans = self:GetSeatTransByPos(data.other.seatPos)
		end

		if nil ~= trans then
			local spriteName = ""
			if MJHuPaiType.IsMJZiMo(data.huType) then
				spriteName = "gameui_icon_zimo"
			else
				if isFanPao then
					spriteName = "gameui_icon_pao"
				else
					spriteName = "gameui_icon_hu"
				end
			end

			if "" ~= spriteName then
				local huTypeTrans = self.luaWindowRoot:GetTrans(trans, "huType")
				self.luaWindowRoot:SetSprite(huTypeTrans, spriteName)
				self.luaWindowRoot:SetActive(huTypeTrans, true)
			end
		end
	end
end

-- 重置玩家游戏状态信息
function UIPlayerHeadCtrl:ResetPlayerGameHeadStatus(eventId,p1,p2)
	local player = p1
	if player == nil then
		return
	end

	local isOutLine = not player.seatInfo.onlineStatus
	local trans = self:GetSeatTransByPos(player.seatPos)

	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans,"timer_root"),false)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans,"label_no_out"),false)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans,"label_offline"), isOutLine)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans,"label_prepare"),false)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans,"label_unprepare"),false)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans,"label_point"),false)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans,"label_bombNum"),false)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans,"label_remainCardNum"),false)

	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans,"ico_friend"),false)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans,"ico_sort"),false)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans,"ico_alone"),false)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans,"ico_zhuangjia"),false)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans,"ico_dealer"),false)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans,"huType"),false)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans, "ico_lord"), false)
end

-- 显示不同颜色名字
function UIPlayerHeadCtrl:SetNameColor(nameTrans, nickName)
	local logicType = PlayGameSys.GetPlayLogicType()
	-- if logicType == PlayLogicTypeEnum.WSK_Normal or logicType == PlayLogicTypeEnum.WSK_Record then
	-- 	self.luaWindowRoot:SetColor(nameTrans, UnityEngine.Color.New(0.812, 1, 0.239))
	-- else
	-- 	self.luaWindowRoot:SetColor(nameTrans, UnityEngine.Color.New(0.486, 0.733, 0.718))
	-- end
	self.luaWindowRoot:SetLabel(nameTrans, utf8sub(nickName, 1, 10))
end

function UIPlayerHeadCtrl:InitHeadItem(trans, seatInfo)
	self.luaWindowRoot:SetActive(trans,true)
	local headIconTrans = self.luaWindowRoot:GetTrans(trans,"ico_head")
	local labelNoPeopleTrans = self.luaWindowRoot:GetTrans(trans,"ico_noPeople")
	local nameTrans = self.luaWindowRoot:GetTrans(trans,"label_name")
	local totalScoreTrans = self.luaWindowRoot:GetTrans(trans,"label_totalScore")
	local timeRootTrans = self.luaWindowRoot:GetTrans(trans,"timer_root")
	local noOutTrans = self.luaWindowRoot:GetTrans(trans,"label_no_out")
	local offlineTrans = self.luaWindowRoot:GetTrans(trans,"label_offline")
	local prepareTrans = self.luaWindowRoot:GetTrans(trans,"label_prepare")
	local unprepareTrans = self.luaWindowRoot:GetTrans(trans,"label_unprepare")
	local pointTrans = self.luaWindowRoot:GetTrans(trans,"label_point")
	local bombNumTrans = self.luaWindowRoot:GetTrans(trans,"label_bombNum")
	local remainCardNumTrans = self.luaWindowRoot:GetTrans(trans,"label_remainCardNum")

	local ico_fangZhu = self.luaWindowRoot:GetTrans(headIconTrans,"ico_fangZhu")
	self.luaWindowRoot:SetActive(timeRootTrans,false)

	if seatInfo then
		self.luaWindowRoot:SetActive(headIconTrans,true)
		self.luaWindowRoot:SetActive(labelNoPeopleTrans,false)
		WindowUtil.LoadHeadIcon(self.luaWindowRoot, headIconTrans, seatInfo.headUrl, seatInfo.sex,false,RessStorgeType.RST_Never)
		self:SetNameColor(nameTrans, seatInfo.nickName)
		self.luaWindowRoot:SetLabel(totalScoreTrans,seatInfo.totalJiFen)

		--设置房主标识
		local showFanzhu = false
		if PlayGameSys.GetPlayLogic() and PlayGameSys.GetPlayLogic().roomObj and PlayGameSys.GetPlayLogic().roomObj.roomInfo then
			showFanzhu = PlayGameSys.GetPlayLogic().roomObj.roomInfo.fangZhu == seatInfo.userId
		end
		self.luaWindowRoot:SetActive(ico_fangZhu,showFanzhu)

	else
		self.luaWindowRoot:SetActive(headIconTrans,true)
		self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(headIconTrans,"ico_friend"),false)
		self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(headIconTrans,"ico_sort"),false)
		self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(headIconTrans,"ico_alone"),false)
		self.luaWindowRoot:SetActive(ico_fangZhu,false)
		self.luaWindowRoot:SetActive(headIconTrans,false)
		
		self.luaWindowRoot:SetActive(noOutTrans,false)
		self.luaWindowRoot:SetActive(offlineTrans,false)
		self.luaWindowRoot:SetActive(prepareTrans,false)
		self.luaWindowRoot:SetActive(unprepareTrans, false)
		self.luaWindowRoot:SetActive(pointTrans,false)
		self.luaWindowRoot:SetActive(bombNumTrans,false)
		--self.luaWindowRoot:SetActive(remainCardNumTrans,false)

		self.luaWindowRoot:SetActive(labelNoPeopleTrans,true)
	end
end

-- 获取节点
function UIPlayerHeadCtrl:GetSeatTransByPos(seatPos)
	if seatPos == SeatPosEnum.South then
		return self.southNodeTrans
	elseif seatPos == SeatPosEnum.East then
		return self.eastNodeTrans
	elseif seatPos == SeatPosEnum.North then
		return self.northNodeTrans
	else
		return self.westNodeTrans
	end
end

-- 刷新头像显示
function UIPlayerHeadCtrl:RefreshPlayerHead(seatPos,seatInfo)
	-- TODO::屏蔽准备中界面
	local trans = self:GetSeatTransByPos(seatPos)
	-- local preparePosTrans = self.luaWindowRoot:GetTrans(trans,"prepare_pos")
	local playPosTrans = self.luaWindowRoot:GetTrans(trans,"play_pos")
	local headItemTrans = self.luaWindowRoot:GetTrans(trans,"head_item")

	self:InitHeadItem(headItemTrans,seatInfo)
end

-- 刷新头像位置
function UIPlayerHeadCtrl:RefreshPlayerHeadPos(seatPos)
	-- TODO::屏蔽准备中界面
	local trans = self:GetSeatTransByPos(seatPos)
	-- local preparePosTrans = self.luaWindowRoot:GetTrans(trans,"prepare_pos")
	local playPosTrans = self.luaWindowRoot:GetTrans(trans,"play_pos")
	local headItemTrans = self.luaWindowRoot:GetTrans(trans,"head_item")

	headItemTrans.parent = playPosTrans
	headItemTrans.localPosition = UnityEngine.Vector3.New(0,0,0)
end

function UIPlayerHeadCtrl:PlayerStatusTipShow_2(seatPos,status)
	local trans = self:GetSeatTransByPos(seatPos)
	local countRoot = self.luaWindowRoot:GetTrans(trans,"countDownRoot")
	
	if self.countDownList[seatPos] ~= nil then
		self.countDownList[seatPos]:Destroy(false, true)
		self.countDownList[seatPos] = nil
	end
	
	if status == PlayerStateEnum.Playing or
		status == PlayerStateEnum.WantLord or
		status == PlayerStateEnum.LordIsOpenPlay or
		status == PlayerStateEnum.FamerAddPlus or
		status == PlayerStateEnum.LordSubPlus then
		self.countDownList[seatPos] = CountDownClock.New(self.luaWindowRoot, countRoot, 15, nil ,false)
	else
	end
end

function UIPlayerHeadCtrl:PlayerStatusTipShow(seatPos,status)
	local trans = self:GetSeatTransByPos(seatPos)
	local timeRootTrans = self.luaWindowRoot:GetTrans(trans,"timer_root")
	if status == PlayerStateEnum.Playing 
		or status == PlayerStateEnum.CallAlone
		or status == PlayerStateEnum.FindFriend then
		
		if self.timerList[seatPos] == nil then
			self.timerList[seatPos] = {}
		end
		self.timerList[seatPos].curRemainTime = 15
		self.luaWindowRoot:SetActive(timeRootTrans,true)
		self.timerList[seatPos].curTimeTrans = self.luaWindowRoot:GetTrans(timeRootTrans,"label_time")
		self.luaWindowRoot:SetLabel(self.timerList[seatPos].curTimeTrans,self.timerList[seatPos].curRemainTime)
	else
		if self.timerList[seatPos] then
			self.timerList[seatPos].curRemainTime = 0
			self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans,"timer_root"),false)
			self.timerList[seatPos] = nil
		end
	end
end

function UIPlayerHeadCtrl:ShowNotOutTips(seatPos,isShow)
	local trans = self:GetSeatTransByPos(seatPos)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans,"label_no_out"),isShow)
end

function UIPlayerHeadCtrl:ClearAllNotOutTips( )
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(self.westNodeTrans,"label_no_out"),false)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(self.southNodeTrans,"label_no_out"),false)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(self.northNodeTrans,"label_no_out"),false)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(self.eastNodeTrans,"label_no_out"),false)
end

function UIPlayerHeadCtrl:PlayerOnlineRefresh(seatPos,isOn)
	local trans = self:GetSeatTransByPos(seatPos)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans,"label_offline"),not isOn)
end
-- 手牌积分刷新
function UIPlayerHeadCtrl:PlayerPointRefresh(seatPos,pointNum)
	local trans = self:GetSeatTransByPos(seatPos)
	local pointTrans = self.luaWindowRoot:GetTrans(trans,"label_point")
	if pointNum <= 0 then
		self.luaWindowRoot:SetActive(pointTrans,false)
	else
		self.luaWindowRoot:SetActive(pointTrans,true)
		self.luaWindowRoot:SetLabel(pointTrans,pointNum.."分")
	end
end

function UIPlayerHeadCtrl:ShowPlayerPointAnim(trans, showScore)
	if showScore == 0 then return end

	local logicType = PlayGameSys.GetPlayLogicType()
	if logicType == PlayLogicTypeEnum.WSK_Normal then
		self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans, "BombAnim"), false)
		for i = 1, 2 do
			local animRoot = self.luaWindowRoot:GetTrans(trans, "SoreAnim_" .. i)
			self.luaWindowRoot:SetActive(animRoot, true)
		
		
			local addScoreTrans = self.luaWindowRoot:GetTrans(animRoot, "AddScore")
			local subScoreTrans = self.luaWindowRoot:GetTrans(animRoot, "SubScore")
			if showScore > 0 then
				self.luaWindowRoot:SetActive(addScoreTrans, true)
				self.luaWindowRoot:SetActive(subScoreTrans, false)
				self.luaWindowRoot:SetLabel(addScoreTrans, "+" .. showScore)
			else
				self.luaWindowRoot:SetActive(addScoreTrans, true)
				self.luaWindowRoot:SetActive(subScoreTrans, false)
				self.luaWindowRoot:SetLabel(subScoreTrans, showScore)
			end

			self.luaWindowRoot:PlayAndStopTweens(animRoot, true)
		end
	end
end

function UIPlayerHeadCtrl:ShowPlayerBombAnim(trans, bombScore)
	if bombScore == 0 then return end
	
	local logicType = PlayGameSys.GetPlayLogicType()
	if logicType == PlayLogicTypeEnum.WSK_Normal then
		local animRoot = self.luaWindowRoot:GetTrans(trans, "BombAnim")
		self.luaWindowRoot:SetActive(animRoot, true)
		self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans, "SoreAnim"), false)
		
		self.luaWindowRoot:SetLabel(self.luaWindowRoot:GetTrans(animRoot, "bombScore"), "x" .. bombScore)
		self.luaWindowRoot:PlayAndStopTweens(animRoot, true)
	end
end

--炸弹赔率刷新
function UIPlayerHeadCtrl:PlayerBombNumRefresh(seatPos,bombNum)
	local trans = self:GetSeatTransByPos(seatPos)
	local bombNumTrans = self.luaWindowRoot:GetTrans(trans,"label_bombNum")
	if bombNum < 0 then
		self.luaWindowRoot:SetActive(bombNumTrans,false)
	else
		self.luaWindowRoot:SetActive(bombNumTrans,true)
		self.luaWindowRoot:SetLabel(bombNumTrans,bombNum.."倍")
	end
end

-- 总积分刷新
function UIPlayerHeadCtrl:PlayerTotalScoreRefresh(seatPos,totalScore)
	local trans = self:GetSeatTransByPos(seatPos)
	local totalScoreTrans = self.luaWindowRoot:GetTrans(trans,"label_totalScore")
	self.luaWindowRoot:SetActive(totalScoreTrans,true)
	self.luaWindowRoot:SetLabel(totalScoreTrans,totalScore)
end

function UIPlayerHeadCtrl:UpdateSec()
	for k,v in pairs(self.timerList) do
		if v.curRemainTime > 0 then
			v.curRemainTime = v.curRemainTime - 1
			self.luaWindowRoot:SetLabel(v.curTimeTrans,v.curRemainTime)
		end
	end
end

function UIPlayerHeadCtrl:HandleWidgetClick(gb)
	local click_name = gb.name
	if string.find(click_name, "headCollider_") then
		local seatPos = string.sub(click_name, string.len("headCollider_") + 1)

		local playCardLogic = PlayGameSys.GetPlayLogic()
		local seatInfo = playCardLogic.roomObj:GetPlayerBySeatPos(seatPos)

		if seatInfo then
			WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "userInfo_ui_window", false , seatInfo)
		else
			-- 是提审状态则不反应
			if not DataManager.isPrePublish then
				-- 没有找到分享房间
				local roomInfo = playCardLogic.roomObj.roomInfo
				if roomInfo then
					HallSys.ShareRoom(roomInfo.roomId)
				end
			end
		end
	end
end

function UIPlayerHeadCtrl:GetHeadBySetPos(SeatPos)
	if SeatPos == SeatPosEnum.South then
		return self.southHeadTrans
	elseif SeatPos == SeatPosEnum.East then
		return self.eastHeadTrans
	elseif SeatPos == SeatPosEnum.North then
		return self.northHeadTrans
	elseif SeatPos == SeatPosEnum.West then
		return self.westHeadTrans
	end
end

-------------------------------------------------------------------------------------------
--添加表情
local myEmojiAnimations = {}
function UIPlayerHeadCtrl:AddMyEmoji(fromSeatPos,dwSpriteAnimation,emojiContent)
	if myEmojiAnimations[fromSeatPos] then
		myEmojiAnimations[fromSeatPos].dwSpriteAnimation:Stop(false)
	end

	myEmojiAnimations[fromSeatPos] = {}
	myEmojiAnimations[fromSeatPos].dwSpriteAnimation = dwSpriteAnimation
	myEmojiAnimations[fromSeatPos].emojiContent = emojiContent
end

--移除表情 隐藏背景
function UIPlayerHeadCtrl:DelMyEmoji(fromSeatPos,luaWindowRoot)
	if myEmojiAnimations[fromSeatPos] then
		luaWindowRoot:SetActive(myEmojiAnimations[fromSeatPos].emojiContent,false)
		myEmojiAnimations[fromSeatPos] = nil
	end
end

--播放动画
function UIPlayerHeadCtrl:PlayEmoji(eventId,p1,p2)
	local emojiInfo = p1
	fromSeatPos = emojiInfo.fromSeatPos
	toSeatPos = emojiInfo.toSeatPos
	emojiID = emojiInfo.emojiID

	local isMy = fromSeatPos == toSeatPos
	--自己表情
	if isMy then
		local fromSeatTrans = self:GetHeadBySetPos(fromSeatPos)
		if fromSeatTrans then
			local resName = "emoji_1_"..emojiID
			local myEmojiContent = self.luaWindowRoot:GetTrans(fromSeatTrans,"myEmojiContent")
			self.luaWindowRoot:SetActive(myEmojiContent,true)

			local emojiGO = WrapSys.ResourceMgr_GetInsAssetInParent(RessType.RT_Emoji, resName, myEmojiContent, RessStorgeType.RST_Never)
			if emojiGO then
				self.luaWindowRoot:SetActive(emojiGO.transform,true)
				emojiGO.transform.position = myEmojiContent.position
				emojiGO.transform.localScale = Vector3.one

				local dwSpriteAnimation = emojiGO:GetComponent("DWSpriteAnimation")
				dwSpriteAnimation.OnFinish = WrapSys.ConversAction(function(_fromSeatPos)
					UIPlayerHeadCtrl:DelMyEmoji(_fromSeatPos,self.luaWindowRoot)
				end,fromSeatPos)
				self:AddMyEmoji(fromSeatPos,dwSpriteAnimation,myEmojiContent)
			end
		end
	else 	--对其他人表情
		local fromSeatTrans = self:GetHeadBySetPos(fromSeatPos)
		local toSeatTrans = self:GetHeadBySetPos(toSeatPos)

		if fromSeatTrans and toSeatTrans then
			local resName = "emoji_2_"..emojiID
			local emojiGO = WrapSys.ResourceMgr_GetInsAssetInParent(RessType.RT_Emoji, resName, self.rootTrans, RessStorgeType.RST_Never)
			if emojiGO then
				self.luaWindowRoot:SetActive(emojiGO.transform,true)
				emojiGO.transform.position = fromSeatTrans.position
			end
			self.luaWindowRoot:PlayTweenPos(emojiGO.transform,fromSeatTrans.position,toSeatTrans.position,0.3,true)
		end
	end

	AudioManager.PlayEmojiAudio(isMy,emojiID)
end
-------------------------------------------------------------------------------------------

function UIPlayerHeadCtrl:Destroy()
	myEmojiAnimations = {}
	UpdateSecond:Remove(self.UpdateSec,self)
	self:UnRegisterEvent()
end

function UIPlayerHeadCtrl:GetHeadPos(isprepare)
	-- TODO::屏蔽准备中位置
	-- if isprepare then
	-- 	return self.headPreparePos
	-- else
	-- 	return self.headPlayPos
	-- end
	return self.headPlayPos
end