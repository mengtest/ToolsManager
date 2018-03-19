--------------------------------------------------------------------------------
--   File      : WebNetHelper.lua
--   author    : guoliang
--   function   : web服网络接口
--   date      : 2017-09-20
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

WebNetHelper = {}
local _s = WebNetHelper

--_s.URL = "http://game.dw7758.com/"-- 正式
--_s.URL = "http://game.dev.dw7758.com/" --开发
_s.URL = "http://game.test.dw7758.com/" -- 测试

_s.loginAddr = "v1/usersvr/user/login"
_s.logoutAddr = "v1/usersvr/user/logout"
_s.loginUserInfo = "v1/usersvr/user/info"
_s.createRoomAddr = "v1/gamesvr/user/createroom"
_s.joinRoomAddr = "v1/gamesvr/user/joinroom"
_s.inviteInfoAddr = "v1/gamesvr/user/shareroom"
_s.shareInfoAddr = "v1/gamesvr/user/sharegame"
_s.shareRewardAddr = "v1/gamesvr/user/sharegamereward"
_s.isShareGameRewarded = "v1/gamesvr/user/issharegamerewarded"
_s.noticeList = "v1/noticesvr/notice/list"
--代理
_s.myagent = "v1/usersvr/user/myagent"
_s.bindagent = "v1/usersvr/user/bindagent"
--结算
_s.singleSettlementShareInfo = "v1/gamesvr/user/sharesinglesettlement"
_s.totalSettlementShareInfo = "v1/gamesvr/user/sharetotalsettlement"
--附近的人
_s.lbsNearbyAddr = "v1/usersvr/lbs/nearby"
--战绩
_s.recordListAddr = "v1/archivesvr/archive/list"
_s.recordDetailAddr = "v1/archivesvr/archive/detail"
_s.recordDetailReplyInfoAddr = "v1/archivesvr/archive/detailrecord"

--ios内购
_s.storeListAddr = "v1/paysvr/apple/goods"
_s.createOrderAddr = "v1/paysvr/apple/createorder"
_s.checkBuyResultAddr = "v1/paysvr/apple/verifyreceipt"
_s.queryBuyResultStatusAddr = "v1/paysvr/apple/checkstatus"

-- 消息系统
_s.msgListAddr = "v1/imsvr/message/list"
_s.msgMailMessageIsReadAddr = "v1/imsvr/message/processed"

-- 俱乐部
_s.clubListAddr = "v1/groupsvr/group/list"
_s.clubDetailAddr = "v1/groupsvr/group/info"
_s.createClubAddr = "v1/groupsvr/group/add"
_s.updateClubAddr = "v1/groupsvr/group/update"
_s.rechargeClubAddr = "v1/groupsvr/group/recharge"
_s.deleteClubAddr = "v1/groupsvr/group/delete"
_s.clubMemListAddr = "v1/groupsvr/groupuser/list"
_s.clubMemDetailAddr = "v1/groupsvr/groupuser/info"
_s.applyJoinClubAddr = "v1/groupsvr/groupuser/applyjoin"
_s.handleApplyClubAddr = "v1/groupsvr/groupuser/handleapply"
_s.kickoutClubAddr = "v1/groupsvr/groupuser/kickout"
_s.quitClubAddr = "v1/groupsvr/groupuser/quit"
_s.msgCategoryListAddr = "v1/groupsvr/message/categorylist"
_s.templateListAddr = "v1/groupsvr/room/tpllist"
_s.templateDetailAddr = "v1/groupsvr/room/tplinfo"
_s.createTemplateAddr = "v1/groupsvr/room/createtpl"
_s.updateTemplateAddr = "v1/groupsvr/room/updatetpl"
_s.deleteTemplateAddr = "v1/groupsvr/room/deletetpl"
_s.roomListAddr = "v1/groupsvr/room/list"
_s.archiveListAddr = "v1/groupsvr/archive/list"
_s.archiveDetailAddr = "v1/groupsvr/archive/detail"
_s.archiveDetailRecordAddr = "v1/groupsvr/archive/detailrecord"
_s.cardFlowAddr = "v1/groupsvr/card/flow"
_s.statisticsPlayedAddr = "v1/groupsvr/statistic/played"

--用户通讯服务器基础协议
_s.communicateSvrAddr = "v1/groupsvr/basic/communicatesvr"

--请求用户通讯服基础信息
function WebNetHelper.RequestUserNetBaseInfo(sucCB,failCB)
	local param_pkg = {}
	_s.PostRequest(WebEvent.UseNetBaseInfo,_s.communicateSvrAddr,param_pkg,sucCB,failCB)
end


-- 请求局数统计
function WebNetHelper.RequestStatisticsPlayed(page_index, page_size, group_id, date_type, sucCB,failCB)
	local param_pkg = {
		[1]={key="group_id",value=group_id},
		[2]={key="date_type",value=date_type}
	}
	_s.PostRequest(WebEvent.StatisticsPlayed,_s.statisticsPlayedAddr,param_pkg,sucCB,failCB)
end

