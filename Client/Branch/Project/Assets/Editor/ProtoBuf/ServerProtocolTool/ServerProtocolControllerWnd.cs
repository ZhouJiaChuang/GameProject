using ExtendEditor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 服务器协议控制工具
/// 包括但不限于:
/// 1. 服务器协议同步;
/// 2. 服务器协议=>Lua代码;
/// 3. 服务器proto=>Lua代码;
/// 4. 服务器消息流向控制;
/// </summary>
public class ServerProtocolControllerWnd : CSEditorWindow
{

    private static ServerProtocolControllerWnd _ProtocolController;
    /// <summary>
    /// 协议控制窗口实例
    /// </summary>
    public static ServerProtocolControllerWnd Instance { get { return _ProtocolController; } }

    [MenuItem("Tools/服务器消息协议控制")]
    public static void OpenDataTool()
    {
        _ProtocolController = EditorWindow.GetWindow<ServerProtocolControllerWnd>("服务器消息协议控制");
        _ProtocolController.Show();
    }

    protected override void InitToolDic()
    {
        Initialize();
        _ProtocolController = this;

        AddToolBar("服务器协议", new ServerProtocolListTool(this));
        AddToolBar("Lua相关服务器消息", new ServerProtocolLuaMessage(this));
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptCompileFinished()
    {
        ProtocolControlCache_ProtoStructure.ClearProtoStructureCache();
    }

    /// <summary>
    /// 初始化,后续代码在此添加
    /// </summary>
    public void Initialize()
    {
        ProtocolControlCache_ProtoStructure.ClearProtoStructureCache();
    }
    
    #region 路径
    /// <summary>
    /// 服务器xml路径
    /// </summary>
    public static string RealServerProtoXMLPath
    {
        get
        {
            return CSEditorPath.GetPath(EEditorPath.RealServerProtoXMLPath);
        }
    }

    /// <summary>
    /// 服务器proto路径
    /// </summary>
    public static string RealServerProtoPath
    {
        get
        {
            return CSEditorPath.GetPath(EEditorPath.RealServerProtoPath);
        }
    }

    /// <summary>
    /// 本地服务器proto路径
    /// </summary>
    public static string LocalServerProtoPath
    {
        get
        {
            return CSEditorPath.GetPath(EEditorPath.LocalServerProtoPath);
        }
    }

    /// <summary>
    /// 本地服务器xml路径
    /// </summary>
    public static string LocalServerProtoXMLPath
    {
        get
        {
            return CSEditorPath.GetPath(EEditorPath.LocalServerProtoXMLPath);
        }
    }

    private static string[] serverProtoNames;
    /// <summary>
    /// 服务器proto路径下proto文件名集合
    /// </summary>
    public static string[] ServerProtoNames
    {
        get
        {
            if (serverProtoNames == null)
            {
                serverProtoNames = Directory.GetFiles(LocalServerProtoPath, "*.proto");
                for (int i = 0; i < serverProtoNames.Length; i++)
                {
                    string path = serverProtoNames[i];
                    string fileName = Path.GetFileNameWithoutExtension(path);
                    serverProtoNames[i] = fileName;
                }
            }
            return serverProtoNames;
        }
    }

    #endregion
    

    #region GUI
    protected override void OnGUI()
    {
        OnGUICommon();
        base.OnGUI();
    }

    private void OnGUICommon()
    {
        #region 路径显示
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(string.Format("服务器XML文件夹路径"), GUILayout.Width(200));
        if (GUILayout.Button("打开文件夹", GUILayout.Width(100)))
        {
            FileUtilityEditor.Open(RealServerProtoXMLPath);
        }
        EditorGUILayout.LabelField(RealServerProtoXMLPath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(string.Format("服务器proto文件夹路径"), GUILayout.Width(200));
        if (GUILayout.Button("打开文件夹", GUILayout.Width(100)))
        {
            FileUtilityEditor.Open(RealServerProtoPath);
        }
        EditorGUILayout.LabelField(RealServerProtoPath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(string.Format("本地proto文件夹路径"), GUILayout.Width(200));
        if (GUILayout.Button("打开文件夹", GUILayout.Width(100)))
        {
            FileUtilityEditor.Open(LocalServerProtoPath);
        }
        EditorGUILayout.LabelField(LocalServerProtoPath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(string.Format("本地xml文件夹路径"), GUILayout.Width(200));
        if (GUILayout.Button("打开文件夹", GUILayout.Width(100)))
        {
            FileUtilityEditor.Open(LocalServerProtoXMLPath);
        }
        EditorGUILayout.LabelField(LocalServerProtoXMLPath);
        EditorGUILayout.EndHorizontal();
        #endregion

        #region 按钮
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("数据操作:", GUILayout.Width(60));
        if (GUILayout.Button("保存数据", GUILayout.Width(100)))
        {
            Save();
        }
        if (GUILayout.Button("一键提交", GUILayout.Width(100)))
        {
        }
        EditorGUILayout.EndHorizontal();
        #endregion
    }

    #endregion

    #region Tools
    private static XmlSerializer xmlSerializer;
    /// <summary>
    /// 协议xml序列化器
    /// </summary>
    protected static XmlSerializer XMLSerializer
    {
        get
        {
            if (xmlSerializer == null)
            {
                xmlSerializer = new XmlSerializer(typeof(ProtocolControlCache_ProtocolXMLStructure));
            }
            return xmlSerializer;
        }
    }

    /// <summary>
    /// 从协议的xml文件中解析出xml的结构
    /// </summary>
    /// <param name="xmlPath">xml文件路径</param>
    /// <returns>xml文件数据</returns>
    public static ProtocolControlCache_ProtocolXMLStructure DeserializeXML(string xmlPath)
    {
        FileStream fs = null;
        try
        {
            if (File.Exists(xmlPath))
            {
                using (fs = new FileStream(xmlPath, FileMode.Open))
                {
                    ProtocolControlCache_ProtocolXMLStructure xml = XMLSerializer.Deserialize(fs) as ProtocolControlCache_ProtocolXMLStructure;
                    fs.Close();
                    fs = null;
                    if (xml != null)
                    {
                        xml.DealData();
                    }
                    return xml;
                }
            }
        }
        catch (Exception)
        {
            if (fs != null)
            {
                fs.Close();
            }
        }
        return null;
    }

    /// <summary>
    /// 从协议的消息结构得到其使用的proto的数据结构,若返回null则表明该消息未使用proto
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    public static ProtocolControlCache_ProtoStructure GetProtoStructure(ProtocolControlCache_Message msg)
    {
        if (msg != null)
        {
            if (msg.hasProto)
            {
                string path = string.Format("{0}/{1}.proto", LocalServerProtoPath, msg.protoData.protoName);
                ProtocolControlCache_ProtoStructure protoStructure = ProtocolControlCache_ProtoStructure.GetProtoStructure(path);
                return protoStructure;
            }
        }
        return null;
    }

    /// <summary>
    /// 获取协议中消息对应的proto中的消息结构
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ProtocolControlCache_ProtoStructure.Message GetMessageStructure(ProtocolControlCache_Message message)
    {
        if (!message.hasProto)
        {
            return null;
        }
        var protoStructure = GetProtoStructure(message);
        if (protoStructure == null)
        {
            return null;
        }
        for (int i = 0; i < protoStructure.Messages.Count; i++)
        {
            var messageStructure = protoStructure.Messages[i];
            if (messageStructure.MessageName == message.protoData.protoMsgName)
            {
                return messageStructure;
            }
        }
        return null;
    }

    /// <summary>
    /// 创建新文件夹,或清空某已存在的文件夹
    /// </summary>
    /// <param name="folderPath">文件夹路径</param>
    public static void CreateOrClearFolder(string folderPath)
    {
        if (string.IsNullOrEmpty(folderPath) == false)
        {
            if (Directory.Exists(folderPath) == false)
            {
                Directory.CreateDirectory(folderPath);
            }
            else
            {
                string[] files = Directory.GetFiles(folderPath, "*", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < files.Length; i++)
                {
                    File.Delete(files[i]);
                }
            }
        }
    }

    /// <summary>
    /// 规范化proto名
    /// </summary>
    /// <param name="protoName">proto文件名</param>
    /// <returns></returns>
    public static string FormatProtoName(string protoName)
    {
        return GetSuitableProtoNameForProtoName(protoName);
    }

    /// <summary>
    /// 清理服务器proto名缓存
    /// </summary>
    public static void ClearServerProtoNameCache()
    {
        serverProtoNames = null;
    }

    /// <summary>
    /// 获取当前磁盘中与输入的proto文件名相匹配的proto文件名
    /// </summary>
    /// <param name="inputProtoName"></param>
    /// <param name="reread"></param>
    /// <returns></returns>
    private static string GetSuitableProtoNameForProtoName(string inputProtoName, bool reread = false)
    {
        if (reread)
        {
            ClearServerProtoNameCache();
        }
        if (ServerProtoNames != null)
        {
            for (int i = 0; i < ServerProtoNames.Length; i++)
            {
                if (string.Equals(inputProtoName, ServerProtoNames[i], StringComparison.OrdinalIgnoreCase))
                {
                    return ServerProtoNames[i];
                }
            }
        }
        if (reread)
        {
            return inputProtoName;
        }
        else
        {
            return GetSuitableProtoNameForProtoName(inputProtoName, true);
        }
    }
    #endregion
}
