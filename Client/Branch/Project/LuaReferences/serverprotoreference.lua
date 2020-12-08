---@class account.AccountLoginReqData
---class properties
---@field public type number
---@field public account string 根据类型,存在不同的账号数据


---@class babel.ResBabelData
---class properties
---@field public openLimit number


---@class babel.ReqBabelLevelData
---class properties
---@field public level number


---@class babel.ResBabelLevelData
---class properties
---@field public level number
---@field public level table<number, babel.BabelUnitData>


---@class babel.BabelUnitData
---class properties
---@field public id number 单位唯一ID
---@field public cid number 单位配置ID,根据type的不同,去不同的表中读取相应的配置
---@field public type number 单位类型
---@field public x number 所在x轴坐标
---@field public y number 所在y轴坐标
---@field public z number 所在z轴坐标
---@field public rotate number 单位8方向旋转


---@class character.ResAllCharacterData
---class properties
---@field public characters table<number, character.ResCharacterData>


---@class character.ResCharacterData
---class properties
---@field public id number
---@field public cid number
---@field public level number


---@class city.ResCityData
---class properties
---@field public id number 城市唯一ID
---@field public TerrainAreaInfo table<number, city.ResTerrainAreaInfo> 地面区域信息
---@field public buildInfos table<number, city.ResBuildInfo> 所有建筑信息


---@class city.ResTerrainAreaInfo
---class properties
---@field public id number 区域ID
---@field public state number 区域状态,0:已开启 1:未开启 2:正在开启
---@field public openTime number 开启时间,这里指的是开启倒计时的结束时间戳(毫秒)


---@class city.ResBuildInfo
---class properties
---@field public id number 建筑唯一ID
---@field public cid number 建筑配置ID
---@field public x number
---@field public y number
---@field public rotate number 摆放旋转方向
---@field public state table<number, number> 状态,0:建造中 1:升级中 2:正常
---@field public upTime number 升级或者建造的结束时间


---@class city.ReqOpenTerrinAreaInfo
---class properties
---@field public id number 区域ID


---@class city.ReqCreateBuild
---class properties
---@field public cid number 建筑配置ID
---@field public x number
---@field public y number
---@field public rotate number 摆放旋转方向


---@class city.ReqPutBuild
---class properties
---@field public id number 建筑唯一ID
---@field public x number
---@field public y number
---@field public rotate number 摆放旋转方向


---@class city.ReqRecycleBuild
---class properties
---@field public id number 建筑唯一ID


---@class city.ReqBuildLevelUp
---class properties
---@field public id number 请求升级


---@class city.ReqAccelerateBuildLevel
---class properties
---@field public id number 建造ID
---@field public costItemId number 加速所消耗的道具


---@class city.ReqFinishBuildLevel
---class properties
---@field public id number 建造ID


---@class role.ResPlayerBasicInfo
---class properties
---@field public name string
---@field public nameSpecified boolean
---@field public roleId number
---@field public roleIdSpecified boolean
---@field public attr table<number, role.PlayerAttribute> 属性 对应AttributeType
---@field public x number
---@field public xSpecified boolean
---@field public y number
---@field public ySpecified boolean
---@field public z number
---@field public zSpecified boolean


---@class role.PlayerAttribute
---class properties
---@field public type number
---@field public num number


