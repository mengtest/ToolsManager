--------------------------------------------------------------------------------
--   File      : disband_room_ui_window.lua
--   author    : guoliang
--   function   : 解散出房间窗口
--   date      : 2017-10-9
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

disband_room_ui_window = {}
local _s = disband_room_ui_window
_s.isClicked = false
_s.dismissInfo = nil -- 服务端下发解散消息
_s.isResigterUpdateSec = false
_s.seat_index_map = {}

local m_luaWindowRoot
local m_state
local m_cardPlayLogic
local m_countTime = 60

local m_labelDisbandInfoTrans
local m_labelDescribeTrans
local m_labelSelfVoteInfoTrans
local m_labelWaitTimeCountTrans
local m_labelSelfTimeCountTrans
local m_bottomRootTrans


function disband_room_ui_window.Init(luaWindowRoot)
	m_luaWindowRoot = luaWindowRoot
end

function disband_room_ui_window.PreCreateWindow()

end

function disband_room_ui_window.CreateWindow()
	m_labelDisbandInfoTrans = m_luaWindowRoot:GetTrans("label_disband_info")
	m_labelDescribeTrans = m_luaWindowRoot:GetTrans("label_describe")
	m_labelSelfVoteInfoTrans = m_luaWindowRoot:GetTrans("label_self_vote_info")
	m_labelWaitTimeCountTrans = m_luaWindowRoot:GetTrans("label_wait_time_count")
	m_labelSelfTimeCountTrans = m_luaWindowRoot:GetTrans("label_self_time_count")
	m_bottomRootTrans = m_luaWindowRoot:GetTrans("bottom_root")
	_s.RegisterEventHandle()
end

function disband_room_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, -1, false)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	_s.m_open = open
	_s.m_state = state
	if open then
		if not _s.isResigterUpdateSec then
			_s.isResigterUpdateSec = true
			UpdateSecond:Add(_s.UpdateSecond, nil)
		end
		_s.InitWindowDetail()
	else
		_s.isResigterUpdateSec = false
		UpdateSecond:Remove(_s.UpdateSecond, nil)
	end
end

function disband_room_ui_window.OnDestroy()
	m_luaWindowRoot = nil
	_s.UnRegisterEventHandle()
end

function disband_room_ui_window.RegisterEventHandle()
	LuaEvent.AddHandle(EEventType.NetReconnectSuccess,_s.NetReconnectSuccess,nil)
	LuaEvent.AddHandle(EEventType.UI_DisbandRoom_VoteRefresh,_s.PlayVoteRefresh,nil)
end

function disband_room_ui_window.UnRegisterEventHandle()
	LuaEvent.RemoveHandle(EEventType.NetReconnectSuccess,_s.NetReconnectSuccess,nil)
	LuaEvent.RemoveHandle(EEventType.UI_DisbandRoom_VoteRefresh,_s.PlayVoteRefresh,nil)
end

function disband_room_ui_window.PlayVoteRefresh(eventId,p1,p2)
	local rsp = p1
	if rsp then
		local player = PlayGameSys.GetPlayLogic().roomObj.playerMgr:GetPlayerByPlayerID(rsp.userId)
		if player then
			_s.InitHeadItem(player.seatInfo.seatId,rsp.actionValue)
		end
	end
end

function disband_room_ui_window.InitWindowDetail()
	m_cardPlayLogic = PlayGameSys.GetPlayLogic()
	_s.dismissInfo = m_cardPlayLogic.roomObj.dismissInfo

	m_countTime = _s.dismissInfo.remainderTime
	_s.InitDisbandInfoTitle()
	--是否自己发起解散
	local isSelfActive = (_s.dismissInfo.applyUserId == DataManager.GetUserID())
	_s.isClicked = isSelfActive
	_s.SelfOperateRefersh(not isSelfActive)

	--设置倒计时
	m_luaWindowRoot:SetLabel(m_labelWaitTimeCountTrans,m_countTime)
	m_luaWindowRoot:SetLabel(m_labelSelfTimeCountTrans,m_countTime)

	_s.InitHead()
end

function disband_room_ui_window.SelfOperateRefersh(isShow)
	m_luaWindowRoot:SetActive(m_bottomRootTrans,isShow)
	m_luaWindowRoot:SetActive(m_labelSelfVoteInfoTrans,not isShow)
	if not isShow then
		m_luaWindowRoot:SetLabel(m_labelWaitTimeCountTrans,m_countTime)
	end
end

