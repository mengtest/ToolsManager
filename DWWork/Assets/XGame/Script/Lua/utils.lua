--------------------------------------------------------------------------------
-- 	 File      : utils.lua
--   author    : 
--   desc      : 
--   version   : 1.0
--   date      : 2017-06-17 14:07:04.
--   copyright : Copyright 2016 EZFun Inc.
--------------------------------------------------------------------------------

local M = {}

-- function M.SafeSerialize(serialized_tables, obj)
--     if serialized_tables[obj] then
--         return "TB_x"
--     else 
--         return "M.serialize"
--     end    
-- end

function M.serialize(obj, depth, serialized_tables, table_seq)
    -- 记录已经解析过的table
    serialized_tables = serialized_tables or {}
    serialized_tables.table_seq = serialized_tables.table_seq or 0

	local depth = depth or 0
	local lua = ""
    local t = type(obj)  

    local pre = "\t"
    local lastpre = ""
    for i=1,depth do
    	lastpre = pre
    	pre = pre .. "\t"
    end
    if t == "number" then  
        lua = lua .. obj  
    elseif t == "boolean" then  
        lua = lua .. tostring(obj)  
    elseif t == "string" then  
        lua = lua .. string.format("%q", obj)  
    elseif t == "table" then  
        -- 取元表，判断用
        local metatable = getmetatable(obj)  
        -- 没有解析过的表才解析，解析过的直接赋值table编号
        if not serialized_tables[obj] then
            -- 解析的是table，记录下来
            serialized_tables.table_seq = serialized_tables.table_seq + 1
            serialized_tables[obj] = serialized_tables.table_seq

            lua = lua .. "TB_" .. serialized_tables.table_seq .. "{\n"

            for k, v in pairs(obj) do  
                if type(v) ~= "function" then
                    -- 用自己当key，这个其实不太可能出现
                    if not serialized_tables[k] then
                        lua = lua .. pre .. "[" .. M.serialize(k, depth + 1, serialized_tables) .. "]="
                    else
                        lua = lua .. pre .. "[TB_" .. serialized_tables[k] .. "]="
                    end
                    -- 没有解析过的表才解析，解析过的直接赋值table编号
                    -- 解析过的表如果再去解析很容易造成死循环，爆堆栈，
                    -- 例如a.class=a，1.解析a，2.碰到一个元素class，开始解析class；class就是a，接着又是第1步，12121212无限循环
                    if not serialized_tables[v] then
                        if metatable ~= nil and metatable == v then
                            lua = lua .. "metatable,\n"  
                        else 
                            lua = lua .. M.serialize(v, depth + 1, serialized_tables) .. ",\n"  
                        end
                    else 
                        lua = lua .. "TB_" .. serialized_tables[v] .. ",\n"  
                    end
                end
        	end  
            -- 元表解析
            if metatable ~= nil and type(metatable.__index) == "table" then  
                if not serialized_tables[metatable.__index] then
                    -- 元表是table，也记录下来
                    serialized_tables.table_seq = serialized_tables.table_seq + 1
                    serialized_tables[metatable.__index] = serialized_tables.table_seq
                    
                    lua = lua .. pre .. "metatable=" .. "TB_" .. serialized_tables.table_seq .. "{\n" 
                    for k, v in pairs(metatable.__index) do  
                        if type(v) ~= "function" then
                            if not serialized_tables[k] then
                                lua = lua .. pre .. "\t" .. "[" .. M.serialize(k, depth + 2, serialized_tables) .. "]="
                            else 
                                lua = lua .. pre .. "\t" .. "[TB_" .. serialized_tables[k] .. "]="
                            end
                            if not serialized_tables[v] then
                                lua = lua .. M.serialize(v, depth + 2, serialized_tables) .. ",\n"  
                            else
                                lua = lua .. "TB_" .. serialized_tables[v] .. ",\n"  
                            end
                        end
                    end
                    lua = lua .. pre ..  "},\n"  
                else 
                    lua = lua .. pre .. "metatable=" .. "TB_" .. serialized_tables[metatable.__index] .. ",\n"
                end
            end
            lua = lua .. lastpre ..  "}"  
        else
            lua = lua .. "TB_" .. serialized_tables[obj] .. ",\n" 
        end
    elseif t == "nil" then  
        return "nil"  
    elseif t == "userdata" then
		return "userdata"
	elseif t == "function" then
		return "function"
	else  
        error("jesus,can not serialize a " .. t .. " type.")
    end  
    return lua
end

function M.print(...)
	local t = {...}
	local ret = {}
	for _,v in pairs(t) do
		table.insert(ret, M.serialize(v))
	end
	print(table.concat(ret, ", "))
end

function M.split(str, delimiter)
	if str==nil or str=='' or delimiter==nil then
		return nil
	end
	
    local result = {}
    for match in (str..delimiter):gmatch("(.-)"..delimiter) do
        table.insert(result, match)
    end
    return result
end

function M.hex(str)
	local len = #str
	local ret = ""
	for i=1,len do
		local c = tonumber(str:byte(i))
		local cstr = string.format("%02X ", c)
		ret = ret .. cstr
	end
	print(ret)
end

return M