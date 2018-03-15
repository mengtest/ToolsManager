--------------------------------------------------------------------------------
-- 	 File      : CardTypeEnum.lua
--   author    : guoliang
--   function   : 牌型枚举
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

CardTypeEnum = 
{
	CT_ERROR = 0,--错误类型
	CT_SINGLE = 1,--单牌类型
	CT_DOUBLE = 2,--对牌类型
	CT_THREE = 3,--三条类型
	CT_THREE_LINE_TAKE_ONE = 4,--三带一单
	CT_THREE_LINE_TAKE_TWO = 5,--三带一对
	CT_SINGLE_LINE = 6,--单连类型
	CT_DOUBLE_LINE = 7,--对连类型
	CT_THREE_LINE = 8,--三连类型
	CT_DOUBLE_SAME_KING = 9,--纯色双王
	CT_FIVE_TEN_K = 10,--副510k
	CT_FIVE_TEN_K_BIG = 11,--正510k
	CT_BOMB_FOUR = 12,--四炸
	CT_BOMB_FIVE = 13,--五炸
	CT_THREE_FIVE_TEM_K = 14,--3副510k
	CT_BOMB_SIX	= 15,--六炸
	CT_THREE_KING = 16,--三王
	CT_BOMB_SEVEN = 17,-- 七炸
	CT_FOUR_FIVE_TEM_K = 18,--4副510k
	CT_BOMB_EIGTH = 19,--八炸
	CT_FOUR_KING = 20,--四个王
	--红心5特殊牌型
	CT_DOUBLE_SAME_HongXin_5 = 21,--对红心5
}

--用来输出日志
for k,v in pairs(CardTypeEnum) do
	CardTypeEnum[v] = k
end