function disband_room_ui_window.InitHead()
	local index = 0
	local t = _s.dismissInfo.playerVote
	local playerMgr = m_cardPlayLogic.roomObj.playerMgr
	for i = 1, #t do
        local _item = t[i]
        if next(_item) then
            _item.seatSort = SeatPosSort[playerMgr:GetPlayerByPlayerID(_item.userId).seatPos]
			_item.nickName = utf8sub(_item.nickName, 1, 10)
        else
        	_item.seatSort = -1
        end
    end
	table.sort(t, function(a, b)
		if a.seatSort == nil and b.seatSort then
 			return true
		elseif a.seatSort and b.seatSort == nil then
			return false
		elseif a.seatSort == nil and b.seatSort == nil then
			return true
		elseif a.seatSort == b.seatSort then
			return false
		else
		 	return a.seatSort < b.seatSort 		
		end

	 end)
	-- DwDebug.Log("排序测试 ")
	for k,v in ipairs(_s.dismissInfo.playerVote) do
		if next(v) ~= nil then
			local player = playerMgr:GetPlayerByPlayerID(v.userId)
			if player then
				_s.seat_index_map[player.seatInfo.seatId] = index
				index = index + 1
				if m_cardPlayLogic:GetType() == PlayLogicTypeEnum.WSK_Normal or 
					m_cardPlayLogic:GetType() == PlayLogicTypeEnum.ThirtyTwo_Normal or
					m_cardPlayLogic:GetType() == PlayLogicTypeEnum.DDZ_Normal then
					_s.InitHeadItem(player.seatInfo.seatId,v.vote)
					if v.vote > 0 and v.userId == DataManager.GetUserID() then -- 自己是否投票
						_s.SelfOperateRefersh(false)
					end
				else
					_s.InitHeadItem(player.seatInfo.seatId,v.voteEnd)
					if v.voteEnd > 0 and v.userId == DataManager.GetUserID() then -- 自己是否投票
						_s.SelfOperateRefersh(false)
					end
				end
			end
		end
	end
	for i=3,index, -1 do
		local headItemTrans = m_luaWindowRoot:GetTrans("headItem_"..i)
		m_luaWindowRoot:SetActive(headItemTrans, false)
	end
end

function disband_room_ui_window.InitHeadItem(seatID,voteNum)
	if not _s.seat_index_map[seatID] then
		return
	end
	local headItemTrans = m_luaWindowRoot:GetTrans("headItem_".._s.seat_index_map[seatID])
	m_luaWindowRoot:SetActive(headItemTrans, true)
	local icoTrans = m_luaWindowRoot:GetTrans(headItemTrans,"ico_agree")
	if voteNum > 0 then
		m_luaWindowRoot:SetActive(icoTrans,true)
		if voteNum == 1 then
			icoSpriteName = "disbandRoomLayer_icon_agree"
		else
			icoSpriteName = "disbandRoomLayer_icon_reject"
		end
		m_luaWindowRoot:SetSprite(icoTrans,icoSpriteName)
	else
		m_luaWindowRoot:SetActive(icoTrans,false)
	end
	local player = m_cardPlayLogic.roomObj.playerMgr:GetPlayerBySeatID(seatID)
	if player then
		WindowUtil.LoadHeadIcon(m_luaWindowRoot,m_luaWindowRoot:GetTrans(headItemTrans,"ico_head"), player.seatInfo.headUrl, player.seatInfo.sex,false,RessStorgeType.RST_Never)
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(headItemTrans,"label_name"),utf8sub(player.seatInfo.nickName,1,10))
	end
end


function disband_room_ui_window.InitDisbandInfoTitle()
	local disbandInfoStr = _s.dismissInfo.applyNickName .." ".. "发起投票解散房间"
	m_luaWindowRoot:SetLabel(m_labelDisbandInfoTrans,disbandInfoStr)
end

function disband_room_ui_window.NetReconnectSuccess()
	if not _s.isClicked then
		_s.SelfOperateRefersh(true)
	end
end

function disband_room_ui_window.HandleWidgetClick(gb)
	WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
	local clickName = gb.name
	if clickName == "btn_no" then
		_s.HandleNoClick()
	elseif clickName == "btn_ok" then
		_s.HandleOKClick()
	end
end

function disband_room_ui_window.HandleOKClick()
	_s.isClicked = true
	_s.SelfOperateRefersh(false)
	m_cardPlayLogic:SendDismissVote(true)
end

function disband_room_ui_window.HandleNoClick()
	_s.isClicked = true
	_s.SelfOperateRefersh(false)
	m_cardPlayLogic:SendDismissVote(false)
end

function disband_room_ui_window.UpdateSecond()
	if m_countTime > 0 then
		m_countTime = m_countTime - 1
		if m_countTime >= 0 then
			--if _s.isClicked then  --状态不准确 一起刷没关系 隐藏不会刷新的
				m_luaWindowRoot:SetLabel(m_labelWaitTimeCountTrans,m_countTime)
			--else
				m_luaWindowRoot:SetLabel(m_labelSelfTimeCountTrans,m_countTime)
			--end
		end
	else
		_s.InitWindow(false,0)
	end
end


