--------------------------------------------------------------------------------
--   File      : HallSys.lua
--   author    : jianing
--   function   : 大厅相关系统
--   date      : 2017-11-09
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

HallSys = {}
local _s = HallSys
local createData  --临时存储创建房间的数据

local m_ShareGameTimeValue

local retryTime = 0
local DWPicPath = Util.m_persistPath .. "/DWPic"

--是否创建或加入房间中
_s.isCreateOrJoinRooming = false

function HallSys.Init()
    m_ShareGameTimeValue = WrapSys.GetTimeValue()

    Util.CreateFolder(Util.m_persistPath .. "/Sceenshot")
    Util.CreateFolder(DWPicPath)
	-- DwDebug.Log("##########################---register")
	LuaUserNetWork.RegisterHandle(WebEvent.MailCPlayMail, _s.NotifyNewMail)
end

function HallSys.Reset()
end

function HallSys.Update()
end

function HallSys.Destroy()
	-- DwDebug.Log("##########################--- un  register")

	LuaUserNetWork.UnRegisterHandle(WebEvent.MailCPlayMail, _s.NotifyNewMail)
end

function HallSys.NotifyNewMail(rsp, head)
    -- DwDebug.LogWarning("xxx", "NotifyNewMail", rsp, head)
    -- DwDebug.LogWarning("#############################################")
    local eventType = rsp.eventType
    local category = rsp.category
    if eventType == EnumMail_EventTypeID.MailGroupApplyJoin then
		DataManager.SetIsShowClubApplyMsgRedPoint(true)
    	-- DwDebug.LogWarning("#############################################")
    end
	DataManager.SetIsShowMailRedPoint(true)
    LuaEvent.AddEventNow(EEventType.RefreshHallMailBtnRedPoint)
	LuaEvent.AddEventNow(EEventType.RefreshClubMailRedPoint)
	LuaEvent.AddEventNow(EEventType.RefreshMailUIList)
end

function HallSys.RegisterMsg()
end

--加入房间成功
local function OnWebJoinRoomSuccess(body, head)
    local data = body.data
    local ip = body.data.ip
    local port = body.data.port
    local play_id = body.data.play_id
    PlayGameSys.StartGame(play_id, ip, port, _s.curJoinRoomID, false)
    _s.curJoinRoomID = 0
end

--加入房间
function HallSys.JoinRoom(roomId, success, failed)
    --刷新定位信息
    WrapSys.PlatInterface_RefreshLocation()
    WrapSys.EZFunWindowMgr_SetWindowStatusForLua(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, true, 20000 + 10, "wait_ui_window", false)
    _s.curJoinRoomID = roomId
    _s.isCreateOrJoinRooming = true
    WebNetHelper.JoinRoom(
        roomId,
        function(body, head)
            WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, false, 0, "wait_ui_window")
            OnWebJoinRoomSuccess(body, head)
            if success then
                success(body, head)
            end
        end,
        function(body, head)
            _s.curJoinRoomID = 0
            _s.isCreateOrJoinRooming = false
            DwDebug.Log("CreateRoom fail")
            WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, false, 0, "wait_ui_window")
            if failed then
                failed(body, head)
            end
            _s.ReLoginExistRoom(body, head)
        end
    )
end

--创建房间成功
local function OnWebCreateRoomSuccess(body, head, room_config, owerID, teamID)
    local data = body.data
    local ip = body.data.ip
    local port = body.data.port
    local play_id = body.data.play_id
    room_config.msgGrouperId = owerID
    room_config.msgGroupId = teamID

    DataManager.SetRoomConfig(room_config)
    PlayGameSys.StartGame(play_id, ip, port, 0, false)
end

--创建房间
function HallSys.CreateRoom(playID, game_num, player_num, card_type, group_id, room_config, succ_cb)
    --刷新定位信息
    WrapSys.PlatInterface_RefreshLocation()
    WrapSys.EZFunWindowMgr_SetWindowStatusForLua(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, true, 20000 + 10, "wait_ui_window", false)
    _s.isCreateOrJoinRooming = true
    WebNetHelper.CreateRoom(
        playID,
        game_num,
        player_num,
        card_type,
        group_id,
        function(body, head)
            WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, false, 0, "wait_ui_window")
            OnWebCreateRoomSuccess(body, head, room_config, "", "")
            if succ_cb then
                succ_cb()
            end
        end,
        function(body, head)
            DwDebug.Log("CreateRoom fail")
            _s.isCreateOrJoinRooming = false
            WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, false, 0, "wait_ui_window")
            LuaEvent.AddEventNow(EEventType.UI_CreateRoomResult, false)
            _s.ReLoginExistRoom(body, head)
        end
    )
