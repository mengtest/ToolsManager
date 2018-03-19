--------------------------------------------------------------------------------
-- 	 File      : PlayLogicType.lua
--   author    : guoliang
--   function   : 游戏公共枚举数据
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
PlayLogicTypeEnum = {
	None 	   = 0,
	WSK_Normal = 1,
	WSK_Record = 2,
	MJ_Normal  = 3,
	MJ_Record  = 4,
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

	Max = 12,
}

-- 房间状态,0-没开始,1-游戏中,2-小局结束,3-所有已结束,4-房间解散中,5-选择打独中,6-选择找朋友

RoomStateEnum = {
	Init = -1,
	Idle = 0,
	Playing = 1,
	SmallRoundOver = 2,
	GameOver = 3,
	Dismissing = 4,
	CallAlone = 5,
	FindFriend = 6,
}

SeatPosEnum = {
	South = "South",
	East = "East",
	North = "North",
	West = "West",
}

SettingDataEnum = 
{
	PKBgType = "FightBg",
	MJBgType = "MJBgType",
	MJPaiType = "MJPaiType",
}