-- 请求房卡流水列表
function WebNetHelper.RequestCardFlow(page_index, page_size, group_id, trade_type, sucCB,failCB)
	local param_pkg = {
		[1]={key="page_index",value=page_index},
		[2]={key="page_size",value=page_size},
		[3]={key="group_id",value=group_id},
		[4]={key="trade_type",value=trade_type}
	}
	_s.PostRequest(WebEvent.CardFlow,_s.cardFlowAddr,param_pkg,sucCB,failCB)
end

-- 请求战绩详情回放
function WebNetHelper.RequestArchiveDetailRecord(group_id, detail_id, sucCB,failCB)
	local param_pkg = {
		[1]={key="group_id",value=group_id},
		[2]={key="detail_id",value=detail_id}
	}
	_s.PostRequest(WebEvent.ArchiveDetailRecord,_s.archiveDetailRecordAddr,param_pkg,sucCB,failCB)
end

-- 请求战绩详情
function WebNetHelper.RequestArchiveDetail(group_id, archive_id, sucCB,failCB)
	local param_pkg = {
		[1]={key="group_id",value=group_id},
		[2]={key="archive_id",value=archive_id}
	}
	_s.PostRequest(WebEvent.ArchiveDetail,_s.archiveDetailAddr,param_pkg,sucCB,failCB)
end

-- 请求战绩列表
function WebNetHelper.RequestArchiveList(page_index, page_size, group_id, date_type, sucCB,failCB)
	local param_pkg = {
		[1]={key="page_index",value=page_index},
		[2]={key="page_size",value=page_size},
		[3]={key="group_id",value=group_id}
	}
	if date_type then
		param_pkg[4]={key="date_type",value=date_type}
	end
	_s.PostRequest(WebEvent.ArchiveList,_s.archiveListAddr,param_pkg,sucCB,failCB)
end

-- 请求房间列表
function WebNetHelper.RequestRoomList(group_id,page_index, page_size, sucCB,failCB)
	local param_pkg = {
		[1]={key="group_id",value=group_id},
		[2]={key="page_index",value=page_index},
		[3]={key="page_size",value=page_size},
	}
	_s.PostRequest(WebEvent.RoomList,_s.roomListAddr,param_pkg,sucCB,failCB, true)
end

-- 请求更新房间模板
function WebNetHelper.RequestUpdateTemplate(group_id, id, play_id, play_config,sucCB,failCB)
	local param_pkg = {
		[1]={key="group_id",value=group_id},
		[2]={key="id",value=id},
		[3]={key="play_id",value=play_id},
		[4]={key="play_config",value=play_config},
	}
	_s.PostRequest(WebEvent.UpdateTemplate,_s.updateTemplateAddr,param_pkg,sucCB,failCB)
end

-- 请求删除模板
function WebNetHelper.RequestDeleteTemplate(group_id,id,sucCB,failCB)
	local param_pkg = {
		[1]={key="group_id",value=group_id},
		[2]={key="id",value=id},
	}
	_s.PostRequest(WebEvent.DeleteTemplate,_s.deleteTemplateAddr,param_pkg,sucCB,failCB)
end

-- 请求创建房间模板
function WebNetHelper.RequestCreateTemplate(group_id,play_id,play_config,sucCB,failCB)
	local param_pkg = {
		[1]={key="group_id",value=group_id},
		[2]={key="play_id",value=play_id},
		[3]={key="play_config",value=play_config},
	}
	_s.PostRequest(WebEvent.CreateTemplate,_s.createTemplateAddr,param_pkg,sucCB,failCB)
end

-- 请求房间模板详情
function WebNetHelper.RequestTemplateDetail(id,sucCB,failCB)
	local param_pkg = {
		[1]={key="id",value=id},
	}
	_s.PostRequest(WebEvent.TemplateDetail,_s.templateDetailAddr,param_pkg,sucCB,failCB)
end

-- 请求房间模板列表
function WebNetHelper.RequestTemplateList(group_id,sucCB,failCB)
	local param_pkg = {
		[1]={key="group_id",value=group_id},
	}
	_s.PostRequest(WebEvent.TemplateList,_s.templateListAddr,param_pkg,sucCB,failCB)
end

-- 请求邮件信息 请求子类消息列表
function WebNetHelper.RequestMsgList(category,page_index,page_size,sucCB,failCB)
	local param_pkg = {
		[1]={key="page_index",value=page_index},
		[2]={key="page_size",value=page_size},
	}
	if category then
		param_pkg[3] = {key="category",value=category}
	end
	_s.PostRequest(WebEvent.MsgList,_s.msgListAddr,param_pkg,sucCB,failCB)
end

-- 发送是否已读
-- 请求邮件信息 请求子类消息列表
function WebNetHelper.RequestReadMailMessageById(id,sucCB,failCB)
	local param_pkg = {
		[1]={key="id",value=id},
	}
	_s.PostRequest(WebEvent.MsgMailMessageIsReadAddr,_s.msgMailMessageIsReadAddr,param_pkg,sucCB,failCB)
end

