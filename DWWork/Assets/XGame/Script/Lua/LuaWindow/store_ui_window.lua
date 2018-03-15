--------------------------------------------------------------------------------
-- 	 File      : store_ui_window.lua
--   author    : guoliang
--   function   : 解散出房间窗口
--   date      : 2017-10-9
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

store_ui_window = {}
local _s = store_ui_window
_s.isBuying = false
local m_luaWindowRoot
local m_state

local m_itemTransList = {}


function store_ui_window.Init(luaWindowRoot)
	m_luaWindowRoot = luaWindowRoot
end

function store_ui_window.PreCreateWindow()

end

function store_ui_window.CreateWindow()
	for i = 1,8 do
		table.insert(m_itemTransList,m_luaWindowRoot:GetTrans(i.."_sale_item"))
	end

	_s.RegisterEventHandle()
end

function store_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, -1, false)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		_s.m_open = open
		_s.m_state = state
		
		_s.InitWindowDetail()
	end

end

function store_ui_window.OnDestroy()
	m_luaWindowRoot = nil
	_s.UnRegisterEventHandle()
end

function store_ui_window.RegisterEventHandle()
	LuaEvent.AddHandle(EEventType.UI_StoreWindowBtnGray,_s.SetBtnGray,nil)

end

function store_ui_window.SetBtnGray(eventId,p1,p2)
	local isAcitve = p1
	if not isAcitve then
		_s.isBuying = false
		m_luaWindowRoot:SetGray(_s.curClickTrans,false,true)
	end
end

function store_ui_window.UnRegisterEventHandle()
	LuaEvent.RemoveHandle(EEventType.UI_StoreWindowBtnGray,_s.SetBtnGray,nil)
end


function store_ui_window.InitWindowDetail()
	_s.isBuying = false
	local saleItemList = CIAPSys.storeItemList
	for k,v in pairs(saleItemList) do
		_s.InitSaleItem(m_itemTransList[k],v)
	end
end

function store_ui_window.InitSaleItem(trans,saleItem)
	if trans and saleItem then
		local labelCardNumTrans = m_luaWindowRoot:GetTrans(trans,"label_card_num")
		local labelCostTrans = m_luaWindowRoot:GetTrans(trans,"label_cost")
		local btnTrans = m_luaWindowRoot:GetTrans(trans,"cost_root")
		m_luaWindowRoot:SetLabel(labelCardNumTrans,saleItem.card_number)
		m_luaWindowRoot:SetLabel(labelCostTrans,saleItem.price/100)
		m_luaWindowRoot:SetGray(_s.curClickTrans,false,true)
	end
end

local function FindSaleIndex(transName)
	if string.find(transName, "_") ~= nil then
		local firstSplit = string.find(transName, "_")
		local id = tonumber(string.sub(transName, 1, firstSplit - 1))
		return id
	end
end

function store_ui_window.HandleWidgetClick(gb)
	local clickName = gb.name
	if clickName == "close_btn" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		_s.InitWindow(false,0)
	elseif clickName == "cost_root" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		local saleIndex = FindSaleIndex(gb.transform.parent.name)
		_s.HandleClickSaleItem(gb.transform,saleIndex)
	end
end


function store_ui_window.HandleClickSaleItem(trans,saleIndex)
	if saleIndex >= 1 and saleIndex <= 8 then
		if not _s.isBuying then
			_s.isBuying = true
			_s.curClickTrans = trans
			m_luaWindowRoot:SetGray(trans,true,true)
			CIAPSys.SendBuyItem(saleIndex)
		end
	end
end




