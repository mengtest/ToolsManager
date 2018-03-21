--------------------------------------------------------------------------------
--   File      : PlayGameSys.lua
--   author    : guoliang
--   function   : 游戏控制系统 （承载不同玩法有逻辑、网络收发、逻辑消息分发）
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
--require "LuaSys.CommonEnum"
require "Logic.LogicUtil"
PlayGameSys = {}
local _s = PlayGameSys
--网络连接节点名字
_s.netName = "GameLogicNet"

_s.heartInterval = 8
_s.logicType = PlayLogicTypeEnum.None
_s.logicCtrl = nil
_s.common_PlayID = 0
-- 是否注册心跳定时
_s.isRegsiteUpdateSec = false
_s.testRoomID = 0
_s.testIsCreate = false
_s.testIsReconnect = false
_s.is_record = false

_s.PlayMap =
    {
        [PlayLogicTypeEnum.WSK_Normal] =
        {
            [Common_PlayID.chongRen_510K] = "Logic.SystemLogic.WSK.CR_WSKNormalPlayCardLogic",
            [Common_PlayID.leAn_510K] = "Logic.SystemLogic.WSK.LA_WSKNormalPlayCardLogic",
            [Common_PlayID.yiHuang_510K] = "Logic.SystemLogic.WSK.YH_WSKNormalPlayCardLogic",
        },
        [PlayLogicTypeEnum.WSK_Record] =
        {
            [Common_PlayID.chongRen_510K] = "Logic.SystemLogic.WSK.CR_WSKRecordPlayCardLogic",
            [Common_PlayID.leAn_510K] = "Logic.SystemLogic.WSK.LA_WSKRecordPlayCardLogic",
            [Common_PlayID.yiHuang_510K] = "Logic.SystemLogic.WSK.YH_WSKRecordPlayCardLogic",
        },
        [PlayLogicTypeEnum.MJ_Normal] =
        {
            [Common_PlayID.chongRen_MJ] = "Logic.SystemLogic.MJ.CR_MJNormalPlayCardLogic",
            [Common_PlayID.leAn_MJ] = "Logic.SystemLogic.MJ.LA_MJNormalPlayCardLogic",
            [Common_PlayID.yiHuang_MJ] = "Logic.SystemLogic.MJ.YH_MJNormalPlayCardLogic",
        },
        [PlayLogicTypeEnum.MJ_Record] =
        {
            [Common_PlayID.chongRen_MJ] = "Logic.SystemLogic.MJ.CR_MJRecordPlayCardLogic",
            [Common_PlayID.leAn_MJ] = "Logic.SystemLogic.MJ.LA_MJRecordPlayCardLogic",
            [Common_PlayID.yiHuang_MJ] = "Logic.SystemLogic.MJ.YH_MJRecordPlayCardLogic",
        },
        [PlayLogicTypeEnum.ThirtyTwo_Normal] =
        {
            [Common_PlayID.ThirtyTwo] = "Logic.SystemLogic.ThirtyTwo.ThirtyTwoNormalLogic",
        },
        [PlayLogicTypeEnum.ThirtyTwo_Record] = 
        {
            [Common_PlayID.ThirtyTwo] = "Logic.SystemLogic.ThirtyTwo.ThirtyTwoRecordLogic",
        },
		[PlayLogicTypeEnum.DDZ_Normal] = 
		{
			[Common_PlayID.DW_DouDiZhu] = "Area.DouDiZhu.NormalDDZ.Logic.SystemLogic.DDZNormalPlayCardLogic",
		},
		[PlayLogicTypeEnum.DDZ_Normal_Record] = 
		{
			[Common_PlayID.DW_DouDiZhu] = "Area.DouDiZhu.NormalDDZ.Logic.SystemLogic.DDZRecordLogic",
		},
    }
    
function PlayGameSys.Init()
    _s.RegisterEventHandle()
end

function PlayGameSys.Destroy()
    _s.UnRegisterEventHandle()
end

