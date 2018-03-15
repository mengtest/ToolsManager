--------------------------------------------------------------------------------
-- 	 File      : CR_WSKRecordPlayCardLogic.lua
--   author    : jianing
--   function   : 崇仁510k游戏回放逻辑 
--   date      : 2017-12-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.SystemLogic.WSK.WSKRecordPlayCardLogic"

CR_WSKRecordPlayCardLogic = class("CR_WSKRecordPlayCardLogic", WSKRecordPlayCardLogic)

function CR_WSKRecordPlayCardLogic:ctor()
	ProtoManager.InitWSKProto(Common_PlayID.chongRen_510K)
	self:BaseCtor()
end

return CR_WSKRecordPlayCardLogic