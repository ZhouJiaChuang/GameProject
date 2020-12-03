using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INetEventProcess
{
    CSNetwork Network { set; get; }

    void Init(CSNetwork network);

    void Process(int eventID, NetInfo obj);

}

public class NetEventProcess : INetEventProcess
{
    protected CSNetwork _network;
    public CSNetwork Network { set { _network = value; } get { return _network; } }

    protected EventHandlerManager mSocketEventHandler = new EventHandlerManager(EventHandlerManager.DispatchType.Socket);
    protected EventHandlerManager mClientEventHandler = new EventHandlerManager(EventHandlerManager.DispatchType.Event);

    public EventHandlerManager SocketEventHandler
    {
        get { return mSocketEventHandler; }
    }
    public EventHandlerManager ClientEventHandler
    {
        get { return mClientEventHandler; }
    }

    public void Init(CSNetwork network)
    {
        this.Network = network;
    }

    public void Process(int eventID, NetInfo obj)
    {
        if (eventID == 0)
        {
            return;
        }

        ProcessEvent(eventID, obj);
        SocketEventHandler.ProcEvent((uint)eventID, obj);
    }

    protected virtual void ProcessEvent(int eventID, NetInfo obj)
    {

    }
}

public struct NetInfo
{
    public int msgId;//id
    public object obj;
    public NetInfo(int _msgid, object _obj)
    {
        this.msgId = _msgid;
        this.obj = _obj;
    }
}