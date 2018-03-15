--------------------------------------------------------------------------------
--   File      : UIMJDeskCenterCtrl.lua
--   author    : mid
--   function  : UI 麻将桌面风向和倒计时
--   date      : 2017-11-10
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
local math_floor = math.floor
UIMJDeskCenterCtrl = class("UIMJDeskCenterCtrl",nil)

function UIMJDeskCenterCtrl:Init(rootTran,luaWin)

	self.light = nil
	self.ten = nil
	self.single = nil

	self.m_normal = {}
	self.m_highlight = {}

	-- member
	self.luaWindowRoot = luaWin
	self.m_rootTran = rootTran
	self.m_data = nil
	self.m_lastFengXiangId = 0
	self.m_countDownSecond = nil
	self.m_detlaSecond1 = 0
	self.m_detlaSecond2 = 0

	self.isPlaying = false

	self.cfg_selfFengXiangAndEnumToFengXiang = {
		[1] = {
			[1] = 1,
			[2] = 2,
			[3] = 3,
			[4] = 4,
		},
		[2] = {
			[3] = 2,
			[4] = 3,
			[1] = 4,
			[2] = 1,
		},
		[3] = {
			[1] = 3,
			[2] = 4,
			[3] = 1,
			[4] = 2,
		},
		[1] = {
			["South"] = 4,
			["East"] = 1,
			["North"] = 2,
			["West"] = 3,
		},
	}

	self:Initwgts()
	self:RegEvent()
	UpdateBeat:Add(self.UpdatePerFrame,self)
end

function UIMJDeskCenterCtrl:Destroy()
	self.isPlaying = false
	self.luaWindowRoot:SetActive(self.m_rootTran,false)
	self:UnRegEvent()
	UpdateBeat:Remove(self.UpdatePerFrame,self)

	self.luaWindowRoot = nil
	self.m_data = nil
	self.m_lastFengXiangId = nil
	self.m_rootTran = nil
	self.m_countDownSecond = 0
	self.m_normal = {}
	self.m_highlight = {}

end

function UIMJDeskCenterCtrl:Initwgts()
	self.luaWindowRoot:SetActive(self.m_rootTran,true)
	self.light = self.luaWindowRoot:GetTrans(self.m_rootTran,"light")
	self.dire = self.luaWindowRoot:GetTrans(self.m_rootTran,"dire")
	self.ten = self.luaWindowRoot:GetTrans(self.m_rootTran,"ten")
	self.single = self.luaWindowRoot:GetTrans(self.m_rootTran,"single")

	self.m_normal[1] = self.luaWindowRoot:GetTrans(self.m_rootTran,"east")
	self.m_normal[2] = self.luaWindowRoot:GetTrans(self.m_rootTran,"south")
	self.m_normal[3] = self.luaWindowRoot:GetTrans(self.m_rootTran,"west")
	self.m_normal[4] = self.luaWindowRoot:GetTrans(self.m_rootTran,"north")

	self.m_highlight[1] = self.luaWindowRoot:GetTrans(self.m_rootTran,"east_hl")
	self.m_highlight[2] = self.luaWindowRoot:GetTrans(self.m_rootTran,"south_hl")
	self.m_highlight[3] = self.luaWindowRoot:GetTrans(self.m_rootTran,"west_hl")
	self.m_highlight[4] = self.luaWindowRoot:GetTrans(self.m_rootTran,"north_hl")

	self.luaWindowRoot:SetActive(self.light,false)
	self.luaWindowRoot:SetActive(self.ten,false)
	self.luaWindowRoot:SetActive(self.single,false)

	self.luaWindowRoot:SetActive(self.m_highlight[1],false)
	self.luaWindowRoot:SetActive(self.m_highlight[2],false)
	self.luaWindowRoot:SetActive(self.m_highlight[3],false)
	self.luaWindowRoot:SetActive(self.m_highlight[4],false)
	self.luaWindowRoot:SetActive(self.m_rootTran,false)

	self.m_countDownSecond = 0
end

function UIMJDeskCenterCtrl:RegEvent()
	LuaEvent.AddHandle(EEventType.MJUpdateWindDir,self.UpdateWindDir,self)
	LuaEvent.AddHandle(EEventType.MJShowDeskCenterCtrl,self.MJShowDeskCenterCtrl,self)
end

function UIMJDeskCenterCtrl:UnRegEvent()
	LuaEvent.RemoveHandle(EEventType.MJUpdateWindDir,self.UpdateWindDir, self)
	LuaEvent.RemoveHandle(EEventType.MJShowDeskCenterCtrl,self.MJShowDeskCenterCtrl,self)
end

