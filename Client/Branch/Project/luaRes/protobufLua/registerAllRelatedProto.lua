--[[本文件为工具自动生成,禁止手动修改]]
---加载所有相关proto
local LoadAllRelatedProto = {}

---@param protobufMgr protobuf管理器
function LoadAllRelatedProto:LoadAll(protobufMgr)
    ---luaTable->C#数据结构转换
    protobufMgr.DecodeTable = {}
    ---调整luaTable
    protobufMgr.AdjustTable = {}

    protobufMgr.RegisterPb("account.proto")
    ---未生成 account.proto 的lua=>C#文件

    protobufMgr.RegisterPb("role.proto")
    ---未生成 role.proto 的lua=>C#文件

end

return LoadAllRelatedProto