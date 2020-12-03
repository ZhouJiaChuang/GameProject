using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// proto文件结构
/// </summary>
public class ProtocolControlCache_ProtoStructure
{
    private readonly static string pattern_Package = @"^\s*package \w+;";
    private readonly static string pattern_ImportProto = @"import\s+""[\w..]+.proto\"";";

    /// <summary>
    /// protobuf协议结构缓存
    /// </summary>
    private static Dictionary<string, ProtocolControlCache_ProtoStructure> ProtocolStructureCache = new Dictionary<string, ProtocolControlCache_ProtoStructure>();

    /// <summary>
    /// 清空缓存
    /// </summary>
    public static void ClearProtoStructureCache()
    {
        ProtocolStructureCache.Clear();
    }

    /// <summary>
    /// 获取proto结构
    /// </summary>
    /// <param name="fullPath"></param>
    /// <returns></returns>
    public static ProtocolControlCache_ProtoStructure GetProtoStructure(string fullPath)
    {
        if (string.IsNullOrEmpty(fullPath))
        {
            return null;
        }
        if (ProtocolStructureCache.ContainsKey(fullPath))
        {
            return ProtocolStructureCache[fullPath];
        }
        var temp = new ProtocolControlCache_ProtoStructure(fullPath);
        ProtocolStructureCache[fullPath] = temp;
        temp.Deal();
        return temp;
    }

    private string protoContent;

    private string protoFileName;
    public string ProtoFileName { get { return protoFileName; } }

    private string protoFullPath;
    public string ProtoFullPath { get { return protoFullPath; } }

    private string package;
    public string Package { get { return package; } }

    private List<Message> messages;
    public List<Message> Messages { get { messages = messages ?? new List<Message>(); return messages; } }

    private List<ProtocolControlCache_ProtoStructure> importedProtos;
    public List<ProtocolControlCache_ProtoStructure> ImportedProtos { get { importedProtos = importedProtos ?? new List<ProtocolControlCache_ProtoStructure>(); return importedProtos; } }

    private List<EnumType> enumsInProto;
    public List<EnumType> EnumsInProto { get { enumsInProto = enumsInProto ?? new List<EnumType>(); return enumsInProto; } }

    private ProtocolControlCache_ProtoStructure(string protoFullPath)
    {
        this.protoFullPath = protoFullPath;
        ProtocolStructureCache[protoFullPath] = this;
        protoFileName = Path.GetFileNameWithoutExtension(protoFullPath);
    }

    public void Deal()
    {
        try
        {
            protoContent = File.ReadAllText(protoFullPath);
        }
        catch (FileNotFoundException ex)
        {
            UnityEngine.Debug.LogError(string.Format("读取proto文件错误,文件路径无效 {0}\r\n{1}\r\n{2}", protoFullPath, ex.Message, ex.StackTrace));
            return;
        }
        catch (DirectoryNotFoundException ex)
        {
            UnityEngine.Debug.LogError(string.Format("读取proto文件错误,目录无效 {0}\r\n{1}\r\n{2}", protoFullPath, ex.Message, ex.StackTrace));
            return;
        }
        catch (IOException ex)
        {
            UnityEngine.Debug.LogError(string.Format("读取proto文件错误,文件IO被占用 {0}\r\n{1}\r\n{2}", protoFullPath, ex.Message, ex.StackTrace));
            return;
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError(string.Format("读取proto文件错误 {0}\r\n{1}\r\n{2}", protoFullPath, ex.Message, ex.StackTrace));
            return;
        }
        Match packageMatch = Regex.Match(protoContent, pattern_Package);
        if (packageMatch != null)
        {
            string strTemp = packageMatch.Value;
            package = strTemp.Replace("package ", string.Empty).Replace(';', ' ').Trim();
        }
        MatchCollection importedProtoMatchCollection = Regex.Matches(protoContent, pattern_ImportProto);
        if (importedProtoMatchCollection != null)
        {
            foreach (Match item in importedProtoMatchCollection)
            {
                string newProtoName = item.Value.Replace("import", string.Empty).Replace('\"', ' ').Replace(';', ' ').Replace(".proto", string.Empty).Trim();
                string dirPath = Path.GetDirectoryName(protoFullPath);
                ProtocolControlCache_ProtoStructure importedProtoTemp = GetProtoStructure(dirPath + '/' + newProtoName + ".proto");
                ImportedProtos.Add(importedProtoTemp);
            }
        }
        MatchCollection messageMatchCollection = Regex.Matches(protoContent, Message.pattern);
        if (messageMatchCollection != null)
        {
            foreach (Match item in messageMatchCollection)
            {
                Message message = new Message(item.Value);
                message.Owner = this;
                if (message.Deal())
                {
                    Messages.Add(message);
                }
            }
        }
        MatchCollection enumMatchCollection = Regex.Matches(protoContent, EnumType.pattern);
        if (enumMatchCollection != null)
        {
            //将所有message中的枚举名放到一起
            List<EnumType> enumnames = new List<EnumType>();
            for (int i = 0; i < Messages.Count; i++)
            {
                for (int j = 0; j < Messages[i].Enums.Length; j++)
                {
                    enumnames.Add(Messages[i].Enums[j]);
                }
            }
            //遍历proto中所有的枚举,若message中没有相同的枚举,则将其加入列表中
            foreach (Match item in enumMatchCollection)
            {
                EnumType enumtemp = new EnumType(item.Value);
                if (enumtemp.Deal())
                {
                    bool isContain = false;
                    for (int i = 0; i < enumnames.Count; i++)
                    {
                        if (enumnames[i].IsEqualTo(enumtemp))
                        {
                            enumnames.RemoveAt(i);
                            isContain = true;
                            break;
                        }
                    }
                    if (!isContain)
                    {
                        EnumsInProto.Add(enumtemp);
                    }
                }
            }
        }
        protoContent = string.Empty;
    }