-- 请求消息类型列表
function WebNetHelper.RequestMsgCategoryList(sucCB,failCB)
	_s.PostRequest(WebEvent.MsgCategoryList,_s.msgCategoryListAddr,nil,sucCB,failCB)
end

-- 请求退出俱乐部
function WebNetHelper.RequestQuitClub(group_id,sucCB,failCB)
	local param_pkg = {
		[1]={key="group_id",value=group_id},
	}
	_s.PostRequest(WebEvent.QuitClub,_s.quitClubAddr,param_pkg,sucCB,failCB)
end

-- 踢出群
function WebNetHelper.RequestKickoutClub(group_id,kick_uid,sucCB,failCB)
	local param_pkg = {
		[1]={key="group_id",value=group_id},
		[2]={key="kick_uid",value=kick_uid},
	}
	_s.PostRequest(WebEvent.KickoutClub,_s.kickoutClubAddr,param_pkg,sucCB,failCB)
end

-- 处理加群申请
function WebNetHelper.RequestHandleApplyClub(group_id,apply_uid,type,message_id,sucCB,failCB)
	local param_pkg = {
		[1]={key="group_id",value=group_id},
		[2]={key="apply_uid",value=apply_uid},
		[3]={key="type",value=type},
		[4]={key="message_id",value=message_id},
	}
	_s.PostRequest(WebEvent.HandleApplyClub,_s.handleApplyClubAddr,param_pkg,sucCB,failCB)
end

-- 请求加入俱乐部
function WebNetHelper.RequestApplyJoinClub(group_id,is_check,addit_msg,sucCB,failCB)
	local param_pkg = {
		[1]={key="group_id",value=group_id},
		[2]={key="is_check",value=is_check},
		[3]={key="addit_msg",value=addit_msg},
	}
	_s.PostRequest(WebEvent.ApplyJoinClub,_s.applyJoinClubAddr,param_pkg,sucCB,failCB)
end

-- 请求俱乐部成员详情
function WebNetHelper.RequestClubMemDetail(group_id,look_uid,sucCB,failCB)
	local param_pkg = {
		[1]={key="group_id",value=group_id},
		[2]={key="look_uid",value=look_uid},
	}
	_s.PostRequest(WebEvent.ClubMemDetail,_s.clubMemDetailAddr,param_pkg,sucCB,failCB)
end

-- 请求俱乐部成员列表
function WebNetHelper.RequestClubMemList(group_id,is_page,page_index,page_size, search,sucCB,failCB)
	local param_pkg = {
		[1]={key="group_id",value=group_id},
		[2]={key="is_page",value=is_page},
	}
	if is_page == 1 then
		param_pkg[3]={key="page_index",value=page_index}
		param_pkg[4]={key="page_size",value=page_size}
	end
	if search then
		param_pkg[5] = {key="search",value=search}
	end
	_s.PostRequest(WebEvent.ClubMemList,_s.clubMemListAddr,param_pkg,sucCB,failCB, true)
end

-- 请求解散俱乐部
function WebNetHelper.RequestDeleteClub(id,sucCB,failCB)
	local param_pkg = {
		[1]={key="id",value=id}
	}
	_s.PostRequest(WebEvent.DeleteClub,_s.deleteClubAddr,param_pkg,sucCB,failCB)
end

-- 请求充值
function WebNetHelper.RequestRechargeClub(id, number, sucCB, failCB)
	local param_pkg = {
		[1] = {key="id", value=id},
		[2] = {key="recharge_number", value=number}
	}
	_s.PostRequest(WebEvent.RechargeClub,_s.rechargeClubAddr,param_pkg,sucCB,failCB)
end

-- 添加可选参数
function WebNetHelper.AddParam(pkg, key, value)
	if nil ~= value then
		table.insert(pkg, {key=key, value=value})
	end
end

-- 请求更新俱乐部
function WebNetHelper.RequestUpdateClub(id,name,g_img,owner_weixin,owner_mobile,sucCB,failCB)
	local param_pkg = {
		[1]={key="id",value=id},
		-- [2]={key="name",value=name},
		-- [3]={key="g_img",value=g_img},
		-- [4]={key="owner_weixin",value=owner_weixin},
		-- [5]={key="owner_mobile",value=owner_mobile}
	}
	_s.AddParam(param_pkg, "name", name)
	_s.AddParam(param_pkg, "g_img", g_img)
	_s.AddParam(param_pkg, "owner_weixin", owner_weixin)
	_s.AddParam(param_pkg, "owner_mobile", owner_mobile)

	_s.PostRequest(WebEvent.UpdateClub,_s.updateClubAddr,param_pkg,sucCB,failCB)
end

-- 请求创建俱乐部
function WebNetHelper.RequestCreateClub(name,g_img,owner_weixin,owner_mobile,sucCB,failCB)
	local param_pkg = {
		[1]={key="name",value=name},
		[2]={key="g_img",value=g_img},
		[3]={key="owner_weixin",value=owner_weixin},
		[4]={key="owner_mobile",value=owner_mobile}
	}
	_s.PostRequest(WebEvent.CreateClub,_s.createClubAddr,param_pkg,sucCB,failCB)
