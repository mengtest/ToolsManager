--------------------------------------------------------------------------------
--   File      : LoginSys.lua
--   author    : guoliang
--   function   : 登录系统
--   date      : 2017-09-20
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "NetWork.WebNetHelper"

LoginSys = {}
local _s = LoginSys
_s.curLoginType = 100
_s.isLoginSuc = false
_s.isRegisterLoginTimer = false

_s.QRPicPath = Util.m_persistPath .. "/LocalDownImg/qrCodePic.png"

function LoginSys.Init()
    LuaEvent.AddHandle(EEventType.UI_Msg_LoginOut,_s.UI_Msg_LoginOut)
    LuaEvent.AddHandle(EEventType.WeiXinLoginEnd,_s.WeiXinLoginCallBack)
end

function LoginSys.Reset()

end

function LoginSys.Destroy()
    LuaEvent.RemoveHandle(EEventType.UI_Msg_LoginOut,_s.UI_Msg_LoginOut)
    LuaEvent.RemoveHandle(EEventType.WeiXinLoginEnd,_s.WeiXinLoginCallBack)
end

function LoginSys.RegisterMsg()

end


function LoginSys.UI_Msg_LoginOut(eventId,p1,p2)
    _s.LoginOutFunc()
end

-- 获取是否同意用户协议
function LoginSys.SetAgreement(agree)
	PlayerDataRefUtil.SetInt("User_Agreement", agree and 1 or 0)
end

function LoginSys.GetAgreement()
	return PlayerDataRefUtil.GetInt("User_Agreement", 1) == 1 and true or false
end

--检查是否自动登陆
function LoginSys.CheckCanAutoLogin()
	if not _s.GetAgreement() then
		return false
	end

	local userID = DataManager.GetUserID()
	local token = DataManager.GetToken()

	if userID ~= 0 and token ~= nil then
		return true
	else
		return false
	end
end

--
function LoginSys.Login(loginType,isReconnect)
    --可能web登陆上了 游戏服没连上
    if not _s.isLoginSuc or GameStateMgr.GetCurStateType() == EGameStateType.LoginState then
    	_s.curLoginType = loginType

		WrapSys.EZFunWindowMgr_SetWindowStatusForLua(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, true, 20000+10, "wait_ui_window" , isReconnect)
        if not isReconnect then
--            _s.RegisterLoginTimer()
        end


    	if WrapSys.IsEditor then
    		DataManager.SetUserID(TestConfig.UserID)
    		DataManager.SetToken(TestConfig.UserToken)
--        else
            --test
            -- DataManager.SetUserID(100042)
            -- DataManager.SetToken("fd3651101637e0d2cd222f43c2e09b59")
        end

    	local userID = DataManager.GetUserID()
    	local token = DataManager.GetToken()

    	if loginType == 50 then --游客登陆
--            DataManager.SetDeviceID(10)
            local deviceID = WrapSys.GetDeviceID()
            if deviceID == "" then
                WindowUtil.LuaShowTips("未能获取到设备ID")
                if WrapSys.IsEditor then
                    deviceID = math.random(2000)
                end
            end
            if deviceID ~= "" then
        		DataManager.SetDeviceID(deviceID)
        		WebNetHelper.Login(loginType,userID, token, _s.OnWebLoginSuccess,_s.OnWebLoginFail)
            end
    	elseif loginType == 100 then
    		if userID ~= 0 and token ~= nil then
    			DwDebug.Log("LoginSys.Login  Token  Login")
    			WebNetHelper.Login(100,userID, token, _s.OnWebLoginSuccess,_s.OnWebLoginFail)
    		end
    	elseif loginType == 1 and not WrapSys.IsEditor then --微信登陆
            if token ~= nil then
    		    WebNetHelper.Login(loginType,userID, token, _s.OnWebLoginSuccess,_s.OnWebLoginFail)
            else
                DwDebug.LogError("微信登录的token 为空")
                _s._OnWebLoginFailed()
            end
    	else
    		if userID ~= 0 and token ~= nil then
    			WebNetHelper.Login(100,userID, token, _s.OnWebLoginSuccess,_s.OnWebLoginFail)
    		end
    	end
    else
        DwDebug.LogError("isLoginSuc is true")
    end
end


function LoginSys.OnWebLoginFail(data,head)
    DwDebug.Log("LoginScene._OnWebLoginCallback  Login  Failed " ..head.errno)
     _s.ResetLoginStatus()
    --游戏中重连容易关掉菊花
    if PlayGameSys.GetPlayLogic() == nil then
	   --WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.wait_ui_window, false,0,"")
	   WrapSys.EZFunWindowMgr_SetWindowStatusForLua(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, false, 0, "wait_ui_window")
    end

--    _s.RemoveLoginTimer()
end

--查看二维码是否为最新 不是就要下载到本地
local function CheckQRIcon(qr_code_url)
    if qr_code_url ~= nil then
        local localUrl = PlayerDataRefUtil.GetString("qr_code_url", "emptry")
        if qr_code_url ~= localUrl or  Util.CheckFileExists(_s.QRPicPath) then
            WrapSys.LoadPic(qr_code_url, _s.QRPicPath, function (isdown,path)
                PlayerDataRefUtil.SetString("qr_code_url", qr_code_url)
            end)
        end
    end
end

