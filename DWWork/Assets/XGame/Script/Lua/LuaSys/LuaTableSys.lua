--------------------------------------------------------------------------------
-- 	 File      : LuaTableSys.lua
--   author    : guoliang
--   desc      : 
--   version   : 1.0
--   date      : 2017-07-03 20:12:46.
--   copyright : Copyright 2016 EZFun Inc.
--------------------------------------------------------------------------------

LuaTableSys = {}

local m_loadDic = {}

function LuaTableSys.GetTableDic(str)
	if not str then
		return nil
	end
	local dic = require("Table." .. str)
	m_loadDic[str] = dic
	return dic
end

function LuaTableSys.GetEntry(str, Id)
	local dic = LuaTableSys.GetTableDic(str)
	if dic then 
		return dic[Id]
	end
	return nil
end

function LuaTableSys.Clear()
	m_loadDic = {}
end

return LuaTableSys