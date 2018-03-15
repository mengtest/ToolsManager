--[[
@author: mid
@date: 2018年1月17日16:01:58
@desc: 单局结算控件
@usage:
]]
UISingleSettlementCtrl = class("UISingleSettlementCtrl", nil)
-- 玩家座位编码
local _seatPosEnum = {"East", "South", "West", "North"}
-- "single_settlement_root"
-- 外部初始化
function UISingleSettlementCtrl:Init(ctx)
	self.ctx = ctx
	self.rootTran = ctx:GetTrans("single_settlement_root", true)
	ctx:SetActive("single_settlement_root", true)
	self.curUIData = nil
	self.uiDataList = {}
	self.curPlayingAudioData = nil
	self.audioPlayList = {}
	self.cache_openedList = {}

	--定时器列表
	self.timerList = {}
	self:_InitUI()
	--初始化分数动画UI
	self:_RegEvent()
end
-- 外部销毁
function UISingleSettlementCtrl:Destroy()
	self:_UnRegEvent()
	self.curUIData = nil
	self.uiDataList = {}
	self.curPlayingAudioData = nil
	self.audioPlayList = {}
	self.ctx = nil
	self.rootTran = nil
	self.roomObj = nil
	self.playerMgr = nil
	self.t_ui = {}
	self.cache_openedList = {}

	self:RemoveTimer()
end

function UISingleSettlementCtrl:RemoveTimer()
	for k,v in pairs(self.timerList) do
		TimerTaskSys.RemoveTask(v)
	end
	self.timerList = {}
	self.curPlayingAudioData = nil
	self.curUIData = nil
end

-- 初始化 UI
function UISingleSettlementCtrl:_InitUI()
	local ctx = self.ctx
	local rootTran = self.rootTran
	self.t_ui = {}
	for i = 1, #_seatPosEnum do
		local _t = {}
		local seatPos = _seatPosEnum[i]
		local node = ctx:GetTrans(rootTran, seatPos)
		ctx:SetActive(node, true)
		_t.node = node
		local plus = ctx:GetTrans(node, "plus")
		ctx:SetActive(plus, false)
		_t.plus = plus
		local minus = ctx:GetTrans(node, "minus")
		ctx:SetActive(minus, false)
		_t.minus = minus
		local node_scoreAni = ctx:GetTrans(node, "node_scoreAni")
		_t.node_scoreAni = node_scoreAni
		ctx:SetActive(node_scoreAni, false)
		local node_paiXing = ctx:GetTrans(node, "node_paiXing")
		_t.node_paiXing = node_paiXing
		ctx:SetActive(node_paiXing, false)
		local txt_paiXing = ctx:GetTrans(node, "txt_paiXing")
		_t.txt_paiXing = txt_paiXing
		self.t_ui[seatPos] = _t
	end
	-- 隐藏所有按钮
	local btns = {
		-- ["btn_ready"] = "singleSettlement_btn_ready",
		["btn_totalSettlement"] = "singleSettlement_btn_totalSettlement"
	}
	for k, v in pairs(btns) do
		self[k] = ctx:GetTrans(rootTran, v)
		ctx:SetActive(self[k], false)
	end
end
-- 注册事件
function UISingleSettlementCtrl:_RegEvent()
	LuaEvent.AddHandle(EEventType.ShowPaiXing, self._ShowPaiXing, self)
	LuaEvent.AddHandle(EEventType.HidePaiXing, self._HidePaiXing, self)
	LuaEvent.AddHandle(EEventType.PK32_SingleSettlement, self._netcb_singleSettlement, self)
	LuaEvent.AddHandle(EEventType.HideAll, self._logic_hideAll, self)
end
-- 反注册事件
function UISingleSettlementCtrl:_UnRegEvent()
	LuaEvent.RemoveHandle(EEventType.ShowPaiXing, self._ShowPaiXing, self)
	LuaEvent.RemoveHandle(EEventType.HidePaiXing, self._HidePaiXing, self)
	LuaEvent.RemoveHandle(EEventType.PK32_SingleSettlement, self._netcb_singleSettlement, self)
	LuaEvent.RemoveHandle(EEventType.HideAll, self._logic_hideAll, self)
