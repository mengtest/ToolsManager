--------------------------------------------------------------------------------
-- 	 File       : club_title_render_ctrl.lua
--   author     : zhisong
--   function   : 俱乐部列表显示控件
--   date       : 2018年1月29日 18:45:06
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------

require "LuaWindow.Module.UINetTipsCtrl"

club_title_render_ctrl = class("club_title_render_ctrl")

-------------------------------------------------------------------------------------------------------
function club_title_render_ctrl:ctor(lua_win_root, parent)
	self.m_lua_win_root = lua_win_root
	self.m_root = self.m_lua_win_root:GetTrans("club_title_list")
	self.m_parent = parent
	self:RegisterEvent()
	self:InitNetTipsCtrl()
end

function club_title_render_ctrl:RegisterEvent()
end

function club_title_render_ctrl:UnRegisterEvent()
end

function club_title_render_ctrl:Destroy()
	self.m_lua_win_root = nil
	self:UnRegisterEvent()
	self:DestroyNetTipsCtrl()
end

function club_title_render_ctrl:InitNetTipsCtrl()
    self.m_netTipsCtrl = UINetTipsCtrl.New()

	self.m_netTipsCtrl:Init(self.m_lua_win_root,self.m_lua_win_root:GetTrans("title_loading_tip"),function (...)
		local call_path, param = ...
		ClubSys.TryGetClubList(
		function(body, head)
            if self.m_lua_win_root and self.m_lua_win_root.m_open then
				self.m_netTipsCtrl:StopWork(true)
				
				local refresh_whole = self:RefreshWholeWindow(call_path, param)

				self.m_data = ClubSys.GetClubs()

				if not self.m_data or #self.m_data <= 0 then
					WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, false, 1, "Club.UIWindow.hall.club_hall_ui_window", false , nil)
				else 
					-- table.sort(self.m_data, function (a, b) return a.m_id < b.m_id end)
					self:InitUIContent()
					self:RenderNormalStatus(self:GetCurClubIndex(call_path, param))
					if refresh_whole then
						self:NotifyRenderClubDetail()
					end
				end
			end
        end,
        function(body, head)
			DwDebug.Log("WebNetHelper RequestCheckAgent failed")
			self.m_lua_win_root:SetActive(self.m_root, false)
            self.m_netTipsCtrl:StopWork(false)
        end, self:ShouldForceRequest(call_path, param))
	end)
end

function club_title_render_ctrl:DestroyNetTipsCtrl()
    if self.m_netTipsCtrl then
        self.m_netTipsCtrl:Destroy()
        self.m_netTipsCtrl = nil
	end
end


--------------------------------------------------------------------------------

function club_title_render_ctrl:TryRenderUI(call_path, group_id)
	self.m_lua_win_root:SetActive(self.m_root, false)
	self.m_netTipsCtrl:StartWork(call_path, group_id)
end

function club_title_render_ctrl:HandleWidgetClick(gb)
	if self.m_netTipsCtrl then
		self.m_netTipsCtrl:HandleWidgetClick(gb)
	end
	local click_name = gb.name
	if string.find(click_name, "club_title_") then
		local index = tonumber(string.sub(click_name, string.len("club_title_") + 1))
		if index and (not self.m_cur_index or (self.m_cur_index and index ~= self.m_cur_index)) then
			self:RenderNormalStatus(index)
			self:NotifyRenderClubDetail()
		end
	end
end

--------------------------------------------------------------------------------

function club_title_render_ctrl:HighLight(data, index)
	local trans
	if self.m_cur_index then
		trans = self.m_lua_win_root:GetTrans(self.m_root, "club_title_"..self.m_cur_index)
		if trans then
			self.m_lua_win_root:SetActive(self.m_lua_win_root:GetTrans(trans, "high_light"), false)
		end
	end
	
	self.m_cur_club = data[index]
	self.m_cur_index = index
	trans = self.m_lua_win_root:GetTrans(self.m_root, "club_title_" .. index)
	if trans then
		self.m_lua_win_root:SetActive(self.m_lua_win_root:GetTrans(trans, "high_light"), true)
	end
end

function club_title_render_ctrl:RenderNormalStatus(index)
	if not self.m_data then
		return
	end

	-- if self.m_cur_club then
	-- 	ClubSys.SendEnterClubUI(false, self.m_cur_club.m_id)
	-- end

	self:HighLight(self.m_data, index)

	ClubSys.SendEnterClubUI(true, self.m_cur_club.m_id)
end

function club_title_render_ctrl:InitUIContent()
	self.m_lua_win_root:SetActive(self.m_root, true)
	for i=1,5 do
		local single_title = self.m_lua_win_root:GetTrans(self.m_root, "club_title_"..i)
		self:RenderSingleTitle(single_title, self.m_data[i])
	end
end

function club_title_render_ctrl:RenderSingleTitle(root, data)
	if data then
		self.m_lua_win_root:SetActive(root, true)
		self.m_lua_win_root:SetActive(self.m_lua_win_root:GetTrans(root, "high_light"), true)
		self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "h_name"), data.m_name)
		self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "name"), data.m_name)
		self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "id"), data.m_id)
		self.m_lua_win_root:SetActive(self.m_lua_win_root:GetTrans(root, "high_light"), false)
	else 
		self.m_lua_win_root:SetActive(root, false)
	end
end

function club_title_render_ctrl:NotifyRenderClubDetail()
	ClubSys.ResetRecvFlag()
	WrapSys.EZFunWindowMgr_SetWindowStatusForLua(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, true, 20000 + 5, "wait_ui_window", false)
	LuaEvent.AddEventNow(EEventType.RenderClubMember, self.m_cur_club)
	LuaEvent.AddEventNow(EEventType.RenderClubRoom, self.m_cur_club)
end

function club_title_render_ctrl:GetCurClubIndex(call_path, group_id)
	local club_id
	if call_path == 1 then
		if group_id then
			club_id = group_id
		else 
			return 1
		end
	else
		if not self.m_cur_club or not self.m_cur_club.m_id then
			return 1
		else 
			club_id = self.m_cur_club.m_id
		end
	end

	for i,v in ipairs(self.m_data) do
		if v.m_id == club_id then
			return i
		end
	end

	return 1
end

function club_title_render_ctrl:RefreshWholeWindow(call_path, group_id)
	if call_path >= 1 and call_path <= 3 then
		return true
	elseif call_path == 4 then
		if group_id and self.m_cur_club and self.m_cur_club.m_id then
			return group_id == self.m_cur_club.m_id
		else 
			return true
		end
	else 
		return true
	end
end


function club_title_render_ctrl:ShouldForceRequest(call_path, params)
	if not call_path then
		return true
	end

	if call_path == 1 or call_path == 4 then
		return false
	else
		return true
	end
end
--------------------------------------------------------------------------------

