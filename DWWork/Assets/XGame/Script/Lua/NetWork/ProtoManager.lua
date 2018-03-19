--------------------------------------------------------------------------------
--   File      : ProtoManager.lua
--   author    : guoliang
--   function   : probuf 协议管理
--   date      : 2017-09-20
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
-- 协议管理器定义
ProtoManager = {}
local _s = ProtoManager
_s.eventMap = {}


-----------------------------------------------web 服---------------------------------------------------------------

-- web pb加载
function ProtoManager.InitWebProto()
	local loadFunc = WrapSys.LuaRootSys_LoadPB
	-- 注册web服pb
	_s._RegisterPB(loadFunc,"web.","v1_errorCode")
	_s._RegisterPB(loadFunc,"web.","v1_usersvr")
	_s._RegisterPB(loadFunc,"web.","v1_gamesvr" )
	_s._RegisterPB(loadFunc,"web.","v1_archivesvr" )
	_s._RegisterPB(loadFunc,"web.","v1_imsvr" )
	_s._RegisterPB(loadFunc,"web.","v1_noticesvr" )
	_s._RegisterPB(loadFunc,"web.","v1_paysvr" )

	--消息映射
	local _RegisterNetMsg = ProtoManager._RegisterNetMsg
	_RegisterNetMsg( WebEvent.LoginWeb, "usersvr.UserLogin" )
	_RegisterNetMsg( WebEvent.LogoutWeb, "usersvr.UserLogout")
	_RegisterNetMsg( WebEvent.GetUserInfoWeb, "usersvr.UserInfo")
	_RegisterNetMsg( WebEvent.NoticeList, "noticesvr.NoticeList")
	_RegisterNetMsg( WebEvent.UserBindAgent, "usersvr.UserBindAgent")
	_RegisterNetMsg( WebEvent.UserMyAgent, "usersvr.UserMyAgent")
	_RegisterNetMsg( WebEvent.CreateRoomWeb, "gamesvr.UserCreateRoom")
	_RegisterNetMsg( WebEvent.JoinRoomWeb, "gamesvr.UserJoinRoom")
	_RegisterNetMsg( WebEvent.RecordList,"archivesvr.ArchiveList")
	_RegisterNetMsg( WebEvent.RecordDetail,"archivesvr.ArchiveDetail")
	_RegisterNetMsg( WebEvent.RecordDetailRoundReplyInfo,"archivesvr.ArchiveDetailRecord")
	_RegisterNetMsg( WebEvent.ShareGame,"gamesvr.UserShareGame")
	_RegisterNetMsg( WebEvent.ShareGameReward,"gamesvr.UserShareGameReward")
	_RegisterNetMsg( WebEvent.ShareRoomWeb,"gamesvr.UserShareRoom")
	_RegisterNetMsg( WebEvent.ShareSingleSettlement ,"gamesvr.UserShareSingleSettlement")
	_RegisterNetMsg( WebEvent.ShareTotalSettlement,"gamesvr.UserShareTotalSettlement")
	_RegisterNetMsg( WebEvent.LbsNearby, "usersvr.LbsNearby" )
	_RegisterNetMsg( WebEvent.IsShareGameRewarded, "gamesvr.UserIsShareGameRewarded")

	_RegisterNetMsg( WebEvent.IOSBuyItemList,"paysvr.AppleGoods")
	_RegisterNetMsg( WebEvent.IOSCreateOrder ,"paysvr.AppleCreateOrder")
	_RegisterNetMsg( WebEvent.IOSCheckBuyResult,"paysvr.AppleVerifyReceipt")
	_RegisterNetMsg( WebEvent.IOSQueryBuyResultStatus,"paysvr.AppleCheckStatus")

	--Web_ERRNO = lua_pb.get_enum("errorcode.Web_ERRNO")
end


----------------------------------用户通讯服-----------------------------------------------------
--初始化用户通讯服协议
function ProtoManager.InitUserSvrProto()
	local loadFunc = WrapSys.LuaRootSys_LoadPB
	-- 注册通讯服pb
	_s._RegisterPB(loadFunc,"com_server.","CMD")
	_s._RegisterPB(loadFunc,"com_server.","error_code")
	_s._RegisterPB(loadFunc,"com_server.","login")
	_s._RegisterPB(loadFunc,"com_server.","group_lobby")
	_s._RegisterPB(loadFunc,"com_server.","group_ui")
	_s._RegisterPB(loadFunc,"com_server.","heartbeat")
	_s._RegisterPB(loadFunc,"com_server.","notice")
	_s._RegisterPB(loadFunc,"com_server.","play_info")
	_s._RegisterPB(loadFunc,"com_server.","return")
	_s._RegisterPB(loadFunc,"com_server.","room_info")
	_s._RegisterPB(loadFunc,"com_server.","play_mail")
	_s._RegisterPB(loadFunc,"com_server.","group_card_warn")
	-- 注册web服pb
	_s._RegisterPB(loadFunc,"web.","v1_groupsvr")
	_s._RegisterPB(loadFunc,"web.","v1_comsvr")
	_s._RegisterPB(loadFunc,"web.","webcommon_messagecontent")
	-- 初始化协议头和错误码
	USER_SVR_CMD = lua_pb.get_enum("event_user_svr.USER_CMD")
	USER_SVR_ERRNO = lua_pb.get_enum("event_user_svr.USER_ERRNO")

	-- 通讯服消息映射
	local _RegisterNetMsg = ProtoManager._RegisterNetMsg
	_RegisterNetMsg( USER_SVR_CMD.CS_LOGIN, "event_user_svr.EventLogin" )
	_RegisterNetMsg( USER_SVR_CMD.SC_COMMON_RSP, "event_user_svr.CReturn" )
	_RegisterNetMsg( USER_SVR_CMD.CS_HEART_BEAT, "event_user_svr.CHeartBeat" )
	_RegisterNetMsg( USER_SVR_CMD.CS_HALL_STATUS_REPORT, "event_user_svr.CGroupLobby" )
	_RegisterNetMsg( USER_SVR_CMD.SC_PLAYER_STATUS_NOTIFY, "event_user_svr.CPlayStatusNotice" )
	_RegisterNetMsg( USER_SVR_CMD.SC_ROOM_STATUS_NOTIFY, "event_user_svr.CRoomInfo" )
	_RegisterNetMsg( USER_SVR_CMD.CS_UI_STATUS_REPORT, "event_user_svr.CGroupUI" )
	-- 俱乐部广播
	_RegisterNetMsg( USER_SVR_CMD.BC_GROUP_GET_NEW_MEMBER, "messagecontent.BroadcastGroupNewUser" )
	_RegisterNetMsg( USER_SVR_CMD.BC_QUIT_GROUP, "messagecontent.BroadcastQuitGroup" )
	_RegisterNetMsg( USER_SVR_CMD.BC_GROUP_KICKOUT, "messagecontent.BroadcastKickGroupUser" )
	_RegisterNetMsg( USER_SVR_CMD.BC_GROUP_DISMISSED, "messagecontent.BroadcastDeleteGroup" )


	_RegisterNetMsg(WebEvent.UseNetBaseInfo,"groupsvr.CommunicateSvr")
	-- 俱乐部的web协议消息隐射
	_RegisterNetMsg( WebEvent.ClubList, "groupsvr.GroupList" )
	_RegisterNetMsg( WebEvent.ClubDetail, "groupsvr.GroupInfo" )
	_RegisterNetMsg( WebEvent.CreateClub, "groupsvr.GroupAdd" )
	_RegisterNetMsg( WebEvent.UpdateClub, "groupsvr.GroupUpdate" )
	_RegisterNetMsg( WebEvent.DeleteClub, "groupsvr.GroupDelete" )
	_RegisterNetMsg( WebEvent.ClubMemList, "groupsvr.GroupUserList" )
	_RegisterNetMsg( WebEvent.ClubMemDetail, "groupsvr.GroupUserInfo" )
	_RegisterNetMsg( WebEvent.ApplyJoinClub, "groupsvr.GroupUserApplyJoin" )
	_RegisterNetMsg( WebEvent.HandleApplyClub, "groupsvr.GroupUserHandleApply" )
	_RegisterNetMsg( WebEvent.KickoutClub, "groupsvr.GroupUserKickout" )
	_RegisterNetMsg( WebEvent.QuitClub, "groupsvr.GroupUserQuit" )

	_RegisterNetMsg( WebEvent.TemplateList, "groupsvr.RoomTplList" )
	_RegisterNetMsg( WebEvent.TemplateDetail, "groupsvr.RoomTplInfo" )
	_RegisterNetMsg( WebEvent.CreateTemplate, "groupsvr.RoomCreateTpl" )
	_RegisterNetMsg( WebEvent.UpdateTemplate, "groupsvr.RoomUpdateTpl" )
	_RegisterNetMsg( WebEvent.DeleteTemplate, "groupsvr.RoomDeleteTpl" )
	_RegisterNetMsg( WebEvent.RoomList, "groupsvr.RoomList" )
	_RegisterNetMsg( WebEvent.ArchiveList, "groupsvr.ArchiveList" )
	_RegisterNetMsg( WebEvent.ArchiveDetail, "groupsvr.ArchiveDetail" )
	_RegisterNetMsg( WebEvent.ArchiveDetailRecord, "groupsvr.ArchiveDetailRecord" )
	_RegisterNetMsg( WebEvent.CardFlow, "groupsvr.CardFlow" )
	_RegisterNetMsg( WebEvent.StatisticsPlayed, "groupsvr.StatisticPlayed" )
	_RegisterNetMsg( WebEvent.RechargeClub, "groupsvr.GroupRecharge")

	-- 消息分类列表
	_RegisterNetMsg( WebEvent.MsgList, "imsvr.MessageList" )
	-- 邮件
	_RegisterNetMsg( WebEvent.MailCPlayMail, "event_user_svr.CPlayMail" )
	_RegisterNetMsg( WebEvent.MailCGroupCardWarn, "event_user_svr.CGroupCardWarn" )
	_RegisterNetMsg( WebEvent.MsgMailMessageIsReadAddr, "imsvr.MessageProcessed" )
	_RegisterNetMsg( WebEvent.MailGroupApplyJoin, "messagecontent.MailGroupApplyJoin")
	_RegisterNetMsg( WebEvent.MailCommon, "messagecontent.MailCommon")
