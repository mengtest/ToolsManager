--------------------------------------------------------------------------------
-- 	 File      : YH_WSKRecordPlayCardLogic.lua
--   author    : jianing
--   function   : 宜黄510k游戏回放逻辑 
--   date      : 2017-12-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.SystemLogic.WSK.WSKRecordPlayCardLogic"

YH_WSKRecordPlayCardLogic = class("YH_WSKRecordPlayCardLogic", WSKRecordPlayCardLogic)

function YH_WSKRecordPlayCardLogic:ctor()
	ProtoManager.InitWSKProto(Common_PlayID.yiHuang_510K)
	self:BaseCtor()
end

return YH_WSKRecordPlayCardLogic