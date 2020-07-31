/***
 * Author --- ZJC
 * Description --- 
 * Function:
 * 
 */
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskDescription("判定距离进行攻击")]
public class Action_Attack : CSActionBase
{
    public CSAvater Avater;

    public override void OnStart()
    {
        base.OnStart();
        UnityEngine.Debug.LogError(Time.time + "  触发Attack");
    }
}
