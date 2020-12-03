using System.IO;
using System.Text;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

/// <summary>
/// 从proto生成对应的lua文件
/// </summary>
public class ServerTool_ProtocolController_GenerateLuaFromProto
{
    /// <summary>
    /// 导出转换文件路径
    /// </summary>
    private string decodeOutputFilePath;
    /// <summary>
    /// 导出调整文件路径
    /// </summary>
    private string adjustOutputFilePath;

    /// <summary>
    /// proto结构
    /// </summary>
    private ProtocolControlCache_ProtoStructure proto;

    /// <summary>
    /// 字符串拼接缓存
    /// </summary>
    private StringBuilder cacheStringBuilder;

    /// <summary>
    /// 计算缩进使用的字符串拼接
    /// </summary>
    private StringBuilder stringBuilderForRetract;

    /// <summary>
    /// 消息用的StringBuilder
    /// </summary>
    private StringBuilder stringBuilderForMessage;

    /// <summary>
    /// 变量用的StringBuilder
    /// </summary>
    private StringBuilder stringBuilderForVariable;

    /// <summary>
    /// lua类提示使用的StringBuilder
    /// </summary>
    private StringBuilder stringBuilderForLuaClassHint;

    /// <summary>
    /// 是否有任意消息被生成
    /// </summary>
    private bool isAnyMessageGenerated = false;

    /// <summary>
    /// 缩进计数
    /// </summary>
    private int retractCount;

    /// <summary>
    /// 从proto文件生成对应的lua文件
    /// </summary>
    public ServerTool_ProtocolController_GenerateLuaFromProto()
    {
        cacheStringBuilder = new StringBuilder();
        stringBuilderForRetract = new StringBuilder();
        stringBuilderForMessage = new StringBuilder();
        stringBuilderForVariable = new StringBuilder();
        stringBuilderForLuaClassHint = new StringBuilder();
    }

    /// <summary>
    /// 处理proto文件
    /// </summary>
    /// <param name="protoPath">proto路径</param>
    /// <param name="decodeOutputPath">转换脚本导出路径,默认与proto同路径,后缀名为".lua"</param>
    /// <param name="adjustOutputPath">调整脚本导出路径,默认与proto同路径,后缀为"_adj.lua"</param>
    /// <param name="needCheckCSharpType">是否需要检查C#类型是否存在</param>
    /// <returns>是否有lua文件生成</returns>
    public bool DealProto(string protoPath, string decodeOutputPath = null, string adjustOutputPath = null, bool needCheckCSharpType = false)
    {
        if (string.IsNullOrEmpty(protoPath))
        {
            return false;
        }
        ClearCache();
        //生成转换代码
        if (string.IsNullOrEmpty(decodeOutputPath))
        {
            decodeOutputPath = Path.ChangeExtension(protoPath, ".lua");
        }
        decodeOutputFilePath = decodeOutputPath;
        proto = ProtocolControlCache_ProtoStructure.GetProtoStructure(protoPath);
        if (proto == null || (proto != null && string.IsNullOrEmpty(proto.Package)))
        {
            ClearCache();
            return false;
        }
        else
        {
            GenerateDecodeLuaCodes(needCheckCSharpType);
            if (!SaveLuaCodes(decodeOutputFilePath))
            {
                ClearCache();
                return false;
            }
            ClearCache();
        }
        //生成调整代码
        if (string.IsNullOrEmpty(adjustOutputPath))
        {
            adjustOutputPath = Path.ChangeExtension(protoPath, ".lua");
            adjustOutputPath = adjustOutputPath.Substring(0, adjustOutputPath.Length - 4) + "_adj.lua";
        }
        adjustOutputFilePath = adjustOutputPath;
        proto = ProtocolControlCache_ProtoStructure.GetProtoStructure(protoPath);
        if (proto == null || (proto != null && string.IsNullOrEmpty(proto.Package)))
        {
            ClearCache();
            return false;
        }
        else
        {
            GenerateAdjustLuaCodes(false);
            if (!SaveLuaCodes(adjustOutputFilePath))
            {
                ClearCache();
                return false;
            }
            ClearCache();
            return true;
        }
    }

    /// <summary>
    /// 清理缓存
    /// </summary>
    private void ClearCache()
    {
        decodeOutputFilePath = null;
        adjustOutputFilePath = null;
        proto = null;
        cacheStringBuilder.Remove(0, cacheStringBuilder.Length);
        stringBuilderForRetract.Remove(0, stringBuilderForRetract.Length);
        stringBuilderForMessage.Remove(0, stringBuilderForMessage.Length);
        stringBuilderForVariable.Remove(0, stringBuilderForVariable.Length);
        retractCount = 0;
        isAnyMessageGenerated = false;
    }