end


-------------------------------------------5 10 k--------------------------------------------------------------
--初始化标志
_s.isInitWSKFlagTable = {}
-- 初始化崇仁WSK proto 入口
function ProtoManager.InitWSKProto(playID)
	DwDebug.Log("InitWSKProto"..playID)
	if _s.isInitWSKFlagTable[playID] == nil then
		_s.isInitWSKFlagTable[playID] = true
		if playID == Common_PlayID.chongRen_510K then
			_s.InitChongRenWSKGamePB()
			-- 初始化协议头和错误码
			GAME_CMD_CR_WSK = lua_pb.get_enum("event_chongren_wsk.GAME_CMD")
			GAME_ERRNO_CR_ERRNO = lua_pb.get_enum("event_chongren_wsk.GAME_ERRNO")

			_s.InitChongRenWSKGamePBMap()
		elseif playID == Common_PlayID.leAn_510K then
			_s.InitLeAnWSKGamePB()
			-- 初始化协议头和错误码
			GAME_CMD_LEAN_WSK = lua_pb.get_enum("event_lean_wsk.GAME_CMD")
			GAME_ERRNO_LEAN_WSK = lua_pb.get_enum("event_lean_wsk.GAME_ERRNO")

			_s.InitLeAnWSKGamePBMap()
		elseif playID == Common_PlayID.yiHuang_510K then
			_s.InitYiHuangWSKGamePB()
			-- 初始化协议头和错误码
			GAME_CMD_YIHUANG_WSK = lua_pb.get_enum("event_yihuang_wsk.GAME_CMD")
			GAME_ERRNO_YIHUANG_WSK = lua_pb.get_enum("event_yihuang_wsk.GAME_ERRNO")

			_s.InitYiHuangWSKGamePBMap()
		end
	end

	if playID == Common_PlayID.chongRen_510K then
		GAME_CMD = GAME_CMD_CR_WSK
		GAME_ERRNO = GAME_ERRNO_CR_ERRNO
		ProtoManager._RegisterNetMsg( WebEvent.RecordGameSvrInfo,"event_chongren_wsk.PBReplay")
	elseif playID == Common_PlayID.leAn_510K then
		GAME_CMD = GAME_CMD_LEAN_WSK
		GAME_ERRNO = GAME_ERRNO_LEAN_WSK
		ProtoManager._RegisterNetMsg( WebEvent.RecordGameSvrInfo,"event_lean_wsk.PBReplay")
	elseif playID == Common_PlayID.yiHuang_510K then
		GAME_CMD = GAME_CMD_YIHUANG_WSK
		GAME_ERRNO = GAME_ERRNO_YIHUANG_WSK
		ProtoManager._RegisterNetMsg( WebEvent.RecordGameSvrInfo,"event_yihuang_wsk.PBReplay")
	end
end

--初始化崇仁510K游戏服PB
function ProtoManager.InitChongRenWSKGamePB()
	local loadFunc = WrapSys.LuaRootSys_LoadPB
	--游戏服pb
	_s._RegisterPB(loadFunc,"chongren_wsk.","CMD")
	_s._RegisterPB(loadFunc,"chongren_wsk.","error_code")
	_s._RegisterPB(loadFunc,"chongren_wsk.","ask_alone")
	_s._RegisterPB(loadFunc,"chongren_wsk.","ask_friend")
	_s._RegisterPB(loadFunc,"chongren_wsk.","ask_out")
	_s._RegisterPB(loadFunc,"chongren_wsk.","ask_skip")
	_s._RegisterPB(loadFunc,"chongren_wsk.","assign_pai")
	_s._RegisterPB(loadFunc,"chongren_wsk.","clear_desktop")
	_s._RegisterPB(loadFunc,"chongren_wsk.","create_room")
	_s._RegisterPB(loadFunc,"chongren_wsk.","dismass_room")
	_s._RegisterPB(loadFunc,"chongren_wsk.","distance_alert")
	_s._RegisterPB(loadFunc,"chongren_wsk.","exit")
	_s._RegisterPB(loadFunc,"chongren_wsk.","game_info")
--	_s._RegisterPB(loadFunc,"chongren_wsk.","game_record")
	_s._RegisterPB(loadFunc,"chongren_wsk.","geo")
	_s._RegisterPB(loadFunc,"chongren_wsk.","heartbeat")
	_s._RegisterPB(loadFunc,"chongren_wsk.","hint_alone")
	_s._RegisterPB(loadFunc,"chongren_wsk.","hint_friend")
	_s._RegisterPB(loadFunc,"chongren_wsk.","hint_out")
	_s._RegisterPB(loadFunc,"chongren_wsk.","join_room")
	_s._RegisterPB(loadFunc,"chongren_wsk.","lbs")
	_s._RegisterPB(loadFunc,"chongren_wsk.","login")
	_s._RegisterPB(loadFunc,"chongren_wsk.","online_status")
	_s._RegisterPB(loadFunc,"chongren_wsk.","re_conn")
	_s._RegisterPB(loadFunc,"chongren_wsk.","ready")
	_s._RegisterPB(loadFunc,"chongren_wsk.","render_ui")
	_s._RegisterPB(loadFunc,"chongren_wsk.","return")
	_s._RegisterPB(loadFunc,"chongren_wsk.","room_info")
	_s._RegisterPB(loadFunc,"chongren_wsk.","seat_info")
	_s._RegisterPB(loadFunc,"chongren_wsk.","kick_out")
	_s._RegisterPB(loadFunc,"chongren_wsk.","small_settlement")
	_s._RegisterPB(loadFunc,"chongren_wsk.","tanpai")
	_s._RegisterPB(loadFunc,"chongren_wsk.","total_settlement")
	_s._RegisterPB(loadFunc,"chongren_wsk.","use_alone")
	_s._RegisterPB(loadFunc,"chongren_wsk.","use_friend")
	_s._RegisterPB(loadFunc,"chongren_wsk.","use_out")
	_s._RegisterPB(loadFunc,"chongren_wsk.","history_score")
	_s._RegisterPB(loadFunc,"chongren_wsk.","skip_alone")
	--510K回放
	_s._RegisterPB(loadFunc,"chongren_wsk.","replay")
	_s._RegisterPB(loadFunc,"chongren_wsk.","replay_game")
end

--初始化崇仁510K游戏服消息-PB 映射
function ProtoManager.InitChongRenWSKGamePBMap()
	local _RegisterNetMsg = ProtoManager._RegisterNetMsg

	_RegisterNetMsg( GAME_CMD_CR_WSK.CS_LOGIN, "event_chongren_wsk.EventLogin" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_COMMON_RSP, "event_chongren_wsk.EventReturn")
	_RegisterNetMsg( GAME_CMD_CR_WSK.CS_CREATE_ROOM, "event_chongren_wsk.PBCreateRoom")
	_RegisterNetMsg( GAME_CMD_CR_WSK.CS_JOIN_ROOM, "event_chongren_wsk.EventJoinRoom" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.CS_HEART_BEAT, "event_chongren_wsk.EventHeartBeat" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_SEAT_INFO_PUSH, "event_chongren_wsk.EventSeatInfo" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.CS_RENDER_UI_SUCCESS, "event_chongren_wsk.EventRenderUI" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.CS_PREPARE, "event_chongren_wsk.EventReady" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_PREPARE_PUSH, "event_chongren_wsk.EventReadyReturn" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_PLAYER_ONLINE_PUSH, "event_chongren_wsk.EventOnlineStatus" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.CS_NET_RECONNECT, "event_chongren_wsk.EventReconn" )
--	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_START_GAMER_PUSH, "event_chongren_wsk.EventLogin" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_ROOM_INFO, "event_chongren_wsk.EventRoomInfo" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_GAME_INFO_PUSH, "event_chongren_wsk.PBGameInfo" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.CS_ASK_DISMISS, "event_chongren_wsk.EventAskDismassRoom" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_NOTIFY_VOTE_PUSH, "event_chongren_wsk.EventQueryDismassRoom" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.CS_PLAYER_VOTE, "event_chongren_wsk.EventReportDismassRoom" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_NOTIFY_DISMISS_RESULT_PUSH, "event_chongren_wsk.EventNoticeDismassRoom" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_DISMISS, "event_chongren_wsk.EventDismassRoom" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.CS_ASK_LBS, "event_chongren_wsk.EventGeo" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_ASK_LBS_REPLY, "event_chongren_wsk.PBGeo" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.CS_ASK_QUIT_ROOM, "event_chongren_wsk.EventExit" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_NOTIFY_QUIT_ROOM_PUSH, "event_chongren_wsk.EventExitStatus" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_NOTIFY_KICK_PUSH, "event_chongren_wsk.EventKickOut" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.CS_ASK_KICK_OUT, "event_chongren_wsk.EventAskKick")
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_ASK_KICK_OUT_REPLY, "event_chongren_wsk.EventKickNotice")
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_SYSTEM_DEAL_CARD_PUSH, "event_chongren_wsk.PBAssignPai" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_NOTIFY_IS_ALONE_PUSH, "event_chongren_wsk.PBHintAlone" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.CS_REPLY_ALONE, "event_chongren_wsk.PBAskAlone" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_ALONE_PLAY_PUSH, "event_chongren_wsk.PBUseAlone" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_NOTIFY_SHOW_CARD_PUSH, "event_chongren_wsk.PBHintFriend" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.CS_PLAYER_SHOW_CARD, "event_chongren_wsk.PBAskFriend" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_SVR_SHOW_CARD_PUSH, "event_chongren_wsk.PBUseFriend" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_NOTIFY_PLAY_CARD_PUSH, "event_chongren_wsk.PBHintOut" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.CS_PLAYER_PLAY_CARD, "event_chongren_wsk.PBAskOut" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_SVR_PLAY_CARD_PUSH, "event_chongren_wsk.PBUseOut" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_CARD_COMPARE_PUSH, "event_chongren_wsk.PBHintCompare" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.CS_PLAYER_NOT_PLAY, "event_chongren_wsk.PBAskSkip" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_CLEAR_CARDS_PUSH, "event_chongren_wsk.PBClearDesktop" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_SHOW_ALL_CARDS_PUSH, "event_chongren_wsk.PBTanPai" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_SMALL_RESULT_PUSH, "event_chongren_wsk.PBSmallSettlement" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_BIG_RESULT_PUSH, "event_chongren_wsk.PBTotalSettlement" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_RECORD_GAME_INFO_PUSH, "event_chongren_wsk.PBReplayGame")
	_RegisterNetMsg( GAME_CMD_CR_WSK.CS_HISTORY_SCORE, "event_chongren_wsk.EventAskHistoryScore" )
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_HISTORY_SCORE_PUSH, "event_chongren_wsk.EventUseHistoryScore")
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_DISTANCE_TIPS_PUSH,"event_chongren_wsk.EventDistanceAlert")
	_RegisterNetMsg( GAME_CMD_CR_WSK.SC_REPLY_NOT_ALONE_PUSH,"event_chongren_wsk.PBSkipAlone")
