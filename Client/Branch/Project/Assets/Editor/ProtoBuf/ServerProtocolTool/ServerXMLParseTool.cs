using ExtendEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class ServerXMLParseTool : IEditorTool
{
    public string[] ToolbarTitles = { "C#相关"};

    private int ToolbarIndex = 0;

    private CSProtobufXMLParseFolder ProtobufXMLParseFolder = new CSProtobufXMLParseFolder(CSEditorPath.Get(EEditorPath.LocalServerProtoXMLPath),
        CSEditorPath.Get(EEditorPath.LocalServerProtoFunctionPath));

    public string SerachTxt = string.Empty;

    public void OnGUI()
    {
        GUILayout.BeginHorizontal();
        {
            ToolbarIndex = GUILayout.Toolbar(ToolbarIndex, ToolbarTitles);
        }
        GUILayout.EndHorizontal();

        if (ToolbarIndex == 0)
            OnGUI_ServerProtoXML_C();
    }

    private void OnGUI_ServerProtoXML_C()
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("搜索:", GUILayout.Width(40));
            SerachTxt = EditorGUILayout.TextField(SerachTxt, GUILayout.Width(300));

            if (GUILayout.Button("刷新", GUILayout.Width(80)))
            {
                ProtobufXMLParseFolder.RefushFolder();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("生成", GUILayout.Width(80)))
            {
            }

            GUILayout.Space(10);

            if (GUILayout.Button("打开", GUILayout.Width(80)))
            {
                FileUtilityEditor.Open(ProtobufXMLParseFolder.OrigionFolder);
            }

        }
        GUILayout.EndHorizontal();

        ProtobufXMLParseFolder.ShowFolderFileList(SerachTxt);
    }

    public void Save()
    {
    }

    public void Load()
    {
    }
}

public class CSProtobufXMLParseFolder
{
    /// <summary>
    /// 原始目录
    /// </summary>
    public string OrigionFolder = string.Empty;
    /// <summary>
    /// 输出目录
    /// </summary>
    public string OutFolder = string.Empty;

    CSBetterList<FileInfo> FileList = new CSBetterList<FileInfo>();

    public CSProtobufXMLParseFolder(string OrigionFolder, string OutFolder)
    {
        this.OrigionFolder = OrigionFolder;
        this.OutFolder = OutFolder;
        RefushFolder();
    }

    public void RefushFolder()
    {
        DirectoryInfo directory = new DirectoryInfo(OrigionFolder);
        if (!directory.Exists) return;
        FileList.Clear();

        FileInfo[] files = directory.GetFiles();
        for (int i = 0; i < files.Length; i++)
        {
            FileList.Add(files[i]);
        }
    }

    Vector2 pos = Vector2.zero;

