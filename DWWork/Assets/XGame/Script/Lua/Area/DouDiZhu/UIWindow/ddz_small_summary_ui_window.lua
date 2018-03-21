--------------------------------------------------------------------------------
-- 	 File       : ddz_small_summary_ui_window.lua
--   author     : zhanghaochun
--   function   : 斗地主小结算界面
--   date       : 2018-2-5
--   copyright  : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
local m_luaWindowRoot
local m_open
local m_state

local m_playLogic
-- 总结算数据
local m_hasBigResult
-- 是否是回放
local m_isRecord
-- 结算信息
local m_data
-- 当前局是否胜利
local m_isWin
-- 玩家当前的身份
local m_isLord

local m_root_1_trans
local m_root_2_trans
local m_root_2_detailRoot

local m_root_2_CountDownLabel

local m_root_1_CountDownTime
local m_root_2_CountDownTime

local m_TimeValue

ddz_small_summary_ui_window = {}
local _s = ddz_small_summary_ui_window

function ddz_small_summary_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function ddz_small_summary_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)

	m_open = open
	if open then
		m_state = state
		_s.InitWindowDetail()
	else
		UpdateSecond:Remove(_s.Root_2_CountDown)
	end
end

function ddz_small_summary_ui_window.CreateWindow()
	m_root_1_trans = m_luaWindowRoot:GetTrans("Root_1")
	m_root_2_trans = m_luaWindowRoot:GetTrans("Root_2")

	m_root_2_CountDownLabel = m_luaWindowRoot:GetTrans(m_root_2_trans, "txt_restSeconds")
	m_root_2_detailRoot =  m_luaWindowRoot:GetTrans(m_root_2_trans, "DetailRoot")

	m_TimeValue = WrapSys.GetTimeValue()

	LuaEvent.AddHandle(EEventType.ScreenShotEnd, ddz_small_summary_ui_window.ScreenShotEnd, nil)
end

function ddz_small_summary_ui_window.InitWindowDetail()
	m_playLogic = PlayGameSys.GetPlayLogic()
	-- 是否有总结算数据
	m_playLogicType = m_playLogic.GetType()
	if m_playLogicType == PlayLogicTypeEnum.DDZ_Normal then
		m_isRecord = false
	else
		m_isRecord = true
	end
	local room = m_playLogic.roomObj
	m_data = room:GetCurrSmallResult()
	m_hasBigResult = not not m_playLogic.roomObj:GetBigResult()
	
	_s.SetBaseInfo()

	if m_data.overStatus == 0 or m_data.overStatus == 1 then
		-- 流局或是解散
		_s.InitRoot2Show()
	else
		_s.InitRoot1Show()
	end
end

function ddz_small_summary_ui_window.InitRoot1Show()
	m_luaWindowRoot:SetActive(m_root_1_trans, true)
	m_luaWindowRoot:SetActive(m_root_2_trans, false)
	
	local title = m_luaWindowRoot:GetTrans(m_root_1_trans, "Title")
	local decorate = m_luaWindowRoot:GetTrans(m_root_1_trans, "Decorate")

	AudioManager.DDZ_PlayWinOrLose(m_isWin)
	if m_isWin then
		m_luaWindowRoot:SetSprite(decorate, "ddz_win_stars")
		if m_isLord then
			m_luaWindowRoot:SetSprite(title, "ddz_dizhu_win")
		else
			m_luaWindowRoot:SetSprite(title, "ddz_nongming_win")
		end
	else
		m_luaWindowRoot:SetSprite(decorate, "ddz_lose_leaf")
		if m_isLord then
			m_luaWindowRoot:SetSprite(title, "ddz_dizhu_lose")
		else
			m_luaWindowRoot:SetSprite(title, "ddz_nongming_lose")
		end
	end	

	m_root_1_CountDownTime = 0
	UpdateSecond:Add(_s.Root_1_CountDown)
end

function ddz_small_summary_ui_window.InitRoot2Show()
	m_luaWindowRoot:SetActive(m_root_1_trans, false)
	m_luaWindowRoot:SetActive(m_root_2_trans, true)

	m_luaWindowRoot:SetActive(m_root_2_detailRoot, false)

	local players = m_data.players
	if players == nil then return end
	-- 设置title
	if m_isWin then
		m_luaWindowRoot:SetSprite(m_luaWindowRoot:GetTrans(m_root_2_trans, "Title"), "ddz_title_win")
	else
		m_luaWindowRoot:SetSprite(m_luaWindowRoot:GetTrans(m_root_2_trans, "Title"), "ddz_title_lose")
	end

	local indexs = LogicUtil.GetSortIndex(players)
	local itemRoot = m_luaWindowRoot:GetTrans(m_root_2_trans, "ItemRoot")
	--for i = 1, 3 do
		--_s.InitItem(m_luaWindowRoot:GetTrans(itemRoot, "item_" .. i), players[indexs[i]])
	--end
	_s.InitItem(m_luaWindowRoot:GetTrans(itemRoot, "item_" .. 1), players[indexs[1]])
	_s.InitItem(m_luaWindowRoot:GetTrans(itemRoot, "item_" .. 2), players[indexs[2]])
	_s.InitItem(m_luaWindowRoot:GetTrans(itemRoot, "item_" .. 3), players[indexs[4]])

	m_root_2_CountDownTime = 15
	UpdateSecond:Add(_s.Root_2_CountDown)
		
	_s.InitGameResultInfo()