end

-- 乐安510K

--初始化乐安510K游戏服PB
function ProtoManager.InitLeAnWSKGamePB()
	local loadFunc = WrapSys.LuaRootSys_LoadPB
	--游戏服pb
	_s._RegisterPB(loadFunc,"lean_wsk.","CMD")
	_s._RegisterPB(loadFunc,"lean_wsk.","error_code")
	_s._RegisterPB(loadFunc,"lean_wsk.","ask_alone")
	_s._RegisterPB(loadFunc,"lean_wsk.","ask_friend")
	_s._RegisterPB(loadFunc,"lean_wsk.","ask_out")
	_s._RegisterPB(loadFunc,"lean_wsk.","ask_skip")
	_s._RegisterPB(loadFunc,"lean_wsk.","assign_pai")
	_s._RegisterPB(loadFunc,"lean_wsk.","clear_desktop")
	_s._RegisterPB(loadFunc,"lean_wsk.","create_room")
	_s._RegisterPB(loadFunc,"lean_wsk.","dismass_room")
	_s._RegisterPB(loadFunc,"lean_wsk.","distance_alert")
	_s._RegisterPB(loadFunc,"lean_wsk.","exit")
	_s._RegisterPB(loadFunc,"lean_wsk.","game_info")
--	_s._RegisterPB(loadFunc,"lean_wsk.","game_record")
	_s._RegisterPB(loadFunc,"lean_wsk.","geo")
	_s._RegisterPB(loadFunc,"lean_wsk.","heartbeat")
	_s._RegisterPB(loadFunc,"lean_wsk.","hint_alone")
	_s._RegisterPB(loadFunc,"lean_wsk.","hint_friend")
	_s._RegisterPB(loadFunc,"lean_wsk.","hint_out")
	_s._RegisterPB(loadFunc,"lean_wsk.","join_room")
	_s._RegisterPB(loadFunc,"lean_wsk.","lbs")
	_s._RegisterPB(loadFunc,"lean_wsk.","login")
	_s._RegisterPB(loadFunc,"lean_wsk.","online_status")
	_s._RegisterPB(loadFunc,"lean_wsk.","re_conn")
	_s._RegisterPB(loadFunc,"lean_wsk.","ready")
	_s._RegisterPB(loadFunc,"lean_wsk.","render_ui")
	_s._RegisterPB(loadFunc,"lean_wsk.","return")
	_s._RegisterPB(loadFunc,"lean_wsk.","room_info")
	_s._RegisterPB(loadFunc,"lean_wsk.","seat_info")
	_s._RegisterPB(loadFunc,"lean_wsk.","kick_out")
	_s._RegisterPB(loadFunc,"lean_wsk.","small_settlement")
	_s._RegisterPB(loadFunc,"lean_wsk.","tanpai")
	_s._RegisterPB(loadFunc,"lean_wsk.","total_settlement")
	_s._RegisterPB(loadFunc,"lean_wsk.","use_alone")
	_s._RegisterPB(loadFunc,"lean_wsk.","use_friend")
	_s._RegisterPB(loadFunc,"lean_wsk.","use_out")
	_s._RegisterPB(loadFunc,"lean_wsk.","history_score")
	_s._RegisterPB(loadFunc,"lean_wsk.","skip_alone")
	--510K回放
	_s._RegisterPB(loadFunc,"lean_wsk.","replay")
	_s._RegisterPB(loadFunc,"lean_wsk.","replay_game")
end

--初始化乐安510K游戏服消息-PB 映射
function ProtoManager.InitLeAnWSKGamePBMap()
	local _RegisterNetMsg = ProtoManager._RegisterNetMsg

	_RegisterNetMsg( GAME_CMD_LEAN_WSK.CS_LOGIN, "event_lean_wsk.EventLogin" )

	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_COMMON_RSP, "event_lean_wsk.EventReturn")
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.CS_CREATE_ROOM, "event_lean_wsk.PBCreateRoom")
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.CS_JOIN_ROOM, "event_lean_wsk.EventJoinRoom" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.CS_HEART_BEAT, "event_lean_wsk.EventHeartBeat" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_SEAT_INFO_PUSH, "event_lean_wsk.EventSeatInfo" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.CS_RENDER_UI_SUCCESS, "event_lean_wsk.EventRenderUI" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.CS_PREPARE, "event_lean_wsk.EventReady" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_PREPARE_PUSH, "event_lean_wsk.EventReadyReturn" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_PLAYER_ONLINE_PUSH, "event_lean_wsk.EventOnlineStatus" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.CS_NET_RECONNECT, "event_lean_wsk.EventReconn" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_ROOM_INFO, "event_lean_wsk.EventRoomInfo" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_GAME_INFO_PUSH, "event_lean_wsk.PBGameInfo" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.CS_ASK_DISMISS, "event_lean_wsk.EventAskDismassRoom" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_NOTIFY_VOTE_PUSH, "event_lean_wsk.EventQueryDismassRoom" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.CS_PLAYER_VOTE, "event_lean_wsk.EventReportDismassRoom" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_NOTIFY_DISMISS_RESULT_PUSH, "event_lean_wsk.EventNoticeDismassRoom" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_DISMISS, "event_lean_wsk.EventDismassRoom" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.CS_ASK_LBS, "event_lean_wsk.EventGeo" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_ASK_LBS_REPLY, "event_lean_wsk.PBGeo" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.CS_ASK_QUIT_ROOM, "event_lean_wsk.EventExit" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_NOTIFY_QUIT_ROOM_PUSH, "event_lean_wsk.EventExitStatus" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_NOTIFY_KICK_PUSH, "event_lean_wsk.EventKickOut" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.CS_ASK_KICK_OUT, "event_lean_wsk.EventAskKick")
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_ASK_KICK_OUT_REPLY, "event_lean_wsk.EventKickNotice")
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_SYSTEM_DEAL_CARD_PUSH, "event_lean_wsk.PBAssignPai" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_NOTIFY_IS_ALONE_PUSH, "event_lean_wsk.PBHintAlone" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.CS_REPLY_ALONE, "event_lean_wsk.PBAskAlone" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_ALONE_PLAY_PUSH, "event_lean_wsk.PBUseAlone" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_NOTIFY_SHOW_CARD_PUSH, "event_lean_wsk.PBHintFriend" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.CS_PLAYER_SHOW_CARD, "event_lean_wsk.PBAskFriend" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_SVR_SHOW_CARD_PUSH, "event_lean_wsk.PBUseFriend" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_NOTIFY_PLAY_CARD_PUSH, "event_lean_wsk.PBHintOut" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.CS_PLAYER_PLAY_CARD, "event_lean_wsk.PBAskOut" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_SVR_PLAY_CARD_PUSH, "event_lean_wsk.PBUseOut" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_CARD_COMPARE_PUSH, "event_lean_wsk.PBHintCompare" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.CS_PLAYER_NOT_PLAY, "event_lean_wsk.PBAskSkip" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_CLEAR_CARDS_PUSH, "event_lean_wsk.PBClearDesktop" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_SHOW_ALL_CARDS_PUSH, "event_lean_wsk.PBTanPai" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_SMALL_RESULT_PUSH, "event_lean_wsk.PBSmallSettlement" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_BIG_RESULT_PUSH, "event_lean_wsk.PBTotalSettlement" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_RECORD_GAME_INFO_PUSH, "event_lean_wsk.PBReplayGame")
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.CS_HISTORY_SCORE, "event_lean_wsk.EventAskHistoryScore" )
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_HISTORY_SCORE_PUSH, "event_lean_wsk.EventUseHistoryScore")
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_DISTANCE_TIPS_PUSH,"event_lean_wsk.EventDistanceAlert")
	_RegisterNetMsg( GAME_CMD_LEAN_WSK.SC_REPLY_NOT_ALONE_PUSH,"event_lean_wsk.PBSkipAlone")
end


-- 宜黄510K
--初始化宜黄510K游戏服PB
function ProtoManager.InitYiHuangWSKGamePB()
	local loadFunc = WrapSys.LuaRootSys_LoadPB
	--游戏服pb
	_s._RegisterPB(loadFunc,"yihuang_wsk.","CMD")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","error_code")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","ask_alone")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","ask_friend")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","ask_out")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","ask_skip")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","assign_pai")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","clear_desktop")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","create_room")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","dismass_room")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","distance_alert")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","exit")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","game_info")
--	_s._RegisterPB(loadFunc,"yihuang_wsk.","game_record")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","geo")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","heartbeat")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","hint_alone")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","hint_friend")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","hint_out")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","join_room")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","lbs")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","login")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","online_status")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","re_conn")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","ready")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","render_ui")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","return")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","room_msg")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","seat_info")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","kick_out")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","small_settlement")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","tanpai")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","total_settlement")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","use_alone")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","use_friend")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","use_out")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","history_score")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","skip_alone")
	--510K回放
	_s._RegisterPB(loadFunc,"yihuang_wsk.","replay")
	_s._RegisterPB(loadFunc,"yihuang_wsk.","replay_game")

end

