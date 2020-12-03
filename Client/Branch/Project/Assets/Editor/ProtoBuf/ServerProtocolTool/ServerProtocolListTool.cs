using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml.Serialization;

/// <summary>
/// 服务器协议
/// 控制应有哪些协议纳入控制范围内
/// </summary>
public class ServerProtocolListTool : EditorWindow, IEditorTool
{
    private int indexToBeRemoved = -1;
    private Vector2 scrollPos = Vector2.zero;
    private string keyword;
    private Color infoColor = new Color(0.45f, 1, 0.72f);

    private string linePattern_ImportProto = @"^import\s+""\w+.proto"";";

    public static ServerProtocolListTool Instance = null;


    public ProtocolControlCache_ServerProtocol Data = new ProtocolControlCache_ServerProtocol();

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

    public ServerProtocolListTool(ServerProtocolControllerWnd wnd)
    {
        Instance = this;
        indexToBeRemoved = -1;
        scrollPos = Vector2.zero;
        Load();
    }

    #region 本地数据的保存与读取
    private static string _CacheFileName = "protocol_list.xml";
    private static string _CacheFilePath = "";
    /// <summary>
    /// 缓存文件路径
    /// </summary>
    public static string CacheFilePath
    {
        get
        {
            if (string.IsNullOrEmpty(_CacheFilePath))
            {
                _CacheFilePath = URL.LocalProjectPath + "/ProjectCaches/" + _CacheFileName;
            }
            return _CacheFilePath;
        }
    }

    XmlSerializer CacheSerializer = new XmlSerializer(typeof(ProtocolControlCache_ServerProtocol));
    public void Save()
    {
        using (FileStream fs = new FileStream(CacheFilePath, FileMode.Create))
        {
            CacheSerializer.Serialize(fs, Data);
        }
    }

    public void Load()
    {
        if (File.Exists(CacheFilePath))
        {
            using (FileStream fs = new FileStream(CacheFilePath, FileMode.Open))
            {
                Data = CacheSerializer.Deserialize(fs) as ProtocolControlCache_ServerProtocol;
            }
        }
        else
        {
            Data = new ProtocolControlCache_ServerProtocol();
        }
    }
    #endregion


    public void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {
            //添加协议按钮
            if (GUILayout.Button("添加协议", GUILayout.Width(100)))
            {
                AddProtocol();
            }
        }
        EditorGUILayout.EndHorizontal();

