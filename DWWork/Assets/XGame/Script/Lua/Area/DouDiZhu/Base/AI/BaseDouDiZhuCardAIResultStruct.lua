--------------------------------------------------------------------------------
-- 	 File       : BaseDouDiZhuCardAIResultStruct.lua
--   author     : zhanghaochun
--   function   : 手牌提示出牌结果结构
--   date       : 2017-01-05
--   copyright  : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------

local AnalysisStruct = require "CommonProduct.CommonBase.Struct.CardsStruct"

local BaseDouDiZhuCardAIResultStruct = class("BaseDouDiZhuCardAIResultStruct")

function BaseDouDiZhuCardAIResultStruct:ctor()
	self:InitBase()
end

function BaseDouDiZhuCardAIResultStruct:InitBase()
	self.Index = 0
	self.ResultCards = nil
	self.struct = AnalysisStruct.New()        -- 手牌结构分析组件
	self.srcStruct = AnalysisStruct.New()     -- 上家出牌结构分析组件

	self.CompareFunc = function(cards1, cards2)
		if cards1[1].logicValue < cards2[1].logicValue then
			return true
		elseif cards1[1].logicValue == cards2[1].logicValue then
			return cards1[1].ID > cards2[1].ID
		else
			return false
		end
	end
end

function BaseDouDiZhuCardAIResultStruct:GetCount(handCards, srcCardType, srcCards, bNewTip)
	if self.ResultCards == nil or bNewTip then
		self:CreateAIResult(handCards, srcCardType, srcCards)
	end

	return #self.ResultCards
end

function BaseDouDiZhuCardAIResultStruct:Analysis(handCards, srcCardType, srcCards, bNewTip)
	if self.ResultCards == nil or bNewTip then
		self:CreateAIResult(handCards, srcCardType, srcCards)
	end
	
	self.Index = self.Index + 1
	local totalCount = #self.ResultCards
	
	if totalCount > 0 then
		if self.Index > totalCount then
			self.Index = 1
		end
		return self.ResultCards[self.Index]    -- 返回单个结果
		--return self.ResultCards    -- 返回所有结果
	else
		return nil
	end
end

function BaseDouDiZhuCardAIResultStruct:ResetAIResult()
	self.Index = 0
	self.ResultCards = nil
end

-----------------------------------私有方法--------------------------------------
function BaseDouDiZhuCardAIResultStruct:CreateAIResult(cards, srcCardType, srcCards)
	self.ResultCards = {}
	self.srcCardType = srcCardType
	self.srcCards = LogicUtil.SortCards(srcCards, true)
	self.handCards = LogicUtil.SortCards(cards, true)
	self.Index = 0
	
	self.struct:Analysis(self.handCards)

	if self.srcCardType == NewCardTypeEnum.NCCT_SINGLE then
		-- 单张
		self:SingleAI()
	elseif self.srcCardType == NewCardTypeEnum.NCCT_DOUBLE then
		-- 对子
		self:DoubleAI()
	elseif self.srcCardType == NewCardTypeEnum.NCCT_THREE then
		-- 三张
		self:ThreeAI()
	elseif self.srcCardType == NewCardTypeEnum.NCCT_THREE_ADD_ONE then
		-- 三带一
		self:ThreeAddOneAI()
	elseif self.srcCardType == NewCardTypeEnum.NCCT_THREE_ADD_DOUBLE then
		-- 三带一对
		self:ThreeAddDoubleAI()
	elseif self.srcCardType == NewCardTypeEnum.NCCT_SINGLE_LINE then
		-- 顺子
		self:SingleLineAI()
	elseif self.srcCardType == NewCardTypeEnum.NCCT_DOUBLE_LINE then
		-- 连对
		self:DoubleLineAI()
	elseif self.srcCardType == NewCardTypeEnum.NCCT_THREE_LINE then
		-- 飞机
		self:AirplaneAI()
	elseif self.srcCardType == NewCardTypeEnum.NCCT_THREE_LINE_ADD_SAME_ONE then
		-- 飞机 + 相同数量的单牌
		self:AirplaneAddSameOneAI()
	elseif self.srcCardType == NewCardTypeEnum.NCCT_THREE_LINE_ADD_SAME_DOUBLE then
		-- 飞机 + 相同数量的对牌
		self:AirplaneAddSameDoubleAI()
	elseif self.srcCardType == NewCardTypeEnum.NCCT_FOUR_ADD_TWO then
		-- 四带二
		self:FourAddTwoAI()
	elseif self.srcCardType == NewCardTypeEnum.NCCT_FOUR_ADD_TWO_DOUBLE then
		-- 四带两对
		self:FourAddTwoDoubleAI()
	elseif self.srcCardType == NewCardTypeEnum.NCCT_BOMB then
		-- 炸弹
		self:BombAI()
	elseif self.srcCardType == NewCardTypeEnum.NCCT_ROCKET then
		-- 火箭
		self:RocketAI()
	end
