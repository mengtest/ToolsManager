--------------------------------------------------------------------------------
-- 	 File      : CR_WSKRecordPlayCardLogic.lua
--   author    : zx
--   function   : 崇仁麻将游戏回放逻辑 
--   date      : 2018-01-02
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.SystemLogic.MJ.MJRecordPlayCardLogic"

CR_MJRecordPlayCardLogic = class("CR_MJRecordPlayCardLogic", MJRecordPlayCardLogic)

function CR_MJRecordPlayCardLogic:ctor()
	ProtoManager.InitMJProto(Common_PlayID.chongRen_MJ)
	self:BaseCtor()
end

return CR_MJRecordPlayCardLogic