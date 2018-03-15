WebEvent = {
    LoginWeb = -1, -- Web 登陆
    LogoutWeb = -2, -- Web 退出登陆
    CreateRoomWeb = -3, -- 创建房间 ( Web )
    JoinRoomWeb = -4, -- 加入房间 ( Web )
    ShareRoomWeb = -5, -- 房间内分享应答消息 ( Web )
    ShareHallWeb = -6, -- 大厅内分享应答消息 ( web )
    NoticeList = -7, -- 公告列表 (web)
    ShareSingleSettlement = -8, --分享单局结算
    ShareTotalSettlement = -9, --分享总结算
    RecordList = -10, -- 战绩列表
    RecordDetail = -11, --战绩详情
    RecordDetailRoundReplyInfo = -12, --战绩详情的一局回放信息
    RecordGameSvrInfo = -13, -- 游戏服回放信息对应ID
    RecordInfo = -14, -- 回放信息
    GetUserInfoWeb = -15, -- 获取用户信息 ( Web )
    ShareGame = -16, -- 分享游戏 ( Web )
    ShareGameReward = -17, -- 分享游戏奖励 ( Web )
    ShareSingleSettlement = -18, -- 分享单局结算 ( Web )
    ShareTotalSettlement = -19, -- 分享总结算 ( Web )
    IOSBuyItemList = -20,
    -- IOS购买列表
    IOSCreateOrder = -21,
    --创建订单
    IOSCheckBuyResult = -22,
    --验证购买回执
    IOSQueryBuyResultStatus = -23,
    --查询服务端验证状态
    UserBindAgent = -24,
    --绑定代理
    UserMyAgent = -25,
    --绑定代理
    LbsNearby = -26, --请求附近的人信息
    IsShareGameRewarded = -27, --是否已经分享过
    UseNetBaseInfo = -49,
    --请求用户通讯服基础信息
    ClubList = -50, -- 请求俱乐部列表
    ClubDetail = -51, -- 请求俱乐部详情
    CreateClub = -52, -- 请求创建俱乐部
    UpdateClub = -53, -- 请求更新俱乐部
    DeleteClub = -54, -- 请求解散俱乐部
    ClubMemList = -55, -- 请求俱乐部成员列表
    ClubMemDetail = -56, -- 请求俱乐部成员详情
    ApplyJoinClub = -57, -- 请求加入俱乐部
    HandleApplyClub = -58, -- post处理申请操作
    KickoutClub = -59, -- 踢出群
    QuitClub = -60, --退出群
    MsgCategoryList = -61, -- 消息类型列表
    TemplateList = -62, --房间模板列表
    TemplateDetail = -63, --房间模板详情
    CreateTemplate = -64, -- 创建房间模板
    UpdateTemplate = -65, -- 修改更新房间模板
    RoomList = -66, -- 俱乐部房间列表
    ArchiveList = -67, -- 俱乐部战绩列表
    ArchiveDetail = -68, -- 俱乐部战绩详情
    ArchiveDetailRecord = -69, -- 俱乐部战绩回放
    CardFlow = -70, -- 俱乐部房卡流水
    StatisticsPlayed = -71, -- 俱乐部局数统计
    RechargeClub = -72, -- 俱乐部充值
    DeleteTemplate = -73, --删除俱乐部模板
    -- imsvr 消息系统
    MsgList = -74, -- 指定类消息列表
    MsgMailMessageIsReadAddr = -75, -- 标记邮件是否已读
    -- 邮件相关
    MailGroupApplyJoin = -300,
    MailCommon = -301,
    -- 邮件的 socket
    MailCPlayMail = 25257, -- 新邮件通知
    MailCGroupCardWarn = 25201 -- 俱乐部房卡不足邮件提醒
}