end
--------------------------------------------------------------------------------

------------------------------------AI------------------------------------------
function BaseDouDiZhuCardAIResultStruct:SingleAI()
	-- 查找单牌
	self.struct:FindCards(self.ResultCards, 1, 1, self.srcCards[1].logicValue)
	local rocketCards = {}
	self:FindRocketCards(rocketCards)
	if #rocketCards == 2 then
		self:CardsDeleteSameCard(self.ResultCards, rocketCards) -- 火箭删除
	end

	-- 查找对牌
	self.struct:FindCards(self.ResultCards, 2, 1, self.srcCards[1].logicValue)
	-- 查找三张牌
	self.struct:FindCards(self.ResultCards, 3, 1, self.srcCards[1].logicValue)
	-- 查找四张牌
	self.struct:FindCards(self.ResultCards, 4, 1, self.srcCards[1].logicValue)
	
	-- 查找炸弹
	self:AddBomb(self.ResultCards)
	-- 查找火箭
	self:AddRocket(self.ResultCards)

	-- 如果有火箭,再拆火箭
	if #rocketCards == 2 then
		for k, v in ipairs(rocketCards) do
			table.insert(self.ResultCards, v)
		end
	end
end

function BaseDouDiZhuCardAIResultStruct:DoubleAI()
	-- 查找对子
	self.struct:FindAllOnlyCards(self.ResultCards, 2, self.srcCards[1].logicValue, false)
	--self.struct:FindCards(self.ResultCards, 2, 2, self.srcCards[1].logicValue)
	--self.struct:FindCards(self.ResultCards, 3, 2, self.srcCards[1].logicValue)
	--self.struct:FindCards(self.ResultCards, 4, 2, self.srcCards[1].logicValue)

	-- 查找炸弹
	self:AddBomb(self.ResultCards)
	-- 查找火箭
	self:AddRocket(self.ResultCards)
end

function BaseDouDiZhuCardAIResultStruct:ThreeAI()
	-- 查找三张
	self.struct:FindAllOnlyCards(self.ResultCards, 3, self.srcCards[1].logicValue, false)
	--self.struct:FindCards(self.ResultCards, 3, 3, self.srcCards[1].logicValue)
	--self.struct:FindCards(self.ResultCards, 4, 3, self.srcCards[1].logicValue)

	-- 查找炸弹
	self:AddBomb(self.ResultCards)
	-- 查找火箭
	self:AddRocket(self.ResultCards)
end

function BaseDouDiZhuCardAIResultStruct:ThreeAddOneAI()
	self.srcStruct:Analysis(self.srcCards)
	local srcThree, srcCount = self.srcStruct:GetThreeCards()

	local threeResult = {}
	self.struct:FindAllOnlyCards(threeResult, 3, srcThree[1][1].logicValue)
	if #threeResult > 0 then
		local oneResult = {}
		self.struct:FindAllOnlyCards(oneResult, 1, 0, false)
	
		for k, v in ipairs(threeResult) do
			for key, value in ipairs(oneResult) do
				if v[1].logicValue ~= value[1].logicValue then
					local vTable = {}
					table.insert(vTable, v)
					local valueTable = {}
					table.insert(valueTable, value)
					self:CardsAdd_2(self.ResultCards, vTable, valueTable)
					break
				end
			end
		end
	end
	
	-- 查找炸弹
	self:AddBomb(self.ResultCards)
	-- 查找火箭
	self:AddRocket(self.ResultCards)
end

