/***
 * Author --- ZJC
 * Description --- 
 * Function:基础事件
 * 
 */
using System;
using System.Collections.Generic;

/// <summary>
/// 基础事件
/// </summary>
public class BaseEvent
{
    /// <summary>
    /// 回调绑定警告的最大值,警告回调触发一次后会将最大值置0
    /// </summary>
    public int callbackBindWarningMaxCount = 0;
    /// <summary>
    /// 回调绑定警告回调  (消息ID, 消息上回调数量)
    /// </summary>
    public Action<int, int> callbackBindWarningCallBack = null;

    public delegate void Callback(uint uiEvtID, params object[] data);

    /// <summary>
    /// 事件委托类
    /// </summary>
    public class EventDelegate
    {
        private List<Callback> arrCallBack = new List<Callback>();
        private List<Callback> arr2Process = new List<Callback>();

        private uint uiEvtID;
        /// <summary>
        /// 事件ID
        /// </summary>
        public uint EvtID { get { return uiEvtID; } }

        public EventDelegate(uint _uiEvtId)
        {
            uiEvtID = _uiEvtId;
        }

        public int GetCallBackCount()
        {
            return arrCallBack.Count;
        }

        public void AddCallBack(Callback cb)
        {
            if (!arrCallBack.Contains(cb))
            {
                arrCallBack.Add(cb);
            }
            else
            {
                //if(Debug.developerConsoleVisible)Debug.Log("AddCallBack Same");
            }
        }

        public void RemoveCallBack(Callback cb)
        {
            arrCallBack.Remove(cb);
        }

        public void ProcEvent(params object[] objData)
        {
            StartStopwatch();
            arr2Process.AddRange(arrCallBack);
            for (int i = 0; i < arr2Process.Count; i++)
            {
                Callback cb = arr2Process[i] as Callback;
                cb(uiEvtID, objData);
            }
            arr2Process.Clear();
            EndStopwatch();
        }
#if UNITY_EDITOR
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
#endif
        private void StartStopwatch()
        {
#if UNITY_EDITOR
            stopwatch.Reset();
            stopwatch.Start();
#endif
        }
        private void EndStopwatch()
        {
#if UNITY_EDITOR
            stopwatch.Stop();
            CostTime = stopwatch.ElapsedMilliseconds / 1000.0f;
#endif
        }

        private float CostTime = 0;
        public float GetStopwatchTime()
        {
            return CostTime;
        }
    }

    public Dictionary<uint, EventDelegate> mDicEvtDelegate = new Dictionary<uint, EventDelegate>();   //事件的集合


    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="uiEvtID"></param>
    /// <param name="callBack"></param>
    public void Reg(uint uiEvtID, Callback callBack)
    {
        EventDelegate evtDelegate = null;
        if (!mDicEvtDelegate.ContainsKey(uiEvtID))
        {
            evtDelegate = new EventDelegate(uiEvtID);
            mDicEvtDelegate.Add(uiEvtID, evtDelegate);
        }
        else
        {
            evtDelegate = mDicEvtDelegate[uiEvtID];
        }
        if (null != evtDelegate)
        {
            evtDelegate.AddCallBack(callBack);
            //绑定消息后,如果该消息上绑定的消息超过了绑定警告的最大值,则调用警告回调
            if (callbackBindWarningMaxCount != 0)
            {
                int count = evtDelegate.GetCallBackCount();
                if (count > callbackBindWarningMaxCount)
                {
                    callbackBindWarningMaxCount = 0;
                    if (callbackBindWarningCallBack != null)
                    {
                        callbackBindWarningCallBack((int)uiEvtID, count);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 删除事件
    /// </summary>
    /// <param name="uiEvtID"></param>
    /// <param name="callBack"></param>
    public void UnReg(uint uiEvtID, Callback callBack)
    {
        if (mDicEvtDelegate.ContainsKey(uiEvtID))
        {
            EventDelegate evtDelegate = mDicEvtDelegate[uiEvtID];
            evtDelegate.RemoveCallBack(callBack);
        }
    }

    /// <summary>
    /// 删除事件
    /// </summary>
    /// <param name="uiEvtId"></param>
    public void UnReg(uint uiEvtId)
    {
        if (mDicEvtDelegate.ContainsKey(uiEvtId))
        {
            mDicEvtDelegate.Remove(uiEvtId);
        }
    }


    /// <summary>
    /// 分发事件
    /// </summary>
    /// <param name="uiEvtID"></param>
    /// <param name="objData"></param>
    /// <returns></returns>
    public void ProcEvent(uint uiEvtID, params object[] objData)
    {
        if (mDicEvtDelegate.ContainsKey(uiEvtID))
        {
            EventDelegate evtDelegate = mDicEvtDelegate[uiEvtID];
            evtDelegate.ProcEvent(objData);
        }
    }

    public bool IsHaveEvent(uint uiEvtID)
    {
        return mDicEvtDelegate.ContainsKey(uiEvtID);
    }
}
