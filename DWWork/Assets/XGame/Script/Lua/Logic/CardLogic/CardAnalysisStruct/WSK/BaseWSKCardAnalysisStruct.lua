--------------------------------------------------------------------------------
-- 	 File      : BaseWSKCardAnalysisStruct.lua
--   author    : guoliang
--   function   : 510K手牌分析结构基类
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

BaseWSKCardAnalysisStruct = class("BaseWSKCardAnalysisStruct", nil)


function BaseWSKCardAnalysisStruct:ctor()

end

function BaseWSKCardAnalysisStruct:Reset()
	self.singleCount = 0
	self.doubleCount = 0
	self.threeCount = 0
	self.fourCount = 0
	self.fiveCount = 0
	self.sixCount = 0
	self.sevenCount = 0
	self.eightCount = 0

	self.ftkCount = 0
	self.ftkBigCount = 0
	self.kingCount = 0-- 特殊对待，king的数量

--cards 互斥
	self.singleCards = {}   --{{1},{2},{3}}
	self.doubleCards = {} -- {{1,1},{2,2}}
	self.threeCards = {} --{{1,1,1},{2,2,2}}
	self.fourCards = {} --{{1,1,1,1},{2,2,2,2}}
	self.fiveCards = {}
	self.sixCards = {}
	self.sevenCards = {}
	self.eightCards = {}

	self.ftkCards = {}--特殊对待，510k与其他cards不互斥 {{5,10,k},{5,10,k}}
	self.ftkBigCards = {}--特殊对待，正510k与其他cards不互斥 {{5,10,k},{5,10,k}}

	self.kingCards = {}-- 特殊对待，king与其他cards不互斥 {k,k,k}

	self.isSameColor = false
end

--生成牌组结构
function BaseWSKCardAnalysisStruct:Analysis(cards)--参数 CCard 列表
	self:Reset()
	local cardCount = #cards
	local sorCards = ShallowCopy(cards)--因为会排序，会影响到传入的数组顺序
	LogicUtil.SortCards(sorCards,true)--升序排列
	
	local sameNum = 0
	local sameColor = true
	local ftkCards = {}
	local sameCards = {}
	local lastCardColor = 0
	local i = 1
	while i <= cardCount do
		sameNum = 0
		sameCards = {}
		local v = sorCards[i]
		if sameColor and lastCardColor > 0 and not v:IsSameColor(lastCardColor) then
			sameColor = false
		end
		lastCardColor = v.color

		sameNum = sameNum + 1
		table.insert(sameCards,v)
		--510K判断
		if v.logicValue == 5 or v.logicValue == 10 or v.logicValue == 13 then
			table.insert(ftkCards,v)
		elseif v.logicValue == 16 or v.logicValue == 17 then -- king 判断
			table.insert(self.kingCards,v)
			self.kingCount = self.kingCount + 1
		end

		-- 正常判断
		for j = i+1,cardCount do
			if v.logicValue == sorCards[j].logicValue then
--				print("sorCards[j].logicValue ".. sorCards[j].logicValue .." ID "..sorCards[j].ID)
				--510K判断
				if v.logicValue == 5 or v.logicValue == 10 or v.logicValue == 13 then
					table.insert(ftkCards,sorCards[j])
				elseif v.logicValue == 16 or v.logicValue == 17 then -- king 判断
					table.insert(self.kingCards,sorCards[j])
					self.kingCount = self.kingCount + 1
				end
				
				sameNum = sameNum + 1
				table.insert(sameCards,sorCards[j])
			else
				break
			end
		end
		self:_Add(sameCards,sameNum)
		i = i + sameNum
	end
	self.isSameColor = sameColor
	--特殊牌型
	self:Check510k(ftkCards)
	self:Check510k_Big(ftkCards)
end



