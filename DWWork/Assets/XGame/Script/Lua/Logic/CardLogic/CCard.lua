--------------------------------------------------------------------------------
-- 	 File      : CCard.lua
--   author    : guoliang
--   function   : 扑克牌对象
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
--花色 1 方片 2 梅花 3 红桃 4 黑桃  5 鬼

CCard = class("CCard", nil)

function CCard:ctor()
	self.ID = nil -- ID
	self.seq = 0 -- 手牌唯一序列号

	self.typeTag = 0 --一些标记 现在用于自选牌 排序
end

function CCard:Init(id,seq)
	self.ID = id
	self.seq = seq
	
	local resCardItem = LuaTableSys.GetEntry("ResPKCardList",id)
	if resCardItem then
		self.logicValue = resCardItem.logicValue
		self.ico_num = resCardItem.ico_num
		self.ico_big = resCardItem.ico_big
		self.color = resCardItem.color
		self.sound = resCardItem.sound
	end
end

function CCard:IsVaild( ... )
	return self.ID >= 3 and self.ID <= 84
end
-- 同花色
function CCard:IsSameColor(color)
	return self.color == color
end

function CCard:IsKing()
	return self.ID == 83 or self.ID == 84
end

function CCard:IsCanLine()
	return self.logicValue < 15  -- 2、王不能連
end


function CCard:Destroy()
	self.ID = nil
end