    /// <summary>
    /// 消息
    /// </summary>
    public class Message
    {
        public readonly static string pattern = @"message((([^}]+)enum([^}]+)})*)([^}]+)\n}";

        private readonly static string messageNamePattern = @"message\s+\w+";

        public ProtocolControlCache_ProtoStructure Owner { get; set; }

        private string content;

        private string messageName;
        public string MessageName { get { return messageName; } }

        private EnumType[] enums;
        public EnumType[] Enums { get { return enums; } }

        private VariableInMessage[] variables;
        public VariableInMessage[] Variables { get { return variables; } }

        public Message(string messageContent)
        {
            content = messageContent;
        }

        public bool Deal()
        {
            bool res = GetMessageName() && GetAllEnums() && GetAllVariables();
            content = string.Empty;
            return res;
        }

        private bool GetMessageName()
        {
            Match messageNameMatch = Regex.Match(content, messageNamePattern);
            if (messageNameMatch.Success)
            {
                messageName = messageNameMatch.Value.Substring(7, messageNameMatch.Value.Length - 7).Trim();
                return true;
            }
            return false;
        }

        private bool GetAllEnums()
        {
            MatchCollection enumTypeCollection = Regex.Matches(content, EnumType.pattern);
            IEnumerator enumIEnumerator = enumTypeCollection.GetEnumerator();
            List<EnumType> enumList = new List<EnumType>();
            if (enumIEnumerator != null)
            {
                while (enumIEnumerator.MoveNext())
                {
                    EnumType enumTemp = new EnumType((enumIEnumerator.Current as Match).Value);
                    if (enumTemp.Deal())
                    {
                        enumList.Add(enumTemp);
                    }
                }
            }
            enums = enumList.ToArray();
            return true;
        }

        private bool GetAllVariables()
        {
            MatchCollection variableCollection = Regex.Matches(content, VariableInMessage.pattern);
            IEnumerator variableIEnumerator = variableCollection.GetEnumerator();
            List<VariableInMessage> variableList = new List<VariableInMessage>();
            while (variableIEnumerator.MoveNext())
            {
                VariableInMessage variableTemp = new VariableInMessage((variableIEnumerator.Current as Match).Value.Trim());
                if (variableTemp.Deal())
                {
                    variableList.Add(variableTemp);
                }
            }
            variables = variableList.ToArray();
            return true;
        }

        public EnumType GetEnumTypeByName(string enumName)
        {
            if (enums == null)
            {
                return null;
            }
            for (int i = 0; i < enums.Length; i++)
            {
                if (enums[i].EnumName == enumName)
                {
                    return enums[i];
                }
            }
            return null;
        }

        public override string ToString()
        {
            StringBuilder sbTemp = new StringBuilder();
            sbTemp.Append("Message ");
            sbTemp.Append(messageName);
            sbTemp.Append(":\n");
            if (enums.Length > 0)
            {
                sbTemp.Append("Enums:\n");
                for (int i = 0; i < enums.Length; i++)
                {
                    sbTemp.Append(enums[i].ToString());
                    sbTemp.Append("\n");
                }
            }
            if (variables.Length > 0)
            {
                sbTemp.Append("Variables:\n");
                for (int i = 0; i < variables.Length; i++)
                {
                    sbTemp.Append(variables[i].ToString());
                    sbTemp.Append("\n");
                }
            }
            return sbTemp.ToString();
        }
    }

