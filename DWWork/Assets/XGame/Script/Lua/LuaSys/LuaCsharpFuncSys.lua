--------------------------------------------------------------------------------
-- 	 File      : LuaCsharpFuncSys.lua
--   author    : shandong   shandong@ezfun.cn
--   desc      : 
--   version   : 1.0
--   date      : 2017-06-26 14:40:58.
--   copyright : Copyright 2016 EZFun Inc.
--------------------------------------------------------------------------------

LuaCsharpFuncSys = {}

local funcSeq = 0
local funcMap = {}
function LuaCsharpFuncSys.RegisterFunc(func)
	funcSeq = funcSeq + 1
	funcMap[funcSeq] = func
	return funcSeq
end

function LuaCsharpFuncSys.UnRegisterFunc(seq)
	if not seq or seq == 0 then
		return nil
	end
	local data = funcMap[seq] 
	funcMap[seq] = nil
	return data
end

-- 打印错误信息
local function __TRACKBACK__(errmsg)
    local track_text = debug.traceback(tostring(errmsg), 6);
    logError("--------------------- TRACKBACK --------------------\n"..track_text.."--------------------- TRACKBACK --------------------\n");
    local exception_text = "LUA EXCEPTION\n" .. track_text;
    return false;
end



function LuaCsharpFuncSys.CallFunc(seq, ...)
	local func = funcMap[seq]
	if func then
		 if Util.isApplePlatform then
	        local args = {...}
	        local func1 = function() func(unpack(args)) end
	        flag, msg = xpcall(func1, __TRACKBACK__)	
	    else
	        flag, msg = xpcall(func, __TRACKBACK__, ...)	
	    end
	else
		-- func removed 
	end
end

return LuaCsharpFuncSys