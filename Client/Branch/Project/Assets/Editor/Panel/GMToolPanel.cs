using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GMToolPanel : CSEditorWindow
{

    [MenuItem("Tools/GM工具面板")]
    public static void OpenDataTool()
    {
        GMToolPanel wnd = EditorWindow.GetWindow<GMToolPanel>("GM工具面板");
        wnd.Show();
    }

    protected override void InitToolDic()
    {
        AddTool(new ProjectPathEditorTool());
    }

}