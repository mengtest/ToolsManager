--require "Global.Time"
UpdateBeat = Event("Update", true)
LateUpdateBeat = Event("LateUpdate", true)
CoUpdateBeat = Event("CoUpdate", true)
FixedUpdateBeat = Event("FixedUpdate", true)
UpdateSecond = Event("UpdateSecond", true)
--下面的没有调用了
local m_detlaSecond = 0
function Update(deltatime, unscaledDeltaTime)
	--Time:SetDeltaTime(deltatime, unscaledDeltaTime)
	UpdateBeat(deltatime,unscaledDeltaTime)
	m_detlaSecond = m_detlaSecond + deltatime
	if m_detlaSecond >= 1 then
		m_detlaSecond = 0
		UpdateSecond()
	end
end

function LateUpdate()
	LateUpdateBeat()
	CoUpdateBeat()
end

function FixedUpdate(fixedTime)
	--Time:SetFixedDelta(fixedTime)
	FixedUpdateBeat()
end

function OnLevelWasLoaded(level)
	Time.timeSinceLevelLoad = 0
end
