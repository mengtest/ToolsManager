--------------------------------------------------------------------------------
-- 	 File			: PlayLogicType.lua
--	 author		: guoliang
--	 function	 : 游戏公共枚举数据
--	 date			: 2017-09-26
--	 copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
--对应玩法ID http://wiki.dw7758.com/#!game/play.md
Common_PlayID = {
    chongRen_510K = 10001,
    leAn_510K = 10004,
    yiHuang_510K = 10005,
    ThirtyTwo = 10007,
    chongRen_MJ = 1,
    leAn_MJ = 3,
    yiHuang_MJ = 4,
    DW_DouDiZhu = 10006, -- 斗地主
    ZhaJinHua = 10003, --扎金花
    UserSvrNet = 25000 -- 用户通讯服连接，特殊情况
}

Common_PlayText = {
    [Common_PlayID.chongRen_510K] = {name = "崇仁打盾"},
    [Common_PlayID.leAn_510K] = {name = "乐安打盾"},
    [Common_PlayID.yiHuang_510K] = {name = "宜黄红心5"},
    [Common_PlayID.chongRen_MJ] = {name = "崇仁麻将"},
    [Common_PlayID.leAn_MJ] = {name = "乐安麻将"},
    [Common_PlayID.yiHuang_MJ] = {name = "宜黄麻将"},
    [Common_PlayID.ThirtyTwo] = {name = "32张"},
    [Common_PlayID.DW_DouDiZhu] = {name = "斗地主"},
    [Common_PlayID.ZhaJinHua] = {name = "扎金花"}
}

CardKindType = {
    PK = 0,
    -- 扑克
    MJ = 1,
    -- 麻将
    QP = 2 --桥牌
}

PlayLogicTypeEnum = {
    None = 0,
    WSK_Normal = 1,
    WSK_Record = 2,
    MJ_Normal = 3,
    MJ_Record = 4,
    ThirtyTwo_Normal = 5,
    ThirtyTwo_Record = 6,
    DDZ_Normal = 7,
    DDZ_Normal_Record = 8,
    ZhaJinHua_Normal = 9,
    ZhaJinHua_Normal_Record = 10
}

-- 玩家状态,1-初始化渲染,2-空闲,3-已经准备,4-叫牌中,5-找朋友,6-出牌中,7-等待其他人出牌 8 牌出完 9 好友观战中

PlayerStateEnum = {
    None = 0,
    Init = 1,
    Idle = 2,
    Prepared = 3,
    CallAlone = 4,
    FindFriend = 5,
    Playing = 6,
    Waiting = 7,
    Finished = 8,
    WatchFriend = 9,
    ChoosedFriend = 10,
    Destroy = 11,
    ExpectPushCard = 12,
    WantLord = 13, -- 抢地主
    LordIsOpenPlay = 14, -- 地主明牌
    FamerAddPlus = 15, -- 农民加倍
    LordSubPlus = 16, -- 地主加倍
    Max = 17
}

-- 麻将 0-没开始,1-小局结束,2-游戏中,3-所有已结束，
MJRoomStateEnum = {
    Init = -1,
    Idle = 0,
    SmallRoundOver = 1,
    Playing = 2,
    GameOver = 3,
    Dismissing = 4
}
MJRoomStateEnum.__index = MJRoomStateEnum

-- 510K房间状态,0-没开始,1-游戏中,2-小局结束,3-所有已结束,4-房间解散中,5-选择打独中,6-选择找朋友
PKRoomStateEnum = {
    Init = -1,
    Idle = 0,
    Playing = 1,
    SmallRoundOver = 2,
    GameOver = 3,
    Dismissing = 4,
    CallAlone = 5,
    FindFriend = 6
}
PKRoomStateEnum.__index = PKRoomStateEnum

-- 32张房间状态,0-没开始,1-游戏中,2-小局结束,3-所有已结束
ThirtyTwoRoomStateEnum = {
    Init = -1,
    Idle = 0,
    Playing = 1,
    SmallRoundOver = 2,
    GameOver = 3,
    Dismissing = 4
}
ThirtyTwoRoomStateEnum.__index = ThirtyTwoRoomStateEnum

-- 斗地主房间状态    5:抢地主状态   6:地主是否亮牌状态   7:农民加倍状态   8:地主反加倍状态
DDZRoomStateEnum = {
    Init = -1,
    Idle = 0,
    Playing = 1,
    SmallRoundOver = 2,
    GameOver = 3,
    Dismissing = 4,
    WantLord = 5,
    LordIsOpenPlay = 6,
    FamerAddPlus = 7,
    LordSubPlus = 8
}
DDZRoomStateEnum.__index = DDZRoomStateEnum

-- 炸金花房间状态,0-没开始,1-游戏中,2-小局结束,3-所有已结束
ZhaJinHuaRoomStateEnum = {
    Init = -1,
    Idle = 0,
    Playing = 1,
    SmallRoundOver = 2,
    GameOver = 3,
    Dismissing = 4
}
ZhaJinHuaRoomStateEnum.__index = ZhaJinHuaRoomStateEnum

RoomStateEnum = {}

SeatPosEnum = {
    South = "South",
    East = "East",
    North = "North",
    West = "West",
    Five = "Five",
    Six = "Six"
}

SeatPosSort = {
    [SeatPosEnum.South] = 1,
    [SeatPosEnum.East] = 2,
    [SeatPosEnum.North] = 3,
    [SeatPosEnum.West] = 4,
    [SeatPosEnum.Five] = 5,
    [SeatPosEnum.Six] = 6
}

