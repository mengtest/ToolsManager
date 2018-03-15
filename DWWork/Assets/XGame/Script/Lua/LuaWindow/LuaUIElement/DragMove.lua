--------------------------------------------------------------------------------
-- 	 File      : DragMove.lua
--   author    : zs
--   function   : UI拖动game object(已弃用)
--   date      : 2017年11月6日
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

DragMove = class("DragMove", nil)

function DragMove:ctor(gb)
	self.gameObject = gb
end

function DragMove:OnDrag(delta)
	local touch = UICamera.currentTouch
	if not self.gameObject or not touch.dragged then
		return
	end

	-- 相较于DragOutCard里边的拖动实现，这里是另一种方式修改gameObject的位置
	local world_pos = WrapSys.ScreenToWorldPoint(touch.pressedCam, UnityEngine.Vector3.New(touch.pos.x, touch.pos.y, 0))
	self.gameObject.transform.position = world_pos
end

return DragMove