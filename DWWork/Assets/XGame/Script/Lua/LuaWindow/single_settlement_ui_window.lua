local m_luaWindowRoot
local m_open
local m_state
local m_data
local m_countDownTime

local m_playLogic
local m_playLogicType
local m_ply2 -- 分清是麻将逻辑还是扑克逻辑
local m_isRecord

local m_isWSKAlone -- 房间是否打独
local m_isWSKIsDataValid -- 510k房间 打独 好友 等状态的数据是否有效的状态值

local m_TimeValue

local M_EPLT2 = {
	MJ = 1,
	WSK = 2,
}

single_settlement_ui_window = {

}

local _s = single_settlement_ui_window


function single_settlement_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function single_settlement_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)

	m_open = open
	m_playLogic = PlayGameSys.GetPlayLogic()

	if open then
		m_state = state
		m_countDownTime = 30
		m_isWSKAlone = false
		m_isWSKIsDataValid = false
		m_playLogicType = m_playLogic.GetType()
		-- 获取是否有总结算数据
		_s.m_hasBigResult = not not m_playLogic.roomObj:GetBigResult()
		_s.ClearPai()

		if m_playLogicType == PlayLogicTypeEnum.MJ_Normal or m_playLogicType == PlayLogicTypeEnum.MJ_Record then
			m_ply2 = M_EPLT2.MJ
		end
		if m_playLogicType == PlayLogicTypeEnum.WSK_Record or m_playLogicType == PlayLogicTypeEnum.WSK_Normal then
			m_ply2 = M_EPLT2.WSK
		end
		if m_playLogicType == PlayLogicTypeEnum.WSK_Record or m_playLogicType == PlayLogicTypeEnum.MJ_Record then
			m_isRecord = true
		else
			m_isRecord = false
		end
		_s.InitWindowDetail()
		UpdateSecond:Add(_s.CountDown, _s)
	else
		_s.ClearPai()

		UpdateSecond:Remove(_s.CountDown, _s)
		m_isWSKAlone = false
		m_isWSKIsDataValid = false

		if m_playLogic and m_playLogic.OnCloseSmallResult then
			m_playLogic:OnCloseSmallResult()
		end
	end
end

function single_settlement_ui_window.ClearPai()
	if _s.itemsToClear then
		local list = _s.itemsToClear
		for i=1,#list do
			m_luaWindowRoot:SetActive( list[i], false )
		end
	end

	_s.itemsToClear = {}
end

function single_settlement_ui_window.CreateWindow()
	if DataManager.isPrePublish then
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("share_btn"),false)
	end

	m_TimeValue = WrapSys.GetTimeValue()
	
	LuaEvent.AddHandle(EEventType.ScreenShotEnd,single_settlement_ui_window.ScreenShotEnd, nil)
end

function single_settlement_ui_window.InitWindowDetail()

	local room = m_playLogic.roomObj
	m_data = room:GetCurrSmallResult()

	_s.InitGameResultInfo()

	-- data
	local players
	if m_ply2 == M_EPLT2.WSK then
		players = m_data.players
	end
	if m_ply2 == M_EPLT2.MJ then
		players = m_data.playerMes
	end

	if not players then return end

	local isShowFriend = (m_ply2 == M_EPLT2.WSK) and (not m_isWSKAlone) and m_isWSKIsDataValid
	local myUserId = DataManager.GetUserID()

	if isShowFriend then
		local friendSeatId = -1
		for i=1,#players do
			if players[i].userId == myUserId then
				friendSeatId = players[i].friendSeatId
				players[i].isFriend = true
			else
				players[i].isFriend = false
			end
		end
		-- print("friendSeatId "..friendSeatId)
		local userId = PlayGameSys.GetPlayLogic().roomObj.playerMgr:GetPlayerBySeatID(friendSeatId).seatInfo.userId
		-- print("friendSeatId "..userId)
		for i=1,#players do
			if players[i].userId == userId then
			-- print("friendSeatId "..friendSeatId)
				players[i].isFriend = true
			end
		end
	else
		for i=1,#players do
			players[i].isFriend = false
		end
	end

	local indexs = LogicUtil.GetSortIndex(players)
	for i=1,4 do
		_s.InitPlayerResultInfo(m_luaWindowRoot:GetTrans("player_" .. i), players[indexs[i]])
	end
