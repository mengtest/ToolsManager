--------------------------------------------------------------------------------
-- 	 File      : CardPlayConfigModule.lua
--   author    : guoliang
--   function   : 玩法模式配置组件（是否打独，是否全开）
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.DWBaseModule"

CardPlayConfigModule = class("CardPlayConfigModule", DWBaseModule)


function CardPlayConfigModule:ctor()

end

function CardPlayConfigModule:Init()

end

function CardPlayConfigModule:SetPointType(pointType)
	self.pointType = pointType
end

function CardPlayConfigModule:SetPlayModel(isAlone)
	self.isAlone = isAlone
end

function CardPlayConfigModule:GetPointType()
	return self.pointType
end

function CardPlayConfigModule:GetPlayModel()
	return self.playModel
end

function CardPlayConfigModule:Clear()

end