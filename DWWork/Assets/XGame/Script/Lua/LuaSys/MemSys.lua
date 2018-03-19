--------------------------------------------------------------------------------
-- 	 File      : MemSys.lua
--   author    : guoliang 
--   desc      : 
--   version   : 1.0
--   date      : 2017-07-29 15:09:02.
--   copyright : Copyright 2016 EZFun Inc.
--------------------------------------------------------------------------------

MemSys = {}


local mri = nil

local fileCount = 0

function MemSys.Sample()
	if not mri then
		mri = require("Global.MemoryReferenceInfo")
	end
	local srcCount = collectgarbage("count")
	collectgarbage("collect")
	mri.m_cMethods.DumpMemorySnapshot(WrapSys.LuaPersistPath, "lua_m_" .. tostring(fileCount) .. ".txt", -1, "_G", _G)
	fileCount = fileCount + 1
	return srcCount, fileCount - 1
end

function MemSys.Compare()
	local srcCount = collectgarbage("count")
	if fileCount == 0 then
		return srcCount
	end
	mri.m_cMethods.DumpMemorySnapshot(WrapSys.LuaPersistPath, "lua_m_" .. tostring(fileCount) .. ".txt", -1, "_G", _G)
	mri.m_cMethods.DumpMemorySnapshotComparedFile(WrapSys.LuaPersistPath, "lua_m_" ..tostring(fileCount -1) .. "-" .. tostring(fileCount) 
		.. ".txt", "-1", WrapSys.LuaPersistPath.. "/lua_m_" .. tostring(fileCount - 1) .. ".txt",
		WrapSys.LuaPersistPath .. "/lua_m_" .. tostring(fileCount) .. ".txt")
	fileCount = fileCount + 1
	return srcCount, fileCount - 1
end

return MemSys