end

-- 请求俱乐部详情
function WebNetHelper.RequestClubDetail(id,sucCB,failCB)
	local param_pkg = {
		[1]={key="id",value=id}
	}
	_s.PostRequest(WebEvent.ClubDetail,_s.clubDetailAddr,param_pkg,sucCB,failCB)
end

-- 请求俱乐部列表
function WebNetHelper.RequestClubList(status,sucCB,failCB)
	local param_pkg = {
		[1]={key="status",value=status}
	}
	_s.PostRequest(WebEvent.ClubList,_s.clubListAddr,param_pkg,sucCB,failCB, true)
end

-- 统一的post请求接口，仅限于前11个参数分别是cid,platform,imei,ua,in_house,os_version,app_version,token,app_id,uid,access_token的请求
-- 只是为了减少重复代码
function WebNetHelper.PostRequest(msg_key, msg_addr, param_pkg, sucCB,failCB, use_seq)
	local uid = DataManager.GetUserID()
	local token = DataManager.GetToken()
	local app_id = DataManager.GetAPPID()

	local param = _s._CreateBaseLoginData()
	local c = #param

	param[c + 1] = "token=" .. LuaUtil.MD5(app_id, uid, token, DataManager.SecretKey, _s.GetUnpackedParam(param_pkg))
	param[c + 2] = "app_id=" .. app_id
	param[c + 3] = "uid=" .. uid
	param[c + 4] = "access_token=" .. token

	for i,v in ipairs(param_pkg or {}) do
		table.insert( param, v.key.."="..v.value)
	end

	local post = true
	local _param = table.concat(param, "&")
	HttpRequest(msg_key,_s.URL .. msg_addr, _param, post,
		sucCB,failCB,true,8, use_seq)
end

function WebNetHelper.GetUnpackedParam(param_pkg)
	local param = {}

	for i,v in ipairs(param_pkg or {}) do
		table.insert( param, v.value)
	end

	return unpack(param)
end

function WebNetHelper.Init()
end

--将头像大图转换为小图
function WebNetHelper.GetSmallWeiXinIconUrl(iconurl)
	if iconurl ~= nil then
		local subIconurl = iconurl
		local len = string.len(iconurl)
		local index = ezfunLuaTool.StringFindLastIndex(iconurl, "/")
		if index >= 0 then
			subIconurl = string.sub(iconurl, 1, index - 1)
			subIconurl = subIconurl .. "/132"
		end

		return subIconurl
	end
	return ""
end

-- 登陆
--type  1 微信登陆，100 token 登陆 50 游客登陆
function WebNetHelper.Login(_type,uid, token, sucCB,failCB)
	local wxuid = 0
	local name = "小亮"
	local gender = "m"
	local iconurl = ""
	local accessToken = ""
	local refreshToken = ""
	local deviceid = 0
	local longitude = WrapSys.GetLongitude()
	local latitude = WrapSys.GetLatitude()
	if _type == 1 then --微信
		local weiXinInfo = DataManager.GetWeiXinInfo()
		wxuid = weiXinInfo.uid
		name = weiXinInfo.name
		gender = weiXinInfo.gender
		iconurl = _s.GetSmallWeiXinIconUrl(weiXinInfo.iconurl)
		accessToken = weiXinInfo.accessToken
		refreshToken = weiXinInfo.refreshToken
		uid = wxuid
		token = accessToken
	end
	local app_id = DataManager.GetAPPID()

	local param = _s._CreateBaseLoginData()
	local c = #param

	local _token = ""
	if _type == 1 then
		_token = LuaUtil.MD5(app_id,DataManager.SecretKey, _type, wxuid, gender, iconurl, accessToken, refreshToken,longitude,latitude)
	elseif _type == 100 then
		_token = LuaUtil.MD5(app_id,uid, token, DataManager.SecretKey, _type,longitude,latitude)
	elseif _type == 50 then
		deviceid = DataManager.GetDeviceID()
		_token = LuaUtil.MD5(app_id,DataManager.SecretKey, _type,deviceid,longitude,latitude)
	end

	param[c + 1] = "token=" .. _token
	param[c + 2] = "type=" .. _type
	param[c + 3] = "uid=" .. uid
	param[c + 4] = "wx_uid=" .. wxuid

	param[c + 5] = "wx_name=" .. LuaUtil.UrlEncode(name)
	param[c + 6] = "wx_gender=" .. gender
	param[c + 7] = "wx_icon_url=" .. iconurl
	param[c + 8] = "wx_access_token=" .. accessToken
	param[c + 9] = "wx_refresh_token=" .. refreshToken
	param[c + 10] = "access_token=" .. DataManager.GetToken()
	param[c + 11] = "device_uid=" .. deviceid
	param[c + 12] = "app_id=" .. app_id
	param[c + 13] = "longitude=" .. longitude
	param[c + 14] = "latitude=" .. latitude
	param[c + 15] = "no_wx_name="..1

	local post = true
	local _param = table.concat(param, "&")

	HttpRequest(WebEvent.LoginWeb,_s.URL .. _s.loginAddr, _param, post,sucCB,failCB,true)
