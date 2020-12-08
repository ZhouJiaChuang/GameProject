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

--region ID:10  city
---返回城市的所有数据
---city  ResCityData
---ToClient
LuaEnumNetDef.ResCityDataMessage = 10001

---请求开启地面区域
---city  ReqOpenTerrinAreaInfo
---ToServer
LuaEnumNetDef.ReqOpenTerrinAreaInfoMessage = 10002

---返回开启地面区域信息
---city  ResTerrainAreaInfo
---ToClient
LuaEnumNetDef.ResTerrainAreaInfoMessage = 10003

---请求创建建筑
---city  ReqCreateBuild
---ToServer
LuaEnumNetDef.ReqCreateBuildMessage = 10004

---返回创建建筑信息
---city  ResBuildInfo
---ToClient
LuaEnumNetDef.ResBuildInfoMessage = 10005

---请求摆放建筑
---city  ReqPutBuild
---ToServer
LuaEnumNetDef.ReqPutBuildMessage = 10006

---请求回收建筑
---city  ReqRecycleBuild
---ToServer
LuaEnumNetDef.ReqRecycleBuildMessage = 10007

---请求建造等级提升
---city  ReqBuildLevelUp
---ToServer
LuaEnumNetDef.ReqBuildLevelUpMessage = 10008

---请求加速建造等级
---city  ReqAccelerateBuildLevel
---ToServer
LuaEnumNetDef.ReqAccelerateBuildLevelMessage = 10009

---请求立即完成建造等级
---city  ReqFinishBuildLevel
---ToServer
LuaEnumNetDef.ReqFinishBuildLevelMessage = 10010
--endregion

--region ID:11  babel
---返回通天塔的基础数据
---babel  ResBabelData
---ToClient
LuaEnumNetDef.ResBabelDataMessage = 11001

---请求指定层的通天塔数据
---babel  ReqBabelLevelData
---ToServer
LuaEnumNetDef.ReqBabelLevelDataMessage = 11002

---返回指定层的通天塔数据
---babel  ResBabelLevelData
---ToClient
LuaEnumNetDef.ResBabelLevelDataMessage = 11003
--endregion