    #region 生成转换文件的lua代码
    /// <summary>
    /// 生成转换lua代码至字符串拼接器中
    /// </summary>
    private void GenerateDecodeLuaCodes(bool needCheckCSharpType)
    {
        //拼接表名
        cacheStringBuilder.Append("--[[本文件为工具自动生成,禁止手动修改]]\r\nlocal ");
        cacheStringBuilder.Append(proto.Package);
        cacheStringBuilder.Append(" = {}\r\n\r\nlocal decodeTable = protobufMgr.DecodeTable");
        //拼接消息内容
        for (int i = 0; i < proto.Messages.Count; i++)
        {
            //若需要检查C#类型是否存在,则在不存在该C#类型时,不生成对应的适配代码
            if (needCheckCSharpType && !ServerTool_ProtocolControllerUtility.IsCSTypeLoaded(proto.Package, proto.Messages[i].MessageName))
            {
                cacheStringBuilder.Append(GetCurrentRetract());
                cacheStringBuilder.Append(GetCurrentRetract());
                cacheStringBuilder.Append("--[[");
                cacheStringBuilder.Append(proto.Package);
                cacheStringBuilder.Append(".");
                cacheStringBuilder.Append(proto.Messages[i].MessageName);
                cacheStringBuilder.Append(" 未在C#中找到对应的类型,不生成对应的lua转换代码");
                cacheStringBuilder.Append("]]");
                UnityEngine.Debug.Log("C#程序集中未找到 " + proto.Package + "." + proto.Messages[i].MessageName + " 类型,故忽略该类型的lua=>C#转化");
                continue;
            }
            isAnyMessageGenerated = true;
            cacheStringBuilder.Append(GetCurrentRetract());
            cacheStringBuilder.Append(GetCurrentRetract());
            cacheStringBuilder.Append("---@param decodedData ");
            cacheStringBuilder.Append(proto.Package);
            cacheStringBuilder.Append(".");
            cacheStringBuilder.Append(proto.Messages[i].MessageName);
            cacheStringBuilder.Append(" lua中的数据结构");
            cacheStringBuilder.Append(GetCurrentRetract());
            cacheStringBuilder.Append("---@return ");
            cacheStringBuilder.Append(proto.Package);
            cacheStringBuilder.Append(".");
            cacheStringBuilder.Append(proto.Messages[i].MessageName);
            cacheStringBuilder.Append(" C#中的数据结构");
            cacheStringBuilder.Append(GenerateDecodeMessage(proto.Messages[i], needCheckCSharpType));
        }
        cacheStringBuilder.Append(GetCurrentRetract());
        cacheStringBuilder.Append(GetCurrentRetract());
        cacheStringBuilder.Append("return ");
        cacheStringBuilder.Append(proto.Package);
    }

    /// <summary>
    /// 生成消息的字符串
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="needCheckCSharpType">是否需要检查C#类型</param>
    /// <returns></returns>
    private string GenerateDecodeMessage(ProtocolControlCache_ProtoStructure.Message msg, bool needCheckCSharpType)
    {
        if (msg == null)
        {
            return string.Empty;
        }
        stringBuilderForMessage.Remove(0, stringBuilderForMessage.Length);
        //消息名
        stringBuilderForMessage.Append(GetCurrentRetract());
        stringBuilderForMessage.Append("function ");
        stringBuilderForMessage.Append(proto.Package);
        stringBuilderForMessage.Append('.');
        stringBuilderForMessage.Append(msg.MessageName);
        stringBuilderForMessage.Append("(decodedData)");
        //判空
        AddRetract();
        stringBuilderForMessage.Append(GetCurrentRetract());
        stringBuilderForMessage.Append("if (decodedData == nil) then");
        AddRetract();
        stringBuilderForMessage.Append(GetCurrentRetract());
        stringBuilderForMessage.Append("return nil");
        MinusRetract();
        stringBuilderForMessage.Append(GetCurrentRetract());
        stringBuilderForMessage.Append("end");
        //C#对象定义
        stringBuilderForMessage.Append(GetCurrentRetract());
        stringBuilderForMessage.Append("local data = CS.");
        stringBuilderForMessage.Append(proto.Package);
        stringBuilderForMessage.Append('.');
        stringBuilderForMessage.Append(msg.MessageName);
        stringBuilderForMessage.Append("()");
        //向C#对象中填充内容
        for (int i = 0; i < msg.Variables.Length; i++)
        {
            var variableTemp = msg.Variables[i];
            if (needCheckCSharpType && !ServerTool_ProtocolControllerUtility.IsFieldExistInCSType(proto.Package, msg.MessageName, variableTemp.VariableName))
            {
                stringBuilderForMessage.Append(GetCurrentRetract());
                stringBuilderForMessage.Append("--C#的");
                stringBuilderForMessage.Append(proto.Package);
                stringBuilderForMessage.Append(".");
                stringBuilderForMessage.Append(msg.MessageName);
                stringBuilderForMessage.Append("类中没有找到");
                stringBuilderForMessage.Append(variableTemp.VariableName);
                stringBuilderForMessage.Append("字段,不填充数据");
                UnityEngine.Debug.LogErrorFormat("未在C#类 {0}.{1} 中找到 {2} 字段或属性,未进行数据填充", proto.Package, msg.MessageName, variableTemp.VariableName);
                continue;
            }
            stringBuilderForMessage.Append(GenerateDecodeVariable(msg, variableTemp));
        }
        stringBuilderForMessage.Append(GetCurrentRetract());
        stringBuilderForMessage.Append("return data");
        MinusRetract();
        stringBuilderForMessage.Append(GetCurrentRetract());
        stringBuilderForMessage.Append("end");
        return stringBuilderForMessage.ToString();
    }