function LoginSys.OnWebLoginSuccess(body,head)
    DwDebug.Log("LoginSys.OnWebLoginSuccess--------------------")
	--WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.wait_ui_window, false,0,"")
	WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, false, 0, "wait_ui_window")
 --   _s.RemoveLoginTimer()

	local data = body.data
    if data then
        _s.isLoginSuc = true
    	local uid = data.uid
    	local token = data.access_token
    	local imToken = data.im_token
    	local roomCardNum = data.card_number
    	DataManager.SetUserID(uid)
    	DataManager.SetToken(token)
    	WrapSys.CNetSys_SetAccessToken(token)
    	DataManager.SetIMToken(imToken)
    	DataManager.SetRoomCardNum(roomCardNum)

    	DataManager.SetUserNickName(data.wx_nickname)
    	DataManager.SetUserHeadUrl(data.wx_headimgurl)
    	DataManager.SetUserGender(data.wx_sex)

    	local ip = data.user_room.ip
    	local port = data.user_room.port
    	local playID = data.user_room.play_id
    	local roomID = data.user_room.room_id
    	DwDebug.Log("playID "..playID)
    	if playID > 0 and roomID > 0 then
            DataManager.SetCurPlayID(playID)
            PlayGameSys.StartGame(playID,ip,port,roomID,true)
        else
            if data.communicate_svr then
                if data.communicate_svr.ip ~= "" then
                    UserNetSys.EnableWork(true)
                    UserNetSys.StartConnect(data.communicate_svr.ip,data.communicate_svr.port)
                    PlayGameSys.QuitToMainCityNoNet()

                    --从游戏中断线重新回来 提示通知
                    if GameStateMgr.GetLastStateType() == EGameStateType.GameState then
                        GameStateQueueTask.AddTask(function ()
                            WindowUtil.ShowErrorWindow(3,"房间已解散！")
                        end,1,EGameStateType.MainCityState)
                    end

                else
                    PlayGameSys.QuitToMainCity()
                end
            else
                PlayGameSys.QuitToMainCity()
            end
        end

        NimChatSys.LoginIm(uid,imToken)
        -- 登录完成后 请求系统通知数据 (不填参数 请求全部通知类型)
        _s.RequestNoticeList()

        CheckQRIcon(data.qr_code_url)
    end
end

-- 退出功能函数
function LoginSys.LoginOutFunc()
   if not WrapSys.IsAndroid then
     NimChatSys.LogoutIm()
   end
    WrapSys.WeiXinLogout()
    DataManager.ClearUserID()
    DataManager.ClearToken()
    _s.ResetLoginStatus()
    PlayGameSys.QuitToLoginScene()
    GameManager.Reset()
end

function LoginSys.ResetLoginStatus()
    _s.isLoginSuc = false
end


--登录退出（外部接口）
function LoginSys.LoginOut()

	local sucCB = function (body,head)
        _s.LoginOutFunc()
	end

    if WrapSys.IsEditor then
        sucCB()
    else
        WebNetHelper.Logout()
        sucCB()
    end

end

-- 刷新公告数据成功
local function OnWebRequestSysNoticeListSuccess(body,head)
    if body and body.data then
    	local data = body.data

    	local list_activity = data.list_activity
    	local list_ad = data.list_ad
    	local list_horn = data.list_horn

    	-- if #list_activity > 0 then
    	-- 	DataManager.SetSysActivityData(list_activity)
    	-- end
    	-- if #list_ad > 0 then
    	-- 	DataManager.SetSysAdData(list_ad)
    	-- end
    	-- if #list_horn > 0 then
    	-- 	DataManager.SetSysHornData(list_horn)
    	-- end
    end
	-- LuaEvent.AddEventNow(EEventType.RefreshNoticeSysHorn)
	-- LuaEvent.AddEventNow(EEventType.RefreshNoticeSysAd)

end

-- 登录时候请求一次所有通告数据,根据通告类型
-- 0：全部类型，1：活动，2：广告，3：系统喇叭
function LoginSys.RequestNoticeList(_type)
    if _s.isLoginSuc then
    	_type = _type or 0
    	WebNetHelper.requestNotice(
    		_type,
    		function ( body,head )
    			OnWebRequestSysNoticeListSuccess(body,head)
    		end,
    		function ( body,head )
    			DwDebug.Log("WebNetHelper requestNotice failed")
    		end
    	)
    end
end

--微信登陆回包
function LoginSys.WeiXinLoginCallBack(eventId,sucsess,data)
    --print("data :" .. data)
    if sucsess then
        if data ~= "isLogined" then
            local weiXinInfo = loadstring("return " .. data)()
            DataManager.SetWeiXinInfo(weiXinInfo)
        end
        _s.Login(1)
    end
end


-- --移除登录定时检测
-- function LoginSys.RemoveLoginTimer()
--     if _s.isRegisterLoginTimer then
--         print("RemoveLoginTimer suc")
--         _s.isRegisterLoginTimer = false
--         UpdateSecond:Remove(_s.UpdateTimer,nil)
--     end
-- end
-- --注册登录定时检测
-- function LoginSys.RegisterLoginTimer()
--     _s.loginCountTime = 0
--     if not _s.isRegisterLoginTimer then
--         print("RegisterLoginTimer suc")
--         _s.isRegisterLoginTimer = true
--         UpdateSecond:Add(_s.UpdateTimer,nil)
--     end
-- end
-- --更新定时器
-- function LoginSys.UpdateTimer()
--     print("UpdateTimer")
--     _s.loginCountTime = _s.loginCountTime + 1
--     if _s.loginCountTime  > 5 then
--         WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.wait_ui_window, false,0,"")
--         _s.loginCountTime = 0
--         _s.RemoveLoginTimer()
--     end
-- end