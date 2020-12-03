using UnityEngine;
using UnityEditor;

/// <summary>
/// 服务器控制缓存结构
/// </summary>
public class ProtocolControlCache
{
    /// <summary>
    /// 缓存数据,以字符串-实例的形式存储
    /// </summary>
    public XMLSerializableDic<string, object> cache = new XMLSerializableDic<string, object>();
}