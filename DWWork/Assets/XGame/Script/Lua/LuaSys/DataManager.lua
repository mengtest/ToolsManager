--------------------------------------------------------------------------------
--   File      : DataManager.lua
--   author    : guoliang
--   function   : 重要数据管理
--   date      : 2017-09-20
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "NetWork.WebNetHelper"


DataManager = {}
local _s = DataManager
_s.SecretKey = "dw77582@gogo_BBc"
_s.userIDKey = "dw_userid"
_s.userTokenKey = "dw_token"
_s.testRoomKey = "dw_roomid"
_s.WSKData = nil
_s.isPrePublish = false
_s.localism = nil

function DataManager.Init()
	_s.reConnectState = false

	_s.userID = 0
	_s.token = nil
	_s.imToken = nil
	_s.playID = 0
	_s.userGender = nil
	_s.userNickName = nil
	_s.userHeadUrl = nil
	_s.roomCardNum = 0
	_s.roomStatus = -1
	_s.roomID = 0
	_s.roomOwnerID = 0
	_s.imFirstUseFlag = nil

	_s.InitSystemStatus()
end


function DataManager.InitSystemStatus()
	local localPublishStatus = _s.GetPrePublish()

	--是否提审预发布
	if WrapSys.Version_CheckIsPrePublish() then
		_s.SetPrePublish(true)
	else
		if localPublishStatus then -- 如果之前是预发布，切换到正式发布状态,需要去掉游客的token
			_s.ClearToken()
			_s.ClearUserID()
		end
		_s.SetPrePublish(false)
	end

	if WrapSys.Constants_RELEASE then
		WebNetHelper.URL = "http://game.dw7758.com/"
	else
		WebNetHelper.URL = "http://game.test.dw7758.com/"
	end
end

-- 设置 提审状态
function DataManager.SetPrePublish(isPrePublish)
	_s.isPrePublish = isPrePublish
	local codeNum = 0
	if isPrePublish then
		codeNum = 1
	end
	PlayerDataRefUtil.SetInt("dw_preCodeNum", codeNum)
end

-- 获取本地 提审状态
function DataManager.GetPrePublish()
	local codeNum = PlayerDataRefUtil.GetInt("dw_preCodeNum", 0)
	if codeNum > 0 then
		return true
	end
	return false
end

function DataManager.Clear()
end

-- 取创建房间参数
function DataManager.GetRoomConfig()
	return _s.roomConfig
end

-- 创建房间配置
function DataManager.SetRoomConfig( room_config )
	_s.roomConfig = room_config
end

-- 设置 RoomID
function DataManager.SetRoomID(roomID)
	_s.roomID = roomID
	PlayerDataRefUtil.SetInt(_s.testRoomKey, roomID)
end

-- 获取 RoomID
function DataManager.GetRoomID()
	if _s.roomID == 0 then
		_s.roomID = PlayerDataRefUtil.GetInt(_s.testRoomKey,0)
	end

	return _s.roomID
end

-- 房间信息
function DataManager.GetRoomDes( )
	return _s.roomDes
end

-- 房间信息
-- TODO::???退出房间的时候清空？？？
function DataManager.SetRoomDes(des)
	_s.roomDes = des
end

-- 设置音乐音量
function DataManager.SetMusicVolume(volume)
	PlayerDataRefUtil.SetFloat("MusicVolume", volume)
end
-- 获取音乐音量
function DataManager.GetMusicVolume()
	return PlayerDataRefUtil.GetFloat("MusicVolume", 1)
end
-- 设置音效音量
function DataManager.SetEffectVolume(volume)
	PlayerDataRefUtil.SetFloat("EffectVolume", volume)
end
-- 获取音效音量
function DataManager.GetEffectVolume()
	return PlayerDataRefUtil.GetFloat("EffectVolume", 1)
end
-- 设置昵称
function DataManager.SetUserNickName(nickname)
	_s.userNickName = nickname
end
-- 获取昵称
function DataManager.GetUserNickName()
	return _s.userNickName or ("Player" .. _s.GetUserID())
end
-- 设置头像
function DataManager.SetUserHeadUrl(headurl)
	_s.userHeadUrl = headurl
end
-- 获取头像
function DataManager.GetUserHeadUrl()
	return _s.userHeadUrl or ""
end
-- 设置性别
function DataManager.SetUserGender(gender)
	_s.userGender = gender
end
-- 获取性别
function DataManager.GetUserGender()
	return _s.userGender or ""
end

-- 设置 userID
function DataManager.SetUserID(userID)
	_s.userID = userID
	if userID ~= 0 then
		WrapSys.Bugly_SetUserID(userID)
	end
	PlayerDataRefUtil.SetInt(_s.userIDKey, userID)
end

-- 获取 userID
function DataManager.GetUserID()
	if _s.userID == 0 then
		_s.userID = PlayerDataRefUtil.GetInt(_s.userIDKey,0)
	end

	return _s.userID
end

-- 清除 userID
function DataManager.ClearUserID()
	_s.userID = 0
	PlayerDataRefUtil.DeleteKey(_s.userIDKey)
end

-- 设置 token
function DataManager.SetToken(token)
	_s.token = token
	DwDebug.Log("[[[token]]] = " .. token)
	PlayerDataRefUtil.SetString(_s.userTokenKey, token)
end

