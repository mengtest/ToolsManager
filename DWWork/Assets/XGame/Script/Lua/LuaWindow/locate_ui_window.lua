--------------------------------------------------------------------------------
-- 	 File      : locate_ui_window.lua
--   author    : guoliang
--   function   : 回放主UI
--   date      : 2017-10-25
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------


local m_luaWindowRoot
local m_state

local m_southNodeTrans
local m_eastNodeTrans
local m_northNodeTrans
local m_westNodeTrans

local m_labelSNTrans
local m_labelSETrans
local m_labelSWTrans
local m_labelENTrans
local m_labelEWTrans
local m_labelWNTrans

locate_ui_window = {
}
local _s = locate_ui_window
local GameType = 
{
	POKE_510K = 1,
	MJ_CongRen = 2,
}


function locate_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function locate_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		m_state = state
		_s.InitWindowDetail()
	end
end

function locate_ui_window.CreateWindow()
	m_luaWindowRoot:TopBarCreateWindow()

	m_southNodeTrans = m_luaWindowRoot:GetTrans("south_node")
	m_eastNodeTrans = m_luaWindowRoot:GetTrans("east_node")
	m_northNodeTrans = m_luaWindowRoot:GetTrans("north_node")
	m_westNodeTrans = m_luaWindowRoot:GetTrans("west_node")

	m_labelSNTrans = m_luaWindowRoot:GetTrans("label_distance_s_n")
	m_labelSETrans = m_luaWindowRoot:GetTrans("label_distance_s_e")
	m_labelSWTrans = m_luaWindowRoot:GetTrans("label_distance_s_w")
	m_labelENTrans = m_luaWindowRoot:GetTrans("label_distance_e_n")
	m_labelEWTrans = m_luaWindowRoot:GetTrans("label_distance_e_w")
	m_labelWNTrans = m_luaWindowRoot:GetTrans("label_distance_n_w")
end



function locate_ui_window.InitWindowDetail()
	local room = PlayGameSys.GetPlayLogic().roomObj
	local player

	_s.ResetWindow()

	for i = 1,4 do
		local trans
		player = room.playerMgr:GetPlayerBySeatID(i-1)
		if player then
			trans = _s.GetSeatTransByPos(player.seatPos)
			_s.InitHeadItem(trans,player.seatInfo)
		end
	end
	--重置距离显示
	_s.InitDistance(nil)
	--显示在线玩家
	_s.InitDistance(room.PBGeoRsp)
end

function locate_ui_window.GetSeatTransByPos(seatPos)
	if seatPos == SeatPosEnum.South then
		return m_southNodeTrans
	elseif seatPos == SeatPosEnum.East then
		return m_eastNodeTrans
	elseif seatPos == SeatPosEnum.North then
		return m_northNodeTrans
	else
		return m_westNodeTrans
	end
end

function locate_ui_window.ResetWindow()
	_s.InitHeadItem(m_northNodeTrans)
	_s.InitHeadItem(m_southNodeTrans)
	_s.InitHeadItem(m_westNodeTrans)
	_s.InitHeadItem(m_eastNodeTrans)
end

function locate_ui_window.InitHeadItem(trans,seatInfo)
	local icoHeadTrans = m_luaWindowRoot:GetTrans(trans,"ico_head")
	local nameTrans = m_luaWindowRoot:GetTrans(trans,"label_name")
	local locateNameTrans = m_luaWindowRoot:GetTrans(trans,"label_locate_name")
	if seatInfo then
		-- 头像ico
		m_luaWindowRoot:SetActive(icoHeadTrans,true)
		WindowUtil.LoadHeadIcon(m_luaWindowRoot, icoHeadTrans, seatInfo.headUrl, seatInfo.sex, false, RessStorgeType.RST_Never)

		m_luaWindowRoot:SetActive(nameTrans, true)
		m_luaWindowRoot:SetLabel(nameTrans, seatInfo.nickName or "")
		m_luaWindowRoot:SetActive(locateNameTrans, true)
		local address = nil
		if (nil == seatInfo.loginAddress or "" == seatInfo.loginAddress) and 
			(nil == seatInfo.LoginAddress or "" == seatInfo.LoginAddress) then
			address = "未知"
		else
			-- 文本截断
			address = seatInfo.loginAddress or seatInfo.LoginAddress
			local len = utf8len(address)
			if len > 6 then
				address = utf8sub(address, 1, 6 * 2) .. "..."
			end
		end
		m_luaWindowRoot:SetLabel(locateNameTrans, address)
	else
		m_luaWindowRoot:SetActive(icoHeadTrans, false)
		m_luaWindowRoot:SetActive(nameTrans, false)
		m_luaWindowRoot:SetActive(locateNameTrans, false)
	end
