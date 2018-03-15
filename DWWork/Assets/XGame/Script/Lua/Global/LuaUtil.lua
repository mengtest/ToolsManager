--------------------------------------------------------------------------------
-- 	 File      : LuaUtil.lua
--   author    : 
--   desc      : 
--   version   : 1.0
--   date      : 2017-05-25 17:12:24.
--   copyright : Copyright 2016 EZFun Inc.
--------------------------------------------------------------------------------

LuaUtil = {}

function LuaUtil.SecToStr(sec)
	local hour = math.modf(sec/3600)
	local minute = math.modf( math.fmod(sec , 3600) / 60)
	local sec = math.fmod(sec, 60)
	if hour == 0 then
		return string.format("%02d:%02d", minute, sec)
	else
		return string.format("%02d:%02d:%02d", hour, minute, sec)
	end
end

function LuaUtil.CreatEnumTabel(tbl, index)
    local enumtbl = {}
    local enumIndex = index or 0
    for i, v in pairs(tbl) do
        enumtbl[v] = enumIndex + i
    end
    return enumtbl
end

function LuaUtil.MD5(...)
	local valTable = {...}
	for i = 1, #valTable do
		local val = valTable[i]
		if type(val) == "number" then
			valTable[i] = tostring(val)
		end
	end
	local str = table.concat(valTable)
	return WrapSys.ComputeHash(str)
end

--字符串分割 "aaa,bb,bb" ","
function LuaUtil.StringSplit(szFullString, szSeparator)
	local nFindStartIndex = 1
	local nSplitIndex = 1
	local nSplitArray = {}
	while true do
	   local nFindLastIndex = string.find(szFullString, szSeparator, nFindStartIndex)
	   if not nFindLastIndex then
	    nSplitArray[nSplitIndex] = string.sub(szFullString, nFindStartIndex, string.len(szFullString))
	    break
	   end
	   nSplitArray[nSplitIndex] = string.sub(szFullString, nFindStartIndex, nFindLastIndex - 1)
	   nFindStartIndex = nFindLastIndex + string.len(szSeparator)
	   nSplitIndex = nSplitIndex + 1
	end
	return nSplitArray
end

function LuaUtil.UrlDecode(s)
    local s = string.gsub(s, '%%(%x%x)', function(h) return string.char(tonumber(h, 16)) end)
    return s
end

function LuaUtil.UrlEncode(s)
    local s = string.gsub(s, "([^%w%.%- ])", function(c) return string.format("%%%02X", string.byte(c)) end)
    return string.gsub(s, " ", "+")
end


return LuaUtil