--初始化宜黄510K游戏服消息-PB 映射
function ProtoManager.InitYiHuangWSKGamePBMap()
	local _RegisterNetMsg = ProtoManager._RegisterNetMsg

	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.CS_LOGIN, "event_yihuang_wsk.EventLogin" )

	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_COMMON_RSP, "event_yihuang_wsk.EventReturn")
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.CS_CREATE_ROOM, "event_yihuang_wsk.PBCreateRoom")
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.CS_JOIN_ROOM, "event_yihuang_wsk.EventJoinRoom" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.CS_HEART_BEAT, "event_yihuang_wsk.EventHeartBeat" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_SEAT_INFO_PUSH, "event_yihuang_wsk.EventSeatInfo" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.CS_RENDER_UI_SUCCESS, "event_yihuang_wsk.EventRenderUI" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.CS_PREPARE, "event_yihuang_wsk.EventReady" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_PREPARE_PUSH, "event_yihuang_wsk.EventReadyReturn" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_PLAYER_ONLINE_PUSH, "event_yihuang_wsk.EventOnlineStatus" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.CS_NET_RECONNECT, "event_yihuang_wsk.EventReconn" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_ROOM_INFO, "event_yihuang_wsk.PBRoomMsg" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_GAME_INFO_PUSH, "event_yihuang_wsk.PBGameInfo" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.CS_ASK_DISMISS, "event_yihuang_wsk.EventAskDismassRoom" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_NOTIFY_VOTE_PUSH, "event_yihuang_wsk.EventQueryDismassRoom" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.CS_PLAYER_VOTE, "event_yihuang_wsk.EventReportDismassRoom" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_NOTIFY_DISMISS_RESULT_PUSH, "event_yihuang_wsk.EventNoticeDismassRoom" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_DISMISS, "event_yihuang_wsk.EventDismassRoom" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.CS_ASK_LBS, "event_yihuang_wsk.EventGeo" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_ASK_LBS_REPLY, "event_yihuang_wsk.PBGeo" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.CS_ASK_QUIT_ROOM, "event_yihuang_wsk.EventExit" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_NOTIFY_QUIT_ROOM_PUSH, "event_yihuang_wsk.EventExitStatus" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_NOTIFY_KICK_PUSH, "event_yihuang_wsk.EventKickOut" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.CS_ASK_KICK_OUT, "event_yihuang_wsk.EventAskKick")
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_ASK_KICK_OUT_REPLY, "event_yihuang_wsk.EventKickNotice")
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_SYSTEM_DEAL_CARD_PUSH, "event_yihuang_wsk.PBAssignPai" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_NOTIFY_IS_ALONE_PUSH, "event_yihuang_wsk.PBHintAlone" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.CS_REPLY_ALONE, "event_yihuang_wsk.PBAskAlone" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_ALONE_PLAY_PUSH, "event_yihuang_wsk.PBUseAlone" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_NOTIFY_SHOW_CARD_PUSH, "event_yihuang_wsk.PBHintFriend" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.CS_PLAYER_SHOW_CARD, "event_yihuang_wsk.PBAskFriend" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_SVR_SHOW_CARD_PUSH, "event_yihuang_wsk.PBUseFriend" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_NOTIFY_PLAY_CARD_PUSH, "event_yihuang_wsk.PBHintOut" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.CS_PLAYER_PLAY_CARD, "event_yihuang_wsk.PBAskOut" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_SVR_PLAY_CARD_PUSH, "event_yihuang_wsk.PBUseOut" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_CARD_COMPARE_PUSH, "event_yihuang_wsk.PBHintCompare" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.CS_PLAYER_NOT_PLAY, "event_yihuang_wsk.PBAskSkip" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_CLEAR_CARDS_PUSH, "event_yihuang_wsk.PBClearDesktop" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_SHOW_ALL_CARDS_PUSH, "event_yihuang_wsk.PBTanPai" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_SMALL_RESULT_PUSH, "event_yihuang_wsk.PBSmallSettlement" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_BIG_RESULT_PUSH, "event_yihuang_wsk.PBTotalSettlement" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_RECORD_GAME_INFO_PUSH, "event_yihuang_wsk.PBReplayGame")
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.CS_HISTORY_SCORE, "event_yihuang_wsk.EventAskHistoryScore" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_HISTORY_SCORE_PUSH, "event_yihuang_wsk.EventUseHistoryScore")
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_DISTANCE_TIPS_PUSH,"event_yihuang_wsk.EventDistanceAlert")
	_RegisterNetMsg( GAME_CMD_YIHUANG_WSK.SC_REPLY_NOT_ALONE_PUSH,"event_yihuang_wsk.PBSkipAlone")
end

-------------------------------------------32张----------------------------------------------------------------------
--初始化标志
_s.isInitThirtyTwoFlagTable = {}
-- 初始化32张 proto 入口
function ProtoManager.InitThirtyTwoProto(playID)
	DwDebug.Log("Init32Proto"..playID)
	if _s.isInitThirtyTwoFlagTable[playID] == nil then
		_s.isInitThirtyTwoFlagTable[playID] = true
		if playID == Common_PlayID.ThirtyTwo then
			_s.InitThirtyTwoGamePB()
			-- 初始化协议头和错误码
			GAME_CMD_THIRTYTWO = lua_pb.get_enum("event_common_thirtytwo.GAME_CMD")
			GAME_ERRNO_CR_ERRNO = lua_pb.get_enum("event_common_thirtytwo.GAME_ERRNO")

			_s.InitThirtyTwoGamePBMap()
		end
	end

	if playID == Common_PlayID.ThirtyTwo then
		GAME_CMD = GAME_CMD_THIRTYTWO
		GAME_ERRNO = GAME_ERRNO_CR_ERRNO
		ProtoManager._RegisterNetMsg(WebEvent.RecordGameSvrInfo,"event_common_thirtytwo.PBReplay")
	end
end

--初始化32张游戏服PB
function ProtoManager.InitThirtyTwoGamePB()
	local loadFunc = WrapSys.LuaRootSys_LoadPB
	--游戏服pb
	_s._RegisterPB(loadFunc,"common_thirtytwo.","CMD")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","error_code")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","ask_bet")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","ask_open")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","ask_shuffle")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","assign_pai")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","create_room")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","dismass_room")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","distance_alert")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","exit")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","game_info")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","game_record")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","geo")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","heartbeat")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","hint_bet")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","hint_shuffle")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","history_score")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","join_room")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","kick_out")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","lbs")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","login")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","online_status")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","re_conn")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","ready")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","render_ui")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","return")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","room_msg")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","seat_info")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","small_settlement")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","total_settlement")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","use_bet")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","use_open")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","use_shuffle")
	--32张回放
	_s._RegisterPB(loadFunc,"common_thirtytwo.","replay")
	_s._RegisterPB(loadFunc,"common_thirtytwo.","replay_game")
end

--初始化32张游戏服消息-PB 映射
function ProtoManager.InitThirtyTwoGamePBMap()
	local _RegisterNetMsg = ProtoManager._RegisterNetMsg

	_RegisterNetMsg( GAME_CMD_THIRTYTWO.CS_LOGIN, "event_common_thirtytwo.EventLogin" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_COMMON_RSP, "event_common_thirtytwo.EventReturn")
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.CS_CREATE_ROOM, "event_common_thirtytwo.PBCreateRoom")
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.CS_JOIN_ROOM, "event_common_thirtytwo.EventJoinRoom" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.CS_HEART_BEAT, "event_common_thirtytwo.EventHeartBeat" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_SEAT_INFO_PUSH, "event_common_thirtytwo.EventSeatInfo" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.CS_RENDER_UI_SUCCESS, "event_common_thirtytwo.EventRenderUI" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.CS_PREPARE, "event_common_thirtytwo.EventReady" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_PREPARE_PUSH, "event_common_thirtytwo.EventReadyReturn" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_PLAYER_ONLINE_PUSH, "event_common_thirtytwo.EventOnlineStatus" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.CS_NET_RECONNECT, "event_common_thirtytwo.EventReconn" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_START_GAMER_PUSH, "event_common_thirtytwo.EventStartGame" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_ROOM_INFO, "event_common_thirtytwo.PBRoomMsg" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_GAME_INFO_PUSH, "event_common_thirtytwo.PBGameInfo" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.CS_ASK_DISMISS, "event_common_thirtytwo.EventAskDismassRoom" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_NOTIFY_VOTE_PUSH, "event_common_thirtytwo.EventQueryDismassRoom" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.CS_PLAYER_VOTE, "event_common_thirtytwo.EventReportDismassRoom" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_NOTIFY_DISMISS_RESULT_PUSH, "event_common_thirtytwo.EventNoticeDismassRoom" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.CS_ASK_LBS, "event_common_thirtytwo.EventGeo" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_ASK_LBS_REPLY, "event_common_thirtytwo.PBGeo" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.CS_ASK_QUIT_ROOM, "event_common_thirtytwo.EventExit" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_NOTIFY_QUIT_ROOM_PUSH, "event_common_thirtytwo.EventExitStatus" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_NOTIFY_KICK_PUSH, "event_common_thirtytwo.EventKickOut" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.CS_HISTORY_SCORE, "event_common_thirtytwo.EventAskHistoryScore" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_HISTORY_SCORE_PUSH, "event_common_thirtytwo.EventUseHistoryScore")
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_DISTANCE_TIPS_PUSH, "event_common_thirtytwo.EventDistanceAlert")
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_DISMISS, "event_common_thirtytwo.EventDismassRoom")
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.CS_ASK_KICK_OUT, "event_common_thirtytwo.EventAskKick")
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_ASK_KICK_OUT_REPLY, "event_common_thirtytwo.EventKickNotice")
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_SYSTEM_DEAL_CARD_PUSH, "event_common_thirtytwo.PBAssignPai" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_NOTIFY_HINT_BET, "event_common_thirtytwo.PBHintBet" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.CS_ASK_BET, "event_common_thirtytwo.PBAskBet" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_USE_BET_PUSH, "event_common_thirtytwo.PBUseBet" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.CS_ASK_OPEN, "event_common_thirtytwo.PBAskOpen" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_USE_OPEN_PUSH, "event_common_thirtytwo.PBUseOpen" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_SMALL_RESULT_PUSH, "event_common_thirtytwo.PBSmallSettlement" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_BIG_RESULT_PUSH, "event_common_thirtytwo.PBTotalSettlement" )
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_RECORD_GAME_INFO_PUSH, "event_common_thirtytwo.PBReplayGame")
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_NOTIFY_HINT_SHUFFLE, "event_common_thirtytwo.PBHintShuffle")
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.CS_ASK_SHUFFLE, "event_common_thirtytwo.PBAskShuffle")
	_RegisterNetMsg( GAME_CMD_THIRTYTWO.SC_NOTIFY_USE_SHUFFLE, "event_common_thirtytwo.PBUseShuffle")