end

function ddz_small_summary_ui_window.SetBaseInfo()
	local players = m_data.players
	if players == nil then
		m_isWin = false
		m_isLord = false
		return
	end

	for k, v in ipairs(players) do
		if v.isBanker then
			if v.userId == DataManager.GetUserID() then
				m_isLord = true
				if v.winScore >= 0 then
					m_isWin = true
				else
					m_isWin = false
				end
			else
				m_isLord = false
				if v.winScore >= 0 then
					m_isWin = false
				else
					m_isWin = true
				end
			end
			break
		end
	end
end

function ddz_small_summary_ui_window.InitItem(trans, info)
	-- 是否是房主
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(trans, "owner"), info.isFangZhu)
	-- 是否是地主
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(trans, "lordflag"), info.isBanker)
	-- 名字
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "name"), info.nickName)
	-- 玩家当前得分
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "score"), info.winScore)
	-- 玩家总得分
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "totalScore"), info.totalScore)
	-- 倍数
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "multiple"), "x" .. info.multiple.sum)
end

function ddz_small_summary_ui_window.InitGameResultInfo()
	local id = 0
	local time = ""
	local name = ""
	local doc = ""
	local now = 0
	local total = 0
	id, time, name, doc, now, total = m_data.id, m_data.time, m_data.name, m_data.doc, m_data.now, m_data.total

	-- 回放或者(最后一局,中途退出)屏蔽关闭按钮和倒计时
	if m_isRecord then -- or (_s.m_hasBigResult)
		m_luaWindowRoot:SetActive(m_root_2_CountDownLabel, false)
		UpdateSecond:Remove(_s.Root_2_CountDown)
	else
		m_luaWindowRoot:SetActive(m_root_2_CountDownLabel, true)
	end

	--不是回放且(最后一局,中途退出)显示总结算按钮
	if not m_isRecord and (now >= total or m_hasBigResult) then
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("settlement_btn"), true)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("continue_btn"), false)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("back_btn"), false)
	--回放且最后一局显示返回按钮
	elseif m_isRecord and not PlayRecordSys.CheckNextRecordReplayInfo() then
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("settlement_btn"), false)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("continue_btn"), false)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("back_btn"), true)
	else
		if m_hasBigResult then
			m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("settlement_btn"), true)
			m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("continue_btn"), false)
			m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("back_btn"), false)
		else
			m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("settlement_btn"), false)
			m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("continue_btn"), true)
			m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("back_btn"), false)
		end
	end
end

--截屏完后 恢复按钮
function ddz_small_summary_ui_window.ScreenShotEnd()
	if m_luaWindowRoot then
		_s.SetShareVisable(true)
	end
end

-----------------------------------点击事件--------------------------------------
function ddz_small_summary_ui_window.HandleWidgetClick(go)
	local click_name = go.name
	if click_name == "share_btn" then
		_s.OnShareClick()
	elseif click_name == "back_btn" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		PlayGameSys.QuitToMainCity()
	elseif click_name == "continue_btn" then
		_s.GoToNextRound()
	elseif click_name == "detailBtn" then
		m_luaWindowRoot:SetActive(m_root_2_detailRoot, true)
		_s.OnDetailBtnClick()
	elseif click_name == "DetailBG" then
		m_luaWindowRoot:SetActive(m_root_2_detailRoot, false)
	elseif click_name == "settlement_btn" then
		if m_playLogic.roomObj:GetBigResult() then
			_s.InitWindow(false, 0)
			m_playLogic.roomObj:ChangeState(RoomStateEnum.GameOver)
		else
			WindowUtil.LuaShowTips("总结算信息准备中,请稍候...")
		end
	end
end

function ddz_small_summary_ui_window.HandleClickCloseBtn()
	_s.NormalCloseWindow()
	_s.GoToIdle()
end

function ddz_small_summary_ui_window.GoToIdle()
	local total, now = 0,0
	total = m_data.total
	now = m_data.now
	if m_playLogic.roomObj:GetBigResult() then
		m_playLogic.roomObj:ChangeState(RoomStateEnum.GameOver)
	else
		if now >= total then
			WindowUtil.LuaShowTips("总结算信息准备中,请稍候...")
		else
			--if not m_isRecord then
				--local player = m_playLogic.roomObj.playerMgr:GetPlayerByPlayerID(DataManager.GetUserID())
			    --if player then
			        --player:ChangeState(PlayerStateEnum.Idle)
			    --end
			--end
		end
	end
end

function ddz_small_summary_ui_window.OnShareClick()
	if m_TimeValue.Value > 0 then
		WindowUtil.LuaShowTips("您点击太快,请稍后再试")
		return
	end
	m_TimeValue.Value = 5
	_s.SetShareVisable(false)
	HallSys.ShareTotalSettlement(false)
end