end

-- 获取用户信息(主场景定时刷新)
function WebNetHelper.RefreshUserInfo(sucCB,failCB)
	-- 经过测试，用PostRequest有效，可以减少代码量
	-- _s.PostRequest(WebEvent.GetUserInfoWeb,_s.loginUserInfo,nil,sucCB,failCB)
	local uid = DataManager.GetUserID()
	local token = DataManager.GetToken()
	local app_id = DataManager.GetAPPID()
	local param = _s._CreateBaseLoginData()
	local c = #param

	param[c + 1] = "token=" .. LuaUtil.MD5(app_id,uid, token, DataManager.SecretKey)
	param[c + 2] = "app_id=" .. app_id
	param[c + 3] = "uid=" .. uid
	param[c + 4] = "access_token=" .. token

	local post = true
	local _param = table.concat(param, "&")
	HttpRequest(WebEvent.GetUserInfoWeb,_s.URL .. _s.loginUserInfo, _param, post, sucCB,failCB,true)
end

-- 退出
function WebNetHelper.Logout(sucCB,failCB)
	local uid = DataManager.GetUserID()
	local token = DataManager.GetToken()
	local app_id = DataManager.GetAPPID()

	local param = _s._CreateBaseLoginData()
	local c = #param

	param[c + 1] = "token=" .. LuaUtil.MD5(app_id,uid, token, DataManager.SecretKey)
	param[c + 2] = "app_id=" .. app_id
	param[c + 3] = "uid=" .. uid
	param[c + 4] = "access_token=" .. DataManager.GetToken()

	local post = true
	local _param = table.concat(param, "&")
	HttpRequest(WebEvent.LogoutWeb,_s.URL .. _s.logoutAddr, _param, post, sucCB,failCB,true)
end

-- 创建房间
function WebNetHelper.CreateRoom(playID, game_num, player_num, card_type, group_id, sucCB, failCB)
	-- 经过测试，用PostRequest有效，可以减少代码量
	-- local param_pkg = {
	-- 	[1]={key="play_id",value=playID},
	-- 	[2]={key="game_num",value=game_num},
	-- 	[3]={key="player_num",value=player_num},
	-- 	[4]={key="card_type",value=card_type}
	-- }
	-- _s.PostRequest(WebEvent.CreateRoomWeb,_s.createRoomAddr,param_pkg,sucCB,failCB)
	print(playID.." "..game_num.." "..player_num .. " "..card_type)
	local uid = DataManager.GetUserID()
	local token = DataManager.GetToken()
	local app_id = DataManager.GetAPPID()

	local param = _s._CreateBaseLoginData()
	local c = #param

	param[c + 1] = "token=" .. LuaUtil.MD5(app_id,uid, token, DataManager.SecretKey, playID,game_num,player_num,card_type)
	param[c + 2] = "app_id=" .. app_id
	param[c + 3] = "uid=" .. uid
	param[c + 4] = "access_token=" .. token
	param[c + 5] = "play_id=" .. playID
	param[c + 6] = "game_num=" .. game_num
	param[c + 7] = "player_num=" .. player_num
	param[c + 8] = "card_type=" .. card_type   --1：房主支付，2：AA支付
	param[c + 9] = "group_id=" .. group_id

	local post = true
	local _param = table.concat(param, "&")
	HttpRequest(WebEvent.CreateRoomWeb, _s.URL .. _s.createRoomAddr, _param, post, sucCB, failCB,true)
end

-- 加入房间
function WebNetHelper.JoinRoom(roomID, sucCB,failCB)
	local uid = DataManager.GetUserID()
	local token = DataManager.GetToken()
	local app_id = DataManager.GetAPPID()

	local param = _s._CreateBaseLoginData()
	local c = #param

	param[c + 1] = "token=" .. LuaUtil.MD5(app_id,uid, token, DataManager.SecretKey, roomID)
	param[c + 2] = "app_id=" .. app_id
	param[c + 3] = "uid=" .. uid
	param[c + 4] = "room_id=" .. roomID
	param[c + 5] = "access_token=" .. token

	local post = true
	local _param = table.concat(param, "&")
	HttpRequest(WebEvent.JoinRoomWeb,_s.URL .. _s.joinRoomAddr, _param, post, sucCB,failCB,true)
end

--获取邀请进入房间的内容
function WebNetHelper.ShareRoom(roomid, sucCB,failCB )
	local param = _s._CreateBaseLoginData()
	local app_id = DataManager.GetAPPID()
	local uid = DataManager.GetUserID()
	local token = DataManager.GetToken()

	local c = #param
	param[c + 1] = "token=" .. LuaUtil.MD5(app_id,uid, token, DataManager.SecretKey, roomid)
	param[c + 2] = "app_id=" .. app_id
	param[c + 3] = "uid=" .. uid
	param[c + 4] = "room_id=" .. roomid
	param[c + 5] = "access_token=" .. token

	local post = true
	local _param = table.concat(param, "&")
	HttpRequest(WebEvent.ShareRoomWeb,_s.URL .. _s.inviteInfoAddr, _param, post, sucCB,failCB)