end

------------------------------------------斗地主---------------------------------------------------------------------
-- 初始化标志
_s.isInitDDZFlagTable = {}
-- 初始化斗地主 proto 入口
function ProtoManager.InitDDZProto(playID)
	DwDebug.Log("InitDDZProto" .. playID)
	if _s.isInitDDZFlagTable[playID] == nil then
		if playID == Common_PlayID.DW_DouDiZhu then
			_s.isInitDDZFlagTable[playID] = true

			_s.InitDDZGamePB()

			-- 初始化协议头和错误码
			GAME_CMD_DOUDIZHU = lua_pb.get_enum("event_common_doudizhu.GAME_CMD")
			GAME_ERRNO_DDZ_ERRNO = lua_pb.get_enum("event_common_doudizhu.GAME_ERRNO")

			_s.InitDDZGamePBMap()
		end
	end

	if playID == Common_PlayID.DW_DouDiZhu then
		GAME_CMD = GAME_CMD_DOUDIZHU
		GAME_ERRNO = GAME_ERRNO_DDZ_ERRNO
		ProtoManager._RegisterNetMsg(WebEvent.RecordGameSvrInfo,"event_common_doudizhu.PBReplay")
	end
end

--初始化斗地主游戏服PB
function ProtoManager.InitDDZGamePB()
	local loadFunc = WrapSys.LuaRootSys_LoadPB
	--游戏服pb
	_s._RegisterPB(loadFunc,"common_doudizhu.","CMD")
	_s._RegisterPB(loadFunc,"common_doudizhu.","error_code")
	_s._RegisterPB(loadFunc,"common_doudizhu.","ask_back")
	_s._RegisterPB(loadFunc,"common_doudizhu.","ask_open")
	_s._RegisterPB(loadFunc,"common_doudizhu.","ask_out")
	_s._RegisterPB(loadFunc,"common_doudizhu.","ask_plus")
	_s._RegisterPB(loadFunc,"common_doudizhu.","ask_rob")
	_s._RegisterPB(loadFunc,"common_doudizhu.","ask_skip")
	_s._RegisterPB(loadFunc,"common_doudizhu.","assign_pai")
	_s._RegisterPB(loadFunc,"common_doudizhu.","clear_desktop")
	_s._RegisterPB(loadFunc,"common_doudizhu.","create_room")
	_s._RegisterPB(loadFunc,"common_doudizhu.","dismass_room")
	_s._RegisterPB(loadFunc,"common_doudizhu.","distance_alert")
	_s._RegisterPB(loadFunc,"common_doudizhu.","exit")
	_s._RegisterPB(loadFunc,"common_doudizhu.","game_info")
	_s._RegisterPB(loadFunc,"common_doudizhu.","geo")
	_s._RegisterPB(loadFunc,"common_doudizhu.","heartbeat")
	_s._RegisterPB(loadFunc,"common_doudizhu.","hint_back")
	_s._RegisterPB(loadFunc,"common_doudizhu.","hint_open")
	_s._RegisterPB(loadFunc,"common_doudizhu.","hint_out")
	_s._RegisterPB(loadFunc,"common_doudizhu.","hint_plus")
	_s._RegisterPB(loadFunc,"common_doudizhu.","hint_rob")
	_s._RegisterPB(loadFunc,"common_doudizhu.","history_score")
	_s._RegisterPB(loadFunc,"common_doudizhu.","join_room")
	_s._RegisterPB(loadFunc,"common_doudizhu.","kick_out")
	_s._RegisterPB(loadFunc,"common_doudizhu.","lbs")
	_s._RegisterPB(loadFunc,"common_doudizhu.","login")
	_s._RegisterPB(loadFunc,"common_doudizhu.","online_status")
	_s._RegisterPB(loadFunc,"common_doudizhu.","re_conn")
	_s._RegisterPB(loadFunc,"common_doudizhu.","ready")
	_s._RegisterPB(loadFunc,"common_doudizhu.","render_ui")
	_s._RegisterPB(loadFunc,"common_doudizhu.","replay")
	_s._RegisterPB(loadFunc,"common_doudizhu.","replay_game")
	_s._RegisterPB(loadFunc,"common_doudizhu.","return")
	_s._RegisterPB(loadFunc,"common_doudizhu.","room_msg")
	_s._RegisterPB(loadFunc,"common_doudizhu.","seat_info")
	_s._RegisterPB(loadFunc,"common_doudizhu.","small_settlement")
	_s._RegisterPB(loadFunc,"common_doudizhu.","tanpai")
	_s._RegisterPB(loadFunc,"common_doudizhu.","total_settlement")
	_s._RegisterPB(loadFunc,"common_doudizhu.","use_back")
	_s._RegisterPB(loadFunc,"common_doudizhu.","use_open")
	_s._RegisterPB(loadFunc,"common_doudizhu.","use_out")
	_s._RegisterPB(loadFunc,"common_doudizhu.","use_plus")
	_s._RegisterPB(loadFunc,"common_doudizhu.","use_rob")
	_s._RegisterPB(loadFunc,"common_doudizhu.","wild_cards")
	_s._RegisterPB(loadFunc,"common_doudizhu.","multiple")
end

--初始化斗地主游戏服消息-PB 映射
function ProtoManager.InitDDZGamePBMap()
	local _RegisterNetMsg = ProtoManager._RegisterNetMsg

	_RegisterNetMsg( GAME_CMD_DOUDIZHU.CS_LOGIN, "event_common_doudizhu.EventLogin" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_COMMON_RSP, "event_common_doudizhu.EventReturn")
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.CS_CREATE_ROOM, "event_common_doudizhu.PBCreateRoom")
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.CS_JOIN_ROOM, "event_common_doudizhu.EventJoinRoom" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.CS_HEART_BEAT, "event_common_doudizhu.EventHeartBeat" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_SEAT_INFO_PUSH, "event_common_doudizhu.EventSeatInfo" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.CS_RENDER_UI_SUCCESS, "event_common_doudizhu.EventRenderUI" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.CS_PREPARE, "event_common_doudizhu.EventReady" )
	--_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_PREPARE_PUSH, "event_common_doudizhu.EventReadyReturn" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_PLAYER_ONLINE_PUSH, "event_common_doudizhu.EventOnlineStatus" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.CS_NET_RECONNECT, "event_common_doudizhu.EventReconn" )
	--_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_START_GAMER_PUSH, "event_common_doudizhu.EventStartGame" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_ROOM_INFO, "event_common_doudizhu.PBRoomMsg" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_GAME_INFO_PUSH, "event_common_doudizhu.PBGameInfo" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.CS_ASK_DISMISS, "event_common_doudizhu.EventAskDismassRoom" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_NOTIFY_VOTE_PUSH, "event_common_doudizhu.EventQueryDismassRoom" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.CS_PLAYER_VOTE, "event_common_doudizhu.EventReportDismassRoom" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_NOTIFY_DISMISS_RESULT_PUSH, "event_common_doudizhu.EventNoticeDismassRoom" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.CS_ASK_LBS, "event_common_doudizhu.EventGeo" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_ASK_LBS_REPLY, "event_common_doudizhu.PBGeo" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.CS_ASK_QUIT_ROOM, "event_common_doudizhu.EventExit" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_NOTIFY_QUIT_ROOM_PUSH, "event_common_doudizhu.EventExitStatus" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_NOTIFY_KICK_PUSH, "event_common_doudizhu.EventKickOut" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.CS_HISTORY_SCORE, "event_common_doudizhu.EventAskHistoryScore" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_HISTORY_SCORE_PUSH, "event_common_doudizhu.EventUseHistoryScore")
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_DISTANCE_TIPS_PUSH, "event_common_doudizhu.EventDistanceAlert")
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_DISMISS, "event_common_doudizhu.EventDismassRoom")
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.CS_ASK_KICK_OUT, "event_common_doudizhu.EventAskKick")
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_ASK_KICK_OUT_REPLY, "event_common_doudizhu.EventKickNotice")
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_SYSTEM_DEAL_CARD_PUSH, "event_common_doudizhu.PBAssignPai" )

	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_NOTIFY_IS_WANT_LORD_PUSH, "event_common_doudizhu.PBHintRob" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.CS_REPLY_WANT_LORD, "event_common_doudizhu.PBAskRob" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_LORD_PUSH, "event_common_doudizhu.PBUseRob" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_BOTTOM_CARDS_PUSH, "event_common_doudizhu.PBWildCards" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_NOTIFY_IS_OPEN_PLAY_PUSH, "event_common_doudizhu.PBHintOpen" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.CS_REPLY_ALONE_LORD_IS_OPEN_PLAY, "event_common_doudizhu.PBAskOpen" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_LORD_OPEN_PLAY_PUSH, "event_common_doudizhu.PBUseOpen" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_NOTIFY_FARMER_ADD_TIMES_PUSH, "event_common_doudizhu.PBHintPlus" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.CS_REPLY_FARMER_ADD_TIMES, "event_common_doudizhu.PBAskPlus" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_FARMER_ADD_TIMES_PUSH, "event_common_doudizhu.PBUsePlus" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_NOTIFY_LORD_SUB_TIMES_PUSH, "event_common_doudizhu.PBHintBack" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.CS_REPLY_LORD_SUB_TIMES, "event_common_doudizhu.PBAskBack" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_LORD_SUB_TIMES, "event_common_doudizhu.PBUseBack" )

	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_NOTIFY_PLAY_CARD_PUSH, "event_common_doudizhu.PBHintOut" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.CS_PLAYER_PLAY_CARD, "event_common_doudizhu.PBAskOut" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_SVR_PLAY_CARD_PUSH, "event_common_doudizhu.PBUseOut" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_CLEAR_CARDS_PUSH, "event_common_doudizhu.PBClearDesktop" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_SHOW_ALL_CARDS_PUSH, "event_common_doudizhu.PBTanPai" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_SMALL_RESULT_PUSH, "event_common_doudizhu.PBSmallSettlement" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_BIG_RESULT_PUSH, "event_common_doudizhu.PBTotalSettlement" )
	_RegisterNetMsg( GAME_CMD_DOUDIZHU.SC_RECORD_GAME_INFO_PUSH, "event_common_doudizhu.PBReplayGame")
