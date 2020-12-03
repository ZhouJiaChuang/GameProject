--打印日志开关
local  IsOpenDebug = true;
local  function  Debug(args)
       if(IsOpenDebug == true)
       then
       print(args);
       end
end;
--动态添加的变量
local DynamicValueTable = {};
local function  AddDynamicValue(key,value)
        Debug('AddDynamicValue');
        if(DynamicValueTable[key] == nil)
        then
            DynamicValueTable[key] = value
        end
        return DynamicValueTable[key];
end

--注意，调用了AddDynamicValue后，在清理的时候一定要调用RemoveDynamicValue，不然内存会泄露
local function RemoveDynamicValue(key)
        DynamicValueTable[key] = nil;
end

return {
    Debug = Debug,
    coroutine_call = coroutine_call,
    AddDynamicValue = AddDynamicValue,
    RemoveDynamicValue = RemoveDynamicValue,
}

