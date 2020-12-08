using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEditor.AnimatedValues;

/// <summary>
/// 服务器消息
/// </summary>
public class ServerProtocolLuaMessage : IEditorTool
{
    private int currentSelectedXMLIndex = 0;
    private string[] protocolStringArray;
    private Dictionary<string, int> xmlNameToIndexDic = new Dictionary<string, int>();
    private int currentDirection = 0;
    private string[] directionStringArray;
    private Vector2 scrollPos;
    private string keyword;
    private string[] viewOnlyDealInXLuaOrOnlyNoDealInXLuaArray;
    private int currentViewOnlyIndex = 0;
    private string[] viewOnlyUseCSStructArray;
    private int currentViewOnlyCallbackToCS = 0;
    private bool isEmptyMsgInViewRange = true;
    private bool showOptimiseUnnecessaryMsg = true;
    private Color infoColor = new Color(0.45f, 1, 0.72f);
    private int previousCount = 0;
    private AnimBool msgOptimiseToolAnimBool = new AnimBool(false);

    public ProtocolControlCache_ServerMessage Data = new ProtocolControlCache_ServerMessage();

    public ServerProtocolLuaMessage(ServerProtocolControllerWnd wnd)
    {
        xmlNameToIndexDic = new Dictionary<string, int>();
        Load();
    }

    #region 本地数据的保存与读取
    private static string _CacheFileName = "protocol_lua.xml";
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