end

function HallSys.ReLoginExistRoom(body, head)
    if head and head.errno == 40300 then
        if body and body.data then
            local ip = body.data.ip
            local port = body.data.port
            local playID = body.data.play_id
            local roomID = body.data.room_id
            if playID > 0 and roomID > 0 then
                DataManager.SetCurPlayID(playID)
                PlayGameSys.StartGame(playID, ip, port, roomID, true)
            end
        end
    end
end

--已有房间的重连房间机制

--刷新用户数据成功
local function OnWebRefreshUserInfoSuccess(body, head)
    local data = body.data
    local roomCardNum = data.card_number
    local message_number = data.message_number
    local agent_id = data.agent_id

    -- 邮件和俱乐部申请列表红点数据
    DwDebug.Log("邮件和俱乐部申请列表红点数据", data.message_number, data.group_message_number)
    DataManager.SetIsShowMailRedPoint(data.message_number > 0)
    DataManager.SetIsShowClubApplyMsgRedPoint(data.group_message_number > 0)
    LuaEvent.AddEventNow(EEventType.RefreshHallMailBtnRedPoint)
    LuaEvent.AddEventNow(EEventType.RefreshClubMailRedPoint)

    if DataManager.GetRoomCardNum() ~= roomCardNum then
        DataManager.SetRoomCardNum(roomCardNum)
        LuaEvent.AddEventNow(EEventType.RefreshUserInfo)
    end
end

--刷新用户数据
function HallSys.RefreshUserInfo()
    WebNetHelper.RefreshUserInfo(
        function(body, head)
            OnWebRefreshUserInfoSuccess(body, head)
        end,
        function(body, head)
        end
    )
end

-- 获取是否提示每日分享
function HallSys.CheckRedDailyShare()
    WebNetHelper.IsShareGameRewarded(
        function(body, head)
            if 0 == body.errcode and not body.data.is_rewarded then
                LuaEvent.AddEventNow(EEventType.RefreshDailyShare, true)
            else
                LuaEvent.AddEventNow(EEventType.RefreshDailyShare, false)
            end
        end,
        function(body, head)
            LuaEvent.AddEventNow(EEventType.RefreshDailyShare, false)
        end
    )

    return false
end

--分享领取奖励
local function OnWebShareRewardSuccess(body, head)
    local data = body.data
    local card_number = body.data.card_number
    local addCard = card_number - DataManager.GetRoomCardNum()
    WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, addCard, "sharetips_ui_window", false, nil)

    DataManager.SetRoomCardNum(card_number)
    LuaEvent.AddEventNow(EEventType.RefreshUserInfo)

    LuaEvent.AddEventNow(EEventType.RefreshDailyShare, false)
end

--分享成功获取奖励
function HallSys.ShareGameReward()
    WebNetHelper.ShareGameReward(
        function(body, head)
            OnWebShareRewardSuccess(body, head)
        end,
        function(body, head)
            DwDebug.Log("ShareGameReward error")
        end
    )
end

--微信分享回调
local function LocalSharegameReward(isSuccess)
    if isSuccess then
        _s.ShareGameReward()
    else
        --WindowUtil.LuaShowTips("取消分享")
    end
end

--获取分享图片数据，调用微信分享接口
local function OnWebShareGameSuccess(body, head)
    local data = body.data
    local img_url = body.data.img_url
    local title = body.data.title
    local href_url = body.data.href_url

    --检查下载图片
    _s.LoadPic(
        img_url,
        function(downsuc, imgPath)
            if downsuc then
                _s.WeiXinShareUrl(
                    " ",
                    title,
                    imgPath,
                    href_url,
                    function(isSuccess)
                        if isSuccess then
                            LocalSharegameReward(isSuccess)
                        else
                            --WindowUtil.LuaShowTips("取消分享")
                        end
                    end,
                    true
                )
            end
        end
    )
end

--分享游戏
function HallSys.ShareGame()
    if m_ShareGameTimeValue.Value > 0 then
        WindowUtil.LuaShowTips("分享太快，请稍后再试")
        return
    end
    m_ShareGameTimeValue.Value = 2

    WebNetHelper.ShareGame(
        function(body, head)
            OnWebShareGameSuccess(body, head)
        end,
        function(body, head)
            DwDebug.Log("ShareGame error")
        end
    )
