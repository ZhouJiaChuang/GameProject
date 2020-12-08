--[[本文件为工具自动生成]]
--[[本文件用于网络消息分发之前,根据网络消息进行全局预处理,可编辑区域为所生成的每个方法内部,对可编辑区域外的修改将在工具下次修改时作废]]
--[[不建议在方法内使用--region和--endregion,以免干扰工具读取]]
---网络消息预处理
netMsgPreprocessing = {}

netMsgPreprocessing.__index = netMsgPreprocessing

--region ID:101 ResConnectSuccessMessage 客户端链接服务器成功
---@param msgID LuaEnumNetDef 消息ID
---@param tblData table lua table类型消息数据
---@param csData userdata C# class类型消息数据(nil)
netMsgPreprocessing[101] = function(msgID, tblData, csData)
    --在此处填入预处理代码
end
--endregion

--region ID:102 ResConnectFailedMessage 客户端链接服务器失败
---@param msgID LuaEnumNetDef 消息ID
---@param tblData table lua table类型消息数据
---@param csData userdata C# class类型消息数据(nil)
netMsgPreprocessing[102] = function(msgID, tblData, csData)
    --在此处填入预处理代码
end
--endregion

--[[
--region ID:12001 ResAllCharacterDataMessage 返回所有角色数据
---@param msgID LuaEnumNetDef 消息ID
---@param tblData table lua table类型消息数据
---@param csData userdata C# class类型消息数据(nil)
netMsgPreprocessing[12001] = function(msgID, tblData, csData)
    --在此处填入预处理代码
end
--endregion
--]]

--[[
--region ID:12002 ResCharacterDataMessage 返回单个角色数据
---@param msgID LuaEnumNetDef 消息ID
---@param tblData table lua table类型消息数据
---@param csData userdata C# class类型消息数据(nil)
netMsgPreprocessing[12002] = function(msgID, tblData, csData)
    --在此处填入预处理代码
end
--endregion
--]]

--account.xml
require "luaRes.netMsgPreprocessing.NetMsgPreprocessing_Account"

--role.xml
require "luaRes.netMsgPreprocessing.NetMsgPreprocessing_Role"

--city.xml
require "luaRes.netMsgPreprocessing.NetMsgPreprocessing_City"

--babel.xml
require "luaRes.netMsgPreprocessing.NetMsgPreprocessing_Babel"