function BaseDouDiZhuCardAIResultStruct:ThreeAddDoubleAI()
	self.srcStruct:Analysis(self.srcCards)
	local srcThree, srcCount = self.srcStruct:GetThreeCards()

	local threeResult = {}
	self.struct:FindAllOnlyCards(threeResult, 3, srcThree[1][1].logicValue)
	if #threeResult > 0 then
		local doubleResult = {}
		self.struct:FindAllOnlyCards(doubleResult, 2, 0, false)
	
		for k, v in ipairs(threeResult) do
			for key, value in ipairs(doubleResult) do
				if v[1].logicValue ~= value[1].logicValue then
					local vTable = {}
					table.insert(vTable, v)
					local valueTable = {}
					table.insert(valueTable, value)
					self:CardsAdd_2(self.ResultCards, vTable, valueTable)
					break
				end
			end
		end
	end
	
	-- 查找炸弹
	self:AddBomb(self.ResultCards)
	-- 查找火箭
	self:AddRocket(self.ResultCards)
end

function BaseDouDiZhuCardAIResultStruct:SingleLineAI()
	self.srcStruct:Analysis(self.srcCards)
	local srcOne, srcCount = self.srcStruct:GetSingleCards()
	
	local oneResult = {}
	self.struct:FindAllOnlyCards(oneResult, 1)
	self:FindLine(self.ResultCards, oneResult, srcCount, srcOne[1][1].logicValue)

	-- 查找炸弹
	self:AddBomb(self.ResultCards)
	-- 查找火箭
	self:AddRocket(self.ResultCards)
end

function BaseDouDiZhuCardAIResultStruct:DoubleLineAI()
	self.srcStruct:Analysis(self.srcCards)
	local srcDouble, srcCount = self.srcStruct:GetDoubleCards()
	
	local doubleResult = {}
	self.struct:FindAllOnlyCards(doubleResult, 2)
	self:FindLine(self.ResultCards, doubleResult, srcCount / 2, srcDouble[1][1].logicValue)
	
	-- 查找炸弹
	self:AddBomb(self.ResultCards)
	-- 查找火箭
	self:AddRocket(self.ResultCards)
end

function BaseDouDiZhuCardAIResultStruct:AirplaneAI()
	self.srcStruct:Analysis(self.srcCards)
	local srcThree, srcCount = self.srcStruct:GetThreeCards()

	local threeCards, myCount = self.struct:GetThreeCards()
	self:FindLine(self.ResultCards, threeCards, srcCount / 3, srcThree[1][1].logicValue)

	-- 查找炸弹
	self:AddBomb(self.ResultCards)
	-- 查找火箭
	self:AddRocket(self.ResultCards)
end

function BaseDouDiZhuCardAIResultStruct:AirplaneAddSameOneAI()
	self.srcStruct:Analysis(self.srcCards)
	local srcThree = {}
	self.srcStruct:FindAllOnlyCards(srcThree, 3)
	local srcLineResult = self:FindFirstLine(srcThree)

	local airplaneLen = #srcLineResult[1] / 3
	
	local threeResult = {}
	self.struct:FindAllOnlyCards(threeResult, 3)
	
	if #threeResult >= airplaneLen then
		local lineResult = {}
		self:FindLine(lineResult, threeResult, airplaneLen, srcThree[1][1].logicValue)

		if #lineResult >= 0 then
			local allResult, allCount = self.struct:GetAllCards()
	
			for k, v in ipairs(lineResult) do
				local tempLine = {}
				table.insert(tempLine, v)
				local divideLine = self:CardsDivede_1(tempLine)
		
				local copyAllResult = ShallowCopy(allResult)
				self:CardsDeleteSameCard(copyAllResult, divideLine)
				if #copyAllResult >= airplaneLen then
					local tempTable = {}
					for i = 1, airplaneLen do
						table.insert(tempTable, copyAllResult[i][1])
					end
			
					local isInsert = true
					if airplaneLen == 2 then
						-- 连续的四张不合法
						local sameNum = 0
						local cardsDivedeNoSame = self:CardsDivede_2(tempLine)
						for i_index = 1, 2 do
							for j_index = 1, 2 do
								if cardsDivedeNoSame[i_index][1].logicValue == tempTable[j_index].logicValue then
									sameNum = sameNum + 1
								end
							end
						end

						if sameNum == 2 then
							if #copyAllResult >= 3 then
								tempTable[2] = copyAllResult[3][1]
							else
								isInsert = false
							end
						end
					end

					if isInsert then
						local vTable = {}
						table.insert(vTable, v)
						local valueTable = {}
						table.insert(valueTable, tempTable)
						self:CardsAdd_2(self.ResultCards, vTable, valueTable)
					end
				end
			end
		end
	end

	-- 查找炸弹
	self:AddBomb(self.ResultCards)
	-- 查找火箭
	self:AddRocket(self.ResultCards)
