--------------------------------------------------------------------------------
-- 	 File      : area_ui_window.lua
--   author    : jianing
--   function  : 选择区域界面 
--   date      : 2018-03-26
--   copyright : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------
require "LuaSys.AreaLuaSys"

local m_luaWindowRoot
local m_state

area_ui_window = {

}

local _s = area_ui_window
local AreaInfoList = nil

local function InitWindowDetail()
    _s.InitScrollView()

    if AreaInfoList == nil then
        AreaInfoList = AreaLuaSys.GetAreaInfoList()
    end
     _s.area_scrollview:InitItemCount(#AreaInfoList,true)
end

function area_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function area_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		m_state = state
		InitWindowDetail()
	end
end

function area_ui_window.InitTrans(trans, item_index)
    local areaInfo = AreaInfoList[item_index + 1]
    if areaInfo then
        m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "txt_userId"), areaInfo.AreaShowName, false)
        for i=1,8 do
            m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "outLine_" .. i), areaInfo.AreaShowName, false)
        end
    end
    trans.name = "areaItem_" .. item_index
end

function area_ui_window.InitScrollView()
    if _s.area_scrollview == nil then
        local rootTrans = m_luaWindowRoot:GetTrans("area_scrollview")
        _s.area_scrollview = EZfunLimitScrollView.GetOrAddLimitScr(rootTrans)
        local item = m_luaWindowRoot:GetTrans("areaItemPre").gameObject
        _s.area_scrollview:InitForLua(rootTrans, item, UnityEngine.Vector2.New(400, 140), UnityEngine.Vector2.New(3, 5), LimitScrollViewDirection.SVD_Vertical, false)
       
        _s.m_initFuncSeq = LuaCsharpFuncSys.RegisterFunc(_s.InitTrans)
        _s.area_scrollview:SetInitItemCallLua(_s.m_initFuncSeq)
    end
end

function area_ui_window.CreateWindow()

end

function area_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name
	if string.find(click_name, "areaItem_") then
        local index = tonumber(string.sub(click_name, string.len("areaItem_") + 1))
		if index then
			_s.ClickAreaItem(index)
		end
	end
end

function area_ui_window.ClickAreaItem(index)
    local areaInfo = AreaInfoList[index + 1]

    local content = string.format("确定要选择 %s 吗?\n选择好地区将为您立即跳转",areaInfo.AreaShowName)
    
    WindowUtil.ShowErrorWindow(4,content,nil,function ( )
        AreaLuaSys.SetNowAreaName(areaInfo.AreaName)
        LoginState.NormalEnter()
    end)
   
end

function area_ui_window.UnRegister()
    m_luaWindowRoot = nil
end

function area_ui_window.OnDestroy()
    
end