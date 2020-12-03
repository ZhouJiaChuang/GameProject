using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 输入对话框
/// </summary>
public class ServerTool_ProtocolController_InputDialog : EditorWindow
{
    private string[] instructions;
    private Action<bool, string[], bool> callback;
    private string[] inputStrs;
    private bool toggle;
    private string toggleInfo;

    public void Initialize(string[] instructions, string toggleInfo, Action<bool, string[], bool> callback)
    {
        this.instructions = instructions;
        this.callback = callback;
        inputStrs = new string[instructions.Length];
        for (int i = 0; i < inputStrs.Length; i++)
        {
            inputStrs[i] = string.Empty;
        }
        toggle = false;
        this.toggleInfo = toggleInfo;
    }

    private void OnGUI()
    {
        for (int i = 0; i < instructions.Length; i++)
        {
            inputStrs[i] = EditorGUILayout.TextField(instructions[i], inputStrs[i]);
        }
        if (!string.IsNullOrEmpty(toggleInfo))
        {
            toggle = EditorGUILayout.ToggleLeft(toggleInfo, toggle);
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("确认"))
        {
            Close();
            if (callback != null)
            {
                callback(true, inputStrs, toggle);
            }
        }
        else if (GUILayout.Button("取消"))
        {
            Close();
            if (callback != null)
            {
                callback(false, inputStrs, toggle);
            }
        }
        EditorGUILayout.Space();
    }
}
