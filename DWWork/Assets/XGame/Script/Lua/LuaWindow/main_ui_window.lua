require "Logic.SystemLogic.WSK.WSKNormalPlayCardLogic"

local trans = {}

local m_luaWindowRoot
local m_state
local m_labelIDTrans
local m_labelRoomIDTrans
local m_isSelectJoin = true
local m_isSelectReconnect = false

main_ui_window = {

}
local _s = main_ui_window



function main_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function main_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		m_state = state
		_s.InitWindowDetail()
	end
end

function main_ui_window.CreateWindow()
	m_luaWindowRoot:TopBarCreateWindow()
	m_labelIDTrans = m_luaWindowRoot:GetTrans("label_ID")
	m_labelRoomIDTrans = m_luaWindowRoot:GetTrans("label_RoomID")
end

function main_ui_window.InitWindowDetail()
	_s.InitTestNodeStatus()
end

function main_ui_window.InitTestNodeStatus()
	m_luaWindowRoot:SetLabel(m_labelIDTrans,DataManager.GetUserID())
	m_luaWindowRoot:SetLabel(m_labelRoomIDTrans,DataManager.GetRoomID())
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("ico_select_createRoom"),m_isSelectJoin)
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("ico_select_isReconnect"),m_isSelectReconnect)
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("input_RoomID"),m_isSelectJoin)
end

function main_ui_window.HandleWidgetClick(gb)
	WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
	local click_name = gb.name
	if click_name == "btn_return" then
		LoginSys.LoginOut()
	elseif click_name == "btn_select_createRoom" then
		m_isSelectJoin = not m_isSelectJoin
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("ico_select_createRoom"),m_isSelectJoin)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("input_RoomID"),m_isSelectJoin)
	elseif click_name == "btn_select_isReconnect" then
		m_isSelectReconnect = not m_isSelectReconnect
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("ico_select_isReconnect"),m_isSelectReconnect)
	elseif click_name == "btn_test" then
		local srcCardIDs = {3,3,3,3,4,4,4,4,5,5,5,5,6,6,6,6,7,7,7,7,8,8,9,9,10}
		local nextCardIDs = {4,4,4,4,5,5,5,5,6,6,6,6,7,7,7,7,8,8,8,8,9,9,10,10,11}
		local handCardCount = 26
		local srcCards = {}
		local nextCards = {}
		local cardLogicContainer = CRWSKCardPlayLogicContainer.New()
		local cardItem

		for k,v in pairs(srcCardIDs) do
			cardItem = CCard.New()
			cardItem:Init(v,k)
			table.insert(srcCards,cardItem)
		end

		for k,v in pairs(nextCardIDs) do
			cardItem = CCard.New()
			cardItem:Init(v,k)
			table.insert(nextCards,cardItem)
		end

		if cardLogicContainer:CompareCard(srcCards,nextCards,handCardCount) then
			print("CompareCard true")
		end

		if cardLogicContainer:CheckCanOutCard(srcCards,handCardCount) then
			print("CompareCard true")
		end

		--test
		-- PlayRecordSys.SendRecordDetailRoundReplayInfoReq(6522)
		-- return

		-- local id = WrapSys.GetUILabelText(m_labelIDTrans)
		-- print(id)
		-- if id ~= "" then
		-- 	DataManager.SetUserID(tonumber(id))
		-- end
		-- local roomID = WrapSys.GetUILabelText(m_labelRoomIDTrans)
		-- print(roomID)
		-- if roomID ~= "" then
		-- 	PlayGameSys.testRoomID = tonumber(roomID)
		-- 	DataManager.SetRoomID(PlayGameSys.testRoomID)
		-- end
		-- DataManager.SetToken("robot")
		-- PlayGameSys.testIsCreate = not m_isSelectJoin
		-- if PlayGameSys.testIsCreate then
		-- 	roomID = 0
		-- end
		-- PlayGameSys.testIsReconnect = m_isSelectReconnect
		-- PlayGameSys.StartGame(10001,"192.168.2.160",7760, PlayGameSys.testIsCreate and 0 or PlayGameSys.testRoomID,m_isSelectReconnect)
		--PlayGameSys.StartGame(10001,"192.168.2.245",7760, PlayGameSys.testIsCreate and 0 or PlayGameSys.testRoomID,m_isSelectReconnect)
	end 
end

function main_ui_window.UnRegister()
    m_luaWindowRoot = nil
end

function main_ui_window.OnDestroy()
    _s.UnRegister()
end