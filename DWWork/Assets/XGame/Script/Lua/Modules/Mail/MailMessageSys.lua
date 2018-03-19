--[[
    @author: mid
    @date: 2018年1月30日10:21:12
    @desc: 邮件消息系统
]]
--[[文件域全局]]
MailMessageSys = {}
local self = MailMessageSys
local Enum_mailMsgType = {
    ApplyJoinClub = 1,
    ExitClub = 2,
    DisbandClub = 3,
    JoinClubSuccess = 4,
    ChargeRoomCard = 5,
    ReturnRoomCard = 6,
}
-- 初始化
function MailMessageSys.Init()
    self.Clean()
end
-- 反初始化
function MailMessageSys.UnInit()
    self.Clean()
end
-- 擦除数据
function MailMessageSys.Clean()
    self.msgList = {}
end
-- 设置数据
function MailMessageSys.SetData(data)
    local dealed_data = self._dealData(data)
    -- self.msgList = dealed_data
end
-- 处理数据
function MailMessageSys._dealData(data)

end
-- 根据类型添加数据
function MailMessageSys.AddItemByType(type,item_data)
end
-- 根据类型和消息id删除数据
function MailMessageSys.DelItemByType(type,item_data)
end
-- 根据类型获取数据
function MailMessageSys.GetListByType(type)
    return self.msgList[type]
end
-- 判断是否显示小红点
function MailMessageSys.IsShowRedPoint()

end


