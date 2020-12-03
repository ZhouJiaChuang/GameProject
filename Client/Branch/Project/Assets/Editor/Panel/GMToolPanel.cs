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
        AddToolBar("GM命令栏", new GMOrderTool());
        AddToolBar("道具生成工具", new GMItemGenerateTool());
        AddToolBar("单位生成工具", new GMAvaterGenerateTool());
        AddToolBar("游戏信息监听", new GMGameInfoListeningTool());
        AddToolBar("其他", new GMOtherExtendTool());
    }
}