        //说明文字
        EditorGUILayout.LabelField("批量添加协议所读的文件中,每行的格式为:\"协议中文名=>协议xml名\"");
        GUI.contentColor = infoColor;
        EditorGUILayout.LabelField("修改协议后,点击下面的按钮");
        GUI.contentColor = Color.white;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("刷新proto相关文件", GUILayout.Width(250)))
        {
            RefreshProtoRelated();
            EditorGUILayout.EndHorizontal();
            return;
        }
        EditorGUILayout.EndHorizontal();
        //筛选
        EditorGUILayout.BeginHorizontal();
        GUI.enabled = true;
        EditorGUILayout.LabelField("搜索: ", GUILayout.Width(50));
        keyword = EditorGUILayout.TextField(keyword, GUILayout.ExpandWidth(true));
        EditorGUILayout.EndHorizontal();

        ShowList();
    }

    private void ShowList()
    {
        #region 协议列表
        EditorGUILayout.Space();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < Data.protocols.Count; i++)
        {
            if (IsConformToKeyword(Data.protocols[i].protocolName) || IsConformToKeyword(Data.protocols[i].xmlName))
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("打开协议", GUILayout.Width(80)))
                {
                    OpenXMLFile(Data.protocols[i]);
                }
                if (GUILayout.Button("打开proto", GUILayout.Width(80)))
                {
                    OpenProtoFile(Data.protocols[i]);
                }
                if (GUILayout.Button("从服务器文件夹同步", GUILayout.Width(120)))
                {
                    SyncProtocol(Data.protocols[i]);
                }
                CSStringBuilder.Clear();
                CSStringBuilder.AppendParams((i + 1).ToString("d4"));
                CSStringBuilder.AppendParams(".  ");
                CSStringBuilder.AppendParams(Data.protocols[i].protocolName);
                EditorGUILayout.LabelField(CSStringBuilder.ToStringParams(), GUILayout.Width(100));
                EditorGUILayout.LabelField(Data.protocols[i].xmlName);
                Data.protocols[i].isProtocolUsedInLua = EditorGUILayout.ToggleLeft("Lua相关的协议", Data.protocols[i].isProtocolUsedInLua, GUILayout.Width(100));
                if (GUILayout.Button("移除  " + Data.protocols[i].xmlName, GUILayout.Width(120), GUILayout.ExpandWidth(true)))
                {
                    indexToBeRemoved = i;
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        if (indexToBeRemoved >= 0)
        {
            Data.protocols.RemoveAt(indexToBeRemoved);
            indexToBeRemoved = -1;
        }
        EditorGUILayout.EndVertical();
        GUI.enabled = true;
        EditorGUILayout.EndScrollView();
        #endregion
    }

    /// <summary>
    /// 添加协议
    /// </summary>
    private void AddProtocol()
    {
        ShowInputDialog("添加协议", new string[] { "中文名", "xml名" }, "是否在lua中使用", OnAddProtocolEnd);
    }

    private void OnAddProtocolEnd(bool res, string[] strs, bool isUsedInLua)
    {
        if (res)
        {
            if (strs != null && strs.Length == 2 && !string.IsNullOrEmpty(strs[0]) && !string.IsNullOrEmpty(strs[1]))
            {
                AddProtocolToData(strs[0], strs[1], isUsedInLua);
            }
            else
            {
                UnityEngine.Debug.LogError("添加协议失败");
            }
            Resort();
        }
        ServerProtocolControllerWnd.Instance.Repaint();
    }

    /// <summary>
    /// 显示输入对话框
    /// </summary>
    /// <param name="title">输入对话框标题</param>
    /// <param name="instructions">输入对话框说明</param>
    /// <param name="instructionCount">说明数量</param>
    /// <param name="callback">输入对话框回调</param>
    public void ShowInputDialog(string title, string[] instructions, string toggleInfo, Action<bool, string[], bool> callback)
    {
        ServerTool_ProtocolController_InputDialog inputdialog = GetWindow<ServerTool_ProtocolController_InputDialog>(title);
        int height = 20 * instructions.Length + 150;
        inputdialog.minSize = new Vector2(400, height);
        inputdialog.Initialize(instructions, toggleInfo, callback);
        inputdialog.Show();
    }

    /// <summary>
    /// 向数据中添加协议和其对应的xml
    /// </summary>
    /// <param name="protocolName"></param>
    /// <param name="xmlName"></param>
    private bool AddProtocolToData(string protocolName, string xmlName, bool isUsedInLua)
    {
        if (Data.GetProtocolByProtocolName(protocolName) != null)
        {
            UnityEngine.Debug.LogError(string.Format("协议名冲突: {0}", protocolName));
            return false;
        }
        else if (Data.GetProtocolByXMLName(xmlName) != null)
        {
            UnityEngine.Debug.LogError(string.Format("XML冲突: {0}", xmlName));
            return false;
        }

        string xmlFullPath = RealServerProtoXMLPath + "/" + xmlName + ".xml";
        if (File.Exists(xmlFullPath))
        {
            ProtocolControlCache_ProtocolXMLStructure xmlStructure = ServerProtocolControllerWnd.DeserializeXML(xmlFullPath);
            if (xmlStructure == null)
            {
                UnityEngine.Debug.LogError(string.Format("XML文件格式解析失败\r\n{0}", xmlFullPath));
                return false;
            }
        }
        else
        {
            UnityEngine.Debug.LogError(string.Format("XML文件不存在\r\n{0}", xmlFullPath));
            return false;
        }
        Data.protocols.Add(new ProtocolControlCache_Protocol(protocolName, xmlName, isUsedInLua));

        UnityEngine.Debug.Log(string.Format("{0} 协议添加成功 {1}", protocolName, xmlName));
        return true;
    }

    /// <summary>
    /// 打开协议对应的xml文件
    /// </summary>
    /// <param name="protocol">协议</param>
    private void OpenXMLFile(ProtocolControlCache_Protocol protocol)
    {
        if (protocol != null)
        {
            CSStringBuilder.Clear();
            CSStringBuilder.AppendParams(LocalServerProtoXMLPath);
            CSStringBuilder.AppendParams('/');
            CSStringBuilder.AppendParams(protocol.xmlName);
            CSStringBuilder.AppendParams(".xml");
            string fullPath = CSStringBuilder.ToStringParams();
            if (!File.Exists(fullPath))
            {
                UnityEngine.Debug.LogError(string.Format("{0} 文件不存在", fullPath));
                return;
            }
            Process.Start(fullPath);
        }
    }

    /// <summary>
    /// 打开协议同名的proto文件
    /// </summary>
    /// <param name="protocol"></param>
    private void OpenProtoFile(ProtocolControlCache_Protocol protocol)
    {
        if (protocol != null)
        {
            ProtocolControlCache_ProtocolXMLStructure xmlStructure = ServerProtocolControllerWnd.DeserializeXML(protocol.xmlName);
            if (xmlStructure != null)
            {
                CSStringBuilder.Clear();
                CSStringBuilder.AppendParams(LocalServerProtoPath);
                CSStringBuilder.AppendParams('/');
                CSStringBuilder.AppendParams(xmlStructure.protoName);
                CSStringBuilder.AppendParams(".proto");
            }
            else
            {
                CSStringBuilder.Clear();
                CSStringBuilder.AppendParams(LocalServerProtoPath);
                CSStringBuilder.AppendParams('/');
                CSStringBuilder.AppendParams(protocol.xmlName);
                CSStringBuilder.AppendParams(".proto");
            }
            string fullPath = CSStringBuilder.ToStringParams();
            if (!File.Exists(fullPath))
            {
                UnityEngine.Debug.LogError(string.Format("{0} 文件不存在", fullPath));
                return;
            }
            Process.Start(fullPath);
        }
    }

    /// <summary>
    /// 排序
    /// </summary>
    private void Resort()
    {
        Data.protocols.Sort((l, r) => { return l.xmlName.CompareTo(r.xmlName); });
    }
    
    /// <summary>
    /// 将proto和xml文件从服务器svn同步到本地svn
    /// </summary>
    private void SyncProtoAndXMLFromServerSVNToLocalSVN()
    {
        if (Data == null || Data.protocols == null)
        {
            return;
        }
        List<string> protocolFileNames = new List<string>();
        for (int i = 0; i < Data.protocols.Count; i++)
        {
            var protocolTemp = Data.protocols[i];
            if (!protocolFileNames.Contains(protocolTemp.xmlName))
            {
                protocolFileNames.Add(protocolTemp.xmlName);
            }
        }
        CopyAllFiles(protocolFileNames, RealServerProtoXMLPath, LocalServerProtoXMLPath, ".xml");
        List<string> protoFileNames = new List<string>();
        for (int i = 0; i < Data.protocols.Count; i++)
        {
            var protocolTemp = Data.protocols[i];
            string temp;
            var list = GetRelatedProtoFromXML(RealServerProtoXMLPath, RealServerProtoPath, protocolTemp.xmlName, out temp);
            for (int j = 0; j < list.Count; j++)
            {
                if (!protoFileNames.Contains(list[j]))
                {
                    protoFileNames.Add(list[j]);
                }
            }
        }
        CopyAllFiles(protoFileNames, RealServerProtoPath, LocalServerProtoPath, ".proto");
    }

    /// <summary>
    /// 同步协议
    /// </summary>
    /// <param name="protocol"></param>
    private void SyncProtocol(ProtocolControlCache_Protocol protocol)
    {
        //拷贝协议
        try
        {
            File.Copy(RealServerProtoXMLPath + "/" + protocol.xmlName + ".xml", LocalServerProtoXMLPath.TrimEnd('/') + "/" + protocol.xmlName + ".xml", true);
            UnityEngine.Debug.LogFormat("从 {0} 复制到 {1} 成功", RealServerProtoXMLPath + "/" + protocol.xmlName + ".xml", 
                LocalServerProtoXMLPath.TrimEnd('/') + "/" + protocol.xmlName + ".xml");
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogErrorFormat("从 {0} 复制到 {1} 失败\n{2}", RealServerProtoXMLPath + "/" + protocol.xmlName + ".xml",
                LocalServerProtoXMLPath.TrimEnd('/') + "/" + protocol.xmlName + ".xml", ex.Message);
        }
        //拷贝主proto
        string mainProto;
        GetRelatedProtoFromXML(RealServerProtoXMLPath, RealServerProtoPath, protocol.xmlName, out mainProto);
        try
        {
            File.Copy(RealServerProtoPath + "/" + mainProto + ".proto", LocalServerProtoPath.TrimEnd('/') + "/" + mainProto + ".proto", true);
            UnityEngine.Debug.LogFormat("从 {0} 复制到 {1} 成功", RealServerProtoXMLPath + "/" + mainProto + ".proto",
                LocalServerProtoXMLPath.TrimEnd('/') + "/" + mainProto + ".proto");
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogErrorFormat("从 {0} 复制到 {1} 失败\n{2}", RealServerProtoXMLPath + "/" + mainProto + ".proto", 
                LocalServerProtoXMLPath.TrimEnd('/') + "/" + mainProto + ".proto", ex.Message);
        }
        //拷贝关联的proto文件
        string temp;
        var list = GetRelatedProtoFromXML(RealServerProtoXMLPath, RealServerProtoPath, protocol.xmlName, out temp);
        CopyAllFiles(list, RealServerProtoPath, LocalServerProtoPath, ".proto");
    }

    #region tools
    /// <summary>
    /// 判断字符串是否符合关键词
    /// </summary>
    /// <param name="label"></param>
    /// <returns></returns>
    private bool IsConformToKeyword(string label)
    {
        if (string.IsNullOrEmpty(label))
        {
            return false;
        }
        if (string.IsNullOrEmpty(keyword))
        {
            return true;
        }
        label = label.ToLower();
        keyword = keyword.ToLower();
        int pointForKeyword = 0;
        for (int i = 0; i < label.Length; i++)
        {
            if (pointForKeyword < keyword.Length && label[i] == keyword[pointForKeyword])
            {
                ++pointForKeyword;
            }
        }
        return pointForKeyword == keyword.Length;
    }

    /// <summary>
    /// 刷新相关proto
    /// </summary>
    public void RefreshProtoRelated()
    {
        if (EditorApplication.isCompiling)
        {
            UnityEngine.Debug.LogError("请等待代码编译完毕");
            return;
        }
        EditorUtility.DisplayProgressBar("刷新相关的proto文件", string.Empty, 0);
        string protoFolderPath = Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "/luaRes/protobufLua/proto";
        string decodeFolderPath = Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "/luaRes/protobufLua/decode";
        string adjustFolderPath = Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "/luaRes/protobufLua/adjust";
        //将proto和proto生成的lua文件复制到对应文件夹
        EditorUtility.DisplayProgressBar("刷新相关的proto文件", "正在清空文件夹", 0.5f);
        CreateEmptyFolder(protoFolderPath);
        CreateEmptyFolder(decodeFolderPath);
        CreateEmptyFolder(adjustFolderPath);
        //proto名列表,保留了关联顺序
        List<string> protoNameList = new List<string>();
        for (int i = 0; i < Data.protocols.Count; i++)
        {
            var temp = Data.protocols[i];
            EditorUtility.DisplayProgressBar("刷新相关的proto文件", "正在解析协议文件 " + temp.xmlName + ".xml", ((float)i / Data.protocols.Count));
            string mainProto;
            List<string> relatedProtos = GetRelatedProtoFromXML(LocalServerProtoXMLPath, LocalServerProtoPath, temp.xmlName, out mainProto);
            if (!string.IsNullOrEmpty(mainProto) && relatedProtos != null)
            {
                for (int j = 0; j < relatedProtos.Count; j++)
                {
                    if (!protoNameList.Contains(relatedProtos[j]))
                    {
                        protoNameList.Add(relatedProtos[j]);
                    }
                }
            }
        }
        EditorUtility.DisplayProgressBar("刷新相关的proto文件", "正在复制proto文件至文件夹中", 0.3f);
        //复制相关的proto文件到proto文件夹
        CopyAllFiles(protoNameList, LocalServerProtoPath, protoFolderPath, ".proto");
        //从proto文件生成lua文件
        EditorUtility.DisplayProgressBar("刷新相关的proto文件", "正在生成Decode文件(同时检查C#数据结构完整性)", 0f);
        List<string> luaGeneratedList = GenerateLuaFilesFromProto(protoNameList, protoFolderPath, decodeFolderPath, adjustFolderPath);
        //注册所有相关的proto文件到lua中
        string registerAllRelatedProtoLuaScriptPath = Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "/luaRes/protobufLua/registerAllRelatedProto.lua";
        try
        {
            if (File.Exists(registerAllRelatedProtoLuaScriptPath))
            {
                File.Delete(registerAllRelatedProtoLuaScriptPath);
            }
            CSStringBuilder.Clear();
            CSStringBuilder.AppendParams("--[[本文件为工具自动生成,禁止手动修改]]\r\n");
            CSStringBuilder.AppendParams("---加载所有相关proto\r\nlocal LoadAllRelatedProto = {}\r\n\r\n---@param protobufMgr protobuf管理器\r\nfunction LoadAllRelatedProto:LoadAll(protobufMgr)\r\n");
            CSStringBuilder.AppendParams("    ---luaTable->C#数据结构转换\r\n    protobufMgr.DecodeTable = {}\r\n    ---调整luaTable\r\n    protobufMgr.AdjustTable = {}\r\n\r\n");
            for (int i = 0; i < protoNameList.Count; i++)
            {
                EditorUtility.DisplayProgressBar("刷新相关的proto文件", "正在绑定Decode文件中", (i / (float)protoNameList.Count));
                CSStringBuilder.AppendParams("    protobufMgr.RegisterPb(\"");
                CSStringBuilder.AppendParams(protoNameList[i]);
                CSStringBuilder.AppendParams(".proto\")\r\n");
                CSStringBuilder.AppendParams("    ");
                if (!luaGeneratedList.Contains(protoNameList[i]))
                {
                    //如果该proto未生成对应的lua解析文件,则不去引用该文件
                    CSStringBuilder.AppendParams("---未生成 ");
                    CSStringBuilder.AppendParams(protoNameList[i]);
                    CSStringBuilder.AppendParams(".proto 的lua=>C#文件\r\n\r\n");
                    UnityEngine.Debug.LogFormat("{0} 文件中未找到任一C#中已生成的类,故忽略该lua=>C#转换文件", protoNameList[i]);
                }
                else
                {
                    CSStringBuilder.AppendParams("protobufMgr.DecodeTable.");
                    CSStringBuilder.AppendParams(protoNameList[i]);
                    CSStringBuilder.AppendParams(" = require(\"luaRes.protobufLua.decode.");
                    CSStringBuilder.AppendParams(protoNameList[i]);
                    CSStringBuilder.AppendParams("\")\r\n    protobufMgr.AdjustTable.");
                    CSStringBuilder.AppendParams(protoNameList[i]);
                    CSStringBuilder.AppendParams("_adj = require(\"luaRes.protobufLua.adjust.");
                    CSStringBuilder.AppendParams(protoNameList[i]);
                    CSStringBuilder.AppendParams("_adj\")\r\n\r\n");
                }
            }
            CSStringBuilder.AppendParams("end\r\n\r\n");
            CSStringBuilder.AppendParams("return LoadAllRelatedProto");
            File.WriteAllText(registerAllRelatedProtoLuaScriptPath, CSStringBuilder.ToStringParams(), Encoding.UTF8);
            UnityEngine.Debug.Log("消息注册完毕");
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log(string.Format("lua注册相关proto失败\r\n{0}\r\n{1}", ex.Message, ex.StackTrace));
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    /// <summary>
    /// 创建新文件夹,或清空某已存在的文件夹
    /// </summary>
    /// <param name="folderPath">文件夹路径</param>
    private void CreateEmptyFolder(string folderPath)
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
    /// 从xml协议中获取相关的proto文件名
    /// </summary>
    /// <param name="xmlDirPath">xml文件夹路径</param>
    /// <param name="xmlName">xml名</param>
    /// <param name="mainProto">该xml对应的proto文件名</param>
    /// <returns></returns>
    private List<string> GetRelatedProtoFromXML(string xmlDirPath, string protoDirPath, string xmlName, out string mainProto)
    {
        List<string> protoNameList = new List<string>();
        CSStringBuilder.Clear();
        CSStringBuilder.AppendParams(xmlDirPath);
        CSStringBuilder.AppendParams("/");
        CSStringBuilder.AppendParams(xmlName);
        CSStringBuilder.AppendParams(".xml");
        ProtocolControlCache_ProtocolXMLStructure xmlStructure = ServerProtocolControllerWnd.DeserializeXML(CSStringBuilder.ToStringParams());
        if (xmlStructure != null)
        {
            mainProto = xmlStructure.protoName;
            GetRelatedProtoFromProto(mainProto, protoDirPath, protoNameList);
        }
        else
        {
            mainProto = string.Empty;
        }
        return protoNameList;
    }

    /// <summary>
    /// 从proto中获取关联的proto文件
    /// </summary>
    /// <param name="protoName"></param>
    /// <param name="protoNameList"></param>
    private void GetRelatedProtoFromProto(string protoName, string protoDirPath, List<string> protoNameList)
    {
        CSStringBuilder.Clear();
        //CSStringBuilder.AppendParams(ServerTool_ProtocolControllerWnd.LocalServerProtoPath);
        CSStringBuilder.AppendParams(protoDirPath);
        CSStringBuilder.AppendParams("/");
        CSStringBuilder.AppendParams(protoName);
        CSStringBuilder.AppendParams(".proto");
        string filePath = CSStringBuilder.ToStringParams();
        try
        {
            if (File.Exists(filePath))
            {
                string[] lineStrs = File.ReadAllLines(filePath);
                for (int i = 0; i < lineStrs.Length; i++)
                {
                    if (Regex.IsMatch(lineStrs[i], linePattern_ImportProto))
                    {
                        string newProtoName = lineStrs[i].Replace("import", string.Empty).Replace('\"', ' ').Replace(';', ' ').Replace(".proto", string.Empty).Trim();
                        if (!protoNameList.Contains(newProtoName))
                        {
                            GetRelatedProtoFromProto(newProtoName, protoDirPath, protoNameList);
                        }
                    }
                }
                if (!protoNameList.Contains(protoName))
                {
                    protoNameList.Add(protoName);
                }
            }
        }
        catch (Exception) { }
    }

    /// <summary>
    /// 从源目录复制所有文件到目标目录
    /// </summary>
    /// <param name="fileNames">需要复制的文件列表</param>
    /// <param name="originFolder">源目录</param>
    /// <param name="targetFolder">目标目录</param>
    /// <param name="extension">后缀名</param>
    private void CopyAllFiles(List<string> fileNames, string originFolder, string targetFolder, string extension = "")
    {
        if (!Directory.Exists(originFolder) || !Directory.Exists(targetFolder) || fileNames == null)
        {
            return;
        }
        for (int i = 0; i < fileNames.Count; i++)
        {
            string originPath = originFolder.TrimEnd('/') + "/" + fileNames[i] + extension;
            string targetPath = targetFolder.TrimEnd('/') + "/" + fileNames[i] + extension;
            try
            {
                File.Copy(originPath, targetPath, true);
                UnityEngine.Debug.LogFormat("从 {0} 复制到 {1} 成功", originPath, targetPath);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogErrorFormat("从 {0} 复制到 {1} 失败\n{2}", originPath, targetPath, ex.Message);
            }
        }
    }

    /// <summary>
    /// 在proto文件夹下批量将proto文件转为lua文件
    /// </summary>
    /// <param name="fileNames">需要生成的文件列表</param>
    /// <param name="protoFolder">proto文件夹</param>
    /// <param name="decodeLuaFolder">转换脚本的lua文件夹</param>
    /// <param name="adjustFolderPath">调整脚本的lua文件夹</param>
    /// <returns>生成了lua文件的proto文件名</returns>
    private List<string> GenerateLuaFilesFromProto(List<string> fileNames, string protoFolder, string decodeLuaFolder, string adjustFolderPath)
    {
        List<string> list = new List<string>();
        if (fileNames != null && !string.IsNullOrEmpty(protoFolder) && Directory.Exists(protoFolder) && !string.IsNullOrEmpty(decodeLuaFolder) && Directory.Exists(decodeLuaFolder))
        {
            ServerTool_ProtocolController_GenerateLuaFromProto generateLuaFromProto = new ServerTool_ProtocolController_GenerateLuaFromProto();
            for (int i = 0; i < fileNames.Count; i++)
            {
                if (generateLuaFromProto.DealProto(protoFolder + "/" + fileNames[i] + ".proto", decodeLuaFolder + "/" + fileNames[i] + ".lua", adjustFolderPath + "/" + fileNames[i] + "_adj.lua", true))
                {
                    list.Add(fileNames[i]);
                }
            }
        }
        return list;
    }

    #endregion
}