--------------------------------------------------------------------------------
-- 	 File      : LA_MJNormalPlayCardLogic.lua
--   author    : zhisong
--   function   : 崇仁麻将玩法
--   date      : 2017年12月22日 17:10:05
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

require "Logic.SystemLogic.MJ.MJNormalPlayCardLogic"

LA_MJNormalPlayCardLogic = class("LA_MJNormalPlayCardLogic", MJNormalPlayCardLogic)

-- function LA_MJNormalPlayCardLogic:GetType()
-- 	return PlayLogicTypeEnum.MJ_Normal
-- end

function LA_MJNormalPlayCardLogic:Init()
	--初始化玩法所需PB
	ProtoManager.InitMJProto(Common_PlayID.leAn_MJ)
	self.super.Init(self)
end

-------------------------------- 网络消息推送 -----------------------------------
-- 网络消息注册
function LA_MJNormalPlayCardLogic:RegisteNetPush()
	self.super.RegisteNetPush(self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD_LEAN_MJ.SC_GAMEPLAY_JIANGMA, self.HandleJiangMaPush, self, 3)
end

-- 网络消息移除
function LA_MJNormalPlayCardLogic:UnRegisteNetPush()
	self.super.UnRegisteNetPush(self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD_LEAN_MJ.SC_GAMEPLAY_JIANGMA, self.HandleJiangMaPush, self)
end

function LA_MJNormalPlayCardLogic:HandleJiangMaPush(rsp, head)
	if rsp and rsp.jiangMaType > 0 then
		WrapSys.EZFunWindowMgr_SetWindowStatusForLua(EZFunWindowEnum.luaWindow, RessType.RT_Window, true, 1, "jiangma_ui_window", rsp)
	end
end

return LA_MJNormalPlayCardLogic