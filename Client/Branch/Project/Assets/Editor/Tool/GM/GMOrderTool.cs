using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// GM命令工具
/// </summary>
public class GMOrderTool : IEditorTool
{
    public void OnGUI()
    {
        OnGUI_CustomOrder();

        GUIWindow.DrawSeparator();
    }

    #region 自定义命令栏
    string CustomOrderTxt = string.Empty;
    private void OnGUI_CustomOrder()
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("命令:", GUILayout.Width(40));

            CustomOrderTxt = EditorGUILayout.TextField(CustomOrderTxt, GUILayout.Width(200));
            if (GUILayout.Button("发送", GUILayout.Width(40)))
            {

            }
        }
        GUILayout.EndHorizontal();
    }
    #endregion

    public void Save()
    {
    }

    public void Load()
    {
    }
}
