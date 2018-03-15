--------------------------------------------------------------------------------
-- 	 File      : CR_WSKNormalPlayCardLogic.lua
--   author    : jianing
--   function  : 崇仁510K
--   date      : 2017-12-22
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

require "Logic.SystemLogic.WSK.WSKNormalPlayCardLogic"

CR_WSKNormalPlayCardLogic = class("CR_WSKNormalPlayCardLogic", WSKNormalPlayCardLogic)

function CR_WSKNormalPlayCardLogic:ctor()
	ProtoManager.InitWSKProto(Common_PlayID.chongRen_510K)
	self.super:ctor()
	self:InitSortType()
end

--处理通用错误码
function CR_WSKNormalPlayCardLogic:HanleCommonError(eventId,p1,p2)
	if p1 == Common_PlayID.chongRen_510K then
		if p2 == 10050 then	 --出牌不符合规则
			--出牌返回错误 重连刷新牌
			WrapSys.CNetSys_ResetAutoConnectt(LuaNetWork.gameNetName)
		end
	end
end

--房间不存在的错误码
function CR_WSKNormalPlayCardLogic:IsNotRoomErrno(errno)
	return errno == 10013
end

----------------------关于五十K排序类型--------------------------
function CR_WSKNormalPlayCardLogic:InitSortType()
	-- 1 : 按照降序排列  2 : 按照炸弹大小排列
    self.wskSortType = PlayerDataRefUtil.GetInt("WSK_SortType", 1)
end

function CR_WSKNormalPlayCardLogic:SetSortType(st)
	if st == self.wskSortType then return end
    
    self.wskSortType = st
    PlayerDataRefUtil.SetInt("WSK_SortType", st)
end

function CR_WSKNormalPlayCardLogic:GetSortType()
	return self.wskSortType
end
---------------------------------------------------------------
return CR_WSKNormalPlayCardLogic