end

-- 获取战绩列表
function WebNetHelper.GetPlayRecordList(pageIndex,pageSizeNum,sucCB,failCB, isForce)
	local uid = DataManager.GetUserID()
	local token = DataManager.GetToken()
	local app_id = DataManager.GetAPPID()

	local param = _s._CreateBaseLoginData()
	local c = #param

	param[c + 1] = "token=" .. LuaUtil.MD5(app_id,uid, token, DataManager.SecretKey,pageIndex,pageSizeNum)
	param[c + 2] = "app_id=" .. app_id
	param[c + 3] = "uid=" .. uid
	param[c + 4] = "access_token=" .. token
	param[c + 5] = "page_index=" .. pageIndex
	param[c + 6] = "page_size=" .. pageSizeNum

	local post = true
	local _param = table.concat(param, "&")
	HttpRequest(WebEvent.RecordList,_s.URL .. _s.recordListAddr, _param, post, sucCB,failCB, isForce)
end

-- 获取战绩详情
function WebNetHelper.GetRecordDetail(recordID,sucCB,failCB, isForce)
	local uid = DataManager.GetUserID()
	local token = DataManager.GetToken()
	local app_id = DataManager.GetAPPID()

	local param = _s._CreateBaseLoginData()
	local c = #param

	param[c + 1] = "token=" .. LuaUtil.MD5(app_id,uid, token, DataManager.SecretKey, recordID)
	param[c + 2] = "app_id=" .. app_id
	param[c + 3] = "uid=" .. uid
	param[c + 4] = "access_token=" .. DataManager.GetToken()
	param[c + 5] = "archive_id=" .. recordID

	local post = true
	local _param = table.concat(param, "&")
	HttpRequest(WebEvent.RecordDetail,_s.URL .. _s.recordDetailAddr, _param, post, sucCB,failCB, isForce)
end

-- 获取战绩详情中的回放信息
function WebNetHelper.GetRecordDetailRoundReplayInfo(detailID, sucCB, failCB)
	local uid = DataManager.GetUserID()
	local token = DataManager.GetToken()
	local app_id = DataManager.GetAPPID()
	local param = _s._CreateBaseLoginData()
	local c = #param

	param[c + 1] = "token=" .. LuaUtil.MD5(app_id,uid, token, DataManager.SecretKey, detailID)
	param[c + 2] = "app_id=" .. app_id
	param[c + 3] = "uid=" .. uid
	param[c + 4] = "access_token=" .. token
	param[c + 5] = "detail_id=" .. detailID

	local post = true
	local _param = table.concat(param, "&")
	HttpRequest(WebEvent.RecordDetailRoundReplyInfo,_s.URL .. _s.recordDetailReplyInfoAddr, _param, post, sucCB,failCB)
end

-- 分享游戏
function WebNetHelper.ShareGame(sucCB, failCB)
	local uid = DataManager.GetUserID()
	local token = DataManager.GetToken()
	local app_id = DataManager.GetAPPID()

	local param = _s._CreateBaseLoginData()
	local c = #param

	param[c + 1] = "token=" .. LuaUtil.MD5(app_id,uid, token, DataManager.SecretKey)
	param[c + 2] = "app_id=" .. app_id
	param[c + 3] = "uid=" .. uid
	param[c + 4] = "access_token=" .. token

	local post = true
	local _param = table.concat(param, "&")
	HttpRequest(WebEvent.ShareGame,_s.URL .. _s.shareInfoAddr, _param, post, sucCB,failCB)
end

-- 分享游戏奖励
function WebNetHelper.ShareGameReward(sucCB, failCB)
	local uid = DataManager.GetUserID()
	local token = DataManager.GetToken()
	local app_id = DataManager.GetAPPID()

	local param = _s._CreateBaseLoginData()
	local c = #param

	param[c + 1] = "token=" .. LuaUtil.MD5(app_id, uid, token, DataManager.SecretKey)
	param[c + 2] = "app_id=" .. app_id
	param[c + 3] = "uid=" .. uid
	param[c + 4] = "access_token=" .. token

	local post = true
	local _param = table.concat(param, "&")
	HttpRequest(WebEvent.ShareGameReward, _s.URL .. _s.shareRewardAddr, _param, post, sucCB, failCB)
end

function WebNetHelper.IsShareGameRewarded(sucCB, failCB)
	local uid = DataManager.GetUserID()
	local token = DataManager.GetToken()
	local app_id = DataManager.GetAPPID()

	local param = _s._CreateBaseLoginData()
	local c = #param

	param[c + 1] = "token=" .. LuaUtil.MD5(app_id, uid, token, DataManager.SecretKey)
	param[c + 2] = "app_id=" .. app_id
	param[c + 3] = "uid=" .. uid
	param[c + 4] = "access_token=" .. token

	local post = true
	local _param = table.concat(param, "&")
	HttpRequest(WebEvent.IsShareGameRewarded, _s.URL .. _s.isShareGameRewarded, _param, post, sucCB, failCB)
