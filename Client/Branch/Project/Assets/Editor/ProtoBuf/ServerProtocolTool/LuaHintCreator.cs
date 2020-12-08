using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 创建Lua提示
/// </summary>
public class LuaHintCreator
{
    private static StringBuilder stringBuilderForLuaClassHint = new StringBuilder();

    private static string protoHintFolderPath;
    /// <summary>
    /// proto提示文件夹路径
    /// </summary>
    public static string ProtoHintFolderPath
    {
        get
        {
            if (string.IsNullOrEmpty(protoHintFolderPath))
            {
                protoHintFolderPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + "LuaReferences";
            }
            return protoHintFolderPath;
        }
    }

    /// <summary>
    /// 更新Lua提示文件
    /// </summary>
    /// <param name="protoFolderPath">proto文件夹路径</param>
    /// <param name="luaRefrenceFileName">lua引用文件名</param>
    /// <param name="isCommitToSVN">是否生成后提交svn</param>
    public static void UpdateLuaHintFile(string protoFolderPath, string luaRefrenceFileName, bool isCommitToSVN)
    {
        if (!Directory.Exists(protoFolderPath))
        {
            UnityEngine.Debug.LogErrorFormat("未找到 {0} 文件夹", protoFolderPath);
            return;
        }
        string luaRefrenceOutputPath = ProtoHintFolderPath;
        if (!Directory.Exists(luaRefrenceOutputPath))
        {
            Directory.CreateDirectory(luaRefrenceOutputPath);
        }
        string[] files = Directory.GetFiles(protoFolderPath);
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < files.Length; i++)
        {
            EditorUtility.DisplayProgressBar("解析proto结构", files[i], ((float)i) / files.Length);
            var protoStructure = ProtocolControlCache_ProtoStructure.GetProtoStructure(files[i]);
            EditorUtility.DisplayProgressBar("生成proto提示文件", files[i], ((float)i) / files.Length);
            for (int j = 0; j < protoStructure.Messages.Count; j++)
            {
                stringBuilder.Append(GenerateLuaClassHint(protoStructure, protoStructure.Messages[j]));
                stringBuilder.Append("\r\n\r\n");
            }
        }
        EditorUtility.ClearProgressBar();
        string outputFilePath = luaRefrenceOutputPath + "/" + luaRefrenceFileName;
        File.WriteAllText(outputFilePath, stringBuilder.ToString());
        UnityEngine.Debug.LogFormat("{0} 生成完毕,idea中可以引用该文件或者其所在文件夹", outputFilePath);
        if (isCommitToSVN)
        {
            //SVNEditorUtility.SVNCommit(luaRefrenceOutputPath);
        }
    }

    /// <summary>
    /// 生成lua类的提示
    /// </summary>
    /// <param name="proto"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    public static string GenerateLuaClassHint(ProtocolControlCache_ProtoStructure proto, ProtocolControlCache_ProtoStructure.Message msg)
    {
        if (msg == null)
        {
            return string.Empty;
        }
        stringBuilderForLuaClassHint.Remove(0, stringBuilderForLuaClassHint.Length);
        stringBuilderForLuaClassHint.Append("---@class ");
        stringBuilderForLuaClassHint.Append(proto.Package);
        stringBuilderForLuaClassHint.Append(".");
        stringBuilderForLuaClassHint.Append(msg.MessageName);
        stringBuilderForLuaClassHint.Append("\r\n");
        stringBuilderForLuaClassHint.Append("---class properties");
        stringBuilderForLuaClassHint.Append("\r\n");
        for (int i = 0; i < msg.Variables.Length; i++)
        {
            var variable = msg.Variables[i];
            string typeStr = null;
            switch (variable.VType)
            {
                case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Bool:
                    typeStr = "boolean";
                    break;
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
                case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.SFixed64:
                    typeStr = "number";
                    break;
                case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.String:
                case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Bytes:
                    typeStr = "string";
                    break;
                case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Others:
                    ServerTool_ProtocolController_GenerateLuaFromProto.VariableTypeInOthers variableTypeInOthers = ServerTool_ProtocolController_GenerateLuaFromProto.GetVariableTypeInOthers(proto, msg, variable);
                    if (variableTypeInOthers.isEnumType && variableTypeInOthers.enumType != null)
                    {
                        typeStr = "number";
                    }
                    else
                    {
                        typeStr = variableTypeInOthers.typeString;
                    }
                    break;
                default:
                    break;
            }
            switch (variable.Modifier)
            {
                case ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Required:
                    stringBuilderForLuaClassHint.Append("---@field public ");
                    stringBuilderForLuaClassHint.Append(variable.VariableName);
                    stringBuilderForLuaClassHint.Append(" ");
                    stringBuilderForLuaClassHint.Append(typeStr);
                    if (!string.IsNullOrEmpty(variable.DefaultValue))
                    {
                        stringBuilderForLuaClassHint.Append(" [default = ");
                        stringBuilderForLuaClassHint.Append(variable.DefaultValue);
                        stringBuilderForLuaClassHint.Append("]");
                    }
                    if (!string.IsNullOrEmpty(variable.Comment))
                    {
                        stringBuilderForLuaClassHint.Append(" ");
                        stringBuilderForLuaClassHint.Append(variable.Comment.Replace("\n", " ").Replace("\r", ""));
                    }
                    stringBuilderForLuaClassHint.Append("\r\n");
                    break;
                case ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Optional:
                    stringBuilderForLuaClassHint.Append("---@field public ");
                    stringBuilderForLuaClassHint.Append(variable.VariableName);
                    stringBuilderForLuaClassHint.Append(" ");
                    stringBuilderForLuaClassHint.Append(typeStr);
                    if (!string.IsNullOrEmpty(variable.DefaultValue))
                    {
                        stringBuilderForLuaClassHint.Append(" [default = ");
                        stringBuilderForLuaClassHint.Append(variable.DefaultValue);
                        stringBuilderForLuaClassHint.Append("]");
                    }
                    if (!string.IsNullOrEmpty(variable.Comment))
                    {
                        stringBuilderForLuaClassHint.Append(" ");
                        stringBuilderForLuaClassHint.Append(variable.Comment.Replace("\n", " ").Replace("\r", ""));
                    }
                    stringBuilderForLuaClassHint.Append("\r\n");
                    stringBuilderForLuaClassHint.Append("---@field public ");
                    stringBuilderForLuaClassHint.Append(variable.VariableName);
                    stringBuilderForLuaClassHint.Append("Specified boolean");
                    stringBuilderForLuaClassHint.Append("\r\n");
                    break;
                case ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Repeated:
                    stringBuilderForLuaClassHint.Append("---@field public ");
                    stringBuilderForLuaClassHint.Append(variable.VariableName);
                    stringBuilderForLuaClassHint.Append(" table<number, ");
                    stringBuilderForLuaClassHint.Append(typeStr);
                    stringBuilderForLuaClassHint.Append(">");
                    if (!string.IsNullOrEmpty(variable.Comment))
                    {
                        stringBuilderForLuaClassHint.Append(" ");
                        stringBuilderForLuaClassHint.Append(variable.Comment.Replace("\n", " ").Replace("\r", ""));
                    }
                    stringBuilderForLuaClassHint.Append("\r\n");
                    break;
                case ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.NULL:
                    break;
                default:
                    break;
            }
        }
        return stringBuilderForLuaClassHint.ToString();
    }
}
