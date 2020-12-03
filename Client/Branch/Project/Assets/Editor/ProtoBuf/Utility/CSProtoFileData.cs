using ExtendEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// Proto文件数据解析
/// </summary>
public class CSProtoFileData
{
    static string pattern = "message (\\S+)(\\s*){[^}]+}";

    //public string java_package;
    //public string java_outer_classname;

    //public string namespaceStr;//package task;

    public static List<CSProtoMessageData> ParseProtoFile(string file)
    {
        List<CSProtoMessageData> list = new List<CSProtoMessageData>();

        string fileContent = FileUtilityEditor.ReadToEnd(file);

        //匹配出
        //message VeinBean {
        //    required int32 newId = 1;//现在的配置表id
        //    optional int32 fightValue = 2;//战斗力
        //}

        MatchCollection matchs = null;

        matchs = Regex.Matches(fileContent, pattern);

        if (matchs != null && matchs.Count > 0)
        {
            for (int i = 0; i < matchs.Count; i++)
            {
                CSProtoMessageData data = new CSProtoMessageData(matchs[i].Groups[0].Value);
                list.Add(data);
                break;
            }
        }

        return list;
    }
}

/// <summary>
/// 一个proto文件内的一个message
/// </summary>
public class CSProtoMessageData
{
    /// 例如:
    /// message AttributeInfo {
    ///     required bytes attributeType = 1;//属性类型
    ///     optional int64 attributeValue = 2;//属性值
    ///     repeated int64 attributeValue = 2;//属性值
    /// }

    public string content;
    public string messageName;//AttributeInfo

    public CSBetterList<CSProtoMessageFieldData> fieldList = new CSBetterList<CSProtoMessageFieldData>();

    private static string pattern = "";
    private MatchCollection matchs = null;

    public CSProtoMessageData(string content)
    {
        this.content = content;

        string pattern = "message ([^{^ ])+";
        MatchCollection matchs = null;

        matchs = Regex.Matches(content, pattern);

        if (matchs != null && matchs.Count > 0)
        {
            messageName = matchs[0].Groups[1].Value;
        }

        pattern = "(required|optional|repeated) (\\S+) ([\\S^=]+)[ ]*=[ ]*([0-9]+)[^;]*;//(\\S+)";
        matchs = Regex.Matches(content, pattern);

        fieldList.Clear();
        if (matchs != null && matchs.Count > 0)
        {
            for (int i = 0; i < matchs.Count; i++)
            {
                GroupCollection group = matchs[i].Groups;
                CSProtoMessageFieldData data = new CSProtoMessageFieldData(group[1].Value, group[2].Value, group[3].Value, uint.Parse(group[4].Value), group[5].Value);
                fieldList.Add(data);
            }
        }
    }
}

/// <summary>
/// 一个proto内的一个message类中的一个字段
/// required int32 id = 1;
/// 或者 optional int32 type = 2;
/// </summary>
public class CSProtoMessageFieldData
{
    public string flag;//optional required 字段
    public string type;
    public string fieldName;
    public uint order;
    public string desc;

    public CSProtoMessageFieldData(string flag, string type, string fieldName, uint order, string desc)
    {
        this.flag = flag;
        this.type = type;
        this.fieldName = fieldName;
        this.order = order;
        this.desc = desc;
    }
}