end
-------------------------------------------麻将-------------------------------------------------------------------

_s.isInitMJFlagTable = {}
function ProtoManager.InitMJProto(playID)
	DwDebug.Log("InitMJProto playID "..playID )
	if _s.isInitMJFlagTable[playID] == nil then
		_s.isInitMJFlagTable[playID] = true
		if playID == Common_PlayID.chongRen_MJ then
			_s.InitChongRenMJGamePB()
			GAME_CMD_CRMJ = lua_pb.get_enum("event_chongren_mj.GAME_CMD")
			GAME_ERRNO_CRMJ = lua_pb.get_enum("event_chongren_mj.GAME_ERRNO")

			_s.InitChongRenMJGamePBMap()
		elseif playID == Common_PlayID.leAn_MJ then
			DwDebug.Log("InitMJProto leAn_MJ")
			_s.InitLeAnMJGamePB()
			GAME_CMD_LEAN_MJ = lua_pb.get_enum("event_lean_mj.GAME_CMD")
			GAME_ERRNO_LEAN_MJ = lua_pb.get_enum("event_lean_mj.GAME_ERRNO")

			_s.InitLeAnMJGamePBMap()
		elseif playID == Common_PlayID.yiHuang_MJ then
			DwDebug.Log("InitMJProto yiHuang_MJ ")
			_s.InitYiHuangMJGamePB()
			GAME_CMD_YIHUANG_MJ = lua_pb.get_enum("event_yihuang_mj.GAME_CMD")
			GAME_ERRNO_YIHUANG_MJ = lua_pb.get_enum("event_yihuang_mj.GAME_ERRNO")

			_s.InitYiHuangMJGamePBMap()
		end
	end
	if playID == Common_PlayID.chongRen_MJ then
		GAME_CMD = GAME_CMD_CRMJ
		GAME_ERRNO = GAME_ERRNO_CRMJ
		ProtoManager._RegisterNetMsg( WebEvent.RecordGameSvrInfo, "event_chongren_mj.EventReplay")
	elseif playID == Common_PlayID.leAn_MJ then
		GAME_CMD = GAME_CMD_LEAN_MJ
		GAME_ERRNO = GAME_ERRNO_LEAN_MJ
		ProtoManager._RegisterNetMsg( WebEvent.RecordGameSvrInfo, "event_lean_mj.EventReplay")
	elseif playID == Common_PlayID.yiHuang_MJ then
		GAME_CMD = GAME_CMD_YIHUANG_MJ
		GAME_ERRNO = GAME_ERRNO_YIHUANG_MJ
		ProtoManager._RegisterNetMsg( WebEvent.RecordGameSvrInfo, "event_yihuang_mj.EventReplay")
	end
end


--崇仁麻将
function ProtoManager.InitChongRenMJGamePB()
	local loadFunc = WrapSys.LuaRootSys_LoadPB
	--游戏服pb
	_s._RegisterPB(loadFunc,"chongren_mj.","CMD")
	_s._RegisterPB(loadFunc,"chongren_mj.","error_code")
	_s._RegisterPB(loadFunc,"chongren_mj.","create")
	_s._RegisterPB(loadFunc,"chongren_mj.","connect")
	_s._RegisterPB(loadFunc,"chongren_mj.","disband_room")
	_s._RegisterPB(loadFunc,"chongren_mj.","distance_alert")
	_s._RegisterPB(loadFunc,"chongren_mj.","exit")
	_s._RegisterPB(loadFunc,"chongren_mj.","flow")
	_s._RegisterPB(loadFunc,"chongren_mj.","game_card_info")
	_s._RegisterPB(loadFunc,"chongren_mj.","heartbeat")
	_s._RegisterPB(loadFunc,"chongren_mj.","jifen")
	_s._RegisterPB(loadFunc,"chongren_mj.","join_room")
	_s._RegisterPB(loadFunc,"chongren_mj.","login")
	_s._RegisterPB(loadFunc,"chongren_mj.","out_card")
	_s._RegisterPB(loadFunc,"chongren_mj.","outcard_pos_notice")
	_s._RegisterPB(loadFunc,"chongren_mj.","pass_notice")
	_s._RegisterPB(loadFunc,"chongren_mj.","player_distance")
	_s._RegisterPB(loadFunc,"chongren_mj.","ready")
	_s._RegisterPB(loadFunc,"chongren_mj.","record")
	_s._RegisterPB(loadFunc,"chongren_mj.","render_ui")
	_s._RegisterPB(loadFunc,"chongren_mj.","replay")
	_s._RegisterPB(loadFunc,"chongren_mj.","return")
	_s._RegisterPB(loadFunc,"chongren_mj.","robot")
	_s._RegisterPB(loadFunc,"chongren_mj.","room_info")
	_s._RegisterPB(loadFunc,"chongren_mj.","shaozhuang")
	_s._RegisterPB(loadFunc,"chongren_mj.","small_settlement")
	_s._RegisterPB(loadFunc,"chongren_mj.","t")
	_s._RegisterPB(loadFunc,"chongren_mj.","tanpai")
	_s._RegisterPB(loadFunc,"chongren_mj.","test")
	_s._RegisterPB(loadFunc,"chongren_mj.","total_settlement")
end

function ProtoManager.InitChongRenMJGamePBMap()
	local _RegisterNetMsg = ProtoManager._RegisterNetMsg

	_RegisterNetMsg( GAME_CMD_CRMJ.CS_LOGIN						, "event_chongren_mj.EventLogin" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_COMMON_RSP				, "event_chongren_mj.EventReturn")
	_RegisterNetMsg( GAME_CMD_CRMJ.CS_CREATE_ROOM				, "event_chongren_mj.EventChongrenPlay")
	_RegisterNetMsg( GAME_CMD_CRMJ.CS_JOIN_ROOM					, "event_chongren_mj.EventJoinRoom" )
	_RegisterNetMsg( GAME_CMD_CRMJ.CS_HEART_BEAT				, "event_chongren_mj.EventHeartBeat" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_SEAT_INFO_PUSH			, "event_chongren_mj.EventSeatInfo" )
	_RegisterNetMsg( GAME_CMD_CRMJ.CS_RENDER_UI_SUCCESS			, "event_chongren_mj.EventRenderUI" )
	_RegisterNetMsg( GAME_CMD_CRMJ.CS_PREPARE					, "event_chongren_mj.EventReady" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_PREPARE_PUSH				, "event_chongren_mj.EventReadyReturn" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_PLAYER_ONLINE_PUSH		, "event_chongren_mj.EventOnlineStatus" )
	_RegisterNetMsg( GAME_CMD_CRMJ.CS_NET_RECONNECT				, "event_chongren_mj.EventReconn" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_START_GAMER_PUSH			, "event_chongren_mj.EventStartGame" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_ROOM_INFO					, "event_chongren_mj.EventRoomInfo" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_GAME_INFO_PUSH			, "event_chongren_mj.EventCardInfo" )
	_RegisterNetMsg( GAME_CMD_CRMJ.CS_GAMEPLAY_PASS				, "event_chongren_mj.EventPassNotice" )
	_RegisterNetMsg( GAME_CMD_CRMJ.CS_SC_GAMEPLAY_PLAY_CARD		, "event_chongren_mj.EventOutCard" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_GAMEPLAY_NOTIFY_PLAY_PUSH	, "event_chongren_mj.EventCardActionNotice" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_GAMEPLAY_CARD_PUSH		, "event_chongren_mj.EventPaiPai" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_GAMEPLAY_WHOS_TURN_PUSH	, "event_chongren_mj.EventOutCardPosNotice" )
	_RegisterNetMsg( GAME_CMD_CRMJ.CS_SC_GAMEPLAY_HU			, "event_chongren_mj.EventHuAction" )
	_RegisterNetMsg( GAME_CMD_CRMJ.CS_SC_GAMEPLAY_GANG			, "event_chongren_mj.EventGangAction" )
	_RegisterNetMsg( GAME_CMD_CRMJ.CS_SC_GAMEPLAY_PENG			, "event_chongren_mj.EventPengAction" )
	_RegisterNetMsg( GAME_CMD_CRMJ.CS_SC_GAMEPLAY_CHI			, "event_chongren_mj.EventChiAction" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_SCORE_PUSH				, "event_chongren_mj.EventJifen" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_SMALL_RESULT_PUSH			, "event_chongren_mj.EventSmallSettlement" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_BIG_RESULT_PUSH			, "event_chongren_mj.EventTotalSettlement" )
	_RegisterNetMsg( GAME_CMD_CRMJ.CS_ASK_QUIT_ROOM				, "event_chongren_mj.EventExit" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_DISMISS					, "event_chongren_mj.EventDisband" )
	_RegisterNetMsg( GAME_CMD_CRMJ.CS_ASK_DISMISS				, "event_chongren_mj.EventApplyDisband" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_NOTIFY_VOTE_PUSH			, "event_chongren_mj.EventDisbandRoomNotice" )
	_RegisterNetMsg( GAME_CMD_CRMJ.CS_PLAYER_VOTE				, "event_chongren_mj.EventDisbandAction" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_NOTIFY_DISMISS_RESULT_PUSH, "event_chongren_mj.EventDisbandEnd" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_SHOW_ALL_CARDS_PUSH		, "event_chongren_mj.EventTanPai" )
	_RegisterNetMsg( GAME_CMD_CRMJ.CS_HISTORY_SCORE				, "event_chongren_mj.EventRecordRequest" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_HISTORY_SCORE_PUSH		, "event_chongren_mj.EventRecordResponse" )
	_RegisterNetMsg( GAME_CMD_CRMJ.CS_DISTANCE_REQ				, "event_chongren_mj.EventPlayerDistanceRequest" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_DISTANCE_RSP				, "event_chongren_mj.EventPlayerDistanceResponse" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_NOTIFY_KICK_PUSH			, "event_chongren_mj.EventT" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_GAMEPLAY_LIUJU			, "event_chongren_mj.EventFlow" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_DISTANCE_TIPS_PUSH		, "event_chongren_mj.EventDistanceAlert" )
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_GAMEPLAY_SHAOZHUANG		, "event_chongren_mj.EventShaoZhuang" )
	_RegisterNetMsg( GAME_CMD_CRMJ.CS_ASK_KICK_OUT              , "event_chongren_mj.EventAskKick")
	_RegisterNetMsg( GAME_CMD_CRMJ.SC_ASK_KICK_OUT_REPLY        , "event_chongren_mj.EventKickNotice")