    /// <summary>
    /// 生成变量的字符串
    /// </summary>
    /// <param name="msg">变量所在的消息</param>
    /// <param name="variable">变量</param>
    /// <returns></returns>
    private string GenerateDecodeVariable(ProtocolControlCache_ProtoStructure.Message msg, ProtocolControlCache_ProtoStructure.VariableInMessage variable)
    {
        if (msg == null || variable == null)
        {
            return string.Empty;
        }
        stringBuilderForVariable.Remove(0, stringBuilderForVariable.Length);
        //optional和repeated类型变量需要先判空
        if (variable.Modifier == ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Optional || variable.Modifier == ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Repeated)
        {
            stringBuilderForVariable.Append(GetCurrentRetract());
            stringBuilderForVariable.Append("if decodedData.");
            stringBuilderForVariable.Append(variable.VariableName);
            stringBuilderForVariable.Append(" ~= nil and decodedData.");
            stringBuilderForVariable.Append(variable.VariableName);
            stringBuilderForVariable.Append("Specified ~= false then");
            AddRetract();
        }
        //require和optional类型变量
        if (variable.Modifier == ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Required || variable.Modifier == ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Optional)
        {
            stringBuilderForVariable.Append(GetCurrentRetract());
            stringBuilderForVariable.Append("data.");
            stringBuilderForVariable.Append(variable.VariableName);
            stringBuilderForVariable.Append(" = ");
            if (variable.VType == ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Others)
            {
                VariableTypeInOthers variableTypeInOthers = GetVariableTypeInOthers(proto, msg, variable);
                stringBuilderForVariable.Append(variableTypeInOthers.decodeHelpString);
                stringBuilderForVariable.Append("(decodedData.");
                stringBuilderForVariable.Append(variable.VariableName);
                stringBuilderForVariable.Append(")");
            }
            else
            {
                stringBuilderForVariable.Append("decodedData.");
                stringBuilderForVariable.Append(variable.VariableName);
            }
        }
        //repeated类型变量
        else
        {
            stringBuilderForVariable.Append(GetCurrentRetract());
            stringBuilderForVariable.Append("for i = 1, #decodedData.");
            stringBuilderForVariable.Append(variable.VariableName);
            stringBuilderForVariable.Append(" do");
            AddRetract();
            stringBuilderForVariable.Append(GetCurrentRetract());
            stringBuilderForVariable.Append("data.");
            stringBuilderForVariable.Append(variable.VariableName);
            stringBuilderForVariable.Append(":Add(");
            if (variable.VType == ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Others)
            {
                VariableTypeInOthers variableTypeInOthers = GetVariableTypeInOthers(proto, msg, variable);
                stringBuilderForVariable.Append(variableTypeInOthers.decodeHelpString);
                stringBuilderForVariable.Append("(decodedData.");
                stringBuilderForVariable.Append(variable.VariableName);
                stringBuilderForVariable.Append("[i])");
            }
            else
            {
                stringBuilderForVariable.Append("decodedData.");
                stringBuilderForVariable.Append(variable.VariableName);
                stringBuilderForVariable.Append("[i]");
            }
            stringBuilderForVariable.Append(")");
            MinusRetract();
            stringBuilderForVariable.Append(GetCurrentRetract());
            stringBuilderForVariable.Append("end");
        }
        //optional和repeated类型变量需要结束判空
        if (variable.Modifier == ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Optional || variable.Modifier == ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Repeated)
        {
            MinusRetract();
            stringBuilderForVariable.Append(GetCurrentRetract());
            stringBuilderForVariable.Append("end");
        }
        return stringBuilderForVariable.ToString();
    }
    #endregion

