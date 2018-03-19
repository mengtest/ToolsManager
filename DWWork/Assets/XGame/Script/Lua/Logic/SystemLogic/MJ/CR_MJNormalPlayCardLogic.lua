--------------------------------------------------------------------------------
-- 	 File      : CR_MJNormalPlayCardLogic.lua
--   author    : zhisong
--   function   : 崇仁麻将玩法
--   date      : 2017年12月22日 17:10:05
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

require "Logic.SystemLogic.MJ.MJNormalPlayCardLogic"

CR_MJNormalPlayCardLogic = class("CR_MJNormalPlayCardLogic", MJNormalPlayCardLogic)

-- function CR_MJNormalPlayCardLogic:GetType()
-- 	return PlayLogicTypeEnum.MJ_Normal
-- end

function CR_MJNormalPlayCardLogic:Init()
	--初始化玩法所需PB
	ProtoManager.InitMJProto(Common_PlayID.chongRen_MJ)
	self.super.Init(self)
end
--房间不存在的错误码
function CR_MJNormalPlayCardLogic:IsNotRoomErrno(errno)
	return errno == 7
end

return CR_MJNormalPlayCardLogic