end

function single_settlement_ui_window.InitGameResultInfo()

	local id = 0
	local time = ""
	local name = ""
	local doc = ""
	local now = 0
	local total = 0

	-- data

	if m_ply2 == M_EPLT2.WSK then
		id, time, name, doc, now, total = m_data.id, m_data.time, m_data.name, m_data.doc, m_data.now, m_data.total
		m_isWSKAlone = m_data.isAlone
		-- print("friendSeatId "..tostring(m_isWSKAlone))
		m_isWSKIsDataValid = m_data.dataValid
	end

	if m_ply2 == M_EPLT2.MJ then
		id, time, name, doc, now, total = m_data.roomId, m_data.settlementTime, m_data.roomName, m_data.playDes, m_data.currentGamepos, m_data.totalGameNum
	end

	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("room_id"),id)
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("time"),time)
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("game_name"),name)
	local describe = string.gsub(doc, ",", "\n")
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("info_1"), describe)
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("round_info"),now .. "/" .. total .. "r")

	-- 回放或者(最后一局,中途退出)屏蔽关闭按钮和倒计时
	if m_isRecord then -- or (_s.m_hasBigResult)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("timer"), false)
		UpdateSecond:Remove(_s.CountDown, _s)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("close_btn"), false)
	else
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("timer"), true)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("close_btn"), true)
	end

	--不是回放且(最后一局,中途退出)显示总结算按钮
	if not m_isRecord and (now >= total or _s.m_hasBigResult) then
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("settlement_btn"), true)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("continue_btn"), false)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("back_btn"), false)
	--回放且最后一局显示返回按钮
	elseif m_isRecord and not PlayRecordSys.CheckNextRecordReplayInfo() then
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("settlement_btn"), false)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("continue_btn"), false)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("back_btn"), true)
	else
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("settlement_btn"), false)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("continue_btn"), true)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("back_btn"), false)
	end
end

function single_settlement_ui_window.InitPlayerResultInfo(root, data)

	-- data
	local userId, nickName, totalScore, isFangZhu, winScore = 0,"",0,true,0
	if m_ply2 == M_EPLT2.WSK then
		userId, nickName, totalScore, isFangZhu, winScore =
		data.userId, data.nickName, data.totalScore, data.isFangZhu, data.winScore
	end
	if m_ply2 == M_EPLT2.MJ then
		userId, nickName, totalScore, isFangZhu, winScore =
		data.userId, data.nickName, data.zongJifen, data.fangZhu, data.jifen
	end

	nickName = utf8sub(nickName,1,10)

	local player = m_playLogic.roomObj.playerMgr:GetPlayerByPlayerID(userId)
	if player then
		WindowUtil.LoadHeadIcon(m_luaWindowRoot,m_luaWindowRoot:GetTrans(root, "head_ico"), player.seatInfo.headUrl, player.seatInfo.sex, false,RessStorgeType.RST_Never)
	end

	-- 510增加是否 打独logo
	local is_show_du = false
	if m_ply2 == M_EPLT2.WSK and m_isWSKIsDataValid then
		is_show_du = data.isAlone
	end
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(root,"510_is_single_logo"),is_show_du)

	-- 510增加是否 好友logo
	local is_show_friend = false
	if m_ply2 == M_EPLT2.WSK then
		is_show_friend = data.isFriend and true or false
	end
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(root,"510_is_friend"),is_show_friend)

	local bg_hl = m_luaWindowRoot:GetTrans(root,"bg_hl")
	m_luaWindowRoot:SetActive( bg_hl, false )

	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "name"),nickName)
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "player_id"),"ID:" .. userId)
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "player_lv"),totalScore)
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(root, "banker"), not not data.zhuangJia)
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(root, "wskBanker"), not not data.isBanker and not is_show_du)
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(root, "host"),isFangZhu)

	m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans(root, "point"), "crmj", m_ply2 == M_EPLT2.MJ)
	m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans(root, "result"), "crmj", m_ply2 == M_EPLT2.MJ)
	-- 如果是510k扑克
	if m_ply2 == M_EPLT2.WSK then
		local result_root = m_luaWindowRoot:GetTrans(m_luaWindowRoot:GetTrans(root, "result"), "cr510k")
		-- 扑克结果
		_s.InitPKResultInfo(result_root, data)

		local point_root = m_luaWindowRoot:GetTrans(m_luaWindowRoot:GetTrans(root, "point"), "cr510k")
		_s.InitPointInfo(point_root, winScore,bg_hl)
	elseif m_ply2 == M_EPLT2.MJ then
		local result_root = m_luaWindowRoot:GetTrans(m_luaWindowRoot:GetTrans(root, "result"), "crmj")
		-- 麻将结果
		_s.InitMJResultInfo(result_root, data)

		local point_root = m_luaWindowRoot:GetTrans(m_luaWindowRoot:GetTrans(root, "point"), "crmj")
		_s.InitPointInfo(point_root, winScore,bg_hl)
	end