--设置玩法逻辑
function PlayGameSys.SelectPlayLogicType(logicType, play_id)
    if logicType then
        if _s.logicType ~= logicType then
            if _s.logicCtrl then
                _s.logicCtrl:Destroy()
                _s.logicCtrl = nil
            end
            _s.logicType = logicType
            local playLogicType = _s.PlayMap[_s.logicType]
            if nil == playLogicType then
                error("no found play logic type")
                return
            else
                local playPath = playLogicType[play_id]
                if nil == playPath then
                    error("no found play logic play id" .. play_id)
                    return
                else
                    local logicCtrl = require(playPath)
                    _s.logicCtrl = logicCtrl.New()
                    _s.common_PlayID = play_id
                end
            end
            if _s.logicCtrl then
                _s.logicCtrl:Init()
            end
        end
    else
        WindowUtil.LuaShowTips("找不到玩法")
    end
end

function PlayGameSys.ReleasePlayLogic()
    if _s.logicCtrl then
        _s.logicCtrl:Destroy()
        _s.logicCtrl = nil
        _s.logicType = PlayLogicTypeEnum.None
    end
end

function PlayGameSys.GetPlayLogic()
    return _s.logicCtrl
end

function PlayGameSys.GetPlayLogicType()
    return _s.logicType
end

--Event消息注册
function PlayGameSys.RegisterEventHandle()
    LuaEvent.AddHandle(EEventType.NetReconnectStart, _s.NetReconnectStart, nil)
    LuaEvent.AddHandle(EEventType.GameSvrConnectSuc, _s.ConnectSvrSuccess, nil)
    LuaEvent.AddHandle(EEventType.NetReconnectSuccess, _s.NetReConnectSuccess, nil)
    LuaEvent.AddHandle(EEventType.NetResetAutoConnect, _s.NetResetAutoConnect, nil)
    LuaEvent.AddHandle(EEventType.ApplicationAwakeEvent, _s.HandleApplicationAwake, nil)
end

function PlayGameSys.UnRegisterEventHandle()
    LuaEvent.RemoveHandle(EEventType.NetReconnectStart, _s.NetReconnectStart)
    LuaEvent.RemoveHandle(EEventType.GameSvrConnectSuc, _s.ConnectSvrSuccess)
    LuaEvent.RemoveHandle(EEventType.NetReconnectSuccess, _s.NetReConnectSuccess)
    LuaEvent.RemoveHandle(EEventType.NetResetAutoConnect, _s.NetResetAutoConnect, nil)
    LuaEvent.RemoveHandle(EEventType.ApplicationAwakeEvent, _s.HandleApplicationAwake, nil)
end

-- 网络连接 start
--正常连接游戏服务器
function PlayGameSys.ConnectSvr(netName,ip, port, IsReconnect)
    if netName == _s.netName then
        DwDebug.Log("PlayGameSys ConnectSvr start")
        --LuaEvent.AddEventNow(EEventType.ShowWaitWindow, true)
        WrapSys.EZFunWindowMgr_SetWindowStatusForLua(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, true, 20000+10, "wait_ui_window" , IsReconnect)
        WrapSys.CNetSys_ConnectSvr(netName,ip, port)
    end
end

--连接游戏服务器成功(断网和正常流程都会调用)
function PlayGameSys.ConnectSvrSuccess(eventId, p1, p2)
    local isSucces = p1
    local netName = p2
    if netName == _s.netName then
        if isSucces then
            DwDebug.Log("PlayGameSys ConnectSvr success")
            _s.logicCtrl:SendLoginSvr()
        else
            LuaEvent.AddEventNow(EEventType.UI_CreateRoomResult, false)
            WindowUtil.LuaShowTips("连接服务器失败，请稍候再试")
        end
    end
end

--断线重连开始开始
function PlayGameSys.NetReconnectStart(eventId, p1, p2)
    local netName = p1
    if netName == _s.netName then
        _s.RemoveHeartBeat()
        --http发送请求ip和port,走重连的流程
        if _s.logicCtrl then
            if HallSys.isCreateOrJoinRooming == true then
                HallSys.isCreateOrJoinRooming = false
                WindowUtil.LuaShowTips("请求数据失败，请稍候重试",1)
                --放弃此次创建或者加入房间流程
                LoginSys.ResetLoginStatus()
                _s.ReleasePlayLogic()
                _s.CloseNetWork()
                return   
            end


            --正常重连流程
            _s.logicCtrl.isLoginSvrSuc = false
            _s.logicCtrl.isReconnect = true
            _s.logicCtrl:EnableStartLoginingFlag(false)
            LoginSys.ResetLoginStatus()
            LoginSys.Login(100, true)
        else --弱网情况创建房间时几率出现
            LoginSys.ResetLoginStatus()
            _s.CloseNetWork()
        end
    end
