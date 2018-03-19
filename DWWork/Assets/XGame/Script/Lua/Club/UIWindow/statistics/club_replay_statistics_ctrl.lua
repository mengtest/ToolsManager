--------------------------------------------------------------------------------
-- 	 File       : club_replay_statistics_ctrl.lua
--   author     : zhisong
--   function   : 战绩统计窗口组件
--   date       : 2018年2月24日
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------
require "Club.UIWindow.statistics.page_scrollview_ctrl"

club_replay_statistics_ctrl = class("club_replay_statistics_ctrl", page_scrollview_ctrl)

-------------------------------------------------------------------------------------------------------
function club_replay_statistics_ctrl:ctor(lua_window_root)
    self:InitParams({
        m_root = lua_window_root:GetTrans("game_statistics"),
        m_show_num = UnityEngine.Vector2.New(1, 4)
    })
    self.super.ctor(self, lua_window_root)
end

function club_replay_statistics_ctrl:Destroy()
    self.super.Destroy(self)
end
--------------------------------------------------------------------------------

-- 初始化ui，此模块显示总入口
function club_replay_statistics_ctrl:InitUI(group_id)
    self.m_group_id = group_id
	self:TryRenderUI(false, 1, 20)
end

-- 按键响应函数
function club_replay_statistics_ctrl:HandleWidgetClick(gb)
    self.super.HandleWidgetClick(self, gb)
    local click_name = gb.name
    
    if click_name == "detail_btn" then
		local archive_name = gb.transform.parent.name
		if string.find(archive_name, "record_") then
            local index = tonumber(string.sub(archive_name, 1, string.find(archive_name, "_record_")-1))
			if index then
                PlayRecordSys.SetCurReqRecord(self.m_data_list[index])
                WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, self.m_data_list[index].archive_id, "record_detail_ui_window", false , self.m_group_id)
            end
		end
    end
end

function club_replay_statistics_ctrl:InitTrans(root, index, data)
	root.name = index .. "_record_" .. data.archive_id

	self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "game_time"), os.date("%Y/%m/%d %H:%M", data.room_create_time))
	self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "game_name"), data.play_name)
	self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "room_id"), data.room_id)
	self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "round_count"), data.total_game_num)
	self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "cost_cards"), data.total_card_num .. "张")

    local name_root = self.m_lua_win_root:GetTrans(root, "player_name")
    local score_root = self.m_lua_win_root:GetTrans(root, "player_score")
    local name_node, score_node
    for i=1,4 do
        name_node = self.m_lua_win_root:GetTrans(name_root, "name_"..i)
        score_node = self.m_lua_win_root:GetTrans(score_root, "score_"..i)
        if data.players[i] then
            self.m_lua_win_root:SetActive(name_node, true) 
            self.m_lua_win_root:SetActive(score_node, true) 
            self.m_lua_win_root:SetLabel(name_node, utf8sub(data.players[i].nickname, 1, 10))
            self.m_lua_win_root:SetLabel(score_node, data.players[i].jifen)
        else
            self.m_lua_win_root:SetActive(name_node, false) 
            self.m_lua_win_root:SetActive(score_node, false) 
        end
    end
end

function club_replay_statistics_ctrl:Request(succ_cb, fail_cb, page_index, page_size)
    WebNetHelper.RequestArchiveList(page_index, page_size, self.m_group_id, nil, succ_cb, fail_cb)
end


