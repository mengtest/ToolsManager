--------------------------------------------------------------------------------
--   File      : UserNetSys.lua
--   author    : guoliang
--   function   : 用户通讯服系统
--   date      : 2018-02-02
--   copyright : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------

UserNetSys = {}
local _s = UserNetSys
--网络连接节点名字
_s.netName = "UserLogicNet"
--心跳间隔
_s.heartInterval = 8
--心跳计数
_s.heartTime = 0
-- 是否注册心跳定时
_s.isRegsiteUpdateSec = false

--是否注册web请求失败重连定时
_s.isRegisterWebFailAutoConnectBeat = false
--重连间隔
_s.autoConnectInterval = 8
--重连计数
_s.autoCountTime = 0

--是否登录成功
_s.isLoginSvrSuc = false

--是否进入工作状态
_s.isWorking = false

function UserNetSys.Init()
    _s.RegisterEventHandle()
    WrapSys.CNetSys_AddNetConnectInstance(_s.netName)
end

function UserNetSys.Destroy()
    _s.UnRegisterEventHandle()
     WrapSys.CNetSys_RemoveNetConnectInstance(_s.netName)
end


--Event消息注册
function UserNetSys.RegisterEventHandle()
    LuaEvent.AddHandle(EEventType.NetReconnectStart, _s.NetReconnectStart, nil)
    LuaEvent.AddHandle(EEventType.GameSvrConnectSuc, _s.ConnectSvrSuccess, nil)
    LuaEvent.AddHandle(EEventType.NetReconnectSuccess, _s.NetReConnectSuccess, nil)
    LuaEvent.AddHandle(EEventType.NetResetAutoConnect, _s.NetResetAutoConnect, nil)
    LuaEvent.AddHandle(EEventType.ApplicationAwakeEvent, _s.HandleApplicationAwake, nil)
end

function UserNetSys.UnRegisterEventHandle()
    LuaEvent.RemoveHandle(EEventType.NetReconnectStart, _s.NetReconnectStart)
    LuaEvent.RemoveHandle(EEventType.GameSvrConnectSuc, _s.ConnectSvrSuccess)
    LuaEvent.RemoveHandle(EEventType.NetReconnectSuccess, _s.NetReConnectSuccess)
    LuaEvent.RemoveHandle(EEventType.NetResetAutoConnect, _s.NetResetAutoConnect, nil)
    LuaEvent.RemoveHandle(EEventType.ApplicationAwakeEvent, _s.HandleApplicationAwake, nil)
end


function UserNetSys.EnableWork(isWorking)
    _s.isWorking = isWorking
end

-- 网络连接 start
--正常连接游戏服务器
function UserNetSys.ConnectSvr(netName,ip, port)
    if netName == _s.netName then
        DwDebug.Log("UserNetSys ConnectSvr")
 --       LuaEvent.AddEventNow(EEventType.ShowWaitWindow, true)
        WrapSys.CNetSys_ConnectSvr(netName,ip, port)
    end
end

--连接游戏服务器成功(断网和正常流程都会调用)
function UserNetSys.ConnectSvrSuccess(eventId, p1, p2)
    local isSucces = p1
    local netName = p2
    if netName == _s.netName then
        if isSucces then
             DwDebug.Log("UserNetSys ConnectSvr success")
            -- 发送登录
            _s.SendLoginSvr()
            --to add
        else
            WindowUtil.LuaShowTips("连接服务器失败，请稍候再试")
        end
    end
end

--断线重连开始开始
function UserNetSys.NetReconnectStart(eventId, p1, p2)
    local netName = p1
    if netName == _s.netName then
        DwDebug.LogError("UserNetSys.NetReconnectStart")
        --是否http发送请求ip和port,走重连的流程 待确定
        _s.StartUseNetWork()
    end
end

--断线重连成功
function UserNetSys.NetReConnectSuccess(eventId, p1, p2)
end


--重置自动重连
function UserNetSys.NetResetAutoConnect(eventId,p1,p2)
    local netName = p1
    if netName == _s.netName then
         DwDebug.LogError("UserNetSys.NetResetAutoConnect")
        WrapSys.CNetSys_ResetAutoConnectt(netName)
    end
end

--应用唤醒(大厅需要断网重连)
function UserNetSys.HandleApplicationAwake(eventId, p1, p2)
    local isAwake = p1
    if isAwake then
        if GameStateMgr.GetCurStateType() == EGameStateType.MainCityState then
            WrapSys.CNetSys_ResetAutoConnectt(_s.netName)
        end
    end
end


--用户通讯服连接开始工作
function UserNetSys.StartUseNetWork()
    _s.CloseNetWork()
    _s.EnableWork(true)
--    WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, true, 20000+10, "wait_ui_window")
    WebNetHelper.RequestUserNetBaseInfo(function (body,head)
        if body and body.data then
            _s.StartConnect(body.data.ip,body.data.port)
        end
    end
    ,function (body,head)
        DwDebug.Log("拉取用户通讯服地址失败")
        if _s.isWorking then
            _s.RegisterWebFailAutoConnectBeat()
        end
--        WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, false, 0, "wait_ui_window")
    end)
end


