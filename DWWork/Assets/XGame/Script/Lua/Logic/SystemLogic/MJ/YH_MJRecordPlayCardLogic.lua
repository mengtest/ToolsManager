--------------------------------------------------------------------------------
-- 	 File      : CR_WSKRecordPlayCardLogic.lua
--   author    : zx
--   function   : 宜黄麻将游戏回放逻辑 
--   date      : 2018-01-02
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.SystemLogic.MJ.MJRecordPlayCardLogic"

YH_MJRecordPlayCardLogic = class("YH_MJRecordPlayCardLogic", MJRecordPlayCardLogic)

function YH_MJRecordPlayCardLogic:ctor()
	ProtoManager.InitMJProto(Common_PlayID.yiHuang_MJ)
	self:BaseCtor()
end

-- 消息处理
function YH_MJRecordPlayCardLogic:PublicEvent(eventId, rsp)
	self.super.PublicEvent(self, eventId, rsp)
	if eventId == GAME_CMD.SC_GAMEPLAY_JIANGMA then
		self:HandleJiangMaPush(rsp)
	end
end

function YH_MJRecordPlayCardLogic:HandleJiangMaPush(rsp)
	if rsp and rsp.jiangMaType > 0 then
		WrapSys.EZFunWindowMgr_SetWindowStatusForLua(EZFunWindowEnum.luaWindow, RessType.RT_Window, true, 1, "jiangma_ui_window", rsp)
	end
end

return YH_MJRecordPlayCardLogic