-- 获取 token
function DataManager.GetToken()
	if _s.token == nil then
		_s.token = PlayerDataRefUtil.GetString(_s.userTokenKey,nil)
	end

	return _s.token
end

-- 清除 token
function DataManager.ClearToken()
	_s.token = nil
	PlayerDataRefUtil.DeleteKey(_s.userTokenKey)
end

-- 设置 IM Token
function DataManager.SetIMToken(imToken)
	_s.imToken = imToken
end

-- 获取 IM Token
function DataManager.GetIMToken()
	return _s.imToken
end

function DataManager.SetRoomCardNum(roomCardNum)
	if roomCardNum ~= nil then
		_s.roomCardNum = roomCardNum
	end
end

-- 获取 系统喇叭数据
function DataManager.GetSysHornData()
	return _s.sysHornData
end

function DataManager.SetSysHornData( list )
	if list ~= nil then
		_s.sysHornData = list
	end
end

-- 获取 系统广告数据
function DataManager.GetSysAdData()
	return _s.sysAdData
end

function DataManager.SetSysAdData( list )
	if list ~= nil then
		_s.sysAdData = list
	end
end

-- 获取 系统活动数据
function DataManager.GetSysActivityData()
	return _s.sysActivityData
end

function DataManager.SetSysActivityData( list )
	if list ~= nil then
		_s.sysActivityData = list
	end
end

-- 获取 房卡数量
function DataManager.GetRoomCardNum()
	return _s.roomCardNum
end

-- 保存玩法ID
function DataManager.SetCurPlayID(playID)
	_s.playID = playID
end
-- 获取玩法ID
function DataManager.GetCurPlayID()
	return _s.playID
end
-- 保存机器识别码
function DataManager.SetDeviceID(deviceID)
 	_s.deviceID = deviceID
end
-- 获取机器识别码
function DataManager.GetDeviceID()
 	return _s.deviceID
end

--发送消息 获取web用户数据 定时刷新
function DataManager.RefreshUserInfo()
	WebNetHelper.GetUserInfo(sucCB,failCB)
end

--微信数据
function DataManager.SetWeiXinInfo(weiXinInfo)
	_s.weiXinInfo = weiXinInfo

	if _s.weiXinInfo.gender == "男" or _s.weiXinInfo.gender == "m" then
		_s.weiXinInfo.gender = "m"
	else
		_s.weiXinInfo.gender = "f"
	end

	_s.SetToken(weiXinInfo.accessToken)
end

--微信数据
function DataManager.GetWeiXinInfo()
 	return _s.weiXinInfo
end

--目前写死 给服务识别 相当于平台ID
function DataManager.GetAPPID()
 	return 10002
end

function DataManager.GetLoginLocationStr()
	local localStr = ""
	local state = WrapSys.GetState()
	if state then
		localStr = localStr .. state
	end
	local city = WrapSys.GetCity()
	if city then
		localStr = localStr .. city
	end

	local subLocality = WrapSys.GetSubLocality()
	if subLocality then
		localStr = localStr .. subLocality
	else
		local thoroughfare = WrapSys.GetThoroughfare()
		if thoroughfare then
			localStr = localStr ..thoroughfare
		end
	end

	return  localStr
end

-- 获取方言设置
-- 1 为方言 2 为普通话
function DataManager.GetLocalism(playID)
	if nil == playID then
		playID = PlayGameSys.GetNowPlayId()
	end

	if nil == _s.localism then
		_s.localism = {}
	end

	if nil == _s.localism[playID] then
		-- if playID == Common_PlayID.chongRen_510K or playID == Common_PlayID.chongRen_MJ then
		-- 	_s.localism[playID] = PlayerDataRefUtil.GetInt("Localism" .. playID, 1)
		-- else
			_s.localism[playID] = PlayerDataRefUtil.GetInt("Localism" .. playID, 2)
--		end

	end

	return _s.localism[playID]
end

-- 保存方言设置
function DataManager.SetLocalism(localism, playID)
	if nil == playID then
		playID = PlayGameSys.GetNowPlayId()
	end

	if nil == _s.localism then
		_s.localism = {}
	end

	_s.localism[playID] = localism
	PlayerDataRefUtil.SetInt("Localism" .. playID, localism)
end

--检查现在是不是方言
function DataManager.CheckNowIsFangYan()
	local playId = PlayGameSys.GetNowPlayId()
	if playId == nil then
		return 0
	end

	local isFangYan = DataManager.GetLocalism(playId) ~= 2
	return isFangYan
end

-- 是否要显示邮件红点
-- 设置
_s.is_show_mail_redPoint = false
function DataManager.SetIsShowMailRedPoint(is_show)
	DwDebug.Log(" check SetIsShowMailRedPoint",is_show)
	_s.is_show_mail_redPoint = is_show
end
-- 获取
function DataManager.GetIsShowMailRedPoint()
	return _s.is_show_mail_redPoint
end
-- 是否要显示俱乐部红点
-- 设置
_s.is_show_clubApplyMsg_redPoint = false
function DataManager.SetIsShowClubApplyMsgRedPoint(is_show)
	DwDebug.Log(" check SetIsShowClubApplyMsgRedPoint",is_show)
	_s.is_show_clubApplyMsg_redPoint = is_show
end
-- 获取
function DataManager.GetIsShowClubApplyMsgRedPoint()
	return _s.is_show_clubApplyMsg_redPoint
end