-- 开始连接
function UserNetSys.StartConnect(ip,port,successCB)
    _s.isLoginSvrSuc = false
    --是否有输入新的ip和port
    if ip and port then
        if ip ~= "" then
            _s.ConnectSvr(_s.netName,ip,port)
            _s.ConnectSuccessCB = successCB
        end
    end

end

--移除心跳定时
function UserNetSys.RemoveHeartBeat()
    if _s.isRegsiteUpdateSec then
        DwDebug.Log("UserNetSys RemoveHeartBeat suc")
        _s.isRegsiteUpdateSec = false
        UpdateSecond:Remove(_s.UpdateSec, nil)
    end
end
--注册心跳定时
function UserNetSys.RegisterHeartBeat()
    if not _s.isRegsiteUpdateSec then
        DwDebug.Log("UserNetSys RegisterHeartBeat suc")
        _s.heartTime = 0
        _s.isRegsiteUpdateSec = true
        UpdateSecond:Add(_s.UpdateSec, nil)
    end
end
--心跳更新
function UserNetSys.UpdateSec()
    _s.heartTime = _s.heartTime + 1
    if _s.heartTime >= _s.heartInterval then
        _s.heartTime = 0
        --发送心跳包
        _s.SendHeartBeat()
    end
end


--移除web请求失败后重连定时
function UserNetSys.RemoveWebFailAutoConnectBeat()
    if _s.isRegisterWebFailAutoConnectBeat then
        DwDebug.Log("UserNetSys RemoveWebFailAutoConnectBeat suc")
        _s.isRegisterWebFailAutoConnectBeat = false
        UpdateSecond:Remove(_s.UpdateAutoConnect, nil)
    end
end

--注册web请求失败后重连定时
function UserNetSys.RegisterWebFailAutoConnectBeat()
    if not _s.isRegisterWebFailAutoConnectBeat then
        DwDebug.Log("UserNetSys RegisterWebFailAutoConnectBeat suc")
        _s.autoCountTime = 0
        _s.isRegisterWebFailAutoConnectBeat = true
        UpdateSecond:Add(_s.UpdateAutoConnect, nil)
    end
end

--重连更新
function UserNetSys.UpdateAutoConnect()
    _s.autoCountTime = _s.autoCountTime + 1
    if _s.autoCountTime >= _s.autoConnectInterval then
        _s.autoCountTime = 0
        _s.RemoveWebFailAutoConnectBeat()
        DwDebug.Log("UpdateAutoConnect")
        _s.StartUseNetWork()
    end
end



--关闭网络连接
function UserNetSys.CloseNetWork()
    DwDebug.Log("UserNetSys CloseNetWork")
    WrapSys.CNetSys_GiveUpAutoConnect(_s.netName)
    _s.RemoveHeartBeat()
    _s.ConnectSuccessCB = nil
    _s.EnableWork(false)
    _s.RemoveWebFailAutoConnectBeat()
end



--登录成功回调处理
function UserNetSys.LoginSvrSuccess()
    _s.isLoginSvrSuc = true
    _s.RegisterHeartBeat()
    --通讯服重连成功
    LuaEvent.AddEventNow(EEventType.UserNetReconnectSuccess)
    if _s.ConnectSuccessCB then
        _s.ConnectSuccessCB()
        _s.ConnectSuccessCB = nil
    end
end

--用户通讯服登录请求接口
function UserNetSys.SendLoginSvr()
    DwDebug.Log("UserNetSys SendLoginSvr")
    local body = {}
    body.userId = DataManager.GetUserID()
    body.accessToken = DataManager.GetToken()
    body.secretString = LuaUtil.MD5(body.userId,body.accessToken,DataManager.SecretKey)
    body.loginAddress = DataManager.GetLoginLocationStr()
    body.loginLng = WrapSys.GetLongitude()
    body.loginLat = WrapSys.GetLatitude()

    DwDebug.Log("==============SendLoginSvr body.loginAddress:", body.loginAddress, "body.loginLng:", body.loginLng, "body.loginLat:", body.loginLat)

    SendUserNetMsg(USER_SVR_CMD.CS_LOGIN,body,function(rsp,head) 
        _s.LoginSvrSuccess()
    end,function(rsp,head)
        if nil ~= rsp.message and "" ~= rsp.message then
            WindowUtil.LuaShowTips(rsp.message)
        else
            WindowUtil.LuaShowTips("登录连接超时,请稍候重试")
        end
        LuaEvent.AddEventNow(EEventType.ShowWaitWindow,false)

    end,true)
end

--用户通讯服心跳包接口

function UserNetSys.SendHeartBeat()
    if not _s.isLoginSvrSuc then
        return
    end
    DwDebug.Log("UserNetSys SendHeartBeat")
    local body = {}
    body.userId = DataManager.GetUserID()
    SendUserNetMsg(USER_SVR_CMD.CS_HEART_BEAT,body,function(rsp,head) 
        -- error("recv CS_HEART_BEAT sucess seq = "..head.seq .."time = "..WrapSys.GetCurrentDateTime())
    end,function(rsp,head)
        -- 失败
        DwDebug.LogError("UserNetSys recv CS_HEART_BEAT fail seq = "..head.seq .."time = "..WrapSys.GetCurrentDateTime())
    end,true)
end
