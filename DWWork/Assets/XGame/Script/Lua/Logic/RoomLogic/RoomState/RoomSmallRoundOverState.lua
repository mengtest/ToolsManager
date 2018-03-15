--------------------------------------------------------------------------------
--   File    : RoomSmallRoundOverState.lua
--   author : guoliang
--   function   : 房间小局结束状态
--   date    : 2017-09-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
RoomSmallRoundOverState = class("RoomSmallRoundOverState", BaseObjectState)

local single_settlement_openEnumPre =
{
    [Common_PlayID.chongRen_MJ] = 10,
    [Common_PlayID.leAn_MJ] = 20,
    [Common_PlayID.yiHuang_MJ] = 30,
    [Common_PlayID.chongRen_510K] = 10,
    [Common_PlayID.leAn_510K] = 20,
    [Common_PlayID.yiHuang_510K] = 30,
    -- [Common_PlayID.ThirtyTwo] = -100,
}

function RoomSmallRoundOverState:ctor()
end

function RoomSmallRoundOverState:Init(parent)
    self.parent = parent
end

function RoomSmallRoundOverState:Enter(data)
    --更新当前房间信息
    local play_logic = PlayGameSys.GetPlayLogic()
    local smallResult = play_logic.roomObj:GetCurrSmallResult()
    local play_id = PlayGameSys.GetNowPlayId()
    local is_record = PlayGameSys.GetIsRecord()
    DwDebug.Log("play_id "..tostring(play_id))
    DwDebug.Log("is_record "..tostring(is_record))

	LuaEvent.AddEventNow(EEventType.RoomSmallRoundEvent, smallResult)
    -- 32张
    if play_id == Common_PlayID.ThirtyTwo then
        -- 小结算事件
        LuaEvent.AddEventNow(EEventType.PK32_SingleSettlement,smallResult)
        return
    end
	-- 斗地主
	if play_id == Common_PlayID.DW_DouDiZhu then
        -- 机器人有可能已经准备了
        -- 玩家切换到准备
        play_logic.roomObj.playerMgr:AllPlayerIdle()
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "CommonProduct.DouDiZhu.UIWindow.ddz_small_summary_ui_window", false, nil)
		return
	end
    -- 获取房间类型
    local openData = single_settlement_openEnumPre[play_id] + (is_record and 1 or 0)

    local noSelf = false
    local kind = PlayGameSys.GetCardKindType()
    local ui_str = nil
    if kind == CardKindType.MJ then
        if is_record then
            ui_str = "mj_single_settlement_ui_window"
        else
            ui_str = "mj_round_over_ui_window"
            noSelf = true
        end
        
    elseif kind == CardKindType.PK then
        ui_str = "pk_single_settlement_ui_window"
    end

    if nil == ui_str then
        DwDebug.Log("kind : "..tostring(ui_str))
        return
    end
    -- local ui_str = kind_str .. "_single_settlement_ui_window"
    -- DwDebug.Log("ui_str : "..tostring(ui_str))
    if smallResult then
        -- 机器人有可能已经准备了
        -- 玩家切换到准备
        play_logic.roomObj.playerMgr:AllPlayerIdle(noSelf)
        --打开结算UI
        WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, openData, ui_str, false, nil)
        --清理操作面板
        LuaEvent.AddEventNow(EEventType.MJOpeTipHideUI)
    else
        -- 没有数据就切换到空闲状态啊
        self.parent:ChangeState(RoomStateEnum.Idle)
    end
end

function RoomSmallRoundOverState:Leave()
-- if WrapSys.EZFunWindowMgr_CheckWindowOpen(EZFunWindowEnum.luaWindow, "single_settlement_ui_window") then
--  WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, false, 1, "single_settlement_ui_window", false , nil)
-- end
end

function RoomSmallRoundOverState:GetType()
    return RoomStateEnum.SmallRoundOver
end