end

function single_settlement_ui_window.InitPointInfo(root, winScore,bg_hl)
	if winScore > 0 then
		m_luaWindowRoot:ShowChild(root, "win", true)
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "win"),winScore)
		-- if winScore > 0 then
			m_luaWindowRoot:SetActive(bg_hl, true )
		-- end
	else
		m_luaWindowRoot:ShowChild(root, "lose", true)
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "lose"),winScore)
	end
end

function single_settlement_ui_window.InitPKResultInfo(root, data)
	m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans(root, "rank"), tostring(data.overOrder), true)
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(root, "smash"), data.isDouble)

	local score_point = data.winScore - data.bombScore
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "score_point"), score_point > 0 and "+" .. score_point or tostring(score_point))

	if data.bombOdds > 0 then
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(root, "bomb"), true)
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "bomb_times"),"x" .. data.bombOdds)
	else
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(root, "bomb"), false)
	end
end

-- 显示麻将结果
function single_settlement_ui_window.InitMJResultInfo(root, data)
	-- 胡牌还是自摸还是放炮
	local result_tip = m_luaWindowRoot:GetTrans(root, "result_tip")
	m_luaWindowRoot:SetActive(result_tip, data.huType == 1 or data.huType == 2 or data.fangPao)
	if data.huType == 1 then
		m_luaWindowRoot:ShowChild(result_tip, "zimo", true)
	elseif data.huType == 2 then
		m_luaWindowRoot:ShowChild(result_tip, "hu", true)
	elseif data.fangPao then
		m_luaWindowRoot:ShowChild(result_tip, "pao", true)
	end
	-- 胡的牌型
	local fanxing = m_luaWindowRoot:GetTrans(root, "fanxing_info")
	m_luaWindowRoot:SetActive(fanxing, data.fanXingId > 0)
	local fanxing_value = m_luaWindowRoot:GetTrans(fanxing, "fanxing_value")
	if data.fanXingId > 0 then
		m_luaWindowRoot:SetSprite(fanxing_value, "fanxing_" .. data.fanXingId)
		m_luaWindowRoot:MakePixelPerfect(fanxing_value)
	end

	_s.genMJView(m_luaWindowRoot:GetTrans(root, "mj_card"),data.opeList, data.normalPai, data.huPai)
end

