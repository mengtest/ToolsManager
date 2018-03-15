--------------------------------------------------------------------------------
-- 	 File      : RoomInitState.lua
--   author    : guoliang
--   function   : 房间初始化状态
--   date      : 2017-09-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

RoomInitState = class("RoomInitState",BaseObjectState)

function RoomInitState:ctor()

end

function RoomInitState:Init(parent)
	self.parent = parent
end


function RoomInitState:Enter(data)
	--先根据服务端信息改变房间状态
	self.parent:StateInit()
	--在根据房间状态初始化玩家
	self.parent:CreatePlayers()
	local sceneInit = false
	--根据房间状态设置房间UI
	if self.parent.roomInfo.status == RoomStateEnum.Idle then
		local play_logic = PlayGameSys.GetPlayLogic()
		local playLogicType = play_logic:GetType()
		if playLogicType == PlayLogicTypeEnum.WSK_Normal then
			sceneInit = self.parent.roomInfo.currentGamepos > 1
		elseif playLogicType == PlayLogicTypeEnum.MJ_Normal then
			sceneInit = self.parent.roomInfo.currentGamepos > 0
		elseif playLogicType == PlayLogicTypeEnum.ThirtyTwo_Normal then
			sceneInit = self.parent.roomInfo.currentGamepos > 1
		elseif playLogicType == PlayLogicTypeEnum.DDZ_Normal then
			sceneInit = self.parent.roomInfo.currentGamepos > 1
		else
			sceneInit = false
		end
	else
		sceneInit = true
	end
	LuaEvent.AddEvent(EEventType.PlaySceneInit,sceneInit)
	--策划要求 开始游戏才刷新
	LuaEvent.AddEvent(EEventType.RefreshRoomRoundNum,self.parent.roomInfo.currentGamepos,self.parent.roomInfo.totalGameNum)

	local roomOwner = self.parent.playerMgr:GetPlayerByPlayerID(self.parent.roomInfo.fangZhu)
	if roomOwner then
		LuaEvent.AddEvent(EEventType.PlayerShowFangZhu,roomOwner.seatPos,true)
	end
end

function RoomInitState:Leave()


end

function RoomInitState:Update()

end

function RoomInitState:GetType()
	return RoomStateEnum.Init
end
