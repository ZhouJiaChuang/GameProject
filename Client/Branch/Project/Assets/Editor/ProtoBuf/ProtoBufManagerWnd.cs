using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProtoBufManagerWnd : CSEditorWindow
{
    [MenuItem("Tools/Protobuf管理面板")]
    public static void OpenDataTool()
    {
        ProtoBufManagerWnd wnd = EditorWindow.GetWindow<ProtoBufManagerWnd>("Protobuf管理面板");
        wnd.Show();
    }

    protected override void InitToolDic()
    {
        AddToolBar("Protobuf文件解析", new ProtobufParseTool());
        AddToolBar("服务器协议解析", new ServerXMLParseTool());
    }

}
