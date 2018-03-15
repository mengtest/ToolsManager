--------------------------------------------------------------------------------
--   File      : UIMJScoreCtrl.lua
--   author    : mid
--   function  : UI 麻将飘分
--   date      : 2017-11-10
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

local math_floor = math.floor

UIMJScoreCtrl = class("UIMJScoreCtrl",nil)

function UIMJScoreCtrl:Init(rootTran,luaWin)
	self.currData = nil
	self.datas = {}

	self.m_luaWindowRoot = luaWin
	self.m_rootTran = rootTran

	self:Initwgts()
	-- luaWin:SetActive(rootTran,false)
	self:RegEvent()
end

function UIMJScoreCtrl:Destroy()
	self:UnRegEvent()

	self.currData = nil
	self.datas = {}
	self.m_luaWindowRoot = nil
	self.m_rootTran = nil

	if self.timer_ExecTwn and self.timer_ExecTwn ~= -1 then
		TimerTaskSys.RemoveTask(self.timer_ExecTwn)
		self.timer_ExecTwn = -1
	end
end

function UIMJScoreCtrl:Initwgts()
	local m_luaWindowRoot = self.m_luaWindowRoot
	local m_rootTran = self.m_rootTran

	self.wgt_West = m_luaWindowRoot:GetTrans(m_luaWindowRoot:GetTrans(m_rootTran,"West"),"node")
	self.wgt_West_plus = m_luaWindowRoot:GetTrans(self.wgt_West,"plus")
	self.wgt_West_minus = m_luaWindowRoot:GetTrans(self.wgt_West,"minus")

	self.wgt_East = m_luaWindowRoot:GetTrans(m_luaWindowRoot:GetTrans(m_rootTran,"East"),"node")
	self.wgt_East_plus = m_luaWindowRoot:GetTrans(self.wgt_East,"plus")
	self.wgt_East_minus = m_luaWindowRoot:GetTrans(self.wgt_East,"minus")

	self.wgt_South = m_luaWindowRoot:GetTrans(m_luaWindowRoot:GetTrans(m_rootTran,"South"),"node")
	self.wgt_South_plus = m_luaWindowRoot:GetTrans(self.wgt_South,"plus")
	self.wgt_South_minus = m_luaWindowRoot:GetTrans(self.wgt_South,"minus")

	self.wgt_North = m_luaWindowRoot:GetTrans(m_luaWindowRoot:GetTrans(m_rootTran,"North"),"node")
	self.wgt_North_plus = m_luaWindowRoot:GetTrans(self.wgt_North,"plus")
	self.wgt_North_minus = m_luaWindowRoot:GetTrans(self.wgt_North,"minus")

	m_luaWindowRoot:SetActive(self.wgt_West,false)
	m_luaWindowRoot:SetActive(self.wgt_East,false)
	m_luaWindowRoot:SetActive(self.wgt_South,false)
	m_luaWindowRoot:SetActive(self.wgt_North,false)

end

function UIMJScoreCtrl:RegEvent()
	LuaEvent.AddHandle(EEventType.MJShowScoreTwn,self.ExecTwn,self)
end

function UIMJScoreCtrl:UnRegEvent()
	LuaEvent.RemoveHandle(EEventType.MJShowScoreTwn, self.ExecTwn, self)
end

-- local function getTest( )
--  local t = {
--      userJifen = {
--          {
--              userId = "North",
--              jifen = 1,
--              onceJifen = 123,
--          },
--          {
--              userId = "South",
--              jifen = 234,
--              onceJifen = 234,
--          },
--          {
--              userId = "East",
--              jifen = 3,
--              onceJifen = -23,
--          },
--          {
--              userId = "West",
--              jifen = 3,
--              onceJifen = -123,
--          },
--      }
--  }

--  return t
-- end

function UIMJScoreCtrl:ExecTwn(eventId, p1)
	if nil == p1 then
		return
	end

	local m_luaWindowRoot = self.m_luaWindowRoot
	local m_rootTran = self.m_rootTran

	-- m_luaWindowRoot:SetActive(m_rootTran,true)

	-- local data = getTest().userJifen

	if nil == self.currData then
		self.currData = p1

		local data = p1.userJifen
		for i=1,#data do
			local item = data[i]

			-- test
			-- local pos = item.userId
			local pos = PlayGameSys.GetPlayLogic().roomObj.playerMgr:GetPlayerByPlayerID(item.userId).seatPos

			if not self["wgt_"..pos] then return end

			local score = item.onceJifen

			-- 通知头像组件分数修改
			LuaEvent.AddEventNow(EEventType.RefreshPlayerTotalScore, pos, item.jifen)

			if score ~= 0 then
				local show_str = score > 0 and "plus" or "minus"
				local hide_str = score < 0 and "plus" or "minus"

				-- print("show_str:", show_str, "hide_str:", hide_str)

				-- self["wgt_"..pos].gameObject:SetActive(true)

				m_luaWindowRoot:SetActive(self["wgt_"..pos], true)
				m_luaWindowRoot:SetActive(self["wgt_"..pos.."_"..show_str], true)
				m_luaWindowRoot:SetActive(self["wgt_"..pos.."_"..hide_str], false)
				if score > 0 then
					score = "+"..score
				end
				m_luaWindowRoot:SetLabel(self["wgt_"..pos.."_"..show_str], score)
			end
		end
		if self.timer_ExecTwn and self.timer_ExecTwn ~= -1 then
			TimerTaskSys.RemoveTask(self.timer_ExecTwn)
			self.timer_ExecTwn = -1

		end
		self.timer_ExecTwn = TimerTaskSys.AddTimerEventByLeftTime(function ()
			self.timer_ExecTwn = -1
			self.currData = nil
			if 0 < #self.datas then
				self:ExecTwn(nil, table.remove(self.datas, 1))
			end
		end,2.11)
	else
		table.insert( self.datas, p1 )
	end
end