    public void ShowFolderFileList(string serachTxt)
    {
        if (FileList == null) return;
        pos = EditorGUILayout.BeginScrollView(pos);
        {
            for (int i = 0; i < FileList.Count; i++)
            {
                if (string.IsNullOrEmpty(serachTxt) || FileList[i].Name.Contains(serachTxt))
                    ShowItemProtoFile(FileList[i]);
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void ShowItemProtoFile(FileInfo file)
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("文件:", GUILayout.Width(40));
            GUILayout.Label(file.Name, GUILayout.Width(300));

            if (GUILayout.Button("生成"))
            {
                ParseSelectedServerXmlFile(file.FullName);
            }
        }
        GUILayout.EndHorizontal();
    }

    public void ParseSelectedServerXmlFile(string fileName)
    {
        ParseXMLToRsp(fileName, OutFolder, false);
        AddToTemplateTop();
    }

    string outXMLDefineContent = string.Empty;

    private string pattern = "";
    private MatchCollection matchs = null;

    public void ParseXMLToRsp(string fileUrl, string outPath, bool isOpenFile, bool isSingleFile = false)
    {
        CSProtoFileData.ParseProtoFile(CSEditorPath.Get(EEditorPath.LocalServerProtoPath) + "activity.proto");

        return;
        outXMLDefineContent = string.Empty;

        FileUtilityEditor.DetectCreateDirectory(outPath);

        FileInfo file = new FileInfo(fileUrl);

        XmlNode doc = XmlEditorUtil.LoadXml(fileUrl);
        XmlNode messageNode = XmlEditorUtil.ReadXml(doc, "messages");
        if (messageNode == null) messageNode = XmlEditorUtil.ReadXml(doc, "message");
        if (messageNode == null)
        {
            UnityEngine.Debug.LogError("无法解析xml,没有messages 节点 = " + fileUrl);
            return;
        }

        XmlElement mElement = messageNode as XmlElement;

        int groupID = System.Convert.ToInt32(mElement.GetAttribute("id").ToString());

        //该XML文件生成的枚举文本
        string msgDefine = string.Empty;

        XmlNodeList nList = messageNode.ChildNodes;
        foreach (var node in nList)
        {
            XmlElement e = node as XmlElement;
            if (e == null)
            {
                UnityEngine.Debug.Log("XmlElement is null = " + fileUrl + " " + node.ToString());
                continue;
            }
            if (!e.HasAttribute("id")) continue;

            int id = System.Convert.ToInt32(e.GetAttribute("id").ToString());
            int msgId = groupID * 1000 + id;
            string enumName = e.GetAttribute("class").ToString();
            string desc = e.GetAttribute("desc").ToString();
            bool toClient = e.GetAttribute("type").ToString() == "toClient";

            //增加一行枚举定义
            AddInfo_MsgDefine(enumName, msgId, desc, ref msgDefine);

            XmlElement protoNode = XmlEditorUtil.ReadXml(e, "proto") as XmlElement;
            //if (protoNode != null)//没有任何内容需要解析,这个消息只是一个空的消息
            //{
            //    if (!toClient)//如果不是从服务器返回的消息(即客户端发送的服务器的消息)
            //    {
            //        AddInfo_ReqString(enumName,)
            //    }
            //}
            //else
            //{

            //    string protoClass = protoNode.GetAttribute("name").ToString();

            //    string patten = "com.sh.game.proto.(\\S+)\\.";
            //    MatchCollection mc = Regex.Matches(protoClass, patten);
            //    if (mc.Count > 0)
            //    {
            //        protoClass = protoClass.Substring(protoClass.LastIndexOf(".") + 1);
            //        string java_outer_classname = mc[0].Groups[1].ToString();
            //        Dictionary<string, CSProtoMessageData> protoDic = GetProtoDic(java_outer_classname);
            //        if (protoDic != null)
            //        {
            //            if (protoDic.ContainsKey(protoClass))
            //            {
            //                CSProtoMessageData data = protoDic[protoClass];
            //                AddInfo(toClient, enumName, desc, msgId, data, ref rspString, ref rspFuncString, ref reqString, ref msgDefine, ref netMsgString);
            //            }
            //            else
            //            {
            //                //UnityEngine.Debug.LogError(protoClass + " is not in protoDic");
            //                AddInfo(toClient, enumName, desc, msgId, null, ref rspString, ref rspFuncString, ref reqString, ref msgDefine, ref netMsgString);
            //            }
            //        }
            //    }
            //}
        }

        ParseToESocketEventEnum(OutFolder, file.Name, groupID, msgDefine, ref outXMLDefineContent);

        AssetDatabase.Refresh();
    }

    #region 服务器消息枚举生成
    /// <summary>
    /// 增加一行枚举定义
    /// </summary>
    /// <param name="enumName"></param>
    /// <param name="msgid"></param>
    /// <param name="desc"></param>
    /// <param name="msgDefine"></param>
    private void AddInfo_MsgDefine(string enumName, int msgid, string desc, ref string msgDefine)
    {
        msgDefine += "    " + enumName + "=" + msgid + ",//" + desc + "\r\n";
    }

    /// <summary>
    /// 将内容枚举内容写入生成文件中
    /// </summary>
    /// <param name="outFolder">生成文件存放路径</param>
    /// <param name="name">写入内容的xml文件名字</param>
    /// <param name="id">写入内容的xml所以功能ID</param>
    /// <param name="content">写入的枚举内容</param>
    /// <param name="fileContent">文本内容</param>
    /// <param name="IsWrite">是否直接写入文件,当批量增加xml的时候,反复的写入会造成大量的开销</param>
    private void ParseToESocketEventEnum(string outFolder, string name, int id, string content, ref string fileContent, bool IsWrite = true)
    {
        string outFile = outFolder + "ESocketEvent.cs";

        if (string.IsNullOrEmpty(fileContent))
            fileContent = FileUtilityEditor.ReadToEnd(outFile);

        if (string.IsNullOrEmpty(fileContent))
        {
            string defaultContext = "/// <summary>\r\n/// 网络事件\r\n/// </summary>\r\npublic enum ESocketEvent\r\n{\r\n}";

            FileUtilityEditor.Write(outFile, defaultContext, false);
            fileContent = defaultContext;
        }

        string startFlag = GetServerProtoEnumStart(name, id);
        string endFlag = GetServerProtoEnumEnd(name, id);

        pattern = startFlag + "([\\s\\S]*)" + endFlag;

        content = startFlag + content + endFlag;
        matchs = Regex.Matches(fileContent, pattern);
        if (matchs != null && matchs.Count > 0)
        {
            string enumContent = matchs[0].Groups[0].Value;
            fileContent = fileContent.Replace(enumContent, content);
        }
        else//沒有找到之前的生成内容,说明这个xml是新加入的,那么插入到最下面
        {
            pattern = "public enum ESocketEvent\r\n{([^}]+)}";
            matchs = Regex.Matches(fileContent, pattern);
            //拿到之前的ESocketEvent枚举内所有内容
            if (matchs != null && matchs.Count > 0)
            {
                string enumContent = matchs[0].Groups[0].Value;
                content = enumContent.Insert(enumContent.Length - 1, "\r\n" + content);
                fileContent = fileContent.Replace(enumContent, content);
            }
        }

        if (IsWrite)
            FileUtilityEditor.Write(outFile, fileContent, false);
    }
    /// <summary>
    /// 服务器消息枚举的开头Flag
    /// </summary>
    /// <param name="name"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    private string GetServerProtoEnumStart(string name, int id)
    {
        string txt = "    #region " + name + "  ID:" + id + "   消息定义\r\n";
        return txt;
    }
    /// <summary>
    /// 服务器消息枚举的结尾Flag
    /// </summary>
    /// <param name="name"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    private string GetServerProtoEnumEnd(string name, int id)
    {
        string txt = "    #endregion " + name + "  ID:" + id + "   消息定义结束\r\n";
        return txt;
    }
    #endregion

    #region 获取所有proto的枚举方法,用来在后续的生成rep以及res方法中使用
    public void UpdateAllProto(string protoFolder)
    {
        DirectoryInfo directory = new DirectoryInfo(protoFolder);
        if (!directory.Exists) return;
        
    }
    #endregion


    #region 服务器消息请求类生成(这个就没有什么好判定插入的了,直接覆盖掉就是了)
    //顺便生成注释,方便调用
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="state"></param>
    /// <param name="roleSettingValue"></param>
    void AddInfo_ReqString(string enumName, CSProtoMessageData data, string desc, ref string reqString)
    {
        //string emptySpace = "    ";
        //string zhushi = "";
        //if (data != null)
        //{
        //    if (data.fieldList.Count > 0)
        //    {
        //        for (int i = 0; i < data.fieldList.Count; i++)
        //        {
        //            CSProtoMessageFieldData fieldData = data.fieldList[i];
        //            zhushi += emptySpace + "/// <param name=\"" + fieldData.fieldName + "\">" + fieldData.desc.Replace("\r", "").Replace("\n", "") + "<param>\r\n";
        //        }
        //    }
        //}
        //zhushi = emptySpace + "/// <summary>\r\n" +
        //    emptySpace + "///" + desc + "\r\n" +
        //    emptySpace + "/// </summary>\r\n" +
        //    zhushi;


        //string funcContent = zhushi + emptySpace + "public static void " + enumName + "(";
        //if (data != null)
        //{
        //    if (data.fieldList.Count > 0)
        //    {
        //        for (int i = 0; i < data.fieldList.Count; i++)
        //        {
        //            CSProtoMessageFieldData fieldData = data.fieldList[i];
        //            if (fieldData.flag == "repeated")
        //            {
        //                funcContent += "List<" + fieldData.type + ">" + " " + fieldData.fieldName + ((i == data.fieldList.Count - 1) ? ")" : ",");
        //            }
        //            else
        //            {
        //                funcContent += fieldData.type + " " + fieldData.fieldName + ((i == data.fieldList.Count - 1) ? ")" : ",");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        funcContent += ")";
        //    }

        //    funcContent += "\r\n" + emptySpace + "{\r\n" + emptySpace + emptySpace;
        //    funcContent += data.namespaceStr + "." + data.messageName + " req = new " + data.namespaceStr + "." + data.messageName + "();\r\n";
        //    for (int i = 0; i < data.fieldList.Count; i++)
        //    {
        //        ProtoTool.ProtoMessageFieldData fieldData = data.fieldList[i];
        //        if (fieldData.flag == "repeated")
        //        {
        //            funcContent += emptySpace + emptySpace + "req." + fieldData.fieldName + ".AddRange(" + fieldData.fieldName + ");" + "\r\n";
        //        }
        //        else
        //        {
        //            funcContent += emptySpace + emptySpace + "req." + fieldData.fieldName + " = " + fieldData.fieldName + ";" + "\r\n";
        //        }
        //    }
        //    funcContent += emptySpace + emptySpace + "CSNetwork.Instance.SendMsg((int)" +
        //        CSEditorPath.Get(EEditorPath.ProjectOverrideName) + "NetDef." + enumName + ", req);" + "\r\n" + emptySpace + "}\r\n";
        //}
        //else
        //{
        //    funcContent += ")";
        //    funcContent += "\r\n" + emptySpace + "{\r\n";
        //    funcContent += emptySpace + emptySpace + "CSNetwork.Instance.SendMsg((int)" +
        //        CSEditorPath.Get(EEditorPath.ProjectOverrideName) + "NetDef." + enumName + ", null);" + "\r\n" + emptySpace + "}\r\n";
        //}


        //reqString += funcContent + "\r\n";
    }

    private string GetServerReqStartFlag(string name, int id)
    {
        string txt = "using System.Collections.Generic;" +
            "\r\n\r\n" +
            "#region " + name + "  ID:" + id + "   请求方法\r\n" +
            "public partial class Net" +
            "{";
        return txt;
    }

    private string GetServerReqEndFlag(string name, int id)
    {
        string txt = "}\r\n" +
            "#endregion " + name + "  ID:" + id + "   请求方法结束\r\n";
        return txt;
    }

    #endregion

    #region 服务器枚举对应返回类生成

    #endregion


    Dictionary<string, CSProtoMessageData> GetProtoDic(string java_outer_classname)
    {
        //if (allProtoDic.ContainsKey(java_outer_classname)) return allProtoDic[java_outer_classname];
        return null;
    }

    void AddInfo(bool toClient, string enumName, string desc, int msgBeginId, CSProtoMessageData data,
        ref string rspString, ref string rspFuncString, ref string reqString, ref string msgDefine, ref string netMsgString)
    {
        //if (toClient)
        //{
        //    AddInfo_ToNetMsg(enumName, data, desc, ref netMsgString);
        //    AddInfo_RspString(enumName, desc, ref rspString);
        //    AddInfo_RspFuncString(enumName, desc, data, ref rspFuncString);
        //}
        //else
        //{
        //}
    }

    string InsertContent(string path, string beginFlag, string endFlag, string insertStr, ref int allEndIndex)
    {
        if (!File.Exists(path))
        {
            allEndIndex = -1;
            return insertStr;
        }
        string content = FileUtilityEditor.ReadToEnd(path);
        int bIndex = content.IndexOf(beginFlag);
        int eIndex = content.IndexOf(endFlag);
        allEndIndex = content.IndexOf("//End");
        if (bIndex == -1 || eIndex == -1)
        {
            if (allEndIndex == -1)
            {
                return insertStr;
            }
            return content.Insert(allEndIndex, insertStr + "\r\n\r\n");
        }
        eIndex = eIndex + content.Substring(eIndex).IndexOf("\r\n\r\n");
        if (eIndex == -1) return insertStr;
        return content.Substring(0, bIndex) + insertStr + content.Substring(eIndex + 4);
    }

    void AddToTemplateTop()
    {
        //string msgDefineOutPath = OutFolder + "ESocketEvent.cs";
        //string msgDefinePath = FileUtilityEditor.GetDirectory(EditorPath.NetVisibleToolFile) + "/Template/ESocketEventTemplate.cs";
        //AddToTemplate(msgDefineOutPath, msgDefinePath);

        //string netMsgStringOutPath = OutFolder + "NetMsg.cs";
        //string netMsgStringPath = FileUtilityEditor.GetDirectory(EditorPath.NetVisibleToolFile) + "/Template/NetMsgTemplate.cs";
        //AddToTemplate(netMsgStringOutPath, netMsgStringPath);

        //string rspPath_CaseOutPath = OutFolder + "NewEventProcess_Case.cs";
        //string rspPath_Case = FileUtilityEditor.GetDirectory(EditorPath.NetVisibleToolFile) + "/Template/NewEventProcess_CaseTemplate.cs";
        //AddToTemplate(rspPath_CaseOutPath, rspPath_Case);

        //string rspPath_FuncOutPath = OutFolder + "NewEventProcess_Func.cs";
        //string rspPath_Func = FileUtilityEditor.GetDirectory(EditorPath.NetVisibleToolFile) + "/Template/NewEventProcess_FuncTemplate.cs";
        //AddToTemplate(rspPath_FuncOutPath, rspPath_Func);
    }

    void AddToTemplate(string outpath, string templatePath)
    {
        string templateContent = File.ReadAllText(templatePath);
        string content = File.ReadAllText(outpath);
        content = string.Format(templateContent, content, OutFolder);
        FileUtilityEditor.Write(outpath, content, false);
    }
}