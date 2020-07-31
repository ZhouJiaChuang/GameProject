using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSEditorPath
{
    private static string AppLocationName = Application.dataPath;
    private static Dictionary<EEditorPath, string> pathDic = new Dictionary<EEditorPath, string>();
    public EEditorPath type = EEditorPath.None;

    public string desc = string.Empty;
    public string defaultValue = string.Empty;
    public string txt = string.Empty;

    private static string keySpit = "_";
    private string Key
    {
        get
        {
            return GetKeyPath(type);
        }
    }

    public CSEditorPath(EEditorPath _type, string desc)
    {
        this.type = _type;
        this.desc = desc;
        this.defaultValue = GetPath(_type);

        txt = PlayerPrefs.GetString(Key);
        if (string.IsNullOrEmpty(txt))
        {
            txt = this.defaultValue;
        }
    }

    public void Save()
    {
        txt = txt.Replace("\\", "/");
        if (Directory.Exists(txt) && txt.LastIndexOf("/") != txt.Length - 1)
        {
            txt = txt + "/";
        }

        SavePath(type, txt);
    }

    public void Reset()
    {
        txt = this.defaultValue;
    }

    /// <summary>
    /// 得到保存的路径
    /// </summary>
    /// <param name="_type"></param>
    /// <returns></returns>
    public static string Get(EEditorPath _type)
    {
        InitPath();
        return pathDic[_type];
        //return PlayerPrefs.GetString(GetKeyPath(_type));
    }

    public static string GetKeyPath(EEditorPath _type)
    {
        return AppLocationName + keySpit + _type.ToString();
    }

    public static void InitPath()
    {
        if (pathDic.Count != 0)
        {
            return;
        }

        string initPath = "";
        initPath = EditorPath_LocalResourcesLoadPath;
        AddPath(EEditorPath.LocalResourcesLoadPath, initPath);
        //initPath = EditorPath.GetBackDir(Application.dataPath, 4) + "/Data/Branch/CurrentUseData/Normal/tableV2/";
        //AddPath(EEditorPath.ExcelTablePath, initPath);
        //initPath = EditorPath.GetBackDir(Application.dataPath, 4) + "/Data/Branch/CurrentUseData/Normal/proto-table/";
        //AddPath(EEditorPath.TableProtoSavePath, initPath);

        //string curPath = Application.dataPath;

        //string path = "Client/Branch/" + EditorPath.LocalProjectName + "/Assets";

        //if (curPath.Contains(path))
        //{
        //    initPath = CSInterfaceSingleton.Unity_Editor.EditorPath_TableBytePath(Application.dataPath, path);
        //}
        //AddPath(EEditorPath.TableBytePath, initPath);

        //initPath = EditorPath.GetBackDir(Application.dataPath, 4) + "/ClassLibrary_wzcq3D/ClassLibrary_Public/Proto/protobufProduce/ClientProto/"; ;
        //AddPath(EEditorPath.ClientTableProCS_SavePath, initPath);

        //initPath = EditorPath.GetBackDir(Application.dataPath, 4) + "/Data/Branch/CurrentUseData/Normal/proto-server/";
        //AddPath(EEditorPath.ServerProtoSavePath, initPath);
        //initPath = EditorPath.GetBackDir(Application.dataPath, 4) + "/ClassLibrary_wzcq3D/ClassLibrary_Public/Proto/protobufProduce/ServerProto/"; ;
        //AddPath(EEditorPath.ServerProCS_SavePath, initPath);
        //initPath = EditorPath.GetBackDir(Application.dataPath, 6) + "/Wz2gData/trunk/proto/"; ;
        //AddPath(EEditorPath.ServerProtocolProtoPath, initPath);
        //initPath = EditorPath.GetBackDir(Application.dataPath, 6) + "/Wz2gData/trunk/xml/"; ;
        //AddPath(EEditorPath.ServerProtocolXMLPath, initPath);
        //initPath = Application.dataPath + "/Script/Network/MsgGen/";
        //AddPath(EEditorPath.MsgGenPath, initPath);

        //initPath = "";
        //AddPath(EEditorPath.ProjectOverridePath, initPath);

        //initPath = "";
        //AddPath(EEditorPath.ProjectOverrideName, initPath);


        //initPath = EditorPath.GetBackDir(Application.dataPath, 1) + "/luaRes/";
        //AddPath(EEditorPath.LuaPath, initPath);
        //initPath = Application.dataPath.Replace("/Client/Branch/ClientProject/Assets", "/Tool/ExcelTool/ExcelTool.exe");
        //AddPath(EEditorPath.ExcelTranslateToolFile, initPath);
    }

    private static void AddPath(EEditorPath type, string initPath)
    {
        string path = PlayerPrefs.GetString(GetKeyPath(type));
        if (string.IsNullOrEmpty(path))
        {
            path = initPath;
        }

        pathDic.Add(type, path);
        PlayerPrefs.SetString(CSEditorPath.GetKeyPath(type), path);
    }

    public static string GetPath(EEditorPath type)
    {
        if (pathDic.Count == 0)
        {
            InitPath();
        }

        return pathDic[type];
    }

    public static void SavePath(EEditorPath type, string path)
    {
        if (pathDic.ContainsKey(type))
        {
            pathDic[type] = path;
        }
        else
        {
            pathDic.Add(type, path);
        }

        PlayerPrefs.SetString(CSEditorPath.GetKeyPath(type), path);
    }

    public static string EditorPath_LocalResourcesLoadPath
    {
        get
        {
            string _LocalResourcesLoadPath = System.Text.RegularExpressions.Regex.Replace(Application.dataPath, "Client/(\\w+)/" + URL.LocalProjectName + "/Assets", "Data/$1/CurrentUseData/Normal/wzcq_android/");
            return _LocalResourcesLoadPath;
        }
    }
}