    /// <summary>
    /// 生成lua类的提示
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    private string GenerateLuaClassHint(ProtocolControlCache_ProtoStructure proto, ProtocolControlCache_ProtoStructure.Message msg)
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
        stringBuilderForLuaClassHint.Append(GetCurrentRetract());
        stringBuilderForLuaClassHint.Append("---class properties");
        stringBuilderForLuaClassHint.Append(GetCurrentRetract());
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
                    VariableTypeInOthers variableTypeInOthers = GetVariableTypeInOthers(proto, msg, variable);
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
                    stringBuilderForLuaClassHint.Append(GetCurrentRetract());
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
                    stringBuilderForLuaClassHint.Append(GetCurrentRetract());
                    stringBuilderForLuaClassHint.Append("---@field public ");
                    stringBuilderForLuaClassHint.Append(variable.VariableName);
                    stringBuilderForLuaClassHint.Append("Specified boolean");
                    stringBuilderForLuaClassHint.Append(GetCurrentRetract());
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
                    stringBuilderForLuaClassHint.Append(GetCurrentRetract());
                    break;
                case ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.NULL:
                    break;
                default:
                    break;
            }
        }
        return stringBuilderForLuaClassHint.ToString();
    }

    #region 生成调整文件的lua代码
    /// <summary>
    /// 生成调整lua代码至字符串拼接器中
    /// </summary>
    /// <param name="needCheckCSharpType"></param>
    private void GenerateAdjustLuaCodes(bool needCheckCSharpType)
    {
        //拼接表名
        cacheStringBuilder.Append("--[[本文件为工具自动生成,禁止手动修改]]\r\nlocal ");
        cacheStringBuilder.Append(proto.Package);
        cacheStringBuilder.Append("_adj = {}\r\n\r\nlocal adjustTable = protobufMgr.AdjustTable");
        //拼接消息内容
        for (int i = 0; i < proto.Messages.Count; i++)
        {
            //若需要检查C#类型是否存在,则在不存在该C#类型时,不生成对应的适配代码
            if (needCheckCSharpType && !ServerTool_ProtocolControllerUtility.IsCSTypeLoaded(proto.Package, proto.Messages[i].MessageName))
            {
                cacheStringBuilder.Append(GetCurrentRetract());
                cacheStringBuilder.Append(GetCurrentRetract());
                cacheStringBuilder.Append("--[[");
                cacheStringBuilder.Append(proto.Package);
                cacheStringBuilder.Append(".");
                cacheStringBuilder.Append(proto.Messages[i].MessageName);
                cacheStringBuilder.Append(" 未在C#中找到对应的类型,不生成对应的lua调整代码");
                cacheStringBuilder.Append("]]");
                UnityEngine.Debug.Log("C#程序集中未找到 " + proto.Package + "." + proto.Messages[i].MessageName + "类型,故忽略该类型的luaTable调整");
                continue;
            }
            isAnyMessageGenerated = true;
            cacheStringBuilder.Append(GetCurrentRetract());
            cacheStringBuilder.Append(GetCurrentRetract());
            cacheStringBuilder.Append(GenerateLuaClassHint(proto, proto.Messages[i]));
            cacheStringBuilder.Append("---function params");
            cacheStringBuilder.Append(GetCurrentRetract());
            cacheStringBuilder.Append("---@param tbl ");
            cacheStringBuilder.Append(proto.Package);
            cacheStringBuilder.Append(".");
            cacheStringBuilder.Append(proto.Messages[i].MessageName);
            cacheStringBuilder.Append(" 待调整的table数据");
            cacheStringBuilder.Append(GenerateAdjustMessage(proto.Messages[i], needCheckCSharpType));
        }
        cacheStringBuilder.Append(GetCurrentRetract());
        cacheStringBuilder.Append(GetCurrentRetract());
        cacheStringBuilder.Append("return ");
        cacheStringBuilder.Append(proto.Package);
        cacheStringBuilder.Append("_adj");
    }

    /// <summary>
    /// 生成消息的字符串
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="needCheckCSharpType">是否需要检查C#类型</param>
    /// <returns></returns>
    private string GenerateAdjustMessage(ProtocolControlCache_ProtoStructure.Message msg, bool needCheckCSharpType)
    {
        if (msg == null)
        {
            return string.Empty;
        }
        stringBuilderForMessage.Remove(0, stringBuilderForMessage.Length);
        //消息名
        stringBuilderForMessage.Append(GetCurrentRetract());
        stringBuilderForMessage.Append("function ");
        stringBuilderForMessage.Append(proto.Package);
        stringBuilderForMessage.Append("_adj.Adjust");
        stringBuilderForMessage.Append(msg.MessageName);
        stringBuilderForMessage.Append("(tbl)");
        AddRetract();
        stringBuilderForMessage.Append(GetCurrentRetract());
        stringBuilderForMessage.Append("if (tbl == nil) then");
        AddRetract();
        stringBuilderForMessage.Append(GetCurrentRetract());
        stringBuilderForMessage.Append("return");
        MinusRetract();
        stringBuilderForMessage.Append(GetCurrentRetract());
        stringBuilderForMessage.Append("end");
        MinusRetract();
        //向C#对象中填充内容
        AddRetract();
        for (int i = 0; i < msg.Variables.Length; i++)
        {
            var variableTemp = msg.Variables[i];
            if (needCheckCSharpType && !ServerTool_ProtocolControllerUtility.IsFieldExistInCSType(proto.Package, msg.MessageName, variableTemp.VariableName))
            {
                stringBuilderForMessage.Append(GetCurrentRetract());
                stringBuilderForMessage.Append("--C#的");
                stringBuilderForMessage.Append(proto.Package);
                stringBuilderForMessage.Append(".");
                stringBuilderForMessage.Append(msg.MessageName);
                stringBuilderForMessage.Append("类中没有找到");
                stringBuilderForMessage.Append(variableTemp.VariableName);
                stringBuilderForMessage.Append("字段,不检查数据");
                UnityEngine.Debug.LogErrorFormat("未在C#类 {0}.{1} 中找到 {2} 字段或属性,未进行数据调整", proto.Package, msg.MessageName, variableTemp.VariableName);
                continue;
            }
            stringBuilderForMessage.Append(GenerateAdjustVariable(msg, variableTemp));
        }
        MinusRetract();
        stringBuilderForMessage.Append(GetCurrentRetract());
        stringBuilderForMessage.Append("end");
        return stringBuilderForMessage.ToString();
    }

    /// <summary>
    /// 生成变量的字符串
    /// </summary>
    /// <param name="msg">变量所在的消息</param>
    /// <param name="variable">变量</param>
    /// <returns></returns>
    private string GenerateAdjustVariable(ProtocolControlCache_ProtoStructure.Message msg, ProtocolControlCache_ProtoStructure.VariableInMessage variable)
    {
        if (msg == null || variable == null)
        {
            return string.Empty;
        }
        stringBuilderForVariable.Remove(0, stringBuilderForVariable.Length);
        VariableTypeInOthers variableTypeInOthers = GetVariableTypeInOthers(proto, msg, variable);
        if (variable.VType == ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Others)
        {
            if (variable.Modifier == ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Optional)
            {
                stringBuilderForVariable.Append(GetCurrentRetract());
                //optional类型
                stringBuilderForVariable.Append("if tbl.");
                stringBuilderForVariable.Append(variable.VariableName);
                stringBuilderForVariable.Append(" == nil then");
                AddRetract();
                stringBuilderForVariable.Append(GetCurrentRetract());
                stringBuilderForVariable.Append("tbl.");
                stringBuilderForVariable.Append(variable.VariableName);
                stringBuilderForVariable.Append("Specified = false");
                stringBuilderForVariable.Append(GetCurrentRetract());
                stringBuilderForVariable.Append("tbl.");
                stringBuilderForVariable.Append(variable.VariableName);
                stringBuilderForVariable.Append(" = ");
                string defaultValue;
                if (variableTypeInOthers.isEnumType)
                {
                    //为枚举类型
                    if (!string.IsNullOrEmpty(variable.DefaultValue))
                    {
                        //默认值不为空时,选取枚举中对应的值
                        if (variableTypeInOthers.enumType != null && variableTypeInOthers.enumType.GetOption(variable.DefaultValue, out int index))
                        {
                            defaultValue = index.ToString();
                        }
                        else
                        {
                            defaultValue = "0";
                        }
                    }
                    else
                    {
                        defaultValue = "\"\"";
                    }
                }
                else
                {
                    //不为枚举类型
                    if (!string.IsNullOrEmpty(variable.DefaultValue))
                    {
                        //若有默认值,则应该是字符串,则赋值字符串过去
                        defaultValue = "\"" + variable.DefaultValue + "\"";
                    }
                    else
                    {
                        //若没有默认值,则直接赋值nil
                        defaultValue = "nil";
                    }
                }
                stringBuilderForVariable.Append(defaultValue);
                MinusRetract();
                stringBuilderForVariable.Append(GetCurrentRetract());
                stringBuilderForVariable.Append("else");
                AddRetract();
                if (variable.VType == ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Others && !variableTypeInOthers.isEnumType && !string.IsNullOrEmpty(variableTypeInOthers.adjustNullAdjustString))
                {
                    stringBuilderForVariable.Append(GetCurrentRetract());
                    //variableTypeInOthers.adjustHelpString
                    stringBuilderForVariable.Append("if tbl.");
                    stringBuilderForVariable.Append(variable.VariableName);
                    stringBuilderForVariable.Append("Specified == nil ");
                    stringBuilderForVariable.Append("then ");
                    AddRetract();
                    stringBuilderForVariable.Append(GetCurrentRetract());
                    stringBuilderForVariable.Append("tbl.");
                    stringBuilderForVariable.Append(variable.VariableName);
                    stringBuilderForVariable.Append("Specified = true");
                    stringBuilderForVariable.Append(GetCurrentRetract());
                    stringBuilderForVariable.Append("if ");
                    stringBuilderForVariable.Append(variableTypeInOthers.adjustNullAdjustString);
                    stringBuilderForVariable.Append(" then");
                    AddRetract();
                    stringBuilderForVariable.Append(GetCurrentRetract());
                    stringBuilderForVariable.Append(variableTypeInOthers.adjustHelpString);
                    stringBuilderForVariable.Append("(tbl.");
                    stringBuilderForVariable.Append(variable.VariableName);
                    stringBuilderForVariable.Append(")");
                    MinusRetract();
                    stringBuilderForVariable.Append(GetCurrentRetract());
                    stringBuilderForVariable.Append("end");
                    MinusRetract();
                    stringBuilderForVariable.Append(GetCurrentRetract());
                    stringBuilderForVariable.Append("end");
                }
                else
                {
                    stringBuilderForVariable.Append(GetCurrentRetract());
                    stringBuilderForVariable.Append("tbl.");
                    stringBuilderForVariable.Append(variable.VariableName);
                    stringBuilderForVariable.Append("Specified = true");
                }
                MinusRetract();
                stringBuilderForVariable.Append(GetCurrentRetract());
                stringBuilderForVariable.Append("end");
            }
            else if (variable.Modifier == ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Repeated)
            {
                stringBuilderForVariable.Append(GetCurrentRetract());
                //repeated类型
                stringBuilderForVariable.Append("if tbl.");
                stringBuilderForVariable.Append(variable.VariableName);
                stringBuilderForVariable.Append(" == nil then");
                AddRetract();
                stringBuilderForVariable.Append(GetCurrentRetract());
                stringBuilderForVariable.Append("tbl.");
                stringBuilderForVariable.Append(variable.VariableName);
                stringBuilderForVariable.Append(" = {}");
                MinusRetract();
                stringBuilderForVariable.Append(GetCurrentRetract());
                stringBuilderForVariable.Append("else");
                AddRetract();
                stringBuilderForVariable.Append(GetCurrentRetract());
                stringBuilderForVariable.Append("if ");
                stringBuilderForVariable.Append(variableTypeInOthers.adjustNullAdjustString);
                stringBuilderForVariable.Append(" then");
                AddRetract();
                stringBuilderForVariable.Append(GetCurrentRetract());
                stringBuilderForVariable.Append("for i = 1, #tbl.");
                stringBuilderForVariable.Append(variable.VariableName);
                stringBuilderForVariable.Append(" do");
                AddRetract();
                stringBuilderForVariable.Append(GetCurrentRetract());
                stringBuilderForVariable.Append(variableTypeInOthers.adjustHelpString);
                stringBuilderForVariable.Append("(tbl.");
                stringBuilderForVariable.Append(variable.VariableName);
                stringBuilderForVariable.Append("[i])");
                MinusRetract();
                stringBuilderForVariable.Append(GetCurrentRetract());
                MinusRetract();
                stringBuilderForVariable.Append("end");
                stringBuilderForVariable.Append(GetCurrentRetract());
                MinusRetract();
                stringBuilderForVariable.Append("end");
                stringBuilderForVariable.Append(GetCurrentRetract());
                stringBuilderForVariable.Append("end");
            }
        }
        else
        {
            if (variable.Modifier == ProtocolControlCache_ProtoStructure.VariableInMessage.ModifierType.Optional)
            {
                stringBuilderForVariable.Append(GetCurrentRetract());
                //普通类型仅对optional类型字段进行Specified判定
                stringBuilderForVariable.Append("if tbl.");
                stringBuilderForVariable.Append(variable.VariableName);
                stringBuilderForVariable.Append(" == nil then");
                AddRetract();
                stringBuilderForVariable.Append(GetCurrentRetract());
                stringBuilderForVariable.Append("tbl.");
                stringBuilderForVariable.Append(variable.VariableName);
                stringBuilderForVariable.Append("Specified = false");
                stringBuilderForVariable.Append(GetCurrentRetract());
                stringBuilderForVariable.Append("tbl.");
                stringBuilderForVariable.Append(variable.VariableName);
                stringBuilderForVariable.Append(" = ");
                string defaultValue;
                switch (variable.VType)
                {
                    case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Bool:
                        if (!string.IsNullOrEmpty(variable.DefaultValue) && variable.DefaultValue.ToLower() == "true")
                        {
                            defaultValue = "true";
                        }
                        else
                        {
                            defaultValue = "false";
                        }
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
                        if (!string.IsNullOrEmpty(variable.DefaultValue) && double.TryParse(variable.DefaultValue, out double valueTemp))
                        {
                            defaultValue = valueTemp.ToString();
                        }
                        else
                        {
                            defaultValue = "0";
                        }
                        break;
                    case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.String:
                    case ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Bytes:
                    default:
                        //字符串类型
                        if (!string.IsNullOrEmpty(variable.DefaultValue))
                        {
                            defaultValue = "\"" + variable.DefaultValue + "\"";
                        }
                        else
                        {
                            defaultValue = "\"\"";
                        }
                        break;
                }
                stringBuilderForVariable.Append(defaultValue);
                MinusRetract();
                stringBuilderForVariable.Append(GetCurrentRetract());
                stringBuilderForVariable.Append("else");
                AddRetract();
                stringBuilderForVariable.Append(GetCurrentRetract());
                stringBuilderForVariable.Append("tbl.");
                stringBuilderForVariable.Append(variable.VariableName);
                stringBuilderForVariable.Append("Specified = true");
                MinusRetract();
                stringBuilderForVariable.Append(GetCurrentRetract());
                stringBuilderForVariable.Append("end");
            }
        }
        return stringBuilderForVariable.ToString();
    }
    #endregion

    #region 代码生成处理
    /// <summary>
    /// 缩进加一
    /// </summary>
    private void AddRetract()
    {
        retractCount++;
    }

    /// <summary>
    /// 缩进减一
    /// </summary>
    private void MinusRetract()
    {
        retractCount--;
        if (retractCount < 0)
        {
            retractCount = 0;
        }
    }

    /// <summary>
    /// 获得当前的缩进字符串
    /// </summary>
    /// <returns></returns>
    private string GetCurrentRetract()
    {
        int summarySpaceCount = retractCount * 4;
        stringBuilderForRetract.Remove(0, stringBuilderForRetract.Length);
        stringBuilderForRetract.Append("\r\n");
        for (int i = 0; i < summarySpaceCount; i++)
        {
            stringBuilderForRetract.Append(' ');
        }
        return stringBuilderForRetract.ToString();
    }

    /// <summary>
    /// 获得变量类型为Others的变量的实际类型
    /// </summary>
    /// <param name="proto">消息所属的proto</param>
    /// <param name="msg">变量所在消息</param>
    /// <param name="variable">需要获取类型的变量</param>
    /// <returns>其他类型变量的类型</returns>
    public static VariableTypeInOthers GetVariableTypeInOthers(ProtocolControlCache_ProtoStructure proto, ProtocolControlCache_ProtoStructure.Message msg, ProtocolControlCache_ProtoStructure.VariableInMessage variable)
    {
        VariableTypeInOthers variableTypeInOthers = new VariableTypeInOthers();
        //Others一般只有枚举和消息两种类型,多层次遍历查询
        //得到去掉包名或者消息名的变量类型名
        string[] strs = variable.VariableTypeString.Split('.');
        string pureVariableTypeString;
        if (strs != null && strs.Length > 1)
        {
            pureVariableTypeString = strs[strs.Length - 1];
        }
        else
        {
            pureVariableTypeString = variable.VariableTypeString;
        }
        //先遍历本消息内定义的枚举
        for (int i = 0; i < msg.Enums.Length; i++)
        {
            if (pureVariableTypeString == msg.Enums[i].EnumName)
            {
                variableTypeInOthers.isEnumType = true;
                variableTypeInOthers.isInOtherPackage = false;
                variableTypeInOthers.decodeHelpString = string.Format("CS.{0}.{1}.{2}.__CastFrom", proto.Package, msg.MessageName, msg.Enums[i].EnumName);
                variableTypeInOthers.typeString = string.Format("{0}.{1}.{2}", proto.Package, msg.MessageName, msg.Enums[i].EnumName);
                variableTypeInOthers.adjustHelpString = string.Empty;
                variableTypeInOthers.adjustNullAdjustString = string.Empty;
                variableTypeInOthers.enumType = msg.Enums[i];
                variableTypeInOthers.messageType = null;
                return variableTypeInOthers;
            }
        }
        //再遍历本proto中定义的枚举
        for (int i = 0; i < proto.EnumsInProto.Count; i++)
        {
            if (pureVariableTypeString == proto.EnumsInProto[i].EnumName)
            {
                variableTypeInOthers.isEnumType = true;
                variableTypeInOthers.isInOtherPackage = false;
                variableTypeInOthers.decodeHelpString = string.Format("CS.{0}.{1}.__CastFrom", proto.Package, proto.EnumsInProto[i].EnumName);
                variableTypeInOthers.typeString = string.Format("{0}.{1}", proto.Package, proto.EnumsInProto[i].EnumName);
                variableTypeInOthers.adjustNullAdjustString = string.Empty;
                variableTypeInOthers.adjustHelpString = string.Empty;
                variableTypeInOthers.enumType = proto.EnumsInProto[i];
                variableTypeInOthers.messageType = null;
                return variableTypeInOthers;
            }
        }
        //而后遍历proto中定义的消息
        for (int i = 0; i < proto.Messages.Count; i++)
        {
            if (pureVariableTypeString == proto.Messages[i].MessageName)
            {
                variableTypeInOthers.isEnumType = false;
                variableTypeInOthers.isInOtherPackage = false;
                variableTypeInOthers.decodeHelpString = string.Format("{0}.{1}", proto.Package, proto.Messages[i].MessageName);
                variableTypeInOthers.typeString = string.Format("{0}.{1}", proto.Package, proto.Messages[i].MessageName);
                variableTypeInOthers.adjustHelpString = string.Format("{0}_adj.Adjust{1}", proto.Package, proto.Messages[i].MessageName);
                variableTypeInOthers.adjustNullAdjustString = string.Format("{0}_adj.Adjust{1} ~= nil", proto.Package, proto.Messages[i].MessageName);
                variableTypeInOthers.enumType = null;
                variableTypeInOthers.messageType = proto.Messages[i];
                return variableTypeInOthers;
            }
        }
        //最后遍历其他proto
        for (int i = 0; i < proto.ImportedProtos.Count; i++)
        {
            //枚举
            for (int j = 0; j < proto.ImportedProtos[i].EnumsInProto.Count; j++)
            {
                if (pureVariableTypeString == proto.ImportedProtos[i].EnumsInProto[j].EnumName)
                {
                    variableTypeInOthers.isEnumType = true;
                    variableTypeInOthers.isInOtherPackage = true;
                    variableTypeInOthers.decodeHelpString = string.Format("CS.{0}.{1}.__CastFrom", proto.ImportedProtos[i].Package, proto.ImportedProtos[i].EnumsInProto[j].EnumName);
                    variableTypeInOthers.typeString = string.Format("{0}.{1}", proto.ImportedProtos[i].Package, proto.ImportedProtos[i].EnumsInProto[j].EnumName);
                    variableTypeInOthers.adjustHelpString = string.Empty;
                    variableTypeInOthers.adjustNullAdjustString = string.Empty;
                    variableTypeInOthers.enumType = proto.ImportedProtos[i].EnumsInProto[j];
                    variableTypeInOthers.messageType = null;
                    return variableTypeInOthers;
                }
            }
            //消息
            for (int j = 0; j < proto.ImportedProtos[i].Messages.Count; j++)
            {
                if (pureVariableTypeString == proto.ImportedProtos[i].Messages[j].MessageName)
                {
                    variableTypeInOthers.isEnumType = false;
                    variableTypeInOthers.isInOtherPackage = true;
                    variableTypeInOthers.decodeHelpString = string.Format("decodeTable.{0}.{1}", proto.ImportedProtos[i].ProtoFileName, proto.ImportedProtos[i].Messages[j].MessageName);
                    variableTypeInOthers.typeString = string.Format("{0}.{1}", proto.ImportedProtos[i].Package, proto.ImportedProtos[i].Messages[j].MessageName);
                    variableTypeInOthers.adjustHelpString = string.Format("adjustTable.{0}_adj.Adjust{1}", proto.ImportedProtos[i].ProtoFileName, proto.ImportedProtos[i].Messages[j].MessageName);
                    variableTypeInOthers.adjustNullAdjustString = string.Format("adjustTable.{0}_adj ~= nil and adjustTable.{0}_adj.Adjust{1} ~= nil", proto.ImportedProtos[i].ProtoFileName, proto.ImportedProtos[i].Messages[j].MessageName);
                    variableTypeInOthers.enumType = null;
                    variableTypeInOthers.messageType = proto.ImportedProtos[i].Messages[j];
                    return variableTypeInOthers;
                }
            }
        }
        //若还未找到,则返回原始的变量类型
        variableTypeInOthers.isEnumType = false;
        variableTypeInOthers.isInOtherPackage = false;
        variableTypeInOthers.decodeHelpString = variable.VariableTypeString;
        variableTypeInOthers.typeString = variable.VariableTypeString;
        variableTypeInOthers.adjustHelpString = variable.VariableTypeString;
        variableTypeInOthers.adjustNullAdjustString = variable.VariableTypeString;
        variableTypeInOthers.enumType = null;
        variableTypeInOthers.messageType = null;
        if (variable.VType == ProtocolControlCache_ProtoStructure.VariableInMessage.VariableType.Others)
        {
            UnityEngine.Debug.LogErrorFormat("未在proto文件或其他proto文件中找到 {0}.{1} 的 {2} 字段类型", proto.Package, msg.MessageName, variable.VariableTypeString);
        }
        return variableTypeInOthers;
    }
    #endregion

    /// <summary>
    /// 保存lua代码
    /// </summary>
    /// <param name="filePath">代码文件路径</param>
    /// <returns>是否有代码生成了</returns>
    private bool SaveLuaCodes(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            if (isAnyMessageGenerated)
            {
                File.WriteAllText(filePath, cacheStringBuilder.ToString());
            }
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex.Message);
        }
        return isAnyMessageGenerated;
    }

    /// <summary>
    /// 其他类中的变量类型描述数据结构
    /// </summary>
    public class VariableTypeInOthers
    {
        public string typeString;
        public string decodeHelpString;
        public string adjustHelpString;
        public string adjustNullAdjustString;
        public bool isEnumType;
        public bool isInOtherPackage;
        public ProtocolControlCache_ProtoStructure.EnumType enumType;
        public ProtocolControlCache_ProtoStructure.Message messageType;
    }
}