using System.Collections.Generic;

/// <summary>
/// 服务器缓存-协议
/// </summary>
public class ProtocolControlCache_ServerProtocol
{
    public List<ProtocolControlCache_Protocol> protocols = new List<ProtocolControlCache_Protocol>();

    /// <summary>
    /// 根据协议名获取协议
    /// </summary>
    /// <param name="protocolName"></param>
    /// <returns></returns>
    public ProtocolControlCache_Protocol GetProtocolByProtocolName(string protocolName)
    {
        if (!string.IsNullOrEmpty(protocolName))
        {
            for (int i = 0; i < protocols.Count; i++)
            {
                if (protocols[i].protocolName == protocolName)
                {
                    return protocols[i];
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 根据XML名获取协议
    /// </summary>
    /// <param name="xmlName"></param>
    /// <returns></returns>
    public ProtocolControlCache_Protocol GetProtocolByXMLName(string xmlName)
    {
        if (!string.IsNullOrEmpty(xmlName))
        {
            for (int i = 0; i < protocols.Count; i++)
            {
                if (protocols[i].xmlName == xmlName)
                {
                    return protocols[i];
                }
            }
        }
        return null;
    }
}

/// <summary>
/// 协议
/// </summary>
public class ProtocolControlCache_Protocol
{
    /// <summary>
    /// 协议名(中文)
    /// </summary>
    public string protocolName;
    /// <summary>
    /// 协议对应的xml名
    /// </summary>
    public string xmlName;
    /// <summary>
    /// 协议在lua中使用
    /// </summary>
    public bool isProtocolUsedInLua = true;

    public ProtocolControlCache_Protocol() { }

    public ProtocolControlCache_Protocol(string protocolName, string xmlName, bool isUsedInLua)
    {
        this.protocolName = protocolName;
        this.xmlName = xmlName;
        isProtocolUsedInLua = isUsedInLua;
    }
}