end

-- 分享单局结算
function WebNetHelper.ShareSingleSettlement(sucCB,failCB)
	local uid = DataManager.GetUserID()
	local token = DataManager.GetToken()
	local app_id = DataManager.GetAPPID()

	local param = _s._CreateBaseLoginData()
	local c = #param

	param[c + 1] = "token=" .. LuaUtil.MD5(app_id,uid, token, DataManager.SecretKey)
	param[c + 2] = "app_id=" .. app_id
	param[c + 3] = "uid=" .. uid
	param[c + 4] = "access_token=" .. token

	local post = true
	local _param = table.concat(param, "&")
	HttpRequest(WebEvent.ShareSingleSettlement,_s.URL .. _s.singleSettlementShareInfo, _param, post, sucCB,failCB)
end

-- 分享总结算
function WebNetHelper.ShareTotalSettlement(sucCB,failCB)
	local uid = DataManager.GetUserID()
	local token = DataManager.GetToken()
	local app_id = DataManager.GetAPPID()

	local param = _s._CreateBaseLoginData()
	local c = #param

	param[c + 1] = "token=" .. LuaUtil.MD5(app_id,uid, token, DataManager.SecretKey)
	param[c + 2] = "app_id=" .. app_id
	param[c + 3] = "uid=" .. uid
	param[c + 4] = "access_token=" .. token

	local post = true
	local _param = table.concat(param, "&")
	HttpRequest(WebEvent.ShareTotalSettlement,_s.URL .. _s.totalSettlementShareInfo, _param, post, sucCB,failCB)
end

-- 获取公告
--[[ @type: 1:活动 2:广告 3:系统喇叭]]
function WebNetHelper.requestNotice( type, suc_cb , fail_cb )
	local uid = DataManager.GetUserID()
	local token = DataManager.GetToken()
	local app_id = DataManager.GetAPPID()

	local param = _s._CreateBaseLoginData()
	local c = #param

	param[c + 1] = "token=" .. LuaUtil.MD5(app_id, uid, token, DataManager.SecretKey, type)
	param[c + 2] = "app_id=" .. app_id

	param[c + 3] = "uid=" .. uid
	param[c + 4] = "access_token=" .. token
	param[c + 5] = "type=" .. type

	local post = true
	local _param = table.concat(param, "&")
	HttpRequest(WebEvent.NoticeList,_s.URL .. _s.noticeList, _param, post,
		suc_cb,fail_cb,true)
	DwDebug.Log("WebNetHelper requestNotice "..type)
end

-- 查询代理
function WebNetHelper.RequestCheckAgent(suc_cb,fail_cb)
	local uid = DataManager.GetUserID()
	local token = DataManager.GetToken()
	local app_id = DataManager.GetAPPID()

	local param = _s._CreateBaseLoginData()
	local c = #param

	param[c + 1] = "token=" .. LuaUtil.MD5(app_id, uid, token, DataManager.SecretKey)
	param[c + 2] = "app_id=" .. app_id

	param[c + 3] = "uid=" .. uid
	param[c + 4] = "access_token=" .. token

	local post = true
	local _param = table.concat(param, "&")
	HttpRequest(WebEvent.UserMyAgent,_s.URL .. _s.myagent, _param, post,
		suc_cb,fail_cb,true)
	DwDebug.Log("WebNetHelper RequestCheckAgent ")
end

-- 绑定代理
function WebNetHelper.RequestBindAgent(agent_id,suc_cb,fail_cb)
	local uid = DataManager.GetUserID()
	local token = DataManager.GetToken()
	local app_id = DataManager.GetAPPID()

	local param = _s._CreateBaseLoginData()
	local c = #param

	param[c + 1] = "token=" .. LuaUtil.MD5(app_id, uid, token, DataManager.SecretKey, agent_id)
	param[c + 2] = "app_id=" .. app_id

	param[c + 3] = "uid=" .. uid
	param[c + 4] = "access_token=" .. token
	param[c + 5] = "agent_id=" .. agent_id

	local post = true
	local _param = table.concat(param, "&")
	HttpRequest(WebEvent.UserBindAgent,_s.URL .. _s.bindagent, _param, post,
		suc_cb,fail_cb,true)
	DwDebug.Log("WebNetHelper RequestBindAgent "..agent_id)
end

-- 拉取IOS商品列表
function WebNetHelper.RequestStoreList(sucCB,failCB)
	local uid = DataManager.GetUserID()
	local token = DataManager.GetToken()
	local app_id = DataManager.GetAPPID()

	local param = _s._CreateBaseLoginData()
	local c = #param

	param[c + 1] = "token=" .. LuaUtil.MD5(app_id, uid, token, DataManager.SecretKey)
	param[c + 2] = "app_id=" .. app_id
	param[c + 3] = "uid=" .. uid
	param[c + 4] = "access_token=" .. token

	local post = true
	local _param = table.concat(param, "&")
	HttpRequest(WebEvent.IOSBuyItemList,_s.URL .. _s.storeListAddr, _param, post,
		sucCB,failCB,true)