end
-- 牌型音频配置表

-- 牌型配置表
local _paiXingAudioCfg = {
	-- [0] = 1, -- 皇帝
	-- [17] = 2, -- 地杠
	-- [16] = 3, -- 天杠
	-- [111] = 4, -- 麻子
	[0] = 0,
	[1] = 1,
	[2] = 2,
	[3] = 3,
	[4] = 4,
	[5] = 5,
	[6] = 6,
	[7] = 7,
	[8] = 8,
	[9] = 9,
	[10] = 10,
	[11] = 11,
	[12] = 12,
	[13] = 13,
	[14] = 14,
	[15] = 15,
	[16] = 16,
	[17] = 17,
	[18] = 18,
	[19] = 19,
	[20] = 20,
	[21] = 21,
	[22] = 22,
	[23] = 23,
	[24] = 24,
	[25] = 25,
	[26] = 26,
	[27] = 27,
	[28] = 28,
	[29] = 29,
	[30] = 30,
	[31] = 31,
	[32] = 32,
	[33] = 33,
	[34] = 34,
	[35] = 35,
	[36] = 36,
	[37] = 37,
	[38] = 38,
	[39] = 39,
	[40] = 40,
	[41] = 41,
	[42] = 42,
	[43] = 43,
	[44] = 44,
	[45] = 45,
	[46] = 46,
	[47] = 47,
	[48] = 48,
	[49] = 49,
	[50] = 50,
	[51] = 51,
	[52] = 52,
	[53] = 53,
	[54] = 54,
	[55] = 55,
	[56] = 56,
	[57] = 57,
	[58] = 58,
	[59] = 59,
	[60] = 60,
	[61] = 61,
	[62] = 62,
	[63] = 63,
	[64] = 63, -- 64 = 63
	[65] = 65,
	[66] = 65, -- 66 = 65
	[67] = 67,
	[68] = 68,
	[69] = 69,
	[70] = 70,
	[71] = 71,
	[72] = 72,
	[73] = 73,
	[74] = 74,
	[75] = 74, -- 75 = 74
	[76] = 76,
	[77] = 76, -- 77 = 76
	[78] = 78,
	[79] = 79,
	[80] = 80,
	[81] = 81,
	[82] = 82,
	[83] = 83,
	[84] = 84,
	[85] = 85,
	[86] = 86,
	[87] = 86, -- 87 = 86
	[88] = 88,
	[89] = 88, -- 89 = 88
	[90] = 90,
	[91] = 91,
	[92] = 92,
	[93] = 93,
	[94] = 94,
	[95] = 95,
	[96] = 96,
	[97] = 96,
	[98] = 98, -- 97 = 96
	[99] = 98, -- 99 = 98
	[100] = 100,
	[101] = 101,
	[102] = 102,
	[103] = 103,
	[104] = 104,
	[105] = 105,
	[106] = 106,
	[107] = 107,
	[108] = 108,
	[109] = 109,
	[110] = 109, -- 110 = 109
	[111] = 111,
	[112] = 112, -- 天九王
	[113] = 113, -- 地九王
}
-- 牌型配置表
local _paiXingCfg = {
	[0] = 1, -- 皇帝
	[17] = 2, -- 地杠
	[16] = 3, -- 天杠
	[111] = 4, -- 麻子
	[112] = 5, -- 天九王
	[113] = 6, -- 地九王
	-- [0] = "皇帝",
	[1] = "对天",
	[2] = "对地",
	[3] = "对人",
	[4] = "对和",
	[5] = "对梅",
	[6] = "对长",
	[7] = "对板",
	[8] = "对斧",
	[9] = "对司",
	[10] = "对大幺",
	[11] = "对小幺",
	[12] = "对九",
	[13] = "对八",
	[14] = "对七",
	[15] = "对五",
	-- [16] = "天杠",
	-- [17] = "地杠",
	[18] = "天九",
	[19] = "地九",
	[20] = "人九",
	[21] = "和九",
	[22] = "梅九",
	[23] = "长九",
	[24] = "板九",
	[25] = "斧九",
	[26] = "司九",
	[27] = "幺九",
	[28] = "天八",
	[29] = "地八",
	[30] = "人八",
	[31] = "和八",
	[32] = "梅八",
	[33] = "斧八",
	[34] = "司八",
	[35] = "八点",
	[36] = "天七",
	[37] = "地七",
	[38] = "人七",
	[39] = "和七",
	[40] = "梅七",
	[41] = "长七",
	[42] = "板七",
	[43] = "斧七",
	[44] = "司七",
	[45] = "七点",
	[46] = "天六",
	[47] = "地六",
	[48] = "人六",
	[49] = "梅六",
	[50] = "长六",
	[51] = "斧六",
	[52] = "司六",
	[53] = "幺六",
	[54] = "六点",
	[55] = "天五",
	[56] = "地五",
	[57] = "人五",
	[58] = "和五",
	[59] = "梅五",
	[60] = "长五",
	[61] = "板五",
	[62] = "司五",
	[63] = "幺五",
	[64] = "幺五",
	[65] = "五点",
	[66] = "五点",
	[67] = "天四",
	[68] = "人四",
	[69] = "和四",
	[70] = "梅四",
	[71] = "长四",
	[72] = "板四",
	[73] = "斧四",
	[74] = "幺四",
	[75] = "幺四",
	[76] = "四点",
	[77] = "四点",
	[78] = "天三",
	[79] = "地三",
	[80] = "人三",
	[81] = "和三",
	[82] = "梅三",
	[83] = "长三",
	[84] = "板三",
	[85] = "司三",
	[86] = "幺三",
	[87] = "幺三",
	[88] = "三点",
	[89] = "三点",
	[90] = "天二",
	[91] = "地二",
	[92] = "人二",
	[93] = "和二",
	[94] = "长二",
	[95] = "板二",
	[96] = "幺二",
	[97] = "幺二",
	[98] = "二点",
	[99] = "二点",
	[100] = "天一",
	[101] = "地一",
	[102] = "人一",
	[103] = "和一",
	[104] = "梅一",
	[105] = "长一",
	[106] = "板一",
	[107] = "斧一",
	[108] = "幺一",
	[109] = "一点",
	[110] = "一点"
	-- [111] = "麻子",
}
-- 显示牌型
function UISingleSettlementCtrl:_ShowPaiXing(eventId, data,playVoice)
	if not data then
		return
	end

	local paiXingId = data.paiClass
	local player = PlayGameSys.GetPlayLogic().roomObj.playerMgr:GetPlayerBySeatID(data.seatId)
	if not player then
		return
	end
	if paiXingId == 16 then
		paiXingId = 112
	elseif paiXingId == 17 then
		paiXingId = 113
	elseif paiXingId > 17 then
		paiXingId = paiXingId - 2
	elseif paiXingId <16 then
		paiXingId = paiXingId
	end
	local seatPos = player.seatPos
	local sex = player.seatInfo.sex
	local ctx = self.ctx
	local uis = self.t_ui[seatPos]
	if not uis then
		DwDebug.Log("开牌位置错误" .. tostring(uis))
		return
	end
	ctx:SetActive(uis.node_paiXing, true)
	ctx:SetLabel(uis.txt_paiXing, tostring(_paiXingCfg[paiXingId]))

	if not not playVoice then
		local audioData = {}
		audioData.sex = sex
		audioData.audioId = _paiXingAudioCfg[paiXingId]
		
		self.cache_openedList[seatPos] = true
		self:TryPlayAudio(audioData)
	end
