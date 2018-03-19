total_settlement_ui_window = {

}
local _s = total_settlement_ui_window

local m_luaWindowRoot
local m_state
local m_data
local m_open
local m_ply2 -- 分清是麻将逻辑还是扑克逻辑

local m_TimeValue

local M_EPLT2 = {
	MJ = 1,
	WSK = 2,
}

function total_settlement_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function total_settlement_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	m_open = open
	if open then
		m_state = state
		_s.InitWindowDetail()
	else 
		m_ply2 = nil
	end
end

function total_settlement_ui_window.CreateWindow()
	if DataManager.isPrePublish then
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("share_btn"),false)
	end

	m_TimeValue = WrapSys.GetTimeValue()

	LuaEvent.AddHandle(EEventType.ScreenShotEnd,total_settlement_ui_window.ScreenShotEnd, self)
end

function total_settlement_ui_window.InitWindowDetail()
	_s.ShowShareWindow(false)

	m_playLogic = PlayGameSys.GetPlayLogic()
	m_playLogicType = m_playLogic.GetType()
	local room = m_playLogic.roomObj
	m_data = room:GetBigResult()

	if m_playLogicType == PlayLogicTypeEnum.MJ_Normal then
		m_ply2 = M_EPLT2.MJ
	end
	if m_playLogicType == PlayLogicTypeEnum.WSK_Normal then
		m_ply2 = M_EPLT2.WSK
	end

	if m_ply2 == nil then
		_s.InitWindow(false, 0)
		PlayGameSys.QuitToMainCity()
	end

	_s.InitGameResultInfo()

	local playerMgr = room.playerMgr

	local maxScore = 0
	local key,key1

	local players = nil
	if m_ply2 == M_EPLT2.MJ then
		players = m_data.playerMes
		key1 = "zongJifen"
	end
	if m_ply2 == M_EPLT2.WSK then
		players = m_data.players
		key1 = "totalScore"
	end

	for i = 1,4 do
		if players[i][key1] > maxScore then
			maxScore = players[i][key1]
		end
	end

	local indexs = LogicUtil.GetSortIndex(players)
	for i=1,4 do
		_s.InitPlayerResultInfo(m_luaWindowRoot:GetTrans("player_" .. i), players[indexs[i]], i, maxScore)
	end
end

function total_settlement_ui_window.InitGameResultInfo()
	-- data
	local id,time,name,doc = "","","",""
	if m_ply2 == M_EPLT2.MJ then
		id,time,name,doc = m_data.roomId, m_data.settlementTime, m_data.roomName, m_data.playDes
	end
	if m_ply2 == M_EPLT2.WSK then
		id,time,name,doc = m_data.id, m_data.time, m_data.name, m_data.doc
	end

	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("room_id"),id)
	if m_ply2 == M_EPLT2.WSK then
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("time"),time)
	elseif m_ply2 == M_EPLT2.MJ then
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("time"),time)
	end
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("game_name"),name)
	local describe = string.gsub(doc, ",", "\n")
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("info_1"), describe)
end

function total_settlement_ui_window.InitPlayerResultInfo(root, data, index,maxScore)
	if not data then
		m_luaWindowRoot:SetActive(root, false)
		return
	end

	-- data
	local nickName, userId, isFangZhu, scores, totalScore = "",1,true,nil,0
	if m_ply2 == M_EPLT2.WSK then
		nickName, userId, isFangZhu, scores, totalScore = data.nickName, data.userId, data.isFangZhu, data.scores, data.totalScore
	end
	if m_ply2 == M_EPLT2.MJ then
		nickName, userId, isFangZhu, scores, totalScore = data.nickName, data.userId, data.fangZhu, data.eachGameJifen, data.zongJifen
	end

	nickName = utf8sub(nickName,1,10)

	local player = PlayGameSys.GetPlayLogic().roomObj.playerMgr:GetPlayerByPlayerID(data.userId)
	if player then
		WindowUtil.LoadHeadIcon(m_luaWindowRoot,m_luaWindowRoot:GetTrans(root, "head_ico"), player.seatInfo.headUrl, player.seatInfo.sex, false,RessStorgeType.RST_Never)
	end

	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "name"),nickName)
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "player_id"),"ID:" .. userId)
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(root, "host"),isFangZhu)
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(root, "king"),totalScore == maxScore and maxScore > 0)

	if totalScore > 0 then
		m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans(root, "bg_panel"), "win_panel", true)
		m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans(root, "total_point"), "win_point", true)
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "win_point"),totalScore)
	else
		m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans(root, "bg_panel"), "lose_panel", true)
		m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans(root, "total_point"), "lose_point", true)
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "lose_point"),totalScore)
	end

	_s.InitScrollView(root, scores, index, totalScore >= 0)
end

