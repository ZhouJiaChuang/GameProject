--[[本文件为工具自动生成]]
--[[本文件用于向服务器发送消息前,对发送的消息进行预校验,返回的bool值决定该消息是否应当发送,可编辑区域为所生成的每个方法内部,对可编辑区域外的修改将在工具下次修改时作废]]
--[[不建议在方法内使用--region和--endregion,以免干扰工具读取]]
---网络消息预校验
netMsgPreverifying = {}

netMsgPreverifying.__index = netMsgPreverifying

--account.xml
require "luaRes.netMsgPreverifying.NetMsgPreverifying_Account"
