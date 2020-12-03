using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSMainPlayer : CSPlayer
{
    protected override void InitAvaterToGo(GameObject go)
    {
        //go.AddComponent<BehaviorDesigner.Runtime.BehaviorTree>();
    }

    public override void InitCreate()
    {
        base.InitCreate();

        Info = new CSMainPlayerInfo(this);
        Model = new CSMainPlayerModel(this);
    }
}
