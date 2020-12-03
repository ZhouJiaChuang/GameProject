using System.Collections.Generic;
using System.Xml.Serialization;

/// <summary>
/// 用于解析协议xml的数据结构
/// </summary>
[XmlRoot("messages")]
public class ProtocolControlCache_ProtocolXMLStructure
{
    [XmlAttribute(AttributeName = "id")]
    public int id;

    [XmlAttribute(AttributeName = "queueId")]
    public int queueId;

    [XmlAttribute(AttributeName = "package")]
    public string package;

    [XmlIgnore]
    public string protoName;

    [XmlElement("message")]
    public List<Message> messageList;

    public void DealData()
    {
        if (messageList != null)
        {
            for (int i = 0; i < messageList.Count; i++)
            {
                if (messageList[i].proto != null)
                {
                    messageList[i].proto.DealData();
                }
            }
            Dictionary<string, int> dic = new Dictionary<string, int>();
            for (int i = 0; i < messageList.Count; i++)
            {
                if (messageList[i].isProto && messageList[i].proto != null)
                {
                    string protoName = messageList[i].proto.protoName;
                    if (dic.ContainsKey(protoName))
                    {
                        dic[protoName]++;
                    }
                    else
                    {
                        dic[protoName] = 1;
                    }
                }
            }
            int maxCount = 0;
            foreach (var item in dic)
            {
                if (item.Value > maxCount)
                {
                    protoName = item.Key;
                    maxCount = item.Value;
                }
            }
        }
        //if (string.IsNullOrEmpty(package) == false)
        //{
        //    string[] strs = package.Split('.');
        //    if (strs != null)
        //    {
        //        protoName = ServerTool_ProtocolControllerWnd.FormatProtoName(strs[strs.Length - 1]);
        //    }
        //}
    }

    [XmlRoot("message")]
    public class Message
    {
        [XmlAttribute(AttributeName = "id")]
        public int id;

        [XmlAttribute(AttributeName = "type")]
        public string type;

        [XmlAttribute(AttributeName = "class")]
        public string messageClass;

        [XmlAttribute(AttributeName = "proto")]
        public bool isProto;

        [XmlAttribute(AttributeName = "desc")]
        public string description;

        [XmlElement(ElementName = "proto")]
        public Proto proto;
    }

    [XmlRoot("proto")]
    public class Proto
    {
        [XmlAttribute(AttributeName = "name")]
        public string name;

        [XmlIgnore]
        public string protoName;

        [XmlIgnore]
        public string protoMsgName;

        /// <summary>
        /// 处理数据,将proto名和message分割开
        /// </summary>
        public void DealData()
        {
            if (string.IsNullOrEmpty(name) == false)
            {
                string[] strs = name.Split('.');
                if (strs != null && strs.Length >= 2)
                {
                    protoName = strs[strs.Length - 2];
                    protoMsgName = strs[strs.Length - 1];
                    protoName = protoName.ToLower();
                    if (protoName.Contains("proto"))
                    {
                        protoName = protoName.Substring(0, protoName.IndexOf("proto"));
                    }
                    protoName = ServerProtocolControllerWnd.FormatProtoName(protoName);
                }
            }
        }
    }
}