end

--断线重连成功
function PlayGameSys.NetReConnectSuccess(eventId, p1, p2)
end

--重置自动重连
function PlayGameSys.NetResetAutoConnect(eventId,p1,p2)
    DwDebug.LogError("PlayGameSys.NetResetAutoConnect "..p1)
    local netName = p1
    if netName == _s.netName then
        --特殊情况过滤
        --如果加入房间或者创建房间从web端拉取ip和port成功，但是连接未成功或者未向游戏服发送login时断网
        --未登录验证时，web服不会认为玩家是加入了房间的，虽然连接上了游戏服
        if _s.logicCtrl then
            if HallSys.isCreateOrJoinRooming == true  then
                HallSys.isCreateOrJoinRooming = false
                WindowUtil.LuaShowTips("请求数据失败，请稍候重试",1)
                --放弃此次创建或者加入房间流程
                LoginSys.ResetLoginStatus()
                _s.ReleasePlayLogic()
                _s.CloseNetWork()
                return   
            end

            WrapSys.CNetSys_ResetAutoConnectt(netName)
        end          
    end
end


--应用唤醒(正常打牌中需要断网重连)
function PlayGameSys.HandleApplicationAwake(eventId, p1, p2)
    local isAwake = p1
    if isAwake then
        if _s.logicCtrl then
            if _s.logicType == PlayLogicTypeEnum.WSK_Normal 
                or _s.logicType == PlayLogicTypeEnum.MJ_Normal 
                or _s.logicType == PlayLogicTypeEnum.ThirtyTwo_Normal
				or _s.logicType == PlayLogicTypeEnum.DDZ_Normal then
                if _s.logicCtrl.roomObj and _s.logicCtrl.roomObj.roomStateMgr then
                    local state = _s.logicCtrl.roomObj.roomStateMgr:GetCurStateType()
                    local bigResult = _s.logicCtrl.roomObj:GetBigResult()
                    local isAllRoundEnd = _s.logicCtrl.isAllRoundEnd
                    if bigResult == nil and state ~= RoomStateEnum.GameOver and not isAllRoundEnd then
                        WrapSys.CNetSys_ResetAutoConnectt(LuaNetWork.gameNetName)
                    end
                end
            end
        end
    end
end
--end
function PlayGameSys.UpdateSec()
    _s.heartTime = _s.heartTime + 1
    if _s.heartTime >= _s.heartInterval then
        _s.heartTime = 0
        _s.logicCtrl:SendHeartBeat()
    end
end

-- 开始游戏
function PlayGameSys.StartGame(play_id, ip, port, roomId, IsReconnect)
    DataManager.SetCurPlayID(play_id)
    LuaNetWork.SetPlayID(play_id)
    _s.InitGameContextByPlayID(play_id)
    _s.is_record = false
    _s.SelectPlayLogicType(_s.GetPlayLogicTypeEnum(play_id), play_id)
    _s.common_PlayID = play_id
    if _s.logicCtrl then
        DwDebug.Log("StartGame roomId" .. roomId)
        _s.logicCtrl.isLoginSvrSuc = false
        _s.logicCtrl.roomId = roomId
        _s.logicCtrl.isCreate = (roomId == 0 or roomId == nil)
        _s.logicCtrl.isReconnect = IsReconnect
    end
    _s.ConnectSvr(_s.netName, ip, port, IsReconnect)
end

-- 播放战绩回放
function PlayGameSys.GoToRecordGame(play_id)
    DataManager.SetCurPlayID(play_id)
    _s.common_PlayID = play_id
    _s.is_record = true
    _s.InitGameContextByPlayID(play_id)
    if GameStateMgr.GetCurStateType() == EGameStateType.GameState then

    else
        _s.SelectPlayLogicType(_s.GetPlayLogicTypeEnum(play_id, true), play_id)
        GameStateMgr.GoToGameScene()
    end
end

-- 退出到游戏大厅
function PlayGameSys.QuitToMainCity()
    _s.QuitToMainCityNoNet()   

    --回到大厅时同步连接通讯服，连接成功后刷新大厅状态
    UserNetSys.StartUseNetWork()
end

-- 仅回到大厅不做通讯服连接
function PlayGameSys.QuitToMainCityNoNet()
    _s.ReleasePlayLogic()
    _s.CloseNetWork()
    GameStateMgr.GoToMainScene()
