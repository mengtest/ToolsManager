--------------------------------------------------------------------------------
-- 	 File      : CR_WSKRecordPlayCardLogic.lua
--   author    : zx
--   function   : 乐安麻将游戏回放逻辑 
--   date      : 2018-01-02
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.SystemLogic.MJ.MJRecordPlayCardLogic"

LA_MJRecordPlayCardLogic = class("LA_MJRecordPlayCardLogic", MJRecordPlayCardLogic)

function LA_MJRecordPlayCardLogic:ctor()
	ProtoManager.InitMJProto(Common_PlayID.leAn_MJ)
	self:BaseCtor()
end

-- 消息处理
function LA_MJRecordPlayCardLogic:PublicEvent(eventId, rsp)
	self.super.PublicEvent(self, eventId, rsp)
	if eventId == GAME_CMD.SC_GAMEPLAY_JIANGMA then
		self:HandleJiangMaPush(rsp)
	end
end

function LA_MJRecordPlayCardLogic:HandleJiangMaPush(rsp)
	if rsp and rsp.jiangMaType > 0 then
		WrapSys.EZFunWindowMgr_SetWindowStatusForLua(EZFunWindowEnum.luaWindow, RessType.RT_Window, true, 1, "jiangma_ui_window", rsp)
	end
end

--房间不存在的错误码
function LA_MJRecordPlayCardLogic:IsNotRoomErrno(errno)
	return errno == 207
end

return LA_MJRecordPlayCardLogic