end

function UISingleSettlementCtrl:TryPlayAudio(audioData)
	local function _playAudio(audioData)
		AudioManager.ThirtyTwo_PlayBaoPai(audioData.sex, audioData.audioId)
	end

	if self.curPlayingAudioData then
		table.insert(self.audioPlayList, audioData)
	else
		self.curPlayingAudioData = audioData
		_playAudio(audioData)
		local timerIndex = TimerTaskSys.AddTimerEventByLeftTime(
			function()
				self.curPlayingAudioData = nil
				if #self.audioPlayList > 0 then
					self:TryPlayAudio(table.remove(self.audioPlayList, 1))
				end
			end,0.3)
		table.insert(self.timerList,timerIndex)
	end
end

-- 隐藏牌型
-- @param data: 有数据隐藏某个方位的 没数据全部隐藏
function UISingleSettlementCtrl:_HidePaiXing(eventId, data)
	if data then
		local player = PlayGameSys.GetPlayLogic().roomObj.playerMgr:GetPlayerBySeatID(data.seatId)
		if not player then
			return
		end

		local seatPos = player.seatPos
		local ctx = self.ctx
		local uis = self.t_ui[seatPos]
		if not uis then
			DwDebug.Log("隐藏开牌位置错误" .. tostring(uis))
			return
		end
		ctx:SetActive(uis.node_paiXing, false)
	else
		local t_ui = self.t_ui
		local ctx = self.ctx
		for i = 1, #_seatPosEnum do
			ctx:SetActive(t_ui[_seatPosEnum[i]].node_paiXing, false)
		end

		self:RemoveTimer()
	end
