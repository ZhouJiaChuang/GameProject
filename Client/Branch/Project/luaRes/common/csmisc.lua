--��ӡ��־����
local  IsOpenDebug = true;
local  function  Debug(args)
       if(IsOpenDebug == true)
       then
       print(args);
       end
end;
--��̬��ӵı���
local DynamicValueTable = {};
local function  AddDynamicValue(key,value)
        Debug('AddDynamicValue');
        if(DynamicValueTable[key] == nil)
        then
            DynamicValueTable[key] = value
        end
        return DynamicValueTable[key];
end

--ע�⣬������AddDynamicValue���������ʱ��һ��Ҫ����RemoveDynamicValue����Ȼ�ڴ��й¶
local function RemoveDynamicValue(key)
        DynamicValueTable[key] = nil;
end

return {
    Debug = Debug,
    coroutine_call = coroutine_call,
    AddDynamicValue = AddDynamicValue,
    RemoveDynamicValue = RemoveDynamicValue,
}