end

function BaseDouDiZhuCardAIResultStruct:AirplaneAddSameDoubleAI()
	self.srcStruct:Analysis(self.srcCards)
	local srcThree = {}
	self.srcStruct:FindAllOnlyCards(srcThree, 3)
	local srcLineResult = self:FindFirstLine(srcThree)

	local airplaneLen = #srcLineResult[1] / 3
	
	local threeResult = {}
	self.struct:FindAllOnlyCards(threeResult, 3)
	
	if #threeResult >= airplaneLen then
		local lineResult = {}
		self:FindLine(lineResult, threeResult, airplaneLen, srcThree[1][1].logicValue)

		if #lineResult > 0 then
			local doubleCards, doubleCount = self.struct:GetAllDoubleCards()
			if #doubleCards >= airplaneLen then
				for k, v in ipairs(lineResult) do
					local tempLine = {}
					table.insert(tempLine, v)
					local divideLine = self:CardsDivede_2(tempLine)
					local copyDoubleResult = ShallowCopy(doubleCards)
					local newDoubleResult = self:CardsDeleteSameCard_2(copyDoubleResult, divideLine)
					if #newDoubleResult >= airplaneLen then
						local doubleCombin = self:CardsCombin(newDoubleResult, airplaneLen)
						local valueTable = {}
						table.insert(valueTable, doubleCombin[1])
						self:CardsAdd_2(self.ResultCards, tempLine, valueTable)
					end
				end
			end
		end
	end
	
	-- 查找炸弹
	self:AddBomb(self.ResultCards)
	-- 查找火箭
	self:AddRocket(self.ResultCards)
end

function BaseDouDiZhuCardAIResultStruct:FourAddTwoAI()
	self.srcStruct:Analysis(self.srcCards)
	local srcFour, srcCount = self.srcStruct:GetFourCards()

	local fourResult = {}
	self.struct:FindAllOnlyCards(fourResult, 4, srcFour[1][1].logicValue)
	if #fourResult > 0 then
		local allResult, allCount = self.struct:GetAllCards()

		for k, v in ipairs(fourResult) do
			local twoTable= {}
			for key, value in ipairs(allResult) do
				if v[1].logicValue ~= value[1].logicValue then
					table.insert(twoTable, value[1])
				end
				
				if #twoTable == 2 then
					break
				end
			end
			if #twoTable == 2 then
				local vTable = {}
				table.insert(vTable, v)
				local valueTable = {}
				table.insert(valueTable, twoTable)
				self:CardsAdd_2(self.ResultCards, vTable, valueTable)
			end
		end
	end

	-- 查找炸弹
	self:AddBomb(self.ResultCards)
	-- 查找火箭
	self:AddRocket(self.ResultCards)
end

