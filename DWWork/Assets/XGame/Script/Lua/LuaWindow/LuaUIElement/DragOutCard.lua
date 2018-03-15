--------------------------------------------------------------------------------
-- 	 File      : DragOutCard.lua
--   author    : zs
--   function   : UI拖动出牌
--   date      : 2017年11月6日
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

-- 用两层继承(基类还要实例化的情况)被坑的体无完肤，不过想要用的可以试试，也是可以实现的，哈哈哈
-- B = class("B", A),B.New()得到的元表全是A，而不是A.New()
-- require "LuaWindow.LuaUIElement.DragMove"
-- DragOutCard = class("DragOutCard", DragMove)

-- 每个ComponentBaseLua实例对应一个table，点击事件不会混乱
DragOutCard = class("DragOutCard", nil)

function DragOutCard:ctor(gb)
	self.gameObject = gb
	self.transform = gb.transform
	self.m_playLogic = PlayGameSys.GetPlayLogic()

	self.dragCheck = nil
end

function DragOutCard:Init(luaComRoot)
	self.m_luaComRoot = luaComRoot
	self.copyGo = nil
end

function DragOutCard:OnClick()
	-- logError("click")
	-- DwDebug.LogError("Drag", "OnClick", self.transform.name.."("..self.transform:GetInstanceID()..")")
	
	--LuaEvent.AddEventNow(EEventType.MJClickCard, self.transform)
end

local dragingTrans = nil
function DragOutCard:OnDragStart()
--	DwDebug.LogError("Drag", "OnDragStart = " .. tostring(self.m_dragStarted), self.transform.name.."("..self.transform:GetInstanceID()..")")
	if self.m_dragStarted then
		return
	end
--	DragEventCheck.AddCurrentDrag(self)
	-- 设置开始拖拽标志位，备份初始位置
	self:CreateCopyGo()
	dragingTrans = self.transform
	self.m_dragStarted = true
	self.m_totalDelta = UnityEngine.Vector3.zero
	self.m_backupLocalPos = self.transform.localPosition
	self.m_luaComRoot:ChangeDepth(self.transform, 10000)
	

	LateUpdateBeat:Add(self.Update, self)
end

function DragOutCard:OnDrag(delta)
	-- DwDebug.LogError("Drag", "OnDrag = " .. tostring(self.m_dragStarted), self.transform.name.."("..self.transform:GetInstanceID()..")")

	if self.m_dragStarted and self.m_totalDelta then
		-- 在编辑器模式下，没有计算窗口大小
		local scale_parm_x = (1280/WrapSys.GameRoot_Screen_width())
		local scale_parm_y = (720/WrapSys.GameRoot_Screen_height())
		-- local scale_parm_x = 1
		-- local scale_parm_y = 1
		-- print("--------------scale_parm_x:", scale_parm_x, "scale_parm_y:", scale_parm_y)
		-- print("--------------delta.x:", delta.x, "delta.y:", delta.y)
		self.m_totalDelta = self.m_totalDelta + Vector3.New(delta.x * scale_parm_x, delta.y * scale_parm_y, 0)
		-- if self.m_totalDelta.y > 30 then
		-- 	if self.m_playLogic and self.m_playLogic:IsMyTurn() then
		-- 		LuaEvent.AddEventNow(EEventType.MJPlayCard, self.transform)
		-- 	end

		-- 	self.transform.localPosition = self.m_backupLocalPos
		-- 	self.m_dragStarted = false
		-- else
		-- 	self.transform.localPosition = self.m_backupLocalPos + UnityEngine.Vector3.New(self.m_totalDelta.x, self.m_totalDelta.y, 0)
		-- end
		self.transform.localPosition = self.m_backupLocalPos + self.m_totalDelta
		-- self.transform.localPosition = self.m_backupLocalPos + UnityEngine.Vector3.New(self.m_totalDelta.x, self.m_totalDelta.y, 0)
	end
end

function DragOutCard:OnDragEnd()
--	 DwDebug.LogError("Drag", "OnDragEnd = " .. tostring(self.m_dragStarted), self.m_totalDelta or "totalDelta=nil", 
		-- self.transform.name.."("..self.transform:GetInstanceID()..")")

	self:DestoryCopyGo()
	if self.m_dragStarted then
--		DragEventCheck.RemoveCurrentDrag(self)
		self:Reset()
	end
end

function DragOutCard:OnDoubleClick()
	-- DwDebug.LogError("Drag", "OnDoubleClick", self.transform.name.."("..self.transform:GetInstanceID()..")")
