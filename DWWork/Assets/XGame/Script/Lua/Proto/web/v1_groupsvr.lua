
‹>
v1_groupsvr.protogroupsvr"°
CommunicateSvr
errcode (Rerrcode
errmsg (	Rerrmsg1
data (2.groupsvr.CommunicateSvr.DataRdata*
Data
ip (	Rip
port (Rport"«
	GroupList
errcode (Rerrcode
errmsg (	Rerrmsg,
data (2.groupsvr.GroupList.DataRdataŸ
Data1
list (2.groupsvr.GroupList.Data.ListRlistù
List
id (Rid
name (	Rname
g_img (RgImg
user_number (R
userNumber
card_number (R
cardNumber
status (Rstatus
	owner_uid (RownerUid!
owner_weixin (	RownerWeixin%
owner_nickname	 (	RownerNickname
	main_play
 (	RmainPlay"Œ
	GroupInfo
errcode (Rerrcode
errmsg (	Rerrmsg,
data (2.groupsvr.GroupInfo.DataRdata‡
Data
id (Rid
name (	Rname
g_img (RgImg
	owner_uid (RownerUid!
owner_weixin (	RownerWeixin!
owner_mobile (	RownerMobile
user_number (R
userNumber
	main_play (	RmainPlay"Å
GroupAdd
errcode (Rerrcode
errmsg (	Rerrmsg+
data (2.groupsvr.GroupAdd.DataRdata
Data
id (Rid"?
GroupUpdate
errcode (Rerrcode
errmsg (	Rerrmsg"—
GroupRecharge
errcode (Rerrcode
errmsg (	Rerrmsg0
data (2.groupsvr.GroupRecharge.DataRdata\
Data(
hall_card_number (RhallCardNumber*
group_card_number (RgroupCardNumber"á
GroupDelete
errcode (Rerrcode
errmsg (	Rerrmsg.
data (2.groupsvr.GroupDelete.DataRdata
Data
id (Rid"˛
GroupUserList
errcode (Rerrcode
errmsg (	Rerrmsg0
data (2.groupsvr.GroupUserList.DataRdataà
Data5
list (2!.groupsvr.GroupUserList.Data.ListRlist»
List
is_paged (RisPaged

page_index (R	pageIndex
	page_size (RpageSize

page_count (R	pageCount
total (Rtotal<
items (2&.groupsvr.GroupUserList.Data.List.ItemRitemsı
Item
uid (Ruid
wx_nickname (	R
wxNickname#
wx_headimgurl (	RwxHeadimgurl
wx_sex (RwxSex
game_number (R
gameNumber
	join_time (RjoinTime#
online_status (RonlineStatus
	owner_uid (RownerUid"∆
GroupUserInfo
errcode (Rerrcode
errmsg (	Rerrmsg0
data (2.groupsvr.GroupUserInfo.DataRdata–
Data
uid (Ruid
wx_nickname (	R
wxNickname#
wx_headimgurl (	RwxHeadimgurl
wx_sex (RwxSex
game_number (R
gameNumber
	join_time (RjoinTime
	owner_uid (RownerUid"Ó
GroupUserImg
errcode (Rerrcode
errmsg (	Rerrmsg/
data (2.groupsvr.GroupUserImg.DataRdata{
Data4
list (2 .groupsvr.GroupUserImg.Data.ListRlist=
List
uid (Ruid#
wx_headimgurl (	RwxHeadimgurl"F
GroupUserApplyJoin
errcode (Rerrcode
errmsg (	Rerrmsg"ô
GroupUserHandleApply
errcode (Rerrcode
errmsg (	Rerrmsg7
data (2#.groupsvr.GroupUserHandleApply.DataRdata
Data
id (	Rid"ì
GroupUserKickout
errcode (Rerrcode
errmsg (	Rerrmsg3
data (2.groupsvr.GroupUserKickout.DataRdata
Data
uid (Ruid"ñ
GroupUserQuit
errcode (Rerrcode
errmsg (	Rerrmsg0
data (2.groupsvr.GroupUserQuit.DataRdata!
Data
group_id (RgroupId"“
RoomTplList
errcode (Rerrcode
errmsg (	Rerrmsg.
data (2.groupsvr.RoomTplList.DataRdata‡
Data3
list (2.groupsvr.RoomTplList.Data.ListRlist
now_tpl_num (R	nowTplNum
max_tpl_num (R	maxTplNumc
List
id (Rid
play_id (RplayId
tpl (Rtpl 
description (	Rdescription"î
RoomCreateTpl
errcode (Rerrcode
errmsg (	Rerrmsg0
data (2.groupsvr.RoomCreateTpl.DataRdata
Data
room_id (	RroomId"A
RoomUpdateTpl
errcode (Rerrcode
errmsg (	Rerrmsg"ã
RoomDeleteTpl
errcode (Rerrcode
errmsg (	Rerrmsg0
data (2.groupsvr.RoomDeleteTpl.DataRdata
Data
id (	Rid"¯
RoomList
errcode (Rerrcode
errmsg (	Rerrmsg+
data (2.groupsvr.RoomList.DataRdataå
Data0
list (2.groupsvr.RoomList.Data.ListRlist—
List
is_paged (RisPaged

page_index (R	pageIndex
	page_size (RpageSize

page_count (R	pageCount
total (Rtotal7
items (2!.groupsvr.RoomList.Data.List.ItemRitemsÉ
Item
room_id (RroomId
play_id (RplayId
	play_name (	RplayName,
current_player_num (RcurrentPlayerNum$
max_player_num (RmaxPlayerNum$
total_game_num (RtotalGameNum
play_des (	RplayDes
status
 (Rstatus
group_id (RgroupId
tpl_id (RtplId
create_time (R
createTimeB
players (2(.groupsvr.RoomList.Data.List.Item.PlayerRplayersh
Player
uid (Ruid
nickname (	Rnickname

headimgurl (	R
headimgurl
sex (Rsex"Ë
RoomPlayDes
errcode (Rerrcode
errmsg (	Rerrmsg.
data (2.groupsvr.RoomPlayDes.DataRdataw
Data3
list (2.groupsvr.RoomPlayDes.Data.ListRlist:
List
room_id (RroomId
play_des (	RplayDes"Ï
ArchiveList
errcode (Rerrcode
errmsg (	Rerrmsg.
data (2.groupsvr.ArchiveList.DataRdata˙
Data3
list (2.groupsvr.ArchiveList.Data.ListRlistº
List

page_index (R	pageIndex
	page_size (RpageSize

page_count (R	pageCount
total (Rtotal:
items (2$.groupsvr.ArchiveList.Data.List.ItemRitemsÜ
Item

archive_id (R	archiveId
room_id (RroomId(
room_create_time (RroomCreateTime
play_id (RplayId
	play_name (	RplayName$
total_game_num (RtotalGameNum$
total_card_num (RtotalCardNumE
players (2+.groupsvr.ArchiveList.Data.List.Item.PlayerRplayersS
Player
user_id (RuserId
nickname (	Rnickname
jifen (Rjifen"§
ArchiveDetail
errcode (Rerrcode
errmsg (	Rerrmsg0
data (2.groupsvr.ArchiveDetail.DataRdataÆ
Data5
list (2!.groupsvr.ArchiveDetail.Data.ListRlistÓ
List
id (Rid

archive_id (R	archiveId
play_id (RplayId
	play_name (	RplayName$
total_game_num (RtotalGameNum
game_num (RgameNum'
settlement_time (RsettlementTimeB
players (2(.groupsvr.ArchiveDetail.Data.List.PlayerRplayersS
Player
user_id (RuserId
nickname (	Rnickname
jifen (Rjifen"Ø
ArchiveDetailRecord
errcode (Rerrcode
errmsg (	Rerrmsg6
data (2".groupsvr.ArchiveDetailRecord.DataRdata.
Data
id (Rid
record (Rrecord"É
CardFlow
errcode (Rerrcode
errmsg (	Rerrmsg+
data (2.groupsvr.CardFlow.DataRdataó
Data0
list (2.groupsvr.CardFlow.Data.ListRlist‹
List
is_paged (RisPaged

page_index (R	pageIndex
	page_size (RpageSize

page_count (R	pageCount
total (Rtotal7
items (2!.groupsvr.CardFlow.Data.List.ItemRitemsé
Item
id (Rid
group_id (RgroupId
	with_type (RwithType

trade_type (R	tradeType
card_number (R
cardNumber&
now_card_number (RnowCardNumber
status (Rstatus

createtime (R
createtime

updatetime	 (R
updatetime"ì
StatisticPlayed
errcode (Rerrcode
errmsg (	Rerrmsg2
data (2.groupsvr.StatisticPlayed.DataRdataô
Data:
total (2$.groupsvr.StatisticPlayed.Data.TotalRtotal7
list (2#.groupsvr.StatisticPlayed.Data.ListRlistá
Total*
total_game_number (RtotalGameNumber*
total_card_number (RtotalCardNumber&
now_card_number (RnowCardNumberë
List
is_paged (RisPaged

page_index (R	pageIndex
	page_size (RpageSize

page_count (R	pageCount
total (Rtotal>
items (2(.groupsvr.StatisticPlayed.Data.List.ItemRitemsº
Item
play_id (RplayId
	play_name (	RplayName*
total_game_number (RtotalGameNumber*
total_card_number (RtotalCardNumber&
now_card_number (RnowCardNumberbproto3