--检查副510K
function BaseWSKCardAnalysisStruct:Check510k(selectCards)
	local oneTable = {}  -- maby be{{5,10,k},{5,10},{10}}
	local fiveIndex = 1
	local tenIndex = 1
	local kIndex = 1

	for k,v in pairs(selectCards) do
		if v.logicValue == 5 then
			if oneTable[fiveIndex] == nil then
				oneTable[fiveIndex] = {}
			end

			table.insert(oneTable[fiveIndex],v)
			fiveIndex = fiveIndex + 1

		elseif v.logicValue == 10 then
			if oneTable[tenIndex] == nil then
				oneTable[tenIndex] = {}
			end

			table.insert(oneTable[tenIndex],v)
			tenIndex = tenIndex + 1
		elseif v.logicValue == 13 then
			if oneTable[kIndex] == nil then
				oneTable[kIndex] = {}
			end

			table.insert(oneTable[kIndex],v)
			kIndex = kIndex + 1
		end
	end
	local sum = 0
	for k,v in pairs(oneTable) do
		sum = 0
		local str = ""
		for k1,v1 in pairs(v) do
			sum = sum + v1.logicValue
			str = str .. " "..v1.ID
		end
		if sum == 28 then -- 5+10+13
			self.ftkCount = self.ftkCount + 1
			table.insert(self.ftkCards,v)
		end
	end
end

-- 检查正510K
function BaseWSKCardAnalysisStruct:Check510k_Big(selectCards)
	local compareFunc = function (card1,card2)
		return card1.ID < card2.ID
	end
	table.sort(selectCards,compareFunc)
	local cardColor = 0
	local tempTable = {}--{1={{5,10,k},{5,10}},2={5,k}}
	local fiveIndex = 1
	local tenIndex = 1
	local kIndex = 1
	for k,v in pairs(selectCards) do
		if cardColor ~= v.color then
			cardColor = v.color
			tempTable[v.color] = {}
			fiveIndex = 1
			tenIndex = 1
			kIndex = 1
		end
		if v.logicValue == 5 then
			if tempTable[v.color][fiveIndex] == nil then
				tempTable[v.color][fiveIndex] = {}
			end
			table.insert(tempTable[v.color][fiveIndex],v)
			fiveIndex = fiveIndex + 1

		elseif v.logicValue == 10 then
			if tempTable[v.color][tenIndex] == nil then
				tempTable[v.color][tenIndex] = {}
				
			end
			table.insert(tempTable[v.color][tenIndex],v)
			tenIndex = tenIndex + 1
		elseif v.logicValue == 13 then
			if tempTable[v.color][kIndex] == nil then
				tempTable[v.color][kIndex] = {}
				
			end
			table.insert(tempTable[v.color][kIndex],v)
			kIndex = kIndex + 1
		end
	end
	local sum =0
	for k,v in pairs(tempTable) do
--		print("color = " ..k)
		for k_c,v_c in pairs(v) do
--			print("array index " .. k_c)
			sum = 0
			local str = ""
			for k1,v1 in pairs(v_c) do
				sum = sum + v1.logicValue
				str = str .. " "..v1.ID
			end
--			print("Check510k_Big ".. str)
			if sum == 28 then -- 5+10+k

				self.ftkBigCount = self.ftkBigCount + 1
				table.insert(self.ftkBigCards,v_c)
			end
		end
	end
end

--添加对应牌数
function  BaseWSKCardAnalysisStruct:_Add(cards,cardNum)
	if cards == nil or cardNum == 0 then
		return
	end
	local tempCards = {}

	if cardNum == 1 then
		self.singleCount = self.singleCount + 1
		table.insert(self.singleCards,cards)
	elseif cardNum == 2 then
		self.doubleCount = self.doubleCount + 1
		table.insert(self.doubleCards,cards)
	elseif cardNum == 3 then
		self.threeCount = self.threeCount + 1
		table.insert(self.threeCards,cards)
	elseif cardNum == 4 then
		self.fourCount = self.fourCount + 1
		table.insert(self.fourCards,cards)
	elseif cardNum == 5 then
		self.fiveCount = self.fiveCount + 1
		table.insert(self.fiveCards,cards)
	elseif cardNum == 6 then
		self.sixCount = self.sixCount + 1
		table.insert(self.sixCards,cards)
	elseif cardNum == 7 then
		self.sevenCount = self.sevenCount + 1
		table.insert(self.sevenCards,cards)
	else
		self.eightCount = self.eightCount + 1
		table.insert(self.eightCards,cards)
	end
end

