using ExtendEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GM其他拓展工具
/// </summary>
public class GMOtherExtendTool : IEditorTool
{
    public void OnGUI()
    {
        if (GUILayout.Button("打开工程"))
        {
            FileUtilityEditor.Open(URL.LocalProjectPath);
        }
        if (GUILayout.Button("打开工程缓存配置文件夹"))
        {
            FileUtilityEditor.Open(URL.LocalProjectPath + "/ProjectCaches");
        }
    }

    public void Load()
    {
    }


    public void Save()
    {
    }
}