end

-- 退出到登录场景（功能函数，外部不能直接调用） 账号登出需要调用LoginSys 的Logout
function PlayGameSys.QuitToLoginScene()
    LoginSys.ResetLoginStatus()
    _s.ReleasePlayLogic()
    _s.CloseNetWork()
    WrapSys.NetSys_SetForceOut(true)
    GameStateMgr.GoToLoginScene()
end

function PlayGameSys.RemoveHeartBeat()
    if _s.isRegsiteUpdateSec then
        DwDebug.Log("RemoveHeartBeat suc")
        _s.isRegsiteUpdateSec = false
        UpdateSecond:Remove(_s.UpdateSec, nil)
    end
end

function PlayGameSys.RegisterHeartBeat()
    if not _s.isRegsiteUpdateSec then
        DwDebug.Log("RegisterHeartBeat suc")
        _s.heartTime = 0
        _s.isRegsiteUpdateSec = true
        UpdateSecond:Add(_s.UpdateSec, nil)
    end
end

function PlayGameSys.CloseNetWork()
    DwDebug.Log("PlayGameSys CloseNetWork")
    WrapSys.CNetSys_GiveUpAutoConnect(_s.netName)
    _s.RemoveHeartBeat()
end

function PlayGameSys.GetPlayLogicTypeEnum(play_id, isRecord)
    if play_id == Common_PlayID.chongRen_510K
        or play_id == Common_PlayID.leAn_510K
        or play_id == Common_PlayID.yiHuang_510K then
        if isRecord then
            return PlayLogicTypeEnum.WSK_Record
        else
            return PlayLogicTypeEnum.WSK_Normal
        end
    elseif play_id == Common_PlayID.chongRen_MJ 
        or play_id == Common_PlayID.leAn_MJ
        or play_id == Common_PlayID.yiHuang_MJ then
        if isRecord then
            return PlayLogicTypeEnum.MJ_Record
        else
            return PlayLogicTypeEnum.MJ_Normal
        end
    elseif play_id == Common_PlayID.ThirtyTwo then
        if isRecord then
            return PlayLogicTypeEnum.ThirtyTwo_Record
        else
            return PlayLogicTypeEnum.ThirtyTwo_Normal
        end	
	elseif play_id == Common_PlayID.DW_DouDiZhu then
		if isRecord then
			return PlayLogicTypeEnum.DDZ_Normal_Record
		else
			return PlayLogicTypeEnum.DDZ_Normal
		end
    end
end

-- 获取是否回放状态
function PlayGameSys.GetIsRecord()
--    DwDebug.LogError("GetIsRecord " .. ToString(_s.is_record))
    return _s.is_record
end

function PlayGameSys.GetCardKindType()
    local cardKindType = CardKindType.PK
    if _s.logicType == PlayLogicTypeEnum.MJ_Normal
        or _s.logicType == PlayLogicTypeEnum.MJ_Record then
        cardKindType = CardKindType.MJ
    end
    return cardKindType
end

function PlayGameSys.InitGameContextByPlayID(play_id)
    if play_id == Common_PlayID.chongRen_MJ 
        or play_id == Common_PlayID.leAn_MJ
        or play_id == Common_PlayID.yiHuang_MJ then
        setmetatable(RoomStateEnum, MJRoomStateEnum)
    elseif play_id == Common_PlayID.chongRen_510K
        or play_id == Common_PlayID.leAn_510K
        or play_id == Common_PlayID.yiHuang_510K then
        setmetatable(RoomStateEnum, PKRoomStateEnum)
    elseif play_id == Common_PlayID.ThirtyTwo then
        setmetatable(RoomStateEnum, ThirtyTwoRoomStateEnum)
	elseif play_id == Common_PlayID.DW_DouDiZhu then
		setmetatable(RoomStateEnum, DDZRoomStateEnum)
    else
        setmetatable(RoomStateEnum, MJRoomStateEnum)
    end
end
--通过玩家ID获取玩家信息
function PlayGameSys.GetPlayerByPlayerID(playId)
    if _s.logicCtrl and _s.logicCtrl.roomObj and _s.logicCtrl.roomObj.playerMgr then
        return _s.logicCtrl.roomObj.playerMgr:GetPlayerByPlayerID(playId)
    end
    return nil
end

--当前的玩法id
function PlayGameSys.GetNowPlayId()
    return _s.common_PlayID
end
