--------------------------------------------------------------------------------
-- 	 File       : club_room_render_ctrl.lua
--   author     : zhisong
--   function   : 俱乐部房间列表显示控件
--   date       : 2018年1月29日 18:45:06
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------

require "LuaWindow.Module.UINetTipsCtrl"

club_room_render_ctrl = class("club_room_render_ctrl")

-------------------------------------------------------------------------------------------------------
function club_room_render_ctrl:ctor(lua_window_root)
	self.m_lua_win_root = lua_window_root
	self.m_root = self.m_lua_win_root:GetTrans("room_root")
	self.m_empty_root = self.m_lua_win_root:GetTrans("empty_room_root")
	self:InitScrollView()
	self:InitNetTipsCtrl()
end

function club_room_render_ctrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.RenderClubRoom, self.TryRenderUI, self)
	LuaEvent.AddHandle(EEventType.RefreshClubRoom, self.RefreshScrollview, self)
end

function club_room_render_ctrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.RenderClubRoom, self.TryRenderUI, self)
	LuaEvent.RemoveHandle(EEventType.RefreshClubRoom, self.RefreshScrollview, self)
end

function club_room_render_ctrl:Destroy()
	self:ClearScrollView()
	self:DestroyNetTipsCtrl()
end
--------------------------------------------------------------------------------

function club_room_render_ctrl:InitNetTipsCtrl()
    self.m_netTipsCtrl = UINetTipsCtrl.New()

	self.m_netTipsCtrl:Init(self.m_lua_win_root,self.m_lua_win_root:GetTrans("room_loading_tip"),function (...)
		local event_id, p1, p2 = ...
		if p1 then
			self.m_curr_club = p1
			p1.m_room_mgr:TryGetClubRoomList(p1.m_id,
				function()
					if self.m_lua_win_root and self.m_lua_win_root.m_open then
						self.m_netTipsCtrl:StopWork(true)
						
						self:RenderNormalStatus()
					end
					ClubSys.RecvData(2, true)
				end,
				function()
					ClubSys.RecvData(2, false)
					DwDebug.Log("WebNetHelper RequestCheckAgent failed")
					self.m_lua_win_root:SetActive(self.m_root, false)
					self.m_netTipsCtrl:StopWork(false)
				end,
				true)
		end
	end)
end

function club_room_render_ctrl:DestroyNetTipsCtrl()
    if self.m_netTipsCtrl then
        self.m_netTipsCtrl:Destroy()
        self.m_netTipsCtrl = nil
	end
end

function club_room_render_ctrl:TryRenderUI(event_id, p1, p2)
	self.m_lua_win_root:SetActive(self.m_root, false)
	self.m_netTipsCtrl:StartWork(event_id, p1, p2)
end

function club_room_render_ctrl:HandleWidgetClick(gb)
	if self.m_netTipsCtrl then
		self.m_netTipsCtrl:HandleWidgetClick(gb)
	end
	local click_name = gb.name
	if click_name == "join_btn" or click_name == "start_btn" then
		local room_name = gb.transform.parent.parent.name
		if string.find(room_name, "playroom_") then
			local room_id = tonumber(string.sub(room_name, string.find(room_name, "playroom_") + string.len("playroom_")))
			if room_id then
				HallSys.JoinRoom(room_id, function()
					MainCityState.AddEnterCallBack(
						function ()
							ClubSys.OpenClub(self.m_curr_club.m_id)
						end
					)
				end,nil)
			end
		end
	elseif click_name == "create_template" then
		if self.m_is_owner then
			if self.m_curr_club and self.m_curr_club.m_id then
				WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, self.m_curr_club.m_id, "Club.UIWindow.clubgame_ui_window", false , nil)
			end
		else
			if self.m_curr_club and self.m_curr_club.m_id then
				local data = {}
				data.clubID = self.m_curr_club.m_id
				WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 0, "Club.UIWindow.clubcreateroom_ui_window", false , data)
			end	 
		end
	end
end

