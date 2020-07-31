using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 网络事件
/// </summary>
public enum ESocketEvent
{
    ConnectSuccess,
    ConnectFailed,

    /// <summary>
    /// 收到更新视野消息
    /// </summary>
    ResUpdateViewMessage,
}
