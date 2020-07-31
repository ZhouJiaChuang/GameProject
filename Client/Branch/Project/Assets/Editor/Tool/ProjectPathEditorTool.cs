using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ProjectPathEditorTool : IEditorTool
{

    public ProjectPathEditorTool()
    {
        CSEditorPath.InitPath();
        PathDic = new Dictionary<string, List<CSEditorPath>>()
        {
            { "工程设置路径", new List<CSEditorPath>(){
                new CSEditorPath( EEditorPath.LocalResourcesLoadPath,"本地资源读取路径"),

            } },
            //{ "工具路径", new List<CSEditorPath>(){
            //    new CSEditorPath(EEditorPath.ExcelTranslateToolFile,"表格工具"),
            //} }
        };
    }

    public Dictionary<string, List<CSEditorPath>> PathDic;
    public void OnGUI()
    {
        if (GUIWindow.DrawHeader("管理"))
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("保存配置", GUILayout.Width(90)))
                {
                    SaveCf();
                }
                if (GUILayout.Button("重置配置", GUILayout.Width(90)))
                {
                    ResetCf();
                }
            }
            GUILayout.EndHorizontal();
        }

        foreach (var pathList in PathDic)
        {
            if (GUIWindow.DrawHeader(pathList.Key))
            {
                OnGUIPathList(pathList.Value);
            }
        }
    }

    private void OnGUIPathList(List<CSEditorPath> list)
    {
        foreach (var path in list)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(path.desc, GUILayout.Width(300));
                path.txt = GUIWindow.TextField(path.txt, GUILayout.Width(800));
                if(GUILayout.Button("OpenPath", GUILayout.Width(100)))
                {
                    if(Directory.Exists(path.txt))
                    {
                        Utility.Open(path.txt);
                    }
                    else
                    {
                        UnityEngine.Debug.LogError(path.txt + " is not exist");
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
    }

    private void SaveCf()
    {
        foreach (var pathList in PathDic)
        {
            foreach (var path in pathList.Value)
            {
                path.Save();
            }
        }
    }


    private void ResetCf()
    {
        foreach (var pathList in PathDic)
        {
            foreach (var path in pathList.Value)
            {
                path.Reset();
            }
        }
    }
    private static string _LocalResourcesLoadPath;
    public static string LocalResourcesLoadPath
    {
        get
        {
            if (string.IsNullOrEmpty(_LocalResourcesLoadPath))
            {
#if UNITY_EDITOR
                if (UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup == UnityEditor.BuildTargetGroup.iOS)
                    _LocalResourcesLoadPath = Application.dataPath.Replace("Client/Branch/ClientIos/Assets", "Data/Branch/CurrentUseData/Normal/wzcq_ios/");
                else
                    _LocalResourcesLoadPath = System.Text.RegularExpressions.Regex.Replace(Application.dataPath, "Client/(\\w+)/" + URL.LocalProjectName + "/Assets", "Data/$1/CurrentUseData/Normal/wzcq_android/");
#endif
            }
            return _LocalResourcesLoadPath;
        }
        set { _LocalResourcesLoadPath = value; }
    }

    private static string _ExcelTablePath;
    public static string ExcelTablePath
    {
        get
        {
            if (string.IsNullOrEmpty(_ExcelTablePath))
            {
                string curPath = Application.dataPath;
                string str1 = "Client/Branch/" + URL.LocalProjectName + "/Assets";
                if (curPath.Contains(str1))
                    _ExcelTablePath = curPath.Replace(str1, "Data/Branch/CurrentUseData/Normal/tableV2/");

                string str2 = "Client/Branch/ClientAndroid_Network/Assets";
                if (curPath.Contains(str2))
                    _ExcelTablePath = curPath.Replace(str2, "Data/Branch/CurrentUseData/Normal/tableV2/");
            }
            return _ExcelTablePath;
        }
        set { _ExcelTablePath = value; }
    }

    private static string _TableProtoSavePath;
    public static string TableProtoSavePath
    {
        get
        {
            if (string.IsNullOrEmpty(_TableProtoSavePath))
            {
                string curPath = Application.dataPath;
                string str1 = "Client/Branch/" + URL.LocalProjectName + "/Assets";
                if (curPath.Contains(str1))
                    _TableProtoSavePath = curPath.Replace(str1, "Data/Branch/CurrentUseData/Normal/proto-table/");
            }

            return _TableProtoSavePath;
        }
        set { _TableProtoSavePath = value; }
    }

    private static string _ServerProtoSavePath;
    public static string ServerProtoSavePath
    {
        get
        {
            if (string.IsNullOrEmpty(_ServerProtoSavePath))
            {
                string curPath = Application.dataPath;
                string str1 = "Client/Branch/" + URL.LocalProjectName + "/Assets";
                if (curPath.Contains(str1))
                    _ServerProtoSavePath = curPath.Replace(str1, "Data/Branch/CurrentUseData/Normal/proto-server/");

                str1 = "Client/Branch/ClientAndroid_Network/Assets";
                if (curPath.Contains(str1))
                    _ServerProtoSavePath = curPath.Replace(str1, "Data/Branch/CurrentUseData/Normal/proto-server/");
            }

            return _ServerProtoSavePath;
        }
        set { _ServerProtoSavePath = value; }
    }


    private static string _TableBytePath;
    public static string TableBytePath
    {
        get
        {
            if (string.IsNullOrEmpty(_TableBytePath))
            {
                string curPath = Application.dataPath;

                string path = string.Empty;
                string str1 = "Client/Branch/" + URL.LocalProjectName + "/Assets";

                if (curPath.Contains(str1))
                {
                    path = str1;
#if UNITY_EDITOR
                    if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
                        _TableBytePath = curPath.Replace(path, "Data/Branch/CurrentUseData/Normal/wzcq_android/Table/");
                    else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
                        _TableBytePath = curPath.Replace(path, "Data/Branch/CurrentUseData/wzcq_ios/Table/");
                    else
                        _TableBytePath = curPath.Replace(path, "Data/Branch/CurrentUseData/wzcq_android/Table/");
#endif
                }
            }

            return _TableBytePath;
        }
        set { _TableBytePath = value; }

    }

    public static string ClientTableProCS_SavePathKey = "ClientTableProCS_SavePath";
    private static string _ClientTableProCS_SavePath;
    public static string ClientTableProCS_SavePath
    {
        get
        {
            if (string.IsNullOrEmpty(_ClientTableProCS_SavePath))
                _ClientTableProCS_SavePath = PlayerPrefs.GetString(ClientTableProCS_SavePathKey);

            if (string.IsNullOrEmpty(_ClientTableProCS_SavePath))
            {
                _ClientTableProCS_SavePath = "";
            }

            return _ClientTableProCS_SavePath;
        }
        set { _ClientTableProCS_SavePath = value; }
    }

    private static string _ServerProCS_SavePath;
    public static string ServerProCS_SavePath
    {
        get
        {
            if (string.IsNullOrEmpty(_ServerProCS_SavePath))
            {
                _ServerProCS_SavePath = "";
            }

            return _ServerProCS_SavePath;
        }
        set { _ServerProCS_SavePath = value; }
    }
}