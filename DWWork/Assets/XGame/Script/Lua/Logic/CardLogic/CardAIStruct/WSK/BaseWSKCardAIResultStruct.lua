--------------------------------------------------------------------------------
-- 	 File      : BaseWSKCardAIResultStruct.lua
--   author    : guoliang
--   function   : 手牌提示出牌结果结构
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

BaseWSKCardAIResultStruct = class("BaseWSKCardAIResultStruct", nil)


function BaseWSKCardAIResultStruct:ctor()
	self:BaseCtor()
end
function BaseWSKCardAIResultStruct:BaseCtor()
	self.analysisStruct = nil
	self.tipIndex = 0 -- 提示牌组顺序
	self.introduceCards = nil
	self.introduceCardKeys = nil
end

function BaseWSKCardAIResultStruct:Reset()
	
end

--重置AI结果
function BaseWSKCardAIResultStruct:ResetAIResult()
	self.introduceCards = nil
	self.introduceCardKeys = nil
end

--添加一组提示牌组
function BaseWSKCardAIResultStruct:AddIntroduceTips(cards)
	local key = ""
	for k,v in pairs(cards) do
		key = key.."_"..v.ID
	end
	if key ~= "" then
--		DwDebug.LogError("AddIntroduceTips "..key)
		if self.introduceCards[key] == nil then
			table.insert(self.introduceCardKeys,key)
			self.introduceCards[key] = cards
		end
	end
end


--获取提示card组:每次调用，返回一组提示牌
function BaseWSKCardAIResultStruct:Analysis(handCards,srcCardType,srcCards)--参数 CCard 列表
	if self.introduceCards == nil or self.introduceCardKeys == nil then
		self:CreateAIResult(handCards,srcCardType,srcCards)
	end
	self.tipIndex = self.tipIndex + 1
	local introCount = #self.introduceCardKeys
	
	if introCount > 0 then
		if self.tipIndex > introCount then
			self.tipIndex = 1
		end
		return self.introduceCards[self.introduceCardKeys[self.tipIndex]]
	else
		return nil
	end
end