end

--分享游戏成功
local function ShareRoomSuccess(body, head)
    local data = body.data
    --local img_url = body.data.img_url
    local title = body.data.title
    local href_url = body.data.href_url
    local content = body.data.content
    local img_url = Util.m_streamPath .. "/Local/icon.png"

    --安卓不能直接分享m_streamPath目录图片
    if WrapSys.IsAndroid then
        --检查下载图片
        _s.LoadPic(
            img_url,
            function(downsuc, imgPath)
                if downsuc then
                    _s.WeiXinShareUrl(
                        content,
                        title,
                        imgPath,
                        href_url,
                        function(isSuccess)
                            if isSuccess then
                                DwDebug.Log("ShareRoomSuccess Success")
                            else
                                DwDebug.Log("ShareRoomSuccess Fail")
                            end
                        end
                    )
                end
            end
        )
    else
        _s.WeiXinShareUrl(
            content,
            title,
            img_url,
            href_url,
            function(isSuccess)
                if isSuccess then
                    DwDebug.Log("ShareRoomSuccess Success")
                else
                    DwDebug.Log("ShareRoomSuccess Fail")
                end
            end
        )
    end
end

--分享房间
function HallSys.ShareRoom(roomid)
    if m_ShareGameTimeValue.Value > 0 then
        WindowUtil.LuaShowTips("分享太快，请稍后再试")
        return
    end
    m_ShareGameTimeValue.Value = 3

    WebNetHelper.ShareRoom(
        roomid,
        function(body, head)
            ShareRoomSuccess(body, head)
        end,
        function(body, head)
            DwDebug.Log("ShareRoom error")
        end
    )
end

local shareIndex = 1 --分享序列 一直增长 防止重复
function HallSys.GetSharePath()
    --清理上一张(防止连续调用出错 留3张吧)
    Util.DeleteFile(Util.m_persistPath .. "/Sceenshot/Sceenshot" .. (shareIndex - 2) .. ".png")
    shareIndex = shareIndex + 1
    return Util.m_persistPath .. "/Sceenshot/Sceenshot" .. shareIndex .. ".png"
end

--分享总局结算(都是一样的 )
function HallSys.ShareTotalSettlement(isWeiXinCircle)
    WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.err_tips_ui_window, false, 0, "", false, nil)

    local picPath = _s.GetSharePath()
    WrapSys.GetScreenTexturePic(
        0,
        0,
        WrapSys.GameRoot_Screen_width(),
        WrapSys.GameRoot_Screen_height(),
        picPath,
        function()
            LuaEvent.AddEvent(EEventType.ScreenShotEnd)
            _s.WeiXinShareUrl(
                "",
                "",
                picPath,
                "",
                function(isSuccess)
                    if isSuccess then
                        DwDebug.Log("ShareTotalSettlement Success")
                    else
                        --WindowUtil.LuaShowTips("分享失败，请重新再试")
                        DwDebug.Log("ShareTotalSettlement Fail")
                    end
                end,
                isWeiXinCircle
            )
        end
    )
end

--微信直接分享url 图片全部走上面方法下 可以控制图片
function HallSys.WeiXinShareUrl(text, title, picUrl, targeturl, luaFunc, isWeiXinCircle)
    m_ShareGameTimeValue.Value = 3

    local platform = UMengSharePlatform.WEIXIN --微信
    if isWeiXinCircle then
        platform = UMengSharePlatform.WEIXIN_CIRCLE --朋友圈
    end
    WrapSys.WeiXinShareLocalUrl(platform, text, title, picUrl, targeturl, luaFunc)
end

--下载图片 检查缓存
function HallSys.LoadPic(picUrl, callBack)
    local picPath = ""
    if picUrl ~= nil then
        local len = string.len(picUrl)
        local index = ezfunLuaTool.StringFindLastIndex(picUrl, "/")
        if index >= 0 then
            picPath = DWPicPath .. string.sub(picUrl, index, len)
        end
    end
    if Util.CheckFileExists(picPath) then
        callBack(true, picPath)
    else
        WrapSys.LoadPic(picUrl, picPath, callBack)
    end
end

--最后分享时间
function HallSys.GetWeiXinSHareTime()
    return m_ShareGameTimeValue.Value
end