function total_settlement_ui_window.InitScrollView(root, data, index, is_winner)
	-- body
	if not data then
		m_luaWindowRoot:SetActive(root, false)
		return
	end

	if not _s.m_panel_scrollView then
		_s.m_panel_scrollView = {}
	end
	if not _s.m_initFuncSeq then
		_s.m_initFuncSeq = {}
	end

	local timeCount = #data
	if _s.m_panel_scrollView[index] == nil then
		local rootTrans = m_luaWindowRoot:GetTrans(root, "record_scrollview")
		_s.m_panel_scrollView[index] = EZfunLimitScrollView.GetOrAddLimitScr(rootTrans)
		local item = m_luaWindowRoot:GetTrans("record").gameObject
		_s.m_panel_scrollView[index]:InitForLua(rootTrans, item, UnityEngine.Vector2.New(5, 6), UnityEngine.Vector2.New(1, 8), LimitScrollViewDirection.SVD_Vertical, false)

		local initFunc = function (trans, item_index)
			_s.InitTrans(trans, item_index + 1, data[item_index + 1], is_winner)
		end
		_s.m_initFuncSeq[index] = LuaCsharpFuncSys.RegisterFunc(initFunc)

		_s.m_panel_scrollView[index]:SetInitItemCallLua(_s.m_initFuncSeq[index])
	end
	_s.m_panel_scrollView[index]:InitItemCount(timeCount, true)
end

-- 显示每条比分
function total_settlement_ui_window.InitTrans(root, index, data, is_winner)
	m_luaWindowRoot:SetActive(root, true)

	local trans
	m_luaWindowRoot:ShowChild(root, is_winner and "win" or "lose", true)
	if is_winner then
		trans = m_luaWindowRoot:GetTrans(root, "win")
	else
		trans = m_luaWindowRoot:GetTrans(root, "lose")
	end

	if m_ply2 == M_EPLT2.WSK then
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "point"), data)
	end
	if m_ply2 == M_EPLT2.MJ then
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "point"), data.jifen)
	end

	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "round_id"), index)
end

function total_settlement_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name
	if click_name == "share_btn" then
		_s.ShareTotalSettlement(false)
	elseif click_name == "close_btn" or click_name == "back_btn" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		_s.InitWindow(false, 0)
		PlayGameSys.QuitToMainCity()
	-- elseif click_name == "share_weixin" then
	--  _s.ShareTotalSettlement(false)
	-- elseif click_name == "share_circle" then
	--  _s.ShareTotalSettlement(true)
	end
end

--显示隐藏按钮
function total_settlement_ui_window.SetShareVisable(show)
	--if m_luaWindowRoot then
		--m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("share_weixin"), show)
		--m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("share_circle"), show)
	--end
end

--截屏完后 恢复按钮
function total_settlement_ui_window.ScreenShotEnd()
	--_s.SetShareVisable(true)
	_s.ShowShareWindow(false)

end

function total_settlement_ui_window.ShareTotalSettlement(isWeiXinCircle)
	if m_TimeValue.Value > 0 then
		WindowUtil.LuaShowTips("您点击太快,请稍后再试")
		return
	end
	m_TimeValue.Value = 5

	_s.ShowShareWindow(true)
	-- _s.SetShareVisable(false)

	HallSys.ShareTotalSettlement(isWeiXinCircle)
end

--是否显示分享界面
function total_settlement_ui_window.ShowShareWindow(show)
	if m_luaWindowRoot then
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("ShareBtns"),not show)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("ShareContent"),show)
		_s.SetShareVisable(show)
	end
	--_s.TransformWindow(show)
end

-- flag为true则缩小并往上移动32，用来分享
function total_settlement_ui_window.TransformWindow(flag)
	local trans_node = m_luaWindowRoot:GetTrans("trans_node")
	if flag then
		trans_node.localPosition = UnityEngine.Vector3.New(0, 32, 0)
		trans_node.localScale = UnityEngine.Vector3.New(0.85, 0.85, 1)
	else
		trans_node.localPosition = UnityEngine.Vector3.zero
		trans_node.localScale = UnityEngine.Vector3.one
	end
end

function total_settlement_ui_window.UnRegister()
    m_luaWindowRoot = nil
	LuaEvent.RemoveHandle(EEventType.ScreenShotEnd,total_settlement_ui_window.ScreenShotEnd, self)
end

function total_settlement_ui_window.OnDestroy()
	if _s.m_initFuncSeq then
		for i=1,#_s.m_initFuncSeq do
			LuaCsharpFuncSys.UnRegisterFunc(_s.m_initFuncSeq[i])
		end
	end

	_s.UnRegister()
	_s.m_initFuncSeq = nil
	_s.m_panel_scrollView = nil
	m_TimeValue = nil
end
