local function Read(path)
     --print('read', path, type(path))
   local file = io.open(path, "r");
   assert(file);
   local data = file:read("*a"); -- ��ȡ��������
   file:close();
   return data;
 end

 local function Write(path,content)
   local file = io.open(path, "w");
   assert(file);
   file:write(content);
   file:close();
 end

 local function ReadLuaFile(fName)
    
    local filePath = ''

    if(isUnityEditor) then
        filePath = string.gsub(CS.UnityEngine.Application.dataPath, 'Assets', fName)
    else
        filePath = CS.UnityEngine.Application.persistentDataPath .."/" .. fName
    end
    -- print('file_util.ReadLuaFile', filePath, fName)
    
    return Read(filePath)
 end

local function Append(path, content)
    local file = io.open(path, "a")
    assert(file)
    file:write(content)
    file:close()
end

 return {
    Read = Read,
    Write = Write,
    ReadLuaFile = ReadLuaFile,
    Append = Append,
}