local cfg_ins = {}
local clubData_sensitive_cfg = {
    [1] = "块",
    [2] = "百",
    [3] = "元",
    [4] = "角",
    [5] = "分",
    [6] = "人民币"
}
function cfg_ins.getTotalCfg()
	if not cfg_ins.table then
		local chat_cfg = require("LuaWindow.Chat.ChatSensitiveCfg")
		local t = {}
		for k, v in pairs (clubData_sensitive_cfg) do
			t[#t+1] = v
		end
		for k, v in pairs (chat_cfg) do
			t[#t+1] = v
		end
		cfg_ins.table = t
	end
	return cfg_ins.table
end

return cfg_ins
