--------------------------------------------------------------------------------
-- 	 File       : DDZCardAIResultStruct.lua
--   author     : zhanghaochun
--   function   : 斗地主手牌提示出牌结果结构
--   date       : 2018-01-05
--   copyright  : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------

local BaseAIStruct = require "CommonProduct.DouDiZhu.Base.AI.BaseDouDiZhuCardAIResultStruct"

local DDZCardAIResultStruct = class("DDZCardAIResultStruct", BaseAIStruct)

function DDZCardAIResultStruct:ctor()
	self:InitBase()
end

return DDZCardAIResultStruct
