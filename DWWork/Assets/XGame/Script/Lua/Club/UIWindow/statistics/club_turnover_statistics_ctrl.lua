--------------------------------------------------------------------------------
-- 	 File       : club_turnover_statistics_ctrl.lua
--   author     : zhisong
--   function   : 局数统计窗口组件
--   date       : 2018年2月24日
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------
require "Club.UIWindow.statistics.page_scrollview_ctrl"

club_turnover_statistics_ctrl = class("club_turnover_statistics_ctrl", page_scrollview_ctrl)

-------------------------------------------------------------------------------------------------------
function club_turnover_statistics_ctrl:ctor(lua_window_root)
    self:InitParams({
        m_root = lua_window_root:GetTrans("charge_statistics"),
        m_show_num = UnityEngine.Vector2.New(1, 13)
    })
    self.super.ctor(self, lua_window_root)
end

function club_turnover_statistics_ctrl:Destroy()
    self.super.Destroy(self)
end
--------------------------------------------------------------------------------

-- 初始化ui，此模块显示总入口
function club_turnover_statistics_ctrl:InitUI(group_id)
    self.m_group_id = group_id
	self:TryRenderUI(false, 1, 26)
end

-- 按键响应函数
function club_turnover_statistics_ctrl:HandleWidgetClick(gb)
    self.super.HandleWidgetClick(self, gb)
	local click_name = gb.name
end

function club_turnover_statistics_ctrl:InitTrans(root, index, data)
	root.name = "record_" .. index

	self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "charge_cards"), data.card_number)
	self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "remain_cards"), data.now_card_number)
	self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "charge_time"), os.date("%Y/%m/%d %H:%M", data.createtime))
end

function club_turnover_statistics_ctrl:Request(succ_cb, fail_cb, page_index, page_size)
    WebNetHelper.RequestCardFlow(page_index, page_size, self.m_group_id, 1, succ_cb, fail_cb)
end


