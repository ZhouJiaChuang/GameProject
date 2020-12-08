using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JCNetEventProcess : NetEventProcess
{
    protected override void ProcessEvent(int eventID, NetInfo obj)
    {
        base.ProcessEvent(eventID, obj);
        switch (eventID)
        {
            //case (int)ESocketEvent.ConnectSuccess:
            //    OnResConnectSuccess(obj);
            //    break;
        }
    }

    //proto解析格式
    //userV2.DisconnectResponse data = global::Network.Deserialize<userV2.DisconnectResponse>(obj);

    private void OnResConnectSuccess(NetInfo obj)
    {
    }

}
