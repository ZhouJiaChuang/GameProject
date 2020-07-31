using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSMainPlayer : CSAvater
{
    protected override void AddAvaterToGo(GameObject obj)
    {
        obj.AddComponent<CSMainPlayer>();
        obj.AddComponent<BehaviorDesigner.Runtime.BehaviorTree>();
    }
}