end

function DragOutCard:OnPress(flag)
--	DwDebug.LogError("Drag", "OnPress", flag, self.transform.name.."("..self.transform:GetInstanceID()..")")
	if flag then
		LuaEvent.AddEventNow(EEventType.MJClickCard, self.transform)
	elseif not flag and self.m_dragStarted then
--		DwDebug.LogError("OnDragEnd havn't been called collectly,but OnPress have been called collectly!!!")
		self:Reset()
	end
end

function DragOutCard:OnHover(flag)
	-- DwDebug.LogError("Drag", "OnHover", flag, self.transform.name.."("..self.transform:GetInstanceID()..")")
end

-- 选择下个带有此脚本的物件才会调用onselect(false)
function DragOutCard:OnSelect(flag)
--	LuaEvent.AddEventNow(EEventType.MJClickCard, self.transform)
	-- DwDebug.LogError("Drag", "OnSelect", flag, self.transform.name.."("..self.transform:GetInstanceID()..")")
end
-- 点击A然后开始拖，鼠标进入B的区域后，调用B的OnDragOver，gb传的A；B为所有带脚本的物件，包括A在内
function DragOutCard:OnDragOver(gb)
	-- DwDebug.LogError("Drag", "OnDragOver", gb.name.."("..gb:GetInstanceID()..")", self.transform.name.."("..self.transform:GetInstanceID()..")")
end
-- 点击A然后开始拖，鼠标离开B的区域后，调用B的OnDragOut，gb传的A；B为所有带脚本的物件，包括A在内
function DragOutCard:OnDragOut(gb)
	-- DwDebug.LogError("Drag", "OnDragOut", gb.name.."("..gb:GetInstanceID()..")", self.transform.name.."("..self.transform:GetInstanceID()..")")
end
-- 点击A然后开始拖,鼠标在B上松开，会调用到B的ondrop，gb传的A；B为除了A的所有其他带脚本的物件
function DragOutCard:OnDrop(gb)
	-- DwDebug.LogError("Drag", "OnDrop", gb.name.."("..gb:GetInstanceID()..")", self.transform.name.."("..self.transform:GetInstanceID()..")")
end

-- 检测点击事件
function DragOutCard:Update()
	if self.m_dragStarted then
		if not WrapSys.TouchIng() or dragingTrans ~= self.transform then
--			DwDebug.LogError("Drag", "Update", WrapSys.TouchIng(), "dragingTrans ~= self.transform", dragingTrans ~= self.transform)
			self:Reset()
		end
	end
end

-- 重置
function DragOutCard:Reset()
	if self.m_totalDelta and self.m_totalDelta.y > 35 then
		if self.m_playLogic and self.m_playLogic:IsMyTurn() then
			LuaEvent.AddEventNow(EEventType.MJPlayCard, self.transform)
		end
	end

	self.transform.localPosition = self.m_backupLocalPos

	self.m_dragStarted = false
	self.m_totalDelta = UnityEngine.Vector3.zero
	self.m_backupLocalPos = nil
	self.m_luaComRoot:ChangeDepth(self.transform, -10000)
	
	self:DestoryCopyGo()
	LateUpdateBeat:Remove(self.Update, self)
	-- 疯狂的快速拖动的话，会出现有调用ondragstart没调用ondragend
	-- LuaEvent.AddEventNow(EEventType.MJRefreshCard, SeatPosEnum.South)
end

-- 创建麻将牌的复制牌
function DragOutCard:CreateCopyGo()
	if self.copyGo == nil then
		self.copyGo = GameObject.Instantiate(self.gameObject, self.transform.parent)
		self.copyGo.transform.localPosition = self.transform.localPosition
		self.copyGo.transform.localScale = self.transform.localScale
		
		local collider = self.copyGo:GetComponent("BoxCollider")
		if collider then
			collider.enabled = false
		end
		
		local baseLua = self.copyGo:GetComponent("ComponentBaseLua")
		if baseLua then
			baseLua.enabled = false
		end
	else
		self:DestoryCopyGo()
		self:CreateCopyGo()
	end
end

-- 销毁麻将牌的复制牌
function DragOutCard:DestoryCopyGo()
	if self.copyGo ~= nil then
		GameObject.Destroy(self.copyGo)
		self.copyGo = nil
	end	
end

-- 不可或缺，没有的话functions.lua里的CreateObject函数中require返回bool而不是table，创建不了table
return DragOutCard