    /// <summary>
    /// 消息中的变量
    /// </summary>
    public class VariableInMessage
    {
        public readonly static string pattern = @"(?<!//[\t\v\f ]*)(required|repeated|optional)\s+[\w..]+\s+\w+\s*=\s*\d+\s*(\[\s*\w+\s*=\s*\S+?\s*\])*\s*;(//[^\r\n]+)?";

        //字段index匹配
        private readonly static string indexPattern = @"=\s*\d+\s*";
        //字段默认值匹配
        private readonly static string defaultValuePattern = @"\[default\s*=\s*(?<defaultValue>\S+?)\s*\]?";

        public enum ModifierType
        {
            Required,
            Optional,
            Repeated,
            NULL
        }

        public enum VariableType
        {
            Bool,
            Double,
            Float,
            Int32,
            UInt32,
            Int64,
            UInt64,
            SInt32,
            SInt64,
            Sing64,
            Fixed32,
            Fixed64,
            SFixed32,
            SFixed64,
            String,
            Bytes,
            Others
        }

        private ModifierType modifier;
        public ModifierType Modifier { get { return modifier; } }

        private VariableType variableType;
        public VariableType VType { get { return variableType; } }

        private string variableTypeString;
        public string VariableTypeString { get { return variableTypeString; } }

        private string variableName;
        public string VariableName { get { return variableName; } }

        private int variableIndex;
        public int VariableIndex { get { return variableIndex; } }

        private string comment;
        public string Comment { get { return comment; } }

        private string content;
        public string Content { get { return content; } }

        private string defaultValue;
        public string DefaultValue { get { return defaultValue; } }

        public VariableInMessage(string variableContent)
        {
            content = variableContent;
        }

        public bool Deal()
        {
            string modifierString = content.Substring(0, 8);
            modifier = GetModifierTypeByString(modifierString);
            string variableAndNameString = content.Substring(8, content.IndexOf('=') - 8).Trim();
            string[] strs = variableAndNameString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (strs != null && strs.Length >= 2)
            {
                if (content.Contains("//"))
                {
                    comment = content.Substring(content.IndexOf("//") + 2).Trim();
                }
                else
                {
                    comment = string.Empty;
                }
                variableTypeString = strs[0];
                variableType = GetVariableTypeByString(variableTypeString);
                variableName = strs[1];
                Match defaultValueMatch = Regex.Match(content, defaultValuePattern);
                if (defaultValueMatch.Success)
                {
                    defaultValue = defaultValueMatch.Groups["defaultValue"].Value;
                }
                else
                {
                    defaultValue = null;
                }
                Match indexMatch = Regex.Match(content, indexPattern);
                if (indexMatch.Success)
                {
                    return modifier != ModifierType.NULL && int.TryParse(indexMatch.Value.Substring(1, indexMatch.Value.Length - 1).Trim(), out variableIndex);
                }
            }
            return false;
        }

        private ModifierType GetModifierTypeByString(string modifierStr)
        {
            switch (modifierStr)
            {
                case "required": return ModifierType.Required;
                case "optional": return ModifierType.Optional;
                case "repeated": return ModifierType.Repeated;
                default: return ModifierType.NULL;
            }
        }

        private VariableType GetVariableTypeByString(string variableTypeStr)
        {
            switch (variableTypeStr)
            {
                case "bool": return VariableType.Bool;
                case "double": return VariableType.Double;
                case "float": return VariableType.Float;
                case "int32": return VariableType.Int32;
                case "uint32": return VariableType.UInt32;
                case "int64": return VariableType.Int64;
                case "uint64": return VariableType.UInt64;
                case "sint32": return VariableType.SInt32;
                case "sint64": return VariableType.SInt64;
                case "fixed32": return VariableType.Fixed32;
                case "fixed64": return VariableType.Fixed64;
                case "sfixed32": return VariableType.SFixed32;
                case "sfixed64": return VariableType.SFixed64;
                case "string": return VariableType.String;
                case "bytes": return VariableType.Bytes;
                default: return VariableType.Others;
            }
        }