--创建AI提示牌组列表
function  BaseWSKCardAIResultStruct:CreateAIResult(cards,srcCardType,srcCards)
--	DwDebug.LogError("CreateAIResult handCardCount ="..#cards .. "srcCardType ="..srcCardType .. "srcCardsCount ="..#srcCards)
	self.introduceCards = {}
	self.introduceCardKeys = {}
	self.analysisStruct = BaseWSKCardAnalysisStruct:New()
	self.analysisStruct:Analysis(cards)
	self.srcCardAnalysisStruct = BaseWSKCardAnalysisStruct:New()
	self.srcCardAnalysisStruct:Analysis(srcCards)

	self.srcCardType = srcCardType
	self.srcCards = srcCards
	self.handCards = ShallowCopy(cards)
	LogicUtil.SortCards(self.handCards,true)
	self.tipIndex = 0

	if srcCardType == CardTypeEnum.CT_SINGLE then
		self:SingleAI()
	elseif srcCardType == CardTypeEnum.CT_DOUBLE or srcCardType == CardTypeEnum.CT_DOUBLE_SAME_KING then
		self:DoubleAI()
	elseif srcCardType == CardTypeEnum.CT_THREE or srcCardType == CardTypeEnum.CT_THREE_LINE_TAKE_ONE 
		or srcCardType == CardTypeEnum.CT_THREE_LINE_TAKE_TWO then
		self:ThreeAI()
	elseif srcCardType == CardTypeEnum.CT_SINGLE_LINE then
		self:SingleLineAI()
	elseif srcCardType == CardTypeEnum.CT_DOUBLE_LINE then
		self:DoubleLineAI()
	elseif srcCardType == CardTypeEnum.CT_THREE_LINE then
		self:ThreeLineAI()
	elseif srcCardType == CardTypeEnum.CT_FIVE_TEN_K or srcCardType == CardTypeEnum.CT_FIVE_TEN_K_BIG then
		self:FiveTenKAI()
	elseif srcCardType >= CardTypeEnum.CT_BOMB_FOUR then
		self:BombAI()
	end
	return #self.introduceCardKeys
end

-- 用于基本类型时的炸弹查询
function BaseWSKCardAIResultStruct:BaseBombCheck()
--	print("BaseBombCheck")
	local result = self.analysisStruct
	-- 5 10 k
--	print("ftkCount = "..result.ftkCount)
	if result.ftkCount > 0 then
		local ftkArray = {}
		for k,v in pairs(result.ftkCards) do
			if self.srcCardType < CardTypeEnum.CT_FIVE_TEN_K then
				self:AddIntroduceTips(v)
				break
			end
		end
	end
	-- 正5 10 k
--	print("ftkBigCount = "..result.ftkBigCount)
	if result.ftkBigCount > 0 then
		for k,v in pairs(result.ftkBigCards) do
			if self.srcCardType < CardTypeEnum.CT_FIVE_TEN_K_BIG then
				self:AddIntroduceTips(v)
				break
			end
		end
	end

	-- bomb
	if result.fourCount > 0 then
		for k,v in pairs(result.fourCards) do
			if self.srcCardType == CardTypeEnum.CT_BOMB_FOUR then
				if v[1].logicValue > self.srcCards[1].logicValue then
					self:AddIntroduceTips(v)
				end
			elseif self.srcCardType < CardTypeEnum.CT_BOMB_FOUR then
				self:AddIntroduceTips(v)
			end
		end
	end

	if result.fiveCount > 0 then
		for k,v in pairs(result.fiveCards) do
			if self.srcCardType == CardTypeEnum.CT_BOMB_FIVE then
				if v[1].logicValue > self.srcCards[1].logicValue then
					self:AddIntroduceTips(v)
				end
			elseif self.srcCardType < CardTypeEnum.CT_BOMB_FIVE then
				self:AddIntroduceTips(v)
			end
		end
	end

		-- 三幅 5 10 K
	if result.ftkCount >= 3 then
		if self.srcCardType < CardTypeEnum.CT_THREE_FIVE_TEM_K then
			local tempList = {}
			for k,v in pairs(result.ftkCards) do
				if k <= 3 then
					for k_ele,v_ele in pairs(v) do
						table.insert(tempList,v_ele)
					end
				else
					break
				end
			end
			self:AddIntroduceTips(tempList)
		end
	end

	if result.sixCount > 0 then
		for k,v in pairs(result.sixCards) do
			if self.srcCardType == CardTypeEnum.CT_BOMB_SIX then
				if v[1].logicValue > self.srcCards[1].logicValue then
					self:AddIntroduceTips(v)
				end
			elseif self.srcCardType < CardTypeEnum.CT_BOMB_SIX then
				self:AddIntroduceTips(v)
			end
		end
	end

	-- 三王
--	print("result.kingCount :"..result.kingCount .. "result.kingCards count = "..#result.kingCards)
	if result.kingCount >= 3 then
		if self.srcCardType < CardTypeEnum.CT_THREE_KING then
			local tempList = {}
			for k,v in pairs(result.kingCards) do
				if k <= 3 then
					table.insert(tempList,v)
				else
					break
				end
			end

			self:AddIntroduceTips(tempList)
		end
	end


	if result.sevenCount > 0 then
		for k,v in pairs(result.sevenCards) do
			if self.srcCardType == CardTypeEnum.CT_BOMB_SEVEN then
				if v[1].logicValue > self.srcCards[1].logicValue then
					self:AddIntroduceTips(v)
				end
			elseif self.srcCardType < CardTypeEnum.CT_BOMB_SEVEN then
				self:AddIntroduceTips(v)
			end
		end
	end

	if result.eightCount > 0 then
		for k,v in pairs(result.eightCards) do
			if self.srcCardType == CardTypeEnum.CT_BOMB_EIGTH then
				if v[1].logicValue > self.srcCards[1].logicValue then
					self:AddIntroduceTips(v)
				end
			elseif self.srcCardType < CardTypeEnum.CT_BOMB_EIGTH then
				self:AddIntroduceTips(v)
			end
		end
	end

	-- 四幅 5 10 K
	if result.ftkCount >= 4 then
		if self.srcCardType < CardTypeEnum.CT_FOUR_FIVE_TEM_K then
			local tempList = {}
			for k,v in pairs(result.ftkCards) do
				if k <= 4 then
					for k_ele,v_ele in pairs(v) do
						table.insert(tempList,v_ele)
					end
				else
					break
				end
			end
			self:AddIntroduceTips(tempList)
		end
	end

	-- 四王
	if result.kingCount == 4 then
		if self.srcCardType < CardTypeEnum.CT_FOUR_KING then
			self:AddIntroduceTips(result.kingCards)
		end
	end
end




function BaseWSKCardAIResultStruct:SingleAI()
	local result = self.analysisStruct
	-- 首先对应牌型
	if result.singleCount > 0 then
		for k,v in pairs(result.singleCards) do
			if v[1].logicValue > self.srcCards[1].logicValue then
				self:AddIntroduceTips(v)
			end
		end
	end
	self:BaseBombCheck()
	-- 走拆分,遍历手牌
	local tempSingleTable = {}
	-- 从对子中找
	if result.doubleCount > 0 then
		self:InsertIntroduceFromTable(result.doubleCards,tempSingleTable,self.srcCards[1].logicValue,1)
	end
	-- 从三张中找
	if result.threeCount > 0 then
		self:InsertIntroduceFromTable(result.threeCards,tempSingleTable,self.srcCards[1].logicValue,1)
	end
	-- 从4张中找
	if result.fourCount > 0 then
		self:InsertIntroduceFromTable(result.fourCards,tempSingleTable,self.srcCards[1].logicValue,1)
	end

	-- 从5张中找
	if result.fiveCount > 0 then
		self:InsertIntroduceFromTable(result.fiveCards,tempSingleTable,self.srcCards[1].logicValue,1)
	end

	-- 从6张中找
	if result.sixCount > 0 then
		self:InsertIntroduceFromTable(result.sixCards,tempSingleTable,self.srcCards[1].logicValue,1)
	end

	-- 从7张中找
	if result.sevenCount > 0 then
		self:InsertIntroduceFromTable(result.sevenCards,tempSingleTable,self.srcCards[1].logicValue,1)
	end

	-- 从8炸中找
	if result.eightCount > 0 then
		self:InsertIntroduceFromTable(result.eightCards,tempSingleTable,self.srcCards[1].logicValue,1)
	end

	local compareFunc = function(cardTable1, cardTable2)
        if cardTable1[1].logicValue < cardTable2[1].logicValue then
            return true
        else
            return false
        end
    end

    table.sort(tempSingleTable,compareFunc)

	for k,v in pairs(tempSingleTable) do
		self:AddIntroduceTips(v)
	end
end

function BaseWSKCardAIResultStruct:DoubleAI()
	local result = self.analysisStruct
	-- 首先对应牌型
	if result.doubleCount > 0 then
		for k,v in pairs(result.doubleCards) do
			if v[1].logicValue > self.srcCards[1].logicValue then
				self:AddIntroduceTips(v)
			end
		end
	end
	self:BaseBombCheck()

	--走拆分
	local tempDoubleTable = {}

	-- 从三张中找
	if result.threeCount > 0 then
		self:InsertIntroduceFromTable(result.threeCards,tempDoubleTable,self.srcCards[1].logicValue,2)
	end
	-- 从4张中找
	if result.fourCount > 0 then
		self:InsertIntroduceFromTable(result.fourCards,tempDoubleTable,self.srcCards[1].logicValue,2)
	end

	-- 从5张中找
	if result.fiveCount > 0 then
		self:InsertIntroduceFromTable(result.fiveCards,tempDoubleTable,self.srcCards[1].logicValue,2)
	end

	-- 从6张中找
	if result.sixCount > 0 then
		self:InsertIntroduceFromTable(result.sixCards,tempDoubleTable,self.srcCards[1].logicValue,2)
	end

	-- 从7张中找
	if result.sevenCount > 0 then
		self:InsertIntroduceFromTable(result.sevenCards,tempDoubleTable,self.srcCards[1].logicValue,2)
	end

	-- 从8炸中找
	if result.eightCount > 0 then
		self:InsertIntroduceFromTable(result.eightCards,tempDoubleTable,self.srcCards[1].logicValue,2)
	end

	local compareFunc = function(cardTable1, cardTable2)
        if cardTable1[1].logicValue < cardTable2[1].logicValue then
            return true
        else
            return false
        end
    end

    table.sort(tempDoubleTable,compareFunc)

	for k,v in pairs(tempDoubleTable) do
		self:AddIntroduceTips(v)
	end
end

function BaseWSKCardAIResultStruct:ThreeAI()
	local result = self.analysisStruct

	--飞机的起始牌值
	local srcThreeCardLogicValue = self.srcCards[1].logicValue
	-- 收集上家打出牌中的3张以上的牌
	local threeValueArray = LogicUtil.CollectThreeCardValueArray(self.srcCardAnalysisStruct)
	if threeValueArray and #threeValueArray > 0 then
		srcThreeCardLogicValue = threeValueArray[1].logicValue
	end

	-- 首先对应牌型

	if result.threeCount > 0 then
		local temp
		for k,v in pairs(result.threeCards) do
			if v[1].logicValue > srcThreeCardLogicValue then
				temp = ShallowCopy(v)
				--从剩余牌组里补充带的牌
				local reminTable = self:GetLimitCountCardsByExclude(result,{v[1].logicValue},2)
				for k,v in pairs(reminTable) do
					table.insert(temp,v)
				end
				if #temp >=3 and #temp <= 5 then
					self:AddIntroduceTips(temp)
				end
			end
		end
	end
	self:BaseBombCheck()
	-- 走拆分

	local tempThreeTable = {}
	-- 从4张中找
	if result.fourCount > 0 then
		self:InsertIntroduceFromTable_three(result,result.fourCards,tempThreeTable,srcThreeCardLogicValue,3)
	end

	-- 从5张中找
	if result.fiveCount > 0 then
		self:InsertIntroduceFromTable_three(result,result.fiveCards,tempThreeTable,srcThreeCardLogicValue,3)
	end

	-- 从6张中找
	if result.sixCount > 0 then
		self:InsertIntroduceFromTable_three(result,result.sixCards,tempThreeTable,srcThreeCardLogicValue,3)
	end

	-- 从7张中找
	if result.sevenCount > 0 then
		self:InsertIntroduceFromTable_three(result,result.sevenCards,tempThreeTable,srcThreeCardLogicValue,3)
	end

	-- 从8炸中找
	if result.eightCount > 0 then
		self:InsertIntroduceFromTable_three(result,result.eightCards,tempThreeTable,srcThreeCardLogicValue,3)
	end

	local compareFunc = function(cardTable1, cardTable2)
        if cardTable1[1].logicValue < cardTable2[1].logicValue then
            return true
        else
            return false
        end
    end

    table.sort(tempThreeTable,compareFunc)

	for k,v in pairs(tempThreeTable) do
		self:AddIntroduceTips(v)
	end
end


function BaseWSKCardAIResultStruct:SingleLineAI()
	local result = self.analysisStruct
	local tempList = {}
	local srcLen = #self.srcCards
	local curLen = 0

	-- 首先对应牌型
	if result.singleCount > 0 then
		for k,v in pairs(result.singleCards) do
			curLen = #tempList
			if curLen > 0 then
				if v[1].logicValue == tempList[curLen].logicValue + 1 and v[1]:IsCanLine()then -- 若连续则加1
					table.insert(tempList,v[1])
				else -- 不连续 重新计数
					tempList = {}
					if v[1].logicValue > self.srcCards[1].logicValue and v[1]:IsCanLine() then
						table.insert(tempList,v[1])
					end
				end
			else
				if v[1].logicValue > self.srcCards[1].logicValue and v[1]:IsCanLine() then
					table.insert(tempList,v[1])
				end
			end
			--检查是否满足要求
			if srcLen == curLen then
				self:AddIntroduceTips(ShallowCopy(tempList))
				table.remove(tempList,1)--移除首位
			end
		end
	end

	self:BaseBombCheck()

	-- 没有走拆分
	tempList = {}
	curLen = 0
	--直接列举手牌
	for k,v in pairs(self.handCards) do
		curLen = #tempList
		if curLen > 0 then
			if v.logicValue == tempList[curLen].logicValue + 1 and v:IsCanLine()then -- 若连续则加1
				table.insert(tempList,v)
				curLen = curLen + 1
			else
				if v.logicValue ~= tempList[curLen].logicValue then-- 不连续 重新计数(相同牌值除外)
					tempList = {}
					--判断新的牌值是否满足起点要求
					if v.logicValue > self.srcCards[1].logicValue and v:IsCanLine() then
						table.insert(tempList,v)
					end
				end
			end
		else
			if v.logicValue > self.srcCards[1].logicValue and v:IsCanLine() then
				table.insert(tempList,v)
			end
		end
		--检查是否满足要求
		if srcLen == curLen then
			self:AddIntroduceTips(ShallowCopy(tempList))
			table.remove(tempList,1)--移除首位
		end
	end
end

function BaseWSKCardAIResultStruct:DoubleLineAI()
	local result = self.analysisStruct
	local tempList = {}
	local srcLen = #self.srcCards/2
	local curLen = 0
	-- 首先对应牌型
--	print("result.doubleCount = "..result.doubleCount)

	local srcDoubleCardLogicValue = self.srcCards[1].logicValue
	if #self.srcCardAnalysisStruct.doubleCards > 0 then
		srcDoubleCardLogicValue = self.srcCardAnalysisStruct.doubleCards[1][1].logicValue
	end

	if result.doubleCount > 0 then
		for k,v in pairs(result.doubleCards) do
			if curLen > 0 then
				if v[1].logicValue == tempList[curLen*2].logicValue + 1 and v[1]:IsCanLine()then -- 若连续则加1
					table.insert(tempList,v[1])
					table.insert(tempList,v[2])
				else -- 不连续 重新计数
					tempList = {}
					--新的牌值是否满足
					if v[1].logicValue > srcDoubleCardLogicValue and v[1]:IsCanLine()then
						table.insert(tempList,v[1])
						table.insert(tempList,v[2])
					end
				end
			else
				if v[1].logicValue > srcDoubleCardLogicValue and v[1]:IsCanLine()then
					table.insert(tempList,v[1])
					table.insert(tempList,v[2])
				end
			end
			--检查是否满足要求
			curLen = #tempList/2
			if srcLen == curLen then
				self:AddIntroduceTips(ShallowCopy(tempList))
				table.remove(tempList,1)--移除首位
				table.remove(tempList,1)--移除首位
				curLen = #tempList/2
			end
		end
	end

	self:BaseBombCheck()

	--走拆分
	--生成所有的2张临时组
	local tempDoubleCards = ShallowCopy(result.doubleCards)
	-- 从3张中找
	if result.threeCount > 0 then
		for k,v in pairs(result.threeCards) do
			table.insert(tempDoubleCards,{v[1],v[2]})
		end
	end
	-- 从4张中找
	if result.fourCount > 0 then
		for k,v in pairs(result.fourCards) do
			table.insert(tempDoubleCards,{v[1],v[2]})
		end
	end

	-- 从5张中找
	if result.fiveCount > 0 then
		for k,v in pairs(result.fiveCards) do
			table.insert(tempDoubleCards,{v[1],v[2]})
		end
	end

	-- 从6张中找
	if result.sixCount > 0 then
		for k,v in pairs(result.sixCards) do
			table.insert(tempDoubleCards,{v[1],v[2]})
		end
	end

	-- 从7张中找
	if result.sevenCount > 0 then
		for k,v in pairs(result.sevenCards) do
			table.insert(tempDoubleCards,{v[1],v[2]})
		end
	end

	-- 从8炸中找
	if result.eightCount > 0 then
		for k,v in pairs(result.eightCards) do
			table.insert(tempDoubleCards,{v[1],v[2]})
		end
	end

	local compareFunc = function(cardTable1, cardTable2)
        if cardTable1[1].logicValue < cardTable2[1].logicValue then
            return true
        else
            return false
        end
    end

    table.sort(tempDoubleCards,compareFunc)

	for k,v in pairs(tempDoubleCards) do
		if curLen > 0 then
			if v[1].logicValue == tempList[curLen*2].logicValue + 1 and v[1]:IsCanLine()then -- 若连续则加1
				table.insert(tempList,v[1])
				table.insert(tempList,v[2])
			else -- 不连续 重新计数
				tempList = {}
				if v[1].logicValue > srcDoubleCardLogicValue and v[1]:IsCanLine()then
					table.insert(tempList,v[1])
					table.insert(tempList,v[2])
				end
			end
		else
			if v[1].logicValue > srcDoubleCardLogicValue and v[1]:IsCanLine()then
				table.insert(tempList,v[1])
				table.insert(tempList,v[2])
			end
		end
		--检查是否满足要求
		curLen = #tempList/2
		if srcLen == curLen then
			self:AddIntroduceTips(ShallowCopy(tempList))
			table.remove(tempList,1)--移除首位
			table.remove(tempList,1)--移除首位
			curLen = #tempList/2
		end
	end
end

function BaseWSKCardAIResultStruct:ThreeLineAI()
	local result = self.analysisStruct
	local tempList = {}
	local handCardsLen = #self.handCards
	local scrCardsLen = #self.srcCards
	local srcLen = math.ceil(scrCardsLen/5) -- 几连
	local curLen = 0
	local excludeTable = {}

	--飞机的起始牌值
	local srcThreeCardLogicValue = self.srcCards[1].logicValue
	-- 收集上家打出牌中的3张以上的牌
	local threeValueArray = LogicUtil.CollectThreeCardValueArray(self.srcCardAnalysisStruct)
	--找出飞机的最大牌型
	local planeTable = self:FindMaxPlaneTable(threeValueArray,scrCardsLen,0)
	if #planeTable > 0 then
		srcThreeCardLogicValue = planeTable[1].logicValue
	end

	-- 首先对应牌型
	if result.threeCount > 0 then
		for k,v in pairs(result.threeCards) do
			if curLen > 0 then
--				DwDebug.LogError("v[1].logicValue = "..v[1].logicValue .. "tempList[curLen].logicValue = "..tempList[curLen*3].logicValue)
				if v[1].logicValue == tempList[curLen*3].logicValue + 1 and v[1]:IsCanLine()then -- 若连续则加1
					table.insert(tempList,v[1])
					table.insert(tempList,v[2])
					table.insert(tempList,v[3])
					table.insert(excludeTable,v[1].logicValue)
				else -- 不连续 重新计数
					tempList = {}
					excludeTable = {}
					if v[1].logicValue > srcThreeCardLogicValue and v[1]:IsCanLine()then
						table.insert(tempList,v[1])
						table.insert(tempList,v[2])
						table.insert(tempList,v[3])
						table.insert(excludeTable,v[1].logicValue)
					end
				end
			else
				if v[1].logicValue > srcThreeCardLogicValue and v[1]:IsCanLine()then
					table.insert(tempList,v[1])
					table.insert(tempList,v[2])
					table.insert(tempList,v[3])
					table.insert(excludeTable,v[1].logicValue)
				end
			end

			--检查是否满足要求
			curLen = #tempList/3
--			DwDebug.LogError("curLen "..curLen .. "srcLen "..srcLen)
			if srcLen == curLen then
				local workTable = ShallowCopy(tempList)
				--从剩余牌组里补充带的牌
				local reminTable = self:GetLimitCountCardsByExclude(result,excludeTable,2*srcLen)
				for k,v in pairs(reminTable) do
					table.insert(workTable,v)
				end
				--DwDebug.LogError("workTable count = "..#workTable)
				if #workTable >= srcLen*3 and #workTable <= srcLen* 5 then
					self:AddIntroduceTips(workTable)
				end
				table.remove(tempList,1)--移除首位
				table.remove(tempList,1)--移除首位
				table.remove(tempList,1)--移除首位
				table.remove(excludeTable,1)

				curLen = #tempList/3
			end
		end
	end
	
	self:BaseBombCheck()

	-- 拆分

	--生成所有的3张临时组
	local tempThreeCards = ShallowCopy(result.threeCards)
	--未进入3张临时组的4张及其以上数量的剩余牌列表
	local tempMemoryRemainCards = {}
	-- 从4张中找
	if result.fourCount > 0 then
		for k,v in pairs(result.fourCards) do
			table.insert(tempThreeCards,{v[1],v[2],v[3]})
			table.insert(tempMemoryRemainCards,v[4])
		end
	end

	-- 从5张中找
	if result.fiveCount > 0 then
		for k,v in pairs(result.fiveCards) do
			table.insert(tempThreeCards,{v[1],v[2],v[3]})
			table.insert(tempMemoryRemainCards,v[4])
			table.insert(tempMemoryRemainCards,v[5])
		end
	end

	-- 从6张中找
	if result.sixCount > 0 then
		for k,v in pairs(result.sixCards) do
			table.insert(tempThreeCards,{v[1],v[2],v[3]})
			table.insert(tempMemoryRemainCards,v[4])
			table.insert(tempMemoryRemainCards,v[5])
			table.insert(tempMemoryRemainCards,v[6])
		end
	end

	-- 从7张中找
	if result.sevenCount > 0 then
		for k,v in pairs(result.sevenCards) do
			table.insert(tempThreeCards,{v[1],v[2],v[3]})
			table.insert(tempMemoryRemainCards,v[4])
			table.insert(tempMemoryRemainCards,v[5])
			table.insert(tempMemoryRemainCards,v[6])
			table.insert(tempMemoryRemainCards,v[7])
		end
	end

	-- 从8炸中找
	if result.eightCount > 0 then
		for k,v in pairs(result.eightCards) do
			table.insert(tempThreeCards,{v[1],v[2],v[3]})
			table.insert(tempMemoryRemainCards,v[4])
			table.insert(tempMemoryRemainCards,v[5])
			table.insert(tempMemoryRemainCards,v[6])
			table.insert(tempMemoryRemainCards,v[7])
			table.insert(tempMemoryRemainCards,v[8])
		end
	end

	excludeTable = {}
	curLen = 0
	tempList = {}

	local compareFunc = function(cardTable1, cardTable2)
        if cardTable1[1].logicValue < cardTable2[1].logicValue then
            return true
        else
            return false
        end
    end

    table.sort(tempThreeCards,compareFunc)
--    DwDebug.LogError("#tempThreeCards = "..#tempThreeCards)
	for k,v in pairs(tempThreeCards) do
		if curLen > 0 then
--			DwDebug.LogError("v[1].logicValue = "..v[1].logicValue .. "tempList[curLen].logicValue = "..tempList[curLen*3].logicValue)
			if v[1].logicValue == tempList[curLen*3].logicValue + 1 and v[1]:IsCanLine()then -- 若连续则加1
				table.insert(tempList,v[1])
				table.insert(tempList,v[2])
				table.insert(tempList,v[3])
				table.insert(excludeTable,v[1].logicValue)
			else -- 不连续 重新计数
				tempList = {}
				excludeTable = {}
--				DwDebug.LogError("v[1].logicValue = "..v[1].logicValue .. "srcThreeCardLogicValue = "..srcThreeCardLogicValue)
				if v[1].logicValue > srcThreeCardLogicValue and v[1]:IsCanLine()then
					table.insert(tempList,v[1])
					table.insert(tempList,v[2])
					table.insert(tempList,v[3])
					table.insert(excludeTable,v[1].logicValue)
				end
			end
		else
--			DwDebug.LogError("v[1].logicValue = "..v[1].logicValue .. "srcThreeCardLogicValue = "..srcThreeCardLogicValue)
			if v[1].logicValue > srcThreeCardLogicValue and v[1]:IsCanLine()then
				table.insert(tempList,v[1])
				table.insert(tempList,v[2])
				table.insert(tempList,v[3])
				table.insert(excludeTable,v[1].logicValue)
			end
		end

		--检查是否满足要求
		curLen = #tempList/3
--		DwDebug.LogError("curLen "..curLen .. "srcLen "..srcLen)
		if srcLen == curLen then
			local workTable = ShallowCopy(tempList)
			--从剩余牌组里补充带的牌

			local reminTable = self:GetLimitCountCardsByExclude(result,excludeTable,2*srcLen)
			for k,v in pairs(reminTable) do
				table.insert(workTable,v)
			end

			-- local testStr = ""

			-- for k,v in pairs(excludeTable) do
			-- 	testStr = testStr .." ".. v
			-- end
			-- DwDebug.LogError(testStr)
			-- testStr = ""
			-- for k,v in pairs(workTable) do
			-- 	testStr = testStr .." ".. v.logicValue
			-- end
			-- DwDebug.LogError(testStr)
			--检查补充的牌是否足够带牌数
			local curTableNum = #workTable
			local normalNum = srcLen*5
--			DwDebug.LogError("curTableNum = "..curTableNum .. "normalNum = "..normalNum)
			if curTableNum < normalNum then-- 不够，就需要从飞机牌型里相等的牌里拿
				local needAddNum = normalNum - curTableNum
				local tempMemoryTableNum = #tempMemoryRemainCards
				if tempMemoryTableNum < needAddNum then
					needAddNum = tempMemoryTableNum
				end
--				DwDebug.LogError("needAddNum = "..needAddNum)
				for i = 1,needAddNum do
					table.insert(workTable,tempMemoryRemainCards[i])
				end
			end

			self:AddIntroduceTips(workTable)
			
			table.remove(tempList,1)--移除首位
			table.remove(tempList,1)--移除首位
			table.remove(tempList,1)--移除首位
			table.remove(excludeTable,1)

			curLen = #tempList/3
		end
	end
end

function BaseWSKCardAIResultStruct:FiveTenKAI()
	self:BaseBombCheck()
end

function BaseWSKCardAIResultStruct:BombAI()
	self:BaseBombCheck()
end


---------------------------------------------- tool function ---------------------------------------------------

-- 检查能否添加指定数量的过滤牌组
function BaseWSKCardAIResultStruct:CheckAndAddLimitCountCards(srcCardTable,desCardTable,excludeTable,limitCount)
	local remainCount = limitCount
	local isInclude = false
	local isFinish = false
	for k,v in pairs(srcCardTable) do
		isInclude = false
		--检查是否在过滤牌值队列里
		for k1,v1 in pairs(excludeTable) do
			if v[1].logicValue == v1 then
				isInclude = true
			end
		end
		--如果不在过滤队列里
		if not isInclude then
			for k1,v1 in pairs(v) do
				if remainCount > 0 then
					remainCount = remainCount -1
					table.insert(desCardTable,v1)
				else
					isFinish = true
					break
				end
			end
		end
	end
	return remainCount,isFinish
end

--从牌组里拿出过滤掉指定牌值的指定数量的牌（仅适用于3张带牌时用来获取带牌）
--srcCardAnalysisStruct 上家牌分析结构
--excludeTable 对应忽略的牌列表
--limitCount 补充牌的数量
function BaseWSKCardAIResultStruct:GetLimitCountCardsByExclude(srcCardAnalysisStruct,excludeTable,limitCount)
	local remainCount = limitCount
	local tempTable = {}
	local isExclude = false
	local isFinish = false
	if srcCardAnalysisStruct.singleCount > 0 then
		remainCount,isFinish = self:CheckAndAddLimitCountCards(srcCardAnalysisStruct.singleCards,tempTable,excludeTable,remainCount)
		if isFinish then
			return tempTable
		end
	end

	-- 从对子中找
	if srcCardAnalysisStruct.doubleCount > 0 then
		remainCount,isFinish = self:CheckAndAddLimitCountCards(srcCardAnalysisStruct.doubleCards,tempTable,excludeTable,remainCount)
		if isFinish then
			return tempTable
		end
	end
	-- 从三张中找
	if srcCardAnalysisStruct.threeCount > 0 then
		remainCount,isFinish = self:CheckAndAddLimitCountCards(srcCardAnalysisStruct.threeCards,tempTable,excludeTable,remainCount)
		if isFinish then
			return tempTable
		end
	end
	-- 从4张中找
	if srcCardAnalysisStruct.fourCount > 0 then
		remainCount,isFinish = self:CheckAndAddLimitCountCards(srcCardAnalysisStruct.fourCards,tempTable,excludeTable,remainCount)
		if isFinish then
			return tempTable
		end
	end

	-- 从5张中找
	if srcCardAnalysisStruct.fiveCount > 0 then
		remainCount,isFinish = self:CheckAndAddLimitCountCards(srcCardAnalysisStruct.fiveCards,tempTable,excludeTable,remainCount)
		if isFinish then
			return tempTable
		end
	end

	-- 从6张中找
	if srcCardAnalysisStruct.sixCount > 0 then
		remainCount,isFinish = self:CheckAndAddLimitCountCards(srcCardAnalysisStruct.sixCards,tempTable,excludeTable,remainCount)
		if isFinish then
			return tempTable
		end
	end

	-- 从7张中找
	if srcCardAnalysisStruct.sevenCount > 0 then
		remainCount,isFinish = self:CheckAndAddLimitCountCards(srcCardAnalysisStruct.sevenCards,tempTable,excludeTable,remainCount)
		if isFinish then
			return tempTable
		end
	end

	-- 从8炸中找
	if srcCardAnalysisStruct.eightCount > 0 then
		remainCount,isFinish = self:CheckAndAddLimitCountCards(srcCardAnalysisStruct.eightCards,tempTable,excludeTable,limitCount)
		if isFinish then
			return tempTable
		end
	end

	return tempTable
end

--从一个类型牌表里获取指定数量的牌放到目标table里（非三张类型）
function BaseWSKCardAIResultStruct:InsertIntroduceFromTable(srcTable,desTable,srcMinValue,eleCount)
	local tempTable = {}
	for k,v in pairs(srcTable) do
		if v[1].logicValue > srcMinValue then
			if #v >= eleCount then
				tempTable = {}
				for i=1,eleCount do
					table.insert(tempTable,v[i])
				end
				table.insert(desTable,tempTable)
			end
		end
	end
end

--从一个类型牌表里获取指定数量的牌放到目标table里（三张类型）
function BaseWSKCardAIResultStruct:InsertIntroduceFromTable_three(srcCardAnalysisStruct,srcTable,desTable,srcMinValue,eleCount)
	local tempTable = {}
	local srcTableNum = #srcTable
	for k,v in pairs(srcTable) do
		if v[1].logicValue > srcMinValue then
			if #v >= eleCount then
				tempTable = {}
				for i=1,eleCount do
					table.insert(tempTable,v[i])
				end
				--从剩余牌组里补充带的牌
				local reminTable = self:GetLimitCountCardsByExclude(srcCardAnalysisStruct,{v[1].logicValue},2)
				for k,v in pairs(reminTable) do
					table.insert(tempTable,v)
				end

				--检查补充的牌是否足够带牌数
				local curTableNum = #tempTable
				local normalNum = 5

				if curTableNum < normalNum then-- 不够，就需要从飞机牌型里相等的牌里拿
					--需要补充的数量
					local needAddNum = normalNum - curTableNum
					--原表里除去eleCount后还剩余的牌数
					local tempMemoryTableNum = srcTableNum - eleCount
					if tempMemoryTableNum > 0 then
						if tempMemoryTableNum < needAddNum then
							needAddNum = tempMemoryTableNum
						end
						--从原表里除去eleCount后还剩余的牌数中来补充
						for i = 1,needAddNum do
							table.insert(tempTable,v[i+eleCount])
						end
					end
				end

				table.insert(desTable,tempTable)
			end
		end
	end
end

-- 从3张数组里找出最大飞机的牌组
-- threeValueArray 元素类型CCard 3张的牌数组{3,4,5}
function BaseWSKCardAIResultStruct:FindMaxPlaneTable(threeValueArray,cardCount,handCardCount)
	--三张连牌判断
	local maxLineNum = 1
	--当前连牌数
	local curLineNum = 1
	--连牌table
	local realTable = {}
	--临时缓存table
	local tempTable = {}

	local card
	firstLogicValue = threeValueArray[1].logicValue

	--连牌判断
	for k,v in pairs(threeValueArray) do --因为从索引从1开始
		card = threeValueArray[k+1]
		if card ~=nil then
			if not card:IsCanLine() or (firstLogicValue ~= card.logicValue - curLineNum)then -- 不能三连的牌 或者 两个三条，连不起来，肯定是错牌（默认升序）
				curLineNum = 1
				firstLogicValue = card.logicValue
				table.insert(tempTable,card)
			else
				curLineNum = curLineNum + 1
				if maxLineNum < curLineNum then
					maxLineNum = curLineNum
					realTable = tempTable
					tempTable = {}
				end
			end
		end
	end

	if maxLineNum*5 == cardCount then -- (3+2) * threeCount == cardCount
		return realTable
	elseif handCardCount and maxLineNum*5 >= handCardCount then
		return realTable
	else
		local realLineNum = math.floor(cardCount/5)
		if realLineNum < maxLineNum then -- 多个飞机下，其他的三张可以当做散牌
			if realLineNum*5 == cardCount then
				local removeNum = maxLineNum - realLineNum
				for i = 1,removeNum do
					table.remove(tempTable,1)
				end
			elseif handCardCount and realLineNum * 5 >= handCardCount then
				return realTable
			end
		end
	end

	return nil
end
