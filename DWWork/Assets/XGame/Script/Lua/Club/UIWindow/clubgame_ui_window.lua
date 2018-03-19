--------------------------------------------------------------------------------
--   File	  : clubgame_ui_window.lua
--   author	: zx
--   function  : 俱乐部玩法管理
--   date	  : 2018-01-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "LuaWindow.Module.UINetTipsCtrl"
require "LuaSys.CreateRoomConfig"
require "3rd.MathBit"


clubgame_ui_window = {

}

local _s = clubgame_ui_window

local m_luaWindowRoot
local m_state
local m_clubGameData
local m_open = false
local m_netTipsCtrl = nil

function clubgame_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function clubgame_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	m_open = open
	if open then
		m_state = state
		_s.InitWindowDetail()
	end
end

-- 反编译协议
local function decodeProtoToConfigString(playID, tpl)
	ProtoManager.InitProtoByPlayID(playID)
	local config = ProtoManager.Decode(GAME_CMD.CS_CREATE_ROOM, tpl)
	if nil == config then
		DwDebug.LogError("decodeProtoToConfigString fail", playID)
		return
	end

	local toggleConfig = CreateRoomConfig.GameToggleConfig[playID]
	if nil == toggleConfig then
		DwDebug.LogError("decodeProtoToConfigString fail", playID)
		return
	end

	local createConfig = CreateRoomConfig.GameCreateConfig[playID]
	if nil == createConfig then
		DwDebug.LogError("decodeProtoToConfigString fail", playID)
		return
	end

	-- 对调key, value
	local nodeConfig = {}
	for k,v in pairs(createConfig) do
		nodeConfig[v] = k
	end

	local des = ""
	for k,v in pairs(config) do
		if nil ~= nodeConfig[k] then
			local toggle = toggleConfig[nodeConfig[k]]
			for i,node in ipairs(toggle) do
				if node.multiple then
					-- 位运算
					if MathBit.andOp(node.value, v) > 0 then
						des = des == "" and node.label or (des .. "，" .. node.label)
					-- if v >= node.value then
					-- 	des = des == "" and node.label or (des .. "，" .. node.label)
					-- 	-- v = v - node.value
					-- else
					-- 	break
					end
				else
					if v == node.value then
						des = des == "" and node.label or (des .. "，" .. node.label)
						break
					end
				end
			end
		end
	end

	return des
end

-- 显示玩法信息
function clubgame_ui_window.InitGameInfo(trans, info)
	if nil == info then
		m_luaWindowRoot:SetActive(trans, false)
	else
		local playID = info.play_id

		local playText = Common_PlayText[playID]
		if nil ~= playText then
			m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "name"), playText.name, false)
		end

		local des = ""
		if nil == info.description or "" == info.description then
			-- 服务器没有返回数据，客户端字节使用tpl解析
			des = decodeProtoToConfigString(playID, info.tpl)
		else
			des = info.description
		end
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "des"), des, false)

		m_luaWindowRoot:SetActive(trans, true)
	end
end

-- 显示添加玩法
function clubgame_ui_window.InitAddGame(count, max)
	local addGame = m_luaWindowRoot:GetTrans("btnAdd")
	if max == count then
		m_luaWindowRoot:SetActive(addGame, false)
	else
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("gameCount"), string.format("%d/%d", count, max))
		m_luaWindowRoot:SetActive(addGame, true)
	end
end

-- 刷新界面数据
function clubgame_ui_window.RefreshPanel(count, max)
	if nil == m_luaWindowRoot then
		return
	end

	-- DwDebug.LogError("m_clubGameData:", m_clubGameData)
	-- 界面上只能显示3个，如果有多个数据说明数据出错
	if 3 == count and 3 == #m_clubGameData then
		-- 将第一个数据移到最后一个
		table.insert(m_clubGameData, table.remove(m_clubGameData, 1))
	end

	for i=1,3 do
		_s.InitGameInfo(m_luaWindowRoot:GetTrans("game" .. i), m_clubGameData[i])
	end
	_s.InitAddGame(count, max)
end

-- 请求裂变数据
function clubgame_ui_window.GetGameList(...)
	if not m_netTipsCtrl then
		m_netTipsCtrl = UINetTipsCtrl.New()
		m_netTipsCtrl:Init(
			m_luaWindowRoot,
			m_luaWindowRoot:GetTrans("animation_ui_root"),
			function()
				WebNetHelper.RequestTemplateList(m_state, function (body, head)
					if not m_open then
						return
					end
					if 0 == body.errcode then
						m_clubGameData = body.data.list
						if nil ~= m_luaWindowRoot then
							_s.RefreshPanel(body.data.now_tpl_num, body.data.max_tpl_num)
						end
						m_netTipsCtrl:StopWork(true)
					else
						WindowUtil.LuaShowTips(body.errmsg)
						m_netTipsCtrl:StopWork(false)
					end
				end,
				function (body, head)
					if nil ~= body and 0 ~= body.errcode and "" ~= body.errmsg then
						WindowUtil.LuaShowTips(body.errmsg)
					end
					m_netTipsCtrl:StopWork(false)
				end)
			end
		)
	end


	m_netTipsCtrl:StartWork()