        public override string ToString()
        {
            if (defaultValue != null)
            {
                return string.Format("{0} {1} {2} = {3} default:{4}", modifier.ToString(), variableType.ToString(), variableName, variableIndex.ToString(), defaultValue);
            }
            else
            {
                return string.Format("{0} {1} {2} = {3}", modifier.ToString(), variableType.ToString(), variableName, variableIndex.ToString());
            }
        }
    }

    /// <summary>
    /// 消息中的枚举类型
    /// </summary>
    public class EnumType
    {
        public readonly static string pattern = @"enum([^}]+)}";

        private readonly static string singleEnumOptionPattern = @"\b\w+\s*=\s*\d+\s*;";
        private readonly static string singleEnumNamePattern = @"enum\s*[\w..]+";
        private readonly static string singleEnumOption_GetOptionNamePattern = @"\w+";
        private readonly static string singleEnumOption_GetOptionIndexPattern = @"=\s*\d+\s*";

        private string enumName;
        public string EnumName { get { return enumName; } }

        private Dictionary<int, string> options;
        public Dictionary<int, string> Options { get { options = options ?? new Dictionary<int, string>(); return options; } }

        private string content;

        public EnumType(string enumContent)
        {
            content = enumContent;
        }

        public bool GetOption(int index, string optionName)
        {
            return Options.TryGetValue(index, out optionName);
        }

        public bool GetOption(string optionName, out int index)
        {
            foreach (var item in Options)
            {
                if (item.Value == optionName)
                {
                    index = item.Key;
                    return true;
                }
            }
            index = -1;
            return false;
        }

        public int GetDefaultOption()
        {
            int min = int.MaxValue;
            foreach (var item in Options)
            {
                min = item.Key < min ? item.Key : min;
            }
            if (min == int.MaxValue)
            {
                min = 0;
            }
            return min;
        }

        public bool Deal()
        {
            MatchCollection enumNameMatchCollection = Regex.Matches(content, singleEnumNamePattern);
            IEnumerator enumNameMatchIEnum = enumNameMatchCollection.GetEnumerator();
            while (enumNameMatchIEnum.MoveNext())
            {
                Match matchTemp = enumNameMatchIEnum.Current as Match;
                string str = matchTemp.Value.Substring(4, matchTemp.Value.Length - 4);
                enumName = str.Trim();
            }
            MatchCollection optionMatchCollection = Regex.Matches(content, singleEnumOptionPattern);
            IEnumerator optionMatchIEnum = optionMatchCollection.GetEnumerator();
            while (optionMatchIEnum.MoveNext())
            {
                Match matchTemp = optionMatchIEnum.Current as Match;
                Match optionNameMatch = Regex.Match(matchTemp.Value, singleEnumOption_GetOptionNamePattern);
                Match indexMatch = Regex.Match(matchTemp.Value, singleEnumOption_GetOptionIndexPattern);
                if (optionNameMatch.Success && indexMatch.Success)
                {
                    string optionName = optionNameMatch.Value;
                    int optionIndex;
                    if (int.TryParse(indexMatch.Value.Substring(1, indexMatch.Value.Length - 1).Trim(), out optionIndex))
                    {
                        Options[optionIndex] = optionName;
                    }
                }
            }
            content = string.Empty;
            if (string.IsNullOrEmpty(enumName) || Options.Count == 0)
            {
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            StringBuilder sbTemp = new StringBuilder();
            sbTemp.Append(enumName);
            sbTemp.Append(" =\r\n");
            sbTemp.Append("{");
            for (int i = 0; i < options.Count; i++)
            {
                sbTemp.Append("\r\n");
                sbTemp.Append("\t");
                sbTemp.Append(options[i].ToString());
            }
            foreach (var item in Options)
            {
                sbTemp.Append("\r\n");
                sbTemp.Append("\t");
                sbTemp.Append(item.Key);
                sbTemp.Append(" = ");
                sbTemp.Append(item.Value);
            }
            sbTemp.Append("\r\n}");
            return sbTemp.ToString();
        }

        /// <summary>
        /// 判断两个枚举是否相同
        /// </summary>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public bool IsEqualTo(EnumType otherType)
        {
            if (otherType == null)
            {
                return false;
            }
            if (EnumName == otherType.EnumName)
            {
                if (Options == null && otherType.Options == null)
                {
                    return true;
                }
                if (Options == null || otherType.Options == null)
                {
                    return false;
                }
                if (Options.Count != otherType.Options.Count)
                {
                    return false;
                }
                foreach (var item in Options)
                {
                    if (otherType.Options.ContainsKey(item.Key))
                    {
                        if (otherType.Options[item.Key] != item.Value)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}