end


--乐安麻将
function ProtoManager.InitLeAnMJGamePB()
	local loadFunc = WrapSys.LuaRootSys_LoadPB
	--游戏服pb
	_s._RegisterPB(loadFunc,"lean_mj.","CMD")
	_s._RegisterPB(loadFunc,"lean_mj.","error_code")
	_s._RegisterPB(loadFunc,"lean_mj.","create")
	_s._RegisterPB(loadFunc,"lean_mj.","disband_room")
	_s._RegisterPB(loadFunc,"lean_mj.","distance_alert")
	_s._RegisterPB(loadFunc,"lean_mj.","exit")
	_s._RegisterPB(loadFunc,"lean_mj.","flow")
	_s._RegisterPB(loadFunc,"lean_mj.","game_card_info")
	_s._RegisterPB(loadFunc,"lean_mj.","heartbeat")
	_s._RegisterPB(loadFunc,"lean_mj.","jiangma")
	_s._RegisterPB(loadFunc,"lean_mj.","jifen")
	_s._RegisterPB(loadFunc,"lean_mj.","join")
	_s._RegisterPB(loadFunc,"lean_mj.","login")
	_s._RegisterPB(loadFunc,"lean_mj.","out_card")
	_s._RegisterPB(loadFunc,"lean_mj.","outcard_pos_notice")
	_s._RegisterPB(loadFunc,"lean_mj.","pass_notice")
	_s._RegisterPB(loadFunc,"lean_mj.","player_distance")
	_s._RegisterPB(loadFunc,"lean_mj.","ready")
	_s._RegisterPB(loadFunc,"lean_mj.","reconnect")
	_s._RegisterPB(loadFunc,"lean_mj.","record")
	_s._RegisterPB(loadFunc,"lean_mj.","render_ui")
	_s._RegisterPB(loadFunc,"lean_mj.","replay")
	_s._RegisterPB(loadFunc,"lean_mj.","return")
	_s._RegisterPB(loadFunc,"lean_mj.","robot")
	_s._RegisterPB(loadFunc,"lean_mj.","room_info")
	_s._RegisterPB(loadFunc,"lean_mj.","shaozhuang")
	_s._RegisterPB(loadFunc,"lean_mj.","small_settlement")
	_s._RegisterPB(loadFunc,"lean_mj.","t")
	_s._RegisterPB(loadFunc,"lean_mj.","tanpai")
	_s._RegisterPB(loadFunc,"lean_mj.","total_settlement")
	_s._RegisterPB(loadFunc,"lean_mj.","user")
end

function ProtoManager.InitLeAnMJGamePBMap()
	local _RegisterNetMsg = ProtoManager._RegisterNetMsg

	_RegisterNetMsg( GAME_CMD_LEAN_MJ.CS_LOGIN						, "event_lean_mj.EventLogin" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_COMMON_RSP					, "event_lean_mj.EventReturn")
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.CS_CREATE_ROOM				, "event_lean_mj.EventCreatePlay")
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.CS_JOIN_ROOM					, "event_lean_mj.EventJoinRoom" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.CS_HEART_BEAT					, "event_lean_mj.EventHeartBeat" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_SEAT_INFO_PUSH				, "event_lean_mj.EventSeatInfo" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.CS_RENDER_UI_SUCCESS			, "event_lean_mj.EventRenderUI" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.CS_PREPARE					, "event_lean_mj.EventReady" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_PREPARE_PUSH				, "event_lean_mj.EventReadyReturn" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_PLAYER_ONLINE_PUSH			, "event_lean_mj.EventOnlineStatus" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.CS_NET_RECONNECT				, "event_lean_mj.EventReconn" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_START_GAMER_PUSH			, "event_lean_mj.EventStartGame" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_ROOM_INFO					, "event_lean_mj.EventRoomInfo" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_GAME_INFO_PUSH				, "event_lean_mj.EventCardInfo" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.CS_GAMEPLAY_PASS				, "event_lean_mj.EventPassNotice" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.CS_SC_GAMEPLAY_PLAY_CARD		, "event_lean_mj.EventOutCard" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_GAMEPLAY_NOTIFY_PLAY_PUSH	, "event_lean_mj.EventCardActionNotice" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_GAMEPLAY_CARD_PUSH			, "event_lean_mj.EventPaiPai" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_GAMEPLAY_WHOS_TURN_PUSH	, "event_lean_mj.EventOutCardPosNotice" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.CS_SC_GAMEPLAY_HU				, "event_lean_mj.EventHuAction" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.CS_SC_GAMEPLAY_GANG			, "event_lean_mj.EventGangAction" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.CS_SC_GAMEPLAY_PENG			, "event_lean_mj.EventPengAction" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.CS_SC_GAMEPLAY_CHI			, "event_lean_mj.EventChiAction" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_SCORE_PUSH					, "event_lean_mj.EventJifen" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_SMALL_RESULT_PUSH			, "event_lean_mj.EventSmallSettlement" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_BIG_RESULT_PUSH			, "event_lean_mj.EventTotalSettlement" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.CS_ASK_QUIT_ROOM				, "event_lean_mj.EventExit" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_DISMISS					, "event_lean_mj.EventDisband" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.CS_ASK_DISMISS				, "event_lean_mj.EventApplyDisband" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_NOTIFY_VOTE_PUSH			, "event_lean_mj.EventDisbandRoomNotice" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.CS_PLAYER_VOTE				, "event_lean_mj.EventDisbandAction" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_NOTIFY_DISMISS_RESULT_PUSH	, "event_lean_mj.EventDisbandEnd" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_SHOW_ALL_CARDS_PUSH		, "event_lean_mj.EventTanPai" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.CS_HISTORY_SCORE				, "event_lean_mj.EventRecordRequest" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_HISTORY_SCORE_PUSH			, "event_lean_mj.EventRecordResponse" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.CS_DISTANCE_REQ				, "event_lean_mj.EventPlayerDistanceRequest" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_DISTANCE_RSP				, "event_lean_mj.EventPlayerDistanceResponse" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_NOTIFY_KICK_PUSH			, "event_lean_mj.EventT" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_GAMEPLAY_LIUJU				, "event_lean_mj.EventFlow" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_DISTANCE_TIPS_PUSH			, "event_lean_mj.EventDistanceAlert" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_GAMEPLAY_SHAOZHUANG		, "event_lean_mj.EventShaoZhuang" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_GAMEPLAY_JIANGMA			, "event_lean_mj.EventJiangMa" )
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.CS_ASK_KICK_OUT               , "event_lean_mj.EventAskKick")
	_RegisterNetMsg( GAME_CMD_LEAN_MJ.SC_ASK_KICK_OUT_REPLY         , "event_lean_mj.EventKickNotice")
end

--宜黄麻将
function ProtoManager.InitYiHuangMJGamePB()
	local loadFunc = WrapSys.LuaRootSys_LoadPB
	--游戏服pb
	_s._RegisterPB(loadFunc,"yihuang_mj.","CMD")
	_s._RegisterPB(loadFunc,"yihuang_mj.","error_code")
	_s._RegisterPB(loadFunc,"yihuang_mj.","create")
	_s._RegisterPB(loadFunc,"yihuang_mj.","disband_room")
	_s._RegisterPB(loadFunc,"yihuang_mj.","distance_alert")
	_s._RegisterPB(loadFunc,"yihuang_mj.","exit")
	_s._RegisterPB(loadFunc,"yihuang_mj.","flow")
	_s._RegisterPB(loadFunc,"yihuang_mj.","game_card_info")
	_s._RegisterPB(loadFunc,"yihuang_mj.","heartbeat")
	_s._RegisterPB(loadFunc,"yihuang_mj.","jiangma")
	_s._RegisterPB(loadFunc,"yihuang_mj.","jifen")
	_s._RegisterPB(loadFunc,"yihuang_mj.","join")
	_s._RegisterPB(loadFunc,"yihuang_mj.","login")
	_s._RegisterPB(loadFunc,"yihuang_mj.","out_card")
	_s._RegisterPB(loadFunc,"yihuang_mj.","outcard_pos_notice")
	_s._RegisterPB(loadFunc,"yihuang_mj.","pass_notice")
	_s._RegisterPB(loadFunc,"yihuang_mj.","player_distance")
	_s._RegisterPB(loadFunc,"yihuang_mj.","ready")
	_s._RegisterPB(loadFunc,"yihuang_mj.","reconnect")
	_s._RegisterPB(loadFunc,"yihuang_mj.","record")
	_s._RegisterPB(loadFunc,"yihuang_mj.","render_ui")
	_s._RegisterPB(loadFunc,"yihuang_mj.","replay")
	_s._RegisterPB(loadFunc,"yihuang_mj.","return")
	_s._RegisterPB(loadFunc,"yihuang_mj.","robot")
	_s._RegisterPB(loadFunc,"yihuang_mj.","room_info")
	_s._RegisterPB(loadFunc,"yihuang_mj.","shaozhuang")
	_s._RegisterPB(loadFunc,"yihuang_mj.","small_settlement")
	_s._RegisterPB(loadFunc,"yihuang_mj.","t")
	_s._RegisterPB(loadFunc,"yihuang_mj.","tanpai")
	_s._RegisterPB(loadFunc,"yihuang_mj.","total_settlement")
	_s._RegisterPB(loadFunc,"yihuang_mj.","user")
