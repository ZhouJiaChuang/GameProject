using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// GM道具生成工具
/// </summary>
public class GMItemGenerateTool : IEditorTool
{
    public void OnGUI()
    {
        OnGUI_Refush();

        GUIWindow.DrawSeparator();

        OnGUI_Detail();

        GUIWindow.DrawSeparator();

        OnGUI_Navigation();

        GUIWindow.DrawSeparator();

        OnGUI_ItemGrid();

    }

    private void OnGUI_Refush()
    {
        if (GUILayout.Button("刷新"))
        {

        }
    }

    private void OnGUI_Detail()
    {

    }

    string SerchTxt = string.Empty;
    private void OnGUI_Navigation()
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("搜索:", GUILayout.Width(40));
            SerchTxt = EditorGUILayout.TextField(SerchTxt);
        }
        GUILayout.EndHorizontal();
    }

    private void OnGUI_ItemGrid()
    {

    }

    public void Save()
    {
    }

    public void Load()
    {
    }
}
