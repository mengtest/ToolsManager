--------------------------------------------------------------------------------
--   File      : joinRoom_ui_window.lua
--   author    : jianing
--   function  : 加入房间
--   date      : 2017-10-22
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

joinRoom_ui_window = {

}

local _s = joinRoom_ui_window

local m_luaWindowRoot
local m_state

local m_labelNumTrans = {}

local label_roomId = 0

function joinRoom_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function joinRoom_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		m_state = state
		_s.InitWindowDetail()
	end
end

--加入房间
local function LocalJoinRoom()
	HallSys.JoinRoom(label_roomId, nil,nil)
end

--刷新上面的数字显示
local function RefreshShowRoomNum()
	local showTable = {}
	for i=6,1,-1 do
		--取位置上的数
		local n = math.pow(10,i)
		local num = math.floor((label_roomId - math.floor( label_roomId / n ) * n ) * 10 / n)

		if num ~= 0 or label_roomId >= n then
			table.insert(showTable,num)
		end
	end

	for i=6,1,-1 do
		local num = showTable[7-i]
		if num == nil then
			num = ""
		end
		m_luaWindowRoot:SetLabel(m_labelNumTrans[i],num)
	end
end

function joinRoom_ui_window.CreateWindow()
	m_labelNumTrans = {}
	local showContent = m_luaWindowRoot:GetTrans("showContent")
	for i=1,6 do
		table.insert(m_labelNumTrans, m_luaWindowRoot:GetTrans(showContent,"labelNum_"..i))
	end
end

function joinRoom_ui_window.InitWindowDetail()
	label_roomId = 0
	RefreshShowRoomNum()
end



function joinRoom_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name

	if click_name == "btnClose" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, false, 1, "joinRoom_ui_window", false , nil)
	elseif click_name == "btnClear" then
		WrapSys.AudioSys_PlayEffect("Common/UI/uiclick")
		label_roomId = 0
		RefreshShowRoomNum()
	elseif click_name == "btnDelect" then
		WrapSys.AudioSys_PlayEffect("Common/UI/uiclick")
		label_roomId =  math.floor( label_roomId / 10 )
		RefreshShowRoomNum()

	elseif string.find(click_name, "btnNum_") then
		WrapSys.AudioSys_PlayEffect("Common/UI/uiclick")
		if label_roomId < 100000 then
			local num = tonumber(string.sub(click_name, string.len("btnNum_") + 1))
			label_roomId = label_roomId * 10 + num
			RefreshShowRoomNum()

			if(label_roomId > 100000) then
				LocalJoinRoom()
			end
		end


	end
end

function joinRoom_ui_window.UnRegister()

end

function joinRoom_ui_window.OnDestroy()
	_s.UnRegister()

	m_labelNumTrans = {}
    label_roomId = 0
    m_luaWindowRoot = nil
end