--------------------------------------------------------------------------------
-- 	 File      : LA_WSKNormalPlayCardLogic.lua
--   author    : jianing
--   function  : 乐安打盾
--   date      : 2017-12-22
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.SystemLogic.WSK.WSKNormalPlayCardLogic"

LA_WSKNormalPlayCardLogic = class("LA_WSKNormalPlayCardLogic", WSKNormalPlayCardLogic)

function LA_WSKNormalPlayCardLogic:ctor()
	self.super:ctor()
	self:InitSortTypef()
end

function LA_WSKNormalPlayCardLogic:Init()
	--初始化玩法所需PB
	ProtoManager.InitWSKProto(Common_PlayID.leAn_510K)
	self.super.Init(self)
end

--处理通用错误码
function LA_WSKNormalPlayCardLogic:HanleCommonError(eventId,p1,p2)
	if p1 == Common_PlayID.leAn_510K then
		if p2 == 10350 then	 --出牌不符合规则
			--出牌返回错误 重连刷新牌
			WrapSys.CNetSys_ResetAutoConnectt(LuaNetWork.gameNetName)
		end
	end
end

--房间不存在的错误码
function LA_WSKNormalPlayCardLogic:IsNotRoomErrno(errno)
	return errno == 10313
end

----------------------关于五十K排序类型--------------------------
function LA_WSKNormalPlayCardLogic:InitSortTypef()
    -- 1 : 按照降序排列  2 : 按照炸弹大小排列
    self.wskSortType = PlayerDataRefUtil.GetInt("LA_WSK_SortType", 1)
end

function LA_WSKNormalPlayCardLogic:SetSortType(st)
	if st == self.wskSortType then return end
    
    self.wskSortType = st
    PlayerDataRefUtil.SetInt("LA_WSK_SortType", st)
end

function LA_WSKNormalPlayCardLogic:GetSortType()
	return self.wskSortType
end

return LA_WSKNormalPlayCardLogic

---------------------------------------------------------------