end

function clubgame_ui_window.InitWindowDetail()
	-- 初始化清空数据
	m_clubGameData = {}
	_s.RefreshPanel(0, 3)

	-- 获取列表
	_s.GetGameList()
end

-- 获取配置id
local function getClubGameIDByIndex(index)
	if nil ~= m_clubGameData and m_clubGameData[index] then
		return m_clubGameData[index].id or 0
	end

	return 0
end

-- 获取玩法id
-- local function getClubGamePlayIDByIndex(index)
-- 	if nil ~= m_clubGameData and m_clubGameData[index] then
-- 		return m_clubGameData[index].play_id or 0
-- 	end

-- 	return 0
-- end

-- 获取数据
local function getClubGameDataByIndex(index)
	-- 填充俱乐部ID
	local data = {}
	data.clubID = m_state

	if nil ~= m_clubGameData and m_clubGameData[index] then
		data.id = m_clubGameData[index].id
		data.play_id = m_clubGameData[index].play_id
	end

	return data
end

-- 删除玩法
local function deleteGame(id)
	if nil == id then
		return
	end

	WindowUtil.ShowErrorWindow(4, "\n确认删除玩法？", nil, function()
		LuaEvent.AddEventNow(EEventType.ShowWaitWindow, true)
		WebNetHelper.RequestDeleteTemplate(m_state, id, function (body, head)
			LuaEvent.AddEventNow(EEventType.ShowWaitWindow, false)
			if not m_open then
				return
			end
			if 0 == body.errcode then
				if nil ~= m_luaWindowRoot then
					_s.GetGameList()
				end
			else
				WindowUtil.LuaShowTips(body.errmsg)
			end

			LuaEvent.AddEventNow(EEventType.UI_RefreshClubList)
		end,
		function (body, head)
			LuaEvent.AddEventNow(EEventType.ShowWaitWindow, false)
			if nil ~= body and 0 ~= body.errcode and "" ~= body.errmsg then
				WindowUtil.LuaShowTips(body.errmsg)
			else
				WindowUtil.LuaShowTips("删除玩法超时，请稍后重试！")
			end
		end)
	end, nil, "")
end

-- 点击事件处理
function clubgame_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name

	if click_name == "btnAdd" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "Club.UIWindow.clubcreateroom_ui_window", false , getClubGameDataByIndex(nil))

	elseif click_name == "btnModfiy" then
		local index = tonumber(string.sub(gb.transform.parent.name, string.len("game") + 1))
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 2, "Club.UIWindow.clubcreateroom_ui_window", false , getClubGameDataByIndex(index))

	elseif click_name == "btnDelete" then
		local index = tonumber(string.sub(gb.transform.parent.name, string.len("game") + 1))
		deleteGame(getClubGameIDByIndex(index))

	elseif click_name == "btnClose" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		_s.InitWindow(false, 0)
	else
		if m_netTipsCtrl then
			m_netTipsCtrl:HandleWidgetClick(gb)
		end
	end
end

-- 解散或退出，关闭界面
function clubgame_ui_window.ClubQuieOrDisBand(eventID, clubID)
	if m_state == clubID then
		_s.InitWindow(false, 0)
	end
end

function clubgame_ui_window.RegisterEventHandle()
	LuaEvent.AddHandle(EEventType.ClubQuieOrDisBand, _s.ClubQuieOrDisBand)
	LuaEvent.AddHandle(EEventType.UI_RefreshClubGameList, _s.GetGameList)
end

function clubgame_ui_window.UnRegisterEventHandle()
	LuaEvent.RemoveHandle(EEventType.ClubQuieOrDisBand, _s.ClubQuieOrDisBand)
	LuaEvent.RemoveHandle(EEventType.UI_RefreshClubGameList, _s.GetGameList)
end

function clubgame_ui_window.CreateWindow()
	_s.RegisterEventHandle()
end

function clubgame_ui_window.OnDestroy()
	_s.UnRegisterEventHandle()

	m_clubGameData = {}

	if m_netTipsCtrl then
		m_netTipsCtrl:Destroy()
		m_netTipsCtrl = nil
	end

	m_luaWindowRoot = nil
end