    XmlSerializer CacheSerializer = new XmlSerializer(typeof(ProtocolControlCache_ServerMessage));
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
                Data = CacheSerializer.Deserialize(fs) as ProtocolControlCache_ServerMessage;
            }
        }
        else
        {
            Data = new ProtocolControlCache_ServerMessage();
        }
    }
    #endregion

    public void OnShown()
    {
        //对数据进行重新排序
        Data.Refresh();
        //协议列表
        List<string> protocolStringList = new List<string>();
        protocolStringList.Add("All");
        currentSelectedXMLIndex = 0;
        xmlNameToIndexDic.Clear();
        for (int i = 0; i < ServerProtocolListTool.Instance.Data.protocols.Count; i++)
        {
            protocolStringList.Add(ServerProtocolListTool.Instance.Data.protocols[i].protocolName + "  " + ServerProtocolListTool.Instance.Data.protocols[i].xmlName);
            xmlNameToIndexDic[ServerProtocolListTool.Instance.Data.protocols[i].xmlName.ToLower()] = i;
        }
        protocolStringArray = protocolStringList.ToArray();
        //流向列表
        directionStringArray = Enum.GetNames(typeof(ProtocolControlCache_Message.MessageDirection));
        //是否只在xlua中处理
        viewOnlyDealInXLuaOrOnlyNoDealInXLuaArray = new string[3] { "All", "只看XLua中处理的消息", "只看C#中处理的消息" };
        //是否仅查看使用C#数据结构分发的消息
        viewOnlyUseCSStructArray = new string[3] { "All", "只看使用C#数据结构的消息", "只看未使用使用C#数据结构的消息" };
    }

    public void OnGUI()
    {
        OnShown();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical(GUILayout.Width(450));
        {
            #region 操作按钮
            //说明文字
            GUI.contentColor = infoColor;
            EditorGUILayout.LabelField("协议更改后,点击下面的按钮");
            GUI.contentColor = Color.white;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("从协议文件刷新消息列表", GUILayout.Width(200)))
            {
                RefreshMessageFromProtocolFile();
            }
            EditorGUILayout.EndHorizontal();
            GUI.contentColor = infoColor;
            EditorGUILayout.LabelField("消息更改后,从左到右依次点击下面的按钮");
            GUI.contentColor = Color.white;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("重写XLua网络消息枚举文件", GUILayout.Width(200)))
            {
                RefreshNetDefEnumInXLua();
            }
            if (GUILayout.Button("重写网络消息处理方式控制文件", GUILayout.Width(200)))
            {
                RewriteMessageDealControlFile();
            }
            if (GUILayout.Button("重写网络消息处理XLua代码", GUILayout.Width(200)))
            {
                RefreshNetXLuaMethods();
            }
            if (GUILayout.Button("更新ServerLua提示文件", GUILayout.Width(200)))
            {
                LuaHintCreator.UpdateLuaHintFile(Application.dataPath.Substring(0, Application.dataPath.Length - 6) + "luaRes/protobufLua/proto", "serverprotoreference.lua", true);
            }
            EditorGUILayout.EndHorizontal();
            #endregion
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUILayout.Width(400));
        {
            #region 消息优化工具
            msgOptimiseToolAnimBool.target = EditorGUILayout.Foldout(msgOptimiseToolAnimBool.target, "消息结构优化工具", true);
            if (EditorGUILayout.BeginFadeGroup(msgOptimiseToolAnimBool.target ? 1 : 0))
            {
                if (GUILayout.Button("一键设定无引用类型的Req消息不使用C#数据结构", GUILayout.Width(380)))
                {
                    SetNonReferFieldReqMessageDoNotUseCSStructure(Data.messages);
                }
                if (GUILayout.Button("刷新protobuf类的Wrap黑名单", GUILayout.Width(380)))
                {
                    ExportWrapNotNecessaryMsgType(Data.messages);
                }
                if (GUILayout.Button("检查Lua中请求方法的格式(处理较慢,慎点!)", GUILayout.Width(380)))
                {
                    CheckRequestMethodInLua(Data.messages);
                }
            }
            EditorGUILayout.EndFadeGroup();
            #endregion
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();

        #region 消息刷新与筛选
        GUILayout.Label(string.Format("筛选出 {0} 条消息", previousCount), GUILayout.Width(400));
        GUI.enabled = true;
        if (protocolStringArray != null)
        {
            var currentSelectedXMLIndexTemp = EditorGUILayout.Popup("根据协议筛选", currentSelectedXMLIndex, protocolStringArray, GUILayout.Width(600));
            if (currentSelectedXMLIndex != currentSelectedXMLIndexTemp)
            {
                currentSelectedXMLIndex = currentSelectedXMLIndexTemp;
                ResetScrollViewPos();
            }
        }
        if (directionStringArray != null)
        {
            var currentDirectionTemp = EditorGUILayout.Popup("根据消息流向筛选", currentDirection, directionStringArray, GUILayout.Width(600));
            if (currentDirection != currentDirectionTemp)
            {
                currentDirection = currentDirectionTemp;
                ResetScrollViewPos();
            }
        }
        if (viewOnlyDealInXLuaOrOnlyNoDealInXLuaArray != null)
        {
            var currentViewOnlyIndexTemp = EditorGUILayout.Popup("根据消息处理方式筛选", currentViewOnlyIndex, viewOnlyDealInXLuaOrOnlyNoDealInXLuaArray, GUILayout.Width(600));
            if (currentViewOnlyIndex != currentViewOnlyIndexTemp)
            {
                currentViewOnlyIndex = currentViewOnlyIndexTemp;
                ResetScrollViewPos();
            }
        }
        if (viewOnlyUseCSStructArray != null)
        {
            var currentViewOnlyCallbackToCSTemp = EditorGUILayout.Popup("根据是否使用C#数据结构筛选", currentViewOnlyCallbackToCS, viewOnlyUseCSStructArray, GUILayout.Width(600));
            if (currentViewOnlyCallbackToCS != currentViewOnlyCallbackToCSTemp)
            {
                currentViewOnlyCallbackToCS = currentViewOnlyCallbackToCSTemp;
                ResetScrollViewPos();
            }
        }
        EditorGUILayout.BeginHorizontal();
        var isEmptyMsgInViewRangeTemp = EditorGUILayout.ToggleLeft("是否显示空消息体的消息", isEmptyMsgInViewRange, GUILayout.Width(200));
        if (isEmptyMsgInViewRange != isEmptyMsgInViewRangeTemp)
        {
            isEmptyMsgInViewRange = isEmptyMsgInViewRangeTemp;
            ResetScrollViewPos();
        }
        var showOptimiseUnnecessaryMsgTemp = EditorGUILayout.ToggleLeft("是否显示无需优化的消息", showOptimiseUnnecessaryMsg, GUILayout.Width(200));
        if (showOptimiseUnnecessaryMsg != showOptimiseUnnecessaryMsgTemp)
        {
            showOptimiseUnnecessaryMsg = showOptimiseUnnecessaryMsgTemp;
            ResetScrollViewPos();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("搜索: ", GUILayout.Width(50));
        var keywordTemp = EditorGUILayout.TextField(keyword, GUILayout.ExpandWidth(true));
        if (keyword != keywordTemp)
        {
            keyword = keywordTemp;
            ResetScrollViewPos();
        }
        EditorGUILayout.EndHorizontal();
        #endregion

        #region 显示消息列表
        EditorGUILayout.Space();
        int currentInShowCount = 0;
        for (int i = 0; i < Data.messages.Count; i++)
        {
            if (IsMessageAbleToShowInScrollViewList(Data.messages[i]))
            {
                currentInShowCount++;
            }
        }
        //绘制数量变化时,重置回原点
        if (currentInShowCount != previousCount)
        {
            previousCount = currentInShowCount;
        }
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        EditorGUILayout.BeginVertical();
        //绘制了多少消息
        int drawCount = 0;
        //当前绘制到的相对坐标
        float currentPos = 0;
        //每条消息的高度
        float heightPerMsg = 20;
        //是否超出了范围
        bool isOutOfRange = true;
        //当前的空格积累的高度值
        float currentSpaceHeight = 0;
        float lowestPos = scrollPos.y + 800;
        for (int i = 0; i < Data.messages.Count; i++)
        {
            if (!IsMessageAbleToShowInScrollViewList(Data.messages[i]))
            {
                continue;
            }
            bool isOutOfRangeTemp = currentPos < (scrollPos.y - 20) || currentPos > lowestPos;
            if (isOutOfRangeTemp)
            {
                //超出范围时
                currentSpaceHeight += heightPerMsg;
            }
            if (isOutOfRange != isOutOfRangeTemp)
            {
                if (currentSpaceHeight > 0)
                {
                    GUILayout.Space(currentSpaceHeight);
                }
                isOutOfRange = isOutOfRangeTemp;
                currentSpaceHeight = 0;
            }
            if (!isOutOfRange)
            {
                ShowMessageContentInScrollViewList(Data.messages[i], i);
                drawCount++;
            }
            currentPos += heightPerMsg;
        }
        if (currentSpaceHeight > 0)
        {
            GUILayout.Space(currentSpaceHeight);
        }
        if (drawCount == 0)
        {
            GUILayout.Space(10);
        }
        GUI.contentColor = Color.white;
        EditorGUILayout.EndVertical();
        GUI.enabled = true;
        EditorGUILayout.EndScrollView();
        #endregion
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 重置ScrollView的坐标
    /// </summary>
    private void ResetScrollViewPos()
    {
        scrollPos.y = 0;
    }

    /// <summary>
    /// 消息是否能在消息列表中显示
    /// </summary>
    /// <param name="messageTemp"></param>
    /// <returns></returns>
    private bool IsMessageAbleToShowInScrollViewList(ProtocolControlCache_Message messageTemp)
    {
        if (messageTemp == null)
        {
            return false;
        }
        //根据协议筛选
        if (currentSelectedXMLIndex != 0)
        {
            if (xmlNameToIndexDic.ContainsKey(messageTemp.xmlName.ToLower()) == false || xmlNameToIndexDic[messageTemp.xmlName.ToLower()] != currentSelectedXMLIndex - 1)
            {
                return false;
            }
        }
        //根据消息流向筛选
        if (currentDirection != (int)ProtocolControlCache_Message.MessageDirection.All)
        {
            if (currentDirection != (int)messageTemp.messageDirection)
            {
                return false;
            }
        }
        //根据是否查看xlua中处理的消息筛选
        if ((currentViewOnlyIndex == 1 && !messageTemp.isMessageDealInXLua) || (currentViewOnlyIndex == 2 && messageTemp.isMessageDealInXLua))
        {
            return false;
        }
        //在xlua中处理的消息,根据是否返包回C#作进一步筛选
        if ((currentViewOnlyCallbackToCS == 1 && !messageTemp.isNeedSendBacktoCSharp) || (currentViewOnlyCallbackToCS == 2 && messageTemp.isNeedSendBacktoCSharp))
        {
            return false;
        }
        //筛选掉空消息体的消息
        if (!isEmptyMsgInViewRange)
        {
            if (messageTemp.hasProto == false)
            {
                return false;
            }
        }
        //筛选掉无需优化的消息
        if (!showOptimiseUnnecessaryMsg)
        {
            if (messageTemp.messageDirection == ProtocolControlCache_Message.MessageDirection.ToClient && messageTemp.isMessageDealInXLua && messageTemp.hasProto && messageTemp.isNeedSendBacktoCSharp)
            {
                if (messageTemp.unableToOptimise)
                {
                    return false;
                }
            }
        }
        //关键词搜索
        if (!(IsConformToKeyword(messageTemp.messageID.ToString()) || IsConformToKeyword(messageTemp.messageName) || IsConformToKeyword(messageTemp.messageDescription)))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 在消息列表中显示消息内容
    /// </summary>
    /// <param name="messageTemp"></param>
    /// <param name="index">索引</param>
    /// <returns>单行消息高度</returns>
    private void ShowMessageContentInScrollViewList(ProtocolControlCache_Message messageTemp, int index)
    {
        //显示消息内容
        EditorGUILayout.BeginHorizontal();
        GUI.contentColor = messageTemp.isMessageDealInXLua ? Color.cyan : Color.white;
        EditorGUILayout.LabelField(index.ToString("d4") + ".  消息ID: " + messageTemp.messageID.ToString(), GUILayout.Width(150));
        EditorGUILayout.LabelField(string.IsNullOrEmpty(messageTemp.xmlName) ? string.Empty : (messageTemp.xmlName.ToString() + ".xml"), GUILayout.Width(80));
        EditorGUILayout.LabelField(messageTemp.messageName, GUILayout.Width(220));
        EditorGUILayout.LabelField(messageTemp.messageDescription, GUILayout.Width(220));
        //服务器发往客户端的消息有需要在lua中解析的选项
        if (messageTemp.messageDirection == ProtocolControlCache_Message.MessageDirection.ToClient)
        {
            messageTemp.isMessageDealInXLua = EditorGUILayout.ToggleLeft("在Lua中处理消息", messageTemp.isMessageDealInXLua, GUILayout.Width(150));
            //解析到的消息是否需要返回到C#中
            if (messageTemp.isMessageDealInXLua)
            {
                if (messageTemp.hasProto)
                {
                    messageTemp.isNeedSendBacktoCSharp = EditorGUILayout.ToggleLeft("Lua解析后将数据返给C#", messageTemp.isNeedSendBacktoCSharp, GUILayout.Width(150));
                }
                else
                {
                    EditorGUILayout.LabelField("消息无对应的proto结构", GUILayout.Width(150));
                }
            }
            else
            {
                EditorGUILayout.LabelField("消息在C#中处理", GUILayout.Width(150));
            }
        }
        else if (messageTemp.messageDirection == ProtocolControlCache_Message.MessageDirection.ToServer)
        {
            messageTemp.isMessageDealInXLua = EditorGUILayout.ToggleLeft("Lua中发送前校验消息", messageTemp.isMessageDealInXLua, GUILayout.Width(150));
            if (messageTemp.hasProto)
            {
                messageTemp.isNeedSendBacktoCSharp = EditorGUILayout.ToggleLeft("Lua发送时新建C#对象", messageTemp.isNeedSendBacktoCSharp, GUILayout.Width(150));
            }
            else
            {
                EditorGUILayout.LabelField("消息无对应的proto结构", GUILayout.Width(150));
            }
        }
        EditorGUILayout.LabelField(messageTemp.messageDirection.ToString(), GUILayout.Width(80));
        //显示协议是否使用proto以及proto名和message名
        EditorGUILayout.LabelField(messageTemp.hasProto ? (messageTemp.protoData.protoName + ".proto  " + messageTemp.protoData.protoMsgName) : string.Empty, GUILayout.Width(280));
        //分析是否可以优化
        if ((messageTemp.messageDirection == ProtocolControlCache_Message.MessageDirection.ToClient && messageTemp.isMessageDealInXLua && messageTemp.hasProto) ||
            (messageTemp.messageDirection == ProtocolControlCache_Message.MessageDirection.ToServer && messageTemp.hasProto))
        {
            if (messageTemp.isNeedSendBacktoCSharp)
            {
                messageTemp.unableToOptimise = EditorGUILayout.ToggleLeft(messageTemp.messageDirection == ProtocolControlCache_Message.MessageDirection.ToClient ? "是否不必优化(C#需使用消息内容)" : "是否不必优化(需要使用C#数据结构中转消息)", messageTemp.unableToOptimise, GUILayout.Width(250));
            }
            else
            {
                EditorGUILayout.LabelField("不需优化", GUILayout.Width(250));
            }
        }
        else
        {
            EditorGUILayout.LabelField("--------", GUILayout.Width(250));
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 刷新xlua中的网络消息枚举文件
    /// </summary>
    public void RefreshNetDefEnumInXLua()
    {
        CSStringBuilder.Clear();
        CSStringBuilder.AppendParams(Application.dataPath.Substring(0, Application.dataPath.Length - 7));
        CSStringBuilder.AppendParams("/luaRes/enum/LuaEnumNetDef.lua");
        string netdefEnumFilePath = CSStringBuilder.ToStringParams();
        string xmlName = string.Empty;
        bool needRegion = false;
        CSStringBuilder.Clear();
        CSStringBuilder.AppendParams("--[[本文件为工具自动生成,禁止手动修改]]\r\n");
        CSStringBuilder.AppendParams("---网络消息定义\r\n---@class LuaEnumNetDef\r\nLuaEnumNetDef = {}\r\n");
        Dictionary<string, int> msgNameDic = new Dictionary<string, int>();
        Dictionary<long, int> msgIDDic = new Dictionary<long, int>();
        for (int i = 0; i < Data.messages.Count; i++)
        {
            if (Data.messages[i] == null)
            {
                continue;
            }
            //折叠
            needRegion = !string.IsNullOrEmpty(Data.messages[i].xmlName) && Data.messages[i].xmlName != xmlName;
            if (needRegion)
            {
                if (!string.IsNullOrEmpty(xmlName))
                {
                    CSStringBuilder.AppendParams("--endregion\r\n");
                }
                xmlName = Data.messages[i].xmlName;
            }
            CSStringBuilder.AppendParams("\r\n");
            if (needRegion)
            {
                CSStringBuilder.AppendParams("--region ");
                CSStringBuilder.AppendParams("ID:");
                CSStringBuilder.AppendParams((int)(Data.messages[i].messageID / 1000));
                CSStringBuilder.AppendParams("  ");
                CSStringBuilder.AppendParams(Data.messages[i].xmlName);
                CSStringBuilder.AppendParams("\r\n");
            }
            //注释
            if (!string.IsNullOrEmpty(Data.messages[i].messageDescription))
            {
                CSStringBuilder.AppendParams("---");
                CSStringBuilder.AppendParams(Data.messages[i].messageDescription);
                CSStringBuilder.AppendParams("\r\n");
            }
            //proto
            if (Data.messages[i].hasProto && Data.messages[i].protoData != null)
            {
                CSStringBuilder.AppendParams("---");
                CSStringBuilder.AppendParams(Data.messages[i].protoData.protoName);
                CSStringBuilder.AppendParams("  ");
                CSStringBuilder.AppendParams(Data.messages[i].protoData.protoMsgName);
                CSStringBuilder.AppendParams("\r\n");
            }
            //消息流向
            CSStringBuilder.AppendParams("---");
            CSStringBuilder.AppendParams(Data.messages[i].messageDirection.ToString());
            CSStringBuilder.AppendParams("\r\n");
            //枚举
            CSStringBuilder.AppendParams("LuaEnumNetDef.");
            CSStringBuilder.AppendParams(Data.messages[i].messageName);
            CSStringBuilder.AppendParams(" = ");
            CSStringBuilder.AppendParams(Data.messages[i].messageID);
            CSStringBuilder.AppendParams("\r\n");
            //记录,以警告重复的msgName和msgID出现
            if (!msgNameDic.ContainsKey(Data.messages[i].messageName))
            {
                msgNameDic[Data.messages[i].messageName] = 1;
            }
            else
            {
                msgNameDic[Data.messages[i].messageName]++;
            }
            if (!msgIDDic.ContainsKey(Data.messages[i].messageID))
            {
                msgIDDic[Data.messages[i].messageID] = 1;
            }
            else
            {
                msgIDDic[Data.messages[i].messageID]++;
            }
        }
        if (!string.IsNullOrEmpty(xmlName))
        {
            CSStringBuilder.AppendParams("--endregion\r\n");
        }
        foreach (var item in msgNameDic)
        {
            if (item.Value > 1)
            {
                UnityEngine.Debug.LogErrorFormat("发现重复的消息名: {0}", item.Key);
            }
        }
        foreach (var item in msgIDDic)
        {
            if (item.Value > 1)
            {
                UnityEngine.Debug.LogErrorFormat("发现重复的消息ID: {0}", item.Key);
            }
        }
        try
        {
            if (File.Exists(netdefEnumFilePath))
            {
                File.Delete(netdefEnumFilePath);
            }
            File.WriteAllText(netdefEnumFilePath, CSStringBuilder.ToStringParams(), Encoding.UTF8);
            UnityEngine.Debug.Log("网络消息刷新完毕");
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError(string.Format("枚举文件转换失败\r\n{0}\r\n{1}", ex.Message, ex.StackTrace));
        }
    }

    /// <summary>
    /// 从协议文件刷新消息
    /// </summary>
    public void RefreshMessageFromProtocolFile()
    {
        if (string.IsNullOrEmpty(ServerProtocolControllerWnd.LocalServerProtoXMLPath))
        {
            UnityEngine.Debug.LogError("xml协议文件夹路径错误");
            return;
        }
        if (Directory.Exists(ServerProtocolControllerWnd.LocalServerProtoXMLPath))
        {
            //之前若有缓存的数据,则新读入的数据将按照messageID使用之前的缓存数据
            Dictionary<long, bool> isXluaDealMsgDicCache = new Dictionary<long, bool>();
            Dictionary<long, bool> isNeedSendBacktoCSCache = new Dictionary<long, bool>();
            Dictionary<long, bool> unableToOptimiseCache = new Dictionary<long, bool>();
            for (int i = 0; i < Data.messages.Count; i++)
            {
                isXluaDealMsgDicCache[Data.messages[i].messageID] = Data.messages[i].isMessageDealInXLua;
                isNeedSendBacktoCSCache[Data.messages[i].messageID] = Data.messages[i].isNeedSendBacktoCSharp;
                unableToOptimiseCache[Data.messages[i].messageID] = Data.messages[i].unableToOptimise;
            }
            Data.messages.Clear();
            string pathTemp;
            AddDefaultMessages();
            if (xmlNameToIndexDic != null && xmlNameToIndexDic.Count > 0)
            {
                foreach (var item in xmlNameToIndexDic)
                {
                    CSStringBuilder.Clear();
                    CSStringBuilder.AppendParams(ServerProtocolControllerWnd.LocalServerProtoXMLPath);
                    CSStringBuilder.AppendParams('/');
                    CSStringBuilder.AppendParams(item.Key);
                    CSStringBuilder.AppendParams(".xml");
                    pathTemp = CSStringBuilder.ToStringParams();
                    AddMessageInXML(ServerProtocolControllerWnd.DeserializeXML(pathTemp));
                }
            }
            for (int i = 0; i < Data.messages.Count; i++)
            {
                Data.messages[i].isMessageDealInXLua = isXluaDealMsgDicCache.ContainsKey(Data.messages[i].messageID) && isXluaDealMsgDicCache[Data.messages[i].messageID];
                Data.messages[i].isNeedSendBacktoCSharp = isNeedSendBacktoCSCache.ContainsKey(Data.messages[i].messageID) ? (isNeedSendBacktoCSCache[Data.messages[i].messageID]) : (false);
                Data.messages[i].unableToOptimise = unableToOptimiseCache.ContainsKey(Data.messages[i].messageID) ? (unableToOptimiseCache[Data.messages[i].messageID]) : (false);
            }
            Data.Refresh();
        }
        else
        {
            UnityEngine.Debug.LogError(string.Format("xml协议文件夹路径不存在:\r\n{0}", ServerProtocolControllerWnd.LocalServerProtoXMLPath));
        }
    }

    /// <summary>
    /// 刷新消息处理控制文件
    /// </summary>
    public void RewriteMessageDealControlFile()
    {
        string networkMsgConfigFilePath = Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "/luaRes/protobufLua/config/networkMsgConfig.txt";
        string dirPath = Path.GetDirectoryName(networkMsgConfigFilePath);
        if (Directory.Exists(dirPath) == false)
        {
            Directory.CreateDirectory(dirPath);
        }
        if (File.Exists(networkMsgConfigFilePath))
        {
            File.Delete(networkMsgConfigFilePath);
        }
        CSStringBuilder.Clear();
        CSStringBuilder.AppendParams("--[[本文件为工具自动生成,禁止手动修改]]");
        for (int i = 0; i < Data.messages.Count; i++)
        {
            if (Data.messages[i].messageDirection == ProtocolControlCache_Message.MessageDirection.ToClient && Data.messages[i].isMessageDealInXLua)
            {
                CSStringBuilder.AppendParams("\r\n");
                CSStringBuilder.AppendParams(Data.messages[i].messageID);
                CSStringBuilder.AppendParams("#On");
                CSStringBuilder.AppendParams(Data.messages[i].messageName);
                CSStringBuilder.AppendParams("Received");
            }
        }
        File.WriteAllText(networkMsgConfigFilePath, CSStringBuilder.ToStringParams(), Encoding.UTF8);
        UnityEngine.Debug.Log("消息处理流向控制文件刷新完毕");
    }

    /// <summary>
    /// 刷新网络相关的方法
    /// </summary>
    public void RefreshNetXLuaMethods()
    {
        try
        {
            ServerTool_ProtocolControllerUtility.ResetProgress();
            ServerTool_ProtocolControllerUtility.SetProgressRecord(0, 0.2f);
            RefreshReqNetXLuaMethods();
            ServerTool_ProtocolControllerUtility.ClearProgressRecord();

            ServerTool_ProtocolControllerUtility.SetProgressRecord(0.2f, 0.2f);
            RefreshResNetXLuaMethods();
            ServerTool_ProtocolControllerUtility.ClearProgressRecord();

            ServerTool_ProtocolControllerUtility.SetProgressRecord(0.4f, 0.2f);
            RefreshDeserializeNetXLuaMethods();
            ServerTool_ProtocolControllerUtility.ClearProgressRecord();

            ServerTool_ProtocolControllerUtility.SetProgressRecord(0.6f, 0.2f);
            RefreshPreprocessingNetMsgXLuaMethods();
            ServerTool_ProtocolControllerUtility.ClearProgressRecord();

            ServerTool_ProtocolControllerUtility.SetProgressRecord(0.8f, 0.2f);
            RefreshPreverifyingNetMsgXLuaMethod();
            ServerTool_ProtocolControllerUtility.ClearProgressRecord();
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError(string.Format("Error:{0}\r\n{1}", ex.Message, ex.StackTrace));
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    #region 添加消息
    /// <summary>
    /// 添加默认消息
    /// </summary>
    private void AddDefaultMessages()
    {
        //连接成功消息
        if (!Data.IsMessageExist(101))
        {
            ProtocolControlCache_Message connectSucceedMsg = new ProtocolControlCache_Message();
            connectSucceedMsg.messageID = (long)ESocketEvent.ConnectSuccess;
            connectSucceedMsg.messageName = "Res" + ESocketEvent.ConnectSuccess.ToString() + "Message";
            connectSucceedMsg.messageDescription = "客户端链接服务器成功";
            connectSucceedMsg.hasProto = false;
            connectSucceedMsg.isMessageDealInXLua = true;
            connectSucceedMsg.messageDirection = ProtocolControlCache_Message.MessageDirection.ToClient;
            connectSucceedMsg.xmlName = string.Empty;
            Data[connectSucceedMsg.messageID] = connectSucceedMsg;
        }
        //连接失败消息
        if (!Data.IsMessageExist(102))
        {
            ProtocolControlCache_Message connectFailedMsg = new ProtocolControlCache_Message();
            connectFailedMsg.messageID = (long)ESocketEvent.ConnectFailed;
            connectFailedMsg.messageName = "Res" + ESocketEvent.ConnectFailed.ToString() + "Message";
            connectFailedMsg.messageDescription = "客户端链接服务器失败";
            connectFailedMsg.hasProto = false;
            connectFailedMsg.isMessageDealInXLua = true;
            connectFailedMsg.messageDirection = ProtocolControlCache_Message.MessageDirection.ToClient;
            connectFailedMsg.xmlName = string.Empty;
            Data[connectFailedMsg.messageID] = connectFailedMsg;
        }
    }

    /// <summary>
    /// 向数据中添加xml文件中的消息
    /// </summary>
    /// <param name="xmlData"></param>
    private void AddMessageInXML(ProtocolControlCache_ProtocolXMLStructure xmlData)
    {
        if (xmlData == null)
        {
            return;
        }
        var serverProtocols = ServerProtocolListTool.Instance.Data;
        for (int i = 0; i < serverProtocols.protocols.Count; i++)
        {
            //规避不在lua中使用的协议
            if (serverProtocols.protocols[i].protocolName == xmlData.package && !serverProtocols.protocols[i].isProtocolUsedInLua)
            {
                return;
            }
        }
        for (int i = 0; i < xmlData.messageList.Count; i++)
        {
            ProtocolControlCache_Message message = new ProtocolControlCache_Message();
            message.messageID = xmlData.id * 1000 + xmlData.messageList[i].id;
            message.messageDirection = xmlData.messageList[i].type == "toServer" ? ProtocolControlCache_Message.MessageDirection.ToServer : ProtocolControlCache_Message.MessageDirection.ToClient;
            message.messageDescription = xmlData.messageList[i].description;
            message.messageName = xmlData.messageList[i].messageClass;
            message.xmlName = xmlData.protoName;
            message.hasProto = xmlData.messageList[i].proto != null && !string.IsNullOrEmpty(xmlData.messageList[i].proto.name);
            if (message.hasProto)
            {
                message.protoData.protoName = ServerProtocolControllerWnd.FormatProtoName(xmlData.messageList[i].proto.protoName);
                message.protoData.protoMsgName = xmlData.messageList[i].proto.protoMsgName;
            }
            Data[message.messageID] = message;
        }
    }
    #endregion

    #region 刷新请求消息
    /// <summary>
    /// 刷新请求消息
    /// </summary>
    private void RefreshReqNetXLuaMethods()
    {
        ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "刷新网络请求", 0);
        CSStringBuilder.Clear();
        CSStringBuilder.AppendParams(Application.dataPath.Substring(0, Application.dataPath.Length - 7));
        CSStringBuilder.AppendParams("/luaRes/protobufLua/networkRequest");
        string reqDirectoryPath = CSStringBuilder.ToStringParams();
        ServerProtocolControllerWnd.CreateOrClearFolder(reqDirectoryPath);
        //生成文件内容
        CSStringBuilder.Clear();
        CSStringBuilder.AppendParams(reqDirectoryPath);
        CSStringBuilder.AppendParams("/NetworkRequest.lua");
        string networkRequestLuaPath = CSStringBuilder.ToStringParams();
        StringBuilder networkReqLuaContent = new StringBuilder();
        networkReqLuaContent.Append("--[[本文件为工具自动生成,禁止手动修改]]\r\n");
        networkReqLuaContent.Append("---网络请求\r\nnetworkRequest = {}\r\n\r\n");
        Dictionary<string, StringBuilder> dirForLuaScripts = new Dictionary<string, StringBuilder>();
        ServerTool_ProtocolControllerUtility.SetProgressRecord(0, 0.8f);
        for (int i = 0; i < Data.messages.Count; i++)
        {
            ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "生成请求方法内容 " + Data.messages[i].messageName, i / ((float)Data.messages.Count));
            if (Data.messages[i].messageDirection == ProtocolControlCache_Message.MessageDirection.ToServer)
            {
                if (!string.IsNullOrEmpty(Data.messages[i].xmlName))
                {
                    if (!dirForLuaScripts.ContainsKey(Data.messages[i].xmlName))
                    {
                        dirForLuaScripts[Data.messages[i].xmlName] = new StringBuilder();
                        dirForLuaScripts[Data.messages[i].xmlName].Append("--[[本文件为工具自动生成,禁止手动修改]]\r\n");
                        dirForLuaScripts[Data.messages[i].xmlName].Append("--");
                        dirForLuaScripts[Data.messages[i].xmlName].Append(Data.messages[i].xmlName);
                        dirForLuaScripts[Data.messages[i].xmlName].Append(".xml\r\n\r\n");
                    }
                    dirForLuaScripts[Data.messages[i].xmlName].Append(GetRequestFunctionStringForMessage(Data.messages[i]));
                    dirForLuaScripts[Data.messages[i].xmlName].Append("\r\n");
                }
                else
                {
                    networkReqLuaContent.Append(GetRequestFunctionStringForMessage(Data.messages[i]));
                    networkReqLuaContent.Append("\r\n");
                }
            }
        }
        ServerTool_ProtocolControllerUtility.ClearProgressRecord();
        char[] c;
        StringBuilder charSB = new StringBuilder();
        ServerTool_ProtocolControllerUtility.SetProgressRecord(0.9f, 0.1f);
        float indexTemp1 = 0;
        int allCount = dirForLuaScripts.Count;
        foreach (var item in dirForLuaScripts)
        {
            ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "生成请求方法引用 " + item.Key, (indexTemp1++ / allCount));
            networkReqLuaContent.Append("--");
            networkReqLuaContent.Append(item.Key);
            networkReqLuaContent.Append(".xml\r\n");
            networkReqLuaContent.Append("require \'luaRes.protobufLua.networkRequest.NetworkRequest_");
            c = item.Key.ToArray();
            c[0] = (char)((c[0] <= 'z' && c[0] >= 'a') ? (c[0] - 32) : c[0]);
            charSB.Remove(0, charSB.Length);
            if (c != null)
            {
                for (int i = 0; i < c.Length; i++)
                {
                    charSB.Append(c[i]);
                }
            }
            networkReqLuaContent.Append(charSB.ToString());
            networkReqLuaContent.Append("\'\r\n");
        }
        ServerTool_ProtocolControllerUtility.ClearProgressRecord();
        //保存文件
        try
        {
            if (File.Exists(networkRequestLuaPath))
            {
                File.Delete(networkRequestLuaPath);
            }
            File.WriteAllText(networkRequestLuaPath, networkReqLuaContent.ToString(), Encoding.UTF8);
            ServerTool_ProtocolControllerUtility.SetProgressRecord(0.9f, 0.1f);
            indexTemp1 = 0;
            allCount = dirForLuaScripts.Count;
            foreach (var item in dirForLuaScripts)
            {
                ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "写入文件中 " + item.Key, (indexTemp1++ / allCount));
                c = item.Key.ToArray();
                c[0] = (char)((c[0] <= 'z' && c[0] >= 'a') ? (c[0] - 32) : c[0]);
                charSB.Remove(0, charSB.Length);
                if (c != null)
                {
                    for (int i = 0; i < c.Length; i++)
                    {
                        charSB.Append(c[i]);
                    }
                }
                CSStringBuilder.Clear();
                CSStringBuilder.AppendParams(reqDirectoryPath);
                CSStringBuilder.AppendParams("/NetworkRequest_");
                CSStringBuilder.AppendParams(charSB.ToString());
                CSStringBuilder.AppendParams(".lua");
                string filePath = CSStringBuilder.ToStringParams();
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.WriteAllText(filePath, item.Value.ToString(), Encoding.UTF8);
            }
            ServerTool_ProtocolControllerUtility.ClearProgressRecord();
            UnityEngine.Debug.Log("NetworkRequest 网络请求脚本刷新完毕");
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError(string.Format("保存文件失败\r\n{0}\r\n{1}", ex.Message, ex.StackTrace));
        }
    }

    /// <summary>
    /// 获取消息对应的请求方法字符串
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    private string GetRequestFunctionStringForMessage(ProtocolControlCache_Message msg)
    {
        if (msg == null)
        {
            return string.Empty;
        }
        ProtocolControlCache_ProtoStructure ps = ServerProtocolControllerWnd.GetProtoStructure(msg);
        ProtocolControlCache_ProtoStructure.Message msgInProto = null;
        if (msg.hasProto)
        {
            if (ps == null)
            {
                UnityEngine.Debug.LogError("未找到proto文件: " + msg.protoData.protoName + "." + msg.protoData.protoMsgName);
                return string.Empty;
            }
            for (int i = 0; i < ps.Messages.Count; i++)
            {
                if (msg.protoData.protoMsgName == ps.Messages[i].MessageName)
                {
                    msgInProto = ps.Messages[i];
                    break;
                }
            }
        }
        CSStringBuilder.Clear();
        CSStringBuilder.AppendParams("--region ID:");
        CSStringBuilder.AppendParams(msg.messageID);
        CSStringBuilder.AppendParams(" ");
        CSStringBuilder.AppendParams(msg.messageDescription);
        CSStringBuilder.AppendParams("\r\n---");
        CSStringBuilder.AppendParams(msg.messageDescription);
        CSStringBuilder.AppendParams("\r\n");
        CSStringBuilder.AppendParams("---msgID: ");
        CSStringBuilder.AppendParams(msg.messageID);
        CSStringBuilder.AppendParams("\r\n");
        if (msgInProto != null)
        {
            for (int i = 0; i < msgInProto.Variables.Length; i++)
            {
                CSStringBuilder.AppendParams("---@param ");
                CSStringBuilder.AppendParams(msgInProto.Variables[i].VariableName);
                CSStringBuilder.AppendParams(" ");
                string variableType = GetStringOfVariableTypeInProto(ps, msgInProto.Variables[i]);
                if (msgInProto.Variables[i].Modifier == ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Repeated)
                {
                    if (msg.isNeedSendBacktoCSharp)
                    {
                        CSStringBuilder.AppendParams("System.Collections.Generic.List1T<");
                        CSStringBuilder.AppendParams(variableType);
                        CSStringBuilder.AppendParams(">");
                    }
                    else
                    {
                        CSStringBuilder.AppendParams("table<");
                        CSStringBuilder.AppendParams(variableType);
                        CSStringBuilder.AppendParams(">");
                    }
                }
                else
                {
                    CSStringBuilder.AppendParams(variableType);
                }
                switch (msgInProto.Variables[i].Modifier)
                {
                    case ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Required:
                        CSStringBuilder.AppendParams(" 必填参数");
                        break;
                    case ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Optional:
                        CSStringBuilder.AppendParams(" 选填参数");
                        break;
                    case ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Repeated:
                        CSStringBuilder.AppendParams(" 列表参数");
                        break;
                    case ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.NULL:
                    default: break;
                }
                if (!string.IsNullOrEmpty(msgInProto.Variables[i].Comment))
                {
                    CSStringBuilder.AppendParams(' ');
                    CSStringBuilder.AppendParams(msgInProto.Variables[i].Comment);
                }
                CSStringBuilder.AppendParams("\r\n");
            }
        }
        CSStringBuilder.AppendParams("---@return boolean 网络请求是否成功发送\r\nfunction networkRequest.");
        CSStringBuilder.AppendParams(msg.messageName.Replace("Message", string.Empty));
        CSStringBuilder.AppendParams('(');
        if (msgInProto != null)
        {
            for (int i = 0; i < msgInProto.Variables.Length; i++)
            {
                CSStringBuilder.AppendParams(msgInProto.Variables[i].VariableName);
                if (i != msgInProto.Variables.Length - 1)
                {
                    CSStringBuilder.AppendParams(", ");
                }
            }
        }
        CSStringBuilder.AppendParams(")\r\n");
        if (msgInProto != null)
        {
            if (msg.isNeedSendBacktoCSharp)
            {
                CSStringBuilder.AppendParams("    local reqData = CS.");
                CSStringBuilder.AppendParams(ps.Package);
                CSStringBuilder.AppendParams('.');
                CSStringBuilder.AppendParams(msgInProto.MessageName);
                CSStringBuilder.AppendParams("()\r\n");
                for (int i = 0; i < msgInProto.Variables.Length; i++)
                {
                    ProtocolControlCache_ProtoStructure.VariableInMessage variable = msgInProto.Variables[i];
                    switch (variable.Modifier)
                    {
                        case ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Required:
                            CSStringBuilder.AppendParams("    reqData.");
                            CSStringBuilder.AppendParams(variable.VariableName);
                            CSStringBuilder.AppendParams(" = ");
                            CSStringBuilder.AppendParams(variable.VariableName);
                            CSStringBuilder.AppendParams("\r\n");
                            break;
                        case ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Optional:
                            CSStringBuilder.AppendParams("    if ");
                            CSStringBuilder.AppendParams(variable.VariableName);
                            CSStringBuilder.AppendParams(" ~= nil then\r\n");
                            CSStringBuilder.AppendParams("        reqData.");
                            CSStringBuilder.AppendParams(variable.VariableName);
                            CSStringBuilder.AppendParams(" = ");
                            CSStringBuilder.AppendParams(variable.VariableName);
                            CSStringBuilder.AppendParams("\r\n    end\r\n");
                            break;
                        case ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Repeated:
                            CSStringBuilder.AppendParams("    if ");
                            CSStringBuilder.AppendParams(variable.VariableName);
                            CSStringBuilder.AppendParams(" ~= nil then\r\n");
                            CSStringBuilder.AppendParams("        reqData.");
                            CSStringBuilder.AppendParams(variable.VariableName);
                            CSStringBuilder.AppendParams(":AddRange(");
                            CSStringBuilder.AppendParams(variable.VariableName);
                            CSStringBuilder.AppendParams(")\r\n    end\r\n");
                            break;
                        case ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.NULL:
                        default: break;
                    }
                }
            }
            else
            {
                CSStringBuilder.AppendParams("    local reqTable = {}\r\n");
                for (int i = 0; i < msgInProto.Variables.Length; i++)
                {
                    ProtocolControlCache_ProtoStructure.VariableInMessage variable = msgInProto.Variables[i];
                    switch (variable.Modifier)
                    {
                        case ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Required:
                            CSStringBuilder.AppendParams("    reqTable.");
                            CSStringBuilder.AppendParams(variable.VariableName);
                            CSStringBuilder.AppendParams(" = ");
                            CSStringBuilder.AppendParams(variable.VariableName);
                            CSStringBuilder.AppendParams("\r\n");
                            break;
                        case ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Optional:
                            CSStringBuilder.AppendParams("    if ");
                            CSStringBuilder.AppendParams(variable.VariableName);
                            CSStringBuilder.AppendParams(" ~= nil then\r\n");
                            CSStringBuilder.AppendParams("        reqTable.");
                            CSStringBuilder.AppendParams(variable.VariableName);
                            CSStringBuilder.AppendParams(" = ");
                            CSStringBuilder.AppendParams(variable.VariableName);
                            CSStringBuilder.AppendParams("\r\n    end\r\n");
                            break;
                        case ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Repeated:
                            CSStringBuilder.AppendParams("    if ");
                            CSStringBuilder.AppendParams(variable.VariableName);
                            CSStringBuilder.AppendParams(" ~= nil then\r\n");
                            CSStringBuilder.AppendParams("        reqTable.");
                            CSStringBuilder.AppendParams(variable.VariableName);
                            CSStringBuilder.AppendParams(" = ");
                            CSStringBuilder.AppendParams(variable.VariableName);
                            CSStringBuilder.AppendParams("\r\n    else\r\n        reqTable.");
                            CSStringBuilder.AppendParams(variable.VariableName);
                            CSStringBuilder.AppendParams(" = {}\r\n    end\r\n");
                            break;
                        case ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.NULL:
                        default: break;
                    }
                }
                CSStringBuilder.AppendParams("    local reqMsgData = protobufMgr.Serialize(\"");
                CSStringBuilder.AppendParams(ps.Package);
                CSStringBuilder.AppendParams(".");
                CSStringBuilder.AppendParams(msgInProto.MessageName);
                CSStringBuilder.AppendParams("\" , reqTable)\r\n");
            }
        }
        CSStringBuilder.AppendParams("    local canSendMsg = true\r\n    if netMsgPreverifying and netMsgPreverifying[LuaEnumNetDef.");
        CSStringBuilder.AppendParams(msg.messageName);
        CSStringBuilder.AppendParams("] then\r\n        canSendMsg = netMsgPreverifying[LuaEnumNetDef.");
        CSStringBuilder.AppendParams(msg.messageName);
        CSStringBuilder.AppendParams("](LuaEnumNetDef.");
        CSStringBuilder.AppendParams(msg.messageName);
        if (msgInProto != null)
        {
            if (msg.isNeedSendBacktoCSharp)
            {
                CSStringBuilder.AppendParams(", reqData)\r\n    end\r\n    if canSendMsg then\r\n        CS.CSNetwork.Instance:SendMsg(LuaEnumNetDef.");
                CSStringBuilder.AppendParams(msg.messageName);
                CSStringBuilder.AppendParams(", reqData, true)\r\n    end\r\n    return canSendMsg\r\n");
            }
            else
            {
                CSStringBuilder.AppendParams(", reqTable)\r\n    end\r\n    if canSendMsg then\r\n        CS.CSNetwork.Instance:SendMsgByteByLua(LuaEnumNetDef.");
                CSStringBuilder.AppendParams(msg.messageName);
                CSStringBuilder.AppendParams(", reqMsgData)\r\n        if isOpenLog then\r\n            luaDebug.WriteNetMsgToLog(\"");
                CSStringBuilder.AppendParams(msg.messageName);
                CSStringBuilder.AppendParams("\", ");
                CSStringBuilder.AppendParams(msg.messageID);
                CSStringBuilder.AppendParams(", \"");
                CSStringBuilder.AppendParams(msg.protoData.protoMsgName);
                CSStringBuilder.AppendParams("\", reqTable, true)\r\n        end\r\n    end\r\n    return canSendMsg\r\n");
            }
        }
        else
        {
            CSStringBuilder.AppendParams(", nil)\r\n    end\r\n    if canSendMsg then\r\n        CS.CSNetwork.Instance:SendMsg(LuaEnumNetDef.");
            CSStringBuilder.AppendParams(msg.messageName);
            CSStringBuilder.AppendParams(", nil, true)\r\n    end\r\n    return canSendMsg\r\n");
        }
        CSStringBuilder.AppendParams("end\r\n--endregion\r\n");
        return CSStringBuilder.ToStringParams();
    }
    #endregion

    #region 刷新响应消息
    /// <summary>
    /// 刷新响应消息
    /// </summary>
    private void RefreshResNetXLuaMethods()
    {
        ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "刷新网络响应", 0);
        CSStringBuilder.Clear();
        CSStringBuilder.AppendParams(Application.dataPath.Substring(0, Application.dataPath.Length - 7));
        CSStringBuilder.AppendParams("/luaRes/protobufLua/networkRespond");
        string resDirectoryPath = CSStringBuilder.ToStringParams();
        ServerProtocolControllerWnd.CreateOrClearFolder(resDirectoryPath);
        //生成文件内容
        CSStringBuilder.Clear();
        CSStringBuilder.AppendParams(resDirectoryPath);
        CSStringBuilder.AppendParams("/NetworkRespond.lua");
        string networkRespondLuaPath = CSStringBuilder.ToStringParams();
        StringBuilder networkResLuaContent = new StringBuilder();
        networkResLuaContent.Append("--[[本文件为工具自动生成,禁止手动修改]]\r\n");
        networkResLuaContent.Append("---网络消息响应\r\nnetworkRespond = {}\r\n\r\n");
        Dictionary<string, StringBuilder> dirForLuaScripts = new Dictionary<string, StringBuilder>();
        ServerTool_ProtocolControllerUtility.SetProgressRecord(0, 0.3f);
        for (int i = 0; i < Data.messages.Count; i++)
        {
            ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "生成响应代码中 " + Data.messages[i].messageName, i / ((float)Data.messages.Count));
            if (Data.messages[i].messageDirection == ProtocolControlCache_Message.MessageDirection.ToClient && Data.messages[i].isMessageDealInXLua)
            {
                if (!string.IsNullOrEmpty(Data.messages[i].xmlName))
                {
                    if (!dirForLuaScripts.ContainsKey(Data.messages[i].xmlName))
                    {
                        dirForLuaScripts[Data.messages[i].xmlName] = new StringBuilder();
                        dirForLuaScripts[Data.messages[i].xmlName].Append("--[[本文件为工具自动生成,禁止手动修改]]\r\n");
                        dirForLuaScripts[Data.messages[i].xmlName].Append("--");
                        dirForLuaScripts[Data.messages[i].xmlName].Append(Data.messages[i].xmlName);
                        dirForLuaScripts[Data.messages[i].xmlName].Append(".xml\r\nlocal protobufMgr = protobufMgr\r\nlocal commonNetMsgDeal = commonNetMsgDeal\r\n\r\n");
                    }
                    dirForLuaScripts[Data.messages[i].xmlName].Append(GetRespondFunctionStringForMessage(Data.messages[i]));
                    dirForLuaScripts[Data.messages[i].xmlName].Append("\r\n");
                }
                else
                {
                    networkResLuaContent.Append(GetRespondFunctionStringForMessage(Data.messages[i]));
                    networkResLuaContent.Append("\r\n");
                }
            }
        }
        ServerTool_ProtocolControllerUtility.ClearProgressRecord();
        char[] c;
        StringBuilder charSB = new StringBuilder();
        ServerTool_ProtocolControllerUtility.SetProgressRecord(0.3f, 0.3f);
        float indexTemp1 = 0;
        int allCount = dirForLuaScripts.Count;
        foreach (var item in dirForLuaScripts)
        {
            ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "写入文件中 " + item.Key, (indexTemp1++ / allCount));
            networkResLuaContent.Append("--");
            networkResLuaContent.Append(item.Key);
            networkResLuaContent.Append(".xml\r\n");
            networkResLuaContent.Append("require \'luaRes.protobufLua.networkRespond.NetworkRespond_");
            c = item.Key.ToArray();
            c[0] = (char)((c[0] <= 'z' && c[0] >= 'a') ? (c[0] - 32) : c[0]);
            charSB.Remove(0, charSB.Length);
            if (c != null)
            {
                for (int i = 0; i < c.Length; i++)
                {
                    charSB.Append(c[i]);
                }
            }
            networkResLuaContent.Append(charSB.ToString());
            networkResLuaContent.Append("\'\r\n");
        }
        ServerTool_ProtocolControllerUtility.ClearProgressRecord();
        //保存文件
        try
        {
            indexTemp1 = 0;
            allCount = dirForLuaScripts.Count;
            ServerTool_ProtocolControllerUtility.SetProgressRecord(0.6f, 0.4f);
            if (File.Exists(networkRespondLuaPath))
            {
                File.Delete(networkRespondLuaPath);
            }
            File.WriteAllText(networkRespondLuaPath, networkResLuaContent.ToString(), Encoding.UTF8);
            foreach (var item in dirForLuaScripts)
            {
                ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "生成响应方法引用 " + item.Key, (indexTemp1++ / allCount));
                c = item.Key.ToArray();
                c[0] = (char)((c[0] <= 'z' && c[0] >= 'a') ? (c[0] - 32) : c[0]);
                charSB.Remove(0, charSB.Length);
                if (c != null)
                {
                    for (int i = 0; i < c.Length; i++)
                    {
                        charSB.Append(c[i]);
                    }
                }
                CSStringBuilder.Clear();
                CSStringBuilder.AppendParams(resDirectoryPath);
                CSStringBuilder.AppendParams("/NetworkRespond_");
                CSStringBuilder.AppendParams(charSB.ToString());
                CSStringBuilder.AppendParams(".lua");
                string filePath = CSStringBuilder.ToStringParams();
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.WriteAllText(filePath, item.Value.ToString(), Encoding.UTF8);
            }
            ServerTool_ProtocolControllerUtility.ClearProgressRecord();
            UnityEngine.Debug.Log("NetworkRespond 网络消息响应脚本刷新完毕");
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError(string.Format("保存文件失败\r\n{0}\r\n{1}", ex.Message, ex.StackTrace));
        }
    }

    /// <summary>
    /// 获取消息对应的响应方法字符串
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    private string GetRespondFunctionStringForMessage(ProtocolControlCache_Message msg)
    {
        if (msg == null)
        {
            return string.Empty;
        }
        ProtocolControlCache_ProtoStructure ps = ServerProtocolControllerWnd.GetProtoStructure(msg);
        ProtocolControlCache_ProtoStructure.Message msgInProto = null;
        if (msg.hasProto)
        {
            if (ps == null)
            {
                UnityEngine.Debug.LogError("未找到proto文件: " + msg.protoData.protoName + "." + msg.protoData.protoMsgName);
                return string.Empty;
            }
            for (int i = 0; i < ps.Messages.Count; i++)
            {
                if (msg.protoData.protoMsgName == ps.Messages[i].MessageName)
                {
                    msgInProto = ps.Messages[i];
                    break;
                }
            }
        }
        CSStringBuilder.Clear();
        CSStringBuilder.AppendParams("---");
        CSStringBuilder.AppendParams(msg.messageDescription);
        CSStringBuilder.AppendParams("\r\n");
        CSStringBuilder.AppendParams("---msgID: ");
        CSStringBuilder.AppendParams(msg.messageID);
        CSStringBuilder.AppendParams("\r\n");
        CSStringBuilder.AppendParams("---@param msgID LuaEnumNetDef 消息ID\r\n---@param tblData table 消息内容\r\n");
        CSStringBuilder.AppendParams("---@return ");
        if (msgInProto != null)
        {
            CSStringBuilder.AppendParams(ps.Package);
            CSStringBuilder.AppendParams('.');
            CSStringBuilder.AppendParams(msgInProto.MessageName);
            CSStringBuilder.AppendParams(" C#数据结构");
        }
        else
        {
            CSStringBuilder.AppendParams("nil");
        }
        CSStringBuilder.AppendParams("\r\n");
        CSStringBuilder.AppendParams("function networkRespond.On");
        CSStringBuilder.AppendParams(msg.messageName);
        CSStringBuilder.AppendParams("Received(msgID, tblData)\r\n");
        if (msgInProto != null)
        {
            CSStringBuilder.AppendParams("    if tblData == nil then\r\n        CS.OnlineDebug.LogError(\"Lua消息内容为空\\r\\nID: ");
            CSStringBuilder.AppendParams(msg.messageID);
            CSStringBuilder.AppendParams(' ');
            CSStringBuilder.AppendParams(ps.Package);
            CSStringBuilder.AppendParams('.');
            CSStringBuilder.AppendParams(msgInProto.MessageName);
            CSStringBuilder.AppendParams(' ');
            CSStringBuilder.AppendParams(msg.messageDescription);
            CSStringBuilder.AppendParams("\")\r\n        commonNetMsgDeal.DoCallback(msgID, nil)\r\n        return nil\r\n    end\r\n");
            if (msg.isNeedSendBacktoCSharp)
            {
                if (ServerTool_ProtocolControllerUtility.IsAnyFieldNotExistInMessage(msg))
                {
                    CSStringBuilder.AppendParams("    ---C#中对消息结构引用到的某个类型的字段未实现,故需要打印出lua的网络日志\r\n    if isOpenLog then\r\n");
                    CSStringBuilder.AppendParams(string.Format("        luaDebug.WriteNetMsgToLog(\"{0}\", {1}, \"{2}\", tblData)\r\n    end\r\n", msg.messageName, msg.messageID, msg.protoData.protoMsgName));
                }
                CSStringBuilder.AppendParams("    local csData\r\n    if protobufMgr.DecodeTable.");
                CSStringBuilder.AppendParams(msg.protoData.protoName);
                CSStringBuilder.AppendParams(" ~= nil and  protobufMgr.DecodeTable.");
                CSStringBuilder.AppendParams(msg.protoData.protoName);
                CSStringBuilder.AppendParams('.');
                CSStringBuilder.AppendParams(msg.protoData.protoMsgName);
                CSStringBuilder.AppendParams(" ~= nil then\r\n");
                CSStringBuilder.AppendParams("        csData = protobufMgr.DecodeTable.");
                CSStringBuilder.AppendParams(msg.protoData.protoName);
                CSStringBuilder.AppendParams('.');
                CSStringBuilder.AppendParams(msg.protoData.protoMsgName);
                CSStringBuilder.AppendParams("(tblData)\r\n    end\r\n");
                CSStringBuilder.AppendParams("    commonNetMsgDeal.DoCallback(msgID, tblData, csData)\r\n");
                CSStringBuilder.AppendParams("    return csData\r\n");
            }
            else
            {
                CSStringBuilder.AppendParams("    ---消息不回写到C#中\r\n    if isOpenLog then\r\n");
                CSStringBuilder.AppendParams(string.Format("        luaDebug.WriteNetMsgToLog(\"{0}\", {1}, \"{2}\", tblData)\r\n    end\r\n", msg.messageName, msg.messageID, msg.protoData.protoMsgName));
                CSStringBuilder.AppendParams("    commonNetMsgDeal.DoCallback(msgID, tblData, nil)\r\n");
                CSStringBuilder.AppendParams("    return nil\r\n");
            }
        }
        else
        {
            CSStringBuilder.AppendParams("    commonNetMsgDeal.DoCallback(msgID, nil, nil)\r\n    return nil\r\n");
        }
        CSStringBuilder.AppendParams("end\r\n");
        return CSStringBuilder.ToStringParams();
    }
    #endregion

    #region 刷新反序列化方法
    /// <summary>
    /// 刷新反序列化方法字符串
    /// </summary>
    private void RefreshDeserializeNetXLuaMethods()
    {
        //EditorUtility.DisplayProgressBar("刷新网络消息相关方法", "刷新网络反序列化", 0.65f);
        ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "刷新网络反序列化", 0);
        CSStringBuilder.Clear();
        CSStringBuilder.AppendParams(Application.dataPath.Substring(0, Application.dataPath.Length - 7));
        CSStringBuilder.AppendParams("/luaRes/protobufLua/networkDeserialize");
        string deserializeDirectoryPath = CSStringBuilder.ToStringParams();
        ServerProtocolControllerWnd.CreateOrClearFolder(deserializeDirectoryPath);
        //生成文件内容
        CSStringBuilder.Clear();
        CSStringBuilder.AppendParams(deserializeDirectoryPath);
        CSStringBuilder.AppendParams("/NetworkDeserialize.lua");
        string networkDeserializeLuaPath = CSStringBuilder.ToStringParams();
        StringBuilder networkDeserializeLuaContent = new StringBuilder();
        networkDeserializeLuaContent.Append("--[[本文件为工具自动生成,禁止手动修改]]\r\n");
        networkDeserializeLuaContent.Append("---网络消息解析\r\nnetworkDeserialize = {}\r\n\r\n");
        Dictionary<string, StringBuilder> dirForLuaScripts = new Dictionary<string, StringBuilder>();
        ServerTool_ProtocolControllerUtility.SetProgressRecord(0, 0.3f);
        for (int i = 0; i < Data.messages.Count; i++)
        {
            ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "生成反序列化代码中  " + Data.messages[i].messageName, ((float)i) / Data.messages.Count);
            if (Data.messages[i].messageDirection == ProtocolControlCache_Message.MessageDirection.ToClient && Data.messages[i].isMessageDealInXLua)
            {
                if (!string.IsNullOrEmpty(Data.messages[i].xmlName))
                {
                    if (!dirForLuaScripts.ContainsKey(Data.messages[i].xmlName))
                    {
                        dirForLuaScripts[Data.messages[i].xmlName] = new StringBuilder();
                        dirForLuaScripts[Data.messages[i].xmlName].Append("--[[本文件为工具自动生成,禁止手动修改]]\r\n");
                        dirForLuaScripts[Data.messages[i].xmlName].Append("--");
                        dirForLuaScripts[Data.messages[i].xmlName].Append(Data.messages[i].xmlName);
                        dirForLuaScripts[Data.messages[i].xmlName].Append(".xml\r\nlocal protobufMgr = protobufMgr\r\nlocal protoAdjust = protobufMgr.AdjustTable\r\n\r\n");
                    }
                    dirForLuaScripts[Data.messages[i].xmlName].Append(GetDeserializeFunctionStringForMessage(Data.messages[i]));
                    dirForLuaScripts[Data.messages[i].xmlName].Append("\r\n");
                }
                else
                {
                    networkDeserializeLuaContent.Append(GetDeserializeFunctionStringForMessage(Data.messages[i]));
                    networkDeserializeLuaContent.Append("\r\n");
                }
            }
        }
        ServerTool_ProtocolControllerUtility.ClearProgressRecord();
        char[] c;
        StringBuilder charSB = new StringBuilder();
        ServerTool_ProtocolControllerUtility.SetProgressRecord(0.3f, 0.3f);
        int indexTemp1 = 0;
        float countTemp1 = dirForLuaScripts.Count;
        foreach (var item in dirForLuaScripts)
        {
            ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "刷新网络反序列化  " + item.Key, indexTemp1++ / countTemp1);
            networkDeserializeLuaContent.Append("--");
            networkDeserializeLuaContent.Append(item.Key);
            networkDeserializeLuaContent.Append(".xml\r\n");
            networkDeserializeLuaContent.Append("require \'luaRes.protobufLua.networkDeserialize.NetworkDeserialize_");
            c = item.Key.ToArray();
            c[0] = (char)((c[0] <= 'z' && c[0] >= 'a') ? (c[0] - 32) : c[0]);
            charSB.Remove(0, charSB.Length);
            if (c != null)
            {
                for (int i = 0; i < c.Length; i++)
                {
                    charSB.Append(c[i]);
                }
            }
            networkDeserializeLuaContent.Append(charSB.ToString());
            networkDeserializeLuaContent.Append("\'\r\n");
        }
        ServerTool_ProtocolControllerUtility.ClearProgressRecord();
        //保存文件
        try
        {
            indexTemp1 = 0;
            countTemp1 = dirForLuaScripts.Count;
            ServerTool_ProtocolControllerUtility.SetProgressRecord(0.6f, 0.4f);
            if (File.Exists(networkDeserializeLuaPath))
            {
                File.Delete(networkDeserializeLuaPath);
            }
            File.WriteAllText(networkDeserializeLuaPath, networkDeserializeLuaContent.ToString(), Encoding.UTF8);
            foreach (var item in dirForLuaScripts)
            {
                ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "刷新网络反序列化  " + item.Key, indexTemp1++ / countTemp1);
                c = item.Key.ToArray();
                c[0] = (char)((c[0] <= 'z' && c[0] >= 'a') ? (c[0] - 32) : c[0]);
                charSB.Remove(0, charSB.Length);
                if (c != null)
                {
                    for (int i = 0; i < c.Length; i++)
                    {
                        charSB.Append(c[i]);
                    }
                }
                CSStringBuilder.Clear();
                CSStringBuilder.AppendParams(deserializeDirectoryPath);
                CSStringBuilder.AppendParams("/NetworkDeserialize_");
                CSStringBuilder.AppendParams(charSB.ToString());
                CSStringBuilder.AppendParams(".lua");
                string filePath = CSStringBuilder.ToStringParams();
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.WriteAllText(filePath, item.Value.ToString(), Encoding.UTF8);
            }
            ServerTool_ProtocolControllerUtility.ClearProgressRecord();
            UnityEngine.Debug.Log("NetworkDeserialize 网络消息反序列化脚本刷新完毕");
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError(string.Format("保存文件失败\r\n{0}\r\n{1}", ex.Message, ex.StackTrace));
        }
    }

    /// <summary>
    /// 获取消息对应的反序列化方法字符串
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    private string GetDeserializeFunctionStringForMessage(ProtocolControlCache_Message msg)
    {
        if (msg == null)
        {
            return string.Empty;
        }
        ProtocolControlCache_ProtoStructure ps = ServerProtocolControllerWnd.GetProtoStructure(msg);
        ProtocolControlCache_ProtoStructure.Message msgInProto = null;
        if (msg.hasProto)
        {
            if (ps == null)
            {
                UnityEngine.Debug.LogError("未找到proto文件: " + msg.protoData.protoName + "." + msg.protoData.protoMsgName);
                return string.Empty;
            }
            for (int i = 0; i < ps.Messages.Count; i++)
            {
                if (msg.protoData.protoMsgName == ps.Messages[i].MessageName)
                {
                    msgInProto = ps.Messages[i];
                    break;
                }
            }
        }
        CSStringBuilder.Clear();
        CSStringBuilder.AppendParams("---");
        CSStringBuilder.AppendParams(msg.messageDescription);
        CSStringBuilder.AppendParams("\r\n");
        CSStringBuilder.AppendParams("---msgID: ");
        CSStringBuilder.AppendParams(msg.messageID);
        CSStringBuilder.AppendParams("\r\n");
        CSStringBuilder.AppendParams("---@param msgID LuaEnumNetDef 消息ID\r\n---@param buffer string 消息内容\r\n");
        CSStringBuilder.AppendParams("---@return ");
        CSStringBuilder.AppendParams((msgInProto != null) ? "table\r\n" : "nil\r\n");
        CSStringBuilder.AppendParams("function networkDeserialize.On");
        CSStringBuilder.AppendParams(msg.messageName);
        CSStringBuilder.AppendParams("Received(msgID, buffer)\r\n");
        if (msgInProto != null)
        {
            CSStringBuilder.AppendParams("    if buffer == nil then\r\n        CS.OnlineDebug.LogError(\"Lua解析消息: 待解析内容为空\\r\\nID: ");
            CSStringBuilder.AppendParams(msg.messageID);
            CSStringBuilder.AppendParams(' ');
            CSStringBuilder.AppendParams(ps.Package);
            CSStringBuilder.AppendParams('.');
            CSStringBuilder.AppendParams(msgInProto.MessageName);
            CSStringBuilder.AppendParams(' ');
            CSStringBuilder.AppendParams(msg.messageDescription);
            CSStringBuilder.AppendParams("\")\r\n        return nil\r\n    end\r\n");

            CSStringBuilder.AppendParams("    local res = protobufMgr.Deserialize(\"");
            CSStringBuilder.AppendParams(ps.Package);
            CSStringBuilder.AppendParams('.');
            CSStringBuilder.AppendParams(msgInProto.MessageName);
            CSStringBuilder.AppendParams("\", buffer)\r\n");
            CSStringBuilder.AppendParams("    if protoAdjust.");
            CSStringBuilder.AppendParams(ps.ProtoFileName);
            CSStringBuilder.AppendParams("_adj ~= nil and protoAdjust.");
            CSStringBuilder.AppendParams(ps.ProtoFileName);
            CSStringBuilder.AppendParams("_adj.Adjust");
            CSStringBuilder.AppendParams(msgInProto.MessageName);
            CSStringBuilder.AppendParams(" ~= nil then\r\n        protoAdjust.");
            CSStringBuilder.AppendParams(ps.ProtoFileName);
            CSStringBuilder.AppendParams("_adj.Adjust");
            CSStringBuilder.AppendParams(msgInProto.MessageName);
            CSStringBuilder.AppendParams("(res)\r\n    end\r\n    return res\r\n");
        }
        else
        {
            CSStringBuilder.AppendParams("    return nil\r\n");
        }
        CSStringBuilder.AppendParams("end\r\n");
        return CSStringBuilder.ToStringParams();
    }
    #endregion

    #region 刷新预处理方法
    private readonly static string pattern_ProcessingPart = @"\r\n--region[\s\S]+?\r\nend\r\n--endregion\r\n";
    private readonly static string pattern_NetMsgPreprocessing = @"netMsgPreprocessing\[(?<id>\d+)\] = function\(msgID\,\s*tblData\,\s*csData\)";
    private readonly static string pattern_MethodDescription = @"--region\s+ID:\s*(?<id>\d+)\s+(?<msgName>\w*?)\s+(?<desc>[\s\S]*?)\r\n";
    private readonly static string pattern_PreprocessingMethodContent = @"function\(msgID\,\s*tblData\,\s*csData\)(?<content>[\s\S]+?)\r\nend\r\n--endregion";
    //已存在的预处理方法集合
    private Dictionary<long, PreprocessingMethod> existingPreprocessingMethodDic;

    /// <summary>
    /// 刷新预处理方法字符串
    /// </summary>
    private void RefreshPreprocessingNetMsgXLuaMethods()
    {
        ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "刷新消息预处理", 0);
        //得到路径
        CSStringBuilder.Clear();
        CSStringBuilder.AppendParams(Application.dataPath.Substring(0, Application.dataPath.Length - 7));
        CSStringBuilder.AppendParams("/luaRes/netMsgPreprocessing");
        string processingDirectory = CSStringBuilder.ToStringParams();
        char[] c;
        StringBuilder charSB = new StringBuilder();

        #region 读取已存在的预处理方法
        bool isReadingFatalError = false;
        List<string> preprocessingProtocolFilePath = new List<string>();
        existingPreprocessingMethodDic = new Dictionary<long, PreprocessingMethod>();
        preprocessingProtocolFilePath.Add(processingDirectory + "/NetMsgPreprocessing.lua");
        try
        {
            string[] filesInDir = Directory.GetFiles(processingDirectory, "*.lua", SearchOption.AllDirectories);
            if (filesInDir != null)
            {
                for (int i = 0; i < filesInDir.Length; i++)
                {
                    preprocessingProtocolFilePath.Add(filesInDir[i]);
                }
            }
        }
        catch (Exception) { }
        ServerTool_ProtocolControllerUtility.SetProgressRecord(0, 0.2f);
        for (int i = 0; i < preprocessingProtocolFilePath.Count; i++)
        {
            try
            {
                ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "读取现有预处理文件中 " + preprocessingProtocolFilePath[i], ((float)i / preprocessingProtocolFilePath.Count));
                if (File.Exists(preprocessingProtocolFilePath[i]))
                {
                    string content = File.ReadAllText(preprocessingProtocolFilePath[i], Encoding.UTF8);
                    MatchCollection matches = Regex.Matches(content, pattern_ProcessingPart);
                    foreach (Match methodMatch in matches)
                    {
                        PreprocessingMethod method = new PreprocessingMethod();
                        Match desMatch = Regex.Match(methodMatch.Value, pattern_MethodDescription);
                        if (desMatch != null && desMatch.Success && desMatch.Groups != null && desMatch.Groups.Count > 0)
                        {
                            if (desMatch.Groups["id"] != null && !string.IsNullOrEmpty(desMatch.Groups["id"].Value))
                            {
                                long.TryParse(desMatch.Groups["id"].Value, out method.messageIndex);
                            }
                            if (desMatch.Groups["msgName"] != null && !string.IsNullOrEmpty(desMatch.Groups["msgName"].Value))
                            {
                                method.messageName = desMatch.Groups["msgName"].Value;
                            }
                            if (desMatch.Groups["desc"] != null && !string.IsNullOrEmpty(desMatch.Groups["desc"].Value))
                            {
                                method.messageDescription = desMatch.Groups["desc"].Value;
                            }
                        }
                        Match contentMatch = Regex.Match(methodMatch.Value, pattern_PreprocessingMethodContent);
                        if (contentMatch != null && contentMatch.Success && contentMatch.Groups != null && contentMatch.Groups.Count > 0)
                        {
                            method.methodContent = contentMatch.Groups["content"].Value;
                        }
                        long id;
                        Match idMatch = Regex.Match(methodMatch.Value, pattern_NetMsgPreprocessing);
                        if (idMatch != null && idMatch.Success && idMatch.Groups != null && idMatch.Groups.Count > 0 && idMatch.Groups["id"] != null && long.TryParse(idMatch.Groups["id"].Value, out id))
                        {
                            if (method.messageIndex == id && id > 0)
                            {
                                existingPreprocessingMethodDic[id] = method;
                            }
                            else
                            {
                                isReadingFatalError = true;
                                UnityEngine.Debug.LogError(string.Format("{0}\r\nregion中的ID与lua代码中的ID不一致:region中的ID:\r\n{1}\r\nlua代码中的ID:{2}\r\n", preprocessingProtocolFilePath[i], method.messageIndex, id));
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
        }
        ServerTool_ProtocolControllerUtility.ClearProgressRecord();
        #endregion

        #region 为已存在的预处理方法区分所在的协议
        ServerTool_ProtocolControllerUtility.SetProgressRecord(0.2f, 0.2f);
        int indexTemp1 = 0;
        float countTemp1 = existingPreprocessingMethodDic.Count;
        foreach (var item in existingPreprocessingMethodDic)
        {
            long id = item.Key;
            bool getXML = false;
            ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "为读入的方法分类协议中 " + id, indexTemp1++ / countTemp1);
            for (int i = 0; i < Data.messages.Count; i++)
            {
                if (id == Data.messages[i].messageID)
                {
                    item.Value.messageXMLName = Data.messages[i].xmlName;
                    ProtocolControlCache_ProtoStructure proto = ServerProtocolControllerWnd.GetProtoStructure(Data.messages[i]);
                    if (Data.messages[i].hasProto && proto != null)
                    {
                        item.Value.messageType = proto.Package + "." + Data.messages[i].protoData.protoMsgName;
                    }
                    else
                    {
                        item.Value.messageType = string.Empty;
                    }
                    item.Value.messageDescription = Data.messages[i].messageDescription;
                    getXML = true;
                    break;
                }
            }
            if (!getXML)
            {
                item.Value.messageXMLName = string.Empty;
            }
        }
        ServerTool_ProtocolControllerUtility.ClearProgressRecord();
        #endregion

        #region 为各个lua中处理的消息生成预处理方法
        ServerTool_ProtocolControllerUtility.SetProgressRecord(0.4f, 0.2f);
        StringBuilder netMsgPreprocessingSB = new StringBuilder();
        netMsgPreprocessingSB.Append("--[[本文件为工具自动生成]]\r\n--[[本文件用于网络消息分发之前,根据网络消息进行全局预处理,可编辑区域为所生成的每个方法内部,对可编辑区域外的修改将在工具下次修改时作废]]\r\n--[[不建议在方法内使用--region和--endregion,以免干扰工具读取]]\r\n---网络消息预处理\r\nnetMsgPreprocessing = {}\r\n\r\nnetMsgPreprocessing.__index = netMsgPreprocessing\r\n");
        Dictionary<string, StringBuilder> preprocessingLuaScriptSBForXML = new Dictionary<string, StringBuilder>();
        Dictionary<long, bool> isPreprocessingEnabled = new Dictionary<long, bool>();
        foreach (var item in existingPreprocessingMethodDic)
        {
            isPreprocessingEnabled[item.Key] = false;
        }
        for (int i = 0; i < Data.messages.Count; i++)
        {
            ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "为lua中处理的消息生成预处理方法中 " + Data.messages[i].messageID, ((float)i) / Data.messages.Count);
            if (Data.messages[i].isMessageDealInXLua && Data.messages[i].messageDirection == ProtocolControlCache_Message.MessageDirection.ToClient)
            {
                if (!existingPreprocessingMethodDic.ContainsKey(Data.messages[i].messageID))
                {
                    existingPreprocessingMethodDic[Data.messages[i].messageID] = new PreprocessingMethod();
                    existingPreprocessingMethodDic[Data.messages[i].messageID].messageIndex = Data.messages[i].messageID;
                    existingPreprocessingMethodDic[Data.messages[i].messageID].messageName = Data.messages[i].messageName;
                    existingPreprocessingMethodDic[Data.messages[i].messageID].messageDescription = Data.messages[i].messageDescription;
                    ProtocolControlCache_ProtoStructure proto = ServerProtocolControllerWnd.GetProtoStructure(Data.messages[i]);
                    if (Data.messages[i].hasProto && proto != null)
                    {
                        existingPreprocessingMethodDic[Data.messages[i].messageID].messageType = proto.Package + "." + Data.messages[i].protoData.protoMsgName;
                    }
                    else
                    {
                        existingPreprocessingMethodDic[Data.messages[i].messageID].messageType = string.Empty;
                    }
                }
                isPreprocessingEnabled[Data.messages[i].messageID] = true;
                if (string.IsNullOrEmpty(Data.messages[i].xmlName))
                {
                    netMsgPreprocessingSB.Append(GeneratePreprocessingNetXLuaMethods(existingPreprocessingMethodDic[Data.messages[i].messageID], Data.messages[i]));
                }
                else
                {
                    if (!preprocessingLuaScriptSBForXML.ContainsKey(Data.messages[i].xmlName))
                    {
                        preprocessingLuaScriptSBForXML[Data.messages[i].xmlName] = new StringBuilder();
                        preprocessingLuaScriptSBForXML[Data.messages[i].xmlName].Append("--[[本文件为工具自动生成]]\r\n--[[本文件用于网络消息分发之前,根据网络消息进行全局预处理,可编辑区域为所生成的每个方法内部,对可编辑区域外的修改将在工具下次修改时作废]]\r\n--[[不建议在方法内使用--region和--endregion,以免干扰工具读取]]\r\n--");
                        preprocessingLuaScriptSBForXML[Data.messages[i].xmlName].Append(Data.messages[i].xmlName);
                        preprocessingLuaScriptSBForXML[Data.messages[i].xmlName].Append(".xml\r\n");
                    }
                    preprocessingLuaScriptSBForXML[Data.messages[i].xmlName].Append(GeneratePreprocessingNetXLuaMethods(existingPreprocessingMethodDic[Data.messages[i].messageID], Data.messages[i]));
                }
            }
        }
        ServerTool_ProtocolControllerUtility.ClearProgressRecord();
        #endregion

        #region 在文件中写入之前已被定义但是当前消息未被使能的预处理方法,并将其注释,以便后续使能消息时能继续使用该代码
        ServerTool_ProtocolControllerUtility.SetProgressRecord(0.6f, 0.2f);
        indexTemp1 = 0;
        countTemp1 = existingPreprocessingMethodDic.Count;
        foreach (var item in existingPreprocessingMethodDic)
        {
            ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "写入被注释了的预处理方法 " + item.Key, indexTemp1 / countTemp1);
            if (!isPreprocessingEnabled[item.Key])
            {
                if (string.IsNullOrEmpty(item.Value.messageXMLName))
                {
                    netMsgPreprocessingSB.Append("\r\n--[[");
                    netMsgPreprocessingSB.Append(GeneratePreprocessingNetXLuaMethods(item.Value));
                    netMsgPreprocessingSB.Append("--]]\r\n");
                }
                else
                {
                    if (!preprocessingLuaScriptSBForXML.ContainsKey(item.Value.messageXMLName))
                    {
                        preprocessingLuaScriptSBForXML[item.Value.messageXMLName] = new StringBuilder();
                        preprocessingLuaScriptSBForXML[item.Value.messageXMLName].Append("--[[本文件为工具自动生成]]\r\n--[[本文件用于网络消息分发之前,根据网络消息进行全局预处理,可编辑区域为所生成的每个方法内部,对可编辑区域外的修改将在工具下次修改时作废]]\r\n--[[不建议在方法内使用--region和--endregion,以免干扰工具读取]]\r\n--");
                        preprocessingLuaScriptSBForXML[item.Value.messageXMLName].Append(item.Value.messageXMLName);
                        preprocessingLuaScriptSBForXML[item.Value.messageXMLName].Append(".xml\r\n");
                    }
                    preprocessingLuaScriptSBForXML[item.Value.messageXMLName].Append("\r\n--[[");
                    preprocessingLuaScriptSBForXML[item.Value.messageXMLName].Append(GeneratePreprocessingNetXLuaMethods(item.Value));
                    preprocessingLuaScriptSBForXML[item.Value.messageXMLName].Append("--]]\r\n");
                }
                isPreprocessingEnabled[item.Key] = true;
            }
        }
        ServerTool_ProtocolControllerUtility.ClearProgressRecord();
        #endregion

        #region 判断是否可以写入文件
        //若遇到致命错误,则不再继续,以免丢失数据
        if (isReadingFatalError)
        {
            return;
        }
        #endregion

        #region 写入文件
        ServerTool_ProtocolControllerUtility.SetProgressRecord(0.8f, 0.2f);
        indexTemp1 = 0;
        countTemp1 = preprocessingLuaScriptSBForXML.Count;
        CSStringBuilder.Clear();
        CSStringBuilder.AppendParams(processingDirectory);
        CSStringBuilder.AppendParams("/");
        CSStringBuilder.AppendParams("NetMsgPreprocessing.lua");
        string netMsgPreprocessingFilePath = CSStringBuilder.ToStringParams();
        if (!Directory.Exists(processingDirectory))
        {
            Directory.CreateDirectory(processingDirectory);
        }
        foreach (var item in preprocessingLuaScriptSBForXML)
        {
            ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "写入文件中 " + item.Key, indexTemp1 / countTemp1);
            c = item.Key.ToArray();
            c[0] = (char)((c[0] <= 'z' && c[0] >= 'a') ? (c[0] - 32) : c[0]);
            charSB.Remove(0, charSB.Length);
            if (c != null)
            {
                for (int i = 0; i < c.Length; i++)
                {
                    charSB.Append(c[i]);
                }
            }
            CSStringBuilder.Clear();
            CSStringBuilder.AppendParams(processingDirectory);
            CSStringBuilder.AppendParams("/NetMsgPreprocessing_");
            CSStringBuilder.AppendParams(charSB.ToString());
            CSStringBuilder.AppendParams(".lua");
            string filePath = CSStringBuilder.ToStringParams();
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.WriteAllText(filePath, item.Value.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(string.Format("网络消息预处理脚本刷新失败\r\n{0}\r\n{1}\r\n{2}", filePath, ex.Message, ex.StackTrace));
            }
            netMsgPreprocessingSB.Append("\r\n--");
            netMsgPreprocessingSB.Append(item.Key);
            netMsgPreprocessingSB.Append(".xml\r\nrequire \"luaRes.netMsgPreprocessing.NetMsgPreprocessing_");
            netMsgPreprocessingSB.Append(charSB.ToString());
            netMsgPreprocessingSB.Append("\"\r\n");
        }
        try
        {
            if (File.Exists(netMsgPreprocessingFilePath))
            {
                File.Delete(netMsgPreprocessingFilePath);
            }
            File.WriteAllText(netMsgPreprocessingFilePath, netMsgPreprocessingSB.ToString(), Encoding.UTF8);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log(string.Format("网络消息预处理核心脚本刷新失败\r\n{0}\r\n{1}\r\n{2}", netMsgPreprocessingFilePath, ex.Message, ex.StackTrace));
        }
        ServerTool_ProtocolControllerUtility.ClearProgressRecord();
        #endregion

        UnityEngine.Debug.Log("netMsgPreprocessing 网络消息预处理脚本刷新完毕");
    }

    /// <summary>
    /// 生成预处理方法代码
    /// </summary>
    /// <param name="method"></param>
    /// <param name="messageCache">方法对应的消息缓存,为null也可</param>
    /// <returns></returns>
    private string GeneratePreprocessingNetXLuaMethods(PreprocessingMethod method, ProtocolControlCache_Message messageCache = null)
    {
        if (method == null)
        {
            return string.Empty;
        }
        if (string.IsNullOrEmpty(method.methodContent))
        {
            method.methodContent = "\r\n    --在此处填入预处理代码";
        }
        CSStringBuilder.Clear();
        CSStringBuilder.AppendParams("\r\n--region ID:");
        CSStringBuilder.AppendParams(method.messageIndex);
        CSStringBuilder.AppendParams(" ");
        CSStringBuilder.AppendParams(method.messageName);
        CSStringBuilder.AppendParams(" ");
        CSStringBuilder.AppendParams(method.messageDescription);
        CSStringBuilder.AppendParams("\r\n---@param msgID LuaEnumNetDef 消息ID\r\n---@param tblData ");
        if (string.IsNullOrEmpty(method.messageType))
        {
            CSStringBuilder.AppendParams("table");
        }
        else
        {
            CSStringBuilder.AppendParams(method.messageType);
        }
        CSStringBuilder.AppendParams(" lua table类型消息数据\r\n---@param csData ");
        if (string.IsNullOrEmpty(method.messageType))
        {
            CSStringBuilder.AppendParams("userdata");
            CSStringBuilder.AppendParams(" C# class类型消息数据(nil)");
        }
        else
        {
            CSStringBuilder.AppendParams(method.messageType);
            CSStringBuilder.AppendParams(" C# class类型消息数据");
            if (messageCache != null && messageCache.messageDirection == ProtocolControlCache_Message.MessageDirection.ToClient && messageCache.isMessageDealInXLua && !messageCache.isNeedSendBacktoCSharp)
            {
                CSStringBuilder.AppendParams("(nil, 被优化掉了)");
            }
        }
        CSStringBuilder.AppendParams("\r\nnetMsgPreprocessing[");
        CSStringBuilder.AppendParams(method.messageIndex);
        CSStringBuilder.AppendParams("] = function(msgID, tblData, csData)");
        CSStringBuilder.AppendParams(method.methodContent);
        CSStringBuilder.AppendParams("\r\nend\r\n--endregion\r\n");
        return CSStringBuilder.ToStringParams();
    }
    #endregion

    #region 刷新预校验方法
    private readonly static string pattern_PreverifyingPart = @"\r\n--region[\s\S]+?\r\nend\r\n--endregion\r\n";
    private readonly static string pattern_NetMsgPreverifying = @"netMsgPreverifying\[(?<id>\d+)\] = function\(msgID\,\s*csData\)";
    private readonly static string pattern_PreverifyingMethodContent = @"function\(msgID\,\s*csData\)(?<content>[\s\S]+?)\r\nend\r\n--endregion";
    //已存在的预校验方法集合
    private Dictionary<long, PreverifyingMethod> existingPreverifyingMethodDic;

    /// <summary>
    /// 刷新预校验方法字符串
    /// </summary>
    private void RefreshPreverifyingNetMsgXLuaMethod()
    {
        ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "刷新消息预校验", 0);
        //得到路径
        CSStringBuilder.Clear();
        CSStringBuilder.AppendParams(Application.dataPath.Substring(0, Application.dataPath.Length - 7));
        CSStringBuilder.AppendParams("/luaRes/netMsgPreverifying");
        string preverifyingDirectory = CSStringBuilder.ToStringParams();
        char[] c;
        StringBuilder charSB = new StringBuilder();

        #region 读取已存在的预校验方法
        ServerTool_ProtocolControllerUtility.SetProgressRecord(0, 0.2f);
        bool isReadingFatalError = false;
        List<string> preverifyingProtocolFilePath = new List<string>();
        existingPreverifyingMethodDic = new Dictionary<long, PreverifyingMethod>();
        preverifyingProtocolFilePath.Add(preverifyingDirectory + "/NetMsgPreverifying.lua");
        try
        {
            string[] filesInDir = Directory.GetFiles(preverifyingDirectory, "*.lua", SearchOption.AllDirectories);
            if (filesInDir != null)
            {
                for (int i = 0; i < filesInDir.Length; i++)
                {
                    preverifyingProtocolFilePath.Add(filesInDir[i]);
                }
            }
        }
        catch (Exception) { }
        for (int i = 0; i < preverifyingProtocolFilePath.Count; i++)
        {
            try
            {
                ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "读取已存在的预校验方法", ((float)i) / preverifyingProtocolFilePath.Count);
                if (File.Exists(preverifyingProtocolFilePath[i]))
                {
                    string content = File.ReadAllText(preverifyingProtocolFilePath[i], Encoding.UTF8);
                    MatchCollection matches = Regex.Matches(content, pattern_PreverifyingPart);
                    foreach (Match methodMatch in matches)
                    {
                        PreverifyingMethod method = new PreverifyingMethod();
                        Match desMatch = Regex.Match(methodMatch.Value, pattern_MethodDescription);
                        if (desMatch != null && desMatch.Success && desMatch.Groups != null && desMatch.Groups.Count > 0)
                        {
                            if (desMatch.Groups["id"] != null && !string.IsNullOrEmpty(desMatch.Groups["id"].Value))
                            {
                                long.TryParse(desMatch.Groups["id"].Value, out method.messageIndex);
                            }
                            if (desMatch.Groups["msgName"] != null && !string.IsNullOrEmpty(desMatch.Groups["msgName"].Value))
                            {
                                method.messageName = desMatch.Groups["msgName"].Value;
                            }
                            if (desMatch.Groups["desc"] != null && !string.IsNullOrEmpty(desMatch.Groups["desc"].Value))
                            {
                                method.messageDescription = desMatch.Groups["desc"].Value;
                            }
                        }
                        Match contentMatch = Regex.Match(methodMatch.Value, pattern_PreverifyingMethodContent);
                        if (contentMatch != null && contentMatch.Success && contentMatch.Groups != null && contentMatch.Groups.Count > 0)
                        {
                            method.methodContent = contentMatch.Groups["content"].Value;
                        }
                        long id;
                        Match idMatch = Regex.Match(methodMatch.Value, pattern_NetMsgPreverifying);
                        if (idMatch != null && idMatch.Success && idMatch.Groups != null && idMatch.Groups.Count > 0 && idMatch.Groups["id"] != null && long.TryParse(idMatch.Groups["id"].Value, out id))
                        {
                            if (method.messageIndex == id && id > 0)
                            {
                                existingPreverifyingMethodDic[id] = method;
                            }
                            else
                            {
                                isReadingFatalError = true;
                                UnityEngine.Debug.LogError(string.Format("{0}\r\nregion中的ID与lua代码中的ID不一致:region中的ID:\r\n{1}\r\nlua代码中的ID:{2}\r\n", preverifyingProtocolFilePath[i], method.messageIndex, id));
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
        }
        ServerTool_ProtocolControllerUtility.ClearProgressRecord();
        #endregion

        #region 为已存在的预校验方法区分所在的协议
        ServerTool_ProtocolControllerUtility.SetProgressRecord(0.2f, 0.2f);
        int indexTemp1 = 0;
        float summaryTemp = existingPreprocessingMethodDic.Count;
        foreach (var item in existingPreverifyingMethodDic)
        {
            ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "为读取到的预校验方法区分所在的协议", indexTemp1 / summaryTemp);
            long id = item.Key;
            bool getXML = false;
            for (int i = 0; i < Data.messages.Count; i++)
            {
                if (id == Data.messages[i].messageID)
                {
                    item.Value.messageXMLName = Data.messages[i].xmlName;
                    ProtocolControlCache_ProtoStructure proto = ServerProtocolControllerWnd.GetProtoStructure(Data.messages[i]);
                    if (Data.messages[i].hasProto && proto != null)
                    {
                        item.Value.messageType = proto.Package + "." + Data.messages[i].protoData.protoMsgName;
                    }
                    else
                    {
                        item.Value.messageType = string.Empty;
                    }
                    item.Value.messageDescription = Data.messages[i].messageDescription;
                    getXML = true;
                    break;
                }
            }
            if (!getXML)
            {
                item.Value.messageXMLName = string.Empty;
            }
        }
        ServerTool_ProtocolControllerUtility.ClearProgressRecord();
        #endregion

        #region 为各个lua中校验的消息生成预校验方法
        ServerTool_ProtocolControllerUtility.SetProgressRecord(0.4f, 0.2f);
        StringBuilder netMsgPreverifyingSB = new StringBuilder();
        netMsgPreverifyingSB.Append("--[[本文件为工具自动生成]]\r\n--[[本文件用于向服务器发送消息前,对发送的消息进行预校验,返回的bool值决定该消息是否应当发送,可编辑区域为所生成的每个方法内部,对可编辑区域外的修改将在工具下次修改时作废]]\r\n--[[不建议在方法内使用--region和--endregion,以免干扰工具读取]]\r\n---网络消息预校验\r\nnetMsgPreverifying = {}\r\n\r\nnetMsgPreverifying.__index = netMsgPreverifying\r\n");
        Dictionary<string, StringBuilder> preverifyingLuaScriptSBForXML = new Dictionary<string, StringBuilder>();
        Dictionary<long, bool> isPreverifyingEnabled = new Dictionary<long, bool>();
        foreach (var item in existingPreverifyingMethodDic)
        {
            isPreverifyingEnabled[item.Key] = false;
        }
        for (int i = 0; i < Data.messages.Count; i++)
        {
            ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "为消息生成预校验方法中 " + Data.messages[i].messageName, ((float)i) / Data.messages.Count);
            if (Data.messages[i].isMessageDealInXLua && Data.messages[i].messageDirection == ProtocolControlCache_Message.MessageDirection.ToServer)
            {
                if (!existingPreverifyingMethodDic.ContainsKey(Data.messages[i].messageID))
                {
                    existingPreverifyingMethodDic[Data.messages[i].messageID] = new PreverifyingMethod();
                    existingPreverifyingMethodDic[Data.messages[i].messageID].messageIndex = Data.messages[i].messageID;
                    existingPreverifyingMethodDic[Data.messages[i].messageID].messageName = Data.messages[i].messageName;
                    existingPreverifyingMethodDic[Data.messages[i].messageID].messageDescription = Data.messages[i].messageDescription;
                    existingPreverifyingMethodDic[Data.messages[i].messageID].isNeedSendBackToCSharp = Data.messages[i].isNeedSendBacktoCSharp;
                    ProtocolControlCache_ProtoStructure proto = ServerProtocolControllerWnd.GetProtoStructure(Data.messages[i]);
                    if (Data.messages[i].hasProto && proto != null)
                    {
                        existingPreverifyingMethodDic[Data.messages[i].messageID].messageType = proto.Package + "." + Data.messages[i].protoData.protoMsgName;
                    }
                    else
                    {
                        existingPreverifyingMethodDic[Data.messages[i].messageID].messageType = string.Empty;
                    }
                }
                isPreverifyingEnabled[Data.messages[i].messageID] = true;
                if (string.IsNullOrEmpty(Data.messages[i].xmlName))
                {
                    netMsgPreverifyingSB.Append(GeneratePreverifyingNetXLuaMethods(existingPreverifyingMethodDic[Data.messages[i].messageID]));
                }
                else
                {
                    if (!preverifyingLuaScriptSBForXML.ContainsKey(Data.messages[i].xmlName))
                    {
                        preverifyingLuaScriptSBForXML[Data.messages[i].xmlName] = new StringBuilder();
                        preverifyingLuaScriptSBForXML[Data.messages[i].xmlName].Append("--[[本文件为工具自动生成]]\r\n--[[本文件用于向服务器发送消息前,对发送的消息进行预校验,返回的bool值决定该消息是否应当发送,可编辑区域为所生成的每个方法内部,对可编辑区域外的修改将在工具下次修改时作废]]\r\n--[[不建议在方法内使用--region和--endregion,以免干扰工具读取]]\r\n--");
                        preverifyingLuaScriptSBForXML[Data.messages[i].xmlName].Append(Data.messages[i].xmlName);
                        preverifyingLuaScriptSBForXML[Data.messages[i].xmlName].Append(".xml\r\n");
                    }
                    preverifyingLuaScriptSBForXML[Data.messages[i].xmlName].Append(GeneratePreverifyingNetXLuaMethods(existingPreverifyingMethodDic[Data.messages[i].messageID]));
                }
            }
        }
        ServerTool_ProtocolControllerUtility.ClearProgressRecord();
        #endregion

        #region 在文件中写入之前已被定义但是当前消息未被使能的预校验方法,并将其注释,以便后续使能消息时能继续使用该代码
        ServerTool_ProtocolControllerUtility.SetProgressRecord(0.6f, 0.2f);
        indexTemp1 = 0;
        summaryTemp = existingPreprocessingMethodDic.Count;
        foreach (var item in existingPreverifyingMethodDic)
        {
            ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "写入已被注释的预校验方法 " + item.Key, indexTemp1 / summaryTemp);
            if (!isPreverifyingEnabled[item.Key])
            {
                if (string.IsNullOrEmpty(item.Value.messageXMLName))
                {
                    netMsgPreverifyingSB.Append("\r\n--[[");
                    netMsgPreverifyingSB.Append(GeneratePreverifyingNetXLuaMethods(item.Value));
                    netMsgPreverifyingSB.Append("--]]\r\n");
                }
                else
                {
                    if (!preverifyingLuaScriptSBForXML.ContainsKey(item.Value.messageXMLName))
                    {
                        preverifyingLuaScriptSBForXML[item.Value.messageXMLName] = new StringBuilder();
                        preverifyingLuaScriptSBForXML[item.Value.messageXMLName].Append("--[[本文件为工具自动生成]]\r\n--[[本文件用于向服务器发送消息前,对发送的消息进行预校验,返回的bool值决定该消息是否应当发送,可编辑区域为所生成的每个方法内部,对可编辑区域外的修改将在工具下次修改时作废]]\r\n--[[不建议在方法内使用--region和--endregion,以免干扰工具读取]]\r\n--");
                        preverifyingLuaScriptSBForXML[item.Value.messageXMLName].Append(item.Value.messageXMLName);
                        preverifyingLuaScriptSBForXML[item.Value.messageXMLName].Append(".xml\r\n");
                    }
                    preverifyingLuaScriptSBForXML[item.Value.messageXMLName].Append("\r\n--[[");
                    preverifyingLuaScriptSBForXML[item.Value.messageXMLName].Append(GeneratePreverifyingNetXLuaMethods(item.Value));
                    preverifyingLuaScriptSBForXML[item.Value.messageXMLName].Append("--]]\r\n");
                }
                isPreverifyingEnabled[item.Key] = true;
            }
        }
        ServerTool_ProtocolControllerUtility.ClearProgressRecord();
        #endregion

        #region 判断是否可以写入文件
        //若遇到致命错误,则不再继续,以免丢失数据
        if (isReadingFatalError)
        {
            return;
        }
        #endregion

        #region 写入文件
        ServerTool_ProtocolControllerUtility.SetProgressRecord(0.8f, 0.2f);
        CSStringBuilder.Clear();
        CSStringBuilder.AppendParams(preverifyingDirectory);
        CSStringBuilder.AppendParams("/");
        CSStringBuilder.AppendParams("NetMsgPreverifying.lua");
        string netMsgPreverifyingFilePath = CSStringBuilder.ToStringParams();
        if (!Directory.Exists(preverifyingDirectory))
        {
            Directory.CreateDirectory(preverifyingDirectory);
        }
        indexTemp1 = 0;
        summaryTemp = preverifyingLuaScriptSBForXML.Count;
        foreach (var item in preverifyingLuaScriptSBForXML)
        {
            ServerTool_ProtocolControllerUtility.DisplayProgress("刷新网络消息相关方法", "写入文件中 " + item.Key, indexTemp1 / summaryTemp);
            c = item.Key.ToArray();
            c[0] = (char)((c[0] <= 'z' && c[0] >= 'a') ? (c[0] - 32) : c[0]);
            charSB.Remove(0, charSB.Length);
            if (c != null)
            {
                for (int i = 0; i < c.Length; i++)
                {
                    charSB.Append(c[i]);
                }
            }
            CSStringBuilder.Clear();
            CSStringBuilder.AppendParams(preverifyingDirectory);
            CSStringBuilder.AppendParams("/NetMsgPreverifying_");
            CSStringBuilder.AppendParams(charSB.ToString());
            CSStringBuilder.AppendParams(".lua");
            string filePath = CSStringBuilder.ToStringParams();
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.WriteAllText(filePath, item.Value.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(string.Format("网络消息预校验脚本刷新失败\r\n{0}\r\n{1}\r\n{2}", filePath, ex.Message, ex.StackTrace));
            }
            netMsgPreverifyingSB.Append("\r\n--");
            netMsgPreverifyingSB.Append(item.Key);
            netMsgPreverifyingSB.Append(".xml\r\nrequire \"luaRes.netMsgPreverifying.NetMsgPreverifying_");
            netMsgPreverifyingSB.Append(charSB.ToString());
            netMsgPreverifyingSB.Append("\"\r\n");
        }
        try
        {
            if (File.Exists(netMsgPreverifyingFilePath))
            {
                File.Delete(netMsgPreverifyingFilePath);
            }
            File.WriteAllText(netMsgPreverifyingFilePath, netMsgPreverifyingSB.ToString(), Encoding.UTF8);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log(string.Format("网络消息预校验核心脚本刷新失败\r\n{0}\r\n{1}\r\n{2}", netMsgPreverifyingFilePath, ex.Message, ex.StackTrace));
        }
        ServerTool_ProtocolControllerUtility.ClearProgressRecord();
        #endregion

        UnityEngine.Debug.Log("netMsgPreprocessing 网络消息预校验脚本刷新完毕");
    }

    /// <summary>
    /// 生成预校验方法代码
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    private string GeneratePreverifyingNetXLuaMethods(PreverifyingMethod method)
    {
        if (method == null)
        {
            return string.Empty;
        }
        if (string.IsNullOrEmpty(method.methodContent))
        {
            method.methodContent = "\r\n    --在此处填入预校验代码\r\n    return true";
        }
        CSStringBuilder.Clear();
        CSStringBuilder.AppendParams("\r\n--region ID:");
        CSStringBuilder.AppendParams(method.messageIndex);
        CSStringBuilder.AppendParams(" ");
        CSStringBuilder.AppendParams(method.messageName);
        CSStringBuilder.AppendParams(" ");
        CSStringBuilder.AppendParams(method.messageDescription);
        CSStringBuilder.AppendParams("\r\n---@param msgID LuaEnumNetDef 消息ID\r\n---@param csData ");
        if (string.IsNullOrEmpty(method.messageType))
        {
            CSStringBuilder.AppendParams("userdata");
            CSStringBuilder.AppendParams(" C# class类型消息数据(nil)");
        }
        else
        {
            CSStringBuilder.AppendParams(method.messageType);
            if (method.isNeedSendBackToCSharp)
            {
                CSStringBuilder.AppendParams(" C#类型消息数据");
            }
            else
            {
                CSStringBuilder.AppendParams(" lua类型消息数据");
            }
        }
        CSStringBuilder.AppendParams("\r\n---@return boolean 是否允许发送消息\r\nnetMsgPreverifying[");
        CSStringBuilder.AppendParams(method.messageIndex);
        CSStringBuilder.AppendParams("] = function(msgID, csData)");
        CSStringBuilder.AppendParams(method.methodContent);
        CSStringBuilder.AppendParams("\r\nend\r\n--endregion\r\n");
        return CSStringBuilder.ToStringParams();
    }
    #endregion

    #region 工具
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
    /// 获取proto中变量类型对应的字符串
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    private string GetStringOfVariableTypeInProto(ProtocolControlCache_ProtoStructure proto, ProtocolControlCache_ProtoStructure.VariableInMessage variable)
    {
        switch (variable.VType)
        {
            case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Bool: return "boolean";
            case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Double:
            case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Float:
            case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Int32:
            case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.UInt32:
            case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Int64:
            case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.UInt64:
            case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.SInt32:
            case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.SInt64:
            case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Sing64:
            case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Fixed32:
            case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Fixed64:
            case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.SFixed32:
            case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.SFixed64: return "number";
            case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.String: return "string";
            case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Bytes: return "byte[]";
            case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Others:
            default: return proto.Package + '.' + variable.VariableTypeString;
        }
    }

    /// <summary>
    /// 设定无引用类型字段的Req消息不再使用C#数据结构
    /// </summary>
    /// <param name="messages"></param>
    private void SetNonReferFieldReqMessageDoNotUseCSStructure(List<ProtocolControlCache_Message> messages)
    {
        if (messages == null)
        {
            return;
        }
        EditorUtility.DisplayProgressBar("一键设定无引用类型字段的Req消息不再使用C#数据结构中", string.Empty, 0);
        for (int i = 0; i < messages.Count; i++)
        {
            var messageTemp = messages[i];
            EditorUtility.DisplayProgressBar("一键设定无引用类型字段的Req消息不再使用C#数据结构中", messageTemp.xmlName + ".xml  " + messageTemp.messageName, ((float)i / messages.Count));
            if (messageTemp.messageDirection == ProtocolControlCache_Message.MessageDirection.ToServer && messageTemp.hasProto && messageTemp.protoData != null)
            {
                var protoStructure = ServerProtocolControllerWnd.GetProtoStructure(messageTemp);
                if (protoStructure != null)
                {
                    ProtocolControlCache_ProtoStructure.Message messageStructure = null;
                    for (int j = 0; j < protoStructure.Messages.Count; j++)
                    {
                        if (protoStructure.Messages[j].MessageName == messageTemp.protoData.protoMsgName)
                        {
                            messageStructure = protoStructure.Messages[j];
                            break;
                        }
                    }
                    if (messageStructure != null)
                    {
                        bool isReferTypeExist = false;
                        for (int j = 0; j < messageStructure.Variables.Length; j++)
                        {
                            if (messageStructure.Variables[j].VType == ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Others
                                || messageStructure.Variables[j].Modifier == ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.NULL
                                || messageStructure.Variables[j].Modifier == ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Repeated)
                            {
                                isReferTypeExist = true;
                                break;
                            }
                        }
                        if (!isReferTypeExist)
                        {
                            messageTemp.isNeedSendBacktoCSharp = false;
                        }
                    }
                }
            }
        }
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// 导出不需要适配代码的消息类型
    /// </summary>
    private void ExportWrapNotNecessaryMsgType(List<ProtocolControlCache_Message> messages)
    {
        if (messages == null)
        {
            return;
        }
        Dictionary<string, bool> protoNeedCSharpWrap = new Dictionary<string, bool>();
        EditorUtility.DisplayProgressBar("正在查询消息对应的C#类是否需要生成Wrap代码", string.Empty, 0);
        for (int i = 0; i < messages.Count; i++)
        {
            var msgTemp = messages[i];
            EditorUtility.DisplayProgressBar("正在查询消息对应的C#类是否需要生成Wrap代码", msgTemp.xmlName + ".xml " + msgTemp.messageName, i / ((float)messages.Count));
            if (msgTemp.hasProto //只考虑有proto结构的消息
                && msgTemp.protoData != null &&//只考虑proto结构正常的消息
                ((msgTemp.isMessageDealInXLua == true && msgTemp.messageDirection == ProtocolControlCache_Message.MessageDirection.ToClient) ||//发往客户端的消息,只考虑在xlua中处理的消息
                (msgTemp.messageDirection == ProtocolControlCache_Message.MessageDirection.ToServer)))//发往服务器的消息,只考虑是否使用了proto数据结构
            {
                var protoStructure = ServerProtocolControllerWnd.GetProtoStructure(msgTemp);
                if (protoStructure == null)
                {
                    continue;
                }
                string protoName = protoStructure.Package + "." + msgTemp.protoData.protoMsgName;
                bool currentState = msgTemp.isNeedSendBacktoCSharp;
                bool previousState;
                if (protoNeedCSharpWrap.TryGetValue(protoName, out previousState))
                {
                    //若某个proto被任意一个message认为需要返回到C#,那么它就需要在C#中生成适配代码
                    if (!previousState)
                    {
                        protoNeedCSharpWrap[protoName] = currentState;
                    }
                }
                else
                {
                    protoNeedCSharpWrap[protoName] = currentState;
                }
            }
        }
        EditorUtility.ClearProgressBar();
        CSStringBuilder.Clear();
        CSStringBuilder.Append("//C# protobuf类的Wrap黑名单\r\n");
        foreach (var item in protoNeedCSharpWrap)
        {
            if (item.Value == false)
            {
                CSStringBuilder.Append(item.Key);
                CSStringBuilder.Append("\r\n");
            }
        }
        string filePath = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + "ProjectCaches/ProtobufCSWrapBlackList.txt";
        File.WriteAllText(filePath, CSStringBuilder.ToString());
        UnityEngine.Debug.LogFormat("保存protobuf类的Wrap黑名单文件到 {0}", filePath);
    }

    /// <summary>
    /// 检查Lua中请求方法的格式
    /// </summary>
    /// <param name="messages"></param>
    private void CheckRequestMethodInLua(List<ProtocolControlCache_Message> messages)
    {
        if (messages == null)
        {
            return;
        }
        //获取所有的方法请求结构
        List<MessageRequestStructure> messageRequests = new List<MessageRequestStructure>();
        for (int i = 0; i < messages.Count; i++)
        {
            var messageCache = messages[i];
            if (messageCache.messageDirection == ProtocolControlCache_Message.MessageDirection.ToServer &&
                messageCache.hasProto)
            {
                var messageProtoStructure = ServerProtocolControllerWnd.GetMessageStructure(messageCache);
                if (messageProtoStructure != null)
                {
                    MessageRequestStructure requestStructure = new MessageRequestStructure();
                    requestStructure.requestMethod = "networkRequest." + messageCache.messageName.Replace("Message", string.Empty);
                    int minParamsCount = 0;
                    for (int j = 0; j < messageProtoStructure.Variables.Length; j++)
                    {
                        var variableTemp = messageProtoStructure.Variables[j];
                        if (variableTemp.Modifier == ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Repeated || variableTemp.Modifier == ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Required)
                        {
                            //仅repeated类型字段和required类型字段需要
                            minParamsCount++;
                        }
                    }
                    requestStructure.minParamsCount = minParamsCount;
                    messageRequests.Add(requestStructure);
                }
            }
        }
        //遍历所有的Lua脚本,找到调用请求方法的地方并检查是否参数数量是否正常
        string luaResDirPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + "luaRes";
        string[] luaScriptPathes = Directory.GetFiles(luaResDirPath, "*.lua", SearchOption.AllDirectories);
        if (luaScriptPathes == null || messageRequests.Count == 0)
        {
            return;
        }
        EditorUtility.DisplayProgressBar("读取Lua文件中...", string.Empty, 0);
        try
        {
            ServerTool_ProtocolControllerUtility.ResetProgress();
            float stepLength = 1f / luaScriptPathes.Length;
            for (int i = 0; i < luaScriptPathes.Length; i++)
            {
                string scriptPathTemp = luaScriptPathes[i];
                string fileRelativePath = scriptPathTemp.Substring(Application.dataPath.Length - 6);
                //读取lua文件
                ServerTool_ProtocolControllerUtility.SetProgressRecord(i * stepLength, stepLength);
                ServerTool_ProtocolControllerUtility.DisplayProgress(fileRelativePath, string.Empty, 0);
                string content = File.ReadAllText(scriptPathTemp);
                for (int j = 0; j < messageRequests.Count; j++)
                {
                    MessageRequestStructure request = messageRequests[j];
                    ServerTool_ProtocolControllerUtility.DisplayProgress(fileRelativePath, "匹配中... " + request.requestMethod, j / ((float)messageRequests.Count));
                    CheckRequestMethodCalledProperInContent(request, content, scriptPathTemp);
                }
                ServerTool_ProtocolControllerUtility.ClearProgressRecord();
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    /// <summary>
    /// 判断请求方法在字符串中是否正常调用
    /// </summary>
    /// <param name="request"></param>
    /// <param name="content"></param>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private void CheckRequestMethodCalledProperInContent(MessageRequestStructure request, string content, string filePath)
    {
        MatchCollection requestMatchCollection = Regex.Matches(content, request.requestMethod);
        if (requestMatchCollection != null)
        {
            foreach (var item in requestMatchCollection)
            {
                var match = item as Match;
                int startIndex = match.Index;
                if (!match.Success)
                {
                    continue;
                }
                int bracketCount = 0;
                int paramsCount = 0;
                bool isParamCounted = false;
                int middleBracketCount = 0;
                bool isBracketChecked = false;
                for (int k = startIndex; k < content.Length; k++)
                {
                    char c = content[k];
                    if (c == '(')
                    {
                        bracketCount++;
                        isBracketChecked = true;
                    }
                    else if (c == ')')
                    {
                        bracketCount--;
                    }
                    else if (c == '[')
                    {
                        middleBracketCount++;
                    }
                    else if (c == ']')
                    {
                        middleBracketCount--;
                    }
                    else if (c == ',')
                    {
                        if (bracketCount == 1 && middleBracketCount == 0)
                        {
                            isParamCounted = false;
                        }
                    }
                    else if (c == ' ' || c == '\u3000')
                    {
                        //空格,不管
                    }
                    else if (c == '\r' || c == '\n')
                    {
                        //换行,不管
                    }
                    else
                    {
                        if (isParamCounted == false)
                        {
                            isParamCounted = true;
                            paramsCount++;
                        }
                    }
                    if (isBracketChecked && bracketCount == 0)
                    {
                        break;
                    }
                }
                if (paramsCount < request.minParamsCount)
                {
                    //若参数数量小于请求方法的最小参数数量,则认为该处的方法未匹配到正确的参数
                    int lineCountInMatchStartIndex = 1;
                    for (int i = 0; i < startIndex; i++)
                    {
                        if (content.Length <= i)
                        {
                            break;
                        }
                        if (content[i] == '\n')
                        {
                            lineCountInMatchStartIndex++;
                        }
                    }
                    UnityEngine.Debug.LogFormat("{0} 方法在 {1} 的第{2}行未匹配到正确参数", request.requestMethod, filePath, lineCountInMatchStartIndex);
                }
            }
        }
    }

    class MessageRequestStructure
    {
        /// <summary>
        /// 请求方法名
        /// </summary>
        public string requestMethod;
        /// <summary>
        /// 最少参数数量
        /// </summary>
        public int minParamsCount;
    }
    #endregion

    /// <summary>
    /// 预处理方法
    /// </summary>
    class PreprocessingMethod
    {
        /// <summary>
        /// 方法对应的消息ID
        /// </summary>
        public long messageIndex = 0;

        /// <summary>
        /// 方法内容
        /// </summary>
        public string methodContent = string.Empty;

        /// <summary>
        /// 消息名
        /// </summary>
        public string messageName = string.Empty;

        /// <summary>
        /// 消息描述
        /// </summary>
        public string messageDescription = string.Empty;

        /// <summary>
        /// 消息所在xml名字
        /// </summary>
        public string messageXMLName = string.Empty;

        /// <summary>
        /// 消息类型
        /// </summary>
        public string messageType = string.Empty;
    }

    /// <summary>
    /// 预校验方法
    /// </summary>
    class PreverifyingMethod
    {
        /// <summary>
        /// 方法对应的消息ID
        /// </summary>
        public long messageIndex = 0;

        /// <summary>
        /// 方法内容
        /// </summary>
        public string methodContent = string.Empty;

        /// <summary>
        /// 消息名
        /// </summary>
        public string messageName = string.Empty;

        /// <summary>
        /// 消息描述
        /// </summary>
        public string messageDescription = string.Empty;

        /// <summary>
        /// 消息所在xml名字
        /// </summary>
        public string messageXMLName = string.Empty;

        /// <summary>
        /// 消息类型
        /// </summary>
        public string messageType = string.Empty;

        /// <summary>
        /// 是否需要会回写到C#
        /// </summary>
        public bool isNeedSendBackToCSharp = false;
    }
}
