/***
 * Author --- ZJC
 * Description --- 
 * Function:
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 网络事件
/// </summary>
public class CSNetwork : Singleton<CSNetwork>
{
    protected EventHandlerManager ClientEventHandler = new EventHandlerManager(EventHandlerManager.DispatchType.Event);
    protected EventHandlerManager SocketEventHandler = new EventHandlerManager(EventHandlerManager.DispatchType.Socket);

    public static void SendClientEvent(EClientEvent e, params object[] param)
    {
        Instance.ClientEventHandler.SendEvent(e, param);
    }
}