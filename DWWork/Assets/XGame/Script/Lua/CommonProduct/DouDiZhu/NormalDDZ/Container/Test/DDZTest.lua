--------------------------------------------------------------------------------
-- 	 File       : DDZTest.lua
--   author     : zhanghaochun
--   function   : 斗地主测试控件
--   date       : 2018-01-22
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------
--path : CommonProduct.DouDiZhu.NormalDDZ.Container.Test.DDZTest
local DDZContainer = require "CommonProduct.DouDiZhu.NormalDDZ.Container.NormalDiZhuContainer"
require "Logic.CardLogic.CCard"
require "CommonProduct.CommonBase.NewCardTypeEnum"

-- 花色
local ColorConfig =
{
	[1] = "方块",
	[2] = "梅花",
	[3] = "红桃",
	[4] = "黑桃",
	[5] = "王",
}

--
local LogicValueConfig =
{
	[3] = "3",
	[4] = "4",
	[5] = "5",
	[6] = "6",
	[7] = "7",
	[8] = "8",
	[9] = "9",
	[10] = "10",
	[11] = "J",
	[12] = "Q",
	[13] = "K",
	[14] = "A",
	[15] = "2",
	[16] = "小",
	[17] = "大",
}

-- card ID : 3 - 87
-- 牌型配置
local DDZCardTypeConfig =
{
	--[[[DDZCardTypeEnum.DDZCT_SINGLE] = {4},    -- 单牌[1]
	[DDZCardTypeEnum.DDZCT_DOUBLE] = {12, 32},    -- 对子[2]
	[DDZCardTypeEnum.DDZCT_THREE] = {13, 33, 53},    -- 三张[3]
	[DDZCardTypeEnum.DDZCT_THREE_ADD_ONE] = {10, 30, 50, 3},    -- 三带一[4]
	[DDZCardTypeEnum.DDZCT_THREE_ADD_DOUBLE] = {5, 25, 45, 4, 24},    -- 三带一对[5]
	[DDZCardTypeEnum.DDZCT_SINGLE_LINE] = {5, 26, 7, 28, 9, 10},    -- 顺子[6]
	[DDZCardTypeEnum.DDZCT_DOUBLE_LINE] = {5, 25, 6, 46},    -- 连对[7]
	[DDZCardTypeEnum.DDZCT_THREE_LINE] = {8, 48, 68, 29, 49, 69},    -- 飞机[8]
	[DDZCardTypeEnum.DDZCT_THREE_LINE_ADD_SAME_ONE] = {10, 30, 50, 11, 31, 51, 3, 4},    -- 飞机带相同数量的单牌[9]
	[DDZCardTypeEnum.DDZCT_THREE_LINE_ADD_SAME_DOUBLE] = {11, 31, 51, 12, 32, 52, 5, 25, 7, 47},    -- 飞机带相同数量的对牌[10]
	[DDZCardTypeEnum.DDZCT_FOUR_ADD_ONE] = {4, 24, 44, 64, 3},    -- 四带一[11]
	[DDZCardTypeEnum.DDZCT_FOUR_ADD_TWO] = {5, 25, 45, 65, 6, 7},    -- 四带二[12]
	[DDZCardTypeEnum.DDZCT_FOUR_ADD_DOUBLE] = {6, 26, 46, 66, 8, 68},    -- 四带一对[13]
	[DDZCardTypeEnum.DDZCT_FOUR_ADD_TWO_DOUBLE] = {9, 29, 49, 69, 3, 23, 24, 44},    -- 四带二对[14]
	[DDZCardTypeEnum.DDZCT_BOMB] = {14, 34, 54, 74},    -- 炸弹[15]
	[DDZCardTypeEnum.DDZCT_ROCKET] = {83, 84},    -- 火箭[16]
	[17] = {14, 34, 54, 74, 13, 33, 53, 7},    -- 飞机+单牌]]
	--[[[1] = {4, 24, 44, 5, 25, 45, 8, 28, 48, 68}, -- 三个4, 三个5, 四个8(飞机带对子)
	[2] = {4, 24, 44, 5, 25, 45, 6, 26, 46, 7, 27, 47, 8, 28, 48, 9},  -- 三个4,三个5,三个6,3个7,三个8,一个9(飞机带单)
	[3] = {4, 24, 44, 5, 25, 45, 6, 26, 46, 7, 27, 47, 8, 28, 48, 9, 29, 49, 10, 30}, -- 三个4,三个5,三个6,三个个7,三个8,三个9，两个10(飞机带单)
	[4] = {4, 24, 44, 5, 25, 45, 6, 26, 46, 8, 28, 48}, -- 三个4,三个5,三个6, 三个8(飞机带单)
	[5] = {4, 24, 44, 5, 25, 45, 65, 10}, -- 三个4,4个五,一个10(飞机带单)]]
	[1] = {4, 24, 44, 5, 25, 45, 6, 26, 46, 8, 28, 48}
}

