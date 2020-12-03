using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEditor;

public class ServerTool_ProtocolControllerUtility : Editor
{
    #region n阶进度条
    //key: 每一段的起始相对位置;  value: 本段的长度
    private static List<KeyValuePair<float, float>> progressCache = new List<KeyValuePair<float, float>>();

    public static void ResetProgress()
    {
        progressCache.Clear();
    }

    public static void ClearProgressRecord()
    {
        if (progressCache.Count > 0)
        {
            progressCache.RemoveAt(progressCache.Count - 1);
        }
    }

    /// <summary>
    /// 设定当前的进度记录
    /// </summary>
    /// <param name="currentProgress">当前进度</param>
    /// <param name="recordLength">当前进度相对于上一级进度的百分比</param>
    public static void SetProgressRecord(float currentProgress, float recordLength)
    {
        progressCache.Add(new KeyValuePair<float, float>(currentProgress, recordLength));
    }

    public static void DisplayProgress(string title, string info, float progress)
    {
        if (progressCache.Count > 0)
        {
            float baseProgress = 0;
            float length = 1;
            for (int i = 0; i < progressCache.Count; i++)
            {
                var temp = progressCache[i];
                baseProgress += length * temp.Key;
                length *= temp.Value;
            }
            progress = baseProgress + length * progress;
        }
        EditorUtility.DisplayProgressBar(title, info, progress);
    }
    #endregion

    #region 检查C#类型是否已加载
    private static System.Reflection.Assembly[] loadedAssemblies;
    /// <summary>
    /// 已加载的程序集
    /// </summary>
    private static System.Reflection.Assembly[] LoadedAssemblies
    {
        get
        {
            if (loadedAssemblies == null)
            {
                loadedAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            }
            return loadedAssemblies;
        }
    }

    /// <summary>
    /// 判断C#类型是否已加载
    /// </summary>
    /// <param name="packageName">包名</param>
    /// <param name="messageName">消息名</param>
    /// <returns></returns>
    public static bool IsCSTypeLoaded(string packageName, string messageName)
    {
        return IsCSTypeLoaded(string.Format("{0}.{1}", packageName, messageName), out Type temp);
    }

    /// <summary>
    /// 判断C#类型是否已加载
    /// </summary>
    /// <param name="cstype"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsCSTypeLoaded(string cstype, out System.Type type)
    {
        if (LoadedAssemblies == null)
        {
            type = null;
            return false;
        }
        for (int i = 0; i < LoadedAssemblies.Length; i++)
        {
            System.Reflection.Assembly assemblyTemp = loadedAssemblies[i];
            System.Type typeTemp = assemblyTemp.GetType(cstype);
            if (typeTemp != null)
            {
                type = typeTemp;
                return true;
            }
        }
        type = null;
        return false;
    }

    /// <summary>
    /// 判断C#类中是否存在某个字段
    /// </summary>
    /// <param name="packageName"></param>
    /// <param name="messageName"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public static bool IsFieldExistInCSType(string packageName, string messageName, string fieldName)
    {
        if (IsCSTypeLoaded(string.Format("{0}.{1}", packageName, messageName), out Type type))
        {
            if (type.GetProperty(fieldName) != null)
            {
                return true;
            }
            if (type.GetField(fieldName) != null)
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    /// <summary>
    /// 消息中是否有某个字段的类型不存在
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static bool IsAnyFieldNotExistInMessage(ProtocolControlCache_Message message)
    {
        if (!message.hasProto || message.protoData == null)
        {
            //未使用proto结构时返回false
            return false;
        }
        ProtocolControlCache_ProtoStructure.Message messageStructure = ServerProtocolControllerWnd.GetMessageStructure(message);
        return IsAnyFieldNotExistInMessageInternal(messageStructure, null);
    }

    /// <summary>
    /// 消息中是否有某个字段的类型不存在
    /// </summary>
    /// <param name="messageStructure"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    private static bool IsAnyFieldNotExistInMessageInternal(ProtocolControlCache_ProtoStructure.Message messageStructure, List<string> list = null)
    {
        if (messageStructure == null || messageStructure.Owner == null)
        {
            //消息不存在时返回false
            return false;
        }
        if (list == null)
        {
            list = new List<string>();
        }
        string msgTypeStr = messageStructure.Owner.Package + "." + messageStructure.MessageName;
        if (list.Contains(msgTypeStr))
        {
            //已经检查过了,防止递归死循环
            return false;
        }
        list.Add(msgTypeStr);
        for (int i = 0; i < messageStructure.Variables.Length; i++)
        {
            var variableTemp = messageStructure.Variables[i];
            if (!ServerTool_ProtocolControllerUtility.IsFieldExistInCSType(messageStructure.Owner.Package, messageStructure.MessageName, variableTemp.VariableName))
            {
                //若字段在C#类中不存在,则返回true
                return true;
            }
            if (variableTemp.VType == ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Others)
            {
                var variableType = ServerTool_ProtocolController_GenerateLuaFromProto.GetVariableTypeInOthers(messageStructure.Owner, messageStructure, variableTemp);
                if (variableType.isEnumType || variableType.messageType == null)
                {
                    continue;
                }
                if (!ServerTool_ProtocolControllerUtility.IsCSTypeLoaded(variableType.typeString, out Type typeTemp))
                {
                    //若字段使用的类型未在C#中找到,则返回true
                    return true;
                }
                if (IsAnyFieldNotExistInMessageInternal(variableType.messageType, list))
                {
                    //若字段使用的类型中也有字段未在C#中实现,则也返回true
                    return true;
                }
            }
        }
        return false;
    }
    
}