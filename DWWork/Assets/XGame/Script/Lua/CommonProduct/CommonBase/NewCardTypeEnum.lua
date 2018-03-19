--------------------------------------------------------------------------------
-- 	 File       : NewCardTypeEnum.lua
--   author     : zhanghaochun
--   function   : 斗地主牌型枚举
--   date       : 2018-01-03
--   copyright  : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------

NewCardTypeEnum = 
{
	NCCT_ERROR = 0,                             -- 错误牌型
	NCCT_SINGLE = 1,                            -- 单牌
	NCCT_DOUBLE = 2,                            -- 对子
	NCCT_THREE = 3,                             -- 三张牌
	NCCT_THREE_ADD_ONE = 4,                     -- 三带一张牌
	NCCT_THREE_ADD_DOUBLE = 5,                  -- 三带一对
	NCCT_SINGLE_LINE = 6,                       -- 顺子(>= 5张)
	NCCT_DOUBLE_LINE = 7,                       -- 连对(连续的对子数量 >= 3)
	NCCT_THREE_LINE = 8,                        -- 飞机(连续的三张牌数量 >= 2)(连续的三张)
	NCCT_THREE_LINE_ADD_SAME_ONE = 9,           -- 飞机带相同数量的单牌(连续的三带一张牌)
	NCCT_THREE_LINE_ADD_SAME_DOUBLE = 10,       -- 飞机带相同数量的对牌(连续的三带一对)
	NCCT_FOUR_ADD_ONE = 11,                     -- 四带一(弃用)
	NCCT_FOUR_ADD_TWO = 12,                     -- 四带二
	NCCT_FOUR_ADD_THREE = 13,                   -- 四带三
	NCCT_FOUR_ADD_TWO_DOUBLE = 14,              -- 四带两对
	NCCT_BOMB = 15,                             -- 炸弹(四张相同的牌)
	NCCT_ROCKET = 16,                           -- 火箭
}
