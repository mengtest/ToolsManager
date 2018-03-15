--------------------------------------------------------------------------------
--   File      : LA_WSKRecordPlayCardLogic.lua
--   author    : jianing
--   function   : 乐安510k游戏回放逻辑 
--   date      : 2017-12-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.SystemLogic.WSK.WSKRecordPlayCardLogic"


LA_WSKRecordPlayCardLogic = class("LA_WSKRecordPlayCardLogic", WSKRecordPlayCardLogic)

function LA_WSKRecordPlayCardLogic:ctor()
	ProtoManager.InitWSKProto(Common_PlayID.leAn_510K)
	self:BaseCtor()
end

return LA_WSKRecordPlayCardLogic