end

function ProtoManager.InitYiHuangMJGamePBMap()
	local _RegisterNetMsg = ProtoManager._RegisterNetMsg

	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.CS_LOGIN						, "event_yihuang_mj.EventLogin" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_COMMON_RSP					, "event_yihuang_mj.EventReturn")
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.CS_CREATE_ROOM					, "event_yihuang_mj.EventCreatePlay")
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.CS_JOIN_ROOM					, "event_yihuang_mj.EventJoinRoom" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.CS_HEART_BEAT					, "event_yihuang_mj.EventHeartBeat" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_SEAT_INFO_PUSH				, "event_yihuang_mj.EventSeatInfo" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.CS_RENDER_UI_SUCCESS			, "event_yihuang_mj.EventRenderUI" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.CS_PREPARE						, "event_yihuang_mj.EventReady" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_PREPARE_PUSH				, "event_yihuang_mj.EventReadyReturn" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_PLAYER_ONLINE_PUSH			, "event_yihuang_mj.EventOnlineStatus" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.CS_NET_RECONNECT				, "event_yihuang_mj.EventReconn" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_START_GAMER_PUSH			, "event_yihuang_mj.EventStartGame" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_ROOM_INFO					, "event_yihuang_mj.EventRoomInfo" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_GAME_INFO_PUSH				, "event_yihuang_mj.EventCardInfo" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.CS_GAMEPLAY_PASS				, "event_yihuang_mj.EventPassNotice" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.CS_SC_GAMEPLAY_PLAY_CARD		, "event_yihuang_mj.EventOutCard" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_GAMEPLAY_NOTIFY_PLAY_PUSH	, "event_yihuang_mj.EventCardActionNotice" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_GAMEPLAY_CARD_PUSH			, "event_yihuang_mj.EventPaiPai" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_GAMEPLAY_WHOS_TURN_PUSH		, "event_yihuang_mj.EventOutCardPosNotice" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.CS_SC_GAMEPLAY_HU				, "event_yihuang_mj.EventHuAction" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.CS_SC_GAMEPLAY_GANG			, "event_yihuang_mj.EventGangAction" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.CS_SC_GAMEPLAY_PENG			, "event_yihuang_mj.EventPengAction" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.CS_SC_GAMEPLAY_CHI				, "event_yihuang_mj.EventChiAction" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_SCORE_PUSH					, "event_yihuang_mj.EventJifen" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_SMALL_RESULT_PUSH			, "event_yihuang_mj.EventSmallSettlement" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_BIG_RESULT_PUSH				, "event_yihuang_mj.EventTotalSettlement" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.CS_ASK_QUIT_ROOM				, "event_yihuang_mj.EventExit" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_DISMISS						, "event_yihuang_mj.EventDisband" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.CS_ASK_DISMISS					, "event_yihuang_mj.EventApplyDisband" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_NOTIFY_VOTE_PUSH			, "event_yihuang_mj.EventDisbandRoomNotice" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.CS_PLAYER_VOTE					, "event_yihuang_mj.EventDisbandAction" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_NOTIFY_DISMISS_RESULT_PUSH	, "event_yihuang_mj.EventDisbandEnd" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_SHOW_ALL_CARDS_PUSH			, "event_yihuang_mj.EventTanPai" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.CS_HISTORY_SCORE				, "event_yihuang_mj.EventRecordRequest" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_HISTORY_SCORE_PUSH			, "event_yihuang_mj.EventRecordResponse" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.CS_DISTANCE_REQ				, "event_yihuang_mj.EventPlayerDistanceRequest" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_DISTANCE_RSP				, "event_yihuang_mj.EventPlayerDistanceResponse" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_NOTIFY_KICK_PUSH			, "event_yihuang_mj.EventT" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_GAMEPLAY_LIUJU				, "event_yihuang_mj.EventFlow" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_DISTANCE_TIPS_PUSH			, "event_yihuang_mj.EventDistanceAlert" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_GAMEPLAY_SHAOZHUANG			, "event_yihuang_mj.EventShaoZhuang" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_GAMEPLAY_JIANGMA			, "event_yihuang_mj.EventJiangMa" )
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.CS_ASK_KICK_OUT                , "event_yihuang_mj.EventAskKick")
	_RegisterNetMsg( GAME_CMD_YIHUANG_MJ.SC_ASK_KICK_OUT_REPLY          , "event_yihuang_mj.EventKickNotice")
end

-------------------------------------------功能区------------------------------------------------

function ProtoManager.Init()
	--重置pb环境
	lua_pb.clear()

	-- 初始化Web消息 PB
	_s.InitWebProto()
	_s.InitUserSvrProto()
	_s.InitWSKProto(10001)
end

-- 获取成功状态码
function ProtoManager.GetSuccesErrorCodeNum(playID)
	if playID == Common_PlayID.chongRen_510K then
		return 10001
	elseif playID == Common_PlayID.chongRen_MJ then
		return 0
	elseif playID == Common_PlayID.leAn_510K then
		return 10301
	elseif playID == Common_PlayID.leAn_MJ then
		return 0
	elseif playID == Common_PlayID.yiHuang_510K then
		return 10401
	elseif playID == Common_PlayID.yiHuang_MJ then
		return 0
	elseif playID == Common_PlayID.ThirtyTwo then
		return 10601
	elseif playID == Common_PlayID.DW_DouDiZhu then
		return 10501
	elseif playID == Common_PlayID.UserSvrNet then
		return 25001
	end
	DwDebug.LogError(playID .. " not find in GetSuccesErrorCodeNum")
	return -1

end

--获取心跳包CMD
function ProtoManager.GetHeatBeatCMD(playID)
	if playID == Common_PlayID.chongRen_510K then
		return 10005
	elseif playID == Common_PlayID.chongRen_MJ then
		return 5
	elseif playID == Common_PlayID.leAn_510K then
		return 10305
	elseif playID == Common_PlayID.leAn_MJ then
		return 205
	elseif playID == Common_PlayID.yiHuang_510K then
		return 10405
	elseif playID == Common_PlayID.yiHuang_MJ then
		return 305
	elseif playID == Common_PlayID.ThirtyTwo then
		return 10605
	elseif playID == Common_PlayID.DW_DouDiZhu then
		return 10505
	elseif playID == Common_PlayID.UserSvrNet then
		return 25252
	end
	DwDebug.LogError(playID .. " not find in GetHeatBeatCMD")
	return -1

end
--获取服务端通用回包CMD
function ProtoManager.GetRspCMD(playID)
	if playID == Common_PlayID.chongRen_510K then
		return 10002
	elseif playID == Common_PlayID.chongRen_MJ then
		return 2
	elseif playID == Common_PlayID.leAn_510K then
		return 10302
	elseif playID == Common_PlayID.leAn_MJ then
		return 202
	elseif playID == Common_PlayID.yiHuang_510K then
		return 10402
	elseif playID == Common_PlayID.yiHuang_MJ then
		return 302
	elseif playID == Common_PlayID.ThirtyTwo then
		return 10602
	elseif playID == Common_PlayID.DW_DouDiZhu then
		return 10502
	elseif playID == Common_PlayID.UserSvrNet then
		return 25251
	end
	DwDebug.LogError(playID .. " not find in GetRspCMD")
	return -1

end

--外部调用初始化协议接口
function ProtoManager.InitProtoByPlayID(playID)
	DwDebug.Log("InitProtoByPlayID playID "..playID)
	if playID >= Common_PlayID.chongRen_510K and playID <= Common_PlayID.yiHuang_510K then
		_s.InitWSKProto(playID)
	elseif playID >= Common_PlayID.chongRen_MJ and playID <= Common_PlayID.yiHuang_MJ then
		_s.InitMJProto(playID)
	elseif playID == Common_PlayID.ThirtyTwo then
		_s.InitThirtyTwoProto(playID)
	elseif playID == Common_PlayID.DW_DouDiZhu then
		_s.InitDDZProto(playID)
	else
		DwDebug.LogError("InitProtoByPlayID playID is not exist")
	end
end


function ProtoManager._RegisterPB(loadFunc,childPath,pbName)
	loadFunc("Proto."..childPath..pbName)
end

function ProtoManager._RegisterNetMsg(cmd,packageName)
	if cmd == nil then
		DwDebug.LogError("ProtoManager._RegisterNetMsg  cmd is nil")
		return
	end
--	print("ProtoManager _RegisterNetMsg "..cmd .. " ".. packageName)
	_s.eventMap[cmd] = packageName
end

--反序列化
function ProtoManager.Decode(cmd, buffer)
	DwDebug.Log("========================================== cmd"..cmd)

	if buffer == nil then
		DwDebug.LogError("ProtoManager.Decode  buffer is nil " .. cmd)
		return nil
	end

	local packageName = _s.eventMap[cmd]
	if packageName == nil then
		DwDebug.LogError("ProtoManager.Decode failed!!!" .. cmd .. " never _RegisterNetMsg")
		return nil
	end

	local msg = lua_pb.decode(packageName,buffer)
	if msg == nil then
		DwDebug.LogError("lua_pb.decode fail!!!" ..  packageName)
	end
	return msg
end

--序列化
function ProtoManager.Encode(cmd, msg)
	if msg == nil then
		DwDebug.LogError("ProtoManager.Encode  buffer is nil " .. cmd)
		return nil
	end

	local packageName = _s.eventMap[cmd]
	if packageName == nil then
		DwDebug.LogError("ProtoManager.Encode failed!!! " .. cmd .. " never _RegisterNetMsg")
		return nil
	end

	local data = lua_pb.encode(packageName, msg)
	if not data then
		DwDebug.LogError("lua_pb.encode fail!!!" ..  packageName .. " never _RegisterPB" .. ToString(msg))
	end

	return data
end