function UIMJDeskCenterCtrl:MJShowDeskCenterCtrl(eventId,p1,p2)
	local isShow = p1
	self.isPlaying = isShow
	self.luaWindowRoot:SetActive(self.m_rootTran,isShow)

	-- 中心旋转
	local south_uid = PlayGameSys.GetPlayLogic().roomObj:GetSouthUID()
	self.selfFengXiang = PlayGameSys.GetPlayLogic().roomObj.playerMgr:GetPlayerByPlayerID(south_uid).seatInfo.fengxiang
	-- print("MJShowDeskCenterCtrl selfFengXiang "..self.selfFengXiang)
	local t = { 0, -90, -180, -270 }
	local rotation = t[self.selfFengXiang]
	if not rotation then return end
	self.rotation = rotation
	self.dire.localEulerAngles = UnityEngine.Vector3.New(0,0,rotation)

end



function UIMJDeskCenterCtrl:UpdatePerFrame(deltatime,unscaledDeltaTime)
	if not self.isPlaying then
		return
	end
	-- print("UIMJDeskCenterCtrl UpdatePerFrame")

	self.m_detlaSecond1 = self.m_detlaSecond1 + deltatime
	self.m_detlaSecond2 = self.m_detlaSecond2 + deltatime
	if self.m_detlaSecond1 >= 1 then
		self.m_detlaSecond1 = 0
		self.luaWindowRoot:SetActive(self.light, not self.light.gameObject.activeSelf)
	end

	if self.m_countDownSecond >= 0 then
		if self.m_detlaSecond2 >= 1 then
			self.m_detlaSecond2 = 0

			if self.m_countDownSecond >= 0 then

				self.m_countDownSecond = self.m_countDownSecond - 1
				-- print(m_countDownSecond)
				self:UpdateNum()
				if self.m_countDownSecond == -1 then
					--self.luaWindowRoot:SetActive(self.light,true)
					-- self.luaWindowRoot:SetActive(self.ten,false)
					-- self.luaWindowRoot:SetActive(self.single,false)
					-- test todo 发送提醒事件
					-- LuaEvent.AddEventNow(EEventType.MJUpdateWindDir,(m_lastFengXiangId)%4+1)

					--超时添加等待提示
					if self:IsFengXiangSelf(self.m_lastFengXiangId) then
					 AnimManager.PlayWaitTip(true)
					end
				end
			end
		end
		
	end

end

function UIMJDeskCenterCtrl:IsFengXiangSelf(fengxiang)
	local own = PlayGameSys.GetPlayLogic().roomObj.playerMgr:GetPlayerByPlayerID(PlayGameSys.GetPlayLogic().roomObj:GetSouthUID())
	if nil ~= own then
		local ownfengxiang = own.seatInfo.fengxiang
		if fengxiang == ownfengxiang then
			return true
		end
	end

	return false
end

function UIMJDeskCenterCtrl:UpdateWindDir(eventId,p1)
	if self.m_lastFengXiangId then
		self.luaWindowRoot:SetActive(self.m_normal[self.m_lastFengXiangId],true)
		self.luaWindowRoot:SetActive(self.m_highlight[self.m_lastFengXiangId],false)
	end

	-- print("test.... "..p1)
	local roomObj = PlayGameSys.GetPlayLogic().roomObj
	local cur_player = roomObj:GetPlayerBySeatPos(p1)
	local fengxiang = cur_player.seatInfo.fengxiang
	-- print("fengxiang"..fengxiang)
	self.m_lastFengXiangId = fengxiang
	self.luaWindowRoot:SetActive(self.m_normal[self.m_lastFengXiangId],false)
	self.luaWindowRoot:SetActive(self.m_highlight[self.m_lastFengXiangId],true)

	self.m_countDownSecond = 15
	local t = {-90,0,90,180}
	local rotation = t[self.m_lastFengXiangId] + 90
	if not rotation then return end
	self.light.localEulerAngles = UnityEngine.Vector3.New(0,0,rotation)
	self.luaWindowRoot:SetActive(self.light,true)

	self.luaWindowRoot:SetActive(self.ten,true)
	self.luaWindowRoot:SetActive(self.single,true)
	self:UpdateNum()

	--清理等待提示
	AnimManager.PlayWaitTip(false)
end

function UIMJDeskCenterCtrl:UpdateNum()
	if self.m_countDownSecond >= 0 then
		local n1 = math_floor(self.m_countDownSecond/10)
		local n2 = math_floor(self.m_countDownSecond%10)
		local sprite_name_pre = "num_"
		self.luaWindowRoot:SetSprite(self.ten,sprite_name_pre..n1)
		self.luaWindowRoot:SetSprite(self.single,sprite_name_pre..n2)

		--播放倒计时声音
		if self.m_countDownSecond > 0 and self.m_countDownSecond <= 5 and PlayGameSys.GetPlayLogic() then
			--判断是否在游戏中
			local roomObj = PlayGameSys.GetPlayLogic().roomObj
			if roomObj then
				local curStateType = roomObj.roomStateMgr:GetCurStateType()
				
				if curStateType == RoomStateEnum.Playing then 
					AudioManager.PlayCommonSound(UIAudioEnum.time_countdown)
				end
			end
		end
	end
end