function ddz_small_summary_ui_window.GoToNextRound()
	_s.NormalCloseWindow()

	if not m_isRecord then
		if m_data.overStatus == 1 then
			PlayGameSys.QuitToMainCity()
		else
			m_playLogic:SendPrepare(0)
		end
	else
		LuaEvent.AddEventNow(EEventType.RecordPlayNextRound)
	end
end

function ddz_small_summary_ui_window.OnDetailBtnClick()
	_s.SetDetailInfoShow()
end
--------------------------------------------------------------------------------

-----------------------------------计时函数--------------------------------------
function ddz_small_summary_ui_window.Root_1_CountDown()
	m_root_1_CountDownTime = m_root_1_CountDownTime + 1
	if m_root_1_CountDownTime >= 2 then
		UpdateSecond:Remove(_s.Root_1_CountDown)
		_s.InitRoot2Show()
	end
end

function ddz_small_summary_ui_window.Root_2_CountDown()
	m_root_2_CountDownTime = m_root_2_CountDownTime - 1
	if m_root_2_CountDownTime >= 0 then
		m_luaWindowRoot:SetLabel(m_root_2_CountDownLabel, m_root_2_CountDownTime .. "s")
	else
		_s.HandleClickCloseBtn()
	end
end

--------------------------------------------------------------------------------

function ddz_small_summary_ui_window.SetDetailInfoShow()
	local myInfo = _s.FindMeInfo()
	if myInfo == nil then
		m_luaWindowRoot:SetActive(m_root_2_detailRoot, false)
		return
	end
	local multipleInfo = myInfo.multiple
	local emptyStr = "- -"
	-- 初始分
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_root_2_detailRoot, "d_init"), "x " .. multipleInfo.base)
	-- 炸弹倍数
	if multipleInfo.bomb > 1 then
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_root_2_detailRoot, "d_bomb"), "x " .. multipleInfo.bomb)
	else
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_root_2_detailRoot, "d_bomb"), emptyStr)
	end
	-- 炸弹是否达到上限
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(m_root_2_detailRoot, "d_bombLimit"), multipleInfo.isBombLimit)
	-- 明牌
	local openText = ""
	if multipleInfo.open > 1 then
		openText = "x " .. multipleInfo.open
	else
		openText = emptyStr
	end
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_root_2_detailRoot, "d_open"), openText)
	-- 春天反春天
	local springText = ""
	if multipleInfo.spring > 1 then
		springText = "x " .. multipleInfo.spring
	elseif multipleInfo.antiSpring > 1 then
		springText = "x " .. multipleInfo.antiSpring
	else
		springText = emptyStr
	end
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_root_2_detailRoot, "d_spring"), springText)
	-- 公共倍数
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_root_2_detailRoot, "d_commonplus"), "x " .. multipleInfo.common)
	-- 农民加倍
	if multipleInfo.plus > 0 then
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_root_2_detailRoot, "d_famerplus"), "x " .. multipleInfo.plus)
	else
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_root_2_detailRoot, "d_famerplus"), emptyStr)
	end
	
	-- 地主加倍
	if multipleInfo.back > 0 then
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_root_2_detailRoot, "d_lordplus"), "x " .. multipleInfo.back)
	else
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_root_2_detailRoot, "d_lordplus"), emptyStr)
	end
	--总倍数
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_root_2_detailRoot, "d_totalplus"), "" .. multipleInfo.sum)
end

function ddz_small_summary_ui_window.FindMeInfo()
	--for k, v in ipairs(m_data.players) do
		--DwDebug.LogError(v.multiple)
	--end
	local play_logic = PlayGameSys.GetPlayLogic()
	for k, v in ipairs(m_data.players) do
		if v.userId == play_logic.roomObj:GetSouthUID() then
			return v
		end
	end	
	
	return nil
end

function ddz_small_summary_ui_window.NormalCloseWindow()
	_s.InitWindow(false, 0)
	--刷新总牌局总结分
	local tempPlayer

	-- data
	local players, total, next = {},0,0
	players = m_data.players
	total = m_data.total
	next = m_data.next


	for k,v in pairs(players) do
		tempPlayer = m_playLogic.roomObj.playerMgr:GetPlayerByPlayerID(v.userId)
		if tempPlayer then
			-- data
			local totalScore = 0
			totalScore = v.totalScore
			LuaEvent.AddEvent(EEventType.RefreshPlayerTotalScore,tempPlayer.seatPos, totalScore)
		end
	end

	--LuaEvent.AddEvent(EEventType.RefreshRoomRoundNum,next,total)

	m_playLogic.roomObj:UpdateCurrentRound(next)
	m_playLogic.roomObj:ChangeState(RoomStateEnum.Idle)
end

--显示隐藏按钮
function ddz_small_summary_ui_window.SetShareVisable(show)
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("node_hideWhenSharing"), show)
end

function ddz_small_summary_ui_window.UnRegister()
	LuaEvent.RemoveHandle(EEventType.ScreenShotEnd, ddz_small_summary_ui_window.ScreenShotEnd, nil)
	m_luaWindowRoot = nil
end

function ddz_small_summary_ui_window.OnDestroy()
	_s.UnRegister()
	m_TimeValue = nil
end