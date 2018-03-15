--------------------------------------------------------------------------------
-- 	 File      : PlayerPointMgrModule.lua
--   author    : guoliang
--   function   : 玩家积分管理组件
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

PlayerPointMgrModule = class("PlayerPointMgrModule", DWBaseModule)


function PlayerPointMgrModule:ctor()
	self.pointTable = {} -- 手牌积分
	self.bombNumTable = {} -- 炸弹赔率
	self.totalScoreTable = {} -- 牌局总积分
end
function PlayerPointMgrModule:Init(parent)
	self.parent = parent
end

function  PlayerPointMgrModule:AddPlayPoint(seatPos,addPoint, isAnim)
	if self.pointTable[seatPos] == nil then
		self.pointTable[seatPos] = 0
	end
	self.pointTable[seatPos] = self.pointTable[seatPos] + addPoint
	
	if isAnim then
		LuaEvent.AddEventNow(EEventType.ShowPlayerPointAnim, seatPos, addPoint)
	end
	
	LuaEvent.AddEventNow(EEventType.RefreshPlayerPoint,seatPos,self.pointTable[seatPos])
	
	
end

function  PlayerPointMgrModule:AddPlayBombNum(seatPos,addBombNum, isAnim)
	if self.bombNumTable[seatPos] == nil then
		self.bombNumTable[seatPos] = 0
	end
	self.bombNumTable[seatPos] = self.bombNumTable[seatPos] + addBombNum

	if isAnim then
		LuaEvent.AddEventNow(EEventType.ShowPlayerBombAnim, seatPos, addBombNum)
	end
	
	LuaEvent.AddEventNow(EEventType.RefreshPlayerBombNum,seatPos,self.bombNumTable[seatPos])
end

function PlayerPointMgrModule:RefreshPlayerTotalScore(seatPos,totalScore)
	self.totalScoreTable[seatPos] = totalScore
end

function PlayerPointMgrModule:GetPlayerTotalScore(seatPos)
	return self.totalScoreTable[seatPos]
end

function PlayerPointMgrModule:ClearForNewRound( )
	self.pointTable = {} -- 手牌积分
	self.bombNumTable = {} -- 炸弹赔率
	self.totalScoreTable = {} -- 牌局总积分
end

function PlayerPointMgrModule:Destroy()
	self.pointTable = nil
	self.bombNumTable = nil
end