end

-- -- 创建订单
-- function WebNetHelper.RequestCreateOrder(goodID,sucCB,failCB)
-- 	local uid = DataManager.GetUserID()
-- 	local token = DataManager.GetToken()
-- 	local app_id = DataManager.GetAPPID()

-- 	local param = _s._CreateBaseLoginData()
-- 	local c = #param

-- 	param[c + 1] = "token=" .. LuaUtil.MD5(app_id, uid, token, DataManager.SecretKey,goodID)
-- 	param[c + 2] = "app_id=" .. app_id
-- 	param[c + 3] = "uid=" .. uid
-- 	param[c + 4] = "access_token=" .. token
-- 	param[c + 5] = "goods_id=" .. goodID

-- 	local post = true
-- 	local _param = table.concat(param, "&")
-- 	HttpRequest(WebEvent.IOSCreateOrder,_s.URL .. _s.createOrderAddr, _param, post,
-- 		sucCB,failCB,true)
-- end

-- 向服务端验证回执
function WebNetHelper.RequestCheckBuyResult(product_id,checkCode,sucCB,failCB)
	local uid = DataManager.GetUserID()
	local token = DataManager.GetToken()
	local app_id = DataManager.GetAPPID()

	local param = _s._CreateBaseLoginData()
	local c = #param

	param[c + 1] = "token=" .. LuaUtil.MD5(app_id, uid, token, DataManager.SecretKey,product_id,checkCode)
	param[c + 2] = "app_id=" .. app_id
	param[c + 3] = "uid=" .. uid
	param[c + 4] = "access_token=" .. token
	param[c + 5] = "receipt_data=" .. checkCode
	param[c + 6] = "product_id=" .. product_id

	local post = true
	local _param = table.concat(param, "&")
	HttpRequest(WebEvent.IOSCheckBuyResult,_s.URL .. _s.checkBuyResultAddr, _param, post,
		sucCB,failCB,true,30)
end

-- 查询服务端是否验证回执成功
function WebNetHelper.RequestSvrCheckStatus(receipt_digest,sucCB,failCB)
	local uid = DataManager.GetUserID()
	local token = DataManager.GetToken()
	local app_id = DataManager.GetAPPID()

	local param = _s._CreateBaseLoginData()
	local c = #param

	param[c + 1] = "token=" .. LuaUtil.MD5(app_id, uid, token, DataManager.SecretKey, receipt_digest)
	param[c + 2] = "app_id=" .. app_id
	param[c + 3] = "uid=" .. uid
	param[c + 4] = "access_token=" .. token
	param[c + 5] = "receipt_digest=" .. receipt_digest

	local post = true
	local _param = table.concat(param, "&")
	HttpRequest(WebEvent.IOSQueryBuyResultStatus,_s.URL .. _s.queryBuyResultStatusAddr, _param, post,
		sucCB,failCB,true)
end



function WebNetHelper._CreateBaseLoginData()
	local param = {}

	local cid = "cid_value"
	param[1] = "cid=" .. cid

	local platform = 3
	if WrapSys.IsAndroid then
		platform = 1
	elseif WrapSys.IsIOS then
		platform = 2
	end
	param[2] = "platform=" .. platform

	local imei = 3--DWPlatformInfo.deviceid
	param[3] = "imei=" .. imei

	local ua = "uaValue"
	param[4] = "ua=" .. ua

	local in_house = "in_house_value"
	param[5] = "in_house=" .. in_house

	local os_version = "os_version_value"
	param[6] = "os_version=" .. os_version

	local app_version = Version.Instance:GetVersion(1)
	param[7] = "app_version=" .. app_version

	return param
end

-- 获取附近的人列表
function WebNetHelper.GetNearByPlayers(pageIndex,pageSizeNum,sucCB,failCB)
	WrapSys.PlatInterface_RefreshLocation()

	local uid = DataManager.GetUserID()
	local token = DataManager.GetToken()
	local app_id = DataManager.GetAPPID()
	local longitude = WrapSys.GetLongitude()
	local latitude = WrapSys.GetLatitude()
	if longitude == "" or latitude == "" then
		WindowUtil.LuaShowTips("未能获取定位信息，请检查是否开启定位权限")
	else
		local param = _s._CreateBaseLoginData()
		local c = #param

		param[c + 1] = "token=" .. LuaUtil.MD5(app_id,uid, token, DataManager.SecretKey,pageIndex,pageSizeNum,longitude,latitude)
		param[c + 2] = "app_id=" .. app_id
		param[c + 3] = "uid=" .. uid
		param[c + 4] = "access_token=" .. token
		param[c + 5] = "page_index=" .. pageIndex
		param[c + 6] = "page_size=" .. pageSizeNum
		param[c + 7] = "longitude=" .. longitude
		param[c + 8] = "latitude=" .. latitude

		local post = true
		local _param = table.concat(param, "&")
		HttpRequest(WebEvent.LbsNearby,_s.URL .. _s.lbsNearbyAddr, _param, post, sucCB,failCB)
	end
end
