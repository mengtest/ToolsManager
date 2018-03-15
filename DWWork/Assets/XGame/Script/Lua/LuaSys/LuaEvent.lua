--------------------------------------------------------------------------------
-- 	 File      : LuaEvent.lua
--   author    : guoliang
--   version   : 1.0
--   date      : 2017-04-10 14:24:46.
--   copyright : Copyright 2016 EZFun Inc.
--------------------------------------------------------------------------------

LuaEvent = {}

local m_eventDic = {}

local m_luaNewEventIdStartIndex = 0

local m_luaEventNewDic = {}

local m_luaEvenetList = {}

function LuaEvent.Init()
	-- 对EEventType扩展，当使用了没有定义的协议号时，就对其进行扩展，但是这些协议内容之在lua中使用就不会再发到C#中
	local eeventTypeMetaTable = getmetatable(EEventType)
	if not eeventTypeMetaTable then
		eeventTypeMetaTable = {}
	end
	m_luaNewEventIdStartIndex = EEventType.Max
	eeventTypeMetaTable.__index = function (t, key)
		local value = m_luaEventNewDic[key] 
		if not value then
			m_luaNewEventIdStartIndex = m_luaNewEventIdStartIndex + 1
			m_luaEventNewDic[key] = m_luaNewEventIdStartIndex
			m_luaEventNewDic[m_luaNewEventIdStartIndex] = key
			value = m_luaNewEventIdStartIndex
		end
		return value
	end
	setmetatable(EEventType, eeventTypeMetaTable)
end

function LuaEvent.AddHandle(eventid, func, obj)

	local list = m_eventDic[eventid]
	local isLuaEvent = m_luaEventNewDic[eventid]
	if not list then
		m_eventDic[eventid] = Event(eventid, true)
		if not isLuaEvent then
			WrapSys.LuaRootSys_AddLuaEvent(eventid)
		end
		--print("eventid:" .. tostring(eventid))
		list =  m_eventDic[eventid]
	end
	list:Add(func, obj)
end

function LuaEvent.RemoveHandle(eventid, func, obj)
	local list = m_eventDic[eventid]
	local isLuaEvent = m_luaEventNewDic[eventid]
	if list then
		list:Remove(func, obj)
	end
	if list and list:Count() == 0 then
		m_eventDic[eventid] = nil
		if not isLuaEvent then
			WrapSys.LuaRootSys_RemoveLuaEvent(eventid)
		end
	end
end

function LuaEvent.AddEventNow(eventid, p1, p2)
	-- body
	local list = m_eventDic[eventid]
	local isLuaEvent = m_luaEventNewDic[eventid]
	if isLuaEvent then
		if list then
			list(eventid, p1, p2)
		end
	else 
		WrapSys.EventSys_AddEventNow(eventid, p1, p2)
	end
end

function LuaEvent.AddEvent(eventid, p1, p2)
	--print("eventid:" .. tostring(eventid))
	local list = m_eventDic[eventid]
	local isLuaEvent = m_luaEventNewDic[eventid]

	if isLuaEvent then
		local tempDic = {}
		tempDic.eventid = eventid
		tempDic.p1 = p1
		tempDic.p2 = p2
		table.insert(m_luaEvenetList, tempDic)
	else
		WrapSys.EventSys_AddEvent(eventid, p1, p2)
	end
end


function LuaEvent.Update()
	if m_luaEvenetList and #m_luaEvenetList > 0 then
		local m_luaEvenetListTemp = m_luaEvenetList
		m_luaEvenetList = {}
		for i,v in ipairs(m_luaEvenetListTemp) do
			LuaEvent.HandleEvent(v.eventid, v.p1, v.p2)
		end
	end
end

function LuaEvent.HandleEvent(eventid, p1, p2)

	local list = m_eventDic[eventid]
	if list then
		list(eventid, p1, p2)
	end
end

return LuaEvent