end

-- 使用统一的外部接口
-- lsbInfo -> distance -> list<seatIdA, seatIdB, distance>
function locate_ui_window.InitDistance(lsbInfo)
	if lsbInfo then
		local room = PlayGameSys.GetPlayLogic().roomObj
		local Aplayer
		local Bplayer
		local posKey

		for key, PBGeoItem in pairs(lsbInfo.distance) do
			-- 服务器发过来的两个id含义不一样，暂时没有在设置函数里面转换为相同的含义
			if nil ~= PBGeoItem.seatIdA and nil ~= PBGeoItem.seatIdB then
				Aplayer = room.playerMgr:GetPlayerBySeatID(PBGeoItem.seatIdA)
				Bplayer = room.playerMgr:GetPlayerBySeatID(PBGeoItem.seatIdB)
			elseif nil ~= PBGeoItem.uIdA and nil ~= PBGeoItem.uIdB then
				Aplayer = room.playerMgr:GetPlayerByPlayerID(PBGeoItem.uIdA)
				Bplayer = room.playerMgr:GetPlayerByPlayerID(PBGeoItem.uIdB)
			end

			if Aplayer and Bplayer then
				posKey = Aplayer.seatPos .."_".. Bplayer.seatPos
			end

			if posKey == "South_North" or posKey == "North_South" then
				m_luaWindowRoot:SetActive(m_labelSNTrans,true)
				m_luaWindowRoot:SetLabel(m_labelSNTrans,PBGeoItem.distance)
			elseif posKey == "South_East" or posKey == "East_South" then
				m_luaWindowRoot:SetActive(m_labelSETrans,true)
				m_luaWindowRoot:SetLabel(m_labelSETrans,PBGeoItem.distance)
			elseif posKey == "South_West" or posKey == "West_South" then
				m_luaWindowRoot:SetActive(m_labelSWTrans,true)
				m_luaWindowRoot:SetLabel(m_labelSWTrans,PBGeoItem.distance)
			elseif posKey == "East_North" or posKey == "North_East" then
				m_luaWindowRoot:SetActive(m_labelENTrans,true)
				m_luaWindowRoot:SetLabel(m_labelENTrans,PBGeoItem.distance)
			elseif posKey == "East_West" or posKey == "West_East" then
				m_luaWindowRoot:SetActive(m_labelEWTrans,true)
				m_luaWindowRoot:SetLabel(m_labelEWTrans,PBGeoItem.distance)
			else
				m_luaWindowRoot:SetActive(m_labelWNTrans,true)
				m_luaWindowRoot:SetLabel(m_labelWNTrans,PBGeoItem.distance)
			end
		end 
	else
		m_luaWindowRoot:SetActive(m_labelSNTrans,false)
		m_luaWindowRoot:SetActive(m_labelSETrans,false)
		m_luaWindowRoot:SetActive(m_labelSWTrans,false)
		m_luaWindowRoot:SetActive(m_labelENTrans,false)
		m_luaWindowRoot:SetActive(m_labelEWTrans,false)
		m_luaWindowRoot:SetActive(m_labelWNTrans,false)
	end
end

function locate_ui_window.HandleWidgetClick(gb)
	WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
	local click_name = gb.name
	if click_name == "btnClose" then
		_s.InitWindow(false,0)
	end
end

function locate_ui_window.UnRegister()
    m_luaWindowRoot = nil
end

function locate_ui_window.OnDestroy()
    
end