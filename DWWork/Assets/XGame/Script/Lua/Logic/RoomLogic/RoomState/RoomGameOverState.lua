--------------------------------------------------------------------------------
--   File      : RoomGameOverState.lua
--   author    : guoliang
--   function   : 房间找朋友状态
--   date      : 2017-09-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
RoomGameOverState = class("RoomGameOverState",BaseObjectState)
local total_settlement_openEnumPre =
{
    [Common_PlayID.chongRen_MJ] = 10,
    [Common_PlayID.leAn_MJ] = 20,
    [Common_PlayID.yiHuang_MJ] = 30,
    [Common_PlayID.chongRen_510K] = 10,
    [Common_PlayID.leAn_510K] = 20,
    [Common_PlayID.yiHuang_510K] = 30,
    [Common_PlayID.ThirtyTwo] = 40,
	[Common_PlayID.DW_DouDiZhu] = 50,
}
function RoomGameOverState:ctor()
end
function RoomGameOverState:Init(parent)
	self.parent = parent
end
function RoomGameOverState:Enter(data)
	local play_logic = PlayGameSys.GetPlayLogic()
    local play_id = PlayGameSys.GetNowPlayId()
    local is_record = PlayGameSys.GetIsRecord()
    -- 获取房间类型
    local openData = total_settlement_openEnumPre[play_id] + (is_record and 1 or 0)
    local kind = PlayGameSys.GetCardKindType()
    local kind_str = nil
    if kind == CardKindType.MJ then
        kind_str = "mj"
    elseif kind == CardKindType.PK then
        kind_str = "pk"
    end
    if not kind_str then
        DwDebug.Log("kind_str error")
        return
    end
    local ui_str = kind_str .. "_total_settlement_ui_window"
	WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, openData, ui_str, false, nil)
end
function RoomGameOverState:Leave()
end
function RoomGameOverState:GetType()
	return RoomStateEnum.GameOver
end