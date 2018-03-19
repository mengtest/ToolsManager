--输出日志--
function log(str)
  WrapSys.Util_Log(str);
end

--打印字符串--
-- function print(str)
--  WrapSys.Util_Log(str);
-- end
--_print = print
function print( ... )
  local args = {...}

  if 1 == #args then
    WrapSys.Util_Log(args[1]);
  else
    local tmp = ""
    for i=1, #args do
      tmp = tmp .. tostring(args[i]) .. "    "
    end
    WrapSys.Util_Log(tmp)
  end
end
--用这个来做原来的抛出异常用吧
orgin_error = error
--错误日志--  这里用的这么多了，我能怎么办，我也很绝望啊
function error(str) 
	WrapSys.Util_LogError(str);
end

function logError(str)
  str = str .. "\n" .. debug.traceback() .. "\n"
	WrapSys.Util_LogError(str);
end

function LoadNetString(str)
	return loadstring("return " .. str)
end

--警告日志--
function warn(str) 
	WrapSys.Util_LogWarning(str);
end

function LimitLenStr(str, limit)
  if not str then
      return ""
  end
  
  local str_len = string.len(str)
  if str_len <= limit then
      return str
  else 
      return string.sub(str, 1, limit)
  end
end

function string.findLast(str, content)
	local curPos = 1
	while true do
		local curMatchPos = string.find(str, content, curPos + 1)
		if curMatchPos then
			curPos = curMatchPos
		else
			return string.sub(str, curPos + 1)
		end
	end
end

function CreateObject(luaPath,...)
	local class = require(luaPath)
	if (class) and type(class) == "table" and class.New then
		local data =  class.New(...)
		return data
	end
	return nil
end

function traceback(msg)
	msg = debug.traceback(msg, 2)
	logError("-----------" .. msg .. "----------------")
	return msg
end

--深copy
function DeepCopy(orig)
  local copy
  if type(orig) == "table" then
    copy = {}
    for orig_key, orig_value in pairs(orig) do
      copy[DeepCopy(orig_key)] = DeepCopy(orig_value)
    end
    setmetatable(copy, DeepCopy(getmetatable(orig)))
  else
    copy = orig
  end
  return copy
end
-- 浅拷贝
function ShallowCopy(orig)
  local copy
  if type(orig) == "table" then
    copy = {}
    for orig_key, orig_value in pairs(orig) do
      copy[orig_key] = orig_value
    end
  else -- number, string, boolean, etc
    copy = orig
  end
  return copy
end

-- table 迭代器

function pairsByKeys(t)  
    local a = {}  
    for n in pairs(t) do  
        a[#a+1] = n  
    end  
    table.sort(a)  
    local i = 0  
    return function()  
        i = i + 1  
        return a[i], t[a[i]]  
    end  
end 

-- 给gb添加name组件，is_instance为false参考FrameSelect，is_instance为true参考DragMove
function GetOrAddLuaComponent(gb, path, is_instance)
	local component = WrapSys.GetOrAddComponentBaseLua(gb)
	component:Init(path, is_instance)

	return component
end

-- 判断table是否为空
function isNilOrEmpty(tab)
  if nil == tab or 0 == #tab then
    return true
  end

  return false
end

-- 分割字符串
function split(str, splitstr)
    local result = {}
    if str == nil or str == "" or split == nil then
        return result
    end

    for match in (str .. splitstr):gmatch("(.-)" .. splitstr) do
        table.insert(result, match)
    end
    return result
end

-- 返回 num1 除 num2 整数部分与余数部分
function Division(num1, num2)
	local integer = math.modf(num1 / num2)
    local remainder = math.fmod(num1 , num2)

    return integer, remainder
end

-- 是否是偶数
function IsEven(number)
    if type(number) ~= "number" then return false end

    local remainder = math.fmod(number, 2)

    return remainder == 0
end

-- 是否是奇数
function IsOdd(number)
    if type(number) ~= "number" then return false end

    local remainder = math.fmod(number, 2)

    return remainder == 1
end