function club_room_render_ctrl:RenderNormalStatus()
	self.m_lua_win_root:SetActive(self.m_root, true)
	self:InitData()
	if not self.m_rooms or #self.m_rooms <= 0 then
		self.m_lua_win_root:SetActive(self.m_empty_root, true)
		self.m_lua_win_root:SetActive(self.m_root, false)

		self.m_is_owner = self.m_curr_club and self.m_curr_club.m_owner_uid == DataManager.GetUserID()
		self.m_lua_win_root:ShowChild(self.m_lua_win_root:GetTrans(self.m_empty_root, "is_leader"), self.m_is_owner and "yes" or "no", true)
	else
		self.m_lua_win_root:SetActive(self.m_empty_root, false)
		self.m_lua_win_root:SetActive(self.m_root, true)

		if self.m_panel_scrollView and self.m_rooms then
			self.m_show_rooms = {}
			self.m_panel_scrollView:InitItemCount(#self.m_rooms, true)
		end
	end
end

function club_room_render_ctrl:InitData()
	if self.m_curr_club and self.m_curr_club.m_room_mgr then
		self.m_rooms = self.m_curr_club.m_room_mgr:GetRooms()
		self:SortList()
	end
end

function club_room_render_ctrl:SortList()
	table.sort(self.m_rooms,
		function (a, b)
			if a.m_priority == b.m_priority then
				return a.m_room_id < b.m_room_id
			else
				return a.m_priority < b.m_priority
			end
		end
	)
end

function club_room_render_ctrl:InitScrollView()
	if self.m_panel_scrollView == nil then
		local rootTrans = self.m_lua_win_root:GetTrans(self.m_root, "room_list")
		self.m_panel_scrollView = EZfunLimitScrollView.GetOrAddLimitScr(rootTrans)
		local item = self.m_lua_win_root:GetTrans(self.m_root, "room").gameObject
		self.m_panel_scrollView:InitForLua(rootTrans, item, UnityEngine.Vector2.New(0, 0), UnityEngine.Vector2.New(1, 4), LimitScrollViewDirection.SVD_Vertical, false)

		local initFunc = function (trans, item_index)
			self:InitTrans(trans, item_index + 1, self.m_rooms[item_index + 1])
		end
		self.m_initFuncSeq = LuaCsharpFuncSys.RegisterFunc(initFunc)

		self.m_panel_scrollView:SetInitItemCallLua(self.m_initFuncSeq)

		local hideFunc = function (trans, item_index)
			self:HideTrans(trans, item_index + 1, self.m_rooms[item_index + 1])
		end
		self.m_hideFuncSeq = LuaCsharpFuncSys.RegisterFunc(hideFunc)

		self.m_panel_scrollView:SetHideItemCallLua(self.m_hideFuncSeq)
	end 
end

function club_room_render_ctrl:ClearScrollView()
	LuaCsharpFuncSys.UnRegisterFunc(self.m_initFuncSeq)
	self.m_initFuncSeq = nil
	LuaCsharpFuncSys.UnRegisterFunc(self.m_hideFuncSeq)
	self.m_hideFuncSeq = nil
	self.m_panel_scrollView = nil
end

function club_room_render_ctrl:HideTrans(root, index, data)
	if data and self.m_show_rooms[data.m_room_id] then
		self.m_show_rooms[data.m_room_id] = nil
	end
end

function club_room_render_ctrl:InitTrans(root, index, data)
	self.m_show_rooms[data.m_room_id] = {root=root,index=index,data=data}
	root.name = tostring(index) .. "_playroom_" .. data.m_room_id

	self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "game_name"), data.m_play_name)
	self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "room_id"), data.m_room_id)
	self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "game_desc"), data.m_play_des)
	self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "round"), data.m_total_game_num)
	self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "players"), self:GenPlayerNumStr(#data.m_players, data.m_max_player_num))

	local club_icon = self.m_lua_win_root:GetTrans(root, "club_icon")
	if not data.m_players or #data.m_players <= 0 then
		self.m_lua_win_root:SetActive(club_icon, true)
		self.m_lua_win_root:SetSprite(club_icon, "clubData_headImg_" .. (self.m_curr_club and self.m_curr_club.m_g_img or "1"))
	else
		self.m_lua_win_root:SetActive(club_icon, false)
	end

	local player_names = "已加入："
	for i=1,4 do
		local head_trans = self.m_lua_win_root:GetTrans(root, "head_"..i)
		if data.m_players and data.m_players[i] then
			self.m_lua_win_root:SetActive(head_trans, true)
			player_names = player_names .. utf8sub(data.m_players[i].nickname, 1, 10) .. "，"
			WindowUtil.LoadHeadIcon(self.m_lua_win_root, head_trans, data.m_players[i].headimgurl, data.m_players[i].sex, false,RessStorgeType.RST_Never)
		else
			self.m_lua_win_root:SetActive(head_trans, false)
		end
	end

	if data.m_status == 1 then
		self.m_lua_win_root:ShowChild(self.m_lua_win_root:GetTrans(root, "status_btns"), "gaming", true)
		self.m_lua_win_root:ShowChild(self.m_lua_win_root:GetTrans(root, "status_desc"), "gaming_desc", true)
	else
		if #data.m_players == 0 then
			self.m_lua_win_root:ShowChild(self.m_lua_win_root:GetTrans(root, "status_btns"), "start_btn", true)
		elseif #data.m_players >= data.m_max_player_num then
			self.m_lua_win_root:ShowChild(self.m_lua_win_root:GetTrans(root, "status_btns"), "gaming", true)
		else
			self.m_lua_win_root:ShowChild(self.m_lua_win_root:GetTrans(root, "status_btns"), "join_btn", true)
		end
		self.m_lua_win_root:ShowChild(self.m_lua_win_root:GetTrans(root, "status_desc"), "wating_desc", true)
		self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "wating_desc"), player_names)
	end
end

-- 又不要颜色了。。。。。。
function club_room_render_ctrl:GenPlayerNumStr(cur_num, total)
	if cur_num > total then
		DwDebug.LogError("club_room_render_ctrl:GenPlayerNumStr get wrong data ", cur_num, total)
		return cur_num .. "/" .. total
	elseif cur_num == total then
		-- return "[442f05]" .. cur_num .. "/" .. total
		return cur_num .. "/" .. total
	else
		-- return "[a3d213]" .. cur_num .. "[442f05]/" .. total
		return cur_num .. "/" .. total
	end
end

function club_room_render_ctrl:RefreshScrollview(event_id, room_id)
	self:RenderNormalStatus()
	-- if not self.m_last_refresh_time or self.m_last_refresh_time + 2 < WrapSys.GetCurrentDateTime() then
	-- 	self.m_last_refresh_time = WrapSys.GetCurrentDateTime()
	-- 	self:RenderNormalStatus()
	-- else
	-- 	if not room_id or not self.m_show_rooms then
	-- 		return
	-- 	end

	-- 	if self.m_show_rooms[room_id] then
	-- 		local show_item = self.m_show_rooms[room_id]
	-- 		self:InitTrans(show_item.root, show_item.index, show_item.data)
	-- 	end
	-- end
end