-- 显示麻将组合和手牌
function single_settlement_ui_window.genMJView(crmj,opeList,normalPai, hupai)
	local groups = {}
	local normals = {}
	for i=0,4 do
		groups[i] = m_luaWindowRoot:GetTrans(crmj,"crmj_node_group_"..i)
		normals[i] = m_luaWindowRoot:GetTrans(crmj,"crmj_node_normal_"..i)
	end

	local group_num = 0
	local root_node, pai_id, seq_id, pos_x, pos_y, pai_ids, is_anGang, is_self

	for i=1,#opeList do
		local op = opeList[i]
		local opeId = op.opeId
		if opeId == 2 then
			group_num = group_num +1
			root_node = groups[group_num]
			local scale = 0.68
			root_node.localPosition = UnityEngine.Vector3.New(142*(group_num-1),0 ,0)
			root_node.localScale = UnityEngine.Vector3.New(scale,scale,scale)
			pai_ids = op.pengPai.pai
			pai_id = pai_ids[1]
			is_anGang = false
			is_self = true
			for j=1,3 do
				seq_id = j
				pos_x = (j-1)*65
				pos_y = 0
				_s.itemsToClear[#_s.itemsToClear+1] = LogicUtil.SetMJCardAtrr(m_luaWindowRoot,root_node,pai_id,seq_id,pos_x,pos_y,j,is_anGang,is_self)
			end
		elseif opeId == 3 then
			group_num = group_num +1
			root_node = groups[group_num]
			local scale = 0.68
			root_node.localPosition = UnityEngine.Vector3.New(142*(group_num-1),0 ,0)
			root_node.localScale = UnityEngine.Vector3.New(scale,scale,scale)
			pai_ids = op.gangPai.pai
			pai_id = pai_ids[1]
			is_anGang = (op.gangPai.gangType == 1)
			is_self = true
			for j=1,4 do
				seq_id = j
				pos_x = (j-1)*65
				if j == 4 then
					pos_x = 65
				end
				if j == 4 or not is_anGang then
					pos_y = -3.4
				else
					pos_y = 0
				end
				_s.itemsToClear[#_s.itemsToClear+1] = LogicUtil.SetMJCardAtrr(m_luaWindowRoot,root_node,pai_id,seq_id,pos_x,pos_y,j,is_anGang,is_self)
			end
		end
	end
	local scale = 0.59
	root_node = normals[group_num]
	root_node.localPosition = UnityEngine.Vector3.New(142*(group_num), 0, 0)
	root_node.localScale = UnityEngine.Vector3.New(scale,scale,scale)
	is_anGang = false
	is_self = true
	pos_y = 5
	for i=1,#normalPai do
		pai_id = normalPai[i]
		seq_id = i
		pos_x = (i-1)*80
		_s.itemsToClear[#_s.itemsToClear+1] = LogicUtil.SetMJCardAtrr(m_luaWindowRoot,root_node,pai_id,seq_id,pos_x,pos_y,j,is_anGang,is_self, true)
	end

	-- 显示胡的那张牌
	root_node = m_luaWindowRoot:GetTrans(crmj, "hupai")
	root_node.localScale = UnityEngine.Vector3.New(scale,scale,scale)
	if hupai > 0 then
		-- m_luaWindowRoot:SetActive(root_node, true)
		local hupai_card = LogicUtil.SetMJCardAtrr(m_luaWindowRoot,root_node,hupai,1,-10,pos_y,1,false,true,true)
		_s.itemsToClear[#_s.itemsToClear+1] = hupai_card
	-- else
		-- m_luaWindowRoot:SetActive(root_node, false)
	end
end

--显示隐藏按钮
function single_settlement_ui_window.SetShareVisable(show)
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("shareContent"), show)
end

function single_settlement_ui_window.OnShareClick()
	if m_TimeValue.Value > 0 then
		WindowUtil.LuaShowTips("您点击太快,请稍后再试")
		return
	end
	m_TimeValue.Value = 5
	_s.SetShareVisable(false)
	HallSys.ShareTotalSettlement(false)
end

function single_settlement_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name
	if click_name == "share_btn" then
		_s.OnShareClick()
	elseif click_name == "back_btn" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		PlayGameSys.QuitToMainCity()
	elseif click_name == "close_btn" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		_s.HandleClickCloseBtn()
	elseif click_name == "continue_btn" then
		_s.GoToNextRound()
	elseif click_name == "settlement_btn" then
		if m_playLogic.roomObj:GetBigResult() then
			_s.InitWindow(false, 0)
			m_playLogic.roomObj:ChangeState(RoomStateEnum.GameOver)
		else
			WindowUtil.LuaShowTips("总结算信息准备中,请稍候...")
		end
	end
end

--截屏完后 恢复按钮
function single_settlement_ui_window.ScreenShotEnd()
	if m_luaWindowRoot then
		_s.SetShareVisable(true)
	end
end

function single_settlement_ui_window.NormalCloseWindow()
	_s.InitWindow(false, 0)
	--刷新总牌局总结分
	local tempPlayer

	-- data
	local players, total, next = {},0,0
	if m_ply2 == M_EPLT2.WSK then
		players = m_data.players
		total = m_data.total
		next = m_data.next
	end
	if m_ply2 == M_EPLT2.MJ then
		players = m_data.playerMes
		total = m_data.totalGameNum
		next = m_data.nextGamepos
	end

	for k,v in pairs(players) do
		tempPlayer = m_playLogic.roomObj.playerMgr:GetPlayerByPlayerID(v.userId)
		if tempPlayer then
			-- data
			local totalScore = 0
			if m_ply2 == M_EPLT2.WSK then
				totalScore = v.totalScore
			end
			if m_ply2 == M_EPLT2.MJ then
				totalScore = v.zongJifen
			end
			LuaEvent.AddEvent(EEventType.RefreshPlayerTotalScore,tempPlayer.seatPos, totalScore)
		end
	end

	LuaEvent.AddEvent(EEventType.RefreshRoomRoundNum,next,total)

	m_playLogic.roomObj:UpdateCurrentRound(next)
	m_playLogic.roomObj:ChangeState(RoomStateEnum.Idle)

	_s.ClearPai()
end

function single_settlement_ui_window.GoToNextRound()
	_s.NormalCloseWindow()

	if not m_isRecord then
		m_playLogic:SendPrepare(0)
	else
		LuaEvent.AddEventNow(EEventType.RecordPlayNextRound)
	end
end

function single_settlement_ui_window.HandleClickCloseBtn()
	_s.NormalCloseWindow()
	_s.GoToIdle()
end

function single_settlement_ui_window.GoToIdle()
	local total, now = 0,0
	if m_ply2 == M_EPLT2.WSK then
		total = m_data.total
		now = m_data.now
	elseif m_ply2 == M_EPLT2.MJ then
		total = m_data.totalGameNum
		now = m_data.currentGamepos
	end

	if m_playLogic.roomObj:GetBigResult() then
		m_playLogic.roomObj:ChangeState(RoomStateEnum.GameOver)
	else
		if now >= total then
			WindowUtil.LuaShowTips("总结算信息准备中,请稍候...")
		else
			-- if not m_isRecord then
			--  local player = m_playLogic.roomObj.playerMgr:GetPlayerByPlayerID(DataManager.GetUserID())
			--  if player then
			--      player:ChangeState(PlayerStateEnum.Idle)
			--  end
			-- else
			-- end
		end
	end
end

function single_settlement_ui_window.CountDown()
	if m_countDownTime > 0 then
		m_countDownTime = m_countDownTime - 1
		if m_countDownTime <= 0 then
			_s.HandleClickCloseBtn()
		end
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("timer"), m_countDownTime .. "s")
	end
end

function single_settlement_ui_window.UnRegister()
	LuaEvent.RemoveHandle(EEventType.ScreenShotEnd,single_settlement_ui_window.ScreenShotEnd, nil)
	m_luaWindowRoot = nil
end

function single_settlement_ui_window.OnDestroy()
	_s.UnRegister()
	m_TimeValue = nil
end
