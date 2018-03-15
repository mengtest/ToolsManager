--------------------------------------------------------------------------------
-- 	 File       : CardsStruct.lua
--   author     : zhanghaochun
--   function   : 斗地主分析结构基类
--   date       : 2017-01-04
--   copyright  : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------

local CardsStruct = class("CardsStruct")

function CardsStruct:ctor()
	-- 排序规则,升序
	self.SCompareFunc = function(cards1, cards2)
		if cards1[1].logicValue < cards2[1].logicValue then
			return true
		elseif cards1[1].logicValue == cards2[1].logicValue then
			return cards1[1].ID > cards2[1].ID
		else
			return false
		end
	end
end

function CardsStruct:Reset()
	self.TotalCount = 0
	
	self.AllCards = {}       -- {{1}，{2}，{3}, {4}}
	self.SingleCards = {}    --{{1}，{2}，{3}}
	self.DoubleCards = {}    --{{1,1}，{2,2}，{3,3}}
	self.ThreeCards = {}     --{{1,1,1},{2,2,2}，{3,3,3}}
	self.FourCards = {}      --{{1,1,1,1},{2,2,2,2}，{3,3,3,3}}
end

--生成牌组结构
function CardsStruct:Analysis(cards)
	self:Reset()
	if cards == nil then return end

	self.TotalCount = #cards

	for k, v in ipairs(cards) do
		local temp = {}
		table.insert(temp, v)
		table.insert(self.AllCards, temp)
	end
	local tempResult = self:StartGroup(cards)
	
	-- 插入到结果组里
	for k, v in pairs(tempResult) do
		local count = #v
		if count == 1 then
			table.insert(self.SingleCards, v)
		elseif count == 2 then
			table.insert(self.DoubleCards, v)
		elseif count == 3 then
			table.insert(self.ThreeCards, v)
		elseif count == 4 then
			table.insert(self.FourCards, v)
		end
	end

	 -- 进行排序(升序)
	local CompareCards = function(cards1, cards2)
		if cards1[1].logicValue < cards2[1].logicValue then
			return true
		elseif cards1[1].logicValue == cards2[1].logicValue then
			return cards1[1].ID > cards2[1].ID
		else
			return false
		end
	end

	table.sort(self.SingleCards, CompareCards)
	table.sort(self.DoubleCards, CompareCards)
	table.sort(self.ThreeCards, CompareCards)
	table.sort(self.FourCards, CompareCards)
end

function CardsStruct:GetTotalCount()
	return self.TotalCount
end

function CardsStruct:GetSingleCards()
	return self.SingleCards, #self.SingleCards
end

function CardsStruct:GetDoubleCards()
	return self.DoubleCards, #self.DoubleCards * 2
end

function CardsStruct:GetThreeCards()
	return self.ThreeCards, #self.ThreeCards * 3
end

function CardsStruct:GetFourCards()
	return self.FourCards, #self.FourCards * 4
end

function CardsStruct:GetAllCards()
	return self.AllCards, #self.AllCards
end

-- 根据单张, 对子, 三张, 四张的顺序得到所有的牌
--n_type : 包含哪些牌
function CardsStruct:GetAllCards_2(n_type)
	local result = {}

	if n_type >= 1 then
		self:GetCardInCardsList(result, self.SingleCards)
	end

	if n_type >= 2 then
		self:GetCardInCardsList(result, self.DoubleCards)
	end

	if n_type >= 3 then
		self:GetCardInCardsList(result, self.ThreeCards)
	end

	if n_type >= 4 then
		self:GetCardInCardsList(result, self.FourCards)
	end
	
	return result, #result
end

function CardsStruct:GetAllDoubleCards()
	local result = {}
	local pass = false
	local cards = self.AllCards
	for i = 1, #cards - 1 do
		if not pass then
			if cards[i][1].logicValue == cards[i + 1][1].logicValue then
				local temp = {}
				table.insert(temp, cards[i][1])
				table.insert(temp, cards[i + 1][1])
				table.insert(result, temp)
				pass = true
			end
		else
			pass = false
		end
	end
	return result, #result * 2
end

-- 得到牌里所有唯一的指定数量的牌(比如有4个2，但只有一个2会放进去)
-- result         :所有的结果都放入result中
-- num            :插入result中牌组中牌的数量
-- biggerThanNum  :结果中的牌大于指定的值
-- isSort         :是否排序
function CardsStruct:FindAllOnlyCards(result, num, biggerThanNum, isSort)
	if num <= 1 then
		self:FindCards(result, 1, num, biggerThanNum)
	end
	
	if num <= 2 then
		self:FindCards(result, 2, num, biggerThanNum)
	end

	if num <= 3 then
		self:FindCards(result, 3, num, biggerThanNum)
	end

	if num <= 4 then
		self:FindCards(result, 4, num, biggerThanNum)
	end

	if isSort == nil then isSort = true end
	
	if isSort then
		local CompareCards = function(cards1, cards2)
			if cards1[1].logicValue < cards2[1].logicValue then
				return true
			elseif cards1[1].logicValue == cards2[1].logicValue then
				return cards1[1].ID > cards2[1].ID
			else
				return false
			end
		end
		table.sort(result, CompareCards)
	end
end

-- 得到牌组
-- result         : 所有的结果都放入result中
-- cardsTableType : 参数说明请见GetCardsListByType函数
-- num            : 插入result中牌组中牌的数量
-- biggerThanNum  : 结果中的牌大于指定的值
function CardsStruct:FindCards(result, cardsTableType, num,	biggerThanNum)
	if num < 1 or num > 4 then return end

	biggerThanNum = biggerThanNum or 0
	local cardsTable = self:GetCardsListByType(cardsTableType)
	for k, v in ipairs(cardsTable) do
		if #v >= num and v[1].logicValue > biggerThanNum then
			local temp = {}
			for i = 1, num do
				table.insert(temp, v[i])
			end
			table.insert(result, temp)
		end
	end
end

-- 弃用
function CardsStruct:CompareFunc(cards1, cards2)
	if cards1[1].logicValue < cards2[1].logicValue then
		return true
	elseif cards1[1].logicValue == cards2[1].logicValue then
		return cards1[1].ID > cards2[1].ID
	else
		return false
	end
end


-------------------------------私有方法------------------------------------------
-- 进行插入分组
function CardsStruct:StartGroup(cards)
	local result = {}
	for k, v in ipairs(cards) do
		if result[v.logicValue] == nil then
			result[v.logicValue] = {}
		end
		table.insert(result[v.logicValue], v)
	end

	return result
end

-- 根据cardsTableType得到相应的cardsList
-- 1:SingleCards   2:DoubleCards   3:ThreeCards   4:FourCards
function CardsStruct:GetCardsListByType(cardsTableType)
	if cardsTableType == 1 then
		return self.SingleCards
	elseif cardsTableType == 2 then
		return self.DoubleCards
	elseif cardsTableType == 3 then
		return self.ThreeCards
	elseif cardsTableType == 4 then
		return self.FourCards
	else
		return {}
	end
end

-- cards : {{2,2}， {3,3}}
-- result : {{2}, {2}, {3}, {3}}
function CardsStruct:GetCardInCardsList(result, cards)
	if type(result) ~= "table" then return end

	for k, v in ipairs(cards) do
		for key, value in ipairs(v) do
			local package = {}
			table.insert(package, value)
			table.insert(result, package)
		end
	end
end
--------------------------------------------------------------------------------

return CardsStruct