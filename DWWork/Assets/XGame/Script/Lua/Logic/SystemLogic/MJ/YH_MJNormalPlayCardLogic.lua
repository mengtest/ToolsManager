--------------------------------------------------------------------------------
-- 	 File      : YH_MJNormalPlayCardLogic.lua
--   author    : zhisong
--   function   : 崇仁麻将玩法
--   date      : 2017年12月22日 17:10:05
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

require "Logic.SystemLogic.MJ.MJNormalPlayCardLogic"

YH_MJNormalPlayCardLogic = class("YH_MJNormalPlayCardLogic", MJNormalPlayCardLogic)

-- function YH_MJNormalPlayCardLogic:GetType()
-- 	return PlayLogicTypeEnum.MJ_Normal
-- end

function YH_MJNormalPlayCardLogic:Init()
	--初始化玩法所需PB
	ProtoManager.InitMJProto(Common_PlayID.yiHuang_MJ)
	self.super.Init(self)
end

-------------------------------- 网络消息推送 -----------------------------------
-- 网络消息注册
function YH_MJNormalPlayCardLogic:RegisteNetPush()
	self.super.RegisteNetPush(self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD_YIHUANG_MJ.SC_GAMEPLAY_JIANGMA, self.HandleJiangMaPush, self, function(rsp,head) return (rsp and rsp.jiangMaType and rsp.jiangMaType > 0) and 3 or 0 end)
end

-- 网络消息移除
function YH_MJNormalPlayCardLogic:UnRegisteNetPush()
	self.super.UnRegisteNetPush(self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD_YIHUANG_MJ.SC_GAMEPLAY_JIANGMA, self.HandleJiangMaPush, self)
end

function YH_MJNormalPlayCardLogic:HandleJiangMaPush(rsp, head)
	if rsp and rsp.jiangMaType > 0 then
		WrapSys.EZFunWindowMgr_SetWindowStatusForLua(EZFunWindowEnum.luaWindow, RessType.RT_Window, true, 1, "jiangma_ui_window", rsp)
	end
end

--房间不存在的错误码
function YH_MJNormalPlayCardLogic:IsNotRoomErrno(errno)
	return errno == 307
end

return YH_MJNormalPlayCardLogic
