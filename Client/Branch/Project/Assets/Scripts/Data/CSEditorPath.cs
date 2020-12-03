using System;
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
        if (!string.IsNullOrEmpty(txt))
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

        AddPath(EEditorPath.LocalServerProtoClassPath);
        AddPath(EEditorPath.LocalTableProtoClassPath);
        AddPath(EEditorPath.LuaPath);
        AddPath(EEditorPath.ClientLoadRootPath);
        AddPath(EEditorPath.LocalResourcesLoadPath);
        AddPath(EEditorPath.LocalTableProtoPath);
        AddPath(EEditorPath.LocalServerProtoPath);
        AddPath(EEditorPath.LocalServerProtoXMLPath);
        
        AddPath(EEditorPath.LocalServerProtoFunctionPath);
        AddPath(EEditorPath.LocalTablePath);
        AddPath(EEditorPath.LocalTableBytesPath);

        AddPath(EEditorPath.RealServerProtoPath);
        AddPath(EEditorPath.RealServerProtoXMLPath);
    }

    private static void AddPath(EEditorPath type)
    {
        string path = PlayerPrefs.GetString(GetKeyPath(type));
        if (string.IsNullOrEmpty(path))
        {
            path = GetDefaultPath(type);
        }

        pathDic[type] = path;

        PlayerPrefs.SetString(GetKeyPath(type), path);
    }

    public static string GetPath(EEditorPath type)
    {
        if (pathDic.Count == 0)
        {
            InitPath();
        }
        string path = string.Empty;
        if (pathDic.TryGetValue(type, out path))
        {
        }
        return path;
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

    private static string GetDefaultPath(EEditorPath type)
    {
        string path = string.Empty;
        switch (type)
        {
            case EEditorPath.LocalTableProtoClassPath:
                path = Application.dataPath + "/Scripts/Proto/Table/";
                break;
            case EEditorPath.LocalServerProtoClassPath:
                path = Application.dataPath + "/Scripts/Proto/Server/";
                break;

            case EEditorPath.LocalServerProtoFunctionPath:
                path = Application.dataPath + "/Scripts/Proto/ServerGen/";
                break;

            case EEditorPath.ClientLoadRootPath:

                path = System.Text.RegularExpressions.Regex.Replace(Application.dataPath, "Client/(\\w+)/" + URL.LocalProjectName + "/Assets",
                    "Data/$1/Normal/");
                break;

            case EEditorPath.LocalResourcesLoadPath:
                path = System.Text.RegularExpressions.Regex.Replace(Application.dataPath, "Client/(\\w+)/" + URL.LocalProjectName + "/Assets",
                    "Data/$1/Normal/android/");
                break;

            case EEditorPath.LocalTableProtoPath:
                path = System.Text.RegularExpressions.Regex.Replace(Application.dataPath, "Client/(\\w+)/" + URL.LocalProjectName + "/Assets",
                    "Data/$1/Normal/proto-table/");
                break;

            case EEditorPath.LocalServerProtoPath:
                path = System.Text.RegularExpressions.Regex.Replace(Application.dataPath, "Client/(\\w+)/" + URL.LocalProjectName + "/Assets",
                    "Data/$1/Normal/proto-server/");
                break;
            case EEditorPath.LocalServerProtoXMLPath:
                path = System.Text.RegularExpressions.Regex.Replace(Application.dataPath, "Client/(\\w+)/" + URL.LocalProjectName + "/Assets",
                    "Data/$1/Normal/proto-server-xml/");
                break;

            case EEditorPath.LocalTablePath:
                path = System.Text.RegularExpressions.Regex.Replace(Application.dataPath, "Client/(\\w+)/" + URL.LocalProjectName + "/Assets",
                    "Data/$1/Normal/table/");
                break;
            case EEditorPath.LocalTableBytesPath:
                path = System.Text.RegularExpressions.Regex.Replace(Application.dataPath, "Client/(\\w+)/" + URL.LocalProjectName + "/Assets",
                    "Data/$1/Normal/android/table/");
                break;
            case EEditorPath.LuaPath:
                path = URL.LocalProjectPath+ "/luaRes/";
                break;

                
        }
        return path;
    }
}