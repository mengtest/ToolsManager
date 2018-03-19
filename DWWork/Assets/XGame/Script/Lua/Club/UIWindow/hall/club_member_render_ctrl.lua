--------------------------------------------------------------------------------
-- 	 File       : club_member_render_ctrl.lua
--   author     : zhisong
--   function   : 俱乐部成员列表显示控件
--   date       : 2018年1月29日 18:45:06
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------

require "LuaWindow.Module.UINetTipsCtrl"

club_member_render_ctrl = class("club_member_render_ctrl")

-------------------------------------------------------------------------------------------------------
function club_member_render_ctrl:ctor(lua_window_root)
	self.m_lua_win_root = lua_window_root
	self.m_root = self.m_lua_win_root:GetTrans("member_root")
	self:InitScrollView()
	self:InitNetTipsCtrl()
end

function club_member_render_ctrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.RenderClubMember, self.TryRenderUI, self)
	LuaEvent.AddHandle(EEventType.RefreshClubMember, self.RefreshScrollview, self)
end

function club_member_render_ctrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.RenderClubMember, self.TryRenderUI, self)
	LuaEvent.RemoveHandle(EEventType.RefreshClubMember, self.RefreshScrollview, self)
end

function club_member_render_ctrl:Destroy()
	self:ClearScrollView()
	self:DestroyNetTipsCtrl()
end
--------------------------------------------------------------------------------
function club_member_render_ctrl:InitNetTipsCtrl()
    self.m_netTipsCtrl = UINetTipsCtrl.New()

	self.m_netTipsCtrl:Init(self.m_lua_win_root,self.m_lua_win_root:GetTrans("member_loading_tip"),function (...)
		local event_id, p1, p2 = ...
		if p1 then
			self.m_curr_club = p1
			p1.m_mem_mgr:TryGetClubMemberList(p1.m_id,
				function()
					if self.m_lua_win_root and self.m_lua_win_root.m_open then
						self.m_netTipsCtrl:StopWork(true)
						
						self:RenderNormalStatus()
					end
					ClubSys.RecvData(1, true)
				end,
				function()
					ClubSys.RecvData(1, false)
					DwDebug.Log("WebNetHelper RequestCheckAgent failed")
					self.m_lua_win_root:SetActive(self.m_root, false)
					self.m_netTipsCtrl:StopWork(false)
				end,
				true)
		end
	end)
end

function club_member_render_ctrl:DestroyNetTipsCtrl()
    if self.m_netTipsCtrl then
        self.m_netTipsCtrl:Destroy()
        self.m_netTipsCtrl = nil
	end
end


function club_member_render_ctrl:TryRenderUI(event_id, p1, p2)
	self.m_lua_win_root:SetActive(self.m_root, false)
	self.m_netTipsCtrl:StartWork(event_id, p1, p2)
end

function club_member_render_ctrl:HandleWidgetClick(gb)
	if self.m_netTipsCtrl then
		self.m_netTipsCtrl:HandleWidgetClick(gb)
	end
	local click_name = gb.name
	if string.find(click_name, "member_") then
		local uid = tonumber(string.sub(click_name, string.find(click_name, "member_") + string.len("member_")))
		if uid then
			-- TODOTODO 点击好友触发什么
		end
	end
end

function club_member_render_ctrl:RenderNormalStatus()
	self.m_lua_win_root:SetActive(self.m_root, true)
	self:InitData()
	if self.m_panel_scrollView and self.m_players then
		self.m_show_players = {}
		self.m_panel_scrollView:InitItemCount(#self.m_players, true)
	end
end

function club_member_render_ctrl:InitData()
	if self.m_curr_club and self.m_curr_club.m_mem_mgr then
		self.m_players = self.m_curr_club.m_mem_mgr:GetMembers()
		self:SortList()
	end
end

function club_member_render_ctrl:SortList()
	table.sort(self.m_players,
		function (a, b)
			if a.m_priority == b.m_priority then
				return a.m_uid < b.m_uid
			else
				return a.m_priority < b.m_priority
			end
		end
	)
end

function club_member_render_ctrl:InitScrollView()
	if self.m_panel_scrollView == nil then
		local rootTrans = self.m_lua_win_root:GetTrans(self.m_root, "member_list")
		self.m_panel_scrollView = EZfunLimitScrollView.GetOrAddLimitScr(rootTrans)
		local item = self.m_lua_win_root:GetTrans(self.m_root, "member").gameObject
		self.m_panel_scrollView:InitForLua(rootTrans, item, UnityEngine.Vector2.New(0, 0), UnityEngine.Vector2.New(1, 4), LimitScrollViewDirection.SVD_Vertical, false)

		local initFunc = function (trans, item_index)
			self:InitTrans(trans, item_index + 1, self.m_players[item_index + 1])
		end
		self.m_initFuncSeq = LuaCsharpFuncSys.RegisterFunc(initFunc)

		self.m_panel_scrollView:SetInitItemCallLua(self.m_initFuncSeq)

		local hideFunc = function (trans, item_index)
			self:HideTrans(trans, item_index + 1, self.m_players[item_index + 1])
		end
		self.m_hideFuncSeq = LuaCsharpFuncSys.RegisterFunc(hideFunc)

		-- self.m_panel_scrollView:SetHideItemCallLua(self.m_hideFuncSeq)
	end
end

function club_member_render_ctrl:ClearScrollView()
	LuaCsharpFuncSys.UnRegisterFunc(self.m_initFuncSeq)
	self.m_initFuncSeq = nil
	LuaCsharpFuncSys.UnRegisterFunc(self.m_hideFuncSeq)
	self.m_hideFuncSeq = nil
	self.m_panel_scrollView = nil
end

function club_member_render_ctrl:HideTrans(root, index, data)
	if data and self.m_show_players[data.m_uid] then
		self.m_show_players[data.m_uid] = nil
	end
end

function club_member_render_ctrl:InitTrans(root, index, data)
	self.m_show_players[data.m_uid] = {root=root,index=index,data=data}
	root.name = tostring(index) .. "_member_" .. data.m_uid

	self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "name"), data.m_wx_nickname)
	self.m_lua_win_root:ShowChild(self.m_lua_win_root:GetTrans(root, "status"), (data.m_online_status == 2 and "gaming") or (data.m_online_status == 1 and "online") or "offline", true)
	WindowUtil.LoadHeadIcon(self.m_lua_win_root,self.m_lua_win_root:GetTrans(root, "head"), data.m_wx_headimgurl, data.m_wx_sex, false,RessStorgeType.RST_Never)
end

function club_member_render_ctrl:RefreshScrollview(event_id, uid)
	self:RenderNormalStatus()
	-- if not self.m_last_refresh_time or self.m_last_refresh_time + 2 < WrapSys.GetCurrentDateTime() then
	-- 	self.m_last_refresh_time = WrapSys.GetCurrentDateTime()
	-- 	self:RenderNormalStatus()
	-- else
	-- 	if not uid or not self.m_show_players then
	-- 		return
	-- 	end

	-- 	if self.m_show_players[uid] then
	-- 		local show_item = self.m_show_players[uid]
	-- 		self:InitTrans(show_item.root, show_item.index, show_item.data)
	-- 	end
	-- end
end