end

-- 隐藏所有开牌节点和动画
function UISingleSettlementCtrl:_logic_hideAll(eventId, data)
	local t_ui = self.t_ui
	for i = 1, #_seatPosEnum do
		local seatPos = _seatPosEnum[i]
		local uis = t_ui[seatPos]
		ctx:SetActive(uis.node_paiXing, false)
		ctx:SetActive(uis.node_scoreAni, false)
	end
	-- ctx:SetActive(self.btn_ready, false )
	ctx:SetActive(self.btn_totalSettlement, false)

	self:RemoveTimer()
end

-- 网络协议 单局结算
function UISingleSettlementCtrl:_netcb_singleSettlement(eventId, data)
	DwDebug.Log("单局结算", data)
	local ctx = self.ctx
	local t_ui = self.t_ui
	local playerMgr = PlayGameSys.GetPlayLogic().roomObj.playerMgr

	local function trydoAni(data)
		local function _do_ani(data)
			for i = 1, #_seatPosEnum do
				local item_data = data.players[i]
				if item_data then -- 2 3人场增加保护
					local player_data = playerMgr:GetPlayerByPlayerID(item_data.userId)
					if not player_data then
						DwDebug.Log("不存在 userId 为 ", item_data.userId)
					end
					local seatPos = player_data.seatPos
					local uis = t_ui[seatPos]
					if not uis then
						DwDebug.Log("开牌位置错误" .. tostring(uis))
						return
					end
					ctx:SetActive(uis.node_scoreAni, true)
					local score = item_data.winScore
					local is_win = score > 0
					ctx:SetActive(uis.plus, is_win)
					ctx:SetActive(uis.minus, not is_win)
					if is_win then
						ctx:SetLabel(uis.plus, "+" .. score .. "分")
					else
						ctx:SetLabel(uis.minus, score .. "分")
					end
					-- 修改头像下总积分
					LuaEvent.AddEventNow(EEventType.RefreshPlayerTotalScore, seatPos, item_data.totalScore)
					-- 添加自己的输赢的音效
					if seatPos == SeatPosEnum.South then
						local myResultAudioStr = is_win and UIAudioEnum.thirtyTwo_shengli or UIAudioEnum.thirtyTwo_shibai
						-- self.myResultAudioStr = myResultAudioStr
						if myResultAudioStr then
							DwDebug.Log("播放结果音效 " .. tostring(myResultAudioStr))
							AudioManager.PlayCommonSound(myResultAudioStr)
							myResultAudioStr = nil
						end
					else
						-- 检测是否存在未开牌的玩家 不存在则再刷新下
						--if not self.cache_openedList[seatPos] then
							-- 翻牌
							-- DwDebug.Log("补充log", seatPos, player_data.seatInfo, player_data.seatInfo.seatId)
							-- DwDebug.Log(self.cache_openedList)
							-- local data = {}
							-- data.seatId = player_data.seatInfo.seatId
							-- data.pai = item_data.handPais
							-- data.paiClass = item_data.paiClass
							-- LuaEvent.AddEventNow(EEventType.ThirtyTwo_OpenCards, data)
						--end
					end
				end
			end
			local timerIndex = TimerTaskSys.AddTimerEventByLeftTime(
				function()
					self.curUIData = nil
					if #self.uiDataList > 0 then
						-- DwDebug.Log("#self.uiDataList "..#self.uiDataList)
						-- WindowRoot.ShowTips("播放结束回调 剩下"..#self.uiDataList)
						_do_ani(table.remove(self.uiDataList, 1))
					end
				end,0.3)
			table.insert(self.timerList,timerIndex)
		end

		if (self.curUIData == nil) then
			self.curUIData = data
			-- WindowRoot.ShowTips("播放 剩余"..#self.uiDataList)
			_do_ani(data)
		else
			-- WindowRoot.ShowTips("插入队列"..#self.uiDataList)
			table.insert(self.uiDataList, data)
		end
	end

	

	local playLogicObj = PlayGameSys.GetPlayLogic()
	if not playLogicObj then
		DwDebug.Log("not playLogic")
		return
	end
	local roomObj = playLogicObj.roomObj
	if not roomObj then
		DwDebug.Log("not roomObj")
		return
	end
	self.playLogicObj = playLogicObj
	self.roomObj = roomObj
	-- 更新按钮状态
	local is_record = PlayGameSys.GetIsRecord()
	local is_dissmissed = false

	if not is_record then
		DwDebug.Log("是否解散房间", tostring(playLogicObj:IsRoomDismissed()))
		is_dissmissed = playLogicObj:IsRoomDismissed()
	end

	if is_dissmissed then
		LuaEvent.AddEventNow(EEventType.ThirtyTwo_ShowBetBtns, false)
	end

	--解散不播放
	if not is_dissmissed then
		trydoAni(data)
	end

	local is_show_total = ((roomObj:GetBigResult() or (data.now == data.total)) or is_dissmissed) and true or false
	local is_show_return = not PlayRecordSys.CheckNextRecordReplayInfo() and is_record
	-- 不是回放 也没有总结算数据
	--DwDebug.Log("是否显示准备", tostring((not is_show_total) and not is_record))
	-- ctx:SetActive(self.btn_ready, (not is_show_total) and not is_record )
	--DwDebug.Log("是否显示总结算", tostring((is_show_total) and not is_record))
	-- 有总结算数据
	ctx:SetActive(self.btn_totalSettlement, is_show_total and not is_record)
	
	--总结算的时候 做一些清理
	if is_show_total and not is_record then
		self:ShowTotalSettlement()
	end
	-- ctx:SetActive(self.btn_totalSettlement, true)
	-- 其他通知
	-- self:_closerLogic()



	DwDebug.Log("结算界面", self.btn_totalSettlement)
	-- 刷新牌局
	-- if (data.now + 1 <= data.total) and (not is_record) then
	--     LuaEvent.AddEvent(EEventType.RefreshRoomRoundNum, data.now + 1, data.total)
	-- end
end

function UISingleSettlementCtrl:ShowTotalSettlement()
	LuaEvent.AddEvent(EEventType.PK32ShowShuffleBtn,false)
	LuaEvent.AddEvent(EEventType.PK32ShowPrepareBtn,false)
	LuaEvent.AddEventNow(EEventType.PK32HideKaiPaiBtn)
end

-- 按钮事件(非回放状态)
function UISingleSettlementCtrl:DispatchEvent(go)
	local name = go.name
	if name == "singleSettlement_btn_ready" then
		self.playLogicObj:SendPrepare(0)
		-- 发送准备
		LuaEvent.AddEventNow(EEventType.RecordPlayNextRound)
		-- 刷新轮次
		self.roomObj:ChangeState(RoomStateEnum.Idle)
	elseif name == "singleSettlement_btn_totalSettlement" then
		if self.roomObj:GetBigResult() then
			self.roomObj:ChangeState(RoomStateEnum.GameOver)
		else
			local uiData = self.curUIData
			if uiData and uiData.now >= uiData.total then
				WindowRoot.ShowTips("总结算信息准备中,请稍候...")
			end
		end
	end
end
