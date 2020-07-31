using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProjectSettingPanel : CSEditorWindow
{

    [MenuItem("Tools/工程设置面板")]
    public static void OpenDataTool()
    {
        ProjectSettingPanel wnd = EditorWindow.GetWindow<ProjectSettingPanel>("工程设置");
        wnd.Show();
    }

    protected override void InitToolDic()
    {
        AddTool(new ProjectPathEditorTool());
    }

}
