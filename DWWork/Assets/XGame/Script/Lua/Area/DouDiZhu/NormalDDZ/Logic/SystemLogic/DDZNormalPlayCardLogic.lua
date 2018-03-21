--------------------------------------------------------------------------------
-- 	 File       : DDZNormalPlayCardLogic.lua
--   author     : zhanghaochun
--   function   : 斗地主正常玩法基类
--   date       : 2018-01-30
--   copyright  : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

require "Area.DouDiZhu.NormalDDZ.Logic.SystemLogic.DDZPlayCardLogic"

DDZNormalPlayCardLogic = class("DDZNormalPlayCardLogic", DDZPlayCardLogic)

function DDZNormalPlayCardLogic:ctor()
	self.super:ctor()
	ProtoManager.InitDDZProto(Common_PlayID.DW_DouDiZhu)
	self:InitSortTypef()
end

----------------------关于斗地主排序类型--------------------------
function DDZNormalPlayCardLogic:InitSortTypef()
    -- 1 : 按照降序排列  2 : 按照炸弹大小排列
    self.wskSortType = PlayerDataRefUtil.GetInt("DDZ_SortType", 1)
end

function DDZNormalPlayCardLogic:SetSortType(st)
	if st == self.wskSortType then return end
    
    self.wskSortType = st
    PlayerDataRefUtil.SetInt("DDZ_SortType", st)
end

function DDZNormalPlayCardLogic:GetSortType()
	return self.wskSortType
end

return DDZNormalPlayCardLogic

---------------------------------------------------------------