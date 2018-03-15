ezfunLuaTool = {}

--callBack.lua
function GenIndentString(count)
	local indent = ""
	for i=1,count do
		indent = indent .. "\t"
	end
	return indent
end

local bval = true
function serialize(obj, indent_width)  
	if indent_width == nil then
		indent_width = 0
	end

    local lua = ""  
    local t = type(obj)  
    if t == "number" then  
        lua = lua .. obj  
    elseif t == "boolean" then  
        lua = lua .. tostring(obj)  
    elseif t == "string" then  
        lua = lua .. string.format("%q", obj)  
    elseif t == "table" then  
    	local indent_next_width = indent_width + 1
    	local indent = GenIndentString(indent_next_width) 
        lua = lua .. "{\n" .. indent
		for k, v in pairs(obj) do  
			lua = lua .. "[" .. serialize(k) .. "]=" .. serialize(v, indent_next_width) .. ",\n" .. indent  
		end  
		local metatable = getmetatable(obj)  
		if metatable ~= nil and type(metatable.__index) == "table" then  
			for k, v in pairs(metatable.__index) do  
				lua = lua .. "[" .. serialize(k) .. "]=" .. serialize(v, indent_next_width) .. ",\n"  .. indent
			end  
		end  
        lua = string.sub(lua, 1, string.len(lua)-1) .. "}"  
    elseif t == "nil" then  
        return nil  
    else
		if bval then
			bval = false
			print(debug.traceback())
		end
        print("can not serialize a " .. t .. " type.")  
    end  
    return lua  
end  

function ToString(obj)
	return serialize(obj)
end

function CopyTable(st)
	local tab = {}

	for k, v in pairs(st or {}) do
		if type(v) ~= "table" then
			tab[k] = v
		else
			tab[k] = CopyTable(v)
		end
	end

	return tab
end

function table2json(tbl)
	if type(tbl) ~= "table" then
		return ""
	end

    local tmp = {}
    for k, v in pairs(tbl) do
            local k_type = type(k)
            local v_type = type(v)
            local key = (k_type == "string" and "\"" .. k .. "\":")
                or (k_type == "number" and "")
            local value = (v_type == "table" and table2json(v))
                or (v_type == "boolean" and tostring(v))
                or (v_type == "string" and "\"" .. v .. "\"")
                or (v_type == "number" and v)
            tmp[#tmp + 1] = key and value and tostring(key) .. tostring(value) or nil
    end
    if table.maxn(tbl) == 0 then
            return "{" .. table.concat(tmp, ",") .. "}"
    else
            return "[" .. table.concat(tmp, ",") .. "]"
    end
end


function GetTableSize( tbl )
	-- body
	local size = 0
	for k,v in pairs(tbl) do
		size = size + 1
	end
	return size
end
---------------------------------table mgr--------------------------------------------------------------------
---[[attention!!!!!: 所有配置表数据都必须是只读的，但lua没有做限制，为了保证配置数据不变，非要修改必须进行深拷贝，对拷贝数据进行操作]]
local m_hasLoadTables = {}
function GetTableConfig( tableName )
	return LuaTableSys.GetTableDic(tableName)
end
----------------------------------end table mgr-------------------------------------------------------------
function EzfunLuaSafeActive(winroot, trans, state)
	if winroot == nil or trans == nil then
	else
		winroot:SetActive(trans, state)
	end
end

--查找最后一个字符
function ezfunLuaTool.StringFindLastIndex(str, matchStr)
	local ts = string.reverse(str)
	local _, i = string.find(ts, matchStr)
    if i then
	   return string.len(ts) - i + 1
    else
	   return -1
    end
end

-- 获取更小的微信头像,size 可选0，46，64，96，132；0为640*640特殊，其他类似46是46*46，其中64为2的指数幂，能获得unity更好支持
function ezfunLuaTool.GetSmallWeiXinIconUrl(iconurl, size)
	if iconurl ~= nil then
		local len = string.len(iconurl)

		if len <= 0 then
			return ""
		end

		local subIconurl = iconurl
		local sizeSuffix = nil
		local index = ezfunLuaTool.StringFindLastIndex(iconurl, "/")
		if index >= 0 and index < len then
			sizeSuffix = tonumber(string.sub(iconurl, index+1, len))
			
			if sizeSuffix then
				subIconurl = string.sub(iconurl, 1, index - 1)
				subIconurl = subIconurl .. "/" .. size
			end
		end
		
		return subIconurl
	end
	return ""
end