function BaseDouDiZhuCardAIResultStruct:FourAddTwoDoubleAI()
	self.srcStruct:Analysis(self.srcCards)
	local srcFour, srcCount = self.srcStruct:GetFourCards()
	
	local FourResult = {}
	self.struct:FindAllOnlyCards(FourResult, 4, srcFour[#srcFour][1].logicValue)
	if #FourResult > 0 then
		local doubleCards, doubleCount = self.struct:GetAllDoubleCards()
		
		if #doubleCards >= 2 then
			for k, v in ipairs(FourResult) do
				local vTable = {}
				table.insert(vTable, v)
				local newDouleCards = self:CardsDeleteSameCard_2(doubleCards, vTable)
				local combinDoubleCards = self:CardsCombin(newDouleCards)
			
				if #combinDoubleCards > 0 then
					local twoDoubleTable = nil
			
					for key, value in ipairs(combinDoubleCards) do
						if value[1].logicValue == value[2].logicValue and
						value[2].logicValue == value[3].logicValue and
						value[3].logicValue == value[4].logicValue then
							if value[1].logicValue - v[1].logicValue ~= 1 and
							v[1].logicValue - value[1].logicValue ~= 1 then
								twoDoubleTable = value
							end 
						else
							twoDoubleTable = value
						end
					end

					if twoDoubleTable ~= nil then
						local vTable = {}
						table.insert(vTable, v)
						local valueTable = {}
						table.insert(valueTable, twoDoubleTable)
						self:CardsAdd_2(self.ResultCards, vTable, valueTable)
					end
				end
			end
		end
	end
	
	-- 查找炸弹
	self:AddBomb(self.ResultCards)
	-- 查找火箭
	self:AddRocket(self.ResultCards)
end

function BaseDouDiZhuCardAIResultStruct:BombAI()
	self.struct:FindAllOnlyCards(self.ResultCards, 4, self.srcCards[1].logicValue)
	-- 查找火箭
	self:AddRocket(self.ResultCards)
end

function BaseDouDiZhuCardAIResultStruct:RocketAI()
	-- 呵呵，人家最大
end

function BaseDouDiZhuCardAIResultStruct:AddBomb(result)
	self.struct:FindAllOnlyCards(result, 4, 0, false)
end

function BaseDouDiZhuCardAIResultStruct:AddRocket(result)
	local rocket = {}
	local cards, count = self.struct:GetSingleCards()
	for k, v in ipairs(cards) do
		if v[1]:IsKing() then
			table.insert(rocket, v[1])
		end
	end

	if #rocket == 2 then
		table.insert(result, rocket)
	end
end

function BaseDouDiZhuCardAIResultStruct:FindRocketCards(result)
	local cards, count = self.struct:GetSingleCards()
	for k, v in ipairs(cards) do
		if v[1]:IsKing() then
			local temp = {}
			table.insert(temp , v[1])
			table.insert(result, temp)
		end
	end
end
--------------------------------------------------------------------------------

-------------------------------私有方法------------------------------------------
-- cards_1 = {{1,1},{2,2}}
-- cards_2 = {{3},{4}}
-- 结果 {{1,1,3},{1,1,4},{2,2,3}，{2,2,4}}
-- 注意 cards_1 与 cards_2 中的每个table里元素数量要相同
function BaseDouDiZhuCardAIResultStruct:CardsAdd(result, cards_1, cards_2)
	for k, v in ipairs(cards_1) do
		for key, value in ipairs(cards_2) do
			if v[1].logicValue ~= value[1].logicValue then
				local temp = ShallowCopy(v)
				for kk, vv in ipairs(value) do
					table.insert(temp, vv)
				end
				table.insert(result, temp)
			end
		end
	end
end

-- 不进行相同的判断
function BaseDouDiZhuCardAIResultStruct:CardsAdd_2(result, cards_1, cards_2)
	for k, v in ipairs(cards_1) do
		for key, value in ipairs(cards_2) do
			local temp = ShallowCopy(v)
			for kk, vv in ipairs(value) do
				table.insert(temp, vv)
			end
			table.insert(result, temp)
		end
	end
end

-- cards = {{1, ...}, {2, ...}, {3, ...}, {4, ...}}
-- len : 多少长度的line
-- smallNum : 要大于的最小的牌
-- 在 cards 中找到指定长度的牌
function BaseDouDiZhuCardAIResultStruct:FindLine(result, cards, len, smallNum)
	local curLen = 0
	for i = 1, #cards do
		local i_card = cards[i][1]
		if not i_card:IsCanLine() then
			break
		end
		if i_card.logicValue > smallNum then
			if i + len - 1 <= #cards then
				local canInsert = true
				for j = 1, len - 1 do
					local j_card = cards[i + j][1]
					if j_card.logicValue - i_card.logicValue ~= j or
					   not j_card:IsCanLine() then
						canInsert = false
						break
					end
				end

				if canInsert then
					local tempResult = ShallowCopy(cards[i])
					local addCard = {}
					for j = 1, len - 1 do
						local list = cards[i + j]
						for k, v in ipairs(list) do
							table.insert(tempResult, v)
						end
					end
					table.insert(result, tempResult)
				end
			else
				break
			end
		end
	end
end

-- 获取cards中第一个line的
function BaseDouDiZhuCardAIResultStruct:FindFirstLine(cards)
	local sIndex, eIndex = 1, 1
	for i = 1, #cards - 1 do
		local iCard = cards[i]
		local i_1_Card = cards[i + 1]
		if iCard[1]:IsCanLine() and i_1_Card[1]:IsCanLine() and
		   i_1_Card[1].logicValue - iCard[1].logicValue == 1 then
			eIndex = eIndex + 1
		else
			if eIndex - sIndex > 0 then
				break
			else
				sIndex = i + 1
				eIndex = i + 1
			end
		end
	end

	local result = {}
	if eIndex ~= sIndex then
		local temp = {}
		for i = sIndex, eIndex do
			for k, v in ipairs(cards[i]) do
				table.insert(temp, v)
			end
		end
		table.insert(result, temp)
	end
	return result
end

--cards   :{{1,1}, {2, 2}, {3, 3}}
--result  :{{1,1,2,2},{1,1,3,3},{2,2,3,3}}
function BaseDouDiZhuCardAIResultStruct:CardsCombin(cards)
	local result = {}
	if #cards < 2 then
		return result
	end
	
	for i = 1, #cards - 1 do
		for j = i + 1, #cards do
			local temp = ShallowCopy(cards[i])
			for k, v in ipairs(cards[j]) do
				table.insert(temp, v)
			end
			table.insert(result, temp)
		end
	end

	return result
end

-- cards = {{1}, {2}, {3}, {4}}
-- num = 3
-- result : {{1,2,3}, {1,2,4},{2,3,4}}
function BaseDouDiZhuCardAIResultStruct:CardsCombin_2(cards, num)
	local result = {}
	if cards == nil then
		return result
	end
	
	if num > #cards then
		return result
	end

	result = self:Combin(cards, 1, num)
	return result
end

function BaseDouDiZhuCardAIResultStruct:Combin(cards, start, num)
	local result = {}
	for i = start, #cards - num + 1 do
		if num == 1 then
			table.insert(result, cards[i])
		else
			local tempResult = self:Combin(cards, i + 1, num - 1)
			local list = {}
			table.insert(list, cards[i])
			self:CardsAdd(result, list, tempResult)
		end
	end

	return result
end

-- cards_1 :{{1,1},{2,2},{3,3},{4,4}}
-- cards_2 :{{3,3,3},{4,4,4},{5,5,5}}
-- result  :{{1,1},{2,2}} 
function BaseDouDiZhuCardAIResultStruct:CardsDeleteSameCard(cards_1, cards_2)
	if #cards_1 == 0 or #cards_2 == 0 then
		return
	end
	
	for k, v in ipairs(cards_2) do
		local deleteCard = v[1]    -- 想要删除的元素
		local deletePos = 0
		for key, value in ipairs(cards_1) do
			local card = value[1]
			if deleteCard.color == card.color and
			   deleteCard.logicValue == card.logicValue then
				-- 找到了
				deletePos = key
				break
			end
		end
		
		if deletePos ~= 0 then
			table.remove(cards_1, deletePos)	
		end
	end
end

-- 不判断花色
function BaseDouDiZhuCardAIResultStruct:CardsDeleteSameCard_2(cards_1, cards_2)
	if #cards_1 == 0 or #cards_2 == 0 then
		return {}
	end
	
	local result = ShallowCopy(cards_1)
	for k, v in ipairs(cards_2) do
		local deleteCard = v[1]    -- 想要删除的元素
		local deletePos = {}
		for key, value in ipairs(result) do
			local card = value[1]
			if deleteCard.logicValue == card.logicValue then
				-- 找到了
				table.insert(deletePos, key)
			end
		end
		
		for i = #deletePos, 1, -1 do
			table.remove(result, deletePos[i])
		end
	end

	return result
end

-- cards  : {{2, 2, 3, 3}, {4, 4, 5, 5}}
-- result : {{2}, {2}, {3}, {3}, {4}, {4}, {5}, {5}}
function BaseDouDiZhuCardAIResultStruct:CardsDivede_1(cards)
	local result = {}
	for k, v in ipairs(cards) do
		for key, value in ipairs(v) do
			local card = {}
			table.insert(card, value)
			table.insert(result, card)
		end
	end
	return result
end

-- cards  : {{2, 2, 3, 3}, {4, 4, 5, 5}}
-- result : {{2}, {3}, {4}, {5}}
-- 会根据card 的 logicValue 去重
function BaseDouDiZhuCardAIResultStruct:CardsDivede_2(cards)
	local cardStruct = {}
	for k, v in ipairs(cards) do
		for key, value in ipairs(v) do
			if cardStruct[value.logicValue] == nil then
				cardStruct[value.logicValue] = {}
			end
			table.insert(cardStruct[value.logicValue], value)
		end
	end
	
	local result = {}
	for k, v in pairs(cardStruct) do
		local card = {}
		table.insert(card, v[1])
		table.insert(result, card)
	end
	return result
end
--------------------------------------------------------------------------------

return BaseDouDiZhuCardAIResultStruct