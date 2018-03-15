--------------------------------------------------------------------------------
-- 	 File      : YH_WSKNormalPlayCardLogic.lua
--   author    : jianing
--   function  : 宜黄红心5
--   date      : 2017-12-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.SystemLogic.WSK.WSKNormalPlayCardLogic"
require "Logic.CardLogic.LogicContainer.WSK.YH_WSKCardPlayLogicContainer"

YH_WSKNormalPlayCardLogic = class("YH_WSKNormalPlayCardLogic", WSKNormalPlayCardLogic)

function YH_WSKNormalPlayCardLogic:ctor()
	self.super:ctor()
	self:InitSortTypef()
end

function YH_WSKNormalPlayCardLogic:Init()
	--初始化玩法所需PB
	ProtoManager.InitWSKProto(Common_PlayID.yiHuang_510K)

	self:BaseInit()
	--房间管理
	self.roomObj = CWSKRoom.New()
	self.roomObj:Init(self)
	-- 玩牌逻辑容器
	self.cardLogicContainer = YH_WSKCardPlayLogicContainer.New()
	self.cardLogicContainer:Init()

	--初始化数据
	local WSKData = DataManager.WSKData
	if WSKData then
		self.isAllOpen = WSKData.isAllOpen
		self.roundNum = WSKData.roundNum
		self.payType = WSKData.payType
		self.msgGrouperId = WSKData.msgGrouperId
		self.msgGroupId = WSKData.msgGroupId
	end
end

--房间不存在的错误码
function YH_WSKNormalPlayCardLogic:IsNotRoomErrno(errno)
	return errno == 10413
end

--处理通用错误码
function YH_WSKNormalPlayCardLogic:HanleCommonError(eventId,p1,p2)
	if p1 == Common_PlayID.yiHuang_510K then
		if p2 == 10450 then	 --出牌不符合规则
			--出牌返回错误 重连刷新牌
			WrapSys.CNetSys_ResetAutoConnectt(LuaNetWork.gameNetName)
		end
	end
end

 --获取是否多飞
function YH_WSKNormalPlayCardLogic:GetDuoFei()
	if self.roomObj and self.roomObj.roomInfo then
		return self.roomObj.roomInfo.canPlanes
	end
	return false
end

----------------------关于五十K排序类型--------------------------
function YH_WSKNormalPlayCardLogic:InitSortTypef()
    -- 1 : 按照降序排列  2 : 按照炸弹大小排列
    self.wskSortType = PlayerDataRefUtil.GetInt("YH_WSK_SortType", 1)
end

function YH_WSKNormalPlayCardLogic:SetSortType(st)
	if st == self.wskSortType then return end
    
    self.wskSortType = st
    PlayerDataRefUtil.SetInt("YH_WSK_SortType", st)
end

function YH_WSKNormalPlayCardLogic:GetSortType()
	return self.wskSortType
end

return YH_WSKNormalPlayCardLogic
---------------------------------------------------------------
