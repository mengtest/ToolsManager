--------------------------------------------------------------------------------
-- 	 File      : MJCardTypeEnum.lua
--   author    : guoliang
--   funMJCTion   : 麻将番型枚举
--   date      : 2017-11-5
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

MJCardTypeEnum = 
{
	MJCT_ERROR = 0,--错误类型
	MJCT_TIAN_HU = 1,--天胡
	MJCT_DI_HU = 2,--地胡
	MJCT_PING_HU = 3,--平胡
	MJCT_PENG_PENG_HU = 4,--对对胡
	MJCT_JIA_HU_ZI_YI_SE = 5,--假字一色
	MJCT_ZHEN_HU_ZI_YI_SE = 6,--真字一色
	MJCT_JIA_QING_YI_SE = 7,--假清一色
	MJCT_ZHEN_QING_YI_SE = 8,--真清一色
	MJCT_QI_DUI = 9,--七对
	MJCT_SHI_SAN_LAN = 10,--十三烂
	MJCT_QI_XING_SHI_SAN_LAN = 11,--七星十三栏
}

--用来输出日志
for k,v in pairs(MJCardTypeEnum) do
	MJCardTypeEnum[v] = k
end