-- 比较大小配置
local CompareCardsConfig = 
{
	[1] = {{4}, {6}},    -- 单牌 单牌
	[2] = {{5, 25},{8, 28}},    -- 对子 对子
	[3] = {{7, 47}, {3, 23, 43, 63}},    -- 单牌 炸弹
	[4] = {{9, 29, 49, 69}, {83, 84}},    -- 炸弹 火箭
	[5] = {{83, 84}, {15, 35, 55, 75}},    -- 火箭 炸弹
	[6] = {{5, 25, 45, 10}, {12, 32, 52, 8}},    -- 三带一 三带一
	[7] = {{3, 4, 5, 6, 7}, {8, 9, 10, 11, 12, 13}}, -- 顺子(5) 顺子(6)
	[8] = {{3, 4, 5, 6, 7}, {8, 9, 10, 11, 12}},    -- 顺子(5) 顺子(5) 
	[9] = {{9, 29, 49, 69, 3}, {11, 31, 51, 71, 10}},    -- 四带一 四带一
	[10] = {{9, 49, 10, 50}, {14, 34, 15, 35}},    -- 连对 连对(不合法)
	[11] = {{4, 24, 44, 5, 25, 45, 9, 3}, {6, 26, 46, 7, 27, 47, 10, 11, 12}},    -- 飞机+相同数量的单牌 飞机+相同数量的单牌(不合法)
	[12] = {{4, 24, 44, 64, 5, 25, 45, 65}, {7, 27, 47, 8, 28, 48, 11, 13}},    --双炸组成的飞机 飞机+相同数量的单牌
	[13] = {{9, 29, 49, 69, 3, 23, 24, 44}, {10, 30, 50, 70, 7, 27, 11, 31}},    --四带两对 四带两对
}

-- 手牌配置
local HandCardsConfig = {3, 23, 4, 24, 44, 64, 7, 10, 11, 31, 13, 33, 14, 34, 54, 15, 83}

-- 玩家出牌配置
local OutCardsConfig = 
{
	[1] = {5, 25, 45, 43},
}

DDZTest = class("DDZTest")

function DDZTest:ctor()
	self:InitValues()
	self:StartTest()
end

function DDZTest:InitValues()
	-- 分析容器
	self.container = DDZContainer.New()
	-- 牌型测试
	self.cardList_1 = {}
	-- 比较测试
	self.compareList = {}
	-- AI测试
	self.HandList = {}
	self.OutCardList = {}
end

function DDZTest:StartTest()
	-- 测试牌型
	self:InitCardsList_1()
	self:AnalysisCardsList_1()

	-- 比较测试
	--self:InitCompareCardsList()
	--self:CompareCardsLists()

	-- AI 测试
	--self:InitHandCardsList()
	--self:InitOutCardsList()
	--self:AITest()
end

--------------------------------牌型测试-----------------------------------------
function DDZTest:InitCardsList_1()
	for k, v in ipairs(DDZCardTypeConfig) do
		local cards ={}
		for key, value in ipairs(v) do
			local card = CCard.New()
			card:Init(value, 1)
			table.insert(cards, card)
		end
		table.insert(self.cardList_1, cards)
	end

	--[[local cc = DDZCardTypeConfig[9]
	local cards ={}
	for key, value in ipairs(cc) do
		local card = CCard.New()
		card:Init(value, 1)
		table.insert(cards, card)
	end
	table.insert(self.cardList_1, cards)]]
end

function DDZTest:AnalysisCardsList_1()
	for k, v in ipairs(self.cardList_1) do
		local canOut, cardType = self.container:DDZCardsIsRule(v, 9)
		logError("cardType is : " .. cardType)
	end
end
--------------------------------------------------------------------------------

------------------------------大小比较测试----------------------------------------
function DDZTest:InitCompareCardsList()
	for k, v in ipairs(CompareCardsConfig) do
		local lists = {}
		for key, value in ipairs(v) do
			local list = {}
			for kk, vv in ipairs(value) do
				local card = CCard.New()
				card:Init(vv)
				table.insert(list, card)
			end
			table.insert(lists, list)
		end
		table.insert(self.compareList, lists)
	end

	--[[local v = CompareCardsConfig[12]
	local lists = {}
	for key, value in ipairs(v) do
		local list = {}
		for kk, vv in ipairs(value) do
			local card = CCard.New()
			card:Init(vv)
			table.insert(list, card)
		end
		table.insert(lists, list)
	end
	table.insert(self.compareList, lists)]]
	
end

function DDZTest:CompareCardsLists()
	for k, v in ipairs(self.compareList) do
		local isBigger, cardsType = self.container:DDZCompareCards(v[1], v[2], 5)
		logError("ID : " .. k .. ". isBigger : " .. tostring(isBigger) .. ". type : " .. tostring(cardsType))
	end
end
--------------------------------------------------------------------------------

-------------------------------AI模块测试----------------------------------------
function DDZTest:InitHandCardsList()
	for k, v in ipairs(HandCardsConfig) do
		local card = CCard.New()
		card:Init(v)
		table.insert(self.HandList, card)
	end
end

function DDZTest:InitOutCardsList()
	for k, v in ipairs(OutCardsConfig) do
		local list = {}
		for key, value in ipairs(v) do
			local card = CCard.New()
			card:Init(value)
			table.insert(list, card)
		end
		table.insert(self.OutCardList, list)
	end

	--[[local v = OutCardsConfig[9]
	local list = {}
	for key, value in ipairs(v) do
		local card = CCard.New()
		card:Init(value)
		table.insert(list, card)
	end
	table.insert(self.OutCardList, list)]]
end

function DDZTest:AITest()
	for k, v in ipairs(self.OutCardList) do
		local result = self.container:DDZFindIntroduceCards(self.HandList, v)
		
		if result == nil then
			logError("k : " .. k .. "is nil")
		else
			for key, value in ipairs(result) do
				local str = ""
				for kk, vv in ipairs(value) do
					str = str .. " " .. ColorConfig[vv.color] .. LogicValueConfig[vv.logicValue]
				end
				logError("k : " .. k .. " key : " .. key .. "    " .. str)
			end
		end
		
	end
end
--------------------------------------------------------------------------------
function DDZTest:Destroy()

end