using System.Collections.Generic;
using System.Xml.Serialization;

/// <summary>
/// 服务器缓存-消息
/// </summary>
public class ProtocolControlCache_ServerMessage
{
    public List<ProtocolControlCache_Message> messages = new List<ProtocolControlCache_Message>();

    public ProtocolControlCache_Message this[long index]
    {
        get
        {
            for (int i = 0; i < messages.Count; i++)
            {
                if (messages[i].messageID == index)
                {
                    return messages[i];
                }
            }
            return null;
        }
        set
        {
            for (int i = 0; i < messages.Count; i++)
            {
                if (messages[i].messageID == index)
                {
                    messages.RemoveAt(i);
                    break;
                }
            }
            messages.Add(value);
        }
    }

    /// <summary>
    /// 判断是否包含消息
    /// </summary>
    /// <param name="messageID">消息ID</param>
    /// <returns></returns>
    public bool IsMessageExist(long messageID)
    {
        for (int i = 0; i < messages.Count; i++)
        {
            if (messages[i].messageID == messageID)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 重新按照消息ID进行排序
    /// </summary>
    public void Refresh()
    {
        messages.Sort((l, r) => { return l.messageID.CompareTo(r.messageID); });
    }
}

/// <summary>
/// 描述消息的性质
/// 
/// 在此类中添加bool等字段时,需要在ServerTool_ProtocolController_ServerMessage类的RefreshMessageFromProtocolFile方法中加上保留缓存数据的操作
/// </summary>
public class ProtocolControlCache_Message
{
    /// <summary>
    /// 消息流向
    /// </summary>
    public enum MessageDirection
    {
        /// <summary>
        /// 通用,仅用于显示
        /// </summary>
        All = 0,
        /// <summary>
        /// 消息流向客户端
        /// </summary>
        ToClient = 1,
        /// <summary>
        /// 消息流向服务器
        /// </summary>
        ToServer = 2,
    }

    /// <summary>
    /// 消息是否有对应的proto文件支持
    /// </summary>
    public bool hasProto;
    /// <summary>
    /// 消息所在的xml名
    /// </summary>
    public string xmlName;
    /// <summary>
    /// 消息名
    /// </summary>
    public string messageName;
    /// <summary>
    /// 消息ID
    /// </summary>
    public long messageID;
    /// <summary>
    /// 消息描述
    /// </summary>
    public string messageDescription;
    /// <summary>
    /// 消息流向
    /// </summary>
    public MessageDirection messageDirection;
    /// <summary>
    /// 消息是否在xlua中处理/消息发送前是否需要校验
    /// </summary>
    public bool isMessageDealInXLua = true;
    /// <summary>
    /// 是否需要使用C#数据结构
    /// </summary>
    public bool isNeedSendBacktoCSharp = false;
    /// <summary>
    /// 无法被优化
    /// </summary>
    public bool unableToOptimise = false;
    /// <summary>
    /// 消息对应的proto信息
    /// </summary>
    public ProtocolControlCache_Proto protoData = new ProtocolControlCache_Proto();

    public class ProtocolControlCache_Proto
    {
        /// <summary>
        /// proto文件名
        /// </summary>
        public string protoName;
        /// <summary>
        /// proto消息名
        /// </summary>
        public string protoMsgName;
    }
}
