--------------------------------------------------------------------------------
-- 	 File      : UIMJHandCardCtrl.lua
--   author    : zhisong
--   function  : 日志控制
--   date      : 2017-12-8
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

local m_module_switch = {
	-- 需要打印的模块名放在这，值为true
	CommonLog = true, -- 没有指定模块时，会走通用模块控制log

	MJPlayLogic = false,
	PlayerState = false,
	LuaNetWork = true,
	MJHandCard = false,
	CreateRoom = false,
	Drag = false,
	xxx = true,
	HttpNetWork = false,
	PlayerState = false,
	IntervalTaskQueue = false,
	MJOpCtrl = false,
}

local table_serializer = require "utils"

local ModulizeDebug = {
	-- 设置log类型的开关
	enable_log = true,
	enable_log_warning = true,
	enable_log_error = true
}

function ModulizeDebug.Seria(...)
	local t = {...}
	local ret = {}
	for k,v in pairs(t) do
		-- logError("k=" .. tostring(k) .. ";v=" .. tostring(v))
		table.insert(ret, table_serializer.serialize(v))
	end

	return table.concat(ret, ",") .. "\n" .. debug.traceback() .. "\n"
end

function ModulizeDebug.Log(module_name, ...)
	if ModulizeDebug.enable_log then
	 	local t = type(module_name)
		if t == "string" then
			--注册了模块，由模块控制
			if m_module_switch[module_name] ~= nil then 
		 		if m_module_switch[module_name] == true then
					if not WrapSys.Constants_RELEASE then
						WrapSys.Util_Log(ModulizeDebug.Seria(...))
					end
				end
			else--未注册走通用模块控制
				if m_module_switch.CommonLog then
					if not WrapSys.Constants_RELEASE then
						WrapSys.Util_Log(ModulizeDebug.Seria(module_name,...))
					end
				end
			end
		else
			if m_module_switch.CommonLog then
				if not WrapSys.Constants_RELEASE then
					WrapSys.Util_Log(ModulizeDebug.Seria(module_name,...))
				end
			end
		end
	end
end

function ModulizeDebug.LogWarning(module_name, ...)
	if ModulizeDebug.enable_log_warning then
		local t = type(module_name)
		if t == "string" then
			--注册了模块，由模块控制
			if m_module_switch[module_name] ~= nil then 
		 		if m_module_switch[module_name] == true then
					if not WrapSys.Constants_RELEASE then
						WrapSys.Util_LogWarning(ModulizeDebug.Seria(...))
					end
				end
			else--未注册走通用模块控制
				if m_module_switch.CommonLog then
					if not WrapSys.Constants_RELEASE then
						WrapSys.Util_LogWarning(ModulizeDebug.Seria(module_name,...))
					end
				end
			end
		else
			if m_module_switch.CommonLog then
				if not WrapSys.Constants_RELEASE then
					WrapSys.Util_LogWarning(ModulizeDebug.Seria(module_name,...))
				end
			end
		end
	end
end
--error级别
function ModulizeDebug.LogError(module_name, ...)
	if ModulizeDebug.enable_log_error then
		local t = type(module_name)
		if t == "string" then
			--注册了模块，由模块控制
			if m_module_switch[module_name] ~= nil then 
		 		if m_module_switch[module_name] == true then
					WrapSys.Util_LogError(ModulizeDebug.Seria(...))
				end
			else--未注册走通用模块控制
				WrapSys.Util_LogError(ModulizeDebug.Seria(module_name,...))
			end
		else
			WrapSys.Util_LogError(ModulizeDebug.Seria(module_name,...))
		end
	end
end

return ModulizeDebug