SeatPosIndex = {
    [1] = SeatPosEnum.South,
    [2] = SeatPosEnum.East,
    [3] = SeatPosEnum.North,
    [4] = SeatPosEnum.West,
    [5] = SeatPosEnum.Five,
    [6] = SeatPosEnum.Six
}

UMengSharePlatform = {
    WEIXIN = 1,
    -- 微信
    WEIXIN_CIRCLE = 2 --微信朋友圈
}

-- 麻将杠类型
-- 1--暗杠 2--明杠 3--转角杠
MJGangType = {
    AnGang = 1,
    MingGang = 2,
    ZhuangJiaoGang = 3
}

-- 麻将胡牌类型
MJHuPaiType = {
    None = 0,
    ZiMo = 1,
    Hu = 2,
    QiangGang = 3,
    GangShangHua = 4,
    LiuJu = 10
}

EnumMail_EventTypeID = {
    -- socket
    MailCPlayMail = 25257, -- 新邮件通知 -- USER_SVR_CMD.SC_NEW_MAIL_NOTIFY, --新邮件通知
    MailCGroupCardWarn = 25201,
    -- 俱乐部房卡不足警告提醒 -- USER_SVR_CMD.MAILTYPE_GROUP_CARD_WARN,
    -- web mail
    MailGroupApplyJoin = 25000, -- 申请加入俱乐部消息 -- USER_SVR_CMD.MAILTYPE_GROUP_APPLY_JOIN_MESSAGE,
    MailPassGroupApplyJoin = 25001, -- 通过审核申请消息 -- USER_SVR_CMD.MAILTYPE_GROUP_APPLY_JOIN_SUCCESS,
    MailRejectGroupApplyJoin = 25002, -- 不通过审核申请消息 -- USER_SVR_CMD.MAILTYPE_GROUP_APPLY_JOIN_FAIL,
    MailRegisterReward = 25003, -- 注册奖励房卡 -- USER_SVR_CMD.MAILTYPE_GROUP_APPLY_JOIN_FAIL,
    MailBindAgentReward = 25004, -- 绑定代理奖励房卡 -- USER_SVR_CMD.MAILTYPE_GROUP_APPLY_JOIN_FAIL,
    MailRecharge = 25005, -- 充值房卡 -- USER_SVR_CMD.MAILTYPE_GROUP_APPLY_JOIN_FAIL,
    MailShareGameReward = 25006, -- 分享朋友圈奖励房卡 -- USER_SVR_CMD.MAILTYPE_GROUP_APPLY_JOIN_FAIL,
    MailTackbackCard = 25007, -- 收回房卡 -- USER_SVR_CMD.MAILTYPE_GROUP_APPLY_JOIN_FAIL,
    MailGiveCard = 25008, -- 赠送房卡 -- USER_SVR_CMD.MAILTYPE_GROUP_APPLY_JOIN_FAIL,
    MailExitClub = 25009, -- 退出俱乐部 -- USER_SVR_CMD.MAILTYPE_GROUP_APPLY_JOIN_FAIL,
    MailKickOutClub = 25010, -- 踢出俱乐部 -- USER_SVR_CMD.MAILTYPE_GROUP_APPLY_JOIN_FAIL,
    MailDisbandClub = 25011 -- 解散俱乐部 -- USER_SVR_CMD.MAILTYPE_GROUP_APPLY_JOIN_FAIL,
}

-- enum 邮件消息类型 大类别
ENUM_MAIL_CATEGORY_TYPE = {
    SysMsg = 1, --系统通知
    BuyCardNotice = 2, -- 购卡通知
    ActiveiNotice = 3, -- 活动通知
    ClubMsg = 101, --群(俱乐部)消息
    ClubApplyJoinNotice = 102 -- 申请入群(俱乐部)通知
}

-- 邮件隐射表 每个 eventType 对应 pbCmd
MAP_Mail_EventType_2_pbCmdId = {
    [EnumMail_EventTypeID.MailCPlayMail] = WebEvent.MailCPlayMail,
    [EnumMail_EventTypeID.MailCGroupCardWarn] = WebEvent.MailCGroupCardWarn,
    [EnumMail_EventTypeID.MailGroupApplyJoin] = WebEvent.MailGroupApplyJoin,
    [EnumMail_EventTypeID.MailPassGroupApplyJoin] = WebEvent.MailCommon,
    [EnumMail_EventTypeID.MailRejectGroupApplyJoin] = WebEvent.MailCommon,
    [EnumMail_EventTypeID.MailRegisterReward] = WebEvent.MailCommon,
    [EnumMail_EventTypeID.MailBindAgentReward] = WebEvent.MailCommon,
    [EnumMail_EventTypeID.MailRecharge] = WebEvent.MailCommon,
    [EnumMail_EventTypeID.MailShareGameReward] = WebEvent.MailCommon,
    [EnumMail_EventTypeID.MailTackbackCard] = WebEvent.MailCommon,
    [EnumMail_EventTypeID.MailGiveCard] = WebEvent.MailCommon,
    [EnumMail_EventTypeID.MailExitClub] = WebEvent.MailCommon,
    [EnumMail_EventTypeID.MailKickOutClub] = WebEvent.MailCommon,
    [EnumMail_EventTypeID.MailDisbandClub] = WebEvent.MailCommon
}

-- 获取是否自摸
function MJHuPaiType.IsMJZiMo(huType)
    if huType == MJHuPaiType.ZiMo or huType == MJHuPaiType.GangShangHua then
        return true
    end
    return false
end

-- 获取是否没有胡
function MJHuPaiType.IsMJNoneOrLiuJu(huType)
    if huType ~= MJHuPaiType.None and huType ~= MJHuPaiType.LiuJu then
        return false
    end
    return true
end
