--------------------------------------------------------------------------------
-- 	 File      : AreaLuaSys.lua
--   author    : jianing
--   function  : 地区包显示
--   date      : 2018-03-22
--   copyright : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------

AreaLuaSys = {}
local _s = AreaLuaSys
local AreaInfoList = {}

--获取配置
function AreaLuaSys.GetAreaInfoList()
    if AreaInfoList == nil or #AreaInfoList <= 0 then
        AreaInfoList = LuaTableSys.GetTableDic("ResAreaInfoList")
    end
	return AreaInfoList
end

--当前区域名称
function AreaLuaSys.GetNowAreaName()
    return AreaSys.Instance.NowAreaName
end

return AreaLuaSys