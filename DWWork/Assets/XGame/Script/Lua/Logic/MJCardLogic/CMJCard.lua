--------------------------------------------------------------------------------
-- 	 File      : CMJCard.lua
--   author    : guoliang
--   function   : 麻将牌对象
--   date      : 2017-11-5
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
--花色 1 万 2 筒 3 条 4 风  5 字 6 花

CMJCard = class("CMJCard", nil)

function CMJCard:ctor()
	self.ID = nil -- ID
	self.seq = 0 -- 手牌唯一序列号
end

function CMJCard:Init(id,seq)
	self.ID = id
	self.seq = seq
	
	local resCardItem = LuaTableSys.GetEntry("ResMJCardList",id)
	if resCardItem then
		self.ico = resCardItem.ico
		self.color = resCardItem.color
		self.sound = resCardItem.sound
	end
end

function CMJCard:IsVaild()
	return self.ID >= 1 and self.ID <= 58
end

-- 同花色
function CMJCard:IsSameColor(color)
	return self.color == color
end


function CMJCard:Destroy()
	self.ID = nil
	self.seq = 0
end