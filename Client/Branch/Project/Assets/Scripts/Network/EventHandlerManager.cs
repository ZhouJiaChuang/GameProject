using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventHandlerManager
{
    void SendEvent(EClientEvent uiEvtID, params object[] objData);
    void RemoveEvent(EClientEvent uiEvitID);
}

public class EventHandlerManager : IEventHandlerManager
{
    public enum DispatchType
    {
        Event,
        Socket,
    }

    protected void Reg(uint msgId, BaseEvent.Callback cb)
    {
        for (int i = 0; i < mCBPairs.Count; ++i)
        {
            if (mCBPairs[i].ID == msgId && mCBPairs[i].CB == cb)
            {
                return;
            }
        }

        MsgCBPair pair;
        pair.ID = msgId;
        pair.CB = cb;
        mCBPairs.Add(pair);

        mDispatcher.Reg(msgId, cb);
    }

    protected void UnReg(uint msgId, BaseEvent.Callback cb)
    {
        for (int i = 0; i < mCBPairs.Count; ++i)
        {
            if (mCBPairs[i].ID == msgId && mCBPairs[i].CB == cb)
            {
                mCBPairs.RemoveAt(i);
                break;
            }
        }

        mDispatcher.UnReg(msgId, cb);
    }

    protected void UnReg(uint msgId)
    {
        for (int i = 0; i < mCBPairs.Count; i++)
        {
            if (mCBPairs[i].ID == msgId)
            {
                mCBPairs.RemoveAt(i);
            }
        }
        mDispatcher.UnReg(msgId);
    }

    public void UnRegAll(bool isCheck = true)
    {
        for (int i = 0; i < mCBPairs.Count; ++i)
        {
            mDispatcher.UnReg(mCBPairs[i].ID, mCBPairs[i].CB);
        }

        mCBPairs.Clear();
    }

    public void ProcEvent(uint uiEvtID, params object[] objData)
    {
        mDispatcher.ProcEvent(uiEvtID, objData);
    }

    public EventHandlerManager(DispatchType dt)
    {
        // mDispatcher = dt == DispatchType.Socket ? msSocketDispatcher : msEventDispatcher;
        switch (dt)
        {
            case DispatchType.Event:
                mDispatcher = msEventDispatcher;
                break;
            case DispatchType.Socket:
                mDispatcher = msServerDispatcher;
                break;
        }
        //list.Add(this);
    }

    public static void Clear()
    {
        //for (int i = 0; i < list.Count; i++)
        //{
        //    list[i].UnRegAll();
        //}
        //list.Clear();
    }

    private struct MsgCBPair
    {
        public uint ID;
        public BaseEvent.Callback CB;
    }

    private List<MsgCBPair> mCBPairs = new List<MsgCBPair>();
    public BaseEvent mDispatcher = null;
   
    private static BaseEvent msEventDispatcher = new BaseEvent();
    private static BaseEvent msServerDispatcher = new BaseEvent();


    #region 针对客户端事件
    public void AddEvent(EClientEvent uiEvtID, BaseEvent.Callback callback)
    {
        Reg((uint)uiEvtID, callback);
    }

    public void RemoveEvent(EClientEvent uiEvitID)
    {
        UnReg((uint)uiEvitID);
    }
    public void RemoveEvent(EClientEvent uiEvitID, BaseEvent.Callback cb)
    {
        UnReg((uint)uiEvitID, cb);
    }

    public void SendEvent(EClientEvent uiEvtID, params object[] objData)
    {
        ProcEvent((uint)uiEvtID, objData);
    }

    #endregion

    public void AddEvent(uint uiEvtID, BaseEvent.Callback callback)
    {
        Reg((uint)uiEvtID, callback);
    }

    public void RemoveEvent(uint uiEvitID)
    {
        UnReg((uint)uiEvitID);
    }
    public void RemoveEvent(uint uiEvitID, BaseEvent.Callback cb)
    {
        UnReg((uint)uiEvitID, cb);
    }

    public void SendEvent(uint uiEvtID, params object[] objData)
    {
        ProcEvent((uint)uiEvtID, objData);
    }

    /// <summary>
    /// 获取客户端事件绑定状态
    /// </summary>
    /// <returns></returns>
    public static string[] GetClientEventCallBackState()
    {
        List<string> list = new List<string>();
        if (msEventDispatcher != null)
        {
            List<BaseEvent.EventDelegate> eventDelegates = new List<BaseEvent.EventDelegate>(msEventDispatcher.mDicEvtDelegate.Values);
            eventDelegates.Sort((l, r) =>
            {
                return r.GetCallBackCount().CompareTo(l.GetCallBackCount());
            });
            for (int i = 0; i < eventDelegates.Count; i++)
            {
                var temp = eventDelegates[i];
                list.Add(string.Format("Client Event  ID:{0}  CallBackCount:{1}", temp.EvtID, temp.GetCallBackCount()));
            }
        }
        return list.ToArray();
    }
}