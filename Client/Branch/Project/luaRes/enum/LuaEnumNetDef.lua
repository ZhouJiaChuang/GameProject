--[[本文件为工具自动生成,禁止手动修改]]
---网络消息定义
---@class LuaEnumNetDef
LuaEnumNetDef = {}

---客户端链接服务器成功
---ToClient
LuaEnumNetDef.ResConnectSuccessMessage = 101

---客户端链接服务器失败
---ToClient
LuaEnumNetDef.ResConnectFailedMessage = 102

--region ID:1  account
---请求登入账号
---account  AccountLoginReqData
---ToServer
LuaEnumNetDef.ReqLoginAccount = 1001

---返回登入账号信息
---ToClient
LuaEnumNetDef.ResLoginAccount = 1002

---请求登入服务器
---ToServer
LuaEnumNetDef.ReqLoginServer = 1003

---返回服务器信息
---ToClient
LuaEnumNetDef.ResLoginServer = 1004
--endregion

--region ID:2  role
---玩家基本数据
---role  ResPlayerBasicInfo
---ToClient
LuaEnumNetDef.ResPlayerBasicInfoMessage = 2001
--endregion
